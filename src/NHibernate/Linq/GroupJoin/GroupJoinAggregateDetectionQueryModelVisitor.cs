using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.GroupJoin
{
	internal class GroupJoinAggregateDetectionQueryModelVisitor : NhQueryModelVisitorBase
	{
		private readonly GroupJoinAggregateDetectionVisitor _groupJoinAggregateDetectionVisitor;

		private GroupJoinAggregateDetectionQueryModelVisitor(IEnumerable<GroupJoinClause> groupJoinClauses)
		{
			_groupJoinAggregateDetectionVisitor = new GroupJoinAggregateDetectionVisitor(groupJoinClauses);
		}

		public static IsAggregatingResults Visit(IEnumerable<GroupJoinClause> groupJoinClause, QueryModel queryModel)
		{
			var visitor = new GroupJoinAggregateDetectionQueryModelVisitor(groupJoinClause);

			visitor.VisitQueryModel(queryModel);

			return visitor._groupJoinAggregateDetectionVisitor.GetResults();
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			_groupJoinAggregateDetectionVisitor.Visit(whereClause.Predicate);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			_groupJoinAggregateDetectionVisitor.Visit(selectClause.Selector);
		}

		public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
		{
			_groupJoinAggregateDetectionVisitor.Visit(ordering.Expression);
		}
	}
}
