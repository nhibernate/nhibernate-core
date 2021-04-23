using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.GroupJoin
{
	internal class GroupJoinAggregateDetectionQueryModelVisitor : NhQueryModelVisitorBase
	{
		private HashSet<GroupJoinClause> _groupJoinClauses;
		private readonly List<Expression> _nonAggregatingExpressions = new List<Expression>();
		private readonly List<GroupJoinClause> _nonAggregatingGroupJoins = new List<GroupJoinClause>();
		private readonly List<GroupJoinClause> _aggregatingGroupJoins = new List<GroupJoinClause>();
		private GroupJoinAggregateDetectionQueryModelVisitor(IEnumerable<GroupJoinClause> groupJoinClauses)
		{
			_groupJoinClauses = new HashSet<GroupJoinClause>(groupJoinClauses);
		}
		public static IsAggregatingResults Visit(IEnumerable<GroupJoinClause> groupJoinClause, QueryModel queryModel)
		{
			var visitor = new GroupJoinAggregateDetectionQueryModelVisitor(groupJoinClause);

			visitor.VisitQueryModel(queryModel);

			return new IsAggregatingResults { NonAggregatingClauses = visitor._nonAggregatingGroupJoins, AggregatingClauses = visitor._aggregatingGroupJoins, NonAggregatingExpressions = visitor._nonAggregatingExpressions };
		}
		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			var results = GroupJoinAggregateDetectionVisitor.Visit(_groupJoinClauses, whereClause.Predicate);
			AddResults(results);
		}
		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			var results = GroupJoinAggregateDetectionVisitor.Visit(_groupJoinClauses, selectClause.Selector);
			AddResults(results);
		}

		public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
		{
			var results = GroupJoinAggregateDetectionVisitor.Visit(_groupJoinClauses, ordering.Expression);
			AddResults(results);
		}

		private void AddResults(IsAggregatingResults results)
		{
			_nonAggregatingExpressions.AddRange(results.NonAggregatingExpressions);
			_nonAggregatingGroupJoins.AddRange(results.NonAggregatingClauses);
			_aggregatingGroupJoins.AddRange(results.AggregatingClauses);
		}
	}
}
