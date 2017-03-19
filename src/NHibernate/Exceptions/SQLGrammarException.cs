using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating that the SQL sent to the database
	/// server was invalid (syntax error, invalid object references, etc). 
	/// </summary>
	[Serializable]
	public class SQLGrammarException : ADOException
	{
#if FEATURE_SERIALIZATION
		public SQLGrammarException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif
		public SQLGrammarException(string message, Exception innerException, string sql) : base(message, innerException, sql) {}
		public SQLGrammarException(string message, Exception innerException) : base(message, innerException) {}
	}
}
