using System;
using System.Runtime.Serialization;


namespace NHibernate
{
	/// <summary>
	/// Indicates failure of an assertion: a possible bug in NHibernate
	/// </summary>
	[Serializable]
	public class AssertionFailure : ApplicationException
	{
		private const string DefaultMessage = "An AssertionFailure occurred - this may indicate a bug in NHibernate or in your custom types.";

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertionFailure"/> class.
		/// </summary>
		public AssertionFailure() : base(String.Empty)
		{
			LoggerProvider.LoggerFor(typeof(AssertionFailure)).Error(DefaultMessage);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertionFailure"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public AssertionFailure(string message) : base(message)
		{
			LoggerProvider.LoggerFor(typeof(AssertionFailure)).Error(DefaultMessage, this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertionFailure"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public AssertionFailure(string message, Exception innerException) : base(message, innerException)
		{
			LoggerProvider.LoggerFor(typeof(AssertionFailure)).Error(DefaultMessage, innerException);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertionFailure"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected AssertionFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
