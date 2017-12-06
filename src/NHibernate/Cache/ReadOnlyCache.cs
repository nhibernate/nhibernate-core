using System;
using System.Collections;
using NHibernate.Cache.Access;

namespace NHibernate.Cache
{
	/// <summary>
	/// Caches data that is never updated
	/// </summary>
	public partial class ReadOnlyCache : ICacheConcurrencyStrategy
	{
		private ICache cache;
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
			set { cache = value; }
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

		/// <summary>
		/// Unsupported!
		/// </summary>
		public ISoftLock Lock(CacheKey key, object version)
		{
			log.Error("Application attempted to edit read only item: {0}", key);
			throw new InvalidOperationException("ReadOnlyCache: Can't write to a readonly object " + key.EntityOrRoleName);
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
