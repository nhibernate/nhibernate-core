using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate
{
	[Serializable]
	public class QueryParameterException : QueryException
	{
		// TODO : without default constructor can't be serialized
		public QueryParameterException(string message) : base(message) { }
		public QueryParameterException(string message, Exception inner) : base(message, inner) { }
#if FEATURE_SERIALIZATION
		protected QueryParameterException(SerializationInfo info,StreamingContext context): base(info, context) { }
#endif
	}
}
