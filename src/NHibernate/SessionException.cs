using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate
{
	[Serializable]
	public class SessionException : HibernateException
	{
		public SessionException(string message)
			: base(message)
		{
		}

#if FEATURE_SERIALIZATION
		protected SessionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
#endif
	}
}
