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
			var inputList = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(inputType), "inputList");

			MethodCallExpression call;

			if (resultOperator.OptionalResultSelector == null)
			{
				var aggregate = ReflectionCache.EnumerableMethods.AggregateWithSeedDefinition
					.MakeGenericMethod(inputType, accumulatorType);

				call = Expression.Call(
					aggregate,
					inputList,
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
					inputList,
					resultOperator.Seed,
					accumulatorFunc,
					resultOperator.OptionalResultSelector
					);
			}

			// NH-3850: changed from list transformer (working on IEnumerable<object>) to post execute
			// transformer (working on IEnumerable<inputType>) for globally aggregating polymorphic results
			// instead of aggregating results for each class separately and yielding only the first.
			// If the aggregation relies on ordering, final result will still be wrong due to
			// polymorphic results being union-ed without re-ordering. (This is a limitation of all polymorphic
			// queries, this is not specific to LINQ provider.)
			tree.AddPostExecuteTransformer(Expression.Lambda(call, inputList));
			// There is no more a list transformer yielding an IList<resultType>, have to override the execute
			// result type.
			tree.ExecuteResultTypeOverride = inputType;
		}
	}
}
