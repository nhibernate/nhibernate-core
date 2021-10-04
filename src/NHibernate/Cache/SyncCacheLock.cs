using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Cache
{
	class SyncCacheLock : ICacheLock
	{
		class MonitorLock : IDisposable
		{
			private readonly object _lockObj;

			public MonitorLock(object lockObj)
			{
				Monitor.Enter(lockObj);
				_lockObj = lockObj;
			}

			public void Dispose()
			{
				Monitor.Exit(_lockObj);
			}
		}

		public void Dispose()
		{
		}

		public IDisposable ReadLock()
		{
			return new MonitorLock(this);
		}

		public IDisposable WriteLock()
		{
			return new MonitorLock(this);
		}

		public Task<IDisposable> ReadLockAsync()
		{
			throw new InvalidOperationException("This locker supports only sync operations.");
		}

		public Task<IDisposable> WriteLockAsync()
		{
			throw new InvalidOperationException("This locker supports only sync operations.");
		}
	}
}
