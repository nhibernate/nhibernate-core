namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	internal class ProcessLock : IResultOperatorProcessor<LockResultOperator>
	{
		public void Process(LockResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddAdditionalCriteria((q, p) => q.SetLockMode(queryModelVisitor.Model.MainFromClause.ItemName, (LockMode)resultOperator.LockMode.Value));
		}
	}
}
