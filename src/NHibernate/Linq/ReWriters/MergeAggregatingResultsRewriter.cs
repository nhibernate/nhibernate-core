using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

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
				queryModel.SelectClause.Selector = new NhShortCountExpression(TransformCountExpression(queryModel.SelectClause.Selector));
				queryModel.ResultOperators.Remove(resultOperator);
			}
			else if (resultOperator is LongCountResultOperator)
			{
				queryModel.SelectClause.Selector = new NhLongCountExpression(TransformCountExpression(queryModel.SelectClause.Selector));
				queryModel.ResultOperators.Remove(resultOperator);
			}

			base.VisitResultOperator(resultOperator, queryModel, index);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			selectClause.TransformExpressions(e => MergeAggregatingResultsInExpressionRewriter.Rewrite(e, new NameGenerator(queryModel)));
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			whereClause.TransformExpressions(e => MergeAggregatingResultsInExpressionRewriter.Rewrite(e, new NameGenerator(queryModel)));
		}

		public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
		{
			ordering.TransformExpressions(e => MergeAggregatingResultsInExpressionRewriter.Rewrite(e, new NameGenerator(queryModel)));
		}

		private static Expression TransformCountExpression(Expression expression)
		{
			if (expression.NodeType == ExpressionType.MemberInit || 
				expression.NodeType == ExpressionType.New ||
				expression.NodeType == QuerySourceReferenceExpression.ExpressionType)
			{
				//Probably it should be done by CountResultOperatorProcessor
				return new NhStarExpression(expression);
			}

			return expression;
		}
	}

	internal class MergeAggregatingResultsInExpressionRewriter : ExpressionTreeVisitor
	{
		private readonly NameGenerator _nameGenerator;

		private MergeAggregatingResultsInExpressionRewriter(NameGenerator nameGenerator)
		{
			_nameGenerator = nameGenerator;
		}

		public static Expression Rewrite(Expression expression, NameGenerator nameGenerator)
		{
			var visitor = new MergeAggregatingResultsInExpressionRewriter(nameGenerator);

			return visitor.VisitExpression(expression);
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
											   e => new NhShortCountExpression(e),
											   () => new CountResultOperator());
					case "LongCount":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
											   e => new NhLongCountExpression(e),
											   () => new LongCountResultOperator());
					case "Min":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
											   e => new NhMinExpression(e),
											   () => new MinResultOperator());
					case "Max":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
											   e => new NhMaxExpression(e),
											   () => new MaxResultOperator());
					case "Sum":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
											   e => new NhSumExpression(e),
											   () => new SumResultOperator());
					case "Average":
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
											   e => new NhAverageExpression(e),
											   () => new AverageResultOperator());
				}
			}

			return base.VisitMethodCallExpression(m);
		}

		private Expression CreateAggregate(Expression fromClauseExpression, LambdaExpression body, Func<Expression, Expression> aggregateFactory, Func<ResultOperatorBase> resultOperatorFactory)
		{
			var fromClause = new MainFromClause(_nameGenerator.GetNewName(), body.Parameters[0].Type, fromClauseExpression);
			var selectClause = body.Body;
			selectClause = ReplacingExpressionTreeVisitor.Replace(body.Parameters[0],
																  new QuerySourceReferenceExpression(
																	fromClause), selectClause);
			var queryModel = new QueryModel(fromClause,
											new SelectClause(aggregateFactory(selectClause)));

			// TODO - this sucks, but we use it to get the Type of the SubQueryExpression correct
			queryModel.ResultOperators.Add(resultOperatorFactory());

			var subQuery = new SubQueryExpression(queryModel);

			queryModel.ResultOperators.Clear();

			return subQuery;
		}
	}
}
