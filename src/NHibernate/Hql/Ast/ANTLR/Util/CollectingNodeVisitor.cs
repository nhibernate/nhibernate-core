using System;
using System.Collections.Generic;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	/// <summary>
	/// Filters nodes in/out of a tree.
	/// </summary>
	/// <param name="node">The node to check.</param>
	/// <returns>true to keep the node, false if the node should be filtered out.</returns>
	[CLSCompliant(false)]
	public delegate bool FilterPredicate(IASTNode node);

	[CLSCompliant(false)]
	public class CollectingNodeVisitor : IVisitationStrategy
	{
		private readonly List<IASTNode> collectedNodes = new List<IASTNode>();
		private readonly FilterPredicate predicate;

		public CollectingNodeVisitor(FilterPredicate predicate)
		{
			this.predicate = predicate;
		}

		#region IVisitationStrategy Members

		public void Visit(IASTNode node)
		{
			if (predicate == null || predicate(node))
			{
				collectedNodes.Add(node);
			}
		}

		#endregion

		public IList<IASTNode> Collect(IASTNode root)
		{
			var traverser = new NodeTraverser(this);
			traverser.TraverseDepthFirst(root);
			return collectedNodes;
		}
	}
}