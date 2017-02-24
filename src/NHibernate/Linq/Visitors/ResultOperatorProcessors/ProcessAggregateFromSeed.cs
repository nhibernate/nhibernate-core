using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessAggregateFromSeed : IResultOperatorProcessor<AggregateFromSeedResultOperator>
	{
		private static readonly MethodInfo CastMethodDefinition = ReflectionHelper.GetMethodDefinition(
			() => Enumerable.Cast<object>(null));
		private static readonly MethodInfo AggregateMethodDefinition = ReflectionHelper.GetMethodDefinition(
			() => Enumerable.Aggregate<object, object>(null, null, null));
		private static readonly MethodInfo AggregateWithResultOpMethodDefinition = ReflectionHelper.GetMethodDefinition(
			() => Enumerable.Aggregate<object, object, object>(null, null, null, null));

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

			var castToItem = CastMethodDefinition.MakeGenericMethod(new[] { inputType });
			var castToItemExpr = Expression.Call(castToItem, inputList);

			MethodCallExpression call;

			if (resultOperator.OptionalResultSelector == null)
			{
				var aggregate = AggregateMethodDefinition.MakeGenericMethod(inputType, accumulatorType);

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
				var aggregate = AggregateWithResultOpMethodDefinition.MakeGenericMethod(inputType, accumulatorType, selectorType);

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
