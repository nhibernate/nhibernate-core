using System;
using System.Data;
using System.Runtime.Serialization;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// Wraps an <c>DataException</c>. Indicates that an exception occurred during an ADO.NET call.
	/// </summary>
	[ Serializable ]
	public class ADOException : HibernateException
	{
		/// <summary></summary>
		public ADOException() : this( "DataException occured", new InvalidOperationException( "Invalid Operation" ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ADOException( string message ) : this( message, new InvalidOperationException( "Invalid Operation" ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public ADOException( DataException root ) : this( "DataException occurred", root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public ADOException( string message, Exception root ) : base( message, root )
		{
			LogManager.GetLogger( typeof( ADOException ) ).Error( message, root );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ADOException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}

	}
}