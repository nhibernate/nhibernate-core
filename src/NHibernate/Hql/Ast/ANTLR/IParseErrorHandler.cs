using System;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Defines the behavior of an error handler for the HQL parsers.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public interface IParseErrorHandler : IErrorReporter 
	{
		int GetErrorCount();

		void ThrowQueryException();
	}
}
