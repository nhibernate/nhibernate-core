using NHibernate.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessInnerFetchOne : ProcessFetch, IResultOperatorProcessor<InnerFetchOneRequest>
    {
        public void Process(InnerFetchOneRequest resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            base.Process(resultOperator, queryModelVisitor, tree, true);
        }
    }
}
