using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Linq.ExpressionTransformers;
using NHibernate.Linq.Visitors;
using NHibernate.Param;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.EagerFetching.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace NHibernate.Linq
{
	public static class NhRelinqQueryParser
	{
		private static readonly QueryParser QueryParser;
		private static readonly IExpressionTreeProcessor PreProcessor;

		static NhRelinqQueryParser()
		{
			var preTransformerRegistry = new ExpressionTransformerRegistry();
			// NH-3247: must remove .Net compiler char to int conversion before
			// parameterization occurs.
			preTransformerRegistry.Register(new RemoveCharToIntConversion());
			PreProcessor = new TransformingExpressionTreeProcessor(preTransformerRegistry);

			var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
			transformerRegistry.Register(new RemoveRedundantCast());
			transformerRegistry.Register(new SimplifyCompareTransformer());

			// If needing a compound processor for adding other processing, do not use
			// ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry), it would
			// cause NH-3961 again by including a PartialEvaluatingExpressionTreeProcessor.
			// Directly instantiate a CompoundExpressionTreeProcessor instead.
			var processor = new TransformingExpressionTreeProcessor(transformerRegistry);

			var nodeTypeProvider = new NHibernateNodeTypeProvider();

			var expressionTreeParser = new ExpressionTreeParser(nodeTypeProvider, processor);
			QueryParser = new QueryParser(expressionTreeParser);
		}

		// Obsolete since v5.3
		/// <summary>
		/// Applies the minimal transformations required before parametrization,
		/// expression key computing and parsing.
		/// </summary>
		/// <param name="expression">The expression to transform.</param>
		/// <returns>The transformed expression.</returns>
		[Obsolete("Use overload with PreTransformationParameters parameter")]
		public static Expression PreTransform(Expression expression)
		{
			// In order to keep the old behavior use a DML query mode to skip detecting variables,
			// which will then generate parameters for each constant expression
			return PreTransform(expression, new PreTransformationParameters(QueryMode.Delete, null)).Expression;
		}

		/// <summary>
		/// Applies the minimal transformations required before parametrization,
		/// expression key computing and parsing.
		/// </summary>
		/// <param name="expression">The expression to transform.</param>
		/// <param name="parameters">The parameters used in the transformation process.</param>
		/// <returns><see cref="PreTransformationResult"/> that contains the transformed expression.</returns>
		public static PreTransformationResult PreTransform(Expression expression, PreTransformationParameters parameters)
		{
			parameters.EvaluatableExpressionFilter = new NhEvaluatableExpressionFilter(parameters.SessionFactory);
			parameters.QueryVariables = new Dictionary<ConstantExpression, QueryVariable>();

			var partiallyEvaluatedExpression = NhPartialEvaluatingExpressionVisitor
				.EvaluateIndependentSubtrees(expression, parameters);

			return new PreTransformationResult(
				PreProcessor.Process(partiallyEvaluatedExpression),
				parameters.SessionFactory,
				parameters.QueryVariables);
		}

		public static QueryModel Parse(Expression expression)
		{
			return QueryParser.GetParsedQuery(expression);
		}
	}

	public class NHibernateNodeTypeProvider : INodeTypeProvider
	{
		private INodeTypeProvider defaultNodeTypeProvider;

		public NHibernateNodeTypeProvider()
		{
			var methodInfoRegistry = new MethodInfoBasedNodeTypeRegistry();

			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.Fetch, default(IQueryable<object>), default(Expression<Func<object, object>>)) },
				typeof(FetchOneExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.FetchLazyProperties, default(IQueryable<object>)) },
				typeof(FetchLazyPropertiesExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.FetchMany, default(IQueryable<object>), default(Expression<Func<object, IEnumerable<object>>>)) },
				typeof(FetchManyExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.ThenFetch, default(INhFetchRequest<object, object>), default(Expression<Func<object, object>>)) },
				typeof(ThenFetchOneExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition( EagerFetchingExtensionMethods.ThenFetchMany, default(INhFetchRequest<object, object>), default(Expression<Func<object, IEnumerable<object>>>)) },
				typeof(ThenFetchManyExpressionNode));
			methodInfoRegistry.Register(
				new[]
				{
					ReflectHelper.FastGetMethodDefinition(LinqExtensionMethods.WithLock, default(IQueryable<object>), default(LockMode)),
					ReflectHelper.FastGetMethodDefinition(LinqExtensionMethods.WithLock, default(IEnumerable<object>), default(LockMode))
				}, 
				typeof(LockExpressionNode));

			var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();
			nodeTypeProvider.InnerProviders.Add(methodInfoRegistry);
			defaultNodeTypeProvider = nodeTypeProvider;
		}

		public bool IsRegistered(MethodInfo method)
		{
			// Avoid Relinq turning IDictionary.Contains into ContainsResultOperator.  We do our own processing for that method.
			if (method.DeclaringType == typeof(IDictionary) && method.Name == "Contains")
				return false;

			return defaultNodeTypeProvider.IsRegistered(method);
		}

		public System.Type GetNodeType(MethodInfo method)
		{
			return defaultNodeTypeProvider.GetNodeType(method);
		}
	}
}
