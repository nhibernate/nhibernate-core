using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.EagerFetching.Parsing;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using AggregateExpressionNode = NHibernate.Linq.Expressions.AggregateExpressionNode;

namespace NHibernate.Linq
{
    public static class NhRelinqQueryParser
    {
        public static readonly MethodCallExpressionNodeTypeRegistry MethodCallRegistry =
            MethodCallExpressionNodeTypeRegistry.CreateDefault();

        static NhRelinqQueryParser()
        {
            MethodCallRegistry.Register(
                new[]
                    {
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(
                            ReflectionHelper.GetMethod(() => Queryable.Aggregate<object>(null, null))),
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(
                            ReflectionHelper.GetMethod(() => Queryable.Aggregate<object, object>(null, null, null)))
                    },
                typeof (AggregateExpressionNode));

            MethodCallRegistry.Register(
                new[]
                    {
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(
                            ReflectionHelper.GetMethod((List<object> l) => l.Contains(null))),

                    },
                typeof (ContainsExpressionNode));

            MethodCallRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("Fetch") }, typeof(FetchOneExpressionNode));
            MethodCallRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("FetchMany") }, typeof(FetchManyExpressionNode));
            MethodCallRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetch") }, typeof(ThenFetchOneExpressionNode));
            MethodCallRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetchMany") }, typeof(ThenFetchManyExpressionNode));

        }

        public static QueryModel Parse(Expression expression)
        {
            return new QueryParser(new ExpressionTreeParser(MethodCallRegistry)).GetParsedQuery(expression);
        }
    }
}