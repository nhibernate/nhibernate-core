using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessAggregate : IResultOperatorProcessor<AggregateResultOperator>
    {
        public void Process(AggregateResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
			var inputExpr = ((StreamedSequenceInfo)queryModelVisitor.PreviousEvaluationType).ItemExpression;
			var inputType = inputExpr.Type;
			var paramExpr = Expression.Parameter(inputType, "item");
			var accumulatorFunc = Expression.Lambda(
				ReplacingExpressionTreeVisitor.Replace(inputExpr, paramExpr, resultOperator.Func.Body),
				resultOperator.Func.Parameters[0],
				paramExpr);

			var inputList = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(typeof(object)), "inputList");

			var castToItem = EnumerableHelper.GetMethod("Cast", new[] { typeof(IEnumerable) }, new[] { inputType });
			var castToItemExpr = Expression.Call(castToItem, inputList);

			var aggregate = ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object>(null, null));
			aggregate = aggregate.GetGenericMethodDefinition().MakeGenericMethod(inputType);

			MethodCallExpression call = Expression.Call(
				aggregate,
				castToItemExpr,
				accumulatorFunc
				);

			tree.AddListTransformer(Expression.Lambda(call, inputList));
		}
    }
}