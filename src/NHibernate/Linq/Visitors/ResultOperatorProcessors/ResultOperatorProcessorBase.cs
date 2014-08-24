using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public abstract class ResultOperatorProcessorBase
    {
        public abstract void Process(ResultOperatorBase resultOperator, QueryModelVisitor queryModel, IntermediateHqlTree tree);
    }
}