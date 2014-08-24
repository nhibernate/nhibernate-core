
namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    internal class ProcessTimeout : IResultOperatorProcessor<TimeoutResultOperator>
    {
        public void Process(TimeoutResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            tree.AddAdditionalCriteria((q, p) => q.SetTimeout((int) resultOperator.Timeout.Value));
        }
    }
}