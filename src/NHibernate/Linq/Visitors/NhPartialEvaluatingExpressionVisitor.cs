using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Linq.Functions;
using NHibernate.Util;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	// Copied from Relinq and added logic for detecting and linking variables with evaluated constant expressions
	/// <summary>
	/// Takes an expression tree and first analyzes it for evaluatable subtrees (using <see cref="EvaluatableTreeFindingExpressionVisitor"/>), i.e.
	/// subtrees that can be pre-evaluated before actually generating the query. Examples for evaluatable subtrees are operations on constant
	/// values (constant folding), access to closure variables (variables used by the LINQ query that are defined in an outer scope), or method
	/// calls on known objects or their members. In a second step, it replaces all of the evaluatable subtrees (top-down and non-recursive) by 
	/// their evaluated counterparts.
	/// </summary>
	/// <remarks>
	/// This visitor visits each tree node at most twice: once via the <see cref="EvaluatableTreeFindingExpressionVisitor"/> for analysis and once
	/// again to replace nodes if possible (unless the parent node has already been replaced).
	/// </remarks>
	internal sealed class NhPartialEvaluatingExpressionVisitor : RelinqExpressionVisitor
	{
		/// <summary>
		/// Takes an expression tree and finds and evaluates all its evaluatable subtrees.
		/// </summary>
		public static Expression EvaluateIndependentSubtrees(
			Expression expressionTree,
			IEvaluatableExpressionFilter evaluatableExpressionFilter,
			IDictionary<ConstantExpression, QueryVariable> queryVariables)
		{
			var partialEvaluationInfo = EvaluatableTreeFindingExpressionVisitor.Analyze(expressionTree, evaluatableExpressionFilter);
			var visitor = new NhPartialEvaluatingExpressionVisitor(partialEvaluationInfo, evaluatableExpressionFilter, queryVariables);

			return visitor.Visit(expressionTree);
		}

		// _partialEvaluationInfo contains a list of the expressions that are safe to be evaluated.
		private readonly PartialEvaluationInfo _partialEvaluationInfo;
		private readonly IEvaluatableExpressionFilter _evaluatableExpressionFilter;
		private readonly IDictionary<ConstantExpression, QueryVariable> _queryVariables;

		private NhPartialEvaluatingExpressionVisitor(
			PartialEvaluationInfo partialEvaluationInfo,
			IEvaluatableExpressionFilter evaluatableExpressionFilter,
			IDictionary<ConstantExpression, QueryVariable> queryVariables)
		{
			_partialEvaluationInfo = partialEvaluationInfo;
			_evaluatableExpressionFilter = evaluatableExpressionFilter;
			_queryVariables = queryVariables;
		}

		public override Expression Visit(Expression expression)
		{
			// Only evaluate expressions which do not use any of the surrounding parameter expressions. Don't evaluate
			// lambda expressions (even if you could), we want to analyze those later on.
			if (expression == null)
				return null;

			if (expression.NodeType == ExpressionType.Lambda || !_partialEvaluationInfo.IsEvaluatableExpression(expression))
				return base.Visit(expression);

			Expression evaluatedExpression;
			try
			{
				evaluatedExpression = EvaluateSubtree(expression);
			}
			catch (Exception ex)
			{
				// Evaluation caused an exception. Skip evaluation of this expression and proceed as if it weren't evaluable.
				var baseVisitedExpression = base.Visit(expression);

				throw new HibernateException($"Evaluation failure on {baseVisitedExpression}", ex);
			}

			if (evaluatedExpression != expression)
			{
				evaluatedExpression = EvaluateIndependentSubtrees(evaluatedExpression, _evaluatableExpressionFilter, _queryVariables);
			}

			// When having multiple level closure, we have to evaluate each closure independently
			if (evaluatedExpression is ConstantExpression constantExpression)
			{
				evaluatedExpression = VisitConstant(constantExpression);
			}

			// Variables in expressions are never a constant, they are encapsulated as fields of a compiler generated class
			if (expression.NodeType != ExpressionType.Constant &&
			    evaluatedExpression is ConstantExpression variableConstant &&
			    !_queryVariables.ContainsKey(variableConstant) &&
			    IsVariable(expression, out var path, out var closureContext))
			{
				_queryVariables.Add(variableConstant, new QueryVariable(path, closureContext));
			}

			return evaluatedExpression;
		}

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (expression.Value is Expression value)
			{
				return EvaluateIndependentSubtrees(value, _evaluatableExpressionFilter, _queryVariables);
			}

			return base.VisitConstant(expression);
		}

		/// <summary>
		/// Evaluates an evaluatable <see cref="Expression"/> subtree, i.e. an independent expression tree that is compilable and executable
		/// without any data being passed in. The result of the evaluation is returned as a <see cref="ConstantExpression"/>; if the subtree
		/// is already a <see cref="ConstantExpression"/>, no evaluation is performed.
		/// </summary>
		/// <param name="subtree">The subtree to be evaluated.</param>
		/// <returns>A <see cref="ConstantExpression"/> holding the result of the evaluation.</returns>
		private Expression EvaluateSubtree(Expression subtree)
		{
			if (subtree.NodeType == ExpressionType.Constant)
			{
				var constantExpression = (ConstantExpression) subtree;
				var valueAsIQueryable = constantExpression.Value as IQueryable;
				if (valueAsIQueryable != null && valueAsIQueryable.Expression != constantExpression)
					return valueAsIQueryable.Expression;

				return constantExpression;
			}
			else
			{
				Expression<Func<object>> lambdaWithoutParameters = Expression.Lambda<Func<object>>(Expression.Convert(subtree, typeof(object)));
				var compiledLambda = lambdaWithoutParameters.Compile();

				object value = compiledLambda();
				return Expression.Constant(value, subtree.Type);
			}
		}

		private bool IsVariable(Expression expression, out string path, out object closureContext)
		{
			Expression childExpression;
			string currentPath;
			switch (expression)
			{
				case MemberExpression memberExpression:
					childExpression = memberExpression.Expression;
					currentPath = memberExpression.Member.Name;
					break;
				case ConstantExpression constantExpression:
					path = null;
					if (constantExpression.Type.Attributes.HasFlag(TypeAttributes.NestedPrivate) &&
					    Attribute.IsDefined(constantExpression.Type, typeof(CompilerGeneratedAttribute), inherit: true))
					{
						closureContext = constantExpression.Value;
						return true;
					}

					closureContext = null;
					return false;
				case UnaryExpression unaryExpression:
					childExpression = unaryExpression.Operand;
					currentPath = $"({unaryExpression.NodeType})";
					break;
				default:
					path = null;
					closureContext = null;
					return false;
			}

			if (!IsVariable(childExpression, out path, out closureContext))
			{
				return false;
			}

			path = path != null ? $"{path}_{currentPath}" : currentPath;
			return true;
		}
	}

	internal struct QueryVariable : IEquatable<QueryVariable>
	{
		public QueryVariable(string path, object closureContext)
		{
			Path = path;
			ClosureContext = closureContext;
		}

		public string Path { get; }

		public object ClosureContext { get; }

		public override bool Equals(object obj)
		{
			return obj is QueryVariable other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Path.GetHashCode() * 397) ^ ClosureContext.GetHashCode();
			}
		}

		public bool Equals(QueryVariable other)
		{
			return Path == other.Path && ReferenceEquals(ClosureContext, other.ClosureContext);
		}
	}

	internal class NhEvaluatableExpressionFilter : EvaluatableExpressionFilterBase
	{
		private readonly ISessionFactoryImplementor _sessionFactory;

		internal NhEvaluatableExpressionFilter(ISessionFactoryImplementor sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public override bool IsEvaluatableConstant(ConstantExpression node)
		{
			if (node.Value is IPersistentCollection && node.Value is IQueryable)
			{
				return false;
			}

			return base.IsEvaluatableConstant(node);
		}

		public override bool IsEvaluatableMember(MemberExpression node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			if (_sessionFactory == null || _sessionFactory.Settings.LinqToHqlLegacyPreEvaluation ||
				!_sessionFactory.Settings.LinqToHqlGeneratorsRegistry.TryGetGenerator(node.Member, out var generator))
				return true;

			return generator.AllowPreEvaluation(node.Member, _sessionFactory);
		}

		public override bool IsEvaluatableMethodCall(MethodCallExpression node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			var attributes = node.Method
				.GetCustomAttributes(typeof(LinqExtensionMethodAttributeBase), false)
				.ToArray(x => (LinqExtensionMethodAttributeBase) x);
			if (attributes.Length > 0)
				return attributes.Any(a => a.PreEvaluation == LinqExtensionPreEvaluation.AllowPreEvaluation);

			if (_sessionFactory == null || _sessionFactory.Settings.LinqToHqlLegacyPreEvaluation ||
				!_sessionFactory.Settings.LinqToHqlGeneratorsRegistry.TryGetGenerator(node.Method, out var generator))
				return true;

			return generator.AllowPreEvaluation(node.Method, _sessionFactory);
		}
	}
}
