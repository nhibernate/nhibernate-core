using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	[Serializable]
	public class SessionException : HibernateException
	{
		public SessionException(string message)
			: base(message)
		{
		}

		protected SessionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
