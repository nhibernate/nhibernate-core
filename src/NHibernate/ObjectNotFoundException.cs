using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when the user tries to pass a deleted object to the <c>ISession</c>.
	/// </summary>
	[Serializable]
	public class ObjectNotFoundException : HibernateException
	{
		private object identifier;
		private System.Type type;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="identifier"></param>
		/// <param name="type"></param>
		public ObjectNotFoundException( string message, object identifier, System.Type type ) : base( message )
		{
			this.identifier = identifier;
			this.type = type;
		}

		/// <summary></summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary></summary>
		public override string Message
		{
			get { return base.Message + ": " + identifier + ", of class: " + type.FullName; }
		}

		/// <summary></summary>
		public System.Type Type
		{
			get { return type; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public ObjectNotFoundException( string message, Exception root ) : this( message, root.Message, typeof( ObjectNotFoundException ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ObjectNotFoundException( string message ) : this( message, message, typeof( ObjectNotFoundException ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public ObjectNotFoundException( Exception root ) : this( root.Message, root.Message, typeof( ObjectNotFoundException ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ObjectNotFoundException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}