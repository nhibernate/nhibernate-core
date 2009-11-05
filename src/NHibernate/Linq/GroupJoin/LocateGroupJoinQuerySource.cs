using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;

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
			if (_results.AggregatingClauses.Contains(expression.ReferencedQuerySource as GroupJoinClause))
			{
				_groupJoin = expression.ReferencedQuerySource as GroupJoinClause;
			}

			return base.VisitQuerySourceReferenceExpression(expression);
		}
	}
}