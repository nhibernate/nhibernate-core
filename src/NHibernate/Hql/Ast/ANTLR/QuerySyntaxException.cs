using System;
using System.Runtime.Serialization;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
	[CLSCompliant(false)]
	[Serializable]
	public class QuerySyntaxException : QueryException
	{
		protected QuerySyntaxException() {}
		public QuerySyntaxException(string message, string hql) : base(message, hql) {}

		public QuerySyntaxException(string message) : base(message) {}
		public QuerySyntaxException(string message, Exception inner) : base(message, inner) {}

		protected QuerySyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) {}

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