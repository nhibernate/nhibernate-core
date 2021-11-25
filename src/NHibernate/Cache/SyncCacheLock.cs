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
			throw AsyncNotSupporteException();
		}

		public Task<IDisposable> WriteLockAsync()
		{
			throw AsyncNotSupporteException();
		}

		private static InvalidOperationException AsyncNotSupporteException()
		{
			return new InvalidOperationException("This locker supports only sync operations.  Change 'cache.read_write_lock_factory' setting to `async` to support async operations.");
		}
	}
}
