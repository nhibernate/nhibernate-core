using System;

namespace NHibernate.Cache
{
	/// <summary>
	/// Represents any exception from an <see cref="ICache"/>.
	/// </summary>
	[Serializable]
	public class CacheException : HibernateException
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public CacheException( string message ) : base( message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public CacheException( Exception e ) : base( e )
		{
		}
	}

}