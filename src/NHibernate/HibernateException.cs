using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Any exception that occurs in the O-R persistence layer.
	/// </summary>
	/// <remarks>Exceptions that occur in the database layer are left as native exceptions</remarks>
	[Serializable]
	public class HibernateException : ApplicationException
	{
		/// <summary>
		/// 
		/// </summary>
		public HibernateException() : base( String.Empty )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public HibernateException( Exception e ) : base( e.Message, e )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public HibernateException( string message, Exception e ) : base( message, e )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public HibernateException( string message ) : base( message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected HibernateException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}