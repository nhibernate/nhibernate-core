using System.Runtime.CompilerServices;
using log4net;
using NHibernate.Shards.Threading;

namespace NHibernate.Shards.Strategy.Access
{
	/// <summary>
	/// Extension of FutureTask that provides slightly different Cancel()
	/// behavior.  We want Cancel() to only return true if the task has not yet run.
	///
	/// Multi-threaded scenario 1:
	/// Run() invoked in T1
	/// The task hasn't been cancelled, so runCalled is set
	/// to true.  This happens in a synchronized block so Cancel() cannot
	/// Execute while the flag is being set.  Once we enter the synchronized
	/// block and get past the cancelled check we are guaranteed to run, and
	/// if Cancel() is invoked at any point afterwards runCalled will be true, so
	/// Cancel() will be unable to return anything other than false, which is what
	/// we want.
	///
	/// Multi-threaded scenario 2:
	/// Cancel() invoked in T1
	/// The method is synchronized, so even if T2 invokes Run() it won't be able to
	/// enter the synchronized block until Cancel() finishes executing.
	/// The cancelled flag is set to true, so we return right away.
	/// Cancel() returns the result of base.Cancel() because runCalled is guaranteed to be false.
	/// </summary>
	public class StartAwareFutureTask<T> : FutureTask<T>
	{
		private readonly int id;

		private readonly ILog log = LogManager.GetLogger(typeof(StartAwareFutureTask<T>));

		private bool cancelled;

		private bool runCalled;

		public StartAwareFutureTask(ICallable<T> callable, int id)
			: base(callable)
		{
			this.id = id;
		}

		public int Id
		{
			get { return id; }
		}

		public override void Run()
		{
			log.DebugFormat("Task {0}: Run invoked", Id);
			lock(this)
			{
				if (cancelled)
				{
					log.DebugFormat("Task {0}: Task will not run.", Id);
					return;
				}
				runCalled = true;
			}
			log.DebugFormat("Task {0}: Task will run.", Id);
			base.Run();
		}

		[MethodImpl(MethodImplOptions.Synchronized)] //Equivalent to syncronized method in Java
		public override bool Cancel(bool mayInterruptIfRunning)
		{
			if (runCalled)
			{
				/* If run has already been called we can't call base. That's because
				base.Cancel() might be called in between the time we leave the
				synchronization block in Run() and the time we call base.Run().
				base.Run() checks the state of the FutureTask before actuall invoking
				the inner task, and if that check sees that this task is cancelled it
				won't run.  That leaves us in a position where a task actually has
				been cancelled but cancel returns true, so we're left with a counter
				that never gets decremented and everything hangs. */
				return false;
			}
			bool result = superCancel(mayInterruptIfRunning);
			cancelled = true;
			log.DebugFormat("Task {0}: Task cancelled.", Id);
			return result;
		}

		protected virtual bool superCancel(bool mayInterruptIfRunning)
		{
			return base.Cancel(mayInterruptIfRunning);
		}
	}
}