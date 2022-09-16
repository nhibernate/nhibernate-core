using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	public class LegacyBatchingCollectionInitializerBuilder : BatchingCollectionInitializerBuilder
	{
		protected override ICollectionInitializer CreateRealBatchingCollectionInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return BatchingCollectionInitializer.CreateBatchingCollectionInitializer(persister, maxBatchSize, factory, enabledFilters);
		}

		protected override ICollectionInitializer CreateRealBatchingOneToManyInitializer(IQueryableCollection persister, int maxBatchSize, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return BatchingCollectionInitializer.CreateBatchingOneToManyInitializer(persister, maxBatchSize, factory, enabledFilters);
		}
	}
}
