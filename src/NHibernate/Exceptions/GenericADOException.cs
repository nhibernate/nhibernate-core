using System;
using System.Runtime.Serialization;
using NHibernate.SqlCommand;

namespace NHibernate.Exceptions
{
	[Serializable]
	public class GenericADOException : ADOException
	{
	    public GenericADOException()
	    {
	    }
	    // Since v5.6
	    [Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		public GenericADOException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public GenericADOException(string message, Exception innerException, string sql) : base(message, innerException, sql) { }
		public GenericADOException(string message, Exception innerException) : base(message, innerException) { }
	}
}
