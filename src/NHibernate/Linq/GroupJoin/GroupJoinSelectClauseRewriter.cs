using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.GroupJoin
{
	public class GroupJoinSelectClauseRewriter : ExpressionTreeVisitor
	{
		private readonly IsAggregatingResults _results;

		public static Expression ReWrite(Expression expression, IsAggregatingResults results)
		{
			return new GroupJoinSelectClauseRewriter(results).VisitExpression(expression);
		}

		private GroupJoinSelectClauseRewriter(IsAggregatingResults results)
		{
			_results = results;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			// If the sub query's main (and only) from clause is one of our aggregating group bys, then swap it
			GroupJoinClause groupJoin = LocateGroupJoinQuerySource(expression.QueryModel);

			if (groupJoin != null)
			{
				Expression innerSelector = new SwapQuerySourceVisitor(groupJoin.JoinClause, expression.QueryModel.MainFromClause).
					Swap(groupJoin.JoinClause.InnerKeySelector);

				expression.QueryModel.MainFromClause.FromExpression = groupJoin.JoinClause.InnerSequence;


				// TODO - this only works if the key selectors are not composite.  Needs improvement...
				expression.QueryModel.BodyClauses.Add(new WhereClause(Expression.Equal(innerSelector, groupJoin.JoinClause.OuterKeySelector)));
			}

			return expression;
		}

		private GroupJoinClause LocateGroupJoinQuerySource(QueryModel model)
		{
			if (model.BodyClauses.Count > 0)
			{
				return null;
			}
			return new LocateGroupJoinQuerySource(_results).Detect(model.MainFromClause.FromExpression);
		}
	}
}