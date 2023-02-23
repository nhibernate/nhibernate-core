using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.ExpressionTransformers;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;

namespace NHibernate.Linq
{
	public static class NhRelinqQueryParser
	{
		private static readonly QueryParser QueryParser;

		static NhRelinqQueryParser()
		{
			var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
			transformerRegistry.Register(new RemoveRedundantCast());
			transformerRegistry.Register(new SimplifyCompareTransformer());
			transformerRegistry.Register(new EnumEqualsTransformer());

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
				parameters.PreTransformer.Invoke(partiallyEvaluatedExpression),
				parameters.SessionFactory,
				parameters.QueryVariables);
		}

		public static QueryModel Parse(Expression expression)
		{
			return QueryParser.GetParsedQuery(expression);
		}

		internal static Func<Expression, Expression> CreatePreTransformer(IExpressionTransformerRegistrar expressionTransformerRegistrar)
		{
			var preTransformerRegistry = new ExpressionTransformerRegistry();
			// NH-3247: must remove .Net compiler char to int conversion before
			// parameterization occurs.
			preTransformerRegistry.Register(new RemoveCharToIntConversion());
			expressionTransformerRegistrar?.Register(preTransformerRegistry);

			return new TransformingExpressionTreeProcessor(preTransformerRegistry).Process;
		}
	}
}
