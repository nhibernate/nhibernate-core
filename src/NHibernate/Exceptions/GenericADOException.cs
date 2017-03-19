using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Exceptions
{
	[Serializable]
	public class GenericADOException : ADOException
	{
	    public GenericADOException()
	    {
	        
	    }
#if FEATURE_SERIALIZATION
		public GenericADOException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public GenericADOException(string message, Exception innerException, string sql) : base(message, innerException, sql) { }
		public GenericADOException(string message, Exception innerException) : base(message, innerException) { }
	}
}
