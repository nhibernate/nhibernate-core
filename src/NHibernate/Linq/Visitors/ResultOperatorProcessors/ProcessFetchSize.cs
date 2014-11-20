namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessFetchSize : IResultOperatorProcessor<FetchSizeResultOperator>
    {
		public void Process(FetchSizeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
			tree.AddAdditionalCriteria((q, p) => q.SetFetchSize((int) resultOperator.Data.Value));
        }
    }
}