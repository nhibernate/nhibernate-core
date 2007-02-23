using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Any exception that occurs in the O-R persistence layer.
	/// </summary>
	/// <remarks>
	/// Exceptions that occur in the database layer are left as native exceptions.
	/// </remarks>
	[Serializable]
	public class HibernateException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateException"/> class.
		/// </summary>
		public HibernateException() : base("An exception occurred in the persistence layer.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public HibernateException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public HibernateException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public HibernateException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateException"/> class 
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected HibernateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}