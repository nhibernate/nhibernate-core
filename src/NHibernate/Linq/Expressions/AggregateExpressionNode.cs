using System;
using System.Linq.Expressions;
using NHibernate.Linq.ResultOperators;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq.Expressions
{
    public class AggregateExpressionNode : ResultOperatorExpressionNodeBase
    {
        public MethodCallExpressionParseInfo ParseInfo { get; set; }
        public Expression OptionalSeed { get; set; }
        public LambdaExpression Accumulator { get; set; }
        public LambdaExpression OptionalSelector { get; set; }

        public AggregateExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression arg1, Expression arg2, LambdaExpression optionalSelector) : base(parseInfo, null, optionalSelector)
        {
            ParseInfo = parseInfo;

            if (arg2 != null)
            {
                OptionalSeed = arg1;
                Accumulator = (LambdaExpression) arg2;
            }
            else
            {
                Accumulator = (LambdaExpression) arg1;
            }

            OptionalSelector = optionalSelector;
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw new NotImplementedException();
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new AggregateResultOperator(ParseInfo, OptionalSeed, Accumulator, OptionalSelector);
        }
    }
}