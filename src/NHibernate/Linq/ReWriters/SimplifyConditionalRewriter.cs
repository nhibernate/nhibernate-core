using NHibernate.Linq.Clauses;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.ReWriters
{
	internal class SimplifyConditionalRewriter : NhQueryModelVisitorBase
	{
		private static readonly SimplifyConditionalVisitor ConditionalVisitor = new SimplifyConditionalVisitor();
		private static readonly SimplifyConditionalRewriter Instance = new SimplifyConditionalRewriter();

		public static void Rewrite(QueryModel queryModel)
		{
			Instance.VisitQueryModel(queryModel);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			whereClause.Predicate = ConditionalVisitor.Visit(whereClause.Predicate);
		}

		public override void VisitNhHavingClause(NhHavingClause havingClause, QueryModel queryModel, int index)
		{
			havingClause.Predicate = ConditionalVisitor.Visit(havingClause.Predicate);
		}

		public override void VisitNhWithClause(NhWithClause withClause, QueryModel queryModel, int index)
		{
			withClause.Predicate = ConditionalVisitor.Visit(withClause.Predicate);
		}
	}
}
