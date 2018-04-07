using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache.Access;

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
	public partial class NonstrictReadWriteCache : ICacheConcurrencyStrategy
	{
		private ICache cache;
		private IBatchableReadCache _batchableReadCache;
		private IBatchableReadWriteCache _batchableReadWriteCache;

		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(NonstrictReadWriteCache));

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
			set
			{
				cache = value;
				// ReSharper disable once SuspiciousTypeConversion.Global
				_batchableReadCache = value as IBatchableReadCache;
				_batchableReadWriteCache = value as IBatchableReadWriteCache;
			}
		}

		/// <summary>
		/// Get the most recent version, if available.
		/// </summary>
		public object Get(CacheKey key, long txTimestamp)
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache lookup: {0}", key);
			}

			object result = cache.Get(key);
			if (result != null)
			{
				log.Debug("Cache hit");
			}
			else
			{
				log.Debug("Cache miss");
			}
			return result;
		}

		public object[] GetMultiple(CacheKey[] keys, long txTimestamp)
		{
			if (_batchableReadCache == null)
			{
				throw new InvalidOperationException($"Cache {cache.GetType()} does not support batching get operation");
			}
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache lookup: {0}", string.Join(",", keys.AsEnumerable()));
			}
			var results = _batchableReadCache.GetMultiple(keys.Select(o => (object) o).ToArray());
			if (!log.IsDebugEnabled())
			{
				return results;
			}
			for (var i = 0; i < keys.Length; i++)
			{
				log.Debug(results[i] != null ? $"Cache hit: {keys[i]}" : $"Cache miss: {keys[i]}");
			}
			return results;
		}

		/// <summary>
		/// Add multiple items to the cache
		/// </summary>
		public bool[] PutMultiple(CacheKey[] keys, object[] values, long timestamp, object[] versions, IComparer[] versionComparers,
		                          bool[] minimalPuts)
		{
			if (_batchableReadWriteCache == null)
			{
				throw new InvalidOperationException($"Cache {cache.GetType()} does not support batching operations");
			}
			var result = new bool[keys.Length];
			if (timestamp == long.MinValue)
			{
				// MinValue means cache is disabled
				return result;
			}

			var checkKeys = new List<CacheKey>();
			var checkKeyIndexes = new List<int>();
			for (var i = 0; i < minimalPuts.Length; i++)
			{
				if (minimalPuts[i])
				{
					checkKeys.Add(keys[i]);
					checkKeyIndexes.Add(i);
				}
			}
			var skipKeyIndexes = new HashSet<int>();
			if (checkKeys.Any())
			{
				var objects = _batchableReadWriteCache.GetMultiple(checkKeys.ToArray());
				for (var i = 0; i < objects.Length; i++)
				{
					if (objects[i] != null)
					{
						if (log.IsDebugEnabled())
						{
							log.Debug("item already cached: {0}", checkKeys[i]);
						}
						skipKeyIndexes.Add(checkKeyIndexes[i]);
					}
				}
			}

			if (skipKeyIndexes.Count == keys.Length)
			{
				return result;
			}

			var putKeys = new object[keys.Length - skipKeyIndexes.Count];
			var putValues = new object[putKeys.Length];
			var j = 0;
			for (var i = 0; i < keys.Length; i++)
			{
				if (skipKeyIndexes.Contains(i))
				{
					continue;
				}
				putKeys[j] = keys[i];
				putValues[j++] = values[i];
				result[i] = true;
			}
			_batchableReadWriteCache.PutMultiple(putKeys, putValues);
			return result;
		}

		/// <summary>
		/// Add an item to the cache
		/// </summary>
		public bool Put(CacheKey key, object value, long txTimestamp, object version, IComparer versionComparator,
		                bool minimalPut)
		{
			if (txTimestamp == long.MinValue)
			{
				// MinValue means cache is disabled
				return false;
			}

			if (minimalPut && cache.Get(key) != null)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("item already cached: {0}", key);
				}
				return false;
			}
			if (log.IsDebugEnabled())
			{
				log.Debug("Caching: {0}", key);
			}
			cache.Put(key, value);
			return true;
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		public ISoftLock Lock(CacheKey key, object version)
		{
			return null;
		}

		public void Remove(CacheKey key)
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("Removing: {0}", key);
			}
			cache.Remove(key);
		}

		public void Clear()
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("Clearing");
			}
			cache.Clear();
		}

		public void Destroy()
		{
			try
			{
				cache.Destroy();
			}
			catch (Exception e)
			{
				log.Warn(e, "Could not destroy cache");
			}
		}

		/// <summary>
		/// Invalidate the item
		/// </summary>
		public void Evict(CacheKey key)
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("Invalidating: {0}", key);
			}
			cache.Remove(key);
		}

		/// <summary>
		/// Invalidate the item
		/// </summary>
		public bool Update(CacheKey key, object value, object currentVersion, object previousVersion)
		{
			Evict(key);
			return false;
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		public bool Insert(CacheKey key, object value, object currentVersion)
		{
			return false;
		}

		/// <summary>
		/// Invalidate the item (again, for safety).
		/// </summary>
		public void Release(CacheKey key, ISoftLock @lock)
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("Invalidating (again): {0}", key);
			}

			cache.Remove(key);
		}

		/// <summary>
		/// Invalidate the item (again, for safety).
		/// </summary>
		public bool AfterUpdate(CacheKey key, object value, object version, ISoftLock @lock)
		{
			Release(key, @lock);
			return false;
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		public bool AfterInsert(CacheKey key, object value, object version)
		{
			return false;
		}
	}
}
