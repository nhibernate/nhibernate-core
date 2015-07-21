using System;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	[CLSCompliant(false)]
	public class NodeTraverser
	{
		private readonly IVisitationStrategy _visitor;

		public NodeTraverser(IVisitationStrategy visitor)
		{
			_visitor = visitor;
		}

		/// <summary>
		/// Traverse the AST tree depth first. Note that the AST passed in is not visited itself.  Visitation starts
		/// with its children.
		/// </summary>
		/// <param name="ast">ast</param>
		public void TraverseDepthFirst(IASTNode ast)
		{
			if (ast == null)
			{
				throw new ArgumentNullException("ast");
			}

			for (int i = 0; i < ast.ChildCount; i++)
			{
				VisitDepthFirst(ast.GetChild(i));
			}
		}

		private void VisitDepthFirst(IASTNode ast)
		{
			if (ast == null)
			{
				return;
			}

			_visitor.Visit(ast);

			for (int i = 0; i < ast.ChildCount; i++)
			{
				VisitDepthFirst(ast.GetChild(i));
			}
		}
	}
}