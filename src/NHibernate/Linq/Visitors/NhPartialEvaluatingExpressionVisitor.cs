using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Collection;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	internal class NhPartialEvaluatingExpressionVisitor : RelinqExpressionVisitor, IPartialEvaluationExceptionExpressionVisitor
	{
		private static readonly NhEvaluatableExpressionFilter ExpressionFilter = new NhEvaluatableExpressionFilter();

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
			var partialEvaluationInfo = NhEvaluatableTreeFindingExpressionVisitor.Analyze(expression, ExpressionFilter);
            
            var firstPassVisitor = new PartialEvaluatingExpressionVisitor(partialEvaluationInfo, ExpressionFilter);

			var evaluatedExpression = firstPassVisitor.Visit(expression);

			var reEvaluatedExpression = new NhPartialEvaluatingExpressionVisitor().Visit(evaluatedExpression);

			return reEvaluatedExpression;
		}

		private NhPartialEvaluatingExpressionVisitor()
		{
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

			//below could be uncommented, but considering usage it is inconsequential
            //if (!EvaluatableTreeFindingExpressionVisitor.IsEvaluatableMethodCall(node))
            //    return false;

			var attributes = node.Method
				.GetCustomAttributes(typeof(LinqExtensionMethodAttributeBase), false)
				.ToArray(x => (LinqExtensionMethodAttributeBase) x);
			return attributes.Length == 0 ||
				attributes.Any(a => a.PreEvaluation == LinqExtensionPreEvaluation.AllowPreEvaluation);
		}
	}
}
