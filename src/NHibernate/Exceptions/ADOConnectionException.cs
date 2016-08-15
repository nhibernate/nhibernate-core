using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating problems with communicating with the
	/// database (can also include incorrect ADO setup). 
	/// </summary>
	[Serializable]
	public class ADOConnectionException : ADOException
	{
#if FEATURE_SERIALIZATION
		public ADOConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif
		public ADOConnectionException(string message, Exception innerException, string sql) : base(message, innerException, sql) {}
		public ADOConnectionException(string message, Exception innerException) : base(message, innerException) {}
	}
}
