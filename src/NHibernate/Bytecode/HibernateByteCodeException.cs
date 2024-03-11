using System;
using System.Runtime.Serialization;

namespace NHibernate.Bytecode
{
	[Serializable]
	public class HibernateByteCodeException : HibernateException
	{
		public HibernateByteCodeException() {}
		public HibernateByteCodeException(string message) : base(message) {}
		public HibernateByteCodeException(string message, Exception inner) : base(message, inner) {}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected HibernateByteCodeException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
