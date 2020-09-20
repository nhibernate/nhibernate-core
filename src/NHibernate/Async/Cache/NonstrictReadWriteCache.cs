﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache.Access;

namespace NHibernate.Cache
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class NonstrictReadWriteCache : IBatchableCacheConcurrencyStrategy
	{

		/// <summary>
		/// Get the most recent version, if available.
		/// </summary>
		public async Task<object> GetAsync(CacheKey key, long txTimestamp, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			CheckCache();
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache lookup: {0}", key);
			}

			var result = await (Cache.GetAsync(key, cancellationToken)).ConfigureAwait(false);
			if (log.IsDebugEnabled())
			{
				log.Debug(result != null ? "Cache hit: {0}" : "Cache miss: {0}", key);
			}

			return result;
		}

		public async Task<object[]> GetManyAsync(CacheKey[] keys, long timestamp, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			CheckCache();
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache lookup: {0}", string.Join(",", keys.AsEnumerable()));
			}

			var results = await (_cache.GetManyAsync(keys, cancellationToken)).ConfigureAwait(false);
			if (log.IsDebugEnabled())
			{
				log.Debug("Cache hit: {0}", string.Join(",", keys.Where((k, i) => results != null)));
				log.Debug("Cache miss: {0}", string.Join(",", keys.Where((k, i) => results == null)));
			}

			return results;
		}

		/// <summary>
		/// Add multiple items to the cache
		/// </summary>
		public async Task<bool[]> PutManyAsync(
			CacheKey[] keys, object[] values, long timestamp, object[] versions, IComparer[] versionComparers,
			bool[] minimalPuts, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var result = new bool[keys.Length];
			if (timestamp == long.MinValue)
			{
				// MinValue means cache is disabled
				return result;
			}

			CheckCache();

			var checkKeys = new List<object>();
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
				var objects = await (_cache.GetManyAsync(checkKeys.ToArray(), cancellationToken)).ConfigureAwait(false);
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
			await (_cache.PutManyAsync(putKeys, putValues, cancellationToken)).ConfigureAwait(false);
			return result;
		}

		/// <summary>
		/// Add an item to the cache
		/// </summary>
		public async Task<bool> PutAsync(CacheKey key, object value, long txTimestamp, object version, IComparer versionComparator,
		                bool minimalPut, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (txTimestamp == long.MinValue)
			{
				// MinValue means cache is disabled
				return false;
			}

			CheckCache();

			if (minimalPut && await (Cache.GetAsync(key, cancellationToken)).ConfigureAwait(false) != null)
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
			await (Cache.PutAsync(key, value, cancellationToken)).ConfigureAwait(false);
			return true;
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		public Task<ISoftLock> LockAsync(CacheKey key, object version, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<ISoftLock>(cancellationToken);
			}
			try
			{
				return Task.FromResult<ISoftLock>(Lock(key, version));
			}
			catch (Exception ex)
			{
				return Task.FromException<ISoftLock>(ex);
			}
		}

		public Task RemoveAsync(CacheKey key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				CheckCache();
				if (log.IsDebugEnabled())
				{
					log.Debug("Removing: {0}", key);
				}
				return Cache.RemoveAsync(key, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public Task ClearAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				CheckCache();
				if (log.IsDebugEnabled())
				{
					log.Debug("Clearing");
				}
				return Cache.ClearAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// Invalidate the item
		/// </summary>
		public Task EvictAsync(CacheKey key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				CheckCache();
				if (log.IsDebugEnabled())
				{
					log.Debug("Invalidating: {0}", key);
				}
				return Cache.RemoveAsync(key, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// Invalidate the item
		/// </summary>
		public async Task<bool> UpdateAsync(CacheKey key, object value, object currentVersion, object previousVersion, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await (EvictAsync(key, cancellationToken)).ConfigureAwait(false);
			return false;
		}

		/// <summary>
		/// Invalidate the item (again, for safety).
		/// </summary>
		public Task ReleaseAsync(CacheKey key, ISoftLock @lock, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				CheckCache();
				if (log.IsDebugEnabled())
				{
					log.Debug("Invalidating (again): {0}", key);
				}

				return Cache.RemoveAsync(key, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// Invalidate the item (again, for safety).
		/// </summary>
		public async Task<bool> AfterUpdateAsync(CacheKey key, object value, object version, ISoftLock @lock, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await (ReleaseAsync(key, @lock, cancellationToken)).ConfigureAwait(false);
			return false;
		}

		/// <summary>
		/// Do nothing
		/// </summary>
		public Task<bool> AfterInsertAsync(CacheKey key, object value, object version, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(AfterInsert(key, value, version));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}
	}
}
