using System;
using System.Collections.Generic;
using System.Threading;
using log4net;
using NHibernate.Shards.Strategy.Exit;
using NHibernate.Shards.Threading;

namespace NHibernate.Shards.Strategy.Access
{
	/// <summary>
	/// Invokes the given operation on the given shards in parallel.
	/// </summary>
	public class ParallelShardAccessStrategy : IShardAccessStrategy
	{
		private readonly ILog log = LogManager.GetLogger(typeof(ParallelShardAccessStrategy));

		#region IShardAccessStrategy Members

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="shards"></param>
		/// <param name="operation"></param>
		/// <param name="exitStrategy"></param>
		/// <param name="exitOperationsCollector"></param>
		/// <returns></returns>
		public T Apply<T>(IList<IShard> shards,
		                  IShardOperation<T> operation,
		                  IExitStrategy<T> exitStrategy,
		                  IExitOperationsCollector exitOperationsCollector)
		{
			IList<StartAwareFutureTask<T>> tasks = new List<StartAwareFutureTask<T>>(shards.Count);

			int taskId = 0;

			CountDownLatch startSignal = new CountDownLatch(1);

			CountDownLatch doneSignal = new CountDownLatch(shards.Count);

			foreach(IShard shard in shards)
			{
				ParallelShardOperationCallable<T> callable = new ParallelShardOperationCallable<T>(
					startSignal,
					doneSignal,
					exitStrategy,
					operation,
					shard,
					tasks);

				StartAwareFutureTask<T> ft = new StartAwareFutureTask<T>(callable, taskId++);

				tasks.Add(ft);

				ThreadPool.QueueUserWorkItem(delegate { ft.Run(); });
			}
			// the tasks List is populated, release the threads!
			startSignal.CountDown();

			try
			{
				log.Debug("Waiting for threads to complete processing before proceeding.");
				//TODO(maxr) let users customize timeout behavior
				/*
				if(!doneSignal.await(10, TimeUnit.SECONDS)) {
				  final String msg = "Parallel operations timed out.";
				  log.error(msg);
				  throw new HibernateException(msg);
				}
				*/
				// now we wait until all threads finish
				doneSignal.Await();
			}
			catch(Exception e)
			{
				// not sure why this would happen or what we should do if it does
				log.Error("Received unexpected exception while waiting for done signal.", e);
			}
			log.Debug("Compiling results.");
			return exitStrategy.CompileResults(exitOperationsCollector);
		}

		#endregion
	}
}