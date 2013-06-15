using System.Linq.Expressions;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.EagerFetching.Parsing;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq.EagerFetching.Parsing
{
    public class InnerFetchManyExpressionNode : FetchManyExpressionNode
    {
        public InnerFetchManyExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression relatedObjectSelector)
            : base(parseInfo, relatedObjectSelector)
        {
        }

        protected override FetchRequestBase CreateFetchRequest()
        {
            return new InnerFetchManyRequest(base.RelationMember);
        }
    }
}
