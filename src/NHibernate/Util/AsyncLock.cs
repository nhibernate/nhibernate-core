using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	public class AsyncLock
	{
		private readonly AsyncSemaphore semaphore;
		private readonly Task<Releaser> releaser;

		public AsyncLock()
		{
			semaphore = new AsyncSemaphore(1);
			releaser = Task.FromResult(new Releaser(this));
		}

		public Task<Releaser> LockAsync()
		{
			var wait = semaphore.WaitAsync();
			return wait.IsCompleted
				? releaser
				: wait.ContinueWith(
					(_, state) => new Releaser((AsyncLock)state),
					this,
					CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		public struct Releaser : IDisposable
		{
			private readonly AsyncLock toRelease;

			internal Releaser(AsyncLock toRelease)
			{
				this.toRelease = toRelease;
			}

			public void Dispose()
			{
				if (toRelease != null)
					toRelease.semaphore.Release();
			}
		}
	}
}