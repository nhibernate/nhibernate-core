using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NHibernate
{
	/// <summary>
	/// Thrown when Hibernate could not resolve an object by id, especially when
	/// loading an association.
	/// </summary>
	[Serializable]
	public class UnresolvableObjectException : HibernateException
	{
		private readonly object identifier;
		private readonly System.Type clazz;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnresolvableObjectException"/> class.
		/// </summary>
		/// <param name="identifier">The identifier of the object that caused the exception.</param>
		/// <param name="clazz">The <see cref="System.Type"/> of the object attempted to be loaded.</param>
		public UnresolvableObjectException( object identifier, System.Type clazz ) :
			this( "No row with the given identifier exists", identifier, clazz )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnresolvableObjectException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="identifier">The identifier of the object that caused the exception.</param>
		/// <param name="clazz">The <see cref="System.Type"/> of the object attempted to be loaded.</param>
		public UnresolvableObjectException( string message, object identifier, System.Type clazz )
			: base( message )
		{
			this.identifier = identifier;
			this.clazz = clazz;
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

		public static void ThrowIfNull( object o, object id, System.Type clazz )
		{
			if( o == null )
			{
				throw new UnresolvableObjectException( id, clazz );
			}
		}

		#region ISerializable Members

		[SecurityPermissionAttribute(SecurityAction.LinkDemand,
		                             Flags=SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);
			info.AddValue( "identifier", identifier );
			info.AddValue( "clazz", clazz );
		}

		protected UnresolvableObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			identifier = info.GetValue( "identifier", typeof( object ) );
			clazz = info.GetValue( "clazz", typeof( System.Type ) ) as System.Type;
		}

		#endregion
	}
}
