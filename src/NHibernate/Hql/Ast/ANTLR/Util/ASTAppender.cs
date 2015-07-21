using System;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	/// <summary>
	/// Appends child nodes to a parent efficiently.
	/// </summary>
	[CLSCompliant(false)]
	public class ASTAppender
	{
		private readonly IASTFactory factory;
		private readonly IASTNode parent;

		public ASTAppender(IASTFactory factory, IASTNode parent)
		{
			this.factory = factory;
			this.parent = parent;
		}

		public IASTNode Append(int type, String text, bool appendIfEmpty)
		{
			if (text != null && (appendIfEmpty || text.Length > 0))
			{
				return parent.AddChild(factory.CreateNode(type, text));
			}

			return null;
		}
	}
}