using NHibernate.Linq.EagerFetching;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessFetch
    {
        public void Process(FetchRequestBase resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree, bool userInnerJoin)
        {
            var querySource = QuerySourceLocator.FindQuerySource(queryModelVisitor.Model, resultOperator.RelationMember.DeclaringType);

            Process(resultOperator, queryModelVisitor, tree, querySource.ItemName, userInnerJoin);
        }

        public void Process(FetchRequestBase resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree, string sourceAlias, bool userInnerJoin)
        {
            var join = tree.TreeBuilder.Dot(
                tree.TreeBuilder.Ident(sourceAlias),
                tree.TreeBuilder.Ident(resultOperator.RelationMember.Name));

            string alias = queryModelVisitor.Model.GetNewName("_");

            if (userInnerJoin)
            {
                tree.AddFromClause(tree.TreeBuilder.FetchJoin(join, tree.TreeBuilder.Alias(alias)));
            }
            else
            {
                tree.AddFromClause(tree.TreeBuilder.LeftFetchJoin(join, tree.TreeBuilder.Alias(alias)));
            }

            tree.AddDistinctRootOperator();

            foreach (var innerFetch in resultOperator.InnerFetchRequests)
            {
                if (innerFetch is InnerFetchOneRequest || innerFetch is InnerFetchManyRequest)
                {
                    Process(innerFetch, queryModelVisitor, tree, alias, true);
                }
                else
                {
                    Process(innerFetch, queryModelVisitor, tree, alias, false);
                }
            }
        }
    }
}