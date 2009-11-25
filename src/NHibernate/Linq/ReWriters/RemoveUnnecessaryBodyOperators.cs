using System;
using System.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;

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