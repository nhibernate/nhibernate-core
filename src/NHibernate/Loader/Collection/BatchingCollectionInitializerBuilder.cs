using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Contract for building <seealso cref="ICollectionInitializer"/> instances capable of performing batch-fetch loading.
	/// </summary>
	public abstract class BatchingCollectionInitializerBuilder
	{
		/// <summary>
		/// Builds a batch-fetch capable ICollectionInitializer for basic and many-to-many collections (collections with
		/// a dedicated collection table).
		/// </summary>
		/// <param name="persister"> THe collection persister </param>
		/// <param name="maxBatchSize"> The maximum number of keys to batch-fetch together </param>
		/// <param name="factory"> The SessionFactory </param>
		/// <param name="enabledFilters"></param>
		/// <returns> The batch-fetch capable collection initializer </returns>
		public virtual ICollectionInitializer CreateBatchingCollectionInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize <= 1)
			{
				// no batching
				return new BasicCollectionLoader(persister, factory, enabledFilters);
			}

			return CreateRealBatchingCollectionInitializer(persister, maxBatchSize, factory, enabledFilters);
		}

		protected abstract ICollectionInitializer CreateRealBatchingCollectionInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters);

		/// <summary>
		/// Builds a batch-fetch capable ICollectionInitializer for one-to-many collections (collections without
		/// a dedicated collection table).
		/// </summary>
		/// <param name="persister"> THe collection persister </param>
		/// <param name="maxBatchSize"> The maximum number of keys to batch-fetch together </param>
		/// <param name="factory"> The SessionFactory </param>
		/// <param name="enabledFilters"></param>
		/// <returns> The batch-fetch capable collection initializer </returns>
		public virtual ICollectionInitializer CreateBatchingOneToManyInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize <= 1)
			{
				// no batching
				return new OneToManyLoader(persister, factory, enabledFilters);
			}

			return CreateRealBatchingOneToManyInitializer(persister, maxBatchSize, factory, enabledFilters);
		}

		protected abstract ICollectionInitializer CreateRealBatchingOneToManyInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters);
	}
}
