using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessAggregate : IResultOperatorProcessor<AggregateResultOperator>
    {
        public void Process(AggregateResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            var inputType = resultOperator.Accumulator.Parameters[1].Type;
            var accumulatorType = resultOperator.Accumulator.Parameters[0].Type;
            var inputList = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(typeof(object)), "inputList");

            var castToItem = EnumerableHelper.GetMethod("Cast", new[] { typeof(IEnumerable) }, new[] { inputType });
            var castToItemExpr = Expression.Call(castToItem, inputList);

            MethodCallExpression call;

            if (resultOperator.ParseInfo.ParsedExpression.Arguments.Count == 2)
            {
                var aggregate = ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object>(null, null));
                aggregate = aggregate.GetGenericMethodDefinition().MakeGenericMethod(inputType);

                call = Expression.Call(
                    aggregate,
                    castToItemExpr,
                    resultOperator.Accumulator
                    );

            }
            else if (resultOperator.ParseInfo.ParsedExpression.Arguments.Count == 3)
            {
                var aggregate = ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object>(null, null, null));
                aggregate = aggregate.GetGenericMethodDefinition().MakeGenericMethod(inputType, accumulatorType);

                call = Expression.Call(
                    aggregate,
                    castToItemExpr,
                    resultOperator.OptionalSeed,
                    resultOperator.Accumulator
                    );
            }
            else
            {
                var selectorType = resultOperator.OptionalSelector.Type.GetGenericArguments()[2];
                var aggregate = ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object, object>(null, null, null, null));
                aggregate = aggregate.GetGenericMethodDefinition().MakeGenericMethod(inputType, accumulatorType, selectorType);

                call = Expression.Call(
                    aggregate,
                    castToItemExpr,
                    resultOperator.OptionalSeed,
                    resultOperator.Accumulator,
                    resultOperator.OptionalSelector
                    );
            }

            tree.AddListTransformer(Expression.Lambda(call, inputList));
        }
    }
}