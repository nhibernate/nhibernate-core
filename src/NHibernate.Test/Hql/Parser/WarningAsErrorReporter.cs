using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR;

namespace NHibernate.Test.Hql.Parser
{
	public class WarningAsErrorReporter : IParseErrorHandler
	{
		public void ReportError(RecognitionException e)
		{
			throw e;
		}

		public void ReportError(string s)
		{
			throw new QueryException(s);
		}

		public void ReportWarning(string s)
		{
			throw new QueryException(s);
		}

		public int GetErrorCount() => 0;

		public void ThrowQueryException()
		{
		}
	}
}
