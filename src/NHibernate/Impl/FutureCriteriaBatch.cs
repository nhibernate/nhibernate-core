using System.Collections;

namespace NHibernate.Impl
{
	public class FutureCriteriaBatch : FutureBatch<ICriteria, IMultiCriteria>
	{
		public FutureCriteriaBatch(ISession session) : base(session) {}

		protected override IMultiCriteria CreateMultiApproach()
		{
			return session.CreateMultiCriteria();
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
	}
}
