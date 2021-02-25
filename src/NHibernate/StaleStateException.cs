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

		protected StaleStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
