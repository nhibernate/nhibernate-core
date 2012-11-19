using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors
{
	public class PagingRewriterSelectClauseVisitor : ExpressionTreeVisitor
	{
		public Expression Swap(Expression expression)
		{
			return VisitExpression(expression);
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			// If a subquery projects to e.g. anonymous type, and the outer
			// query projects again to a different type, the property in the MemberExpression
			// will refer to a property on the type returned by the subquery. When the from
			// clauses are swapped, Relinq will attempt to apply the property to the type of the
			// from clause in the subquery, which will be a different type and cause exception.

			// So replace the MemberExpression so that it applies to the inner projection
			// directly. Then optimize that in Swap() with TransparentIdentifierRemovingExpressionTreeVisitor.

			var querySource = expression.Expression as QuerySourceReferenceExpression;
			if (querySource != null)
			{
				var fromClause = querySource.ReferencedQuerySource as MainFromClause;
				if (fromClause != null)
				{
					var subQuery = fromClause.FromExpression as SubQueryExpression;
					if (subQuery != null)
					{
						var innerSelector = subQuery.QueryModel.SelectClause.Selector as NewExpression;
						if (innerSelector != null)
						{
							var access = Expression.MakeMemberAccess(innerSelector, expression.Member);
							return TransparentIdentifierRemovingExpressionTreeVisitor.ReplaceTransparentIdentifiers(access);
						}
					}
				}
			}

			return base.VisitMemberExpression(expression);
		}
	}
}