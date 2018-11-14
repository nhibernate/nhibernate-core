using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// "Batch" loads collections, using multiple foreign key values in the SQL Where clause
	/// </summary>
	/// <seealso cref="BasicCollectionLoader"/>
	/// <seealso cref="OneToManyLoader"/>
	public partial class BatchingCollectionInitializer : ICollectionInitializer
	{
		private readonly Lazy<Loader>[] loaders;
		private readonly int[] batchSizes;
		private readonly ICollectionPersister collectionPersister;

		public BatchingCollectionInitializer(ICollectionPersister collectionPersister, int[] batchSizes, Loader[] loaders)
			: this(collectionPersister, batchSizes, Array.ConvertAll(loaders, loader => new Lazy<Loader>(() => loader)))
		{
		}

		public BatchingCollectionInitializer(ICollectionPersister collectionPersister, int[] batchSizes, Lazy<Loader>[] loaders)
		{
			this.loaders = loaders;
			this.batchSizes = batchSizes;
			this.collectionPersister = collectionPersister;
		}

		public void Initialize(object id, ISessionImplementor session)
		{
			object[] batch = session.PersistenceContext.BatchFetchQueue.GetCollectionBatch(collectionPersister, id, batchSizes[0]);

			for (int i = 0; i < batchSizes.Length; i++)
			{
				int smallBatchSize = batchSizes[i];
				if (batch[smallBatchSize - 1] != null)
				{
					object[] smallBatch = new object[smallBatchSize];
					Array.Copy(batch, 0, smallBatch, 0, smallBatchSize);
					loaders[i].Value.LoadCollectionBatch(session, smallBatch, collectionPersister.KeyType);
					return; //EARLY EXIT!
				}
			}

			loaders[batchSizes.Length - 1].Value.LoadCollection(session, id, collectionPersister.KeyType);
		}

		public static ICollectionInitializer CreateBatchingOneToManyInitializer(
			OneToManyPersister persister,
			int maxBatchSize,
			ISessionFactoryImplementor factory,
			IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize > 1)
			{
				var batchSizesToCreate = ArrayHelper.GetBatchSizes(maxBatchSize);
				var loadersToCreate = new Lazy<Loader>[batchSizesToCreate.Length];
				for (int i = 0; i < batchSizesToCreate.Length; i++)
				{
					var batchSize = batchSizesToCreate[i];
					loadersToCreate[i] = new Lazy<Loader>(
						() => new OneToManyLoader(persister, batchSize, factory, enabledFilters));
				}

				return new BatchingCollectionInitializer(persister, batchSizesToCreate, loadersToCreate);
			}

			return new OneToManyLoader(persister, factory, enabledFilters);
		}

		public static ICollectionInitializer CreateBatchingCollectionInitializer(
			IQueryableCollection persister,
			int maxBatchSize,
			ISessionFactoryImplementor factory,
			IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize > 1)
			{
				var batchSizesToCreate = ArrayHelper.GetBatchSizes(maxBatchSize);
				var loadersToCreate = new Lazy<Loader>[batchSizesToCreate.Length];
				for (int i = 0; i < batchSizesToCreate.Length; i++)
				{
					var batchSize = batchSizesToCreate[i];
					loadersToCreate[i] = new Lazy<Loader>(
						() => new BasicCollectionLoader(persister, batchSize, factory, enabledFilters));
				}

				return new BatchingCollectionInitializer(persister, batchSizesToCreate, loadersToCreate);
			}

			return new BasicCollectionLoader(persister, factory, enabledFilters);
		}
	}
}
