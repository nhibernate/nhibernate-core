namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public interface IResultOperatorProcessor<T>
	{
		void Process(T resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree);
	}
}