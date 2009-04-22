namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Defines the behavior of an error handler for the HQL parsers.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	public interface IParseErrorHandler : IErrorReporter 
	{
		int GetErrorCount();

		void ThrowQueryException();
	}
}
