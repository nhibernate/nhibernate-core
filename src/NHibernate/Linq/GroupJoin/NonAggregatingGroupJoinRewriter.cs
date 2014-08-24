using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.GroupJoin;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class NonAggregatingGroupJoinRewriter
	{
		private readonly QueryModel _model;
		private readonly IEnumerable<GroupJoinClause> _groupJoinClauses;
		private QuerySourceUsageLocator _locator;

		private NonAggregatingGroupJoinRewriter(QueryModel model, IEnumerable<GroupJoinClause> groupJoinClauses)
		{
			_model = model;
			_groupJoinClauses = groupJoinClauses;
		}

		public static void ReWrite(QueryModel model)
		{
			// firstly, get the group join clauses
			var clauses = model.BodyClauses.OfType<GroupJoinClause>().ToArray();
			if (!clauses.Any())
			{
				// No group join here..
				return;
			}

			new NonAggregatingGroupJoinRewriter(model, clauses).ReWrite();
		}

		private void ReWrite()
		{
			var aggregateDetectorResults = GetGroupJoinInformation(_groupJoinClauses);

			foreach (var nonAggregatingJoin in aggregateDetectorResults.NonAggregatingClauses)
			{
				// Group joins get processed (currently) in one of three ways.
				// Option 1: results of group join are not further referenced outside of the final projection.
				//           In this case, replace the group join with a join, and add a client-side grouping operator
				//		     to build the correct hierarchy
				//
				// Option 2: Results of group join are only used in a plain "from" expression, such as:
				//                from o in db.Orders
				//                from p in db.Products
				//                join d in db.OrderLines
				//                    on new {o.OrderId, p.ProductId} equals new {d.Order.OrderId, d.Product.ProductId}
				//                    into details
				//                from d in details
				//                select new {o.OrderId, p.ProductId, d.UnitPrice};
				//           In this case, simply change the group join to a join; the results of the grouping are being 
				//           removed by the subsequent "from"
				//
				// Option 3: Results of group join are only used in a "from ... DefaultIfEmpty()" construction, such as:
				//                from o in dc.Orders
				//                join v in dc.Vendors on o.VendorId equals v.Id into ov
				//                from x in ov.DefaultIfEmpty()
				//                join s in dc.Status on o.StatusId equals s.Id into os
				//                from y in os.DefaultIfEmpty()
				//                select new { o.OrderNumber, x.VendorName, y.StatusName }
				//            This is used to repesent an outer join, and again the "from" is removing the hierarchy. So
				//            simply change the group join to an outer join

				_locator = new QuerySourceUsageLocator(nonAggregatingJoin);

				foreach (var bodyClause in _model.BodyClauses)
				{
					_locator.Search(bodyClause);
				}

				if (IsHierarchicalJoin(nonAggregatingJoin))
				{
				}
				else if (IsFlattenedJoin(nonAggregatingJoin))
				{
					ProcessFlattenedJoin(nonAggregatingJoin);
				}
				else if (IsOuterJoin(nonAggregatingJoin))
				{

				}
				else
				{
					// Wonder what this is?
					throw new NotSupportedException();
				}
			}
		}

		private void ProcessFlattenedJoin(GroupJoinClause nonAggregatingJoin)
		{
			// Need to:
			// 1. Remove the group join and replace it with a join
			// 2. Remove the corresponding "from" clause (the thing that was doing the flattening)
			// 3. Rewrite the selector to reference the "join" rather than the "from" clause
			SwapClause(nonAggregatingJoin, nonAggregatingJoin.JoinClause);

			// TODO - don't like use of _locator here; would rather we got this passed in.  Ditto on next line (esp. the cast)
			_model.BodyClauses.Remove(_locator.Clauses[0]);

			var querySourceSwapper = new SwapQuerySourceVisitor((IQuerySource)_locator.Clauses[0], nonAggregatingJoin.JoinClause);
			_model.SelectClause.TransformExpressions(querySourceSwapper.Swap);
		}

		// TODO - store the indexes of the join clauses when we find them, then can remove this loop
		private void SwapClause(IBodyClause oldClause, IBodyClause newClause)
		{
			for (int i = 0; i < _model.BodyClauses.Count; i++)
			{
				if (_model.BodyClauses[i] == oldClause)
				{
					_model.BodyClauses.RemoveAt(i);
					_model.BodyClauses.Insert(i, newClause);
				}
			}
		}

		private bool IsOuterJoin(GroupJoinClause nonAggregatingJoin)
		{
			return false;
		}

		private bool IsFlattenedJoin(GroupJoinClause nonAggregatingJoin)
		{
			if (_locator.Clauses.Count == 1)
			{
				var from = _locator.Clauses[0] as AdditionalFromClause;

				if (from != null)
				{
					return true;
				}
			}

			return false;
		}

		private bool IsHierarchicalJoin(GroupJoinClause nonAggregatingJoin)
		{
			return _locator.Clauses.Count == 0;
		}

		// TODO - rename this and share with the AggregatingGroupJoinRewriter
		private IsAggregatingResults GetGroupJoinInformation(IEnumerable<GroupJoinClause> clause)
		{
			return GroupJoinAggregateDetectionVisitor.Visit(clause, _model.SelectClause.Selector);
		}

	}

	internal class QuerySourceUsageLocator : ExpressionTreeVisitor
	{
		private readonly IQuerySource _querySource;
		private bool _references;
		private readonly List<IBodyClause> _clauses = new List<IBodyClause>();

		public QuerySourceUsageLocator(IQuerySource querySource)
		{
			_querySource = querySource;
		}

		public IList<IBodyClause> Clauses
		{
			get { return _clauses.AsReadOnly(); }
		}

		public void Search(IBodyClause clause)
		{
			_references = false;

			clause.TransformExpressions(ExpressionSearcher);

			if (_references)
			{
				_clauses.Add(clause);
			}
		}

		private Expression ExpressionSearcher(Expression arg)
		{
			VisitExpression(arg);
			return arg;
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			if (expression.ReferencedQuerySource == _querySource)
			{
				_references = true;
			}

			return expression;
		}
	}
}