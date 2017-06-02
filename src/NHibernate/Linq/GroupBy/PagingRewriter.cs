using System.Linq;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.GroupBy
{
	internal static class PagingRewriter
	{
		private static readonly System.Type[] PagingResultOperators =
			new[]
			{
				typeof (SkipResultOperator),
				typeof (TakeResultOperator),
			};

		public static void ReWrite(QueryModel queryModel)
		{
			if (queryModel.MainFromClause.FromExpression is SubQueryExpression subQueryExpression &&
				subQueryExpression.QueryModel.ResultOperators.All(x => PagingResultOperators.Contains(x.GetType())))
			{
				FlattenSubQuery(subQueryExpression, queryModel);
			}
		}

		private static void FlattenSubQuery(SubQueryExpression subQueryExpression, QueryModel queryModel)
		{
			// we can not flatten subquery if outer query has body clauses.
			var subQueryModel = subQueryExpression.QueryModel;
			var subQueryMainFromClause = subQueryModel.MainFromClause;
			if (queryModel.BodyClauses.Count == 0)
			{
				foreach (var resultOperator in subQueryModel.ResultOperators)
					queryModel.ResultOperators.Add(resultOperator);

				foreach (var bodyClause in subQueryModel.BodyClauses)
					queryModel.BodyClauses.Add(bodyClause);

				var visitor1 = new PagingRewriterSelectClauseVisitor(queryModel.MainFromClause);
				queryModel.SelectClause.TransformExpressions(visitor1.Swap);
			}
			else
			{
				var cro = new ContainsResultOperator(new QuerySourceReferenceExpression(subQueryMainFromClause));

				// Cloning may cause having/join/with clauses listed in VisitorParameters to no more be matched.
				// Not a problem for now, because those clauses imply a projection, which is not supported
				// by the "new WhereClause(new SubQueryExpression(newSubQueryModel))" below. See
				// SingleKeyGroupAndCountWithHavingClausePagingAndOuterWhere test by example.
				var newSubQueryModel = subQueryModel.Clone();
				newSubQueryModel.ResultOperators.Add(cro);
				newSubQueryModel.ResultTypeOverride = typeof(bool);

				var where = new WhereClause(new SubQueryExpression(newSubQueryModel));
				queryModel.BodyClauses.Add(where);

				if (!queryModel.BodyClauses.OfType<OrderByClause>().Any())
				{
					var orderByClauses = subQueryModel.BodyClauses.OfType<OrderByClause>();
					foreach (var orderByClause in orderByClauses)
						queryModel.BodyClauses.Add(orderByClause);
				}
			}

			// Point all query source references to the outer from clause
			var visitor2 = new SwapQuerySourceVisitor(queryModel.MainFromClause, subQueryMainFromClause);
			queryModel.TransformExpressions(visitor2.Swap);

			// Replace the outer query source
			queryModel.MainFromClause = subQueryMainFromClause;
		}
	}
}
