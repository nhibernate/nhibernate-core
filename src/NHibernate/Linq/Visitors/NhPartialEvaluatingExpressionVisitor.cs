using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	internal class NhPartialEvaluatingExpressionVisitor : RelinqExpressionVisitor
	{
		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (expression.Value as Expression == null)
			{
				return base.VisitConstant(expression);
			}

			return EvaluateIndependentSubtrees(expression.Value as Expression);
		}

		public static Expression EvaluateIndependentSubtrees(Expression expression)
		{
			var evaluatedExpression = PartialEvaluatingExpressionVisitor.EvaluateIndependentSubtrees(expression, new NhEvaluatableExpressionFilter());
			return new NhPartialEvaluatingExpressionVisitor().Visit(evaluatedExpression);
		}

		internal class NhEvaluatableExpressionFilter : EvaluatableExpressionFilterBase
		{
			public NhEvaluatableExpressionFilter()
			{ }
			
			public override bool IsEvaluatableMethodCall(MethodCallExpression node)
			{
				if (node == null)
					throw new ArgumentNullException(nameof(node));

				return node.Method.GetCustomAttributes(typeof(DBOnlyAttribute), false).Length == 0;
			}
		}
	}
}