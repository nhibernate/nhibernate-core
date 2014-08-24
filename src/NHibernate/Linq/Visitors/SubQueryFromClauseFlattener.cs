using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors
{
	public class SubQueryFromClauseFlattener : QueryModelVisitorBase
	{
		private static readonly System.Type[] FlattenableResultOperators = new[]
			{
				typeof (FetchOneRequest),
				typeof (FetchManyRequest),
			};

		public static void ReWrite(QueryModel queryModel)
		{
			new SubQueryFromClauseFlattener().VisitQueryModel(queryModel);
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			var subQueryExpression = fromClause.FromExpression as SubQueryExpression;
			if (subQueryExpression != null)
				FlattenSubQuery(subQueryExpression, fromClause, queryModel, index + 1);
			base.VisitAdditionalFromClause(fromClause, queryModel, index);
		}

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			var subQueryExpression = fromClause.FromExpression as SubQueryExpression;
			if (subQueryExpression != null)
				FlattenSubQuery(subQueryExpression, fromClause, queryModel, 0);
			base.VisitMainFromClause(fromClause, queryModel);
		}

		private static bool CheckFlattenable(QueryModel subQueryModel)
		{
			if (subQueryModel.BodyClauses.OfType<OrderByClause>().Any()) 
				return false;

			if (subQueryModel.ResultOperators.Count == 0) 
				return true;
			
			return HasJustAllFlattenableOperator(subQueryModel.ResultOperators);
		}

		private static bool HasJustAllFlattenableOperator(IEnumerable<ResultOperatorBase> resultOperators)
		{
			return resultOperators.All(x => FlattenableResultOperators.Contains(x.GetType()));
		}

		private static void CopyFromClauseData(FromClauseBase source, FromClauseBase destination)
		{
			destination.FromExpression = source.FromExpression;
			destination.ItemName = source.ItemName;
			destination.ItemType = source.ItemType;
		}

		private static void FlattenSubQuery(SubQueryExpression subQueryExpression, FromClauseBase fromClause, QueryModel queryModel, int destinationIndex)
		{
			if (!CheckFlattenable(subQueryExpression.QueryModel))
				return;

			var mainFromClause = subQueryExpression.QueryModel.MainFromClause;
			CopyFromClauseData(mainFromClause, fromClause);

			var innerSelectorMapping = new QuerySourceMapping();
			innerSelectorMapping.AddMapping(fromClause, subQueryExpression.QueryModel.SelectClause.Selector);
			queryModel.TransformExpressions(ex => ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(ex, innerSelectorMapping, false));

			InsertBodyClauses(subQueryExpression.QueryModel.BodyClauses, queryModel, destinationIndex);
			InsertResultOperators(subQueryExpression.QueryModel.ResultOperators, queryModel);

			var innerBodyClauseMapping = new QuerySourceMapping();
			innerBodyClauseMapping.AddMapping(mainFromClause, new QuerySourceReferenceExpression(fromClause));
			queryModel.TransformExpressions(ex => ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(ex, innerBodyClauseMapping, false));
		}

		private static void InsertResultOperators(IEnumerable<ResultOperatorBase> resultOperators, QueryModel queryModel)
		{
			var index = 0;
			foreach (var bodyClause in resultOperators)
			{
				queryModel.ResultOperators.Insert(index, bodyClause);
				++index;
			}
		}

		private static void InsertBodyClauses(IEnumerable<IBodyClause> bodyClauses, QueryModel queryModel, int destinationIndex)
		{
			foreach (var bodyClause in  bodyClauses)
			{
				queryModel.BodyClauses.Insert(destinationIndex, bodyClause);
				++destinationIndex;
			}
		}
	}
}