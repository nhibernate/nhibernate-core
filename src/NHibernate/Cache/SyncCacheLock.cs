using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Cache
{
	class SyncCacheLock : ICacheLock
	{
		private readonly MonitorLock _monitorLock;

		class MonitorLock : IDisposable
		{
			private readonly Lock _lockObj = new Lock();

			public IDisposable Lock()
			{
				_lockObj.Enter();
				return this;
			}

			public void Dispose()
			{
				_lockObj.Exit();
			}
		}

		public SyncCacheLock()
		{
			_monitorLock = new();
		}

		public void Dispose()
		{
		}

		public IDisposable ReadLock()
		{
			return _monitorLock.Lock();
		}

		public IDisposable WriteLock()
		{
			return _monitorLock.Lock();
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
