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
		private readonly PartialEvaluationInfo _partialEvaluationInfo;
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

			var visitor = new NhPartialEvaluatingExpressionVisitor(partialEvaluationInfo);
			var reEvaluatedExpression = visitor.Visit(evaluatedExpression);

			return reEvaluatedExpression;
		}

		private NhPartialEvaluatingExpressionVisitor(PartialEvaluationInfo partialEvaluationInfo)
		{
			_partialEvaluationInfo = partialEvaluationInfo ?? throw new ArgumentNullException(nameof(partialEvaluationInfo));
		}

        /*
		public override Expression Visit(Expression expression)
		{
			if (expression == null)
				return null;
			if (expression.NodeType != ExpressionType.Lambda)
			{
				if (_partialEvaluationInfo.IsEvaluatableExpression(expression))
				{
					Expression subtree;
					try
					{
						subtree = EvaluateSubtree(expression);
					}
					catch (Exception ex)
					{
						return new PartialEvaluationExceptionExpression(ex, base.Visit(expression));
					}
					if (subtree != expression)
						return EvaluateIndependentSubtrees(subtree);
					return subtree;
				}
			}
			return base.Visit(expression);
		}
        */

        /*
        /// <summary>
        /// Evaluates an evaluatable <see cref="T:System.Linq.Expressions.Expression" /> subtree, i.e. an independent expression tree that is compilable and executable
        /// without any data being passed in. The result of the evaluation is returned as a <see cref="T:System.Linq.Expressions.ConstantExpression" />; if the subtree
        /// is already a <see cref="T:System.Linq.Expressions.ConstantExpression" />, no evaluation is performed.
        /// </summary>
        /// <param name="subtree">The subtree to be evaluated.</param>
        /// <returns>A <see cref="T:System.Linq.Expressions.ConstantExpression" /> holding the result of the evaluation.</returns>
        private Expression EvaluateSubtree(Expression subtree)
        {
	        if (subtree == null) throw new ArgumentNullException(nameof(subtree));

	        if (subtree.NodeType != ExpressionType.Constant)
		        return Expression.Constant(Expression.Lambda<Func<object>>(Expression.Convert(subtree, typeof(object))).Compile()(), subtree.Type);
	        
	        var constantExpression = (ConstantExpression) subtree;

	        if (constantExpression.Value is IQueryable queryable && queryable.Expression != constantExpression)
		        return queryable.Expression;
	        
	        return constantExpression;
        }
        */

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

			if (node.Arguments.Any(a => typeof(IQueryable).IsAssignableFrom(a.Type)))
				return false;

			var attributes = node.Method
				.GetCustomAttributes(typeof(LinqExtensionMethodAttributeBase), false)
				.ToArray(x => (LinqExtensionMethodAttributeBase) x);
			return attributes.Length == 0 ||
				attributes.Any(a => a.PreEvaluation == LinqExtensionPreEvaluation.AllowPreEvaluation);
		}
	}
}
