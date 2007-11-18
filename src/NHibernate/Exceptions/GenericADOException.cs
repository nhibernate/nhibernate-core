using System;
using System.Runtime.Serialization;
using NHibernate.SqlCommand;

namespace NHibernate.Exceptions
{
	public class GenericADOException : ADOException
	{
		public GenericADOException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public GenericADOException(string message, Exception innerException, SqlString sql) : base(message, innerException, sql) { }
		public GenericADOException(string message, Exception innerException) : base(message, innerException) { }
	}
}