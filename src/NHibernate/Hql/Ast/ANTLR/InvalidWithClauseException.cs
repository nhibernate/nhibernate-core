using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Hql.Ast.ANTLR
{
	[CLSCompliant(false)]
	[Serializable]
	public class InvalidWithClauseException : QuerySyntaxException
	{
		protected InvalidWithClauseException() {}
		public InvalidWithClauseException(string message) : base(message) {}
		public InvalidWithClauseException(string message, Exception inner) : base(message, inner) {}

#if FEATURE_SERIALIZATION
		protected InvalidWithClauseException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif
	}
}
