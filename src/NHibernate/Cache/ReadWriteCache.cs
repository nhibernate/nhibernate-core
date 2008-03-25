using System;
using System.Collections;
using log4net;

namespace NHibernate.Cache
{
	/// <summary>
	/// Caches data that is sometimes updated while maintaining the semantics of
	/// "read committed" isolation level. If the database is set to "repeatable
	/// read", this concurrency strategy <em>almost</em> maintains the semantics.
	/// Repeatable read isolation is compromised in the case of concurrent writes.
	/// This is an "asynchronous" concurrency strategy.
	/// </summary>
	/// <remarks>
	/// If this strategy is used in a cluster, the underlying cache implementation
	/// must support distributed hard locks (which are held only momentarily). This
	/// strategy also assumes that the underlying cache implementation does not do
	/// asynchronous replication and that state has been fully replicated as soon
	/// as the lock is released.
	/// <seealso cref="NonstrictReadWriteCache"/> for a faster algorithm
	/// <seealso cref="ICacheConcurrencyStrategy"/>
	/// </remarks>
	public class ReadWriteCache : ICacheConcurrencyStrategy
	{
		public interface ILockable
		{
			CacheLock Lock(long timeout, int id);
			bool IsLock { get; }
			bool IsGettable(long txTimestamp);
			bool IsPuttable(long txTimestamp, object newVersion, IComparer comparator);
		}

		private static readonly ILog log = LogManager.GetLogger(typeof(ReadWriteCache));

		private readonly object _lockObject = new object();
		private ICache cache;
		private int _nextLockId;

		public ReadWriteCache()
		{
		}

		/// <summary>
		/// Gets the cache region name.
		/// </summary>
		public string RegionName
		{
			get { return cache.RegionName; }
		}

		public ICache Cache
		{
			get { return cache; }
			set { cache = value; }
		}

		/// <summary>
		/// Generate an id for a new lock. Uniqueness per cache instance is very
		/// desirable but not absolutely critical. Must be called from one of the
		/// synchronized methods of this class.
		/// </summary>
		/// <returns></returns>
		private int NextLockId()
		{
			if (_nextLockId == int.MaxValue)
			{
				_nextLockId = int.MinValue;
			}
			return _nextLockId++;
		}

		/// <summary>
		/// Do not return an item whose timestamp is later than the current
		/// transaction timestamp. (Otherwise we might compromise repeatable
		/// read unnecessarily.) Do not return an item which is soft-locked.
		/// Always go straight to the database instead.
		/// </summary>
		/// <remarks>
		/// Note that since reading an item from that cache does not actually
		/// go to the database, it is possible to see a kind of phantom read
		/// due to the underlying row being updated after we have read it
		/// from the cache. This would not be possible in a lock-based
		/// implementation of repeatable read isolation. It is also possible
		/// to overwrite changes made and committed by another transaction
		/// after the current transaction read the item from the cache. This
		/// problem would be caught by the update-time version-checking, if 
		/// the data is versioned or timestamped.
		/// </remarks>
		public object Get(CacheKey key, long txTimestamp)
		{
			lock (_lockObject)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Cache lookup: " + key);
				}

				// commented out in H3.1
				/*try
				{
					cache.Lock( key );*/

				ILockable lockable = (ILockable) cache.Get(key);

				bool gettable = lockable != null && lockable.IsGettable(txTimestamp);

				if (gettable)
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("Cache hit: " + key);
					}

