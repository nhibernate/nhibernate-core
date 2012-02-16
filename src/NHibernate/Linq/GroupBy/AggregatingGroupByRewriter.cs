using System;
using System.Linq;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.GroupBy
{
	/// <summary>
	/// An AggregatingGroupBy is a query such as:
	/// <code>
	///		from p in db.Products
	///		group p by p.Category.CategoryId
	///		into g
	///		select new
	///		{
	///			g.Key,
	///			MaxPrice = g.Max(p => p.UnitPrice)
	///		};
	/// </code>
	/// <para>
	/// Where the grouping operation is being fully aggregated and hence does not create any form of hierarchy.
	/// This class takes such queries, flattens out the re-linq sub-query and re-writes the outer select
	/// </para>
	/// </summary>
	public class AggregatingGroupByRewriter
	{
		private AggregatingGroupByRewriter() { }

		public static void ReWrite(QueryModel queryModel)
		{
			var subQueryExpression = queryModel.MainFromClause.FromExpression as SubQueryExpression;

			if ((subQueryExpression != null) &&
				(subQueryExpression.QueryModel.ResultOperators.Count() == 1) &&
				(subQueryExpression.QueryModel.ResultOperators[0] is GroupResultOperator))
			{
				FlattenSubQuery(subQueryExpression, queryModel);
			}
		}

		private static void FlattenSubQuery(SubQueryExpression subQueryExpression, QueryModel queryModel)
		{
			// Move the result operator up 
			if (queryModel.ResultOperators.Count != 0)
			{
				throw new NotImplementedException();
			}

			var groupBy = (GroupResultOperator) subQueryExpression.QueryModel.ResultOperators[0];

			queryModel.ResultOperators.Add(groupBy);

			for (int i = 0; i < queryModel.BodyClauses.Count; i++)
			{
				var clause = queryModel.BodyClauses[i];
				clause.TransformExpressions(s => GroupBySelectClauseRewriter.ReWrite(s, groupBy, subQueryExpression.QueryModel));

				//all outer where clauses actually are having clauses
				var whereClause = clause as WhereClause;
				if (whereClause != null)
				{
					queryModel.BodyClauses.RemoveAt(i);
					queryModel.BodyClauses.Insert(i, new NhHavingClause(whereClause.Predicate));
				}
			}

			foreach (var bodyClause in subQueryExpression.QueryModel.BodyClauses)
			{
				queryModel.BodyClauses.Add(bodyClause);
			}

			// Replace the outer select clause...
			queryModel.SelectClause.TransformExpressions(s => 
				GroupBySelectClauseRewriter.ReWrite(s, groupBy, subQueryExpression.QueryModel));

			// Point all query source references to the outer from clause
			queryModel.TransformExpressions(s =>
				new SwapQuerySourceVisitor(queryModel.MainFromClause, subQueryExpression.QueryModel.MainFromClause).Swap(s));

			// Replace the outer query source
			queryModel.MainFromClause = subQueryExpression.QueryModel.MainFromClause;
		}
	}
}