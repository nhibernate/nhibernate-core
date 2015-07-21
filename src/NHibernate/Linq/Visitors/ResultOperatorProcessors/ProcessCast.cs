using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessCast : IResultOperatorProcessor<CastResultOperator>
	{
		public void Process(CastResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			// what we can do with Cast, so far, is only ignore it.
			// The meaning of Cast<T>() is different than OfType<T>.
			// With OfType<T> the user selects a specific entity-type; 
			// with Cast<T> the user "hopes" that everything will work with the same type
			// When we will have some more detail we can change this "implementation" ;)
		}
	}
}