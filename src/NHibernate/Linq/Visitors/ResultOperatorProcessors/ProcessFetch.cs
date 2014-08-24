using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessFetch
    {
        public void Process(FetchRequestBase resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            var querySource = QuerySourceLocator.FindQuerySource(queryModelVisitor.Model, resultOperator.RelationMember.DeclaringType);

            Process(resultOperator, queryModelVisitor, tree, querySource.ItemName);
        }

        public void Process(FetchRequestBase resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree, string sourceAlias)
        {
            var join = tree.TreeBuilder.Dot(
                tree.TreeBuilder.Ident(sourceAlias),
                tree.TreeBuilder.Ident(resultOperator.RelationMember.Name));

            string alias = queryModelVisitor.Model.GetNewName("_");

            tree.AddFromClause(tree.TreeBuilder.LeftFetchJoin(join, tree.TreeBuilder.Alias(alias)));
            tree.AddDistinctRootOperator();

            foreach (var innerFetch in resultOperator.InnerFetchRequests)
            {
                Process(innerFetch, queryModelVisitor, tree, alias);
            }
        }

    }
}