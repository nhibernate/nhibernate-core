using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessGroupBy : IResultOperatorProcessor<GroupResultOperator>
    {
        public ProcessResultOperatorReturn Process(GroupResultOperator resultOperator, QueryModelVisitor queryModelVisitor)
        {
            return new ProcessResultOperatorReturn
                       {
                           GroupBy =
                               queryModelVisitor.TreeBuilder.GroupBy(
                                   HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.KeySelector, queryModelVisitor.VisitorParameters).
                                       AsExpression())
                       };
        }
    }
}