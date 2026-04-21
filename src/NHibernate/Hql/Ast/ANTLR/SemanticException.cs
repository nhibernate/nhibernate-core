using System;
using System.Runtime.Serialization;

namespace NHibernate.Hql.Ast.ANTLR
{
	[Serializable]
	public class SemanticException : QueryException
	{
		public SemanticException(string message) : base(message)
		{
		}

		public SemanticException(string message, Exception inner)
			: base(message, inner)
		{
		}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected SemanticException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
