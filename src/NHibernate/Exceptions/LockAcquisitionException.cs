using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating a problem acquiring lock
	/// on the database. 
	/// </summary>
	[Serializable]
	public class LockAcquisitionException : ADOException
	{
#if FEATURE_SERIALIZATION
		public LockAcquisitionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif
		public LockAcquisitionException(string message, Exception innerException, string sql) : base(message, innerException, sql) {}
		public LockAcquisitionException(string message, Exception innerException) : base(message, innerException) {}
	}
}
