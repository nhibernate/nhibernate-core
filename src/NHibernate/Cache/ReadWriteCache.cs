using System;

namespace NHibernate.Cache 
{
	/// <summary>
	/// Caches data that is sometimes updated while maintaining "read committed"
	/// isolation level.
	/// </summary>
	/// <remarks>
	/// Works at the "Read Committed" isolation level
	/// </remarks>
	public class ReadWriteCache : ICacheConcurrencyStrategy 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(ReadWriteCache) );
		private readonly object lockObject = new object();
		private ICache _cache;

		public ReadWriteCache() 
		{
		}

		#region ICacheConcurrencyStrategy Members

		public object Get(object key, long txTimestamp) 
		{
			lock( lockObject ) 
			{
				if( log.IsDebugEnabled ) 
				{
					log.Debug("Cache lookup: " + key);
				}

				CachedItem item = _cache.Get(key) as CachedItem;
				if (
					item!=null &&
					item.FreshTimestamp < txTimestamp &&
					item.IsFresh // || txTimestamp < item.LockTimestamp
					) 
				{
					if( log.IsDebugEnabled ) 
					{
						log.Debug( "Cache hit: " + key );
					}
					return item.Value;
				} 
				else 
				{
					if( log.IsDebugEnabled ) 
					{
						log.Debug( "Cache miss: " + key );
					}
					return null;
				}
			}
		}

		//TODO: Actually keep locked CacheItems in a different Hashtable 
		// in this class until unlocked

		public void Lock(object key) 
		{
			lock( lockObject ) 
			{
				if( log.IsDebugEnabled ) 
				{
					log.Debug("Invalidating: " + key);
				}
			
				CachedItem item = _cache.Get( key ) as CachedItem;
				if ( item==null ) item = new CachedItem( null );
				item.Lock();
				_cache.Put( key, item );
			}
		}

		public bool Put(object key, object value, long txTimestamp) 
		{
			lock( lockObject ) 
			{
				if( log.IsDebugEnabled ) 
				{
					log.Debug( "Caching: " + key );
				}
			
				CachedItem item = _cache.Get( key ) as CachedItem;
				if (
					item==null ||
					(item.IsUnlocked && !item.IsFresh && item.UnlockTimestamp < txTimestamp)
					) 
				{
					_cache.Put( key, new CachedItem( value ) );
					if( log.IsDebugEnabled ) 
					{
						log.Debug( "Cached: " + key );
					}
					return true;
				} 
				else
				{
					if( log.IsDebugEnabled ) 
					{
						log.Debug( "Could not cache: " + key );
					}
					return false;
				}
			}
		}

		public void Release(object key) 
		{
			lock( lockObject ) 
			{
				if( log.IsDebugEnabled ) 
				{
					log.Debug( "Releasing: " + key );
				}
				CachedItem item = _cache.Get( key ) as CachedItem;
				if( item!=null ) 
				{
					item.Unlock();
					_cache.Put( key, item );
				} 
				else 
				{
					log.Warn( "An item was expired by the cache while it was locked" );
				}
			}
		}

		public void Clear() 
		{
			_cache.Clear();
		}

		public void Remove(object key) 
		{
			_cache.Remove( key );
		}

		public void Destroy() 
		{
			try 
			{
				_cache.Destroy();
			}
			catch(Exception e) 
			{
				log.Warn( "Could not destroy cache", e );
			}
		}

		public ICache Cache 
		{
			get { return _cache; }
			set { _cache = value; }
		}

		#endregion

	}
}
