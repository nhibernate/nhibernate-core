using System.Collections;

namespace NHibernate.Impl
{
    public partial class FutureQueryBatch : FutureBatch<IQuery, IMultiQuery>
    {
        public FutureQueryBatch(SessionImpl session) : base(session) {}

    	protected override IMultiQuery CreateMultiApproach(bool isCacheable, string cacheRegion)
    	{
			return
				session.CreateMultiQuery()
					.SetCacheable(isCacheable)
					.SetCacheRegion(cacheRegion);
    	}

    	protected override void AddTo(IMultiQuery multiApproach, IQuery query, System.Type resultType)
    	{
			multiApproach.Add(resultType, query);
    	}

    	protected override IList GetResultsFrom(IMultiQuery multiApproach)
    	{
			return multiApproach.List();
    	}

    	protected override void ClearCurrentFutureBatch()
    	{
			session.FutureQueryBatch = null;
		}

		protected override bool IsQueryCacheable(IQuery query)
		{
			return ((AbstractQueryImpl)query).Cacheable;
		}

		protected override string CacheRegion(IQuery query)
		{
			return ((AbstractQueryImpl)query).CacheRegion;
		}
    }
}
