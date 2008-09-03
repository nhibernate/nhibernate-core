using System;
using System.Runtime.Serialization;

namespace NHibernate.Classic
{
	/// <summary>
	/// Thrown from <see cref="IValidatable.Validate" /> when an invariant was violated. Some applications
	/// might subclass this exception in order to provide more information about the violation
	/// </summary>
	[Serializable]
	public class ValidationFailure : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationFailure"/> class.
		/// </summary>
		public ValidationFailure() : base("A validation failure occurred")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationFailure"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public ValidationFailure(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationFailure"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public ValidationFailure(Exception innerException) : base("A validation failure occurred", innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationFailure"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public ValidationFailure(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationFailure"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected ValidationFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
