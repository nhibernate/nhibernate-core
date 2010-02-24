using System.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessFirst : ProcessFirstOrSingleBase, IResultOperatorProcessor<FirstResultOperator>
    {
        public ProcessResultOperatorReturn Process(FirstResultOperator resultOperator, QueryModelVisitor queryModelVisitor)
        {
            var firstMethod = resultOperator.ReturnDefaultWhenEmpty
                                  ? ReflectionHelper.GetMethod(() => Queryable.FirstOrDefault<object>(null))
                                  : ReflectionHelper.GetMethod(() => Queryable.First<object>(null));

            return ProcessFirstOrSingle(firstMethod, queryModelVisitor);
        }
    }
}