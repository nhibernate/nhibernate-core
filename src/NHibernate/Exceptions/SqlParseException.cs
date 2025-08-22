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

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected SqlParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
