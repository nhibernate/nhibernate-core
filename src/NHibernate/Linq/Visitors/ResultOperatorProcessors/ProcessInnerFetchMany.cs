using NHibernate.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessInnerFetchMany : ProcessFetch, IResultOperatorProcessor<InnerFetchManyRequest>
    {
        public void Process(InnerFetchManyRequest resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            base.Process(resultOperator, queryModelVisitor, tree, true);
        }
    }
}
