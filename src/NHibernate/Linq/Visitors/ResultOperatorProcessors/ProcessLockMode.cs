namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessLockMode : IResultOperatorProcessor<LockModeResultOperator>
	{
		public void Process(LockModeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddAdditionalCriteria((q, p) => q.SetLockMode(queryModelVisitor.Model.MainFromClause.ItemName, (LockMode)resultOperator.Data.Value));
		}
	}
}