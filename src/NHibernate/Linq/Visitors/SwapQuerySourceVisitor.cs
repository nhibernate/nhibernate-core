using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors
{
	public class SwapQuerySourceVisitor : NhExpressionTreeVisitor
	{
		private readonly IQuerySource _oldClause;
		private readonly IQuerySource _newClause;

		public SwapQuerySourceVisitor(IQuerySource oldClause, IQuerySource newClause)
		{
			_oldClause = oldClause;
			_newClause = newClause;
		}

		public Expression Swap(Expression expression)
		{
			return VisitExpression(expression);
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			if (expression.ReferencedQuerySource == _oldClause)
			{
				return new QuerySourceReferenceExpression(_newClause);
			}

			// TODO - really don't like this drill down approach.  Feels fragile
			var mainFromClause = expression.ReferencedQuerySource as MainFromClause;

			if (mainFromClause != null)
			{
				mainFromClause.FromExpression = VisitExpression(mainFromClause.FromExpression);
			}

			return expression;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(VisitExpression);
			return base.VisitSubQueryExpression(expression);
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			// If a subquery projects to e.g. anonymous type, and the outer
			// query projects again to a different type, the property in the MemberExpression
			// will refer to a property on the type returned by the subquery. When the from
			// clauses are swapped, Relinq will attempt to apply the property to the type of the
			// from clause in the subquery, which will be a different type and cause exception.

			// So replace the MemberExpression so that it applies to the inner projection
			// directly. Then optimize that with TransparentIdentifierRemovingExpressionTreeVisitor.

			var querySource = expression.Expression as QuerySourceReferenceExpression;
			if (querySource != null)
			{
				var innerSelector = GetSubQuerySelectorOrNull(querySource);

				if (innerSelector != null)
				{
					var access = Expression.MakeMemberAccess(innerSelector, expression.Member);
					return TransparentIdentifierRemovingExpressionTreeVisitor.ReplaceTransparentIdentifiers(access);
				}
			}

			return base.VisitMemberExpression(expression);
		}


		protected override Expression VisitNewExpression(NewExpression expression)
		{
			var arguments = new List<Expression>();

			foreach (var argument in expression.Arguments)
			{
				var newArg = VisitAndConvert(argument, "VisitNewExpression");

				// Consider a query where the outer select grabs the entire element from
				// the sub select (NH-3326):
				//     var list = db.Products
				//                  .Select(p => new { p.ProductId, p.Name })
				//                  .Skip(5).Take(10)
				//                  .Select(a => new { ExpandedElement = a, a.Name, a.ProductId })
				//                  .ToList();
				// The QuerySourceReferenceExpression 'a' (of an anonymous
				// type) will be replaced by a QuerySourceReferenceExpression typed as "Product")
				// by VisitQuerySourceReferenceExpression(). This doesn't match the expected
				// type in the outer select clause, causing InvalidOperationException "No coercion operator...".
				// So we need to replace the QuerySourceReferenceExpression with the NewExpression from
				// its select clause to get the correct type.

				var querySource = argument as QuerySourceReferenceExpression;
				if (querySource != null && newArg != argument && newArg.Type != argument.Type)
				{
					NewExpression innerSelector = GetSubQuerySelectorOrNull(querySource);

					if (innerSelector != null)
						newArg = innerSelector;
				}

				arguments.Add(newArg);
			}

			var expr = Expression.New(expression.Constructor, arguments, expression.Members);
			return expr;
		}


		/// <summary>
		/// If the querySource is a subquery, return the SelectClause's selector if it's
		/// NewExpression. Otherwise, return null.
		/// </summary>
		private static NewExpression GetSubQuerySelectorOrNull(QuerySourceReferenceExpression querySource)
		{
			var fromClause = querySource.ReferencedQuerySource as MainFromClause;
			if (fromClause == null)
				return null;

			var subQuery = fromClause.FromExpression as SubQueryExpression;
			if (subQuery == null)
				return null;

			return subQuery.QueryModel.SelectClause.Selector as NewExpression;
		}
	}
}