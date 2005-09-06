using System;
using System.Data;
using System.Runtime.Serialization;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// Wraps exceptions that occur during ADO.NET calls. Exceptions thrown
	/// by various ADO.NET providers are not derived from a common base class
	/// (<c>SQLException</c> in Java), so just <c>Exception</c>
	/// </summary>
	[ Serializable ]
	public class ADOException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ADOException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public ADOException( string message, Exception innerException ) : base( message, innerException )
		{
			LogManager.GetLogger( typeof( ADOException ) ).Error( message, innerException );
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ADOException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected ADOException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}

	}
}