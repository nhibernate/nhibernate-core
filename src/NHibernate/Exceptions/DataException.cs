using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating that evaluation of the
	/// valid SQL statement against the given data resulted in some
	/// illegal operation, mismatched types or incorrect cardinality. 
	/// </summary>
	[Serializable]
	public class DataException : ADOException
	{
#if FEATURE_SERIALIZATION
		public DataException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif
		public DataException(string message, Exception innerException, string sql) : base(message, innerException, sql) {}
		public DataException(string message, Exception innerException) : base(message, innerException) {}
	}
}
