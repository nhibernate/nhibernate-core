using System;
using System.Threading.Tasks;
using NHibernate.Util;

namespace NHibernate.Cache
{
	/// <summary>
	/// Implementors provide a locking mechanism for the cache.
	/// </summary>
	public interface ICacheLock : IDisposable
	{
		/// <summary>
		/// Acquire synchronously a read lock.
		/// </summary>
		/// <returns>A read lock.</returns>
		IDisposable ReadLock();

		/// <summary>
		/// Acquire synchronously a write lock.
		/// </summary>
		/// <returns>A write lock.</returns>
		IDisposable WriteLock();

		/// <summary>
		/// Acquire asynchronously a read lock.
		/// </summary>
		/// <returns>A read lock.</returns>
		Task<IDisposable> ReadLockAsync();

		/// <summary>
		/// Acquire asynchronously a write lock.
		/// </summary>
		/// <returns>A write lock.</returns>
		Task<IDisposable> WriteLockAsync();
	}

	/// <summary>
	/// Define a factory for cache locks.
	/// </summary>
	public interface ICacheReadWriteLockFactory
	{
		/// <summary>
		/// Create a cache lock provider.
		/// </summary>
		ICacheLock Create();
	}

	internal class AsyncCacheReadWriteLockFactory : ICacheReadWriteLockFactory
	{
		public ICacheLock Create()
		{
			return new AsyncReaderWriterLock();
		}
	}

	internal class SyncCacheReadWriteLockFactory : ICacheReadWriteLockFactory
	{
		public ICacheLock Create()
		{
			return new SyncCacheLock();
		}
	}
}
