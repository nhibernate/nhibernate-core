using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	[Serializable]
	public class QueryParameterException : QueryException
	{
		// TODO : without default constructor can't be serialized
		public QueryParameterException(string message) : base(message) { }
		public QueryParameterException(string message, Exception inner) : base(message, inner) { }
		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected QueryParameterException(SerializationInfo info,StreamingContext context): base(info, context) { }
	}
}
