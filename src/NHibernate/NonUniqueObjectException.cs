using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// This exception is thrown when an operation would
	/// break session-scoped identity. This occurs if the
	/// user tries to associate two different instances of
	/// the same Java class with a particular identifier,
	/// in the scope of a single <c>ISession</c>.
	/// </summary>
	[Serializable]
	public class NonUniqueObjectException : HibernateException
	{
		private readonly object identifier;
		private readonly System.Type clazz;

		public NonUniqueObjectException() : this( null, null )
		{
		}

		public NonUniqueObjectException( String message, object id, System.Type clazz )
			: base( message )
		{
			this.clazz = clazz;
			this.identifier = id;
		}

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

		protected NonUniqueObjectException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}