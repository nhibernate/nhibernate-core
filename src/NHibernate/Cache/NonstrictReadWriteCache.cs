using System;
using log4net;

namespace NHibernate.Cache
{
	/// <summary>
	/// Summary description for NonstrictReadWriteCache.
	/// </summary>
	public class NonstrictReadWriteCache : ICacheConcurrencyStrategy
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( NonstrictReadWriteCache ) );
		private static readonly long timeout = 10000;

		private ICache _cache;

		/// <summary></summary>
		public NonstrictReadWriteCache()
		{
		}

		#region ICacheConcurrencyStrategy Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="txTimestamp"></param>
		/// <returns></returns>
		public object Get( object key, long txTimestamp )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Cache lookup: " + key );
			}

			object result = _cache.Get( key );
			if( result != null & !( result is Int64 ) )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Cache hit" );
				}
				return result;
			}
			else
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Cache miss" );
				}
			}

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="txTimestamp"></param>
		/// <returns></returns>
		public bool Put( object key, object value, long txTimestamp )
		{
			object result = _cache.Get( key );
			if( result == null )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Caching new: " + key );
				}
			}
			else if( ( result is Int64 ) && ( ( Int64 ) result < txTimestamp/Timestamper.OneMs ) )
			{
				// note that this is not guaranteed to be correct in a cluster
				// because system times could be inconsistent
				if( log.IsDebugEnabled )
				{
					log.Debug( "Caching invalidated: " + key );
				}
			}
			else
			{
				return false; // note early exit
			}

			_cache.Put( key, value );
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Lock( object key )
		{
			// in case the server crashes, we need the lock to timeout
			_cache.Put( key, ( timeout + Timestamper.Next()/Timestamper.OneMs ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Release( object key )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Invalidating: " + key );
			}

			//remove the lock (any later transactions can recache)
			_cache.Put( key, Timestamper.Next()/Timestamper.OneMs );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Remove( object key )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Removing: " + key );
			}
			_cache.Remove( key );
		}

		/// <summary></summary>
		public void Clear()
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Clearing" );
			}
			_cache.Clear();
		}

		/// <summary></summary>
		public void Destroy()
		{
			try
			{
				_cache.Destroy();
			}
			catch( Exception e )
			{
				log.Warn( "Could not destroy cache", e );
			}
		}

		/// <summary></summary>
		public ICache Cache
		{
			get { return _cache; }
			set { _cache = value; }
		}

		#endregion
	}
}