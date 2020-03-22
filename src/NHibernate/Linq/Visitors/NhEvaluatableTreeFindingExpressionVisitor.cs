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

		public override Expression Visit (Expression expression)
		{
			_ancestors.Push(expression);

			var result = base.Visit(expression);

			_ancestors.Pop();

			if (expression?.NodeType == ExpressionType.Lambda)
			{
				// defined parameters go out of scope; chained extension methods often use the same parameter names
				var parameterNames = _parameters.Where(p => p.Value.OwningExpression == expression).Select(p => p.Key).ToArray();
				foreach (var name in parameterNames)
					_parameters.Remove(name);
			}

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
			// nameless parameters are generated when updating through linq, no need to handle them yet
			if (expression?.Name == null)
				return false;

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
		///		Note that if parameter is evaluated method call must be evaluatad too eliminating lambda and all the parameters.
		///		If lambda is not eliminated, evaluated parameter produces an error complaining that evaluating lambda parameter
		///		must return non-null expression of the same type.
		/// </remarks>
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

						result.IsEvaluatable = IsMethodSupplyingEvaluatableParameterValues(result.MethodCallInvokingLambda, result.MethodArgumentAcceptingLambda);

						return result;
					}

					result.MethodArgumentAcceptingLambda = ancestor;
				}
			}

			return result;
		}

		/// <summary>
		///		Can be used to determine whether parameters defined by lambda expression are evaluatable by examining method which will
		///		supply parameters to it. Relies on the visiting sequence: lambda parameters are visited after its body. When parameter
		///		is visited, the status of the defining lambda and method call supplying parameters to it has not been established and
		///		<see cref="EvaluatableTreeFindingExpressionVisitor.PartialEvaluationInfo"/> would not contain them. There's also a
		///		chicken-and-egg situation as parameter is visited as a child of lambda body (potentially deep in the tree), but its
		///		evaluatability is determined at the level above lambda. To solve this here we examine the method that accepts the
		///		lambda as one of its parameters and determine if all other arguments are evaluatable, expecting all of them to be
		///		already visited and present in <see cref="EvaluatableTreeFindingExpressionVisitor.PartialEvaluationInfo"/>.
		/// </summary>
		///  <param name="methodExpression">
		///		Method invoking lambda.
		///  </param>
		///  <param name="methodArgumentAcceptingLambda">
		///		Argument (quote) for the lambda expression to which the method passes parameter values.
		/// </param>
		///  <remarks>
		/// 		Member expressions on e.g. repository creating query instances must be evaluated into constants
		/// 		and queryable constants must be evaluated (e.g. for subqueries to be expanded properly),
		/// 		but parameters accepting values from them are not evaluatable.
		///  </remarks>
		private bool IsMethodSupplyingEvaluatableParameterValues(MethodCallExpression methodExpression, Expression methodArgumentAcceptingLambda)
		{
			if (methodExpression == null) throw new ArgumentNullException(nameof(methodExpression));
			if (methodArgumentAcceptingLambda == null) throw new ArgumentNullException(nameof(methodArgumentAcceptingLambda));

			if (!IsEvaluatableMethodCall(methodExpression))
				return false;

			if (methodExpression.Object != null && !PartialEvaluationInfo.IsEvaluatableExpression(methodExpression.Object))
				return false;

			return methodExpression.Arguments.All(a => a == methodArgumentAcceptingLambda || PartialEvaluationInfo.IsEvaluatableExpression(a));
		}

		private bool IsParameterOwner(Expression expression, ParameterExpression parameterExpression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));
			if (parameterExpression == null) throw new ArgumentNullException(nameof(parameterExpression));

			return (expression is LambdaExpression result) && result.Parameters.Any(x => x.Name == parameterExpression.Name);
		}
	}
}
