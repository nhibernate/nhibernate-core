using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// A BatchingCollectionInitializerBuilder that builds ICollectionInitializer instances capable of dynamically building
	/// its batch-fetch SQL based on the actual number of collections keys waiting to be fetched.
	/// </summary>
	public partial class DynamicBatchingCollectionInitializerBuilder : BatchingCollectionInitializerBuilder
	{
		protected override ICollectionInitializer CreateRealBatchingCollectionInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return new DynamicBatchingCollectionInitializer(persister, maxBatchSize, factory, enabledFilters);
		}

		protected override ICollectionInitializer CreateRealBatchingOneToManyInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return new DynamicBatchingCollectionInitializer(persister, maxBatchSize, factory, enabledFilters);
		}
	}
}
