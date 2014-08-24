using System;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// An interface for initializeable AST nodes.
	/// </summary>
	public interface IInitializableNode
	{
		/// <summary>
		/// Initializes the node with the parameter.
		/// </summary>
		/// <param name="param">the initialization parameter.</param>
		void Initialize(Object param);
	}
}