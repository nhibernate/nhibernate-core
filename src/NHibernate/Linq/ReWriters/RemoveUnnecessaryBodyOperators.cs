using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.ReWriters
{
    public class RemoveUnnecessaryBodyOperators : QueryModelVisitorBase
    {
        private RemoveUnnecessaryBodyOperators()
        {
        }

        public static void ReWrite(QueryModel queryModel)
        {
            var rewriter = new RemoveUnnecessaryBodyOperators();

            rewriter.VisitQueryModel(queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (resultOperator is CountResultOperator)
            {
                // For count operators, we can remove any order-by result operators
                foreach (var orderby in queryModel.BodyClauses.Where(bc => bc is OrderByClause).ToList())
                {
                    queryModel.BodyClauses.Remove(orderby);
                }
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }
    }
}