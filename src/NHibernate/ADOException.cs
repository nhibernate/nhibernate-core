using System;
using System.Data;
using System.Runtime.Serialization;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// Wraps exceptions that occur during ADO.NET calls. Exceptions thrown
	/// by various ADO.NET providers are not derived from a common base class
	/// (<c>SQLException</c> in Java), so just <c>Exception</c>
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