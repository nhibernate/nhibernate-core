using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate
{
	[Serializable]
	public class StaleStateException : HibernateException
	{
		public StaleStateException(string message) : base(message)
		{
		}

#if FEATURE_SERIALIZATION
		protected StaleStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
#endif
	}
}
