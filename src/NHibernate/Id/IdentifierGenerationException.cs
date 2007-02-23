using System;
using System.Runtime.Serialization;

namespace NHibernate.Id
{
	/// <summary>
	/// Thrown by <see cref="IIdentifierGenerator" /> implementation class when ID generation fails
	/// </summary>
	[Serializable]
	public class IdentifierGenerationException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class.
		/// </summary>
		public IdentifierGenerationException() : base("An exception occurred during ID generation.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public IdentifierGenerationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="e">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public IdentifierGenerationException(string message, Exception e) : base(message, e)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected IdentifierGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}