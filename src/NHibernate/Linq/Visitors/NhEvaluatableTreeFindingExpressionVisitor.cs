using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	public class NhEvaluatableTreeFindingExpressionVisitor : EvaluatableTreeFindingExpressionVisitor
	{
		private class ParameterStatus
		{
			public ParameterExpression Expression { get; set; }

			public bool IsEvaluatable { get; set; }

			public LambdaExpression OwningExpression { get; set; }

			public MethodCallExpression MethodCallInvokingLambda { get; set; }

			public Expression MethodArgumentAcceptingLambda { get; set; }
		}

		private readonly Stack<Expression> _ancestors = new Stack<Expression>(20);
		private readonly Dictionary<string, ParameterStatus> _parameters = new Dictionary<string, ParameterStatus>();

		public Expression TopAncestor => _ancestors.FirstOrDefault();

		public new static PartialEvaluationInfo Analyze(Expression expressionTree, IEvaluatableExpressionFilter evaluatableExpressionFilter)
		{
			if (expressionTree == null) throw new ArgumentNullException(nameof(expressionTree));
			if (evaluatableExpressionFilter == null) throw new ArgumentNullException(nameof(evaluatableExpressionFilter));

			var visitor = new NhEvaluatableTreeFindingExpressionVisitor(evaluatableExpressionFilter);
			visitor.Visit(expressionTree);

			return visitor.PartialEvaluationInfo;
		}

		protected NhEvaluatableTreeFindingExpressionVisitor(IEvaluatableExpressionFilter evaluatableExpressionFilter)
			: base(evaluatableExpressionFilter)
		{
		}

		/// <param name="currentExpression">
		///     If already added to <see cref="_ancestors"/>, it should be at the top.
		/// </param>
		/// <returns>
		///     if <paramref name="currentExpression"/> is <see cref="TopAncestor"/>, then its parent;
		///     otherwise the <see cref="TopAncestor"/>.
		/// </returns>
		public Expression GetParentExpression(Expression currentExpression)
		{
			return _ancestors.FirstOrDefault(x => x != currentExpression);
		}

		public override Expression Visit (Expression expression)
		{
			_ancestors.Push(expression);

			var result = base.Visit(expression);

			_ancestors.Pop();

			return result;
		}

		/// <inheritdoc />
		protected override Expression VisitParameter(ParameterExpression expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			// Parameters are evaluatable if they are supplied by evaluatable expression.
			// look up lambda defining the parameter, up the ancestor list and check if method call to which it is passed is on evaluatable instance of extension method accepting evaluatable arguments
			// note that lambda body is visited prior to parameters
			// since method call is visited in the order {instance, arguments} and extension methods get instance as first parameter
			// the source of the parameter is already visited and its evaluatability established
			if (IsCurrentSubtreeEvaluatable)
				IsCurrentSubtreeEvaluatable = IsParameterEvaluatable(expression);

			return expression;
		}

		private bool IsParameterEvaluatable(ParameterExpression expression)
		{
			if (_parameters.TryGetValue(expression.Name, out var status))
				return status.IsEvaluatable;

			status = CalcParameterStatus(expression);
			_parameters.Add(expression.Name, status);

			return status.IsEvaluatable;
		}

		/// <remarks>
		///		Parameters are evaluatable if they are supplied by evaluatable expression.
		///		Look up lambda defining the parameter, up the ancestor list and check if method call to which it is passed is on evaluatable
		///		instance or extension method accepting evaluatable arguments.
		///		Note that lambda body is visited prior to parameters.
		///		Since method call is visited in the order [instance, arguments] and extension methods get instance as first parameter
		///		the source of the parameter is already visited and its evaluatability established.
		/// </remarks>>
		private ParameterStatus CalcParameterStatus(ParameterExpression expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			var result = new ParameterStatus {Expression = expression};

			foreach (var ancestor in _ancestors)
			{
				if (result.MethodArgumentAcceptingLambda == null && IsParameterOwner(ancestor, expression))
				{
					result.MethodArgumentAcceptingLambda = result.OwningExpression = (LambdaExpression)ancestor;
				}
				else if (result.MethodArgumentAcceptingLambda != null)
				{
					if (ancestor.NodeType == ExpressionType.Call)
					{
						result.MethodCallInvokingLambda = (MethodCallExpression) ancestor;

						result.IsEvaluatable = result.MethodCallInvokingLambda.Object != null
							? PartialEvaluationInfo.IsEvaluatableExpression(result.MethodCallInvokingLambda.Object)
							: result.MethodCallInvokingLambda.Arguments.All(a => a == result.MethodArgumentAcceptingLambda || PartialEvaluationInfo.IsEvaluatableExpression(a));

						return result;
					}

					result.MethodArgumentAcceptingLambda = ancestor;
				}

			}

			return result;
		}

		private bool IsParameterOwner(Expression expression, ParameterExpression parameterExpression)
		{
			return (expression is LambdaExpression result) && result.Parameters.Any(x => x.Name == parameterExpression.Name);
		}
	}
}
