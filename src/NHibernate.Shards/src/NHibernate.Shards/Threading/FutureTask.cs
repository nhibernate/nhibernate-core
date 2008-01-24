using System;
using System.Diagnostics;
using System.Threading;
using NHibernate.Shards.Threading.Exception;

namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// A cancellable asynchronous computation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FutureTask<T> : IRunnableFuture<T>
	{
		private ICallable<T> callable;
		private System.Exception exception;
		private T result;
		private volatile Thread runner;
		private StateTask state;

		public FutureTask(ICallable<T> callable)
		{
			if (callable == null)
				throw new ArgumentNullException("callable", "The callable can't be null");
			this.callable = callable;
		}


		public FutureTask(IRunnable task, T result) :
			this(Executors.Callable(task, result))
		{
		}

		#region IRunnableFuture<T> Members

		/// <summary>
		/// Returns true if this task was cancelled before it completed normally.
		/// </summary>
		public virtual bool IsCancelled
		{
			get { return state == StateTask.Canceled ? true : false; }
		}

		/// <summary>
		/// Returns true if this task completed.
		/// </summary>
		public virtual bool IsDone
		{
			get
			{
				lock(this)
				{
					return RanOrCancelled(state) && (runner == null) ? true : false;
				}
			}
		}

		/// <summary>
		///  Attempts to cancel execution of this task.
		/// </summary>
		/// <param name="mayInterruptIfRunning"></param>
		/// <returns>false if the task was canceled or 
		/// ran before the call on this method. true in otherwise.</returns>
		public virtual bool Cancel(bool mayInterruptIfRunning)
		{
			lock(this)
			{
				if (RanOrCancelled(state))
					return false;

				state = StateTask.Canceled;

				if (mayInterruptIfRunning)
				{
					try
					{
						if (runner != null)
							runner.Interrupt();
					}
					catch(ThreadInterruptedException)
					{
						//Do nothing
					}
					releaseShared();
				}
			}

			Done();
			return true;
		}

		/// <summary>
		/// Waits if necessary for the computation to complete, and then retrieves its result.
		/// </summary>
		/// <returns></returns>
		public virtual T Get()
		{
			lock(this)
			{
				while(!IsDone)
				{
					Monitor.Wait(this);
				}

				if (state == StateTask.Canceled)
					throw new CancellationException();

				if (exception != null)
					throw new ExecutionException(exception);

				return result;
			}
		}

		/// <summary>
		/// Waits if necessary for at most the given time for the computation to complete, 
		/// and then retrieves its result, if available.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public virtual T Get(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// When an object implementing interface Runnable is used to create a thread, 
		/// starting the thread causes the object's run method to be called in that 
		/// separately executing thread.
		/// </summary>
		public virtual void Run()
		{
			lock(this)
			{
				if (state != 0) return;

				state = StateTask.Running;
				runner = Thread.CurrentThread;
			}
			try
			{
				Set(callable.Call());
			}
			catch(System.Exception ex)
			{
				SetException(ex);
			}
		}

		#endregion

		protected virtual void releaseShared()
		{
			this.runner = null;
			Monitor.PulseAll(this);
		}

		/// <summary>
		///  Sets the result of this Future to the given value unless this future 
		/// has already been set or has been cancelled.
		/// </summary>
		/// <param name="obj"></param>
		protected virtual void Set(T obj)
		{
			lock(this)
			{
				if (state == StateTask.Ran)
					return;

				if (state == StateTask.Canceled)
				{
					releaseShared();
					return;
				}

				state = StateTask.Ran;
				result = obj;
				releaseShared();
			}
			Done();
		}

		protected virtual void SetException(System.Exception ex)
		{
			lock(this)
			{
				if (state == StateTask.Ran)
					return;

				if (state == StateTask.Canceled)
				{
					releaseShared();
					return;
				}

				state = StateTask.Ran; // the task it's over
				exception = ex;
				result = default(T);
				releaseShared();
			}

			Done(); //subclasses must override this member in order to use it.
		}

		/// <summary>
		/// For subclasses.
		/// </summary>
		protected virtual void Done()
		{
		}

		/// <summary>
		/// Was ran or were Canceled ?
		/// </summary>
		/// <param name="aState">State to evaluate</param>
		/// <returns></returns>
		public static bool RanOrCancelled(StateTask aState)
		{
			return aState == StateTask.Ran || aState == StateTask.Canceled;
		}
	}
}