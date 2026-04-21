using System;
using System.Runtime.Serialization;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating problems with communicating with the
	/// database (can also include incorrect ADO setup). 
	/// </summary>
	[Serializable]
	public class ADOConnectionException : ADOException
	{
		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		public ADOConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public ADOConnectionException(string message, Exception innerException, string sql) : base(message, innerException, sql) {}
		public ADOConnectionException(string message, Exception innerException) : base(message, innerException) {}
	}
}
