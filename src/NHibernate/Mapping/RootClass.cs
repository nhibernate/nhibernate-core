using System;
using System.Collections;
using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Declaration of a System.Type mapped with the <c>&lt;class&gt;</c> element that
	/// is the root class of a table-per-subclass, or table-per-concrete-class 
	/// inheritance heirarchy.
	/// </summary>
	[Serializable]
	public class RootClass : PersistentClass, ITableOwner
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(RootClass));

		/// <summary>
		/// The default name of the column for the Identifier
		/// </summary>
		/// <value><c>id</c> is the default column name for the Identifier.</value>
		public const string DefaultIdentifierColumnName = "id";

		/// <summary>
		/// The default name of the column for the Discriminator
		/// </summary>
		/// <value><c>class</c> is the default column name for the Discriminator.</value>
		public const string DefaultDiscriminatorColumnName = "class";

		private Property identifierProperty;
		private IKeyValue identifier;
		private Property version;
		private bool polymorphic;
		private string cacheConcurrencyStrategy;
		private string cacheRegionName;
		private bool lazyPropertiesCacheable = true;
		private IValue discriminator;
		private bool mutable;
		private bool embeddedIdentifier = false;
		private bool explicitPolymorphism;
		private System.Type entityPersisterClass;
		private bool forceDiscriminator;
		private string where;
		private Table table;
		private bool discriminatorInsertable = true;
		private int nextSubclassId = 0;

		public override int SubclassId
		{
			get { return 0; }
		}

		/// <summary>
		/// Gets a boolean indicating if this mapped class is inherited from another. 
		/// </summary>
		/// <value>
		/// <see langword="false" /> because this is the root mapped class.
		/// </value>
		public override bool IsInherited
		{
			get { return false; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Property"/> objects that this mapped class contains.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Property"/> objects that 
		/// this mapped class contains.
		/// </value>
		public override IEnumerable<Property> PropertyClosureIterator
		{
			get { return PropertyIterator; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Table"/> objects that this 
		/// mapped class reads from and writes to.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Table"/> objects that 
		/// this mapped class reads from and writes to.
		/// </value>
		/// <remarks>
		/// There is only one <see cref="Table"/> in the <see cref="ICollection"/> since
		/// this is the root class.
		/// </remarks>
		public override IEnumerable<Table> TableClosureIterator
		{
			get { return new SingletonEnumerable<Table>(Table); }
		}

		public override IEnumerable<IKeyValue> KeyClosureIterator
		{
			get { return new SingletonEnumerable<IKeyValue>(Key); }
		}

		/// <summary>
		/// Gets a boolean indicating if the mapped class has a version property.
		/// </summary>
		/// <value><see langword="true" /> if there is a Property for a <c>version</c>.</value>
		public override bool IsVersioned
		{
			get { return version != null; }
		}

		public override System.Type EntityPersisterClass
		{
			get { return entityPersisterClass; }
			set { entityPersisterClass = value; }
		}

		/// <summary>
		/// Gets the <see cref="Table"/> of the class
		/// that is mapped in the <c>class</c> element.
		/// </summary>
		/// <value>
		/// The <see cref="Table"/> of the class this mapped class.
		/// </value>
		public override Table RootTable
		{
			get { return Table; }
		}

		/// <summary>
		/// Gets or sets a boolean indicating if the identifier is 
		/// embedded in the class.
		/// </summary>
		/// <value><see langword="true" /> if the class identifies itself.</value>
		/// <remarks>
		/// An embedded identifier is true when using a <c>composite-id</c> specifying
		/// properties of the class as the <c>key-property</c> instead of using a class
		/// as the <c>composite-id</c>.
		/// </remarks>
		public override bool HasEmbeddedIdentifier
		{
			get { return embeddedIdentifier; }
			set { embeddedIdentifier = value; }
		}

		/// <summary>
		/// Gets or sets the cache region name.
		/// </summary>
		/// <value>The region name used with the Cache.</value>
		public string CacheRegionName
		{
			get { return cacheRegionName ?? EntityName; }
			set { cacheRegionName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsJoinedSubclass
		{
			get { return false; }
		}

		public virtual ISet<Table> IdentityTables
		{
			get
			{
				ISet<Table> tables = new HashSet<Table>();
				foreach (PersistentClass clazz in SubclassClosureIterator)
				{
					if (!clazz.IsAbstract.GetValueOrDefault())
						tables.Add(clazz.IdentityTable);
				}
				return tables;
			}

		}

		internal override int NextSubclassId()
		{
			return ++nextSubclassId;
		}

		public override Table Table
		{
			get { return table; }
		}

		Table ITableOwner.Table
		{
			set { table = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="Property"/> that is used as the <c>id</c>.
		/// </summary>
		/// <value>
		/// The <see cref="Property"/> that is used as the <c>id</c>.
		/// </value>
		public override Property IdentifierProperty
		{
			get { return identifierProperty; }
			set
			{
				identifierProperty = value;
				identifierProperty.PersistentClass = this;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the identifier.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the identifier.</value>
		public override IKeyValue Identifier
		{
			get { return identifier; }
			set { identifier = value; }
		}

		/// <summary>
		/// Gets a boolean indicating if the mapped class has a Property for the <c>id</c>.
		/// </summary>
		/// <value><see langword="true" /> if there is a Property for the <c>id</c>.</value>
		public override bool HasIdentifierProperty
		{
			get { return identifierProperty != null; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the discriminator.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the discriminator.</value>
		public override IValue Discriminator
		{
			get { return discriminator; }
			set { discriminator = value; }
		}

		/// <summary>
		/// Gets or sets if the mapped class has subclasses.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the mapped class has subclasses.
		/// </value>
		public override bool IsPolymorphic
		{
			get { return polymorphic; }
			set { polymorphic = value; }
		}

		/// <summary>
		/// Gets the <see cref="RootClazz"/> of the class that is mapped in the <c>class</c> element.
		/// </summary>
		/// <value>
		/// <c>this</c> since this is the root mapped class.
		/// </value>
		public override RootClass RootClazz
		{
			get { return this; }
		}

		/// <summary>
		/// Adds a <see cref="Subclass"/> to the class hierarchy.
		/// </summary>
		/// <param name="subclass">The <see cref="Subclass"/> to add to the hierarchy.</param>
		/// <remarks>
		/// When a <see cref="Subclass"/> is added this mapped class has the property <see cref="IsPolymorphic"/>
		/// set to <see langword="true" />.
		/// </remarks>
		public override void AddSubclass(Subclass subclass)
		{
			base.AddSubclass(subclass);
			polymorphic = true;
		}

		/// <summary>
		/// Gets or sets a boolean indicating if explicit polymorphism should be used in Queries.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if only classes queried on should be returned, <see langword="false" />
		/// if any class in the hierarchy should implicitly be returned.
		/// </value>
		public override bool IsExplicitPolymorphism
		{
			get { return explicitPolymorphism; }
			set { explicitPolymorphism = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="Property"/> that is used as the version.
		/// </summary>
		/// <value>The <see cref="Property"/> that is used as the version.</value>
		public override Property Version
		{
			get { return version; }
			set { version = value; }
		}

		/// <summary>
		/// Gets or set a boolean indicating if the mapped class has properties that can be changed.
		/// </summary>
		/// <value><see langword="true" /> if the object is mutable.</value>
		public override bool IsMutable
		{
			get { return mutable; }
			set { mutable = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="PersistentClass"/> that this mapped class is extending.
		/// </summary>
		/// <value>
		/// <see langword="null" /> since this is the root class.
		/// </value>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the setter is called.  The Superclass can not be set on the 
		/// RootClass, only the SubclassType can have a Superclass set.
		/// </exception>
		public override PersistentClass Superclass
		{
			get { return null; }
			set { throw new InvalidOperationException("Can not set the Superclass on a RootClass."); }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the Key.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the Key.</value>
		public override IKeyValue Key
		{
			get { return Identifier; }
			set { throw new InvalidOperationException(); }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsDiscriminatorInsertable
		{
			get { return discriminatorInsertable; }
			set { discriminatorInsertable = value; }
		}

		/// <summary>
		/// Gets or sets a boolean indicating if only values in the discriminator column that
		/// are mapped will be included in the sql.
		/// </summary>
		/// <value><see langword="true" /> if the mapped discriminator values should be forced.</value>
		public override bool IsForceDiscriminator
		{
			get { return forceDiscriminator; }
			set { forceDiscriminator = value; }
		}

		/// <summary>
		/// Gets or sets the sql string that should be a part of the where clause.
		/// </summary>
		/// <value>
		/// The sql string that should be a part of the where clause.
		/// </value>
		public override string Where
		{
			get { return where; }
			set { where = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		public override void Validate(IMapping mapping)
		{
			base.Validate(mapping);
			if (!Identifier.IsValid(mapping))
			{
				throw new MappingException(
					string.Format("identifier mapping has wrong number of columns: {0} type: {1}", EntityName,
								  Identifier.Type.Name));
			}
			CheckCompositeIdentifier();
		}

		private void CheckCompositeIdentifier()
		{
			Component id = Identifier as Component;
			if (id!=null)
			{
				if (!id.IsDynamic)
				{
					System.Type idClass = id.ComponentClass;
					if (idClass != null && !ReflectHelper.OverridesEquals(idClass))
					{
						log.Warn("composite-id class does not override Equals(): " + id.ComponentClass.FullName);
					}
					if (!ReflectHelper.OverridesGetHashCode(idClass))
					{
						log.Warn("composite-id class does not override GetHashCode(): " + id.ComponentClass.FullName);
					}
					// NH: Not ported
					//if (!typeof(System.Runtime.Serialization.ISerializable).IsAssignableFrom(idClass))
					//{
					//  throw new MappingException("composite-id class must implement Serializable: " + id.ComponentClass.FullName);
					//}
				}
			}
		}

		/// <summary>
		/// Gets or sets the CacheConcurrencyStrategy
		/// to use to read/write instances of the persistent class to the Cache.
		/// </summary>
		/// <value>The CacheConcurrencyStrategy used with the Cache.</value>
		public override string CacheConcurrencyStrategy
		{
			get { return cacheConcurrencyStrategy; }
			set { cacheConcurrencyStrategy = value; }
		}

		public override bool IsLazyPropertiesCacheable
		{
			get { return lazyPropertiesCacheable; }
		}

		public void SetLazyPropertiesCacheable(bool isLazyPropertiesCacheable)
		{
			lazyPropertiesCacheable = isLazyPropertiesCacheable;
		}

		public override object Accept(IPersistentClassVisitor mv)
		{
			return mv.Accept(this);
		}
	}
}
