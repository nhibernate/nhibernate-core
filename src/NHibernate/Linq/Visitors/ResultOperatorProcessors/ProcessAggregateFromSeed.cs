using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Util;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessAggregateFromSeed : IResultOperatorProcessor<AggregateFromSeedResultOperator>
	{
		public void Process(AggregateFromSeedResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var inputExpr = ((StreamedSequenceInfo)queryModelVisitor.PreviousEvaluationType).ItemExpression;
			var inputType = inputExpr.Type;
			var paramExpr = Expression.Parameter(inputType, "item");
			var accumulatorFunc = Expression.Lambda(
				ReplacingExpressionTreeVisitor.Replace(inputExpr, paramExpr, resultOperator.Func.Body),
				resultOperator.Func.Parameters[0],
				paramExpr);

			var accumulatorType = resultOperator.Func.Parameters[0].Type;
			var inputList = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(typeof(object)), "inputList");

			var castToItem = ReflectionCache.EnumerableMethods.CastDefinition.MakeGenericMethod(new[] { inputType });
			var castToItemExpr = Expression.Call(castToItem, inputList);

			MethodCallExpression call;

			if (resultOperator.OptionalResultSelector == null)
			{
				var aggregate = ReflectionCache.EnumerableMethods.AggregateWithSeedDefinition
					.MakeGenericMethod(inputType, accumulatorType);

				call = Expression.Call(
					aggregate,
					castToItemExpr,
					resultOperator.Seed,
					accumulatorFunc
					);
			}
			else
			{
				var selectorType = resultOperator.OptionalResultSelector.Type.GetGenericArguments()[2];
				var aggregate = ReflectionCache.EnumerableMethods.AggregateWithSeedAndResultSelectorDefinition
					.MakeGenericMethod(inputType, accumulatorType, selectorType);

				call = Expression.Call(
					aggregate,
					castToItemExpr,
					resultOperator.Seed,
					accumulatorFunc,
					resultOperator.OptionalResultSelector
					);
			}

			tree.AddListTransformer(Expression.Lambda(call, inputList));
		}
	}
}
