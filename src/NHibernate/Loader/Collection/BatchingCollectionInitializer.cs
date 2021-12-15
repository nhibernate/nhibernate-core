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
	public partial class BatchingCollectionInitializer : AbstractBatchingCollectionInitializer
	{
		private readonly Loader[] loaders;
		private readonly int[] batchSizes;
		private readonly ICollectionPersister collectionPersister;

		//Since 5.4
		[Obsolete("Please use ctor with IQueryableCollection collectionPersister")]
		public BatchingCollectionInitializer(ICollectionPersister collectionPersister, int[] batchSizes, Loader[] loaders) : this((IQueryableCollection) collectionPersister, batchSizes, loaders)
		{
		}

		public BatchingCollectionInitializer(IQueryableCollection collectionPersister, int[] batchSizes, Loader[] loaders) : base(collectionPersister)
		{
			this.loaders = loaders;
			this.batchSizes = batchSizes;
			this.collectionPersister = collectionPersister;
		}

		public override void Initialize(object id, ISessionImplementor session)
		{
			object[] batch =
				session.PersistenceContext.BatchFetchQueue.GetCollectionBatch(collectionPersister, id, batchSizes[0]);

			for (int i = 0; i < batchSizes.Length; i++)
			{
				int smallBatchSize = batchSizes[i];
				if (batch[smallBatchSize - 1] != null)
				{
					object[] smallBatch = new object[smallBatchSize];
					Array.Copy(batch, 0, smallBatch, 0, smallBatchSize);
					loaders[i].LoadCollectionBatch(session, smallBatch, collectionPersister.KeyType);
					return; //EARLY EXIT!
				}
			}

			loaders[batchSizes.Length - 1].LoadCollection(session, id, collectionPersister.KeyType);
		}

		//Since 5.4
		[Obsolete("Please use overload with IQueryableCollection persister")]
		public static ICollectionInitializer CreateBatchingOneToManyInitializer(
			OneToManyPersister persister,
			int maxBatchSize,
			ISessionFactoryImplementor factory,
			IDictionary<string, IFilter> enabledFilters)
		{
			return CreateBatchingOneToManyInitializer((IQueryableCollection) persister, maxBatchSize, factory, enabledFilters);
		}

		public static ICollectionInitializer CreateBatchingOneToManyInitializer(IQueryableCollection persister, int maxBatchSize,
																				ISessionFactoryImplementor factory,
																				IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize > 1)
			{
				int[] batchSizesToCreate = ArrayHelper.GetBatchSizes(maxBatchSize);
				Loader[] loadersToCreate = new Loader[batchSizesToCreate.Length];
				for (int i = 0; i < batchSizesToCreate.Length; i++)
				{
					loadersToCreate[i] = new OneToManyLoader(persister, batchSizesToCreate[i], factory, enabledFilters);
				}

				return new BatchingCollectionInitializer(persister, batchSizesToCreate, loadersToCreate);
			}
			else
			{
				return new OneToManyLoader(persister, factory, enabledFilters);
			}
		}

		public static ICollectionInitializer CreateBatchingCollectionInitializer(IQueryableCollection persister,
																				 int maxBatchSize,
																				 ISessionFactoryImplementor factory,
																				 IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize > 1)
			{
				int[] batchSizesToCreate = ArrayHelper.GetBatchSizes(maxBatchSize);
				Loader[] loadersToCreate = new Loader[batchSizesToCreate.Length];
				for (int i = 0; i < batchSizesToCreate.Length; i++)
				{
					loadersToCreate[i] = new BasicCollectionLoader(persister, batchSizesToCreate[i], factory, enabledFilters);
				}
				return new BatchingCollectionInitializer(persister, batchSizesToCreate, loadersToCreate);
			}
			else
			{
				return new BasicCollectionLoader(persister, factory, enabledFilters);
			}
		}
	}
}
