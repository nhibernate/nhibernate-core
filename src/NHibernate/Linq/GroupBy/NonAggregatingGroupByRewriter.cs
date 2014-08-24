using System;
using System.Linq.Expressions;
using NHibernate.Linq.ResultOperators;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.GroupBy
{
	public static class NonAggregatingGroupByRewriter
	{
		public static void ReWrite(QueryModel queryModel)
		{
			if (queryModel.ResultOperators.Count == 1 
				&& queryModel.ResultOperators[0] is GroupResultOperator 
				&& IsNonAggregatingGroupBy(queryModel))
			{
				var resultOperator = (GroupResultOperator)queryModel.ResultOperators[0];
				queryModel.ResultOperators.Clear();
				queryModel.ResultOperators.Add(new NonAggregatingGroupBy(resultOperator));
				return;
			}

			var subQueryExpression = queryModel.MainFromClause.FromExpression as SubQueryExpression;

			if ((subQueryExpression != null) 
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

			queryModel.ResultOperators.Add(new NonAggregatingGroupBy((GroupResultOperator) subQueryModel.ResultOperators[0]));
			queryModel.ResultOperators.Add(clientSideSelect);
		}

		private static ClientSideSelect CreateClientSideSelect(Expression expression, QueryModel queryModel)
		{
			// TODO - don't like calling GetGenericArguments here...

			var parameter = Expression.Parameter(expression.Type.GetGenericArguments()[0], "inputParameter");
			
			var mapping = new QuerySourceMapping();
			mapping.AddMapping(queryModel.MainFromClause, parameter);
			
			var body = ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(queryModel.SelectClause.Selector, mapping, false);
			
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