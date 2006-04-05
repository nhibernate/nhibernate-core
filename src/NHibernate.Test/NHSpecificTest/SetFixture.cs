using System;
using System.Collections;
using NHibernate.Persister.Collection;
using NUnit.Framework;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Test.NHSpecificTest
{
	class CollectionSnapshotStub : ICollectionSnapshot
	{
		#region ICollectionSnapshot Members

		public bool Dirty
		{
			get
			{
				// TODO:  Add CollectionSnapshotStub.Dirty getter implementation
				return false;
			}
		}

		public object Key
		{
			get
			{
				// TODO:  Add CollectionSnapshotStub.Key getter implementation
				return null;
			}
		}

		public string Role
		{
			get
			{
				// TODO:  Add CollectionSnapshotStub.Role getter implementation
				return null;
			}
		}

		public void SetDirty()
		{
			// TODO:  Add CollectionSnapshotStub.SetDirty implementation
		}

		public bool WasDereferenced
		{
			get
			{
				// TODO:  Add CollectionSnapshotStub.WasDereferenced getter implementation
				return false;
			}
		}

		public ICollection Snapshot
		{
			get
			{
				// TODO:  Add CollectionSnapshotStub.Snapshot getter implementation
				return null;
			}
		}

		#endregion
	}


	class CollectionPersisterStub : ICollectionPersister
	{
		#region ICollectionPersister Members

		public System.Type OwnerClass
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.OwnerClass getter implementation
				return null;
			}
		}

		public bool HasCache
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.HasCache getter implementation
				return false;
			}
		}

		public NHibernate.Id.IIdentifierGenerator IdentifierGenerator
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IdentifierGenerator getter implementation
				return null;
			}
		}

		public bool IsInverse
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IsInverse getter implementation
				return false;
			}
		}

		public NHibernate.Type.IType IndexType
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IndexType getter implementation
				return null;
			}
		}

		public bool HasIndex
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.HasIndex getter implementation
				return false;
			}
		}

		public bool IsOneToMany
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IsOneToMany getter implementation
				return false;
			}
		}

		public string GetManyToManyFilterFragment( string alias, IDictionary enabledFilters )
		{
			throw new NotImplementedException();
		}

		public System.Type ElementClass
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.ElementClass getter implementation
				return null;
			}
		}

		public NHibernate.Type.IType KeyType
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.KeyType getter implementation
				return null;
			}
		}

		public void InsertRows(IPersistentCollection collection, object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.InsertRows implementation
		}

		public bool IsLazy
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IsLazy getter implementation
				return false;
			}
		}

		public NHibernate.Type.PersistentCollectionType CollectionType
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.CollectionType getter implementation
				return null;
			}
		}

		public void UpdateRows(IPersistentCollection collection, object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.UpdateRows implementation
		}

		public void DeleteRows(IPersistentCollection collection, object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.DeleteRows implementation
		}

		public void WriteElement(System.Data.IDbCommand st, object elt, bool writeOrder, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.WriteElement implementation
		}

		public void Recreate(IPersistentCollection collection, object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.Recreate implementation
		}

		public bool HasOrdering
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.HasOrdering getter implementation
				return false;
			}
		}

		private IType elementType;
		public IType ElementType
		{
			get { return elementType; }
			set { elementType = value; }
		}

		public void Remove(object id, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.Remove implementation
		}

		public object ReadElement(System.Data.IDataReader rs, object owner, string[] aliases, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.ReadElement implementation
			return null;
		}

		public string Role
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.Role getter implementation
				return null;
			}
		}

		public NHibernate.Metadata.ICollectionMetadata CollectionMetadata
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.CollectionMetadata getter implementation
				return null;
			}
		}

		public object ReadIndex(System.Data.IDataReader rs, string[] aliases, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.ReadIndex implementation
			return null;
		}

		public void Initialize(object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.Initialize implementation
		}

		public object ReadKey(System.Data.IDataReader rs, string[] aliases, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.ReadKey implementation
			return null;
		}

		public NHibernate.Type.IType IdentifierType
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IdentifierType getter implementation
				return null;
			}
		}

		public bool IsArray
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IsArray getter implementation
				return false;
			}
		}

		public NHibernate.Cache.ICacheConcurrencyStrategy Cache
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.Cache getter implementation
				return null;
			}
		}

		public bool IsPrimitiveArray
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IsPrimitiveArray getter implementation
				return false;
			}
		}

		public object ReadIdentifier(System.Data.IDataReader rs, string alias, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.ReadIdentifier implementation
			return null;
		}

		public object CollectionSpace
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.CollectionSpace getter implementation
				return null;
			}
		}

		public bool HasOrphanDelete
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.HasOrphanDelete getter implementation
				return false;
			}
		}

		public void PostInstantiate()
		{
		}

		public string[ ] GetKeyColumnAliases( string suffix )
		{
			return null;
		}

		public string[ ] GetIndexColumnAliases( string suffix )
		{
			return null;
		}

		public string[ ] GetElementColumnAliases( string suffix )
		{
			return null;
		}

		public string GetIdentifierColumnAlias( string suffix )
		{
			return null;
		}

		#endregion
	}

	[TestFixture]
	public class SetFixture
	{
		[Test]
		public void DisassembleAndAssemble()
		{
			Set set = new Set( null, new Iesi.Collections.ListSet() );

			set.CollectionSnapshot = new CollectionSnapshotStub();

			set.Add(10);
			set.Add(20);

			CollectionPersisterStub collectionPersister = new CollectionPersisterStub();
			collectionPersister.ElementType = NHibernateUtil.Int32;

			object disassembled = set.Disassemble(collectionPersister);

			Set assembledSet = new Set( null );
			assembledSet.InitializeFromCache( collectionPersister, disassembled, null );

			Assert.AreEqual( 2, assembledSet.Count );
			Assert.IsTrue( assembledSet.Contains( 10 ) );
			Assert.IsTrue( assembledSet.Contains( 20 ) );
		}
	}
}
