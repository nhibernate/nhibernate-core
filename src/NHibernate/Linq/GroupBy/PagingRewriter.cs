using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.GroupBy
{
	public static class PagingRewriter
	{
		private static readonly System.Type[] PagingResultOperators = new[]
																		  {
																			  typeof (SkipResultOperator),
																			  typeof (TakeResultOperator),
																		  };

		public static void ReWrite(QueryModel queryModel)
		{
			var subQueryExpression = queryModel.MainFromClause.FromExpression as SubQueryExpression;

			if (subQueryExpression != null &&
				subQueryExpression.QueryModel.ResultOperators.All(x => PagingResultOperators.Contains(x.GetType())))
			{
				FlattenSubQuery(subQueryExpression, queryModel);
			}
		}

		private static void FlattenSubQuery(SubQueryExpression subQueryExpression, QueryModel queryModel)
		{
			// we can not flattern subquery if outer query has body clauses.
			var subQueryMainFromClause = subQueryExpression.QueryModel.MainFromClause;
			if (queryModel.BodyClauses.Count == 0)
			{
				foreach (var resultOperator in subQueryExpression.QueryModel.ResultOperators)
					queryModel.ResultOperators.Add(resultOperator);

				foreach (var bodyClause in subQueryExpression.QueryModel.BodyClauses)
					queryModel.BodyClauses.Add(bodyClause);
			}
			else
			{
				var cro = new ContainsResultOperator(new QuerySourceReferenceExpression(subQueryMainFromClause));

				var subQueryModel = subQueryExpression.QueryModel.Clone();
				subQueryModel.ResultOperators.Add(cro);
				subQueryModel.ResultTypeOverride = typeof (bool);

				var where = new WhereClause(new SubQueryExpression(subQueryModel));

				queryModel.BodyClauses.Add(where);
			}

			// Point all query source references to the outer from clause
			queryModel.TransformExpressions(s => new SwapQuerySourceVisitor(queryModel.MainFromClause, subQueryMainFromClause).Swap(s));

			// Replace the outer query source
			queryModel.MainFromClause = subQueryMainFromClause;
		}
	}
}
