using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
	[CLSCompliant(false)]
	public class QuerySyntaxException : QueryException
	{
		public QuerySyntaxException(string message) : base(message)
		{
		}

		public QuerySyntaxException(string message, string hql)
			: base(message, hql)
		{
		}

		public static QuerySyntaxException Convert(RecognitionException e)
		{
			return Convert(e, null);
		}

		public static QuerySyntaxException Convert(RecognitionException e, string hql)
		{
			string positionInfo = e.Line > 0 && e.CharPositionInLine > 0
					? " near line " + e.Line + ", column " + e.CharPositionInLine
					: "";
			return new QuerySyntaxException(e.Message + positionInfo, hql);
		}
	}
}
