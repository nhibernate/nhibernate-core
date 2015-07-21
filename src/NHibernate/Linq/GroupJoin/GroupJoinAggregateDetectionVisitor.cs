using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.GroupJoin
{
	internal class GroupJoinAggregateDetectionVisitor : NhExpressionTreeVisitor
	{
		private readonly HashSet<GroupJoinClause> _groupJoinClauses;
		private readonly StackFlag _inAggregate = new StackFlag();
		private readonly StackFlag _parentExpressionProcessed = new StackFlag();

		private readonly List<Expression> _nonAggregatingExpressions = new List<Expression>();
		private readonly List<GroupJoinClause> _nonAggregatingGroupJoins = new List<GroupJoinClause>();
		private readonly List<GroupJoinClause> _aggregatingGroupJoins = new List<GroupJoinClause>();

		private GroupJoinAggregateDetectionVisitor(IEnumerable<GroupJoinClause> groupJoinClause)
		{
			_groupJoinClauses = new HashSet<GroupJoinClause>(groupJoinClause);
		}

		public static IsAggregatingResults Visit(IEnumerable<GroupJoinClause> groupJoinClause, Expression selectExpression)
		{
			var visitor = new GroupJoinAggregateDetectionVisitor(groupJoinClause);

			visitor.VisitExpression(selectExpression);

			return new IsAggregatingResults { NonAggregatingClauses = visitor._nonAggregatingGroupJoins, AggregatingClauses = visitor._aggregatingGroupJoins, NonAggregatingExpressions = visitor._nonAggregatingExpressions };
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			VisitExpression(expression.QueryModel.SelectClause.Selector);
			return expression;
		}

		protected override Expression VisitNhAggregate(NhAggregatedExpression expression)
		{
			using (_inAggregate.SetFlag())
			{
				return base.VisitNhAggregate(expression);
			}
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			if (_inAggregate.FlagIsFalse && _parentExpressionProcessed.FlagIsFalse)
			{
				_nonAggregatingExpressions.Add(expression);
			}

			using (_parentExpressionProcessed.SetFlag())
			{
				return base.VisitMemberExpression(expression);
			}
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			var fromClause = (FromClauseBase) expression.ReferencedQuerySource;

			var querySourceReference = fromClause.FromExpression as QuerySourceReferenceExpression;
			if (querySourceReference != null)
			{
				var groupJoinClause = querySourceReference.ReferencedQuerySource as GroupJoinClause;
				if (groupJoinClause != null && _groupJoinClauses.Contains(groupJoinClause))
				{
					if (_inAggregate.FlagIsFalse)
					{
						_nonAggregatingGroupJoins.Add(groupJoinClause);
					}
					else
					{
						_aggregatingGroupJoins.Add(groupJoinClause);
					}
				}
			}

			return base.VisitQuerySourceReferenceExpression(expression);
		}

		internal class StackFlag
		{
			public bool FlagIsTrue { get; private set; }

			public bool FlagIsFalse { get { return !FlagIsTrue; } }

			public IDisposable SetFlag()
			{
				return new StackFlagDisposable(this);
			}

			internal class StackFlagDisposable : IDisposable
			{
				private readonly StackFlag _parent;
				private readonly bool _old;

				public StackFlagDisposable(StackFlag parent)
				{
					_parent = parent;
					_old = parent.FlagIsTrue;
					parent.FlagIsTrue = true;
				}

				public void Dispose()
				{
					_parent.FlagIsTrue = _old;
				}
			}
		}
	}
}