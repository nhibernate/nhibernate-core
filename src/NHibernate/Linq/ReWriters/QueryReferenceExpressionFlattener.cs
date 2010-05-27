using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq.ReWriters
{
	public class QueryReferenceExpressionFlattener : NhExpressionTreeVisitor
	{
	    private readonly QueryModel _model;

	    private QueryReferenceExpressionFlattener(QueryModel model)
		{
		    _model = model;
		}

	    public static void ReWrite(QueryModel model)
		{
			var visitor = new QueryReferenceExpressionFlattener(model);
			model.TransformExpressions(visitor.VisitExpression);
		}

        protected override Expression VisitSubQueryExpression(SubQueryExpression subQuery)
        {
            if ((subQuery.QueryModel.BodyClauses.Count == 0) &&
                ((subQuery.QueryModel.ResultOperators.Count == 0) || (subQuery.QueryModel.ResultOperators.Count == 1 && subQuery.QueryModel.ResultOperators[0] is CacheableResultOperator))
                )
            {
                var selectQuerySource =
                    subQuery.QueryModel.SelectClause.Selector as QuerySourceReferenceExpression;

                if (selectQuerySource != null && selectQuerySource.ReferencedQuerySource == subQuery.QueryModel.MainFromClause)
                {
                    if (subQuery.QueryModel.ResultOperators.Count == 1)
                    {
                        _model.ResultOperators.Add(subQuery.QueryModel.ResultOperators[0]);
                    }

                    return subQuery.QueryModel.MainFromClause.FromExpression;
                }
            }

            return base.VisitSubQueryExpression(subQuery);
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			var fromClauseBase = expression.ReferencedQuerySource as FromClauseBase;

			if (fromClauseBase != null && 
			    fromClauseBase.FromExpression is QuerySourceReferenceExpression &&
			    expression.Type == fromClauseBase.FromExpression.Type)
			{
				return fromClauseBase.FromExpression;
			}

		    return base.VisitQuerySourceReferenceExpression(expression);
		}
	}
}