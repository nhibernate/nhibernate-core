using System;
using System.Runtime.Serialization;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// A problem occurred trying to lazily initialize a collection or proxy (for example the session
	/// was closed) or iterate query results.
	/// </summary>
	[Serializable]
	public class LazyInitializationException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public LazyInitializationException( Exception root ) : this( root.Message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public LazyInitializationException( string message ) : base( message )
		{
			LogManager.GetLogger( typeof( LazyInitializationException ) ).Error( message, this );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public LazyInitializationException( string message, Exception root ) : this( message + " " + root.Message )
		{
		}

		/// <summary></summary>
		public LazyInitializationException() : this( "LazyInitalizationException" )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected LazyInitializationException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}

	}
}