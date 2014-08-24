using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ResultOperatorProcessor<T> : ResultOperatorProcessorBase
        where T : ResultOperatorBase
    {
        private readonly IResultOperatorProcessor<T> _processor;

        public ResultOperatorProcessor(IResultOperatorProcessor<T> processor)
        {
            _processor = processor;
        }

        public override void Process(ResultOperatorBase resultOperator, QueryModelVisitor queryModel, IntermediateHqlTree tree)
        {
            _processor.Process((T)resultOperator, queryModel, tree);
        }
    }
}