using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Implementations will report or handle errors invoked by an ANTLR base parser.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	public interface IErrorReporter
	{
		void ReportError(RecognitionException e);

		void ReportError(string s);

		void ReportWarning(string s);
	}
}
