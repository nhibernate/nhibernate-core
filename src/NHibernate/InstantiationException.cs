using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown if Hibernate can't instantiate an entity or component class at runtime.
	/// </summary>
	[Serializable]
	public class InstantiationException : HibernateException
	{
		private System.Type type;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="root"></param>
		public InstantiationException( string message, System.Type type, Exception root )
			: base( message, root )
		{
			this.type = type;
		}

		/// <summary></summary>
		public System.Type PersistentType
		{
			get { return type; }
		}

		/// <summary></summary>
		public override string Message
		{
			get { return base.Message + type.FullName; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public InstantiationException( string message, Exception root ) : this( message, typeof( InstantiationException ), root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public InstantiationException( string message ) : this( message, typeof( InstantiationException ), new InvalidOperationException( "Invalid Operation" ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public InstantiationException() : this( "Exception occured", typeof( InstantiationException ), new InvalidOperationException( "Invalid Operation" ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected InstantiationException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}