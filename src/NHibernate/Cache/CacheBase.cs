using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cache.Entry;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// Base class for letting implementors define a caching algorithm.
	/// </summary>
	/// <remarks>
	/// <threadsafety instance="true" />
	/// <para>
	/// All implementations <em>must</em> be threadsafe.
	/// </para>
	/// <para>
	/// The key is the identifier of the object that is being cached. The key is in most cases
	/// a <see cref="CacheKey" />.
	/// </para>
	/// <para>
	/// The value can be a <see cref="CacheEntry"/>, a <see cref="CollectionCacheEntry"/>,
	/// a <see cref="AnyType.ObjectTypeCacheEntry"/>, an <see cref="IList"/> or
	/// <see cref="IDictionary"/> implementation, all containing simple values or array of
	/// simple values. It can also be directly a simple value or an array of simple values.
	/// And it can be a <see cref="CachedItem"/> containing any of the previous types, or
	/// a <see cref="CacheLock"/>.
	/// </para>
	/// <para>
	/// All those types are binary serializable.
	/// </para>
	/// <para>
	/// This base class provides minimal async method implementations delegating their work to their
	/// synchronous counterparts. Override them for supplying actual async operations.
	/// </para>
	/// <para>
	/// Similarly, this base class provides minimal multiple get/put/lock/unlock implementations
	/// delegating their work to their single operation counterparts. Override them if your cache
	/// implementation supports multiple operations.
	/// </para>
	/// </remarks>
	public abstract partial class CacheBase :
		// 6.0 TODO: remove ICache
#pragma warning disable 618
		ICache
