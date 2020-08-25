// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Linq;
using System.Linq.Expressions;
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
		#region Relinq adjusted code

		/// <summary>
		/// Takes an expression tree and finds and evaluates all its evaluatable subtrees.
		/// </summary>
		public static Expression EvaluateIndependentSubtrees(
			Expression expressionTree,
			PreTransformationParameters preTransformationParameters)
		{
			var partialEvaluationInfo = EvaluatableTreeFindingExpressionVisitor.Analyze(
				expressionTree,
				preTransformationParameters.EvaluatableExpressionFilter);
			var visitor = new NhPartialEvaluatingExpressionVisitor(partialEvaluationInfo, preTransformationParameters);

			return visitor.Visit(expressionTree);
		}

		// _partialEvaluationInfo contains a list of the expressions that are safe to be evaluated.
		private readonly PartialEvaluationInfo _partialEvaluationInfo;
		private readonly PreTransformationParameters _preTransformationParameters;

		private NhPartialEvaluatingExpressionVisitor(
			PartialEvaluationInfo partialEvaluationInfo,
			PreTransformationParameters preTransformationParameters)
		{
			_partialEvaluationInfo = partialEvaluationInfo;
			_preTransformationParameters = preTransformationParameters;
		}

		public override Expression Visit(Expression expression)
		{
			// Only evaluate expressions which do not use any of the surrounding parameter expressions. Don't evaluate
			// lambda expressions (even if you could), we want to analyze those later on.
			if (expression == null)
				return null;

			if (expression.NodeType == ExpressionType.Lambda || !_partialEvaluationInfo.IsEvaluatableExpression(expression) ||
				#region NH additions
				// Variables should be evaluated only when they are part of an evaluatable expression (e.g. o => string.Format("...", variable))
				ContainsVariable(expression))
				#endregion
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
				evaluatedExpression = EvaluateIndependentSubtrees(evaluatedExpression, _preTransformationParameters);
			}

			#region NH additions

			// When having multiple level closure, we have to evaluate each closure independently
			if (evaluatedExpression is ConstantExpression constantExpression)
			{
				evaluatedExpression = VisitConstant(constantExpression);
			}

			// Variables in expressions are never a constant, they are encapsulated as fields of a compiler generated class.
			if (expression.NodeType != ExpressionType.Constant &&
			    _preTransformationParameters.MinimizeParameters &&
			    evaluatedExpression is ConstantExpression variableConstant &&
			    !_preTransformationParameters.QueryVariables.ContainsKey(variableConstant) &&
			    ExpressionsHelper.IsVariable(expression, out var path, out var closureContext))
			{
				_preTransformationParameters.QueryVariables.Add(variableConstant, new QueryVariable(path, closureContext));
			}

			#endregion

			return evaluatedExpression;
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

		#region NH additions

		private bool ContainsVariable(Expression expression)
		{
			if (!(expression is UnaryExpression unaryExpression))
			{
				return false;
			}

			return ExpressionsHelper.IsVariable(unaryExpression.Operand, out _, out _) ||
			       // Check whether the variable is casted due to comparison with a nullable expression
			       // (e.g. o.NullableShort == shortVariable)
			       unaryExpression.Operand is UnaryExpression subUnaryExpression &&
			       unaryExpression.Type.UnwrapIfNullable() == subUnaryExpression.Type &&
			       ExpressionsHelper.IsVariable(subUnaryExpression.Operand, out _, out _);
		}

		#endregion

		#endregion

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (expression.Value is Expression value)
			{
				return EvaluateIndependentSubtrees(value, _preTransformationParameters);
			}
			return base.VisitConstant(expression);
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
