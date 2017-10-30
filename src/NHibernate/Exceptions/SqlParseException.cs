using System;
using System.Runtime.Serialization;

namespace NHibernate.Exceptions
{
	[Serializable]
	public class SqlParseException : Exception 
	{

		public SqlParseException(string Message) : base(Message)
		{
		}

		protected SqlParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