#pragma warning restore 618
	{
		/// <summary>
		/// A reasonable "lock timeout".
		/// </summary>
		public abstract int Timeout { get; }

		/// <summary>
		/// The name of the cache region.
		/// </summary>
		public abstract string RegionName { get; }

		/// <summary>
		/// Should batched get operations be preferred other single get calls?
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property should yield <see langword="false" /> if <see cref="GetMany" /> delegates
		/// its implementation to <see cref="Get"/>.
		/// </para>
		/// <para>
		/// When <see langword="true" />, NHibernate will attempt to get other non initialized proxies or
		/// collections from the cache instead of only getting the proxy or collection which initialization
		/// is asked for. If this cache implementation does not benefit from batching together get operations,
		/// this may result in a performance loss.
		/// </para>
		/// <para>
		/// When <see langword="false" />, NHibernate will still call <see cref="GetMany" /> when it has many
		/// gets to perform. Its <see cref="CacheBase" /> default implementation is adequate for this case.
		/// </para>
		/// </remarks>
		public abstract bool PreferMultipleGet { get; }

		#region Basic abstract operations

		/// <summary>
		/// Get the item from the cache.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <returns>The cached item.</returns>
		public abstract object Get(object key);

		/// <summary>
		/// Put the item into the cache.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <param name="value">The item.</param>
		public abstract void Put(object key, object value);

		/// <summary>
		/// Remove an item from the cache.
		/// </summary>
		/// <param name="key">The item key.</param>
		public abstract void Remove(object key);

		/// <summary>
		/// Clear the cache.
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Clean up.
		/// </summary>
		public abstract void Destroy();

		/// <summary>
		/// Lock the item from being concurrently changed.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <returns>A lock object to use for unlocking the item. Can be <see langword="null" />.</returns>
		/// <remarks>The implementation is allowed to do nothing for non-clustered cache.</remarks>
		public abstract object Lock(object key);

		/// <summary>
		/// Unlock an item which was previously locked.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <param name="lockValue">The lock object to use for unlocking the item, as received from <see cref="Lock"/>.</param>
		/// <remarks>The implementation should do nothing if <see cref="Lock"/> own implementation does nothing.</remarks>
		public abstract void Unlock(object key, object lockValue);

		/// <summary>
		/// Generate a timestamp.
		/// </summary>
		/// <returns>A timestamp.</returns>
		public abstract long NextTimestamp();

		#endregion

		#region Batch operations default implementation

		/// <summary>
		/// Get multiple items from the cache.
		/// </summary>
		/// <param name="keys">The keys to be retrieved from the cache.</param>
		/// <returns>The cached items, matching each key of <paramref name="keys"/> respectively. For each missed key,
		/// it will contain a <see langword="null" />.</returns>
		public virtual object[] GetMany(object[] keys)
		{
			if (keys == null)
				throw new ArgumentNullException(nameof(keys));
			var result = new object[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				result[i] = Get(keys[i]);
			}

			return result;
		}

		/// <summary>
		/// Add multiple items to the cache.
		/// </summary>
		/// <param name="keys">The keys of the items.</param>
		/// <param name="values">The items.</param>
		public virtual void PutMany(object[] keys, object[] values)
		{
			if (keys == null)
				throw new ArgumentNullException(nameof(keys));
			if (values == null)
				throw new ArgumentNullException(nameof(values));
			if (keys.Length != values.Length)
				throw new ArgumentException(
					$"{nameof(keys)} and {nameof(values)} must have the same length. Found {keys.Length} and " +
					$"{values.Length} respectively");

			for (var i = 0; i < keys.Length; i++)
			{
				Put(keys[i], values[i]);
			}
		}

		/// <summary>
		/// Lock the items from being concurrently changed.
		/// </summary>
		/// <param name="keys">The keys of the items.</param>
		/// <returns>A lock object to use for unlocking the items. Can be <see langword="null" />.</returns>
		/// <remarks>The implementation is allowed to do nothing for non-clustered cache.</remarks>
		public virtual object LockMany(object[] keys)
		{
			if (keys == null)
				throw new ArgumentNullException(nameof(keys));
			// When delegating to the single operation lock, agreggate lock values into one.
			var lockValues = new object[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				lockValues[i] = Lock(keys[i]);
			}

			return lockValues;
		}

		/// <summary>
		/// Unlock the items that were previously locked.
		/// </summary>
		/// <param name="keys">The keys of the items.</param>
		/// <param name="lockValue">The lock object to use for unlocking the items, as received from <see cref="Lock"/>.</param>
		/// <remarks>The implementation should do nothing if <see cref="Lock"/> own implementation does nothing.</remarks>
		public virtual void UnlockMany(object[] keys, object lockValue)
		{
			if (keys == null)
				throw new ArgumentNullException(nameof(keys));
			if (!(lockValue is object[] lockValues) || lockValues.Length != keys.Length)
				throw new ArgumentException(
					$"When {nameof(UnlockMany)} is not overriden, {nameof(lockValue)} must be an array of the lock " +
					$"values resulting of sequentially single-locking each key.",
					nameof(lockValue));
			for (var i = 0; i < keys.Length; i++)
			{
				Unlock(keys[i], lockValues[i]);
			}
		}

		#endregion

		#region Async basic operations default implementations

		/// <summary>
		/// Get the item from the cache.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		/// <returns>The cached item.</returns>
		public virtual Task<object> GetAsync(object key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}

			try
			{
				return Task.FromResult(Get(key));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// Put the item into the cache.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <param name="value">The item.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		public virtual Task PutAsync(object key, object value, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}

			try
			{
				Put(key, value);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// Remove an item from the cache.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		public virtual Task RemoveAsync(object key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}

			try
			{
				Remove(key);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// Clear the cache.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		public virtual Task ClearAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}

			try
			{
				Clear();
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// If this is a clustered cache, lock the item.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		/// <returns>A lock object to use for unlocking the key. Can be <see langword="null" />.</returns>
		public virtual Task<object> LockAsync(object key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}

			try
			{
				return Task.FromResult(Lock(key));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary>
		/// If this is a clustered cache, unlock the item.
		/// </summary>
		/// <param name="key">The item key.</param>
		/// <param name="lockValue">The lock object to use for unlocking the key, as received from <see cref="Lock"/>.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		public virtual Task UnlockAsync(object key, object lockValue, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}

			try
			{
				Unlock(key, lockValue);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		#endregion

		#region Obsolete ICache implementation

		void ICache.Lock(object key)
		{
			Lock(key);
		}

		void ICache.Unlock(object key)
		{
			Unlock(key, null);
		}

		// Async Generator does not generate an explicit missing async counterpart when a method
		// differing only by its return type already exists.
		Task ICache.LockAsync(object key, CancellationToken cancellationToken)
		{
			return LockAsync(key, cancellationToken);
		}

		#endregion
	}
}
