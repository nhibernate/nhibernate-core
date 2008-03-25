using System;
using System.Runtime.Serialization;
using NHibernate.SqlCommand;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating problems with communicating with the
	/// database (can also include incorrect ADO setup). 
	/// </summary>
	[Serializable]
	public class ADOConnectionException : ADOException
	{
		public ADOConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public ADOConnectionException(string message, Exception innerException, SqlString sql) : base(message, innerException, sql) {}
		public ADOConnectionException(string message, Exception innerException) : base(message, innerException) {}
	}
}
