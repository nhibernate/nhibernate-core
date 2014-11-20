namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessReadOnly : IResultOperatorProcessor<ReadOnlyResultOperator>
	{
		public void Process(ReadOnlyResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddAdditionalCriteria((q, p) => q.SetReadOnly((bool)resultOperator.Data.Value));
		}
	}
}