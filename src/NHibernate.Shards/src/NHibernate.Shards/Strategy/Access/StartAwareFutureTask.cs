using NHibernate.Shards.Threading;

namespace NHibernate.Shards.Strategy.Access
{
	/// <summary>
	/// /// Extension of FutureTask that provides slightly different cancel()
	/// behavior.  We want cancel() to only return true if the task has not yet run.
	///
	/// Multi-threaded scenario 1:
	/// run() invoked in T1
	/// The task hasn't been cancelled, so runCalled is set
	/// to true.  This happens in a synchronized block so cancel() cannot
	/// execute while the flag is being set.  Once we enter the synchronized
	/// block and get past the cancelled check we are guaranteed to run, and
	/// if cancel() is invoked at any point afterwards runCalled will be true, so
	/// cancel() will be unable to return anything other than false, which is what
	/// we want.
	///
	/// Multi-threaded scenario 2:
	/// cancel() invoked in T1
	/// The method is synchronized, so even if T2 invokes run() it won't be able to
	/// enter the synchronized block until cancel() finishes executing.
	/// The cancelled flag is set to true, so we return right away.
	/// cancel() returns the result of super.cancel() because runCalled is guaranteed to be false.
	/// </summary>
	public class StartAwareFutureTask : FutureTask<object>
	{
		private readonly int id;

		public StartAwareFutureTask(ICallable<object> callable, int id)
			: base(callable)
		{
			this.id = id;
		}
	}
}