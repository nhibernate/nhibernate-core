using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessSkip : IResultOperatorProcessor<SkipResultOperator>
    {
        public void Process(SkipResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            NamedParameter parameterName;

            if (queryModelVisitor.VisitorParameters.ConstantToParameterMap.TryGetValue(resultOperator.Count as ConstantExpression, out parameterName))
            {
                tree.AddAdditionalCriteria((q, p) => q.SetFirstResult((int)p[parameterName.Name].First));
            }
            else
            {
                tree.AddAdditionalCriteria((q, p) => q.SetFirstResult(resultOperator.GetConstantCount()));
            }
        }
    }
}