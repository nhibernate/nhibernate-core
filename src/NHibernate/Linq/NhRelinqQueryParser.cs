using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.EagerFetching.Parsing;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace NHibernate.Linq
{
	public static class NhRelinqQueryParser
	{
		private static readonly QueryParser _queryParser;

		static NhRelinqQueryParser()
		{
			var nodeTypeProvider = new NHibernateNodeTypeProvider();

			var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
			// Register custom expression transformers here:
			// transformerRegistry.Register (new MyExpressionTransformer());

			var processor = ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry);
			// Add custom processors here:
			// processor.InnerProcessors.Add (new MyExpressionTreeProcessor());

			var expressionTreeParser = new ExpressionTreeParser(nodeTypeProvider, processor);

			_queryParser = new QueryParser(expressionTreeParser);			
		}

		public static QueryModel Parse(Expression expression)
		{
			return _queryParser.GetParsedQuery(expression);
		}
	}

	public class NHibernateNodeTypeProvider : INodeTypeProvider
	{
		private INodeTypeProvider defaultNodeTypeProvider;

		public NHibernateNodeTypeProvider()
		{
			var methodInfoRegistry = new MethodInfoBasedNodeTypeRegistry();

			methodInfoRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("Fetch") }, typeof(FetchOneExpressionNode));
			methodInfoRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("FetchMany") }, typeof(FetchManyExpressionNode));
			methodInfoRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetch") }, typeof(ThenFetchOneExpressionNode));
			methodInfoRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetchMany") }, typeof(ThenFetchManyExpressionNode));

			methodInfoRegistry.Register(
				new[]
					{
						typeof(LinqExtensionMethods).GetMethod("Cacheable"),
						typeof(LinqExtensionMethods).GetMethod("CacheMode"),
						typeof(LinqExtensionMethods).GetMethod("CacheRegion"),
					}, typeof(CacheableExpressionNode));

			methodInfoRegistry.Register(
				new[]
					{
						ReflectionHelper.GetMethodDefinition(() => Queryable.AsQueryable(null)),
						ReflectionHelper.GetMethodDefinition(() => Queryable.AsQueryable<object>(null)),
					}, typeof(AsQueryableExpressionNode)
				);

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

		protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
		{
			return queryModel;
		}
	}

	public class CacheableExpressionNode : ResultOperatorExpressionNodeBase
	{
		private readonly MethodCallExpressionParseInfo _parseInfo;
		private readonly ConstantExpression _data;

		public CacheableExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression data) : base(parseInfo, null, null)
		{
			_parseInfo = parseInfo;
			_data = data;
		}

		public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
		{
			throw new NotImplementedException();
		}

		protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
		{
			return new CacheableResultOperator(_parseInfo, _data);
		}
	}

	public class CacheableResultOperator : ResultOperatorBase
	{
		public MethodCallExpressionParseInfo ParseInfo { get; private set; }
		public ConstantExpression Data { get; private set; }

		public CacheableResultOperator(MethodCallExpressionParseInfo parseInfo, ConstantExpression data)
		{
			ParseInfo = parseInfo;
			Data = data;
		}

		public override IStreamedData ExecuteInMemory(IStreamedData input)
		{
			throw new NotImplementedException();
		}

		public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
		{
			return inputInfo;
		}

		public override ResultOperatorBase Clone(CloneContext cloneContext)
		{
			throw new NotImplementedException();
		}

		public override void TransformExpressions(Func<Expression, Expression> transformation)
		{
		}
	}
}