using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Engine;
using Iesi.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Declaration of a System.Type mapped with the <c>&lt;subclass&gt;</c> or 
	/// <c>&lt;joined-subclass&gt;</c> element.
	/// </summary>
	public class Subclass : PersistentClass
	{
		private PersistentClass superclass;
		private SimpleValue key;
		private System.Type classPersisterClass;
		private int subclassId;

		/// <summary>
		/// Initializes a new instance of the <see cref="Subclass"/> class.
		/// </summary>
		/// <param name="superclass">The <see cref="PersistentClass"/> that is the superclass.</param>
		public Subclass(PersistentClass superclass)
		{
			this.superclass = superclass;
			this.subclassId = NextSubclassId();
		}

		internal override int NextSubclassId()
		{
			return Superclass.NextSubclassId();
		}

		public override int SubclassId
		{
			get { return subclassId; }
		}

		/// <summary>
		/// Gets or sets the CacheConcurrencyStrategy
		/// to use to read/write instances of the persistent class to the Cache.
		/// </summary>
		/// <value>The CacheConcurrencyStrategy used with the Cache.</value>
		public override string CacheConcurrencyStrategy
		{
			get { return Superclass.CacheConcurrencyStrategy; }
			set { Superclass.CacheConcurrencyStrategy = value; }
		}

		/// <summary>
		/// Gets the <see cref="RootClass"/> of the class that is mapped in the <c>class</c> element.
		/// </summary>
		/// <value>
		/// The <see cref="RootClass"/> of the Superclass that is mapped in the <c>class</c> element.
		/// </value>
		public override RootClass RootClazz
		{
			get { return Superclass.RootClazz; }
		}

		/// <summary>
		/// Gets or sets the <see cref="PersistentClass"/> that this mapped class is extending.
		/// </summary>
		/// <value>
		/// The <see cref="PersistentClass"/> that this mapped class is extending.
		/// </value>
		public override PersistentClass Superclass
		{
			get { return superclass; }
			set { this.superclass = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override System.Type ClassPersisterClass
		{
			get
			{
				if (classPersisterClass == null)
				{
					return Superclass.ClassPersisterClass;
				}
				else
				{
					return classPersisterClass;
				}
			}
			set
			{
				classPersisterClass = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Property"/> that is used as the <c>id</c>.
		/// </summary>
		/// <value>
		/// The <see cref="Property"/> from the Superclass that is used as the <c>id</c>.
		/// </value>
		public override Property IdentifierProperty
		{
			get { return Superclass.IdentifierProperty; }
			set { Superclass.IdentifierProperty = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the identifier.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> from the Superclass that contains information about the identifier.</value>
		public override SimpleValue Identifier
		{
			get { return Superclass.Identifier; }
			set { Superclass.Identifier = value; }
		}

		/// <summary>
		/// Gets a boolean indicating if the mapped class has a Property for the <c>id</c>.
		/// </summary>
		/// <value><c>true</c> if in the Superclass there is a Property for the <c>id</c>.</value>
		public override bool HasIdentifierProperty
		{
			get { return Superclass.HasIdentifierProperty; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the discriminator.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> from the Superclass that contains information about the discriminator.</value>
		public override SimpleValue Discriminator
		{
			get { return Superclass.Discriminator; }
			set { Superclass.Discriminator = value; }
		}

		/// <summary>
		/// Gets or set a boolean indicating if the mapped class has properties that can be changed.
		/// </summary>
		/// <value><c>true</c> if the Superclass is mutable.</value>
		public override bool IsMutable
		{
			get { return Superclass.IsMutable; }
			set { Superclass.IsMutable = value; }
		}

		/// <summary>
		/// Gets a boolean indicating if this mapped class is inherited from another. 
		/// </summary>
		/// <value>
		/// <c>true</c> because this is a Subclass.
		/// </value>
		public override bool IsInherited
		{
			get { return true; }
		}

		/// <summary>
		/// Gets or sets if the mapped class is a subclass.
		/// </summary>
		/// <value>
		/// <c>true</c> since this mapped class is a subclass.
		/// </value>
		/// <remarks>
		/// The setter should not be used to set the value to anything but <c>true</c>.  
		/// </remarks>
		public override bool IsPolymorphic
		{
			get { return true; }
			set
			{
				if (value != true)
				{
					throw new AssertionFailure("IsPolymorphic has to be true for subclasses.  There is a bug in NHibernate somewhere.");
				}
			}
		}

		/// <summary>
		/// Add the <see cref="Property"/> to this PersistentClass.
		/// </summary>
		/// <param name="p">The <see cref="Property"/> to add.</param>
		/// <remarks>
		/// This also adds the <see cref="Property"/> to the Superclass' collection
		/// of Subclass Properties.
		/// </remarks>
		public override void AddProperty(Property p)
		{
			base.AddProperty(p);
			Superclass.AddSubclassProperty(p);
		}

		/// <summary>
		/// Gets or Sets the <see cref="Table"/> that this class is stored in.
		/// </summary>
		/// <value>The <see cref="Table"/> this class is stored in.</value>
		/// <remarks>
		/// This also adds the <see cref="Table"/> to the Superclass' collection
		/// of Subclass Tables.
		/// </remarks>
		public override Table Table
		{
			get { return base.Table; }
			set
			{
				base.Table = value;
				Superclass.AddSubclassTable(value);
			}
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Property"/> objects that this mapped class contains.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Property"/> objects that 
		/// this mapped class contains.
		/// </value>
		/// <remarks>
		/// This is all of the properties of this mapped class and each mapped class that
		/// it is inheriting from.
		/// </remarks>
		public override ICollection PropertyClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange(PropertyCollection);
				retVal.AddRange(Superclass.PropertyClosureCollection);
				return retVal;
			}
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
		/// This is all of the tables of this mapped class and each mapped class that
		/// it is inheriting from.
		/// </remarks>
		public override ICollection TableClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange(Superclass.TableClosureCollection);
				retVal.Add(Table);
				return retVal;
			}
		}

		/// <summary>
		/// Adds a <see cref="Property"/> that is implemented by a subclass.
		/// </summary>
		/// <param name="p">The <see cref="Property"/> implemented by a subclass.</param>
		/// <remarks>
		/// This also adds the <see cref="Property"/> to the Superclass' collection
		/// of Subclass Properties.
		/// </remarks>
		public override void AddSubclassProperty(Property p)
		{
			base.AddSubclassProperty(p);
			Superclass.AddSubclassProperty(p);
		}

		/// <summary>
		/// Adds a <see cref="Table"/> that a subclass is stored in.
		/// </summary>
		/// <param name="table">The <see cref="Table"/> the subclass is stored in.</param>
		/// <remarks>
		/// This also adds the <see cref="Table"/> to the Superclass' collection
		/// of Subclass Tables.
		/// </remarks>
		public override void AddSubclassTable(Table table)
		{
			base.AddSubclassTable(table);
			Superclass.AddSubclassTable(table);
		}

		/// <summary>
		/// Gets a boolean indicating if the mapped class has a version property.
		/// </summary>
		/// <value><c>true</c> if for the Superclass there is a Property for a <c>version</c>.</value>
		public override bool IsVersioned
		{
			get { return Superclass.IsVersioned; }
		}

		/// <summary>
		/// Gets or sets the <see cref="Property"/> that is used as the version.
		/// </summary>
		/// <value>The <see cref="Property"/> from the Superclass that is used as the version.</value>
		public override Property Version
		{
			get { return Superclass.Version; }
			set { Superclass.Version = value; }
		}

		/// <summary>
		/// Gets or sets a boolean indicating if the identifier is 
		/// embedded in the class.
		/// </summary>
		/// <value><c>true</c> if the Superclass has an embedded identifier.</value>
		/// <remarks>
		/// An embedded identifier is true when using a <c>composite-id</c> specifying
		/// properties of the class as the <c>key-property</c> instead of using a class
		/// as the <c>composite-id</c>.
		/// </remarks>
		public override bool HasEmbeddedIdentifier
		{
			get { return Superclass.HasEmbeddedIdentifier; }
			set { Superclass.HasEmbeddedIdentifier = value; }
		}

		/// <summary>
		/// Gets the <see cref="Table"/> of the class
		/// that is mapped in the <c>class</c> element.
		/// </summary>
		/// <value>
		/// The <see cref="Table"/> of the Superclass that is mapped in the <c>class</c> element.
		/// </value>
		public override Table RootTable
		{
			get { return Superclass.RootTable; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the Key.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the Key.</value>
		public override SimpleValue Key
		{
			get
			{
				if (key == null)
				{
					return Identifier;
				}
				else
				{
					return key;
				}
			}
			set { key = value; }
		}

		/// <summary>
		/// Gets or sets a boolean indicating if explicit polymorphism should be used in Queries.
		/// </summary>
		/// <value>
		/// The value of the Superclasses <c>IsExplicitPolymorphism</c> property.
		/// </value>
		public override bool IsExplicitPolymorphism
		{
			get { return Superclass.IsExplicitPolymorphism; }
			set { Superclass.IsExplicitPolymorphism = value; }
		}

		/// <summary>
		/// Gets the sql string that should be a part of the where clause.
		/// </summary>
		/// <value>
		/// The sql string that should be a part of the where clause.
		/// </value>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the setter is called.  The where clause can not be set on the 
		/// Subclass, only the RootClass.
		/// </exception>
		public override string Where
		{
			get { return Superclass.Where; }
			set { throw new InvalidOperationException("The Where string can not be set on the Subclass - use the RootClass instead."); }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsJoinedSubclass
		{
			get { return Table != RootTable; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsDiscriminatorInsertable
		{
			get { return Superclass.IsDiscriminatorInsertable; }
			set { throw new InvalidOperationException("The DiscriminatorInsertable property can not be set on the Subclass - use the Superclass instead."); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		public override void Validate(IMapping mapping)
		{
			base.Validate(mapping);
			if (Key != null && !Key.IsValid(mapping))
			{
				throw new MappingException(string.Format("subclass key has wrong number of columns: {0} type: {1}", MappedClass.Name, Key.Type.Name));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void CreateForeignKey()
		{
			if (!IsJoinedSubclass)
			{
				throw new AssertionFailure("Not a joined-subclass");
			}

			Key.CreateForeignKeyOfClass(Superclass.MappedClass);
		}

		public override int PropertyClosureSpan
		{
			get { return Superclass.PropertyClosureSpan + base.PropertyClosureSpan; }
		}

		public override ISet SynchronizedTables
		{
			get
			{
				HashedSet result = new HashedSet();
				result.AddAll(synchronizedTablesField);
				result.AddAll(Superclass.SynchronizedTables);
				return result;
			}
		}

		public override bool HasSubselectLoadableCollections
		{
			get { return base.HasSubselectLoadableCollections || Superclass.HasSubselectLoadableCollections; }
			set { base.HasSubselectLoadableCollections = value; }
		}

		public override IDictionary FilterMap
		{
			get { return Superclass.FilterMap; }
		}

		public override ICollection ReferenceablePropertyCollection
		{
			get
			{
				if (IsJoinedSubclass)
				{
					return PropertyCollection;
				}
				else
				{
					return base.ReferenceablePropertyCollection;
				}
			}
		}
	}
}
