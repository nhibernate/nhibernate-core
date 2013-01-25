using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.ResultOperators;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessNonAggregatingGroupBy : IResultOperatorProcessor<NonAggregatingGroupBy>
	{
		public void Process(NonAggregatingGroupBy resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var tSource = queryModelVisitor.Model.SelectClause.Selector.Type;
			var tKey = resultOperator.GroupBy.KeySelector.Type;
			var tElement = resultOperator.GroupBy.ElementSelector.Type;

			// Stuff in the group by that doesn't map to HQL.  Run it client-side
			var listParameter = Expression.Parameter(typeof(IEnumerable<object>), "list");

			var keySelectorExpr = CreateSelector(tSource, resultOperator.GroupBy.KeySelector);

			var elementSelectorExpr = CreateSelector(tSource, resultOperator.GroupBy.ElementSelector);

			var groupByMethod = EnumerableHelper.GetMethod("GroupBy",
														   new[] { typeof(IEnumerable<>), typeof(Func<,>), typeof(Func<,>) },
														   new[] { tSource, tKey, tElement });

			var castToItem = EnumerableHelper.GetMethod("Cast", new[] { typeof(IEnumerable) }, new[] { tSource });

			var toList = EnumerableHelper.GetMethod("ToList", new[] { typeof(IEnumerable<>) }, new[] { resultOperator.GroupBy.ItemType });

			Expression castToItemExpr = Expression.Call(castToItem, listParameter);

			var groupByExpr = Expression.Call(groupByMethod, castToItemExpr, keySelectorExpr, elementSelectorExpr);

			var toListExpr = Expression.Call(toList, groupByExpr);

			var lambdaExpr = Expression.Lambda(toListExpr, listParameter);

			tree.AddListTransformer(lambdaExpr);
		}

		private static LambdaExpression CreateSelector(System.Type sourceType, Expression selector)
		{
			var parameter = Expression.Parameter(sourceType, "item");
			
			var querySource = GetQuerySourceReferenceExpression(selector);
			
			Expression selectorBody;
			if (sourceType != querySource.Type)
			{
				//TODO: it looks like some "magic".
				var member = sourceType.GetMember(((QuerySourceReferenceExpression) selector).ReferencedQuerySource.ItemName)[0];

				selectorBody = Expression.MakeMemberAccess(parameter, member);
			}
			else
			{
				selectorBody = ReplacingExpressionTreeVisitor.Replace(querySource, parameter, selector);
			}
			
			return Expression.Lambda(selectorBody, parameter);
		}

		private static Expression GetQuerySourceReferenceExpression(Expression keySelector)
		{
			return new GroupByKeySourceFinder().Visit(keySelector);
		}
	}
}
