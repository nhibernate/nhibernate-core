using System;
using System.Runtime.Serialization;

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
		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		public DataException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public DataException(string message, Exception innerException, string sql) : base(message, innerException, sql) {}
		public DataException(string message, Exception innerException) : base(message, innerException) {}
	}
}
