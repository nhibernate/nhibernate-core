using System.Linq.Expressions;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq.ResultOperators
{
    public class AggregateResultOperator : ClientSideTransformOperator
    {
        public MethodCallExpressionParseInfo ParseInfo { get; set; }
        public Expression OptionalSeed { get; set; }
        public LambdaExpression Accumulator { get; set; }
        public LambdaExpression OptionalSelector { get; set; }

        public AggregateResultOperator(MethodCallExpressionParseInfo parseInfo, Expression optionalSeed, LambdaExpression accumulator, LambdaExpression optionalSelector)
        {
            ParseInfo = parseInfo;
            OptionalSeed = optionalSeed;
            Accumulator = accumulator;
            OptionalSelector = optionalSelector;
        }
    }
}