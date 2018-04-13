using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessAsQueryable : IResultOperatorProcessor<AsQueryableResultOperator>
	{
		public void Process(AsQueryableResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			// Ignore AsQueryable
			// It could be used to detect accidental usage of IEnumerable-based extension methods
			// on query roots, resulting in in-memory queries instead of database queries.
		}
	}
}
