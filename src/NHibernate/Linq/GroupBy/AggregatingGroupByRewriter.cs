using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.ReWriters;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

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
	public static class AggregatingGroupByRewriter
	{
		private static readonly ICollection<System.Type> AcceptableOuterResultOperators = new HashSet<System.Type>
			{
				typeof(SkipResultOperator),
				typeof(TakeResultOperator),
				typeof(FirstResultOperator),
				typeof(SingleResultOperator),
				typeof(AnyResultOperator),
				typeof(AllResultOperator),
				typeof(ContainsResultOperator),
			};

		public static void ReWrite(QueryModel queryModel)
		{
			if (queryModel.MainFromClause.FromExpression is SubQueryExpression subQueryExpression)
			{
				var operators = subQueryExpression.QueryModel.ResultOperators
					.Where(x => !QueryReferenceExpressionFlattener.FlattenableResultOperators.Contains(x.GetType()))
					.ToArray();

				if (operators.Length == 1)
				{
					if (operators[0] is GroupResultOperator groupBy)
					{
						FlattenSubQuery(queryModel, subQueryExpression.QueryModel, groupBy);
						RemoveCostantGroupByKeys(queryModel, groupBy);
					}
				}
			}
		}

		private static void FlattenSubQuery(QueryModel queryModel, QueryModel subQueryModel, GroupResultOperator groupBy)
		{
			foreach (var resultOperator in queryModel.ResultOperators.Where(resultOperator => !AcceptableOuterResultOperators.Contains(resultOperator.GetType())))
			{
				throw new NotImplementedException("Cannot use group by with the " + resultOperator.GetType().Name + " result operator.");
			}

			// Move the result operator up.
			SubQueryFromClauseFlattener.InsertResultOperators(subQueryModel.ResultOperators, queryModel);

			for (var i = 0; i < queryModel.BodyClauses.Count; i++)
			{
				var clause = queryModel.BodyClauses[i];
				clause.TransformExpressions(s => GroupBySelectClauseRewriter.ReWrite(s, groupBy, subQueryModel));
				if (clause is WhereClause whereClause)
				{
					queryModel.BodyClauses[i] = new NhHavingClause(whereClause.Predicate);
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

		private static void RemoveCostantGroupByKeys(QueryModel queryModel, GroupResultOperator groupBy)
		{
			var keys = groupBy.ExtractKeyExpressions().Where(x => !(x is ConstantExpression)).ToList();

			if (!keys.Any())
			{
				// Remove the Group By clause completely if all the keys are constant (redundant)
				queryModel.ResultOperators.Remove(groupBy);
			}
			else
			{
				// Re-write the KeySelector as an object array of the non-constant keys
				// This should be safe because we've already re-written the select clause using the original keys
				groupBy.KeySelector = Expression.NewArrayInit(typeof(object), keys.Select(x => x.Type.IsValueType ? Expression.Convert(x, typeof(object)) : x));
			}
		}
	}
}
