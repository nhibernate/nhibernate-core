using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when <c>ISession.Load()</c> fails to select a row with
	/// the given primary key (identifier value). This exception might not
	/// be thrown when <c>Load()</c> is called, even if there was no
	/// row on the database, because <c>Load()</c> returns a proxy if
	/// possible. Applications should use <c>ISession.Get()</c> to test if 
	/// a row exists in the database.
	/// </summary>
	[Serializable]
	public class ObjectNotFoundException : HibernateException, ISerializable
	{
		private object identifier;
		private System.Type type;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
		/// </summary>
		public ObjectNotFoundException(  ) : base( "No object could be found with the supplied identifier." )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public ObjectNotFoundException( string message ) : base( message )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public ObjectNotFoundException( string message, Exception innerException ) : base( message, innerException  )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="identifier">The identifier of the object that was attempting to be loaded.</param>
		/// <param name="type">The <see cref="System.Type"/> that NHibernate was trying to find a row for in the database.</param>
		public ObjectNotFoundException( string message, object identifier, System.Type type ) : base( message )
		{
			this.identifier = identifier;
			this.type = type;
		}

		/// <summary>
		/// Gets the identifier of the object that was attempting to be loaded.
		/// </summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="ObjectNotFoundException"/>.
		/// </summary>
		/// <value>
		/// The error message that explains the reason for this exception and details of
		/// the object that was attempting to be loaded.
		/// </value>
		public override string Message
		{
			get { return base.Message + ": " + identifier + ", of class: " + type.FullName; }
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that NHibernate was trying to find a row for in the database.
		/// </summary>
		public System.Type Type
		{
			get { return type; }
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected ObjectNotFoundException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			type = (System.Type)info.GetValue( "type", typeof(System.Type) );
			identifier = info.GetValue( "identifier", typeof(object) ) ;
		}

		/// <summary>
		/// Sets the serialization info for <see cref="ObjectNotFoundException"/> after 
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
			info.AddValue( "type", type, typeof(System.Type) );
			info.AddValue( "identifier", identifier, typeof(object) );
		}

		#endregion
	}
}