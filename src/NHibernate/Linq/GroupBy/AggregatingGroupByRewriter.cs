using System;
using System.Linq;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.GroupBy
{
	/// <summary>
	/// An AggregatingGroupBy is a query such as:
	/// 
	///		from p in db.Products
	///		group p by p.Category.CategoryId
	///		into g
	///		    select new
	///		               {
	///		                   g.Key,
	///		                   MaxPrice = g.Max(p => p.UnitPrice)
	///		               };
	/// 
	/// Where the grouping operation is being fully aggregated and hence does not create any form of heirarchy.
	/// This class takes such queries, flattens out the re-linq sub-query and re-writes the outer select
	/// </summary>
	public class AggregatingGroupByRewriter
	{
		private AggregatingGroupByRewriter()
		{
		}

		public static void ReWrite(QueryModel queryModel)
		{
			var subQueryExpression = queryModel.MainFromClause.FromExpression as SubQueryExpression;

			if ((subQueryExpression != null) &&
			    (subQueryExpression.QueryModel.ResultOperators.Count() == 1) &&
			    (subQueryExpression.QueryModel.ResultOperators[0] is GroupResultOperator) &&
			    (IsAggregatingGroupBy(queryModel)))
			{
				var rewriter = new AggregatingGroupByRewriter();
				rewriter.FlattenSubQuery(subQueryExpression, queryModel.MainFromClause, queryModel);
			}
		}

		private static bool IsAggregatingGroupBy(QueryModel queryModel)
		{
			return new GroupByAggregateDetectionVisitor().Visit(queryModel.SelectClause.Selector);
		}

		private void FlattenSubQuery(SubQueryExpression subQueryExpression, FromClauseBase fromClause,
		                             QueryModel queryModel)
		{
			// Move the result operator up 
			if (queryModel.ResultOperators.Count != 0)
			{
				throw new NotImplementedException();
			}

			var groupBy = (GroupResultOperator) subQueryExpression.QueryModel.ResultOperators[0];

			// Replace the outer select clause...
			queryModel.SelectClause.TransformExpressions(s => GroupBySelectClauseRewriter.ReWrite(s, groupBy, subQueryExpression.QueryModel));

			queryModel.SelectClause.TransformExpressions(
				s =>
				new SwapQuerySourceVisitor(queryModel.MainFromClause, subQueryExpression.QueryModel.MainFromClause).Swap
					(s));

			MainFromClause innerMainFromClause = subQueryExpression.QueryModel.MainFromClause;
			CopyFromClauseData(innerMainFromClause, fromClause);

			foreach (var bodyClause in subQueryExpression.QueryModel.BodyClauses)
			{
				queryModel.BodyClauses.Add(bodyClause);
			}

			queryModel.ResultOperators.Add(groupBy);
		}

		protected void CopyFromClauseData(FromClauseBase source, FromClauseBase destination)
		{
			destination.FromExpression = source.FromExpression;
			destination.ItemName = source.ItemName;
			destination.ItemType = source.ItemType;
		}
	}
}