using System;
using System.Runtime.Serialization;

namespace NHibernate.Hql
{
	[Serializable]
	public class QueryExecutionRequestException : QueryException
	{
		public QueryExecutionRequestException(string message, string queryString) : base(message, queryString)
		{
		}

		protected QueryExecutionRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
