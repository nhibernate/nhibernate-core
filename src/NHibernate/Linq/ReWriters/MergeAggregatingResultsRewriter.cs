using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.ReWriters
{
	public class MergeAggregatingResultsRewriter : QueryModelVisitorBase
	{
		private MergeAggregatingResultsRewriter()
		{
		}

		public static void ReWrite(QueryModel model)
		{
			var rewriter = new MergeAggregatingResultsRewriter();

			rewriter.VisitQueryModel(model);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			if (resultOperator is SumResultOperator)
			{
				queryModel.SelectClause.Selector = new NhSumExpression(queryModel.SelectClause.Selector);
				queryModel.ResultOperators.Remove(resultOperator);
			}
			else if (resultOperator is AverageResultOperator)
			{
				queryModel.SelectClause.Selector = new NhAverageExpression(queryModel.SelectClause.Selector);
				queryModel.ResultOperators.Remove(resultOperator);
			}
			else if (resultOperator is MinResultOperator)
			{
				queryModel.SelectClause.Selector = new NhMinExpression(queryModel.SelectClause.Selector);
				queryModel.ResultOperators.Remove(resultOperator);
			}
			else if (resultOperator is MaxResultOperator)
			{
				queryModel.SelectClause.Selector = new NhMaxExpression(queryModel.SelectClause.Selector);
				queryModel.ResultOperators.Remove(resultOperator);
			}
			else if (resultOperator is DistinctResultOperator)
			{
				queryModel.SelectClause.Selector = new NhDistinctExpression(queryModel.SelectClause.Selector);
				queryModel.ResultOperators.Remove(resultOperator);
			}
			else if (resultOperator is CountResultOperator)
			{
				queryModel.SelectClause.Selector = new NhShortCountExpression(queryModel.SelectClause.Selector);
				queryModel.ResultOperators.Remove(resultOperator);
			}
            else if (resultOperator is LongCountResultOperator)
            {
                queryModel.SelectClause.Selector = new NhLongCountExpression(queryModel.SelectClause.Selector);
                queryModel.ResultOperators.Remove(resultOperator);
            }

			base.VisitResultOperator(resultOperator, queryModel, index);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			selectClause.TransformExpressions(s => new MergeAggregatingResultsInExpressionRewriter().Visit(s));
		}
	}

	internal class MergeAggregatingResultsInExpressionRewriter : NhExpressionTreeVisitor
	{
		public Expression Visit(Expression expression)
		{
			return VisitExpression(expression);
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			MergeAggregatingResultsRewriter.ReWrite(expression.QueryModel);
			return expression;
		}

		protected override Expression VisitMethodCallExpression(MethodCallExpression m)
		{
			if (m.Method.DeclaringType == typeof(Queryable) ||
			    m.Method.DeclaringType == typeof(Enumerable))
			{
				// TODO - dynamic name generation needed here
				switch (m.Method.Name)
				{
					case "Count":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
						                       e => new NhShortCountExpression(e));
					case "Min":
						return CreateAggregate(m.Arguments[0], (LambdaExpression) m.Arguments[1],
						                       e => new NhMinExpression(e));
					case "Max":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
						                       e => new NhMaxExpression(e));
					case "Sum":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
						                       e => new NhSumExpression(e));
					case "Average":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
						                       e => new NhAverageExpression(e));
				}
			}

			return base.VisitMethodCallExpression(m);
		}

		private Expression CreateAggregate(Expression fromClauseExpression, LambdaExpression body, Func<Expression,Expression> factory)
		{
			var fromClause = new MainFromClause("x2", body.Parameters[0].Type, fromClauseExpression);
			var selectClause = body.Body;
			selectClause = ReplacingExpressionTreeVisitor.Replace(body.Parameters[0],
			                                                      new QuerySourceReferenceExpression(
			                                                      	fromClause), selectClause);
			var queryModel = new QueryModel(fromClause,
			                                new SelectClause(factory(selectClause)));

			queryModel.ResultOperators.Add(new AverageResultOperator());

			var subQuery = new SubQueryExpression(queryModel);

			queryModel.ResultOperators.Clear();

			return subQuery;
		}
	}
}