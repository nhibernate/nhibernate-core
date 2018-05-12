using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache.Access;

namespace NHibernate.Cache
{
	/// <summary>
	/// Caches data that is never updated
	/// </summary>
	public partial class ReadOnlyCache : IBatchableCacheConcurrencyStrategy
	{
		private ICache cache;
		private IBatchableReadCache _batchableReadCache;
		private IBatchableReadWriteCache _batchableReadWriteCache;
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(ReadOnlyCache));

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

		public object Get(CacheKey key, long timestamp)
		{
			object result = cache.Get(key);
			if (result != null && log.IsDebugEnabled())
			{
				log.Debug("Cache hit: {0}", key);
			}
			return result;	
		}

		public object[] GetMany(CacheKey[] keys, long timestamp)
		{
			if (_batchableReadCache == null)
			{
				throw new InvalidOperationException($"Cache {cache.GetType()} does not support batching get operation");
			}
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache lookup: {0}", string.Join(",", keys.AsEnumerable()));
			}
			var results = _batchableReadCache.GetMany(keys.Select(o => (object) o).ToArray());
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
		/// Unsupported!
		/// </summary>
		public ISoftLock Lock(CacheKey key, object version)
		{
			log.Error("Application attempted to edit read only item: {0}", key);
			throw new InvalidOperationException("ReadOnlyCache: Can't write to a readonly object " + key.EntityOrRoleName);
		}

		public bool[] PutMany(CacheKey[] keys, object[] values, long timestamp, object[] versions, IComparer[] versionComparers,
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
				var objects = _batchableReadWriteCache.GetMany(checkKeys.Select(o => (object) o).ToArray());
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
			_batchableReadWriteCache.PutMany(putKeys, putValues);
			return result;
		}

		public bool Put(CacheKey key, object value, long timestamp, object version, IComparer versionComparator,
						bool minimalPut)
		{
			if (timestamp == long.MinValue)
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
		/// Unsupported!
		/// </summary>
		public void Release(CacheKey key, ISoftLock @lock)
		{
			log.Error("Application attempted to edit read only item: {0}", key);
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
				log.Warn(e, "Could not destroy cache");
			}
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public bool AfterUpdate(CacheKey key, object value, object version, ISoftLock @lock)
		{
			log.Error("Application attempted to edit read only item: {0}", key);
			throw new InvalidOperationException("ReadOnlyCache: Can't write to a readonly object " + key.EntityOrRoleName);
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		public bool AfterInsert(CacheKey key, object value, object version)
		{
			// Ignore
			return true;
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		public void Evict(CacheKey key)
		{
			// NOOP
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		public bool Insert(CacheKey key, object value, object currentVersion)
		{
			return false;
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public bool Update(CacheKey key, object value, object currentVersion, object previousVersion)
		{
			log.Error("Application attempted to edit read only item: {0}", key);
			throw new InvalidOperationException("ReadOnlyCache: Can't write to a readonly object " + key.EntityOrRoleName);
		}
	}
}
