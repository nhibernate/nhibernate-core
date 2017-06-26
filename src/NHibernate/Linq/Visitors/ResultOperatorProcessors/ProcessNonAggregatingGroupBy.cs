using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.ResultOperators;
using NHibernate.Util;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessNonAggregatingGroupBy : IResultOperatorProcessor<NonAggregatingGroupBy>
	{
		public void Process(NonAggregatingGroupBy resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var selector = queryModelVisitor.Model.SelectClause.Selector;
			var keySelector = resultOperator.GroupBy.KeySelector;
			var elementSelector = resultOperator.GroupBy.ElementSelector;

			var sourceType = selector.Type;
			var keyType = keySelector.Type;
			var elementType = elementSelector.Type;

			// Stuff in the group by that doesn't map to HQL.  Run it client-side
			var listParameter = Expression.Parameter(typeof(IEnumerable<object>), "list");

			var keySelectorExpr = ReverseResolvingExpressionTreeVisitor.ReverseResolve(selector, keySelector);

			var elementSelectorExpr = ReverseResolvingExpressionTreeVisitor.ReverseResolve(selector, elementSelector);

			var groupByMethod = ReflectionCache.EnumerableMethods.GroupByWithElementSelectorDefinition
				.MakeGenericMethod(new[] { sourceType, keyType, elementType });

			var castToItem = ReflectionCache.EnumerableMethods.CastDefinition.MakeGenericMethod(new[] { sourceType });

			var toList = ReflectionCache.EnumerableMethods.ToListDefinition.MakeGenericMethod(new[] { resultOperator.GroupBy.ItemType });

			Expression castToItemExpr = Expression.Call(castToItem, listParameter);

			var groupByExpr = Expression.Call(groupByMethod, castToItemExpr, keySelectorExpr, elementSelectorExpr);

			var toListExpr = Expression.Call(toList, groupByExpr);

			var lambdaExpr = Expression.Lambda(toListExpr, listParameter);

			tree.AddListTransformer(lambdaExpr);
		}
	}
}
