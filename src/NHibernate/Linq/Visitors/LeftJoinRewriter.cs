using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors
{
	public class LeftJoinRewriter : QueryModelVisitorBase
	{
		public static void ReWrite(QueryModel queryModel, VisitorParameters parameters)
		{
			new LeftJoinRewriter(parameters).VisitQueryModel(queryModel);
		}

		private readonly VisitorParameters _parameters;

		public LeftJoinRewriter(VisitorParameters parameters)
		{
			_parameters = parameters;
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			var subQuery = fromClause.FromExpression as SubQueryExpression;
			if (subQuery == null)
				return;

			var subQueryModel = subQuery.QueryModel;
			if (!IsLeftJoin(subQueryModel))
				return;

			var mainFromClause = subQueryModel.MainFromClause;

			var join = new AdditionalFromClause(mainFromClause.ItemName, mainFromClause.ItemType, mainFromClause.FromExpression);
			var restrictions = subQueryModel.BodyClauses.OfType<WhereClause>().ToList();
			_parameters.AddLeftJoin(join, restrictions);

			var innerSelectorMapping = new QuerySourceMapping();
			innerSelectorMapping.AddMapping(fromClause, subQueryModel.SelectClause.Selector);

			queryModel.TransformExpressions(ex => ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(ex, innerSelectorMapping, false));

			queryModel.BodyClauses.RemoveAt(index);
			queryModel.BodyClauses.Insert(index, join);
			InsertBodyClauses(subQueryModel.BodyClauses, queryModel, index + 1);

			var innerBodyClauseMapping = new QuerySourceMapping();
			innerBodyClauseMapping.AddMapping(mainFromClause, new QuerySourceReferenceExpression(join));

			queryModel.TransformExpressions(ex => ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(ex, innerBodyClauseMapping, false));
		}

		private static void InsertBodyClauses(IEnumerable<IBodyClause> bodyClauses, QueryModel destinationQueryModel, int destinationIndex)
		{
			foreach (var bodyClause in bodyClauses)
			{
				destinationQueryModel.BodyClauses.Insert(destinationIndex, bodyClause);
				destinationIndex++;
			}
		}

		private static bool IsLeftJoin(QueryModel subQueryModel)
		{
			return subQueryModel.ResultOperators.Count == 1 &&
				subQueryModel.ResultOperators[0] is DefaultIfEmptyResultOperator;
		}
	}
}