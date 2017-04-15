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
using Remotion.Linq.Parsing.ExpressionVisitors;

namespace NHibernate.Linq.ReWriters
{
	public class MergeAggregatingResultsRewriter : QueryModelVisitorBase
	{
		private MergeAggregatingResultsRewriter()
		{ }

		public static void ReWrite(QueryModel model)
		{
			var rewriter = new MergeAggregatingResultsRewriter();

			rewriter.VisitQueryModel(model);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			switch (resultOperator)
			{
				case SumResultOperator sum:
					queryModel.SelectClause.Selector = new NhSumExpression(queryModel.SelectClause.Selector);
					queryModel.ResultOperators.Remove(resultOperator);
					break;
				case AverageResultOperator avg:
					queryModel.SelectClause.Selector = new NhAverageExpression(queryModel.SelectClause.Selector);
					queryModel.ResultOperators.Remove(resultOperator);
					break;
				case MinResultOperator min:
					queryModel.SelectClause.Selector = new NhMinExpression(queryModel.SelectClause.Selector);
					queryModel.ResultOperators.Remove(resultOperator);
					break;
				case MaxResultOperator max:
					queryModel.SelectClause.Selector = new NhMaxExpression(queryModel.SelectClause.Selector);
					queryModel.ResultOperators.Remove(resultOperator);
					break;
				case DistinctResultOperator distinct:
					queryModel.SelectClause.Selector = new NhDistinctExpression(queryModel.SelectClause.Selector);
					queryModel.ResultOperators.Remove(resultOperator);
					break;
				case CountResultOperator count:
					queryModel.SelectClause.Selector = new NhShortCountExpression(TransformCountExpression(queryModel.SelectClause.Selector));
					queryModel.ResultOperators.Remove(resultOperator);
					break;
				case LongCountResultOperator longCount:
					queryModel.SelectClause.Selector = new NhLongCountExpression(TransformCountExpression(queryModel.SelectClause.Selector));
					queryModel.ResultOperators.Remove(resultOperator);
					break;
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
				expression is QuerySourceReferenceExpression)
			{
				//Probably it should be done by CountResultOperatorProcessor
				return new NhStarExpression(expression);
			}

			return expression;
		}
	}

	internal class MergeAggregatingResultsInExpressionRewriter : RelinqExpressionVisitor
	{
		private readonly NameGenerator _nameGenerator;

		private MergeAggregatingResultsInExpressionRewriter(NameGenerator nameGenerator)
		{
			_nameGenerator = nameGenerator;
		}

		public static Expression Rewrite(Expression expression, NameGenerator nameGenerator)
		{
			var visitor = new MergeAggregatingResultsInExpressionRewriter(nameGenerator);

			return visitor.Visit(expression);
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			MergeAggregatingResultsRewriter.ReWrite(expression.QueryModel);
			return expression;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			if (m.Method.DeclaringType == typeof(Queryable) ||
				m.Method.DeclaringType == typeof(Enumerable))
			{
				switch (m.Method.Name)
				{
					case nameof(Queryable.Count):
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
							e => new NhShortCountExpression(e),
							() => new CountResultOperator());
					case nameof(Queryable.LongCount):
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
							e => new NhLongCountExpression(e),
							() => new LongCountResultOperator());
					case nameof(Queryable.Min):
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
							e => new NhMinExpression(e),
							() => new MinResultOperator());
					case nameof(Queryable.Max):
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
							e => new NhMaxExpression(e),
							() => new MaxResultOperator());
					case nameof(Queryable.Sum):
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
							e => new NhSumExpression(e),
							() => new SumResultOperator());
					case nameof(Queryable.Average):
						return CreateAggregate(m.Arguments[0], (LambdaExpression)m.Arguments[1],
							e => new NhAverageExpression(e),
							() => new AverageResultOperator());
				}
			}

			return base.VisitMethodCall(m);
		}

		private Expression CreateAggregate(Expression fromClauseExpression, LambdaExpression body,
			Func<Expression, Expression> aggregateFactory, Func<ResultOperatorBase> resultOperatorFactory)
		{
			var fromClause = new MainFromClause(_nameGenerator.GetNewName(), body.Parameters[0].Type, fromClauseExpression);
			var selectClause = body.Body;
			selectClause = ReplacingExpressionVisitor.Replace(body.Parameters[0],
				new QuerySourceReferenceExpression(fromClause), selectClause);
			var queryModel = new QueryModel(fromClause, new SelectClause(aggregateFactory(selectClause)));

			// TODO - this sucks, but we use it to get the Type of the SubQueryExpression correct
			queryModel.ResultOperators.Add(resultOperatorFactory());

			var subQuery = new SubQueryExpression(queryModel);

			queryModel.ResultOperators.Clear();

			return subQuery;
		}
	}
}
