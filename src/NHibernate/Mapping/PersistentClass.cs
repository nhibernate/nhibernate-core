using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.SqlCommand;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Base class for the <see cref="RootClass" /> mapped by <c>&lt;class&gt;</c> and a 
	/// <see cref="Subclass"/> that is mapped by <c>&lt;subclass&gt;</c> or 
	/// <c>&lt;joined-subclass&gt</c>.
	/// </summary>
	public abstract class PersistentClass
	{
		private static readonly Alias PKAlias = new Alias( 15, "PK" );

		private System.Type persistentClass;
		private string discriminatorValue;
		private ArrayList properties = new ArrayList();
		private Table table;
		private System.Type proxyInterface;
		private readonly ArrayList subclasses = new ArrayList();
		private readonly ArrayList subclassProperties = new ArrayList();
		private readonly ArrayList subclassTables = new ArrayList();
		private bool dynamicInsert;
		private bool dynamicUpdate;

		/// <summary>
		/// Gets or Sets if the Insert Sql is built dynamically.
		/// </summary>
		/// <value><c>true</c> if the Sql is built at runtime.</value>
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
		/// <value><c>true</c> if the Sql is built at runtime.</value>
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
		public virtual void AddSubclass( Subclass subclass )
		{
			subclasses.Add( subclass );
		}

		/// <summary>
		/// Gets a boolean indicating if this PersistentClass has any subclasses.
		/// </summary>
		/// <value><c>true</c> if this PeristentClass has any subclasses.</value>
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
				foreach( Subclass sc in subclasses )
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
				foreach( Subclass sc in subclasses )
				{
					retVal.AddRange( sc.SubclassCollection );
				}

				// finally add the subclasses from this PersistentClass into
				// the collection to return
				retVal.AddRange( subclasses );

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
		/// Add the <see cref="Property"/> to this PersistentClass.
		/// </summary>
		/// <param name="p">The <see cref="Property"/> to add.</param>
		public virtual void AddProperty( Property p )
		{
			properties.Add( p );
		}

		/// <summary>
		/// Gets or Sets the <see cref="Table"/> that this class is stored in.
		/// </summary>
		/// <value>The <see cref="Table"/> this class is stored in.</value>
		/// <remarks>
		/// The value of this is set by the <c>table</c> attribute. 
		/// </remarks>
		public virtual Table Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Property"/> objects.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Property"/> objects.
		/// </value>
		public virtual ICollection PropertyCollection
		{
			get { return properties; }
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that is being mapped.
		/// </summary>
		/// <value>The <see cref="System.Type"/> that is being mapped.</value>
		/// <remarks>
		/// The value of this is set by the <c>name</c> attribute on the <c>&lt;class&gt;</c> 
		/// element.
		/// </remarks>
		public virtual System.Type PersistentClazz
		{
			get { return persistentClass; }
			set { persistentClass = value; }
		}

		/// <summary>
		/// Gets the fully qualified name of the type being persisted.
		/// </summary>
		/// <value>The fully qualified name of the type being persisted.</value>
		public virtual string Name
		{
			get { return persistentClass.FullName; }
		}

		/// <summary>
		/// When implemented by a class, gets or set a boolean indicating 
		/// if the mapped class has properties that can be changed.
		/// </summary>
		/// <value><c>true</c> if the object is mutable.</value>
		/// <remarks>
		/// The value of this is set by the <c>mutable</c> attribute. 
		/// </remarks>
		public abstract bool IsMutable { get; set; }

		/// <summary>
		/// When implemented by a class, gets a boolean indicating
		/// if the mapped class has a Property for the <c>id</c>.
		/// </summary>
		/// <value><c>true</c> if there is a Property for the <c>id</c>.</value>
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
		/// When implemented by a class, gets or sets the <see cref="Value"/>
		/// that contains information about the identifier.
		/// </summary>
		/// <value>The <see cref="Value"/> that contains information about the identifier.</value>
		public abstract Value Identifier { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="Property"/>
		/// that is used as the version.
		/// </summary>
		/// <value>The <see cref="Property"/> that is used as the version.</value>
		public abstract Property Version { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="Value"/>
		/// that contains information about the discriminator.
		/// </summary>
		/// <value>The <see cref="Value"/> that contains information about the discriminator.</value>
		public abstract Value Discriminator { get; set; }

		/// <summary>
		/// When implemented by a class, gets a boolean indicating if this
		/// mapped class is inherited from another. 
		/// </summary>
		/// <value>
		/// <c>true</c> if this class is a <c>subclass</c> or <c>joined-subclass</c>
		/// that inherited from another <c>class</c>.
		/// </value>
		public abstract bool IsInherited { get; }

		/// <summary>
		/// When implemented by a class, gets or sets if the mapped class has subclasses or is
		/// a subclass.
		/// </summary>
		/// <value>
		/// <c>true</c> if the mapped class has subclasses or is a subclass.
		/// </value>
		public abstract bool IsPolymorphic { get; set; }

		/// <summary>
		/// When implemented by a class, gets a boolean indicating if the mapped class
		/// has a version property.
		/// </summary>
		/// <value><c>true</c> if there is a <c>&lt;version&gt;</c> property.</value>
		public abstract bool IsVersioned { get; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="ICacheConcurrencyStrategy"/> 
		/// to use to read/write instances of the persistent class to the Cache.
		/// </summary>
		/// <value>The <see cref="ICacheConcurrencyStrategy"/> used with the Cache.</value>
		public abstract ICacheConcurrencyStrategy Cache { get; set; }

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
		/// <c>true</c> if only classes queried on should be returned, <c>false</c>
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
		public virtual void AddSubclassProperty( Property p )
		{
			subclassProperties.Add( p );
		}

		/// <summary>
		/// Adds a <see cref="Table"/> that a subclass is stored in.
		/// </summary>
		/// <param name="table">The <see cref="Table"/> the subclass is stored in.</param>
		public virtual void AddSubclassTable( Table table )
		{
			subclassTables.Add( table );
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
				retVal.AddRange( PropertyClosureCollection );
				retVal.AddRange( subclassProperties );
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
				retVal.AddRange( TableClosureCollection );
				retVal.AddRange( subclassTables );
				return retVal;
			}
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
		/// <value><c>true</c> if the mapped discriminator values should be forced.</value>
		/// <remarks>
		/// The value of this is set by the <c>force</c> attribute on the <c>discriminator</c> element. 
		/// </remarks>
		public virtual bool IsForceDiscriminator
		{
			get { return false; }
			set { throw new NotImplementedException( "subclasses need to override this method" ); }
		}

		/// <summary>
		/// When implemented by a class, gets or sets a boolean indicating if the identifier is 
		/// embedded in the class.
		/// </summary>
		/// <value><c>true</c> if the class identifies itself.</value>
		/// <remarks>
		/// An embedded identifier is true when using a <c>composite-id</c> specifying
		/// properties of the class as the <c>key-property</c> instead of using a class
		/// as the <c>composite-id</c>.
		/// </remarks>
		public abstract bool HasEmbeddedIdentifier { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the <see cref="System.Type"/> of 
		/// the Persister.
		/// </summary>
		/// <value>The <see cref="System.Type"/> of the Persister.</value>
		/// <remarks>The value of this is set by the <c>persister</c> attribute.</remarks>
		public abstract System.Type Persister { get; set; }

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
		/// When implemented by a class, gets or sets the <see cref="Value"/>
		/// that contains information about the Key.
		/// </summary>
		/// <value>The <see cref="Value"/> that contains information about the Key.</value>
		public abstract Value Key { get; set; }

		/// <summary>
		/// Creates the <see cref="PrimaryKey"/> for the <see cref="Table"/>
		/// this type is persisted in.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> that is used to Alias columns.</param>
		public virtual void CreatePrimaryKey( Dialect.Dialect dialect )
		{
			PrimaryKey pk = new PrimaryKey();
			pk.Table = table;
			pk.Name = PKAlias.ToAliasString( table.Name, dialect );
			table.PrimaryKey = pk;

			foreach( Column col in Key.ColumnCollection )
			{
				pk.AddColumn( col );
			}
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

	}
}