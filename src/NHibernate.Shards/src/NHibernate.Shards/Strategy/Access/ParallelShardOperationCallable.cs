using System;
using System.Collections.Generic;
using log4net;
using NHibernate.Shards.Strategy.Exit;
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
	public class ParallelShardOperationCallable<T> : ICallable<T>
	{
		private static readonly bool INTERRUPT_IF_RUNNING = false;

		private readonly CountDownLatch doneSignal;

		private readonly IExitStrategy<T> exitStrategy;

		private readonly IList<StartAwareFutureTask<T>> futureTasks;
		private readonly ILog log = LogManager.GetLogger(typeof(ParallelShardOperationCallable<T>));

		private readonly IShardOperation<T> operation;

		private readonly IShard shard;
		private readonly CountDownLatch startSignal;

		public ParallelShardOperationCallable(
			CountDownLatch startSignal,
			CountDownLatch doneSignal,
			IExitStrategy<T> exitStrategy,
			IShardOperation<T> operation,
			IShard shard,
			IList<StartAwareFutureTask<T>> futureTasks)
		{
			this.startSignal = startSignal;
			this.doneSignal = doneSignal;
			this.exitStrategy = exitStrategy;
			this.operation = operation;
			this.shard = shard;
			this.futureTasks = futureTasks;
		}

		#region ICallable<T> Members

		/// <summary>
		/// Computes a result, or throws an exception if unable to do so.
		/// </summary>
		/// <returns></returns>
		public T Call()
		{
			try
			{
				WaitForStartSignal();
				log.DebugFormat("Starting execution of {0} against shard {1}", operation.OperationName, shard);

				///If addResult() returns true it means there is no more work to be
				///performed. Cancel all the outstanding tasks.

				if (exitStrategy.AddResult(operation.Execute(shard), shard))
				{
					log.DebugFormat("Short-circuiting execution of {0} on other threads after execution against shard {1}",
						operation.OperationName, shard);
					
					//It's ok to cancel ourselves because StartAwareFutureTask.cancel()
					//will return false if a task has already started executing, and we're
					//already executing.

					log.DebugFormat("Checking {0} future tasks to see if they need to be cancelled.", futureTasks.Count);
					foreach(StartAwareFutureTask<T> ft in futureTasks)
					{
						log.DebugFormat("Preparing to cancel future task %d.", ft.Id);

						//If a task was successfully cancelled that means it had not yet
						//started running.  Since the task won't run, the task won't be
						// able to decrement the CountDownLatch.  We need to decrement
						//it on behalf of the cancelled task.

						if (ft.Cancel(INTERRUPT_IF_RUNNING))
						{
							log.Debug("Task cancel returned true, decrementing counter on its behalf.");
							doneSignal.CountDown();
						}
						else log.Debug("Task cancel returned false, not decrementing counter on its behalf.");
					}
				}
				else
				{
					log.DebugFormat("No need to short-cirtcuit execution of {0} on other threads after execution against shard {1}",
					                operation.OperationName, shard);
				}
			}
			finally
			{
				// counter must get decremented no matter what
				log.DebugFormat("Decrementing counter for operation {0} on shard {1}", operation.OperationName, shard);
				doneSignal.CountDown();
			}
			return default(T);
		}

		#endregion

		private void WaitForStartSignal()
		{
			try
			{
				startSignal.Await();
			}
			catch(Exception ex)
			{
				// I see no reason why this should happen
				String msg = String.Format("Received interrupt while waiting to begin execution of {0} against shard {1}",
				                           operation.OperationName, shard);
				log.ErrorFormat(msg);
				throw new HibernateException(msg, ex);
			}
		}
	}
}