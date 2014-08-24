namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Implementors will return additional display text, which will be used
	/// by the ASTPrinter to display information (besides the node type and node
	/// text).
	/// </summary>
	public interface IDisplayableNode
	{
		/// <summary>
		/// Returns additional display text for the AST node.
		/// </summary>
		/// <returns>The additional display text.</returns>
		string GetDisplayText();
	}
}
