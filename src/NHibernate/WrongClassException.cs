using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when <c>ISession.Load()</c> selects a row with the given primary key (identifier value)
	/// but the row's discriminator value specifies a different subclass from the one requested
	/// </summary>
	[Serializable]
	public class WrongClassException : HibernateException, ISerializable
	{
		private object identifier;
		private System.Type type;

		/// <summary>
		/// Initializes a new instance of the <see cref="WrongClassException"/> class.
		/// </summary>
		public WrongClassException(  ) 
			: base( "A row with the supplied identifier was found but the discriminator specifies a different subclass." )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WrongClassException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public WrongClassException( string message ) : base( message )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WrongClassException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public WrongClassException( string message, Exception innerException ) : base( message, innerException  )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WrongClassException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="identifier">The identifier of the object that was being loaded.</param>
		/// <param name="type">The <see cref="System.Type"/> that NHibernate was told to load.</param>
		public WrongClassException( string message, object identifier, System.Type type ) : base( message )
		{
			this.identifier = identifier;
			this.type = type;
		}

		/// <summary>
		/// Gets the identifier of the object that was being loaded.
		/// </summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that NHibernate was told to load.
		/// </summary>
		public System.Type Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="WrongClassException"/>.
		/// </summary>
		/// <value>The error message that explains the reason for this exception.</value>
		public override string Message
		{
			get
			{
				return "Object with id: " + identifier
					+ " was not of the specified sublcass: " + type.FullName
					+ " (" + base.Message + ")";
			}
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="WrongClassException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected WrongClassException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			type = (System.Type)info.GetValue( "type", typeof(System.Type) );
			identifier = info.GetValue( "identifier", typeof(object) ) ;
		}

		/// <summary>
		/// Sets the serialization info for <see cref="WrongClassException"/> after 
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