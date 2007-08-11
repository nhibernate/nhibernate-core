using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Base class for the <see cref="RootClass" /> mapped by <c>&lt;class&gt;</c> and a 
	/// <see cref="Subclass"/> that is mapped by <c>&lt;subclass&gt;</c> or 
	/// <c>&lt;joined-subclass&gt;</c>.
	/// </summary>
	public abstract class PersistentClass : IFilterable
	{
		private static readonly Alias PKAlias = new Alias(15, "PK");

		/// <summary></summary>
		public const string NullDiscriminatorMapping = "null";

		/// <summary></summary>
		public const string NotNullDiscriminatorMapping = "not null";

		private System.Type mappedClass;
		private string discriminatorValue;
		private bool lazy;
		private ArrayList properties = new ArrayList();
		private System.Type proxyInterface;
		private readonly ArrayList subclasses = new ArrayList();
		private readonly ArrayList subclassProperties = new ArrayList();
		private readonly ArrayList subclassTables = new ArrayList();
		private bool dynamicInsert;
		private bool dynamicUpdate;
		private int batchSize = 1;
		private bool selectBeforeUpdate;
		private OptimisticLockMode optimisticLockMode;
		private IDictionary metaAttributes;
		private readonly ArrayList joins = new ArrayList();
		private readonly ArrayList subclassJoins = new ArrayList();
		private IDictionary filters = new Hashtable();


		private SqlString customSQLInsert;
		private bool customInsertCallable;
		private ExecuteUpdateResultCheckStyle insertCheckStyle;

		private SqlString customSQLDelete;
		private bool customDeleteCallable;
		private ExecuteUpdateResultCheckStyle deleteCheckStyle;

		private SqlString customSQLUpdate;
		private bool customUpdateCallable;
		private ExecuteUpdateResultCheckStyle updateCheckStyle;

		private string loaderName;

		protected readonly ISet synchronizedTablesField = new HashedSet();
		private bool hasSubselectLoadableCollections;

		/// <summary>
		/// Gets or Sets if the Insert Sql is built dynamically.
		/// </summary>
		/// <value><see langword="true" /> if the Sql is built at runtime.</value>
		/// <remarks>
		/// The value of this is set by the <c>dynamic-insert</c> attribute. 
		/// </remarks>
		public virtual bool DynamicInsert
		{
			get { return dynamicInsert; }
			set { dynamicInsert = value; }
		}

		/// <summary>
		/// Gets or Sets if the Update Sql is built dynamically.
		/// </summary>
		/// <value><see langword="true" /> if the Sql is built at runtime.</value>
		/// <remarks>
		/// The value of this is set by the <c>dynamic-update</c> attribute. 
		/// </remarks>
		public virtual bool DynamicUpdate
		{
			get { return dynamicUpdate; }
			set { dynamicUpdate = value; }
		}

		/// <summary>
		/// Gets or Sets the value to use as the discriminator for the Class.
		/// </summary>
		/// <value>
		/// A value that distinguishes this subclass in the database.
		/// </value>
		/// <remarks>
		/// The value of this is set by the <c>discriminator-value</c> attribute.  Each <c>&lt;subclass&gt;</c>
		/// in a heirarchy must define a unique <c>discriminator-value</c>.  The default value 
		/// is the class name if no value is supplied.
		/// </remarks>
		public virtual string DiscriminatorValue
		{
			get { return discriminatorValue; }
			set { discriminatorValue = value; }
		}

		/// <summary>
		/// Adds a <see cref="Subclass"/> to the class hierarchy.
		/// </summary>
		/// <param name="subclass">The <see cref="Subclass"/> to add to the hierarchy.</param>
		public virtual void AddSubclass(Subclass subclass)
		{
			// Inheritable cycle detection (paranoid check)
			PersistentClass superclass = Superclass;
			while (superclass != null)
			{
				if (subclass.Name == superclass.Name)
				{
					throw new MappingException(
						string.Format("Circular inheritance mapping detected: {0} will have itself as superclass when extending {1}",
						              subclass.Name, Name));
				}
				superclass = superclass.Superclass;
			}
			subclasses.Add(subclass);
		}

		/// <summary>
		/// Gets a boolean indicating if this PersistentClass has any subclasses.
		/// </summary>
		/// <value><see langword="true" /> if this PeristentClass has any subclasses.</value>
		public virtual bool HasSubclasses
		{
			get { return subclasses.Count > 0; }
		}

		/// <summary>
		/// Gets the number of subclasses that inherit either directly or indirectly.
		/// </summary>
		/// <value>The number of subclasses that inherit from this PersistentClass.</value>
		public virtual int SubclassSpan
		{
			get
			{
				int n = subclasses.Count;
				foreach (Subclass sc in subclasses)
				{
					n += sc.SubclassSpan;
				}
				return n;
			}
		}

		/// <summary>
		/// Gets the Collection of Subclasses for this PersistentClass.  
		/// </summary>
		/// <value>
		/// It will recursively go through Subclasses so that if a Subclass has Subclasses
		/// it will pick those up also.
		/// </value>
		public virtual ICollection SubclassCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();

				// check to see if there are any subclass in our subclasses 
				// and add them into the collection
				foreach (Subclass sc in subclasses)
				{
					retVal.AddRange(sc.SubclassCollection);
				}

				// finally add the subclasses from this PersistentClass into
				// the collection to return
				retVal.AddRange(subclasses);

				return retVal;
			}
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Subclass"/> objects
		/// that directly inherit from this PersistentClass.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Subclass"/> objects
		/// that directly inherit from this PersistentClass.
		/// </value>
		public virtual ICollection DirectSubclasses
		{
			get { return subclasses; }
		}

		/// <summary>
		/// Change the property definition or add a new property definition
		/// </summary>
		/// <param name="p">The <see cref="Property"/> to add.</param>
		public virtual void AddProperty(Property p)
		{
			properties.Add(p);
			p.PersistentClass = this;
		}

		/// <summary>
		/// Gets or Sets the <see cref="Table"/> that this class is stored in.
		/// </summary>
		/// <value>The <see cref="Table"/> this class is stored in.</value>
		/// <remarks>
		/// The value of this is set by the <c>table</c> attribute. 
		/// </remarks>
		public abstract Table Table { get; }

		public virtual int PropertyClosureSpan
		{
			get
			{
				int span = properties.Count;
				foreach (Join join in joins)
				{
					span += join.PropertySpan;
				}
				return span;
			}
		}

		public virtual int GetJoinNumber(Property prop)
		{
			int result = 1;
			foreach (Join join in SubclassJoinClosureCollection)
			{
				if (join.ContainsProperty(prop))
					return result;
				result++;
			}
			return 0;
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Property"/> objects.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Property"/> objects.
		/// </value>
		public virtual ICollection PropertyCollection
		{
			get
			{
				ArrayList result = new ArrayList();
				result.AddRange(properties);
				foreach (Join join in joins)
				{
					result.AddRange(join.PropertyCollection);
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that is being mapped.
		/// </summary>
		/// <value>The <see cref="System.Type"/> that is being mapped.</value>
		/// <remarks>
		/// The value of this is set by the <c>name</c> attribute on the <c>&lt;class&gt;</c> 
		/// element.
		/// </remarks>
		public virtual System.Type MappedClass
		{
			get { return mappedClass; }
			set { mappedClass = value; }
		}

		/// <summary>
		/// Gets the fully qualified name of the type being persisted.
		/// </summary>
		/// <value>The fully qualified name of the type being persisted.</value>
		public virtual string Name
		{
			get { return mappedClass.FullName; }
		}

		/// <summary>
		/// When implemented by a class, gets or set a boolean indicating 
		/// if the mapped class has properties that can be changed.
		/// </summary>
		/// <value><see langword="true" /> if the object is mutable.</value>
		/// <remarks>
		/// The value of this is set by the <c>mutable</c> attribute. 
		/// </remarks>
		public abstract bool IsMutable { get; set; }

		/// <summary>
		/// When implemented by a class, gets a boolean indicating
		/// if the mapped class has a Property for the <c>id</c>.
		/// </summary>
		/// <value><see langword="true" /> if there is a Property for the <c>id</c>.</value>
		public abstract bool HasIdentifierProperty { get; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="Property"/>
		/// that is used as the <c>id</c>.
		/// </summary>
		/// <value>
		/// The <see cref="Property"/> that is used as the <c>id</c>.
		/// </value>
		public abstract Property IdentifierProperty { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="SimpleValue"/>
		/// that contains information about the identifier.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the identifier.</value>
		public abstract SimpleValue Identifier { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="Property"/>
		/// that is used as the version.
		/// </summary>
		/// <value>The <see cref="Property"/> that is used as the version.</value>
		public abstract Property Version { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="SimpleValue"/>
		/// that contains information about the discriminator.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the discriminator.</value>
		public abstract SimpleValue Discriminator { get; set; }

		/// <summary>
		/// When implemented by a class, gets a boolean indicating if this
		/// mapped class is inherited from another. 
		/// </summary>
		/// <value>
		/// <see langword="true" /> if this class is a <c>subclass</c> or <c>joined-subclass</c>
		/// that inherited from another <c>class</c>.
		/// </value>
		public abstract bool IsInherited { get; }

		/// <summary>
		/// When implemented by a class, gets or sets if the mapped class has subclasses or is
		/// a subclass.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the mapped class has subclasses or is a subclass.
		/// </value>
		public abstract bool IsPolymorphic { get; set; }

		/// <summary>
		/// When implemented by a class, gets a boolean indicating if the mapped class
		/// has a version property.
		/// </summary>
		/// <value><see langword="true" /> if there is a <c>&lt;version&gt;</c> property.</value>
		public abstract bool IsVersioned { get; }

		/// <summary>
		/// When implemented by a class, gets or sets the CacheConcurrencyStrategy
		/// to use to read/write instances of the persistent class to the Cache.
		/// </summary>
		/// <value>The CacheConcurrencyStrategy used with the Cache.</value>
		public abstract string CacheConcurrencyStrategy { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="PersistentClass"/>
		/// that this mapped class is extending.
		/// </summary>
		/// <value>
		/// The <see cref="PersistentClass"/> that this mapped class is extending.
		/// </value>
		public abstract PersistentClass Superclass { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets a boolean indicating if 
		/// explicit polymorphism should be used in Queries.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if only classes queried on should be returned, <see langword="false" />
		/// if any class in the heirarchy should implicitly be returned.</value>
		/// <remarks>
		/// The value of this is set by the <c>polymorphism</c> attribute. 
		/// </remarks>
		public abstract bool IsExplicitPolymorphism { get; set; }

		/// <summary>
		/// When implemented by a class, gets an <see cref="ICollection"/> 
		/// of <see cref="Property"/> objects that this mapped class contains.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Property"/> objects that 
		/// this mapped class contains.
		/// </value>
		/// <remarks>
		/// This is all of the properties of this mapped class and each mapped class that
		/// it is inheriting from.
		/// </remarks>
		public abstract ICollection PropertyClosureCollection { get; }

		/// <summary>
		/// When implemented by a class, gets an <see cref="ICollection"/> 
		/// of <see cref="Table"/> objects that this mapped class reads from
		/// and writes to.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Table"/> objects that 
		/// this mapped class reads from and writes to.
		/// </value>
		/// <remarks>
		/// This is all of the tables of this mapped class and each mapped class that
		/// it is inheriting from.
		/// </remarks>
		public abstract ICollection TableClosureCollection { get; }

		/// <summary>
		/// Adds a <see cref="Property"/> that is implemented by a subclass.
		/// </summary>
		/// <param name="p">The <see cref="Property"/> implemented by a subclass.</param>
		public virtual void AddSubclassProperty(Property p)
		{
			subclassProperties.Add(p);
		}

		public virtual void AddSubclassJoin(Join join)
		{
			subclassJoins.Add(join);
		}

		/// <summary>
		/// Adds a <see cref="Table"/> that a subclass is stored in.
		/// </summary>
		/// <param name="table">The <see cref="Table"/> the subclass is stored in.</param>
		public virtual void AddSubclassTable(Table table)
		{
			subclassTables.Add(table);
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Property"/> objects that
		/// this mapped class contains and that all of its subclasses contain.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Property"/> objects that
		/// this mapped class contains and that all of its subclasses contain.
		/// </value>
		public virtual ICollection SubclassPropertyClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange(PropertyClosureCollection);
				retVal.AddRange(subclassProperties);
				foreach (Join join in subclassJoins)
				{
					retVal.AddRange(join.PropertyCollection);
				}
				return retVal;
			}
		}

		public virtual ICollection SubclassJoinClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange(JoinClosureCollection);
				retVal.AddRange(subclassJoins);
				return retVal;
			}
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of all of the <see cref="Table"/> objects that the 
		/// subclass finds its information in.  
		/// </summary>
		/// <value>An <see cref="ICollection"/> of <see cref="Table"/> objects.</value>
		/// <remarks>It adds the TableClosureCollection and the subclassTables into the ICollection.</remarks>
		public virtual ICollection SubclassTableClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange(TableClosureCollection);
				retVal.AddRange(subclassTables);
				return retVal;
			}
		}

		public virtual bool IsClassOrSuperclassJoin(Join join)
		{
			return joins.Contains(join);
		}

		public virtual bool IsClassOrSuperclassTable(Table closureTable)
		{
			return Table == closureTable;
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Type"/> to use as a Proxy.
		/// </summary>
		/// <value>The <see cref="System.Type"/> to use as a Proxy.</value>
		/// <remarks>
		/// The value of this is set by the <c>proxy</c> attribute. 
		/// </remarks>
		public virtual System.Type ProxyInterface
		{
			get { return proxyInterface; }
			set { proxyInterface = value; }
		}

		/// <summary>
		/// Gets or sets a boolean indicating if only values in the discriminator column that
		/// are mapped will be included in the sql.
		/// </summary>
		/// <value><see langword="true" /> if the mapped discriminator values should be forced.</value>
		/// <remarks>
		/// The value of this is set by the <c>force</c> attribute on the <c>discriminator</c> element. 
		/// </remarks>
		public virtual bool IsForceDiscriminator
		{
			get { return false; }
			set { throw new NotImplementedException("subclasses need to override this method"); }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsDiscriminatorValueNotNull
		{
			get { return NotNullDiscriminatorMapping.Equals(DiscriminatorValue); }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsDiscriminatorValueNull
		{
			get { return NullDiscriminatorMapping.Equals(DiscriminatorValue); }
		}

		public IDictionary MetaAttributes
		{
			get { return metaAttributes; }
			set { metaAttributes = value; }
		}

		public MetaAttribute GetMetaAttribute(string name)
		{
			return (MetaAttribute) metaAttributes[name];
		}

		public virtual ICollection JoinCollection
		{
			get { return joins; }
		}

		public virtual ICollection JoinClosureCollection
		{
			get { return joins; }
		}

		public virtual void AddJoin(Join join)
		{
			joins.Add(join);
			join.PersistentClass = this;
		}

		public virtual int JoinClosureSpan
		{
			get { return joins.Count; }
		}

		public bool IsLazy
		{
			get { return lazy; }
			set { lazy = value; }
		}

		/// <summary>
		/// When implemented by a class, gets or sets a boolean indicating if the identifier is 
		/// embedded in the class.
		/// </summary>
		/// <value><see langword="true" /> if the class identifies itself.</value>
		/// <remarks>
		/// An embedded identifier is true when using a <c>composite-id</c> specifying
		/// properties of the class as the <c>key-property</c> instead of using a class
		/// as the <c>composite-id</c>.
		/// </remarks>
		public abstract bool HasEmbeddedIdentifier { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="System.Type"/> of the Persister.
		/// </summary>
		public abstract System.Type ClassPersisterClass { get; set; }

		/// <summary>
		/// When implemented by a class, gets the <see cref="Table"/> of the class
		/// that is mapped in the <c>class</c> element.
		/// </summary>
		/// <value>
		/// The <see cref="Table"/> of the class that is mapped in the <c>class</c> element.
		/// </value>
		public abstract Table RootTable { get; }

		/// <summary>
		/// When implemented by a class, gets the <see cref="RootClass"/> of the class
		/// that is mapped in the <c>class</c> element.
		/// </summary>
		/// <value>
		/// The <see cref="RootClass"/> of the class that is mapped in the <c>class</c> element.
		/// </value>
		public abstract RootClass RootClazz { get; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="SimpleValue"/>
		/// that contains information about the Key.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the Key.</value>
		public abstract SimpleValue Key { get; set; }

		/// <summary>
		/// Creates the <see cref="PrimaryKey"/> for the <see cref="Table"/>
		/// this type is persisted in.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> that is used to Alias columns.</param>
		public virtual void CreatePrimaryKey(Dialect.Dialect dialect)
		{
			PrimaryKey pk = new PrimaryKey();
			Table table = Table;
			pk.Table = table;
			pk.Name = PKAlias.ToAliasString(table.Name, dialect);
			table.PrimaryKey = pk;

			foreach (Column col in Key.ColumnCollection)
			{
				pk.AddColumn(col);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool SelectBeforeUpdate
		{
			get { return selectBeforeUpdate; }
			set { selectBeforeUpdate = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public Property GetProperty(string propertyName)
		{
			return GetProperty(propertyName, PropertyClosureCollection);
		}

		private Property GetProperty(string propertyName, ICollection iter)
		{
			foreach (Property prop in iter)
			{
				if (prop.Name == propertyName)
				{
					return prop;
				}
			}
			throw new MappingException(string.Format("property not found: {0} on entity {1}", propertyName, Name));
		}

		public OptimisticLockMode OptimisticLockMode
		{
			get { return optimisticLockMode; }
			set { optimisticLockMode = value; }
		}

		/// <summary>
		/// When implemented by a class, gets or sets the sql string that should 
		/// be a part of the where clause.
		/// </summary>
		/// <value>
		/// The sql string that should be a part of the where clause.
		/// </value>
		/// <remarks>
		/// The value of this is set by the <c>where</c> attribute. 
		/// </remarks>
		public abstract string Where { get; set; }

		public override string ToString()
		{
			return GetType() + " for " + MappedClass;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsJoinedSubclass { get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsDiscriminatorInsertable { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		public virtual void Validate(IMapping mapping)
		{
			foreach (Property prop in PropertyCollection)
			{
				if (!prop.IsValid(mapping))
				{
					throw new MappingException(
						string.Format("property mapping has wrong number of columns: {0} type: {1}",
						              StringHelper.Qualify(MappedClass.Name, Name), prop.Type.Name));
				}
			}
		}

		public bool HasPojoRepresentation
		{
			// TODO H3:
			get { return true; }
		}

		public object IsAbstract
		{
			// TODO H3:
			get { return null; }
		}

		public Property GetRecursiveProperty(string propertyPath)
		{
			return GetRecursiveProperty(propertyPath, PropertyCollection);
		}

		private Property GetRecursiveProperty(string propertyPath, ICollection iter)
		{
			Property property = null;

			StringTokenizer st = new StringTokenizer(propertyPath, ".", false);
			try
			{
				foreach (string element in st)
				{
					if (property == null)
					{
						property = GetProperty(element, iter);
					}
					else
					{
						//flat recursive algorithm
						property = ((Component) property.Value).GetProperty(element);
					}
				}
			}
			catch (MappingException e)
			{
				throw new MappingException(
					"property not found: " + propertyPath +
					" in entity: " + Name, e
					);
			}

			return property;
		}

		public string LoaderName
		{
			get { return loaderName; }
			set { loaderName = value; }
		}

		public SqlString CustomSQLInsert
		{
			get { return customSQLInsert; }
		}

		public SqlString CustomSQLDelete
		{
			get { return customSQLDelete; }
		}

		public SqlString CustomSQLUpdate
		{
			get { return customSQLUpdate; }
		}

		public bool IsCustomInsertCallable
		{
			get { return customInsertCallable; }
		}

		public bool IsCustomDeleteCallable
		{
			get { return customDeleteCallable; }
		}

		public bool IsCustomUpdateCallable
		{
			get { return customUpdateCallable; }
		}

		public ExecuteUpdateResultCheckStyle CustomSQLInsertCheckStyle
		{
			get { return insertCheckStyle; }
		}

		public ExecuteUpdateResultCheckStyle CustomSQLDeleteCheckStyle
		{
			get { return deleteCheckStyle; }
		}

		public ExecuteUpdateResultCheckStyle CustomSQLUpdateCheckStyle
		{
			get { return updateCheckStyle; }
		}

		public void SetCustomSQLInsert(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle)
		{
			customSQLInsert = SqlString.Parse(sql);
			customInsertCallable = callable;
			insertCheckStyle = checkStyle;
		}

		public void SetCustomSQLDelete(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle)
		{
			customSQLDelete = SqlString.Parse(sql);
			customDeleteCallable = callable;
			deleteCheckStyle = checkStyle;
		}

		public void SetCustomSQLUpdate(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle)
		{
			customSQLUpdate = SqlString.Parse(sql);
			customUpdateCallable = callable;
			updateCheckStyle = checkStyle;
		}

		public abstract ISet SynchronizedTables { get; }

		public void AddSynchronizedTable(string table)
		{
			synchronizedTablesField.Add(table);
		}

		public void AddFilter(string name, string condition)
		{
			filters.Add(name, condition);
		}

		public virtual IDictionary FilterMap
		{
			get { return filters; }
		}

		public virtual bool HasSubselectLoadableCollections
		{
			get { return hasSubselectLoadableCollections; }
			set { hasSubselectLoadableCollections = value; }
		}

		internal abstract int NextSubclassId();

		public abstract int SubclassId { get; }

		/// <summary>
		/// Given a property path, locate the appropriate referenceable property reference.
		/// </summary>
		/// <remarks>
		/// A referenceable property is a property  which can be a target of a foreign-key
		/// mapping (an identifier or explicitly named in a property-ref).
		/// </remarks>
		/// <param name="propertyPath">The property path to resolve into a property reference.</param>
		/// <returns>The property reference (never null).</returns>
		/// <exception cref="MappingException">If the property could not be found.</exception>
		public Property GetReferencedProperty(string propertyPath)
		{
			try
			{
				return GetRecursiveProperty(propertyPath, ReferenceablePropertyCollection);
			}
			catch (MappingException e)
			{
				throw new MappingException(
					"property-ref [" + propertyPath + "] not found on entity [" + MappedClass + "]", e
					);
			}
		}

		/// <summary>
		/// Build a collection of properties which are "referenceable".
		/// </summary>
		/// <remarks>
		/// See <see cref="GetReferencedProperty"/> for a discussion of "referenceable".
		/// </remarks>
		public virtual ICollection ReferenceablePropertyCollection
		{
			get { return PropertyClosureCollection; }
		}

		public abstract bool IsLazyPropertiesCacheable { get;}

	}
}
