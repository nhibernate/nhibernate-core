using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	[CLSCompliant(false)]
	public static class ASTUtil
	{
		public static void MakeSiblingOfParent(IASTNode parent, IASTNode child)
		{
			parent.RemoveChild(child);
			parent.AddSibling(child);
		}

		public static string GetPathText(IASTNode n)
		{
			StringBuilder buf = new StringBuilder();
			GetPathText(buf, n);
			return buf.ToString();
		}

		private static void GetPathText(StringBuilder buf, IASTNode n)
		{
			IASTNode firstChild = n.GetChild(0);

			// If the node has a first child, recurse into the first child.
			if (firstChild != null)
			{
				GetPathText(buf, firstChild);
			}

			// Append the text of the current node.
			buf.Append(n.Text);

			// If there is a second child (RHS), recurse into that child.
			if (firstChild != null && n.ChildCount > 1)
			{
				GetPathText(buf, n.GetChild(1));
			}
		}

		/// <summary>
		/// Returns the 'list' representation with some brackets around it for debugging.
		/// </summary>
		/// <param name="n">The tree.</param>
		/// <returns>The list representation of the tree.</returns>
		public static string GetDebugstring(IASTNode n)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("[ ");
			buf.Append((n == null) ? "{null}" : n.ToStringTree());
			buf.Append(" ]");
			return buf.ToString();
		}

		/// <summary>
		/// Determine if a given node (test) is contained anywhere in the subtree
		/// of another given node (fixture).
		/// </summary>
		/// <param name="fixture">The node against which to be checked for children.</param>
		/// <param name="test">The node to be tested as being a subtree child of the parent.</param>
		/// <returns>True if child is contained in the parent's collection of children.</returns>
		public static bool IsSubtreeChild(IASTNode fixture, IASTNode test)
		{
			for (int i = 0; i < fixture.ChildCount; i++)
			{
				IASTNode n = fixture.GetChild(i);

				if (n == test)
				{
					return true;
				}
				if (n.ChildCount > 0 && IsSubtreeChild(n, test))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Finds the first node of the specified type in the chain of children.
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <param name="type">The type to find.</param>
		/// <returns>The first node of the specified type, or null if not found.</returns>
		public static IASTNode FindTypeInChildren(IASTNode parent, int type)
		{
			for (int i = 0; i < parent.ChildCount; i++)
			{
				var child = parent.GetChild(i);
				if (child.Type == type)
				{
					return child;
				}
			}
			return null;
		}

		public static IList<IASTNode> CollectChildren(IASTNode root, FilterPredicate predicate)
		{
			return new CollectingNodeVisitor(predicate).Collect(root);
		}
	}
}