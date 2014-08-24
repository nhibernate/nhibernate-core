using System;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.GroupBy
{
	internal class GroupBySelectClauseRewriter : ExpressionTreeVisitor
	{
		public static Expression ReWrite(Expression expression, GroupResultOperator groupBy, QueryModel model)
		{
			var visitor = new GroupBySelectClauseRewriter(groupBy, model);
			return TransparentIdentifierRemovingExpressionTreeVisitor.ReplaceTransparentIdentifiers(visitor.VisitExpression(expression));
		}

		private readonly GroupResultOperator _groupBy;
		private readonly QueryModel _model;

		private GroupBySelectClauseRewriter(GroupResultOperator groupBy, QueryModel model)
		{
			_groupBy = groupBy;
			_model = model;
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			if (expression.ReferencedQuerySource == _groupBy)
			{
				return _groupBy.ElementSelector;
			}

			return base.VisitQuerySourceReferenceExpression(expression);
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			if (!IsMemberOfModel(expression))
			{
				return base.VisitMemberExpression(expression);
			}

			if (expression.Member.Name == "Key")
			{
				return _groupBy.KeySelector;
			}

			var elementSelector = _groupBy.ElementSelector;

			if ((elementSelector is MemberExpression) || (elementSelector is QuerySourceReferenceExpression))
			{
				// If ElementSelector is MemberExpression, just return
				return base.VisitMemberExpression(expression);
			}

			if (elementSelector is NewExpression && elementSelector.Type == expression.Expression.Type)
			{
				//TODO: probably we should check this with a visitor
				return Expression.MakeMemberAccess(elementSelector, expression.Member);
			}

			throw new NotImplementedException();
		}

		// TODO - dislike this code intensly.  Should probably be a tree-walk in its own right
		private bool IsMemberOfModel(MemberExpression expression)
		{
			var querySourceRef = expression.Expression as QuerySourceReferenceExpression;

			if (querySourceRef == null)
			{
				return false;
			}

			var fromClause = querySourceRef.ReferencedQuerySource as FromClauseBase;

			if (fromClause == null)
			{
				return false;
			}

			var subQuery = fromClause.FromExpression as SubQueryExpression;

			if (subQuery != null)
			{
				return subQuery.QueryModel == _model;
			}

			var referencedQuery = fromClause.FromExpression as QuerySourceReferenceExpression;

			if (referencedQuery == null)
			{
				return false;
			}

			var querySource = referencedQuery.ReferencedQuerySource as FromClauseBase;

			var subQuery2 = querySource.FromExpression as SubQueryExpression;

			return subQuery2 != null && subQuery2.QueryModel == _model;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			// TODO - is this safe?  All we are extracting is the select clause from the sub-query.  Assumes that everything
			// else in the subquery has been removed.  If there were two subqueries, one aggregating & one not, this may not be a 
			// valid assumption.  Should probably be passed a list of aggregating subqueries that we are flattening so that we can check...
			return ReWrite(expression.QueryModel.SelectClause.Selector, _groupBy, _model);
		}
	}
}