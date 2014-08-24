using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessFetchMany : ProcessFetch, IResultOperatorProcessor<FetchManyRequest>
    {
        public void Process(FetchManyRequest resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            base.Process(resultOperator, queryModelVisitor, tree);
        }
    }
}