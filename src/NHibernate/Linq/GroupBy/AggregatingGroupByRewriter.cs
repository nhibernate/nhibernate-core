using System;
using System.Collections.Generic;
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

		private static readonly ICollection<System.Type> AcceptableOuterResultOperators = new HashSet<System.Type>
			{
				typeof (SkipResultOperator),
				typeof (TakeResultOperator),
				typeof (FirstResultOperator),
				typeof (SingleResultOperator),
				typeof (AnyResultOperator),
				typeof (AllResultOperator),
				typeof (TimeoutResultOperator),
			};

		public static void ReWrite(QueryModel queryModel)
		{
			var subQueryExpression = queryModel.MainFromClause.FromExpression as SubQueryExpression;

			if ((subQueryExpression != null) &&
				(subQueryExpression.QueryModel.ResultOperators.Count == 1) &&
				(subQueryExpression.QueryModel.ResultOperators[0] is GroupResultOperator))
			{
				FlattenSubQuery(queryModel, subQueryExpression.QueryModel);
			}
		}

		private static void FlattenSubQuery(QueryModel queryModel, QueryModel subQueryModel)
		{
			foreach (var resultOperator in queryModel.ResultOperators.Where(resultOperator => !AcceptableOuterResultOperators.Contains(resultOperator.GetType())))
			{
				throw new NotImplementedException("Cannot use group by with the " + resultOperator.GetType().Name + " result operator.");
			}

			// Move the result operator up.
			var groupBy = (GroupResultOperator) subQueryModel.ResultOperators[0];

			queryModel.ResultOperators.Insert(0, groupBy);

			for (int i = 0; i < queryModel.BodyClauses.Count; i++)
			{
				var clause = queryModel.BodyClauses[i];
				clause.TransformExpressions(s => GroupBySelectClauseRewriter.ReWrite(s, groupBy, subQueryModel));

				//all outer where clauses actually are having clauses
				var whereClause = clause as WhereClause;
				if (whereClause != null)
				{
					queryModel.BodyClauses.RemoveAt(i);
					queryModel.BodyClauses.Insert(i, new NhHavingClause(whereClause.Predicate));
				}
			}

			foreach (var bodyClause in subQueryModel.BodyClauses)
				queryModel.BodyClauses.Add(bodyClause);

			// Replace the outer select clause...
			queryModel.SelectClause.TransformExpressions(s => 
				GroupBySelectClauseRewriter.ReWrite(s, groupBy, subQueryModel));

			// Point all query source references to the outer from clause
			var visitor = new SwapQuerySourceVisitor(queryModel.MainFromClause, subQueryModel.MainFromClause);
			queryModel.TransformExpressions(visitor.Swap);

			// Replace the outer query source
			queryModel.MainFromClause = subQueryModel.MainFromClause;
		}
	}
}