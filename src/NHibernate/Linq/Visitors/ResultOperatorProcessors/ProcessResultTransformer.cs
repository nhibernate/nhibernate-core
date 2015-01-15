using NHibernate.Transform;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessResultTransformer : IResultOperatorProcessor<ResultTransformerResultOperator>
    {
		public void Process(ResultTransformerResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
			tree.AddAdditionalCriteria((q, p) => q.SetResultTransformer((IResultTransformer) resultOperator.Data.Value));
        }
    }
}