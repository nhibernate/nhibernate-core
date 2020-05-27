using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Linq.Clauses;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors
{
	internal class QueryModelKeyVisitor : NhQueryModelVisitorBase, INhQueryModelVisitorExtended
	{
		private static readonly QueryModel DefaultQueryModel = new QueryModel(
			new MainFromClause(
				"x",
				typeof(QueryModelVisitor),
				Expression.Constant(0)),
			new SelectClause(Expression.Constant(0)));

		private readonly ExpressionKeyVisitor _keyVisitor;
		private readonly StringBuilder _string;
		private HashSet<IQuerySource> _processedSources;

		public QueryModelKeyVisitor(ExpressionKeyVisitor keyVisitor, StringBuilder stringBuilder)
		{
			_keyVisitor = keyVisitor;
			_string = stringBuilder;
		}

		public override void VisitQueryModel(QueryModel queryModel)
		{
			if (queryModel.IsIdentityQuery())
			{
				_keyVisitor.Visit(queryModel.MainFromClause.FromExpression);
			}
			else
			{
				VisitMainFromClause(queryModel.MainFromClause, queryModel);
				VisitBodyClauses(queryModel.BodyClauses, queryModel);
				VisitSelectClause(queryModel.SelectClause, queryModel);
			}

			VisitResultOperators(queryModel.ResultOperators, queryModel);
		}

		public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
		{
			VisitJoinClause(groupJoinClause.JoinClause, queryModel, index);
			_string.Append(" into ");
			_string.Append(groupJoinClause.ItemType.Name);
			_string.Append(" ");
			_string.Append(groupJoinClause.ItemName);
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			VisitJoin(joinClause.ItemType, joinClause.ItemName, joinClause.InnerSequence);
			_string.Append(" on ");
			_keyVisitor.Visit(joinClause.OuterKeySelector);
			_string.Append(" equals ");
			_keyVisitor.Visit(joinClause.InnerKeySelector);
		}

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			VisitFromClauseBase(fromClause);
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			VisitFromClauseBase(fromClause);
		}

		public override void VisitNhHavingClause(NhHavingClause havingClause, QueryModel queryModel, int index)
		{
			_string.Append(" having ");
			_keyVisitor.Visit(havingClause.Predicate);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			_string.Append(" select ");
			_keyVisitor.Visit(selectClause.Selector);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			_string.Append(" where ");
			_keyVisitor.Visit(whereClause.Predicate);
		}

		public override void VisitNhJoinClause(NhJoinClause joinClause, QueryModel queryModel, int index)
		{
			VisitJoin(joinClause.ItemType, joinClause.ItemName, joinClause.FromExpression);
		}

		public void VisitNhOuterJoinClause(NhOuterJoinClause nhOuterJoinClause, QueryModel queryModel, int index)
		{
			_string.Append(" outer");
			VisitJoinClause(nhOuterJoinClause.JoinClause, queryModel, index);
		}

		public override void VisitNhWithClause(NhWithClause nhWhereClause, QueryModel queryModel, int index)
		{
			_string.Append(" with ");
			_keyVisitor.Visit(nhWhereClause.Predicate);
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			_string.Append(" orderby ");
			base.VisitOrderByClause(orderByClause, queryModel, index);
		}

		public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
		{
			if (index > 0)
			{
				_string.Append(',');
			}

			_keyVisitor.Visit(ordering.Expression);
			_string.Append(ordering.OrderingDirection == OrderingDirection.Asc ? " asc" : " desc");
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			_string.Append(" => ");

			// Custom visitors for operators that do not expose all information with TransformExpressions method in order to mimic their ToString method
			switch (resultOperator)
			{
				case CastResultOperator castResult:
					VisitTypeChangeOperator("Cast", castResult.CastItemType);
					break;
				case FetchRequestBase fetchBase:
					VisitFetchRequestBase(fetchBase);
					break;
				case ChoiceResultOperatorBase operatorBase:
					VisitChoiceResultOperatorBase(operatorBase);
					break;
				case OfTypeResultOperator ofTypeResult:
					VisitTypeChangeOperator("OfType", ofTypeResult.SearchedItemType);
					break;
				default:
					VisitResultOperatorBase(resultOperator);
					break;
			}
		}

		public void VisitQuerySource(IQuerySource querySource)
		{
			if (!AddReferencedQuerySource(querySource))
			{
				_string.Append(querySource.ItemName);
				return;
			}

			switch (querySource)
			{
				case MainFromClause mainFromClause:
					VisitMainFromClause(mainFromClause, DefaultQueryModel);
					break;
				case ResultOperatorBase resultOperator:
					VisitResultOperator(resultOperator, DefaultQueryModel, 0);
					break;
				case NhClauseBase nhClauseBase:
					nhClauseBase.Accept(this, DefaultQueryModel, 0);
					break;
				case IBodyClause bodyClause:
					bodyClause.Accept(this, DefaultQueryModel, 0);
					break;
				default:
					throw new NotSupportedException($"Unknown query source {querySource}");
			}
		}

		private void VisitResultOperatorBase(ResultOperatorBase resultOperator)
		{
			_string.Append(resultOperator.GetType().Name.Replace("ResultOperator", "("));
			var index = 0;
			resultOperator.TransformExpressions(
				expression =>
				{
					if (expression == null)
					{
						return null;
					}

					if (index > 0)
					{
						_string.Append(',');
					}

					_keyVisitor.Visit(expression);
					index++;

					return expression;
				});

			_string.Append(')');
		}

		private void VisitChoiceResultOperatorBase(ChoiceResultOperatorBase operatorBase)
		{
			_string.Append(operatorBase.GetType().Name.Replace("ResultOperator", ""));
			if (operatorBase.ReturnDefaultWhenEmpty)
			{
				_string.Append("OrDefault");
			}

			_string.Append("()");
		}

		private void VisitFetchRequestBase(FetchRequestBase fetchBase)
		{
			_string.Append("Fetch (");
			_string.Append(ExpressionKeyVisitor.GetTypeName(fetchBase.RelationMember.DeclaringType));
			_string.Append('.');
			_string.Append(fetchBase.RelationMember.Name);
			_string.Append(')');

			foreach (var innerFetch in fetchBase.InnerFetchRequests)
			{
				VisitFetchRequestBase(innerFetch);
			}
		}

		private void VisitTypeChangeOperator(string name, System.Type type)
		{
			_string.Append(name);
			_string.Append('<');
			_string.Append(ExpressionKeyVisitor.GetTypeName(type));
			_string.Append(">()");
		}

		private void VisitJoin(System.Type itemType, string itemName, Expression expression)
		{
			_string.Append(" join ");
			_string.Append(itemType.Name);
			_string.Append(" ");
			_string.Append(itemName);
			_string.Append(" in ");
			_keyVisitor.Visit(expression);
		}

		private void VisitFromClauseBase(FromClauseBase fromClause)
		{
			_string.Append(" from ");
			_string.Append(fromClause.ItemType.Name);
			_string.Append(" ");
			_string.Append(fromClause.ItemName);
			_string.Append(" in ");
			_keyVisitor.Visit(fromClause.FromExpression);
		}

		private bool AddReferencedQuerySource(IQuerySource querySource)
		{
			if (_processedSources == null)
			{
				_processedSources = new HashSet<IQuerySource>();
			}

			return _processedSources.Add(querySource);
		}
	}
}
