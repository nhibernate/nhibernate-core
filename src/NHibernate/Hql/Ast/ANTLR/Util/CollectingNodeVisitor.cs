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

	//Since 5.3
	[Obsolete("Use generic version instead")]
	[CLSCompliant(false)]
	public class CollectingNodeVisitor : CollectingNodeVisitor<IASTNode>
	{
		public CollectingNodeVisitor(FilterPredicate predicate) : base(predicate)
		{
		}
	}

	[CLSCompliant(false)]
	public class CollectingNodeVisitor<TNode> : IVisitationStrategy 
	{
		private readonly List<TNode> collectedNodes = new List<TNode>();
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
				collectedNodes.Add((TNode) node);
			}
		}

		#endregion

		public IList<TNode> Collect(IASTNode root)
		{
			var traverser = new NodeTraverser(this);
			traverser.TraverseDepthFirst(root);
			return collectedNodes;
		}
	}
}
