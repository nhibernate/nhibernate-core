using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when the user tries to pass a deleted object to the <c>ISession</c>.
	/// </summary>
	[Serializable]
	public class ObjectDeletedException : HibernateException
	{
		private object identifier;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="identifier"></param>
		public ObjectDeletedException( string message, object identifier ) : base( message )
		{
			this.identifier = identifier;
		}

		/// <summary></summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary></summary>
		public override string Message
		{
			get { return base.Message + ": " + identifier; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public ObjectDeletedException( string message, Exception root ) : this( message, root.Message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ObjectDeletedException( string message ) : this( message, message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public ObjectDeletedException( Exception root ) : this( root.Message, root.Message )
		{
		}

		/// <summary></summary>
		public ObjectDeletedException() : this( string.Empty, string.Empty )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ObjectDeletedException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}