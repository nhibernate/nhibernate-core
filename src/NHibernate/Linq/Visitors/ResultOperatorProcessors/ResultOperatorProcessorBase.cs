using Remotion.Data.Linq.Clauses;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public abstract class ResultOperatorProcessorBase
    {
        public abstract ProcessResultOperatorReturn Process(ResultOperatorBase resultOperator, QueryModelVisitor queryModel);
    }
}