using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Indicates that an expected getter or setter method could not be found on a class
	/// </summary>
	[Serializable]
	public class FKUnmatchingColumnsException : MappingException
	{
				/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public FKUnmatchingColumnsException(string message)
			: base(message)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public FKUnmatchingColumnsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected FKUnmatchingColumnsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

}
