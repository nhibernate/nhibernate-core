using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when the user tries to pass a deleted object to the <c>ISession</c>.
	/// </summary>
	[Serializable]
	public class ObjectDeletedException : HibernateException, ISerializable
	{
		private object identifier;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectDeletedException"/> class.
		/// </summary>
		public ObjectDeletedException() : this( "User tried to pass a deleted object to the ISession." )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectDeletedException"/> class.
		/// </summary>
		/// <param name="message"></param>
		public ObjectDeletedException( string message ) : this( message, "n/a" )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectDeletedException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="identifier">The identifier of the object that was attempting to be deleted.</param>
		public ObjectDeletedException( string message, object identifier ) : base( message )
		{
			this.identifier = identifier;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectDeletedException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public ObjectDeletedException( string message, Exception innerException ) : base( message, innerException )
		{
		}

		/// <summary>
		/// Gets the identifier of the object that was attempting to be deleted.
		/// </summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="WrongClassException"/>.
		/// </summary>
		/// <value>
		/// The error message that explains the reason for this exception and the identifier
		/// of the object.
		/// </value>
		public override string Message
		{
			get { return base.Message + ": " + identifier; }
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectDeletedException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected ObjectDeletedException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			identifier = info.GetValue( "identifer", typeof(object) );
		}
		
		
		/// <summary>
		/// Sets the serialization info for <see cref="ObjectDeletedException"/> after 
		/// getting the info from the base Exception.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData( info, context );
			info.AddValue( "identifier", identifier  );
		}

		#endregion
	}
}