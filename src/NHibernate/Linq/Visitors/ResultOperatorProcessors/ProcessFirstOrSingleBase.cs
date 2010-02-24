using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessFirstOrSingleBase
    {
        protected static ProcessResultOperatorReturn ProcessFirstOrSingle(MethodInfo target, QueryModelVisitor queryModelVisitor)
        {
            target = target.MakeGenericMethod(queryModelVisitor.CurrentEvaluationType.DataType);

            var parameter = Expression.Parameter(queryModelVisitor.PreviousEvaluationType.DataType, null);

            var lambda = Expression.Lambda(
                Expression.Call(
                    target,
                    parameter),
                parameter);

            return new ProcessResultOperatorReturn { AdditionalCriteria = (q, p) => q.SetMaxResults(1), PostExecuteTransformer = lambda };
        }
    }
}