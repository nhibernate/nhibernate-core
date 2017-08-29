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
using NHibernate.Cache.Access;

namespace NHibernate.Cache
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class ReadOnlyCache : ICacheConcurrencyStrategy
	{

		public async Task<object> GetAsync(CacheKey key, long timestamp, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object result = await (cache.GetAsync(key, cancellationToken)).ConfigureAwait(false);
			if (result != null && log.IsDebugEnabled)
			{
				log.Debug("Cache hit: " + key);
			}
			return result;	
		}

		/// <summary>
		/// Unsupported!
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

		public async Task<bool> PutAsync(CacheKey key, object value, long timestamp, object version, IComparer versionComparator,
						bool minimalPut, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (timestamp == long.MinValue)
			{
				// MinValue means cache is disabled
				return false;
			}

			if (minimalPut && await (cache.GetAsync(key, cancellationToken)).ConfigureAwait(false) != null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("item already cached: " + key);
				}
				return false;
			}
			if (log.IsDebugEnabled)
			{
				log.Debug("Caching: " + key);
			}
			await (cache.PutAsync(key, value, cancellationToken)).ConfigureAwait(false);
			return true;
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public Task ReleaseAsync(CacheKey key, ISoftLock @lock, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				Release(key, @lock);
				return Task.CompletedTask;
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
			return cache.ClearAsync(cancellationToken);
		}

		public Task RemoveAsync(CacheKey key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return cache.RemoveAsync(key, cancellationToken);
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public Task<bool> AfterUpdateAsync(CacheKey key, object value, object version, ISoftLock @lock, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(AfterUpdate(key, value, version, @lock));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		/// <summary>
		/// Do nothing.
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

		/// <summary>
		/// Do nothing.
		/// </summary>
		public Task EvictAsync(CacheKey key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				Evict(key);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
			// NOOP
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public Task<bool> UpdateAsync(CacheKey key, object value, object currentVersion, object previousVersion, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(Update(key, value, currentVersion, previousVersion));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}
	}
}
