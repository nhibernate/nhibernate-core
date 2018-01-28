using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	internal class NhPartialEvaluatingExpressionVisitor : RelinqExpressionVisitor, IPartialEvaluationExceptionExpressionVisitor
	{
		protected override Expression VisitConstant(ConstantExpression expression)
		{
			var value = expression.Value as Expression;
			if (value == null)
			{
				return base.VisitConstant(expression);
			}

			return EvaluateIndependentSubtrees(value);
		}

		public static Expression EvaluateIndependentSubtrees(Expression expression)
		{
			var evaluatedExpression = CustomPartialEvaluatingExpressionVisitor.EvaluateIndependentSubtrees(expression, new NhEvaluatableExpressionFilter());
			return new NhPartialEvaluatingExpressionVisitor().Visit(evaluatedExpression);
		}

		public Expression VisitPartialEvaluationException(PartialEvaluationExceptionExpression partialEvaluationExceptionExpression)
		{
			throw new HibernateException(
				$"Evaluation failure on {partialEvaluationExceptionExpression.EvaluatedExpression}",
				partialEvaluationExceptionExpression.Exception);
		}
	}

	internal class NhEvaluatableExpressionFilter : EvaluatableExpressionFilterBase
	{
		public override bool IsEvaluatableMethodCall(MethodCallExpression node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			var attributes = node.Method
				.GetCustomAttributes(typeof(LinqExtensionMethodAttributeBase), false)
				.Cast<LinqExtensionMethodAttributeBase>().ToArray();
			return attributes.Length == 0 ||
				attributes.Any(a => a.PreEvaluation == LinqExtensionPreEvaluation.AllowPreEvaluation);
		}
	}
}
