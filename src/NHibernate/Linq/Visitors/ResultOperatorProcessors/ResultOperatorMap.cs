using System;
using System.Collections.Generic;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ResultOperatorMap
    {
        readonly Dictionary<System.Type, ResultOperatorProcessorBase> _map = new Dictionary<System.Type, ResultOperatorProcessorBase>();

        public void Add<TOperator, TProcessor>()
            where TOperator : ResultOperatorBase
            where TProcessor : IResultOperatorProcessor<TOperator>, new()
        {
            _map.Add(typeof(TOperator), new ResultOperatorProcessor<TOperator>(new TProcessor()));
        }

        public void Process(ResultOperatorBase resultOperator, QueryModelVisitor queryModel, IntermediateHqlTree tree)
        {
            ResultOperatorProcessorBase processor;

            if (_map.TryGetValue(resultOperator.GetType(), out processor))
            {
                processor.Process(resultOperator, queryModel, tree);
            }
            else
            {
                throw new NotSupportedException(string.Format("The {0} result operator is not current supported",
                                                          resultOperator.GetType().Name));
            }
        }
    }
}