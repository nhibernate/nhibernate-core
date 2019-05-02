using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	// Idea from:
	// https://github.com/kpreisser/AsyncReaderWriterLockSlim
	// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-7-asyncreaderwriterlock/
	internal class AsyncReaderWriterLock
	{
		private readonly SemaphoreSlim _writeLockSemaphore = new SemaphoreSlim(1, 1);
		private readonly SemaphoreSlim _readLockSemaphore = new SemaphoreSlim(0, 1);
		private readonly IDisposable _writerReleaser;
		private readonly IDisposable _readerReleaser;
		private readonly Task<IDisposable> _readerReleaserTask;
		private SemaphoreSlim _waitingReadLockSemaphore;
		private int _readersWaiting;
		private int _currentReaders;
		private int _writersWaiting;

		public AsyncReaderWriterLock()
		{
			_writerReleaser = new Releaser(this, true);
			_readerReleaser = new Releaser(this, false);
			_readerReleaserTask = Task.FromResult(_readerReleaser);
		}

		public IDisposable WriteLock()
		{
			if (!CanEnterWriteLock(out var waitForReadLocks))
			{
				_writeLockSemaphore.Wait();
				lock (_writeLockSemaphore)
				{
					_writersWaiting--;
				}
			}

			if (waitForReadLocks)
			{
				_readLockSemaphore.Wait();
			}

			return _writerReleaser;
		}

		public async Task<IDisposable> WriteLockAsync()
		{
			if (!CanEnterWriteLock(out var waitForReadLocks))
			{
				await _writeLockSemaphore.WaitAsync().ConfigureAwait(false);
				lock (_writeLockSemaphore)
				{
					_writersWaiting--;
				}
			}

			if (waitForReadLocks)
			{
				await _readLockSemaphore.WaitAsync().ConfigureAwait(false);
			}

			return _writerReleaser;
		}

		public IDisposable ReadLock()
		{
			if (CanEnterReadLock())
			{
				return _readerReleaser;
			}

			_waitingReadLockSemaphore.Wait();
			ReleaseWaitingReader();

			return _readerReleaser;
		}

		public Task<IDisposable> ReadLockAsync()
		{
			return CanEnterReadLock() ? _readerReleaserTask : ReadLockInternalAsync();

			async Task<IDisposable> ReadLockInternalAsync()
			{
				await _waitingReadLockSemaphore.WaitAsync().ConfigureAwait(false);
				ReleaseWaitingReader();

				return _readerReleaser;
			}
		}

		private bool CanEnterWriteLock(out bool waitForReadLocks)
		{
			waitForReadLocks = false;
			lock (_writeLockSemaphore)
			{
				if (_writeLockSemaphore.CurrentCount > 0 && _writeLockSemaphore.Wait(0))
				{
					waitForReadLocks = _currentReaders > 0;
					return true;
				}

				_writersWaiting++;
			}

			return false;
		}

		private void ExitWriteLock()
		{
			lock (_writeLockSemaphore)
			{
				if (_writeLockSemaphore.CurrentCount == 1)
				{
					throw new InvalidOperationException();
				}

				// Writers have the highest priority even if they came last
				if (_writersWaiting > 0 || _waitingReadLockSemaphore == null)
				{
					_writeLockSemaphore.Release();
					return;
				}

				if (_readersWaiting > 0)
				{
					_currentReaders += _readersWaiting;
					_waitingReadLockSemaphore.Release(_readersWaiting);
				}

				_writeLockSemaphore.Release();
			}
		}

		private bool CanEnterReadLock()
		{
			lock (_writeLockSemaphore)
			{
				if (_writersWaiting == 0 && _writeLockSemaphore.CurrentCount > 0)
				{
					_currentReaders++;

					return true;
				}

				if (_waitingReadLockSemaphore == null)
				{
					_waitingReadLockSemaphore = new SemaphoreSlim(0);
				}

				_readersWaiting++;

				return false;
			}
		}

		private void ExitReadLock()
		{
			lock (_writeLockSemaphore)
			{
				if (_currentReaders == 0)
				{
					throw new InvalidOperationException();
				}

				_currentReaders--;
				if (_currentReaders == 0 && _writeLockSemaphore.CurrentCount == 0)
				{
					_readLockSemaphore.Release();
				}
			}
		}

		private void ReleaseWaitingReader()
		{
			lock (_writeLockSemaphore)
			{
				_readersWaiting--;
				if (_readersWaiting != 0)
				{
					return;
				}

				_waitingReadLockSemaphore.Dispose();
				_waitingReadLockSemaphore = null;
			}
		}

		private sealed class Releaser : IDisposable
		{
			private readonly AsyncReaderWriterLock _toRelease;
			private readonly bool _writer;

			internal Releaser(AsyncReaderWriterLock toRelease, bool writer)
			{
				_toRelease = toRelease;
				_writer = writer;
			}

			public void Dispose()
			{
				if (_toRelease == null)
				{
					return;
				}

				if (_writer)
				{
					_toRelease.ExitWriteLock();
				}
				else
				{
					_toRelease.ExitReadLock();
				}
			}
		}
	}
}
