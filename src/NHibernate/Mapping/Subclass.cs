using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Declaration of a System.Type mapped with the <c>&lt;subclass&gt;</c> or 
	/// <c>&lt;joined-subclass&gt;</c> element.
	/// </summary>
	[Serializable]
	public class Subclass : PersistentClass
	{
		private PersistentClass superclass;
		private System.Type classPersisterClass;
		private readonly int subclassId;

		/// <summary>
		/// Initializes a new instance of the <see cref="Subclass"/> class.
		/// </summary>
		/// <param name="superclass">The <see cref="PersistentClass"/> that is the superclass.</param>
		public Subclass(PersistentClass superclass)
		{
			this.superclass = superclass;
			subclassId = superclass.NextSubclassId();
		}

		public override int SubclassId
		{
			get { return subclassId; }
		}

		/// <summary>
		/// Gets a boolean indicating if this mapped class is inherited from another. 
		/// </summary>
		/// <value>
		/// <see langword="true" /> because this is a SubclassType.
		/// </value>
		public override bool IsInherited
		{
			get { return true; }
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
		public override IEnumerable<Property> PropertyClosureIterator
		{
			get { return new JoinedEnumerable<Property>(Superclass.PropertyClosureIterator, PropertyIterator); }
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
		public override IEnumerable<Table> TableClosureIterator
		{
			get { return new JoinedEnumerable<Table>(Superclass.TableClosureIterator, new SingletonEnumerable<Table>(Table)); }
		}

		public override IEnumerable<IKeyValue> KeyClosureIterator
		{
			get { return new JoinedEnumerable<IKeyValue>(Superclass.KeyClosureIterator, new SingletonEnumerable<IKeyValue>(Key)); }
		}

		/// <summary>
		/// Gets a boolean indicating if the mapped class has a version property.
		/// </summary>
		/// <value><see langword="true" /> if for the Superclass there is a Property for a <c>version</c>.</value>
		public override bool IsVersioned
		{
			get { return Superclass.IsVersioned; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override System.Type EntityPersisterClass
		{
			get
			{
				if (classPersisterClass == null)
				{
					return Superclass.EntityPersisterClass;
				}
				else
				{
					return classPersisterClass;
				}
			}
			set { classPersisterClass = value; }
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
		/// 
		/// </summary>
		public override bool IsJoinedSubclass
		{
			get { return Table != RootTable; }
		}

		public override int JoinClosureSpan
		{
			get { return Superclass.JoinClosureSpan + base.JoinClosureSpan; }
		}

		public override int PropertyClosureSpan
		{
			get { return Superclass.PropertyClosureSpan + base.PropertyClosureSpan; }
		}

		public override IEnumerable<Join> JoinClosureIterator
		{
			get { return new JoinedEnumerable<Join>(Superclass.JoinClosureIterator, base.JoinClosureIterator); }
		}

		public override ISet<string> SynchronizedTables
		{
			get
			{
				HashedSet<string> result = new HashedSet<string>();
				result.AddAll(synchronizedTables);
				result.AddAll(Superclass.SynchronizedTables);
				return result;
			}
		}

		public override IDictionary<string, string> FilterMap
		{
			get { return Superclass.FilterMap; }
		}

		public override IDictionary<EntityMode, System.Type> TuplizerMap
		{
			get
			{
				IDictionary<EntityMode, System.Type> specificTuplizerDefs = base.TuplizerMap;
				IDictionary<EntityMode, System.Type> superclassTuplizerDefs = Superclass.TuplizerMap;
				if (specificTuplizerDefs == null && superclassTuplizerDefs == null)
				{
					return null;
				}
				else
				{
					IDictionary<EntityMode, System.Type> combined = new Dictionary<EntityMode, System.Type>();
					if (superclassTuplizerDefs != null)
					{
						ArrayHelper.AddAll(combined, superclassTuplizerDefs);
					}
					if (specificTuplizerDefs != null)
					{
						ArrayHelper.AddAll(combined, specificTuplizerDefs);
					}
					return combined;
				}
			}
		}

		internal override int NextSubclassId()
		{
			return Superclass.NextSubclassId();
		}

		/// <summary>
		/// Gets or sets the CacheConcurrencyStrategy
		/// to use to read/write instances of the persistent class to the Cache.
		/// </summary>
		/// <value>The CacheConcurrencyStrategy used with the Cache.</value>
		public override string CacheConcurrencyStrategy
		{
			get { return Superclass.CacheConcurrencyStrategy; }
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets the <see cref="RootClazz"/> of the class that is mapped in the <c>class</c> element.
		/// </summary>
		/// <value>
		/// The <see cref="RootClazz"/> of the Superclass that is mapped in the <c>class</c> element.
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
			set { superclass = value; }
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
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the identifier.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> from the Superclass that contains information about the identifier.</value>
		public override IKeyValue Identifier
		{
			get { return Superclass.Identifier; }
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets a boolean indicating if the mapped class has a Property for the <c>id</c>.
		/// </summary>
		/// <value><see langword="true" /> if in the Superclass there is a Property for the <c>id</c>.</value>
		public override bool HasIdentifierProperty
		{
			get { return Superclass.HasIdentifierProperty; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the discriminator.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> from the Superclass that contains information about the discriminator.</value>
		public override IValue Discriminator
		{
			get { return Superclass.Discriminator; }
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets or set a boolean indicating if the mapped class has properties that can be changed.
		/// </summary>
		/// <value><see langword="true" /> if the Superclass is mutable.</value>
		public override bool IsMutable
		{
			get { return Superclass.IsMutable; }
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets or sets if the mapped class is a subclass.
		/// </summary>
		/// <value>
		/// <see langword="true" /> since this mapped class is a subclass.
		/// </value>
		/// <remarks>
		/// The setter should not be used to set the value to anything but <see langword="true" />.  
		/// </remarks>
		public override bool IsPolymorphic
		{
			get { return true; }
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Add the <see cref="Property"/> to this PersistentClass.
		/// </summary>
		/// <param name="p">The <see cref="Property"/> to add.</param>
		/// <remarks>
		/// This also adds the <see cref="Property"/> to the Superclass' collection
		/// of SubclassType Properties.
		/// </remarks>
		public override void AddProperty(Property p)
		{
			base.AddProperty(p);
			Superclass.AddSubclassProperty(p);
		}

		public override void AddJoin(Join join)
		{
			base.AddJoin(join);
			Superclass.AddSubclassJoin(join);
		}

		/// <summary>
		/// Adds a <see cref="Property"/> that is implemented by a subclass.
		/// </summary>
		/// <param name="p">The <see cref="Property"/> implemented by a subclass.</param>
		/// <remarks>
		/// This also adds the <see cref="Property"/> to the Superclass' collection
		/// of SubclassType Properties.
		/// </remarks>
		public override void AddSubclassProperty(Property p)
		{
			base.AddSubclassProperty(p);
			Superclass.AddSubclassProperty(p);
		}

		public override void AddSubclassJoin(Join join)
		{
			base.AddSubclassJoin(join);
			Superclass.AddSubclassJoin(join);
		}

		/// <summary>
		/// Adds a <see cref="Table"/> that a subclass is stored in.
		/// </summary>
		/// <param name="table">The <see cref="Table"/> the subclass is stored in.</param>
		/// <remarks>
		/// This also adds the <see cref="Table"/> to the Superclass' collection
		/// of SubclassType Tables.
		/// </remarks>
		public override void AddSubclassTable(Table table)
		{
			base.AddSubclassTable(table);
			Superclass.AddSubclassTable(table);
		}

		/// <summary>
		/// Gets or sets the <see cref="Property"/> that is used as the version.
		/// </summary>
		/// <value>The <see cref="Property"/> from the Superclass that is used as the version.</value>
		public override Property Version
		{
			get { return Superclass.Version; }
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets or sets a boolean indicating if the identifier is 
		/// embedded in the class.
		/// </summary>
		/// <value><see langword="true" /> if the Superclass has an embedded identifier.</value>
		/// <remarks>
		/// An embedded identifier is true when using a <c>composite-id</c> specifying
		/// properties of the class as the <c>key-property</c> instead of using a class
		/// as the <c>composite-id</c>.
		/// </remarks>
		public override bool HasEmbeddedIdentifier
		{
			get { return Superclass.HasEmbeddedIdentifier; }
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets or sets the <see cref="SimpleValue"/> that contains information about the Key.
		/// </summary>
		/// <value>The <see cref="SimpleValue"/> that contains information about the Key.</value>
		public override IKeyValue Key
		{
			get { return Superclass.Identifier; }
			set { /* Don't change the SuperClass */ }
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
			set { /* Don't change the SuperClass */ }
		}

		/// <summary>
		/// Gets the sql string that should be a part of the where clause.
		/// </summary>
		/// <value>
		/// The sql string that should be a part of the where clause.
		/// </value>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the setter is called.  The where clause can not be set on the 
		/// SubclassType, only the RootClass.
		/// </exception>
		public override string Where
		{
			get { return Superclass.Where; }
			set { throw new InvalidOperationException("The Where string can not be set on the Subclass - use the RootClass instead."); }
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

			Key.CreateForeignKeyOfEntity(Superclass.EntityName);
		}

		public override bool IsLazyPropertiesCacheable
		{
			get { return Superclass.IsLazyPropertiesCacheable; }
		}

		public override bool IsClassOrSuperclassJoin(Join join)
		{
			return base.IsClassOrSuperclassJoin(join) || Superclass.IsClassOrSuperclassJoin(join);
		}

		public override bool IsClassOrSuperclassTable(Table closureTable)
		{
			return base.IsClassOrSuperclassTable(closureTable) || Superclass.IsClassOrSuperclassTable(closureTable);
		}

		/// <summary>
		/// Gets or Sets the <see cref="Table"/> that this class is stored in.
		/// </summary>
		/// <value>The <see cref="Table"/> this class is stored in.</value>
		/// <remarks>
		/// This also adds the <see cref="Table"/> to the Superclass' collection
		/// of SubclassType Tables.
		/// </remarks>
		public override Table Table
		{
			get { return Superclass.Table; }
		}

		public override bool IsForceDiscriminator
		{
			get { return Superclass.IsForceDiscriminator; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsDiscriminatorInsertable
		{
			get { return Superclass.IsDiscriminatorInsertable; }
			set
			{
				throw new InvalidOperationException(
					"The DiscriminatorInsertable property can not be set on the Subclass - use the Superclass instead.");
			}
		}

		public override object Accept(IPersistentClassVisitor mv)
		{
			return mv.Accept(this);
		}

		public override bool HasSubselectLoadableCollections
		{
			get { return base.HasSubselectLoadableCollections || Superclass.HasSubselectLoadableCollections; }
			set { base.HasSubselectLoadableCollections = value; }
		}

		public override string GetTuplizerImplClassName(EntityMode mode)
		{
			string impl = base.GetTuplizerImplClassName(mode);
			if (impl == null)
				impl = Superclass.GetTuplizerImplClassName(mode);

			return impl;
		}

		public override Component IdentifierMapper
		{
			get { return superclass.IdentifierMapper; }
		}

		public override Versioning.OptimisticLock OptimisticLockMode
		{
			get { return superclass.OptimisticLockMode; }
		}
	}
}
