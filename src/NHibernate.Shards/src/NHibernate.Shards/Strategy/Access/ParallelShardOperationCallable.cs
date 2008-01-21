using System;
using NHibernate.Shards.Threading;

namespace NHibernate.Shards.Strategy.Access
{
	/// <summary>
	/// Runs a single operation on a single shard, collecting the result of the
	/// operation with an ExitStrategy.  The interesting bit here is that
	/// if the ExitStrategy indicates that there is no more work to be performed,
	/// this object has the ability to cancel the work being performed by all the
	/// other threads.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ParallelShardOperationCallable<T> : ICallable<T>
	{
		#region ICallable<T> Members

		/// <summary>
		/// Computes a result, or throws an exception if unable to do so.
		/// </summary>
		/// <returns></returns>
		public T Call()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}