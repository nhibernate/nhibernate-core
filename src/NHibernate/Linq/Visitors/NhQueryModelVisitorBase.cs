using NHibernate.Linq.Clauses;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors
{
	public class NhQueryModelVisitorBase : QueryModelVisitorBase, INhQueryModelVisitor, AsQueryableResultOperator.ISupportedByIQueryModelVistor
	{
		public virtual void VisitNhHavingClause(NhHavingClause havingClause, QueryModel queryModel, int index)
		{
		}

		public virtual void VisitNhJoinClause(NhJoinClause joinClause, QueryModel queryModel, int index)
		{
		}

		public virtual void VisitNhWithClause(NhWithClause nhWhereClause, QueryModel queryModel, int index)
		{
		}
	}
}
