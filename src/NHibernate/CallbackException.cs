using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary></summary>
	[ Serializable ]
	public class CallbackException : HibernateException
	{
		/// <summary></summary>
		public CallbackException() : this( "An exception occured in a callback" )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public CallbackException( Exception root ) : this( "An exception occured in a callback", root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public CallbackException( string message ) : base( message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public CallbackException( string message, Exception e ) : base( message, e )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected CallbackException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}