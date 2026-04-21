using System;
using System.Runtime.Serialization;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating a problem acquiring lock
	/// on the database. 
	/// </summary>
	[Serializable]
	public class LockAcquisitionException : ADOException
	{
		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		public LockAcquisitionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public LockAcquisitionException(string message, Exception innerException, string sql) : base(message, innerException, sql) {}
		public LockAcquisitionException(string message, Exception innerException) : base(message, innerException) {}
	}
}
