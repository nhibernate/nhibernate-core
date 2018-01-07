using System;
using System.Collections;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Test.NHSpecificTest.GH1515
{
	class PersistentLayzInitializationTestCollection : IPersistentCollection
	{

		private bool isInitialized;

		public PersistentLayzInitializationTestCollection(bool isInitialized)
		{
			this.isInitialized = isInitialized;
		}

		public bool WasInitialized { get { return isInitialized; } }

		public Task ForceInitializationAsync(CancellationToken cancellationToken)
		{
			return Task.Run(() => ForceInitialization(), cancellationToken);
		}
		public void ForceInitialization()
		{
			isInitialized = true;
		}

		public Task InitializeFromCacheAsync(
			ICollectionPersister persister,
			object disassembled,
			object owner,
			CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<object> ReadFromAsync(
			DbDataReader reader,
			ICollectionPersister role,
			ICollectionAliases descriptor,
			object owner,
			CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<bool> EqualsSnapshotAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<object> DisassembleAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<bool> NeedsInsertingAsync(object entry, int i, IType elemType, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<bool> NeedsUpdatingAsync(object entry, int i, IType elemType, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable> GetDeletesAsync(ICollectionPersister persister, bool indexIsFormula, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ICollection> GetQueuedOrphansAsync(string entityName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task PreInsertAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ICollection> GetOrphansAsync(object snapshot, string entityName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public object Owner { get; set; }
		public object GetValue()
		{
			throw new NotImplementedException();
		}

		public bool RowUpdatePossible { get; }
		public object Key { get; }
		public string Role { get; }
		public bool IsUnreferenced { get; }
		public bool IsDirty { get; }
		public object StoredSnapshot { get; }
		public bool Empty { get; }
		public void SetSnapshot(object key, string role, object snapshot)
		{
			throw new NotImplementedException();
		}

		public void PostAction()
		{
			throw new NotImplementedException();
		}

		public void BeginRead()
		{
			throw new NotImplementedException();
		}

		public bool EndRead(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public bool AfterInitialize(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public bool IsDirectlyAccessible { get; }
		public bool UnsetSession(ISessionImplementor currentSession)
		{
			throw new NotImplementedException();
		}

		public bool SetCurrentSession(ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			throw new NotImplementedException();
		}

		public IEnumerable Entries(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public object ReadFrom(DbDataReader reader, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			throw new NotImplementedException();
		}

		public object GetIdentifier(object entry, int i)
		{
			throw new NotImplementedException();
		}

		public object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public object GetElement(object entry)
		{
			throw new NotImplementedException();
		}

		public object GetSnapshotElement(object entry, int i)
		{
			throw new NotImplementedException();
		}

		public void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			throw new NotImplementedException();
		}

		public bool EqualsSnapshot(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public bool IsSnapshotEmpty(object snapshot)
		{
			throw new NotImplementedException();
		}

		public object Disassemble(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public bool NeedsRecreate(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public object GetSnapshot(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public bool IsWrapper(object collection)
		{
			throw new NotImplementedException();
		}

		public bool HasQueuedOperations { get; }
		public IEnumerable QueuedAdditionIterator { get; }
		public ICollection GetQueuedOrphans(string entityName)
		{
			throw new NotImplementedException();
		}

		public void ClearDirty()
		{
			throw new NotImplementedException();
		}

		public void Dirty()
		{
			throw new NotImplementedException();
		}

		public void PreInsert(ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public void AfterRowInsert(ICollectionPersister persister, object entry, int i, object id)
		{
			throw new NotImplementedException();
		}

		public ICollection GetOrphans(object snapshot, string entityName)
		{
			throw new NotImplementedException();
		}

		public bool EntryExists(object entry, int i)
		{
			throw new NotImplementedException();
		}

		public bool NeedsInserting(object entry, int i, IType elemType)
		{
			throw new NotImplementedException();
		}

		public bool NeedsUpdating(object entry, int i, IType elemType)
		{
			throw new NotImplementedException();
		}

		public IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			throw new NotImplementedException();
		}
	}
}
