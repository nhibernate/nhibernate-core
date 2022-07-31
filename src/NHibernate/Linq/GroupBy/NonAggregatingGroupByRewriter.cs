using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.ResultOperators;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionVisitors;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.GroupBy
{
	public static class NonAggregatingGroupByRewriter
	{
		public static void ReWrite(QueryModel queryModel)
		{
			if (queryModel.ResultOperators.Count > 0 
			    && queryModel.ResultOperators.All(r => r is GroupResultOperator)
			    && IsNonAggregatingGroupBy(queryModel))
			{
				var source = queryModel.SelectClause.Selector;
				for (var index = 0; index < queryModel.ResultOperators.Count; index++)
				{
					var r = (GroupResultOperator) queryModel.ResultOperators[index];
					queryModel.ResultOperators[index] = new NonAggregatingGroupBy(r, source);
					source = new QuerySourceReferenceExpression(r);
				}

				return;
			}

			if ((queryModel.MainFromClause.FromExpression is SubQueryExpression subQueryExpression) 
				&& (subQueryExpression.QueryModel.ResultOperators.Count == 1) 
				&& (subQueryExpression.QueryModel.ResultOperators[0] is GroupResultOperator) 
				&& (IsNonAggregatingGroupBy(queryModel)))
			{
				FlattenSubQuery(subQueryExpression, queryModel);
			}
		}

		private static void FlattenSubQuery(SubQueryExpression subQueryExpression, QueryModel queryModel)
		{
			// Create a new client-side select for the outer
			var clientSideSelect = CreateClientSideSelect(subQueryExpression, queryModel);

			var subQueryModel = subQueryExpression.QueryModel;

			// Replace the outer select clause...
			queryModel.SelectClause = subQueryModel.SelectClause;

			// Replace the outer from clause...
			queryModel.MainFromClause = subQueryModel.MainFromClause;

			foreach (var bodyClause in subQueryModel.BodyClauses)
			{
				queryModel.BodyClauses.Add(bodyClause);
			}

			// Move the result operator up 
			if (queryModel.ResultOperators.Count != 0)
			{
				throw new NotImplementedException();
			}

			queryModel.ResultOperators.Add(new NonAggregatingGroupBy((GroupResultOperator) subQueryModel.ResultOperators[0], queryModel.SelectClause.Selector));
			queryModel.ResultOperators.Add(clientSideSelect);
		}

		private static ClientSideSelect CreateClientSideSelect(Expression expression, QueryModel queryModel)
		{
			// TODO - don't like calling GetGenericArguments here...

			var parameter = Expression.Parameter(expression.Type.GetGenericArguments()[0], "inputParameter");
			
			var mapping = new QuerySourceMapping();
			mapping.AddMapping(queryModel.MainFromClause, parameter);
			
			var body = ReferenceReplacingExpressionVisitor.ReplaceClauseReferences(queryModel.SelectClause.Selector, mapping, false);
			
			var lambda = Expression.Lambda(body, parameter);
			
			return new ClientSideSelect(lambda);
		}

		private static bool IsNonAggregatingGroupBy(QueryModel queryModel)
		{
			return new IsNonAggregatingGroupByDetectionVisitor().IsNonAggregatingGroupBy(queryModel.SelectClause.Selector);
		}
	}

	public class ClientSideSelect : ClientSideTransformOperator
	{
		public LambdaExpression SelectClause { get; private set; }

		public ClientSideSelect(LambdaExpression selectClause)
		{
			SelectClause = selectClause;
		}
	}

	public class ClientSideSelect2 : ClientSideTransformOperator
	{
		public LambdaExpression SelectClause { get; private set; }

		public ClientSideSelect2(LambdaExpression selectClause)
		{
			SelectClause = selectClause;
		}
	}
}
