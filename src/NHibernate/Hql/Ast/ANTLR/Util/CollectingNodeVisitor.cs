using System.Collections.Generic;
using Antlr.Runtime.Tree;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	public delegate bool FilterPredicate(IASTNode node);

	public class CollectingNodeVisitor : IVisitationStrategy
	{
		private readonly FilterPredicate predicate;
		private readonly List<IASTNode> collectedNodes = new List<IASTNode>();

		public CollectingNodeVisitor(FilterPredicate predicate)
		{
			this.predicate = predicate;
		}

		public void Visit(IASTNode node)
		{
			if ( predicate == null || predicate( node ) ) 
			{
				collectedNodes.Add( node );
			}
		}

		public IList<IASTNode> GetCollectedNodes() {
			return collectedNodes;
		}

		public IList<IASTNode> Collect(IASTNode root)
		{
			NodeTraverser traverser = new NodeTraverser( this );
			traverser.TraverseDepthFirst( root );
			return collectedNodes;
		}
	}
}
