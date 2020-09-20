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
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(ReadOnlyCache));

		private CacheBase _cache;
		private bool _isDestroyed;

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

		// 6.0 TODO: make implicit and switch to auto-property
		CacheBase IBatchableCacheConcurrencyStrategy.Cache
		{
			get => _cache;
			set => _cache = value;
		}

		public object Get(CacheKey key, long timestamp)
		{
			CheckCache();
			var result = Cache.Get(key);
			if (log.IsDebugEnabled())
			{
				log.Debug(result != null ? "Cache hit: {0}" : "Cache miss: {0}", key);
			}

			return result;
		}

		public object[] GetMany(CacheKey[] keys, long timestamp)
		{
			CheckCache();
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache lookup: {0}", string.Join(",", keys.AsEnumerable()));
			}

			var results = _cache.GetMany(keys);
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache hit: {0}", string.Join(",", keys.Where((k, i) => results[i] != null)));
				log.Debug("Cache miss: {0}", string.Join(",", keys.Where((k, i) => results[i] == null)));
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

			CheckCache();

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
				var objects = _cache.GetMany(checkKeys.ToArray());
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
			_cache.PutMany(putKeys, putValues);
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

			CheckCache();

			if (minimalPut && Cache.Get(key) != null)
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
			Cache.Put(key, value);
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
			CheckCache();
			Cache.Clear();
		}

		public void Remove(CacheKey key)
		{
			CheckCache();
			Cache.Remove(key);
		}

		public void Destroy()
		{
			// The cache is externally provided and may be shared. Destroying the cache is
			// not the responsibility of this class.
			_isDestroyed = true;
			Cache = null;
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

		private void CheckCache()
		{
			if (_cache == null || _isDestroyed)
				throw new InvalidOperationException(_isDestroyed ? "The cache has already been destroyed" : "The concrete cache is not defined");
		}
	}
}
