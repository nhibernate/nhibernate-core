using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Throw when the user passes a transient instance to a <c>ISession</c> method that expects
	/// a persistent instance
	/// </summary>
	[Serializable]
	public class TransientObjectException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransientObjectException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public TransientObjectException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransientObjectException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected TransientObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}