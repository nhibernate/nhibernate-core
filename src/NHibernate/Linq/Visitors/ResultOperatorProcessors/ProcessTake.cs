using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessTake : IResultOperatorProcessor<TakeResultOperator>
    {
        public void Process(TakeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            NamedParameter parameterName;

            // TODO - very similar to ProcessSkip, plus want to investigate the scenario in the "else"
            // clause to see if it is valid
            if (queryModelVisitor.VisitorParameters.ConstantToParameterMap.TryGetValue(resultOperator.Count as ConstantExpression, out parameterName))
            {
                tree.AddAdditionalCriteria((q, p) => q.SetMaxResults((int)p[parameterName.Name].First));
            }
            else
            {
                tree.AddAdditionalCriteria((q, p) => q.SetMaxResults(resultOperator.GetConstantCount()));
            }
        }
    }
}