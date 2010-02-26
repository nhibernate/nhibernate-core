using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessFirstOrSingleBase
    {
        protected static void ProcessFirstOrSingle(MethodInfo target, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            target = target.MakeGenericMethod(queryModelVisitor.CurrentEvaluationType.DataType);

            var parameter = Expression.Parameter(queryModelVisitor.PreviousEvaluationType.DataType, null);

            var lambda = Expression.Lambda(
                Expression.Call(
                    target,
                    parameter),
                parameter);

            tree.AddAdditionalCriteria((q, p) => q.SetMaxResults(1));
            tree.AddPostExecuteTransformer(lambda);
        }
    }
}