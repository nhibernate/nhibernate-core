using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Collection;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	internal class NhPartialEvaluatingExpressionVisitor : RelinqExpressionVisitor, IPartialEvaluationExceptionExpressionVisitor
	{
		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (expression.Value is Expression value)
			{
				return EvaluateIndependentSubtrees(value);
			}

			return base.VisitConstant(expression);
		}

		public static Expression EvaluateIndependentSubtrees(Expression expression)
		{
			var evaluatedExpression = PartialEvaluatingExpressionVisitor.EvaluateIndependentSubtrees(expression, new NhEvaluatableExpressionFilter());
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
		public override bool IsEvaluatableConstant(ConstantExpression node)
		{
			if (node.Value is IPersistentCollection && node.Value is IQueryable)
			{
				return false;
			}

			return base.IsEvaluatableConstant(node);
		}

		public override bool IsEvaluatableMethodCall(MethodCallExpression node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			var attributes = node.Method
				.GetCustomAttributes(typeof(LinqExtensionMethodAttributeBase), false)
				.ToArray(x => (LinqExtensionMethodAttributeBase) x);
			return attributes.Length == 0 ||
				attributes.Any(a => a.PreEvaluation == LinqExtensionPreEvaluation.AllowPreEvaluation);
		}
	}
}
