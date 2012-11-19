using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors
{
	internal class PagingRewriterSelectClauseVisitor : NhExpressionTreeVisitor
	{
		public Expression Swap(Expression expression)
		{
			return VisitExpression(expression);
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			var innerSelector = GetSubQuerySelectorOrNull(expression);
			if (innerSelector != null)
			{
				return TransparentIdentifierRemovingExpressionTreeVisitor.ReplaceTransparentIdentifiers(VisitExpression(innerSelector));
			}

			return base.VisitQuerySourceReferenceExpression(expression);
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
