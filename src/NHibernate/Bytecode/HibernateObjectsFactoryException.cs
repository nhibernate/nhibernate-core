using System;
using System.Runtime.Serialization;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Thrown if NHibernate can't instantiate the <see cref="IObjectsFactory"/> type.
	/// </summary>
	[Serializable]
	public class HibernateObjectsFactoryException : HibernateException
	{
		public HibernateObjectsFactoryException() { }
		public HibernateObjectsFactoryException(string message) : base(message) { }
		public HibernateObjectsFactoryException(string message, Exception inner) : base(message, inner) { }

		protected HibernateObjectsFactoryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
