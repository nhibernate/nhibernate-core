using System;
using Antlr.Runtime;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

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

#if FEATURE_SERIALIZATION
		protected QuerySyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif

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
