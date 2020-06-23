using NHibernate.Linq.Clauses;
using Remotion.Linq;

namespace NHibernate.Linq
{
	public interface INhQueryModelVisitor: IQueryModelVisitor
	{
		void VisitNhJoinClause(NhJoinClause nhJoinClause, QueryModel queryModel, int index);

		void VisitNhWithClause(NhWithClause nhWhereClause, QueryModel queryModel, int index);

		void VisitNhHavingClause(NhHavingClause nhWhereClause, QueryModel queryModel, int index);
	}

	// TODO 6.0: Move members into INhQueryModelVisitor 
	internal interface INhQueryModelVisitorExtended : INhQueryModelVisitor
	{
		void VisitNhOuterJoinClause(NhOuterJoinClause nhOuterJoinClause, QueryModel queryModel, int index);
	}
}
