namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Exception thrown when an invalid path is found in a query.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	public class InvalidPathException : SemanticException 
	{
		public InvalidPathException(string s) : base(s) 
		{
		}
	}
}
