namespace NHibernate.Hql.Ast.ANTLR
{
	class InvalidWithClauseException : QuerySyntaxException
	{
		public InvalidWithClauseException(string message) : base(message)
		{
		}

		public InvalidWithClauseException(string message, string hql) : base(message, hql)
		{
		}
	}
}
