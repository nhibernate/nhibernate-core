using System.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessSingle : ProcessFirstOrSingleBase, IResultOperatorProcessor<SingleResultOperator>
    {
        public ProcessResultOperatorReturn Process(SingleResultOperator resultOperator, QueryModelVisitor queryModelVisitor)
        {
            var firstMethod = resultOperator.ReturnDefaultWhenEmpty
                                  ? ReflectionHelper.GetMethod(() => Queryable.SingleOrDefault<object>(null))
                                  : ReflectionHelper.GetMethod(() => Queryable.Single<object>(null));

            return ProcessFirstOrSingle(firstMethod, queryModelVisitor);
        }
    }
}