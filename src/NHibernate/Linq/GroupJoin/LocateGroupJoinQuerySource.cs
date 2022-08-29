using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.GroupJoin
{
	public class LocateGroupJoinQuerySource : RelinqExpressionVisitor
	{
		private readonly IsAggregatingResults _results;
		private GroupJoinClause _groupJoin;

		public LocateGroupJoinQuerySource(IsAggregatingResults results)
		{
			_results = results;
		}

		public GroupJoinClause Detect(Expression expression)
		{
			Visit(expression);
			return _groupJoin;
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			var groupJoinClause = expression.ReferencedQuerySource as GroupJoinClause;
			if (groupJoinClause != null && _results.AggregatingClauses.Contains(groupJoinClause))
			{
				_groupJoin = groupJoinClause;
			}

			return base.VisitQuerySourceReference(expression);
		}
	}
}
