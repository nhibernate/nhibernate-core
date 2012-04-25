using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.GroupJoin
{
	public class LocateGroupJoinQuerySource : NhExpressionTreeVisitor
	{
		private readonly IsAggregatingResults _results;
		private GroupJoinClause _groupJoin;

		public LocateGroupJoinQuerySource(IsAggregatingResults results)
		{
			_results = results;
		}

		public GroupJoinClause Detect(Expression expression)
		{
			VisitExpression(expression);
			return _groupJoin;
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			var groupJoinClause = expression.ReferencedQuerySource as GroupJoinClause;
			if (groupJoinClause != null && _results.AggregatingClauses.Contains(groupJoinClause))
			{
				_groupJoin = groupJoinClause;
			}

			return base.VisitQuerySourceReferenceExpression(expression);
		}
	}
}