using System;
using System.Threading.Tasks;
using NHibernate.Util;

namespace NHibernate.Cache
{
	public interface ICacheLock : IDisposable
	{
		IDisposable ReadLock();
		IDisposable WriteLock();
		Task<IDisposable> ReadLockAsync();
		Task<IDisposable> WriteLockAsync();
	}

	public interface ICacheReadWriteLockFactory
	{
		ICacheLock Create();
	}

	class AsyncCacheReadWriteLockFactory : ICacheReadWriteLockFactory
	{
		public ICacheLock Create()
		{
			return new AsyncReaderWriterLock();
		}
	}

	class SyncCacheReadWriteLockFactory : ICacheReadWriteLockFactory
	{
		public ICacheLock Create()
		{
			return new SyncCacheLock();
		}
	}
}
