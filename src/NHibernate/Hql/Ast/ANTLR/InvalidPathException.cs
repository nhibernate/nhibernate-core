using System;
using System.Runtime.Serialization;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Exception thrown when an invalid path is found in a query.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[Serializable]
	public class InvalidPathException : SemanticException 
	{
		public InvalidPathException(string s) : base(s) 
		{
		}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected InvalidPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
