using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.ExpressionTransformers;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.EagerFetching.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
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

		/// <summary>
		/// Applies the minimal transformations required before parameterization,
		/// expression key computing and parsing.
		/// </summary>
		/// <param name="expression">The expression to transform.</param>
		/// <returns>The transformed expression.</returns>
		public static Expression PreTransform(Expression expression)
		{
			var partiallyEvaluatedExpression = NhPartialEvaluatingExpressionVisitor.EvaluateIndependentSubtrees(expression);
			return PreProcessor.Process(partiallyEvaluatedExpression);
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
				new[] { ReflectHelper.GetMethodDefinition(() => EagerFetchingExtensionMethods.Fetch<object, object>(null, null)) },
				typeof(FetchOneExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.GetMethodDefinition(() => EagerFetchingExtensionMethods.FetchMany<object, object>(null, null)) },
				typeof(FetchManyExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.GetMethodDefinition(() => EagerFetchingExtensionMethods.ThenFetch<object, object, object>(null, null)) },
				typeof(ThenFetchOneExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.GetMethodDefinition(() => EagerFetchingExtensionMethods.ThenFetchMany<object, object, object>(null, null)) },
				typeof(ThenFetchManyExpressionNode));

			methodInfoRegistry.Register(
				new[]
					{
						ReflectHelper.GetMethodDefinition(() => Queryable.AsQueryable(null)),
						ReflectHelper.GetMethodDefinition(() => Queryable.AsQueryable<object>(null)),
					}, typeof(AsQueryableExpressionNode)
				);

			methodInfoRegistry.Register(new[] { LinqExtensionMethods.SetOptionsDefinition }, typeof(OptionsExpressionNode));

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

	public class AsQueryableExpressionNode : MethodCallExpressionNodeBase
	{
		public AsQueryableExpressionNode(MethodCallExpressionParseInfo parseInfo) : base(parseInfo)
		{
		}

		public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
		{
			return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
		}

		protected override void ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
		{
		}
	}

	internal class OptionsExpressionNode : ResultOperatorExpressionNodeBase
	{
		private readonly MethodCallExpressionParseInfo _parseInfo;
		private readonly ConstantExpression _setOptions;

		public OptionsExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression setOptions)
			: base(parseInfo, null, null)
		{
			_parseInfo = parseInfo;
			if (setOptions.NodeType == ExpressionType.Constant)
			{
				_setOptions = (ConstantExpression) setOptions;
			}
			else
			{
				var lambda = setOptions as Expression<Action<IQueryableOptions>>;
				if (lambda!=null)
				{
					_setOptions = Expression.Constant(lambda.Compile());
				}
			}
		}

		public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
		{
			return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
		}

		protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
		{
			if (_setOptions != null)
			{
				return new OptionsResultOperator(_parseInfo, _setOptions);
			}
			return null;
		}
	}

	internal class OptionsResultOperator : SequenceTypePreservingResultOperatorBase
	{
		public MethodCallExpressionParseInfo ParseInfo { get; }
		public ConstantExpression SetOptions { get; }

		public OptionsResultOperator(MethodCallExpressionParseInfo parseInfo, ConstantExpression setOptions)
		{
			ParseInfo = parseInfo;
			SetOptions = setOptions;
		}

		

		public override ResultOperatorBase Clone(CloneContext cloneContext)
		{
			throw new NotImplementedException();
		}

		public override void TransformExpressions(Func<Expression, Expression> transformation)
		{
		}

		public override StreamedSequence ExecuteInMemory<T>(StreamedSequence input)
		{
			return input;
		}
	}
}
