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

		protected SemanticException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
