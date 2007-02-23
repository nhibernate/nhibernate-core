using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Indicated that a transaction could not be begun, committed, or rolled back
	/// </summary>
	[Serializable]
	public class TransactionException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public TransactionException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public TransactionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected TransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}