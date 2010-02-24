using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessSkip : IResultOperatorProcessor<SkipResultOperator>
    {
        public ProcessResultOperatorReturn Process(SkipResultOperator resultOperator, QueryModelVisitor queryModelVisitor)
        {
            NamedParameter parameterName;

            if (queryModelVisitor.VisitorParameters.ConstantToParameterMap.TryGetValue(resultOperator.Count as ConstantExpression, out parameterName))
            {
                return new ProcessResultOperatorReturn { AdditionalCriteria = (q, p) => q.SetFirstResult((int)p[parameterName.Name].First) };
            }
            else
            {
                return new ProcessResultOperatorReturn { AdditionalCriteria = (q, p) => q.SetFirstResult(resultOperator.GetConstantCount()) };
            }
        }
    }
}