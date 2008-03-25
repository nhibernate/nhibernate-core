using System;
using System.Runtime.Serialization;

namespace NHibernate.Cache
{
	/// <summary>
	/// Represents any exception from an <see cref="ICache"/>.
	/// </summary>
	[Serializable]
	public class CacheException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CacheException"/> class.
		/// </summary>
		public CacheException() : base("There was an Exception in the Cache.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public CacheException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public CacheException(Exception innerException) : base(innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public CacheException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected CacheException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}