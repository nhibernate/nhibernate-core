using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors;

namespace NHibernate.Linq.Visitors
{
	internal class PagingRewriterSelectClauseVisitor : RelinqExpressionVisitor
	{
		private readonly FromClauseBase querySource;

		public PagingRewriterSelectClauseVisitor(FromClauseBase querySource)
		{
			this.querySource = querySource;
		}

		public Expression Swap(Expression expression)
		{
			return TransparentIdentifierRemovingExpressionVisitor.ReplaceTransparentIdentifiers(Visit(expression));
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			var innerSelector = GetSubQuerySelectorOrNull(expression);
			if (innerSelector != null)
			{
				return Visit(innerSelector);
			}

			return base.VisitQuerySourceReference(expression);
		}

		/// <summary>
		/// If the querySource is a subquery, return the SelectClause's selector if it's
		/// NewExpression. Otherwise, return null.
		/// </summary>
		private Expression GetSubQuerySelectorOrNull(QuerySourceReferenceExpression expression)
		{
			if (expression.ReferencedQuerySource != querySource)
				return null;

			var fromClause = expression.ReferencedQuerySource as FromClauseBase;
			if (fromClause == null)
				return null;

			var subQuery = fromClause.FromExpression as SubQueryExpression;
			if (subQuery == null)
				return null;

			return subQuery.QueryModel.SelectClause.Selector;
		}
	}
}
