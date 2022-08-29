using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	// Idea from:
	// https://github.com/kpreisser/AsyncReaderWriterLockSlim
	// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-7-asyncreaderwriterlock/
	internal class AsyncReaderWriterLock : IDisposable, Cache.ICacheLock
	{
		private readonly SemaphoreSlim _writeLockSemaphore = new SemaphoreSlim(1, 1);
		private readonly SemaphoreSlim _readLockSemaphore = new SemaphoreSlim(0, 1);
		private readonly Releaser _writerReleaser;
		private readonly Releaser _readerReleaser;
		private readonly Task<Releaser> _readerReleaserTask;
		private SemaphoreSlim _waitingReadLockSemaphore;
		private SemaphoreSlim _waitingDisposalSemaphore;
		private int _readersWaiting;
		private int _currentReaders;
		private int _writersWaiting;
		private bool _disposed;

		public AsyncReaderWriterLock()
		{
			_writerReleaser = new Releaser(this, true);
			_readerReleaser = new Releaser(this, false);
			_readerReleaserTask = Task.FromResult(_readerReleaser);
		}

		internal int CurrentReaders => _currentReaders;

		internal int WritersWaiting => _writersWaiting;

		internal int ReadersWaiting => _readersWaiting;

		internal bool Writing => _currentReaders == 0 && _writeLockSemaphore.CurrentCount == 0;

		internal bool AcquiredWriteLock => _writeLockSemaphore.CurrentCount == 0;

		IDisposable Cache.ICacheLock.WriteLock()
		{
			return WriteLock();
		}

		public Releaser WriteLock()
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

			DisposeWaitingSemaphore();

			return _writerReleaser;
		}

		async Task<IDisposable> Cache.ICacheLock.WriteLockAsync()
		{
			return await WriteLockAsync().ConfigureAwait(false);
		}

		public async Task<Releaser> WriteLockAsync()
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

			DisposeWaitingSemaphore();

			return _writerReleaser;
		}

		IDisposable Cache.ICacheLock.ReadLock()
		{
			return ReadLock();
		}

		public Releaser ReadLock()
		{
			if (CanEnterReadLock(out var waitingReadLockSemaphore))
			{
				return _readerReleaser;
			}

			waitingReadLockSemaphore.Wait();

			return _readerReleaser;
		}

		async Task<IDisposable> Cache.ICacheLock.ReadLockAsync()
		{
			return await ReadLockAsync().ConfigureAwait(false);
		}

		public Task<Releaser> ReadLockAsync()
		{
			return CanEnterReadLock(out var waitingReadLockSemaphore) ? _readerReleaserTask : ReadLockInternalAsync();

			async Task<Releaser> ReadLockInternalAsync()
			{
				await waitingReadLockSemaphore.WaitAsync().ConfigureAwait(false);

				return _readerReleaser;
			}
		}

		public void Dispose()
		{
			lock (_writeLockSemaphore)
			{
				_writeLockSemaphore.Dispose();
				_readLockSemaphore.Dispose();
				_waitingReadLockSemaphore?.Dispose();
				_waitingDisposalSemaphore?.Dispose();
				_disposed = true;
			}
		}

		private bool CanEnterWriteLock(out bool waitForReadLocks)
		{
			waitForReadLocks = false;
			lock (_writeLockSemaphore)
			{
				AssertNotDisposed();
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
				AssertNotDisposed();
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
					_readersWaiting = 0;
					// We have to dispose the waiting read lock only after all readers finished using it
					_waitingDisposalSemaphore = _waitingReadLockSemaphore;
					_waitingReadLockSemaphore = null;
				}

				_writeLockSemaphore.Release();
			}
		}

		private bool CanEnterReadLock(out SemaphoreSlim waitingReadLockSemaphore)
		{
			lock (_writeLockSemaphore)
			{
				AssertNotDisposed();
				if (_writersWaiting == 0 && _writeLockSemaphore.CurrentCount > 0)
				{
					_currentReaders++;
					waitingReadLockSemaphore = null;

					return true;
				}

				if (_waitingReadLockSemaphore == null)
				{
					_waitingReadLockSemaphore = new SemaphoreSlim(0);
				}

				_readersWaiting++;
				waitingReadLockSemaphore = _waitingReadLockSemaphore;

				return false;
			}
		}

		private void ExitReadLock()
		{
			lock (_writeLockSemaphore)
			{
				AssertNotDisposed();
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

		private void DisposeWaitingSemaphore()
		{
			_waitingDisposalSemaphore?.Dispose();
			_waitingDisposalSemaphore = null;
		}

		private void AssertNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(nameof(AsyncReaderWriterLock));
			}
		}

		public struct Releaser : IDisposable
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
