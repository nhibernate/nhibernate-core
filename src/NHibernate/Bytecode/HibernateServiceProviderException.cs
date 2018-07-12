using System;
using System.Runtime.Serialization;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Thrown if NHibernate can't instantiate the <see cref="IServiceProvider"/> type.
	/// </summary>
	[Serializable]
	public class HibernateServiceProviderException : HibernateException
	{
		public HibernateServiceProviderException() {}
		public HibernateServiceProviderException(string message) : base(message) {}
		public HibernateServiceProviderException(string message, Exception inner) : base(message, inner) {}

		protected HibernateServiceProviderException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
