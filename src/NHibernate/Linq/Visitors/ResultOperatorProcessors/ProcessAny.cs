using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessAny : IResultOperatorProcessor<AnyResultOperator>
    {
        public ProcessResultOperatorReturn Process(AnyResultOperator anyOperator, QueryModelVisitor queryModelVisitor)
        {
            return new ProcessResultOperatorReturn
                       {
                           TreeNode = queryModelVisitor.TreeBuilder.Exists((HqlQuery) queryModelVisitor.Root)
                       };
        }
    }
}