using System;
using System.Runtime.Serialization;

namespace NHibernate.Cfg
{
	/// <summary>
	/// An exception that occurs at configuration time, rather than runtime, as a result of
	/// something screwy in the hibernate.cfg.xml.
	/// </summary>
	[Serializable]
	public class HibernateConfigException : MappingException
	{
		private const string baseMessage = "An exception occurred during configuration of persistence layer.";

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateConfigException"/> class.
		/// </summary>
		/// <remarks>Default message is used.</remarks>
		public HibernateConfigException()
			: base(baseMessage)
		{ 
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateConfigException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public HibernateConfigException(Exception innerException)
			: base(baseMessage, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateConfigException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public HibernateConfigException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateConfigException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public HibernateConfigException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateConfigException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected HibernateConfigException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
