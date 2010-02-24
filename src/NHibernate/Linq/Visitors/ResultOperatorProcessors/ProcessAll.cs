using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessAll : IResultOperatorProcessor<AllResultOperator>
    {
        public ProcessResultOperatorReturn Process(AllResultOperator resultOperator, QueryModelVisitor queryModelVisitor)
        {
            return new ProcessResultOperatorReturn
                       {
                           WhereClause = queryModelVisitor.TreeBuilder.BooleanNot(
                               HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Predicate, queryModelVisitor.VisitorParameters).
                                   AsBooleanExpression()),
                           TreeNode = queryModelVisitor.TreeBuilder.BooleanNot(queryModelVisitor.TreeBuilder.Exists((HqlQuery)queryModelVisitor.Root))
                       };
        }
    }
}