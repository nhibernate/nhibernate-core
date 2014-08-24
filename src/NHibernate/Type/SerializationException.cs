using System;
using System.Runtime.Serialization;

namespace NHibernate.Type
{
	/// <summary>
	/// Thrown when a property cannot be serialized/deserialized
	/// </summary>
	[Serializable]
	public class SerializationException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SerializationException"/> class.
		/// </summary>
		public SerializationException()
			: base("The Property associated with a SerializableType threw an Exception during serialization or deserialization.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public SerializationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="e">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public SerializationException(string message, Exception e) : base(message, e)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializationException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected SerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}