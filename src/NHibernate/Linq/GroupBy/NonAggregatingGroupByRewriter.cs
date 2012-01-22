using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.GroupBy
{
	public class NonAggregatingGroupByRewriter
	{
		private NonAggregatingGroupByRewriter() { }

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
				&& (subQueryExpression.QueryModel.ResultOperators.Count() == 1) 
				&& (subQueryExpression.QueryModel.ResultOperators[0] is GroupResultOperator) 
				&& (IsNonAggregatingGroupBy(queryModel)))
			{
				var rewriter = new NonAggregatingGroupByRewriter();
				rewriter.FlattenSubQuery(subQueryExpression, queryModel);
			}
		}

		private void FlattenSubQuery(SubQueryExpression subQueryExpression, QueryModel queryModel)
		{
			// Create a new client-side select for the outer
			// TODO - don't like calling GetGenericArguments here...
			var clientSideSelect = new ClientSideSelect(
				new NonAggregatingGroupBySelectRewriter()
					.Visit(queryModel.SelectClause.Selector, subQueryExpression.Type.GetGenericArguments()[0], queryModel.MainFromClause));

			// Replace the outer select clause...
			queryModel.SelectClause = subQueryExpression.QueryModel.SelectClause;

			// Replace the outer from clause...
			queryModel.MainFromClause = subQueryExpression.QueryModel.MainFromClause;

			foreach (var bodyClause in subQueryExpression.QueryModel.BodyClauses)
			{
				queryModel.BodyClauses.Add(bodyClause);
			}

			// Move the result operator up 
			if (queryModel.ResultOperators.Count != 0)
			{
				throw new NotImplementedException();
			}

			queryModel.ResultOperators.Add(new NonAggregatingGroupBy((GroupResultOperator)subQueryExpression.QueryModel.ResultOperators[0]));
			queryModel.ResultOperators.Add(clientSideSelect);
		}

		private static bool IsNonAggregatingGroupBy(QueryModel queryModel)
		{
			return new IsNonAggregatingGroupByDetectionVisitor().IsNonAggregatingGroupBy(queryModel.SelectClause.Selector);
		}
	}

	internal class NonAggregatingGroupBySelectRewriter : NhExpressionTreeVisitor
	{
		private ParameterExpression _inputParameter;
		private IQuerySource _querySource;

		public LambdaExpression Visit(Expression clause, System.Type resultType, IQuerySource querySource)
		{
			_inputParameter = Expression.Parameter(resultType, "inputParameter");
			_querySource = querySource;

			return Expression.Lambda(VisitExpression(clause), _inputParameter);
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			if (expression.ReferencedQuerySource == _querySource)
			{
				return _inputParameter;
			}

			return expression;
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
}