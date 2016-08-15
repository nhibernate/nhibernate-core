using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Bytecode
{
	[Serializable]
	public class HibernateByteCodeException : HibernateException
	{
		public HibernateByteCodeException() {}
		public HibernateByteCodeException(string message) : base(message) {}
		public HibernateByteCodeException(string message, Exception inner) : base(message, inner) {}

#if FEATURE_SERIALIZATION
		protected HibernateByteCodeException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif
	}
}
