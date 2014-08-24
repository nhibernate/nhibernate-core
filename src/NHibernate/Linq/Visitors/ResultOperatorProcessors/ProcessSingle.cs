using System.Linq;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessSingle : ProcessFirstOrSingleBase, IResultOperatorProcessor<SingleResultOperator>
    {
        public void Process(SingleResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            var firstMethod = resultOperator.ReturnDefaultWhenEmpty
                                  ? ReflectionHelper.GetMethodDefinition(() => Queryable.SingleOrDefault<object>(null))
                                  : ReflectionHelper.GetMethodDefinition(() => Queryable.Single<object>(null));

            AddClientSideEval(firstMethod, queryModelVisitor, tree);
        }
    }
}