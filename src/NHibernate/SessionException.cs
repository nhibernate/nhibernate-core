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

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected SessionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
