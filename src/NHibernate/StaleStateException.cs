using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	[Serializable]
	public class StaleStateException : HibernateException
	{
		public StaleStateException(string message) : base(message)
		{
		}
		public StaleStateException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected StaleStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
