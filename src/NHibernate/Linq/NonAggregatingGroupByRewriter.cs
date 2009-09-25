using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq
{
    public class NonAggregatingGroupByRewriter
    {
        public void ReWrite(QueryModel queryModel)
        {
            var subQueryExpression = queryModel.MainFromClause.FromExpression as SubQueryExpression;

            if ((subQueryExpression != null) &&
                (subQueryExpression.QueryModel.ResultOperators.Count() == 1) &&
                (subQueryExpression.QueryModel.ResultOperators[0] is GroupResultOperator) &&
                (IsNonAggregatingGroupBy(queryModel)))
            {
                FlattenSubQuery(subQueryExpression, queryModel.MainFromClause, queryModel);
            }
        }

        private void FlattenSubQuery(SubQueryExpression subQueryExpression, MainFromClause fromClause,
                                     QueryModel queryModel)
        {
            // Create a new client-side select for the outer
            // TODO - don't like calling GetGenericArguments here...
            var clientSideSelect = new ClientSideSelect(new NonAggregatingGroupBySelectRewriter().Visit(queryModel.SelectClause.Selector, subQueryExpression.Type.GetGenericArguments()[0], queryModel.MainFromClause));

            // Replace the outer select clause...
            queryModel.SelectClause = subQueryExpression.QueryModel.SelectClause;

            MainFromClause innerMainFromClause = subQueryExpression.QueryModel.MainFromClause;

            CopyFromClauseData(innerMainFromClause, fromClause);

            foreach (var bodyClause in subQueryExpression.QueryModel.BodyClauses)
            {
                queryModel.BodyClauses.Add(bodyClause);
            }

            // Move the result operator up 
            if (queryModel.ResultOperators.Count != 0)
            {
                throw new NotImplementedException();
            }

            queryModel.ResultOperators.Add(new NonAggregatingGroupBy((GroupResultOperator) subQueryExpression.QueryModel.ResultOperators[0]));
            queryModel.ResultOperators.Add(clientSideSelect);
        }

        protected void CopyFromClauseData(FromClauseBase source, FromClauseBase destination)
        {
            destination.FromExpression = source.FromExpression;
            destination.ItemName = source.ItemName;
            destination.ItemType = source.ItemType;
        }

        private static bool IsNonAggregatingGroupBy(QueryModel queryModel)
        {
            return new GroupByAggregateDetectionVisitor().Visit(queryModel.SelectClause.Selector) == false;
        }
    }

    internal class NonAggregatingGroupBySelectRewriter : NhExpressionTreeVisitor
    {
        private ParameterExpression _inputParameter;
        private IQuerySource _querySource;

        public LambdaExpression Visit(Expression clause, System.Type resultType, IQuerySource querySource)
        {
            _inputParameter = Expression.Parameter(resultType, "inputParameter");
            _querySource = querySource;

            return Expression.Lambda(VisitExpression(clause), _inputParameter);
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            if (expression.ReferencedQuerySource == _querySource)
            { 
                return _inputParameter; 
            }
            
            return expression;
        }
    }

    internal class ClientSideSelect : ClientSideTransformOperator
    {
        public LambdaExpression SelectClause { get; private set; }

        public ClientSideSelect(LambdaExpression selectClause)
        {
            SelectClause = selectClause;
        }
    }
}