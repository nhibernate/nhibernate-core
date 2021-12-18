using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	internal partial class DynamicBatchingCollectionInitializer : AbstractBatchingCollectionInitializer
	{
		readonly int _maxBatchSize;
		readonly Loader _singleKeyLoader;
		readonly DynamicBatchingCollectionLoader _batchLoader;

		public DynamicBatchingCollectionInitializer(IQueryableCollection collectionPersister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(collectionPersister)
		{
			_maxBatchSize = maxBatchSize;

			_singleKeyLoader = collectionPersister.IsOneToMany
				? (Loader) new OneToManyLoader(collectionPersister, 1, factory, enabledFilters)
				: new BasicCollectionLoader(collectionPersister, 1, factory, enabledFilters);

			_batchLoader = new DynamicBatchingCollectionLoader(collectionPersister, factory, enabledFilters);
		}

		public override void Initialize(object id, ISessionImplementor session)
		{
			// first, figure out how many batchable ids we have...
			object[] batch = session.PersistenceContext.BatchFetchQueue.GetCollectionBatch(CollectionPersister, id, _maxBatchSize);
			var numberOfIds = DynamicBatchingHelper.GetIdsToLoad(batch, out var idsToLoad);
			if (numberOfIds <= 1)
			{
				_singleKeyLoader.LoadCollection(session, id, CollectionPersister.KeyType);
				return;
			}

			_batchLoader.LoadCollectionBatch(session, idsToLoad, CollectionPersister.KeyType);
		}
	}
}
