using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Ast.ANTLR.Util;

namespace NHibernate.Hql.Ast.ANTLR
{
    internal class QuerySourceDetector : IVisitationStrategy
    {
        private readonly IASTNode _tree;
        private readonly List<IASTNode> _nodes;

        public QuerySourceDetector(IASTNode tree)
        {
            _tree = tree;
            _nodes = new List<IASTNode>();
        }

        public IList<IASTNode> LocateQuerySources()
        {
            // Find all the polymorphic query sources
            var nodeTraverser = new NodeTraverser(this);
            nodeTraverser.TraverseDepthFirst(_tree);

            return _nodes;
        }

        public void Visit(IASTNode node)
        {
            if (node.Type == HqlSqlWalker.FROM)
            {
                _nodes.AddRange(node.Where(child => child.Type == HqlSqlWalker.RANGE).Select(range => range.GetChild(0)));
            }
        }
    }
}