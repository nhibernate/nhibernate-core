using System.Collections;

namespace NHibernate.Impl
{
	public class FutureCriteriaBatch : FutureBatch<ICriteria, IMultiCriteria>
	{
		public FutureCriteriaBatch(SessionImpl session) : base(session) {}

		protected override IMultiCriteria CreateMultiApproach(bool isCacheable, string cacheRegion)
		{
			return
				session.CreateMultiCriteria()
					.SetCacheable(isCacheable)
					.SetCacheRegion(cacheRegion);
		}

		protected override void AddTo(IMultiCriteria multiApproach, ICriteria query, System.Type resultType)
		{
			multiApproach.Add(resultType, query);
		}

		protected override IList GetResultsFrom(IMultiCriteria multiApproach)
		{
			return multiApproach.List();
		}

		protected override void ClearCurrentFutureBatch()
		{
			session.FutureCriteriaBatch = null;
		}

		protected override bool IsQueryCacheable(ICriteria query)
		{
			return ((CriteriaImpl)query).Cacheable;
		}

		protected override string CacheRegion(ICriteria query)
		{
			return ((CriteriaImpl)query).CacheRegion;
		}
	}
}
