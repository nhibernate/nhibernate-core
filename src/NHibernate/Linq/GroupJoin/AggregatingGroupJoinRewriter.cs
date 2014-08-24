using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.GroupJoin
{
	/// <summary>
	/// An AggregatingGroupJoin is a query such as:
	/// 
	///	   from c in db.Customers
	///    join o in db.Orders on c.CustomerId equals o.Customer.CustomerId into ords
	///    join e in db.Employees on c.Address.City equals e.Address.City into emps
	///    select new { c.ContactName, ords = ords.Count(), emps = emps.Count() };
	/// 
	/// where the results of the joins are being fully aggregated and hence do not create any form of hierarchy.
	/// This class takes such expressions and turns them into this form:
	/// 
	///	   from c in db.Customers
	///    select new
	///    {
	///        c.ContactName,
	///        ords = (from o2 in db.Orders where o2.Customer.CustomerId == c.CustomerId select o2).Count(),
	///        emps = (from e2 in db.Employees where e2.Address.City == c.Address.City select e2).Count()
	///    };
	/// 
	/// </summary>
	public static class AggregatingGroupJoinRewriter
	{
		public static void ReWrite(QueryModel model)
		{
			// firstly, get the group join clauses
			var groupJoin = model.BodyClauses.OfType<GroupJoinClause>();

			if (!groupJoin.Any())
			{
				// No group join here..
				return;
			}

			// Now walk the tree to decide which groupings are fully aggregated (and can hence be done in hql)
			var aggregateDetectorResults = IsAggregatingGroupJoin(model, groupJoin);

			if (aggregateDetectorResults.AggregatingClauses.Count > 0)
			{
				// Re-write the select expression
				model.SelectClause.TransformExpressions(s => GroupJoinSelectClauseRewriter.ReWrite(s, aggregateDetectorResults));

				// Remove the aggregating group joins
				foreach (var aggregatingGroupJoin in aggregateDetectorResults.AggregatingClauses)
				{
					model.BodyClauses.Remove(aggregatingGroupJoin);
				}
			}
		}

		private static IsAggregatingResults IsAggregatingGroupJoin(QueryModel model, IEnumerable<GroupJoinClause> clause)
		{
			return GroupJoinAggregateDetectionVisitor.Visit(clause, model.SelectClause.Selector);
		}
	}
}