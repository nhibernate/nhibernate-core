using System;
using System.Collections;

using log4net;

namespace NHibernate.Cache
{
	/// <summary>
	/// Caches data that is sometimes updated without ever locking the cache. 
	/// If concurrent access to an item is possible, this concurrency strategy 
	/// makes no guarantee that the item returned from the cache is the latest 
	/// version available in the database. Configure your cache timeout accordingly! 
	/// This is an "asynchronous" concurrency strategy.
	/// <seealso cref="ReadWriteCache"/> for a much stricter algorithm
	/// </summary>
	public class NonstrictReadWriteCache : ICacheConcurrencyStrategy
	{
		private ICache cache;

		private static readonly ILog log = LogManager.GetLogger( typeof( NonstrictReadWriteCache ) );

		/// <summary></summary>
		public NonstrictReadWriteCache()
		{
		}

		/// <summary>
		/// Gets the cache region name.
		/// </summary>
		public string RegionName
		{
			get { return cache.RegionName; }
		}

		/// <summary></summary>
		public ICache Cache
		{
			get { return cache; }
			set { cache = value; }
		}

		/// <summary>
		/// Get the most recent version, if available.
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

			object result = cache.Get( key );
			if( result != null )
			{
				log.Debug( "Cache hit" );
			}
			else
			{
				log.Debug( "Cache miss" );
			}
			return result;
		}

		/// <summary>
		/// Add an item to the cache
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="txTimestamp"></param>
		/// <returns></returns>
		public bool Put( object key, object value, long txTimestamp, object version, IComparer versionComparator, bool minimalPut )
		{
			if( txTimestamp == long.MinValue )
			{
				// MinValue means cache is disabled
				return false;
			}

			if( minimalPut && cache.Get( key ) != null )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "item already cached: " + key );
				}
				return false;
			}
			if( log.IsDebugEnabled )
			{
				log.Debug( "Caching: " + key );
			}
			cache.Put( key, value );
			return true;
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		/// <param name="key"></param>
		/// <param name="version"></param>
		public ISoftLock Lock( object key, object version )
		{
			return null;
		}

		public void Remove( object key )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Removing: " + key );
			}
			cache.Remove( key );
		}

		public void Clear()
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Clearing" );
			}
			cache.Clear();
		}

		/// <summary></summary>
		public void Destroy()
		{
			try
			{
				cache.Destroy();
			}
			catch( Exception e )
			{
				log.Warn( "Could not destroy cache", e );
			}
		}

		/// <summary>
		/// Invalidate the item
		/// </summary>
		/// <param name="key"></param>
		public void Evict( object key )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Invalidating: " + key );
			}
			cache.Remove( key );
		}

		/// <summary>
		/// Invalidate the item
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Update( object key, object value )
		{
			Evict( key );
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Insert( object key, object value )
		{
		}

		/// <summary>
		/// Invalidate the item (again, for safety).
		/// </summary>
		/// <param name="key"></param>
		/// <param name="lock"></param>
		public void Release( object key, ISoftLock @lock )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Invalidating (again): " + key );
			}

			cache.Remove( key );
		}

		/// <summary>
		/// Invalidate the item (again, for safety).
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="version"></param>
		/// <param name="lock"></param>
		public void AfterUpdate( object key, object value, object version, ISoftLock @lock )
		{
			Release( key, @lock );
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="version"></param>
		public void AfterInsert( object key, object value, object version )
		{
		}
	}
}
