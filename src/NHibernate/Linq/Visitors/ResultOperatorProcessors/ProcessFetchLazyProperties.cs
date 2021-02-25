namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	internal class ProcessFetchLazyProperties : IResultOperatorProcessor<FetchLazyPropertiesResultOperator>
	{
		public void Process(FetchLazyPropertiesResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddFromLastChildClause(tree.TreeBuilder.Fetch());
		}
	}
}
