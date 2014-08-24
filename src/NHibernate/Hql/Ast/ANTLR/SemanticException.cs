using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
	public class SemanticException : QueryException
	{
		public SemanticException(string message) : base(message)
		{
		}

		public SemanticException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
