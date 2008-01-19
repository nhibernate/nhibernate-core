using System;
using System.Threading;

namespace NHibernate.Shards.Threading
{
	public class FutureTask<T> : IFuture<T>, IRunnable
	{
		private volatile Thread thread;

		/// <summary>
		/// Returns true if this task was cancelled before it completed normally.
		/// </summary>
		public bool IsCancelled
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Returns true if this task completed.
		/// </summary>
		public bool IsDone
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		///  Attempts to cancel execution of this task.
		/// </summary>
		/// <param name="mayInterruptIfRunning"></param>
		public void Cancel(bool mayInterruptIfRunning)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Waits if necessary for the computation to complete, and then retrieves its result.
		/// </summary>
		/// <returns></returns>
		public T Get()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Waits if necessary for at most the given time for the computation to complete, 
		/// and then retrieves its result, if available.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public T Get(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// When an object implementing interface Runnable is used to create a thread, 
		/// starting the thread causes the object's run method to be called in that 
		/// separately executing thread.
		/// </summary>
		public void Run()
		{
			throw new NotImplementedException();
		}
	}
}