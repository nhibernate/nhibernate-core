using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// An exception that usually occurs at configuration time, rather than runtime, as a result of
	/// something screwy in the O-R mappings
	/// </summary>
	[Serializable]
	public class MappingException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public MappingException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public MappingException(Exception innerException)
			: base(innerException)
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
		public MappingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected MappingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}