using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when the user passes a persistent instance to a <c>ISession</c> method that expects a
	/// transient instance
	/// </summary>
	[Serializable]
	public class PersistentObjectException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PersistentObjectException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public PersistentObjectException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PersistentObjectException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected PersistentObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}