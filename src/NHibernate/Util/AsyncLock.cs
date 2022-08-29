using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	// Source from https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx
	public sealed class AsyncLock
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly IDisposable _releaser;
		private readonly Task<IDisposable> _releaserTask;

		public AsyncLock()
		{
			_releaser = new Releaser(this);
			_releaserTask = Task.FromResult(_releaser);
		}

		public Task<IDisposable> LockAsync()
		{
			var wait = _semaphore.WaitAsync();
			return wait.IsCompleted ?
				_releaserTask :
				wait.ContinueWith(
					(_, state) => (IDisposable) state,
					_releaser, CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}

		public IDisposable Lock()
		{
			_semaphore.Wait();
			return _releaser;
		}

		private sealed class Releaser : IDisposable
		{
			private readonly AsyncLock _toRelease;
			internal Releaser(AsyncLock toRelease) { _toRelease = toRelease; }
			public void Dispose() { _toRelease._semaphore.Release(); }
		}
	}
}
