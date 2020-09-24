using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache.Access;
using NHibernate.Util;

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
	public partial class ReadWriteCache : IBatchableCacheConcurrencyStrategy
	{
		public interface ILockable
		{
			CacheLock Lock(long timeout, int id);
			bool IsLock { get; }
			bool IsGettable(long txTimestamp);
			bool IsPuttable(long txTimestamp, object newVersion, IComparer comparator);
		}

		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(ReadWriteCache));

		private CacheBase _cache;
		private bool _isDestroyed;
		private int _nextLockId;
		private readonly AsyncReaderWriterLock _asyncReaderWriterLock = new AsyncReaderWriterLock();

		/// <summary>
		/// Gets the cache region name.
		/// </summary>
		public string RegionName
		{
			get { return Cache?.RegionName; }
		}

		// 6.0 TODO: remove
#pragma warning disable 618
		public ICache Cache
#pragma warning restore 618
		{
			get { return _cache; }
			set { _cache = value?.AsCacheBase(); }
		}

		// 6.0 TODO: Rename to Cache and make public (possible breaking change for reader when null).
		private CacheBase InternalCache
		{
			get
			{
				if (_cache == null || _isDestroyed)
					throw new InvalidOperationException(_isDestroyed ? "The cache has already been destroyed" : "The concrete cache is not defined");
				return _cache;
			}
			set => _cache = value;
		}

		// 6.0 TODO: remove
		CacheBase IBatchableCacheConcurrencyStrategy.Cache
		{
			get => _cache;
			set => _cache = value;
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
			var cache = InternalCache;
			using (_asyncReaderWriterLock.ReadLock())
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Cache lookup: {0}", key);
				}

				// commented out in H3.1
				/*try
				{
					cache.Lock( key );*/
				var lockable = (ILockable) cache.Get(key);
				return GetValue(txTimestamp, key, lockable);
				/*}
				finally
				{
					cache.Unlock( key );
				}*/
			}
		}

		public object[] GetMany(CacheKey[] keys, long timestamp)
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache lookup: {0}", string.Join(",", keys.AsEnumerable()));
			}
			var cache = InternalCache;
			var result = new object[keys.Length];
			using (_asyncReaderWriterLock.ReadLock())
			{
				var lockables = cache.GetMany(keys);
				for (var i = 0; i < lockables.Length; i++)
				{
					var o = (ILockable) lockables[i];
					result[i] = GetValue(timestamp, keys[i], o);
				}
			}
			return result;
		}

		private static object GetValue(long timestamp, CacheKey key, ILockable lockable)
		{
			var gettable = lockable != null && lockable.IsGettable(timestamp);

			if (gettable)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Cache hit: {0}", key);
				}

				return ((CachedItem) lockable).Value;
			}

			if (log.IsDebugEnabled())
			{
				log.Debug(lockable == null ? "Cache miss: {0}" : "Cached item was locked: {0}", key);
			}

			return null;
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
			var cache = InternalCache;
			using (_asyncReaderWriterLock.WriteLock())
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Invalidating: {0}", key);
				}

				var lockValue = cache.Lock(key);
				try
				{
					ILockable lockable = (ILockable) cache.Get(key);
					long timeout = cache.NextTimestamp() + cache.Timeout;
					CacheLock @lock = lockable == null ?
					                  CacheLock.Create(timeout, NextLockId(), version) :
					                  lockable.Lock(timeout, NextLockId());
					cache.Put(key, @lock);
					return @lock;
				}
				finally
				{
					cache.Unlock(key, lockValue);
				}
			}
		}

		/// <summary>
		/// Do not add an item to the cache unless the current transaction
		/// timestamp is later than the timestamp at which the item was
		/// invalidated. (Otherwise, a stale item might be re-added if the
		/// database is operating in repeatable read isolation mode.)
		/// </summary>
		/// <returns>Whether the items were actually put into the cache</returns>
		public bool[] PutMany(
			CacheKey[] keys, object[] values, long timestamp, object[] versions, IComparer[] versionComparers,
			bool[] minimalPuts)
		{
			var result = new bool[keys.Length];
			if (timestamp == long.MinValue)
			{
				// MinValue means cache is disabled
				return result;
			}

			var cache = InternalCache;
			using (_asyncReaderWriterLock.WriteLock())
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Caching: {0}", string.Join(",", keys.AsEnumerable()));
				}

				var lockValue = cache.LockMany(keys);
				try
				{
					var putBatch = new Dictionary<object, object>();
					var lockables = cache.GetMany(keys);
					for (var i = 0; i < keys.Length; i++)
					{
						var key = keys[i];
						var version = versions[i];
						var lockable = (ILockable) lockables[i];
						bool puttable = lockable == null ||
						                lockable.IsPuttable(timestamp, version, versionComparers[i]);
						if (puttable)
						{
							putBatch.Add(key, CachedItem.Create(values[i], cache.NextTimestamp(), version));
							if (log.IsDebugEnabled())
							{
								log.Debug("Cached: {0}", key);
							}
							result[i] = true;
						}
						else
						{
							if (log.IsDebugEnabled())
							{
								log.Debug(
									lockable.IsLock ? "Item was locked: {0}" : "Item was already cached: {0}",
									key);
							}
							result[i] = false;
						}
					}

					if (putBatch.Count > 0)
					{
						cache.PutMany(putBatch.Keys.ToArray(), putBatch.Values.ToArray());
					}
				}
				finally
				{
					cache.UnlockMany(keys, lockValue);
				}
			}
			return result;
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

			var cache = InternalCache;
			using (_asyncReaderWriterLock.WriteLock())
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Caching: {0}", key);
				}

				var lockValue = cache.Lock(key);
				try
				{
					ILockable lockable = (ILockable) cache.Get(key);

					bool puttable = lockable == null ||
					                lockable.IsPuttable(txTimestamp, version, versionComparator);

					if (puttable)
					{
						cache.Put(key, CachedItem.Create(value, cache.NextTimestamp(), version));
						if (log.IsDebugEnabled())
						{
							log.Debug("Cached: {0}", key);
						}
						return true;
					}
					else
					{
						if (log.IsDebugEnabled())
						{
							log.Debug(lockable.IsLock ? "Item was locked: {0}" : "Item was already cached: {0}", key);
						}
						return false;
					}
				}
				finally
				{
					cache.Unlock(key, lockValue);
				}
			}
		}

		/// <summary>
		/// decrement a lock and put it back in the cache
		/// </summary>
		private void DecrementLock(object key, CacheLock @lock)
		{
			//decrement the lock
			var cache = InternalCache;
			@lock.Unlock(cache.NextTimestamp());
			cache.Put(key, @lock);
		}

		public void Release(CacheKey key, ISoftLock clientLock)
		{
			var cache = InternalCache;
			using (_asyncReaderWriterLock.WriteLock())
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Releasing: {0}", key);
				}

				var lockValue = cache.Lock(key);
				try
				{
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
					cache.Unlock(key, lockValue);
				}
			}
		}

		internal void HandleLockExpiry(object key)
		{
			log.Warn("An item was expired by the cache while it was locked (increase your cache timeout): {0}", key);
			var cache = InternalCache;
			long ts = cache.NextTimestamp() + cache.Timeout;
			// create new lock that times out immediately
			CacheLock @lock = CacheLock.Create(ts, NextLockId(), null);
			@lock.Unlock(ts);
			cache.Put(key, @lock);
		}

		public void Clear()
		{
			InternalCache.Clear();
		}

		public void Remove(CacheKey key)
		{
			InternalCache.Remove(key);
		}

		public void Destroy()
		{
			if (_isDestroyed)
				return;
			_isDestroyed = true;
			// The cache is externally provided and may be shared. Destroying the cache is
			// not the responsibility of this class.
			_cache = null;
			_asyncReaderWriterLock.Dispose();
		}

		/// <summary>
		/// Re-cache the updated state, if and only if there there are
		/// no other concurrent soft locks. Release our lock.
		/// </summary>
		public bool AfterUpdate(CacheKey key, object value, object version, ISoftLock clientLock)
		{
			var cache = InternalCache;
			using (_asyncReaderWriterLock.WriteLock())
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Updating: {0}", key);
				}

				var lockValue = cache.Lock(key);
				try
				{
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
							cache.Put(key, CachedItem.Create(value, cache.NextTimestamp(), version));
							if (log.IsDebugEnabled())
							{
								log.Debug("Updated: {0}", key);
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
					cache.Unlock(key, lockValue);
				}
			}
		}

		public bool AfterInsert(CacheKey key, object value, object version)
		{
			var cache = InternalCache;
			using (_asyncReaderWriterLock.WriteLock())
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Inserting: {0}", key);
				}

				var lockValue = cache.Lock(key);
				try
				{
					ILockable lockable = (ILockable) cache.Get(key);
					if (lockable == null)
					{
						cache.Put(key, CachedItem.Create(value, cache.NextTimestamp(), version));
						if (log.IsDebugEnabled())
						{
							log.Debug("Inserted: {0}", key);
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
					cache.Unlock(key, lockValue);
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
