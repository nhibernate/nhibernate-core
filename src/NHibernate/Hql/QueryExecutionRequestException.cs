using System;

namespace NHibernate.Hql
{
	public class QueryExecutionRequestException : QueryException
	{
		public QueryExecutionRequestException(string message, string queryString) : base(message, queryString)
		{
		}
	}
}