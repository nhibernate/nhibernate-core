using System;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.ReWriters
{
	public class ResultOperatorRemover : NhQueryModelVisitorBase
	{
		private readonly Func<ResultOperatorBase, bool> _predicate;

		private ResultOperatorRemover(Func<ResultOperatorBase, bool> predicate)
		{
			_predicate = predicate;
		}

		public static void Remove(QueryModel queryModel, Func<ResultOperatorBase, bool> predicate)
		{
			var rewriter = new ResultOperatorRemover(predicate);

			rewriter.VisitQueryModel(queryModel);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			if (_predicate(resultOperator))
			{
				queryModel.ResultOperators.Remove(resultOperator);
				return;
			}
			base.VisitResultOperator(resultOperator, queryModel, index);
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			var subQueryExpression = fromClause.FromExpression as SubQueryExpression;
			if (subQueryExpression != null)
				VisitQueryModel(subQueryExpression.QueryModel);
			base.VisitAdditionalFromClause(fromClause, queryModel, index);
		}

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			var subQueryExpression = fromClause.FromExpression as SubQueryExpression;
			if (subQueryExpression != null)
				VisitQueryModel(subQueryExpression.QueryModel);
			base.VisitMainFromClause(fromClause, queryModel);
		}
	}
}