					return ((CachedItem) lockable).Value;
				}
				else
				{
					if (log.IsDebugEnabled)
					{
						if (lockable == null)
						{
							log.Debug("Cache miss: " + key);
						}
						else
						{
							log.Debug("Cached item was locked: " + key);
						}
					}
					return null;
				}
				/*}
				finally
				{
					cache.Unlock( key );
				}*/
			}
		}

		/// <summary>
		/// Stop any other transactions reading or writing this item to/from
		/// the cache. Send them straight to the database instead. (The lock
		/// does time out eventually.) This implementation tracks concurrent
		/// locks by transactions which simultaneously attempt to write to an
		/// item.
		/// </summary>
		public ISoftLock Lock(CacheKey key, object version)
		{
			lock (_lockObject)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Invalidating: " + key);
				}

				try
				{
					cache.Lock(key);

					ILockable lockable = (ILockable) cache.Get(key);
					long timeout = cache.NextTimestamp() + cache.Timeout;
					CacheLock @lock = lockable == null ?
					                  new CacheLock(timeout, NextLockId(), version) :
					                  lockable.Lock(timeout, NextLockId());
					cache.Put(key, @lock);
					return @lock;
				}
				finally
				{
					cache.Unlock(key);
				}
			}
		}

		/// <summary>
		/// Do not add an item to the cache unless the current transaction
		/// timestamp is later than the timestamp at which the item was
		/// invalidated. (Otherwise, a stale item might be re-added if the
		/// database is operating in repeatable read isolation mode.)
		/// </summary>
		/// <returns>Whether the item was actually put into the cache</returns>
		public bool Put(CacheKey key, object value, long txTimestamp, object version, IComparer versionComparator,
		                bool minimalPut)
		{
			if (txTimestamp == long.MinValue)
			{
				// MinValue means cache is disabled
				return false;
			}

			lock (_lockObject)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Caching: " + key);
				}

				try
				{
					cache.Lock(key);

					ILockable lockable = (ILockable) cache.Get(key);

					bool puttable = lockable == null ||
					                lockable.IsPuttable(txTimestamp, version, versionComparator);

					if (puttable)
					{
						cache.Put(key, new CachedItem(value, cache.NextTimestamp(), version));
						if (log.IsDebugEnabled)
						{
							log.Debug("Cached: " + key);
						}
						return true;
					}
					else
					{
						if (log.IsDebugEnabled)
						{
							if (lockable.IsLock)
							{
								log.Debug("Item was locked: " + key);
							}
							else
							{
								log.Debug("Item was already cached: " + key);
							}
						}
						return false;
					}
				}
				finally
				{
					cache.Unlock(key);
				}
			}
		}

		/// <summary>
		/// decrement a lock and put it back in the cache
		/// </summary>
		private void DecrementLock(object key, CacheLock @lock)
		{
			//decrement the lock
			@lock.Unlock(cache.NextTimestamp());
			cache.Put(key, @lock);
		}

		public void Release(CacheKey key, ISoftLock clientLock)
		{
			lock (_lockObject)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Releasing: " + key);
				}

				try
				{
					cache.Lock(key);

					ILockable lockable = (ILockable) cache.Get(key);
					if (IsUnlockable(clientLock, lockable))
					{
						DecrementLock(key, (CacheLock) lockable);
					}
					else
					{
						HandleLockExpiry(key);
					}
				}
				finally
				{
					cache.Unlock(key);
				}
			}
		}

		internal void HandleLockExpiry(object key)
		{
			log.Warn("An item was expired by the cache while it was locked (increase your cache timeout): " + key);
			long ts = cache.NextTimestamp() + cache.Timeout;
			// create new lock that times out immediately
			CacheLock @lock = new CacheLock(ts, NextLockId(), null);
			@lock.Unlock(ts);
			cache.Put(key, @lock);
		}

		public void Clear()
		{
			cache.Clear();
		}

		public void Remove(CacheKey key)
		{
			cache.Remove(key);
		}

		public void Destroy()
		{
			try
			{
				cache.Destroy();
			}
			catch (Exception e)
			{
				log.Warn("Could not destroy cache", e);
			}
		}

		/// <summary>
		/// Re-cache the updated state, if and only if there there are
		/// no other concurrent soft locks. Release our lock.
		/// </summary>
		public bool AfterUpdate(CacheKey key, object value, object version, ISoftLock clientLock)
		{
			lock (_lockObject)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Updating: " + key);
				}

				try
				{
					cache.Lock(key);

					ILockable lockable = (ILockable) cache.Get(key);
					if (IsUnlockable(clientLock, lockable))
					{
						CacheLock @lock = (CacheLock) lockable;
						if (@lock.WasLockedConcurrently)
						{
							// just decrement the lock, don't recache
							// (we don't know which transaction won)
							DecrementLock(key, @lock);
						}
						else
						{
							//recache the updated state
							cache.Put(key, new CachedItem(value, cache.NextTimestamp(), version));
							if (log.IsDebugEnabled)
							{
								log.Debug("Updated: " + key);
							}
						}
						return true;
					}
					else
					{
						HandleLockExpiry(key);
						return false;
					}
				}
				finally
				{
					cache.Unlock(key);
				}
			}
		}

		public bool AfterInsert(CacheKey key, object value, object version)
		{
			lock (_lockObject)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("Inserting: " + key);
				}

				try
				{
					cache.Lock(key);

					ILockable lockable = (ILockable) cache.Get(key);
					if (lockable == null)
					{
						cache.Put(key, new CachedItem(value, cache.NextTimestamp(), version));
						if (log.IsDebugEnabled)
						{
							log.Debug("Inserted: " + key);
						}
						return true;
					}
					else
					{
						return false;
					}
				}
				finally
				{
					cache.Unlock(key);
				}
			}
		}

		public void Evict(CacheKey key)
		{
			// NOOP
		}

		public bool Insert(CacheKey key, object value, object currentVersion)
		{
			return false;
		}

		public bool Update(CacheKey key, object value, object currentVersion, object previousVersion)
		{
			return false;
		}

		/// <summary>
		/// Is the client's lock commensurate with the item in the cache?
		/// If it is not, we know that the cache expired the original
		/// lock.
		/// </summary>
		private bool IsUnlockable(ISoftLock clientLock, ILockable myLock)
		{
			//null clientLock is remotely possible but will never happen in practice
			return myLock != null &&
			       myLock.IsLock &&
			       clientLock != null &&
			       ((CacheLock) clientLock).Id == ((CacheLock) myLock).Id;
		}
	}
}