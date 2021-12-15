using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	internal partial class DynamicBatchingCollectionInitializer : AbstractBatchingCollectionInitializer
	{
		readonly int maxBatchSize;
		readonly Loader singleKeyLoader;
		readonly DynamicBatchingCollectionLoader batchLoader;

		public DynamicBatchingCollectionInitializer(IQueryableCollection collectionPersister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(collectionPersister)
		{
			this.maxBatchSize = maxBatchSize;

			if (collectionPersister.IsOneToMany)
			{
				this.singleKeyLoader = new OneToManyLoader(collectionPersister, 1, factory, enabledFilters);
			}
			else
			{
				this.singleKeyLoader = new BasicCollectionLoader(collectionPersister, 1, factory, enabledFilters);
			}

			this.batchLoader = new DynamicBatchingCollectionLoader(collectionPersister, factory, enabledFilters);
		}

		public override void Initialize(object id, ISessionImplementor session)
		{
			// first, figure out how many batchable ids we have...
			object[] batch = session.PersistenceContext.BatchFetchQueue.GetCollectionBatch(CollectionPersister, id, maxBatchSize);
			var numberOfIds = DynamicBatchingHelper.GetIdsToLoad(batch, out var idsToLoad);
			if (numberOfIds <= 1)
			{
				singleKeyLoader.LoadCollection(session, id, CollectionPersister.KeyType);
				return;
			}

			batchLoader.LoadCollectionBatch(session, idsToLoad, CollectionPersister.KeyType);
		}
	}
}
