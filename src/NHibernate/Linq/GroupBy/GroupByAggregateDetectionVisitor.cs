using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq.GroupBy
{
	// TODO: This needs strengthening.  Possibly a lot in common with the GroupJoinAggregateDetectionVisitor class, which does many more checks
	/// <summary>
	/// Detects if an expression tree contains aggregate functions
	/// </summary>
	internal class GroupByAggregateDetectionVisitor : NhExpressionTreeVisitor
	{
		public bool ContainsAggregateMethods { get; private set; }

		public bool Visit(Expression expression)
		{
			ContainsAggregateMethods = false;

			VisitExpression(expression);

			return ContainsAggregateMethods;
		}

		// TODO - this should not exist, since it should be handled either by re-linq or by the MergeAggregatingResultsRewriter
		protected override Expression VisitMethodCallExpression(MethodCallExpression m)
		{
			if (m.Method.DeclaringType == typeof (Queryable) ||
			    m.Method.DeclaringType == typeof (Enumerable))
			{
				switch (m.Method.Name)
				{
					case "Count":
					case "Min":
					case "Max":
					case "Sum":
					case "Average":
						ContainsAggregateMethods = true;
						break;
				}
			}

			return m;
		}

		protected override Expression VisitNhAggregate(NhAggregatedExpression expression)
		{
			ContainsAggregateMethods = true;
			return expression;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			ContainsAggregateMethods =
				new GroupByAggregateDetectionVisitor().Visit(expression.QueryModel.SelectClause.Selector);

			return expression;
		}
	}
}