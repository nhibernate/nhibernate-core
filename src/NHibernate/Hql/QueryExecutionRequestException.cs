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

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected QueryExecutionRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
