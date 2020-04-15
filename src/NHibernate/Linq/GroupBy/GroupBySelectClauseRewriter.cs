using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.GroupBy
{
	//This should be renamed. It handles entire querymodels, not just select clauses
	internal class GroupBySelectClauseRewriter : RelinqExpressionVisitor
	{
		private readonly static HashSet<ExpressionType> _validElementSelectorTypes = new HashSet<ExpressionType>
		{
			ExpressionType.ArrayIndex,
			ExpressionType.New,
			ExpressionType.MemberInit
		};

		public static Expression ReWrite(Expression expression, GroupResultOperator groupBy, QueryModel model)
		{
			var visitor = new GroupBySelectClauseRewriter(groupBy, model);
			return TransparentIdentifierRemovingExpressionVisitor.ReplaceTransparentIdentifiers(visitor.Visit(expression));
		}

		private readonly GroupResultOperator _groupBy;
		private readonly QueryModel _model;
		private readonly Expression _nominatedKeySelector;

		private GroupBySelectClauseRewriter(GroupResultOperator groupBy, QueryModel model)
		{
			_groupBy = groupBy;
			_model = model;
			_nominatedKeySelector = GroupKeyNominator.Visit(groupBy);
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			if (!IsMemberOfModel(expression))
			{
				return base.VisitQuerySourceReference(expression);
			}

			if (expression.IsGroupingElementOf(_groupBy))
			{
				return _groupBy.ElementSelector;
			}

			return base.VisitQuerySourceReference(expression);
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			if (!IsMemberOfModel(expression))
			{
				return base.VisitMember(expression);
			}

			if (expression.IsGroupingKeyOf(_groupBy))
			{
				// If we have referenced the Key, then return the nominated key expression
				return _nominatedKeySelector;
			}

			var elementSelector = _groupBy.ElementSelector;

			if ((elementSelector is MemberExpression) || (elementSelector is QuerySourceReferenceExpression))
			{
				// If ElementSelector is MemberExpression, just return
				return base.VisitMember(expression);
			}

			if (_validElementSelectorTypes.Contains(UnwrapUnary(elementSelector).NodeType) &&
				elementSelector.Type == expression.Expression.Type)
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

			return IsMemberOfModel(querySourceRef);
		}
		
		private bool IsMemberOfModel(QuerySourceReferenceExpression expression)
		{
			var fromClause = expression.ReferencedQuerySource as FromClauseBase;

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

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			//If the subquery is a Count(*) aggregate with a condition
			if (expression.QueryModel.MainFromClause.FromExpression.Type == _groupBy.ItemType)
			{
				var where = expression.QueryModel.BodyClauses.OfType<WhereClause>().FirstOrDefault();
				if (where != null &&
				    expression.QueryModel.SelectClause.Selector is NhCountExpression countExpression &&
				    countExpression.Expression is NhStarExpression)
				{
					//return it as a CASE [column] WHEN [predicate] THEN 1 ELSE NULL END
					return
						countExpression.CreateNew(
							Expression.Condition(
								where.Predicate,
								Expression.Constant(1, typeof(int?)),
								Expression.Constant(null, typeof(int?))));
				}
			}

			//In the subquery body clauses, references to the grouping key should be restored to the KeySelector expression. NOT the resolved value.
			//This feels a bit backwards, but solving it here is probably a smaller operation than fixing the previous rewriting
			if (expression.QueryModel.BodyClauses.Any())
			{
				foreach (var bodyClause in expression.QueryModel.BodyClauses)
				{
					bodyClause.TransformExpressions((e) => new KeySelectorVisitor(_groupBy).Visit(e));
				}
				return base.VisitSubQuery(expression);
			}

			// TODO - is this safe?  All we are extracting is the select clause from the sub-query.  Assumes that everything
			// else in the subquery has been removed.  If there were two subqueries, one aggregating & one not, this may not be a 
			// valid assumption.  Should probably be passed a list of aggregating subqueries that we are flattening so that we can check...
			return ReWrite(expression.QueryModel.SelectClause.Selector, _groupBy, _model);
		}

		private static Expression UnwrapUnary(Expression expression)
		{
			return expression is UnaryExpression unaryExpression
				? unaryExpression.Operand
				: expression;
		}
	}
}
