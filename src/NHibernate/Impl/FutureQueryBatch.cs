using System.Collections;

namespace NHibernate.Impl
{
    public class FutureQueryBatch : FutureBatch<IQuery, IMultiQuery>
    {
        public FutureQueryBatch(SessionImpl session) : base(session) {}

    	protected override IMultiQuery CreateMultiApproach()
    	{
			return session.CreateMultiQuery();
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
    }
}
