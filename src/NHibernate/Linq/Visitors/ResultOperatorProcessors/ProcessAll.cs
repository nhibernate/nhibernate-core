using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessAll : IResultOperatorProcessor<AllResultOperator>
    {
        public void Process(AllResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            tree.AddWhereClause(tree.TreeBuilder.BooleanNot(
                               HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Predicate, queryModelVisitor.VisitorParameters).
                                   ToBooleanExpression()));

            tree.SetRoot(tree.TreeBuilder.BooleanNot(tree.TreeBuilder.Exists((HqlQuery) tree.Root)));
        }
    }
}