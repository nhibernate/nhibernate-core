using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Copied from https://github.com/re-motion/Relinq. Unsealing needed to modify treatment of parameters in expressions
	/// and treat them as evaluatable if the derive value from evaluatable expressions.
	/// 
	/// Takes an expression tree and first analyzes it for evaluatable subtrees (using <see cref="T:Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation.EvaluatableTreeFindingExpressionVisitor" />), i.e.
	/// subtrees that can be pre-evaluated before actually generating the query. Examples for evaluatable subtrees are operations on constant
	/// values (constant folding), access to closure variables (variables used by the LINQ query that are defined in an outer scope), or method
	/// calls on known objects or their members. In a second step, it replaces all of the evaluatable subtrees (top-down and non-recursive) by
	/// their evaluated counterparts.
	/// </summary>
	/// <remarks>
	/// This visitor visits each tree node at most twice: once via the <see cref="T:Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation.EvaluatableTreeFindingExpressionVisitor" /> for analysis and once
	/// again to replace nodes if possible (unless the parent node has already been replaced).
	/// </remarks>
	public sealed class PartialEvaluatingExpressionVisitor : RelinqExpressionVisitor
	{
		private readonly PartialEvaluationInfo _partialEvaluationInfo;
		private readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter;

		/// <summary>
		/// Takes an expression tree and finds and evaluates all its evaluatable subtrees.
		/// </summary>
		public static Expression EvaluateIndependentSubtrees(
			Expression expressionTree
			, IEvaluatableExpressionFilter evaluatableExpressionFilter)
		{
			if (expressionTree == null) throw new ArgumentNullException(nameof(expressionTree));
			if (evaluatableExpressionFilter == null) throw new ArgumentNullException(nameof(evaluatableExpressionFilter));
			return new PartialEvaluatingExpressionVisitor(EvaluatableTreeFindingExpressionVisitor.Analyze(expressionTree, evaluatableExpressionFilter)
				, evaluatableExpressionFilter).Visit(expressionTree);
		}

		public PartialEvaluatingExpressionVisitor(
			PartialEvaluationInfo partialEvaluationInfo
			, IEvaluatableExpressionFilter evaluatableExpressionFilter)
		{
			_partialEvaluationInfo = partialEvaluationInfo;
			_evaluatableExpressionFilter = evaluatableExpressionFilter;
		}

		public override Expression Visit(Expression expression)
		{
			if (expression == null)
				return null;
			if (expression.NodeType != ExpressionType.Lambda
			    // inserted after allowing some parameters to be evaluatable; parameters should not be evaluated separately, but
			    // only as part of the method invoking lambda definining them
				&& expression.NodeType != ExpressionType.Parameter)
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
						return EvaluateIndependentSubtrees(subtree, _evaluatableExpressionFilter);
					return subtree;
				}
			}

			return base.Visit(expression);
		}

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

			// inserted after allowing some parameters to be evaluatable; parameters should not be evaluated separately, but
			// only as part of the method invoking lambda definining them
			if (subtree.NodeType == ExpressionType.Parameter)
				return subtree;

			if (subtree.NodeType != ExpressionType.Constant)
				return Expression.Constant(
					Expression.Lambda<Func<object>>(Expression.Convert(subtree, typeof(object))).Compile()(), subtree.Type);

			ConstantExpression constantExpression = (ConstantExpression) subtree;
			IQueryable queryable = constantExpression.Value as IQueryable;
			if (queryable != null && queryable.Expression != constantExpression)
				return queryable.Expression;
			return constantExpression;
		}
	}
}
