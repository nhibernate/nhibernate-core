using System;
using System.Runtime.Serialization;

namespace NHibernate.Exceptions
{
	[Serializable]
	public class SqlParseException : Exception 
	{

		public SqlParseException(string message) : base(message)
		{
		}

		protected SqlParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
