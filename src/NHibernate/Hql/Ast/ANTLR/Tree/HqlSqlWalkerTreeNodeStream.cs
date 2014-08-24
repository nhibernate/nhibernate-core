using System;
using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class HqlSqlWalkerTreeNodeStream : BufferedTreeNodeStream
	{
		public HqlSqlWalkerTreeNodeStream(object tree) : base(tree) {}

		/// <summary>
		/// Insert a new node into both the Tree and the Node Array. Add DOWN and UP nodes if needed.
		/// </summary>
		/// <param name="parent">The parent node</param>
		/// <param name="child">The child node</param>
		public void InsertChild(IASTNode parent, IASTNode child)
		{
			if (child.ChildCount > 0)
			{
				throw new InvalidOperationException("Currently do not support adding nodes with children");
			}

			int parentIndex = nodes.IndexOf(parent);
			int numberOfChildNodes = NumberOfChildNodes(parentIndex);
			int insertPoint;
			
			if (numberOfChildNodes == 0)
			{
				insertPoint = parentIndex + 1;  // We want to insert immediately after the parent
				nodes.Insert(insertPoint, down);
				insertPoint++;  // We've just added a new node
			}
			else
			{
				insertPoint = parentIndex + numberOfChildNodes;
			}

			parent.AddChild(child);
			nodes.Insert(insertPoint, child);
			insertPoint++;

			if (numberOfChildNodes == 0)
			{
				nodes.Insert(insertPoint, up);
			}
		}

		/// <summary>
		/// Count the number of child nodes (including DOWNs and UPs) of a parent node
		/// </summary>
		/// <param name="parentIndex">The index of the parent in the node array</param>
		/// <returns>The number of child nodes</returns>
		int NumberOfChildNodes(int parentIndex)
		{
			if (nodes.Count -1 == parentIndex)
			{
				// We are at the end
				return 0;
			}
			
			if (nodes[parentIndex + 1] != down)
			{
				// Next node is not a DOWN node, so we have no children
				return 0;
			}

			// Count the DOWNs & UPs
			int downCount = 0;
			int index = 1;
			do
			{
				if (nodes[parentIndex + index] == down)
				{
					downCount++;
				}
				else if (nodes[parentIndex + index] == up)
				{
					downCount--;
				}

				index++;
				
			} while (downCount > 0);

			return index - 1;
		}
	}
}