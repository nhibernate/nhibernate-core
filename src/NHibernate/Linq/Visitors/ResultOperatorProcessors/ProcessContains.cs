using System.Linq;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessContains : IResultOperatorProcessor<ContainsResultOperator>
    {
        public void Process(ContainsResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            var itemExpression =
                HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Item, queryModelVisitor.VisitorParameters)
                    .AsExpression();

            var from = GetFromRangeClause(tree.Root);
            var source = from.Children.First();

            if (source is HqlParameter)
            {
                // This is an "in" style statement
                tree.SetRoot(tree.TreeBuilder.In(itemExpression, source));
            }
            else
            {
                // This is an "exists" style statement
                tree.AddWhereClause(tree.TreeBuilder.Equality(
                                   tree.TreeBuilder.Ident(GetFromAlias(tree.Root).AstNode.Text),
                                   itemExpression));
                tree.SetRoot(tree.TreeBuilder.Exists((HqlQuery)tree.Root));
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