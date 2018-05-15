using System;
using System.Runtime.Serialization;

namespace NHibernate.Hql.Ast.ANTLR
{
	[Serializable]
	public class DetailedSemanticException : SemanticException
	{
		public DetailedSemanticException(string message) : base(message)
		{
		}

		public DetailedSemanticException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected DetailedSemanticException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
