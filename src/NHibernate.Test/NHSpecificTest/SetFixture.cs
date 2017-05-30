using System;
using System.Collections;
using System.Data.Common;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest
{
	internal partial class CollectionPersisterStub : ICollectionPersister
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

		public IEntityPersister OwnerEntityPersister
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.OwnerEntityPersister getter implementation
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

		public IIdentifierGenerator IdentifierGenerator
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

		public IType IndexType
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

		public bool IsManyToMany
		{
			get { throw new NotImplementedException(); }
		}

		public string GetManyToManyFilterFragment(string alias, IDictionary<string, IFilter> enabledFilters)
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

		public IType KeyType
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
				// TODO:  Add CollectionPersisterStub.IsLazyProperty getter implementation
				return false;
			}
		}

		private CollectionType collectionType = new GenericSetType<int>(null, null);

		public CollectionType CollectionType
		{
			get { return collectionType; }
		}

		public void UpdateRows(IPersistentCollection collection, object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.UpdateRows implementation
		}

		public void DeleteRows(IPersistentCollection collection, object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.DeleteRows implementation
		}

		public void WriteElement(DbCommand st, object elt, bool writeOrder, ISessionImplementor session)
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

		public object ReadElement(DbDataReader rs, object owner, string[] aliases, ISessionImplementor session)
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

		public ICollectionMetadata CollectionMetadata
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.CollectionMetadata getter implementation
				return null;
			}
		}

		public bool CascadeDeleteEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public object ReadIndex(DbDataReader rs, string[] aliases, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.ReadIndex implementation
			return null;
		}

		public void Initialize(object key, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.Initialize implementation
		}

		public object ReadKey(DbDataReader rs, string[] aliases, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.ReadKey implementation
			return null;
		}

		public IType IdentifierType
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IdentifierType getter implementation
				return null;
			}
		}

		public string[] CollectionSpaces
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsArray
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IsArray getter implementation
				return false;
			}
		}

		public ICacheConcurrencyStrategy Cache
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.Cache getter implementation
				return null;
			}
		}

		public ICacheEntryStructure CacheEntryStructure
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsPrimitiveArray
		{
			get
			{
				// TODO:  Add CollectionPersisterStub.IsPrimitiveArray getter implementation
				return false;
			}
		}

		public object ReadIdentifier(DbDataReader rs, string alias, ISessionImplementor session)
		{
			// TODO:  Add CollectionPersisterStub.ReadIdentifier implementation
			return null;
		}

		public string CollectionSpace
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

		public string[] GetKeyColumnAliases(string suffix)
		{
			return null;
		}

		public string[] GetIndexColumnAliases(string suffix)
		{
			return null;
		}

		public string[] GetElementColumnAliases(string suffix)
		{
			return null;
		}

		public string GetIdentifierColumnAlias(string suffix)
		{
			return null;
		}

		public int GetSize(object key, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool IndexExists(object key, object index, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool ElementExists(object key, object element, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object GetElementByIndex(object key, object index, ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public object NotFoundObject
		{
			get { throw new NotImplementedException(); }
		}

		public ISessionFactoryImplementor Factory
		{
			get { return null; }
		}

		public bool IsExtraLazy
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsAffectedByEnabledFilters(ISessionImplementor session)
		{
			return false;
		}

		public bool HasManyToManyOrdering
		{
			get { return false; }
		}

		public bool IsVersioned
		{
			get { return false; }
		}

		public bool IsMutable
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	[TestFixture]
	public class SetFixture: TestCase
	{
		[Test]
		public void DisassembleAndAssemble()
		{
			using (ISession s = OpenSession())
			{
				ISessionImplementor si = (ISessionImplementor) s;
				var set = new PersistentGenericSet<int>(si, new HashSet<int>());

				set.Add(10);
				set.Add(20);

				CollectionPersisterStub collectionPersister = new CollectionPersisterStub();
				collectionPersister.ElementType = NHibernateUtil.Int32;

				object disassembled = set.Disassemble(collectionPersister);

				var assembledSet = new PersistentGenericSet<int>(si);
				assembledSet.InitializeFromCache(collectionPersister, disassembled, null);

				Assert.AreEqual(2, assembledSet.Count);
				Assert.IsTrue(assembledSet.Contains(10));
				Assert.IsTrue(assembledSet.Contains(20));
			}
		}

		protected override IList Mappings
		{
			get { return new List<string>(); }
		}
	}
}
