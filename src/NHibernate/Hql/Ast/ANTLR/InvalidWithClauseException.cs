using System;
using System.Runtime.Serialization;

namespace NHibernate.Hql.Ast.ANTLR
{
	[CLSCompliant(false)]
	[Serializable]
	public class InvalidWithClauseException : QuerySyntaxException
	{
		protected InvalidWithClauseException() {}
		public InvalidWithClauseException(string message) : base(message) {}
		public InvalidWithClauseException(string message, Exception inner) : base(message, inner) {}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected InvalidWithClauseException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
