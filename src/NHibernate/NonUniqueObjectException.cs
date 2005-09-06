using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// This exception is thrown when an operation would
	/// break session-scoped identity. This occurs if the
	/// user tries to associate two different instances of
	/// the same class with a particular identifier,
	/// in the scope of a single <see cref="ISession"/>.
	/// </summary>
	[Serializable]
	public class NonUniqueObjectException : HibernateException
	{
		private readonly object identifier;
		private readonly System.Type clazz;

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueObjectException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="id">The identifier of the object that caused the exception.</param>
		/// <param name="clazz">The <see cref="System.Type"/> of the object attempted to be loaded.</param>
		public NonUniqueObjectException( String message, object id, System.Type clazz )
			: base( message )
		{
			this.clazz = clazz;
			this.identifier = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueObjectException"/> class.
		/// </summary>
		/// <param name="id">The identifier of the object that caused the exception.</param>
		/// <param name="clazz">The <see cref="System.Type"/> of the object attempted to be loaded.</param>
		public NonUniqueObjectException( object id, System.Type clazz )
			: this( "a different object with the same identifier value was already associated with the session", id, clazz )
		{
		}

		public object Identifier
		{
			get { return identifier; }
		}

		public override string Message
		{
			get { return base.Message + ": " + identifier + ", of class: " + clazz.FullName; }
		}

		public System.Type PersistentClass
		{
			get { return clazz; }
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueObjectException"/> class.
		/// </summary>
		protected NonUniqueObjectException( SerializationInfo info, StreamingContext context ) 
			: base( info, context )
		{
			identifier = info.GetValue( "identifier", typeof(System.Object) );
			clazz = info.GetValue( "clazz", typeof(System.Type) ) as System.Type;
		}

		/// <summary>
		/// Sets the serialization info for <see cref="InstantiationException"/> after 
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
			info.AddValue( "identifier", identifier, typeof(System.Object) );
			info.AddValue( "clazz", clazz, typeof(System.Type) );
		}

		#endregion

	}
}