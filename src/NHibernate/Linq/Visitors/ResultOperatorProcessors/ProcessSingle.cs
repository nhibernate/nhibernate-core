using System.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessSingle : ProcessFirstOrSingleBase, IResultOperatorProcessor<SingleResultOperator>
    {
        public void Process(SingleResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            var firstMethod = resultOperator.ReturnDefaultWhenEmpty
                                  ? ReflectionHelper.GetMethod(() => Queryable.SingleOrDefault<object>(null))
                                  : ReflectionHelper.GetMethod(() => Queryable.Single<object>(null));

            AddClientSideEval(firstMethod, queryModelVisitor, tree);
        }
    }
}