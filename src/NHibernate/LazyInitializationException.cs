using System;
using System.Runtime.Serialization;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// A problem occurred trying to lazily initialize a collection or proxy (for example the session
	/// was closed) or iterate query results.
	/// </summary>
	[Serializable]
	public class LazyInitializationException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public LazyInitializationException(string message) : this(message, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public LazyInitializationException(Exception innerException)
			: this("NHibernate lazy initialization problem", innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public LazyInitializationException(string message, Exception innerException) : base(message, innerException)
		{
			LogManager.GetLogger(typeof(LazyInitializationException)).Error(message, this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected LazyInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}