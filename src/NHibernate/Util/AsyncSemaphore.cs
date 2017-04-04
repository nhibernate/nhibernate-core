using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHibernate.Util
{
	public class AsyncSemaphore
	{
		private static readonly Task completed = Task.FromResult(true);
		private readonly Queue<TaskCompletionSource<bool>> waiters = new Queue<TaskCompletionSource<bool>>();
		private int currentCount;

		public AsyncSemaphore(int initialCount)
		{
			if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
			currentCount = initialCount;
		}
		public Task WaitAsync()
		{
			lock (waiters)
			{
				if (currentCount > 0)
				{
					--currentCount;
					return completed;
				}
				var waiter = new TaskCompletionSource<bool>();
				waiters.Enqueue(waiter);
				return waiter.Task;
			}
		}

		public void Release()
		{
			TaskCompletionSource<bool> toRelease = null;
			lock (waiters)
			{
				if (waiters.Count > 0)
					toRelease = waiters.Dequeue();
				else
					++currentCount;
			}
			if (toRelease != null)
				toRelease.SetResult(true);
		}
	}
}