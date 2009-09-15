using System;
using System.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq
{
    public class AggregatingGroupByRewriter : QueryModelVisitorBase
    {
        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            var subQueryExpression = fromClause.FromExpression as SubQueryExpression;

            if ((subQueryExpression != null) &&
                (subQueryExpression.QueryModel.ResultOperators.Count() == 1) &&
                (subQueryExpression.QueryModel.ResultOperators[0] is GroupResultOperator) &&
                (IsAggregatingGroupBy(queryModel)))
            {
                FlattenSubQuery(subQueryExpression, fromClause, queryModel);
            }

            base.VisitMainFromClause(fromClause, queryModel);
        }

        private static bool IsAggregatingGroupBy(QueryModel queryModel)
        {
            return new AggregateDetectionVisitor().Visit(queryModel.SelectClause.Selector);
        }

        private void FlattenSubQuery(SubQueryExpression subQueryExpression, FromClauseBase fromClause,
                                     QueryModel queryModel)
        {
            // Replace the outer select clause...
            queryModel.SelectClause.TransformExpressions(GroupBySelectClauseVisitor.Visit);

            MainFromClause innerMainFromClause = subQueryExpression.QueryModel.MainFromClause;
            CopyFromClauseData(innerMainFromClause, fromClause);

            // Move the result operator up 
            if (queryModel.ResultOperators.Count != 0)
            {
                throw new NotImplementedException();
            }

            queryModel.ResultOperators.Add(subQueryExpression.QueryModel.ResultOperators[0]);
        }

        protected void CopyFromClauseData(FromClauseBase source, FromClauseBase destination)
        {
            destination.FromExpression = source.FromExpression;
            destination.ItemName = source.ItemName;
            destination.ItemType = source.ItemType;
        }
    }
}