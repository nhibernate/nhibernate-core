using System.Linq;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessContains : IResultOperatorProcessor<ContainsResultOperator>
    {
        public ProcessResultOperatorReturn Process(ContainsResultOperator resultOperator, QueryModelVisitor queryModelVisitor)
        {
            var itemExpression =
                HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Item, queryModelVisitor.VisitorParameters)
                    .AsExpression();

            var from = GetFromRangeClause(queryModelVisitor.Root);
            var source = from.Children.First();

            if (source is HqlParameter)
            {
                // This is an "in" style statement
                return new ProcessResultOperatorReturn {TreeNode = queryModelVisitor.TreeBuilder.In(itemExpression, source)};
            }
            else
            {
                // This is an "exists" style statement
                return new ProcessResultOperatorReturn
                           {
                               WhereClause = queryModelVisitor.TreeBuilder.Equality(
                                   queryModelVisitor.TreeBuilder.Ident(GetFromAlias(queryModelVisitor.Root).AstNode.Text),
                                   itemExpression),
                               TreeNode = queryModelVisitor.TreeBuilder.Exists((HqlQuery)queryModelVisitor.Root)
                           };
            }
        }

        private static HqlRange GetFromRangeClause(HqlTreeNode node)
        {
            return node.NodesPreOrder.Single(n => n is HqlRange).As<HqlRange>();
        }

        private static HqlAlias GetFromAlias(HqlTreeNode node)
        {
            return node.NodesPreOrder.Single(n => n is HqlRange).Children.Single(n => n is HqlAlias) as HqlAlias;
        }
    }
}