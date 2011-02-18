using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.EagerFetching.Parsing;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Data.Linq.Parsing.Structure.NodeTypeProviders;

namespace NHibernate.Linq
{
    public static class NhRelinqQueryParser
    {
    	private static readonly QueryParser _queryParser;

        static NhRelinqQueryParser()
        {
			var methodInfoRegistry = new MethodInfoBasedNodeTypeRegistry();

			// Here would be useful to have something like our ReflectionHelper.IsMethodOf because it is impossible to know
			// which is the implementation of ICollection<T> used by our user. Reported to Stefan Wenig as possible (via mail instead of their JIAR)

			// FIXME - The version of ReLinq we're using might eliminate the need for this.  Look into it.
			methodInfoRegistry.Register(
				new[]
        			{
								MethodInfoBasedNodeTypeRegistry.GetRegisterableMethodDefinition(
								  ReflectionHelper.GetMethodDefinition((List<object> l) => l.Contains(null))),
								typeof (ICollection<>).GetMethod("Contains"),
        			},
				typeof(ContainsExpressionNode));

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


			var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();
			nodeTypeProvider.InnerProviders.Add(methodInfoRegistry);

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