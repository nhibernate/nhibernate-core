using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessGroupBy : IResultOperatorProcessor<GroupResultOperator>
    {
        public void Process(GroupResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            tree.AddGroupByClause(tree.TreeBuilder.GroupBy(
                HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.KeySelector, queryModelVisitor.VisitorParameters)
                    .AsExpression()));
        }
    }
}