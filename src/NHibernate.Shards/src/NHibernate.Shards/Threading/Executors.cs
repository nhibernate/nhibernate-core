using System;

namespace NHibernate.Shards.Threading
{
	public class Executors
	{
		public static ICallable<T> Callable<T>(IRunnable task, T result)
		{
			if (task == null)
				throw new ArgumentNullException();
			return new RunnableAdapter<T>(task, result);
		}

		#region Nested type: RunnableAdapter

		/// <summary>
		/// A callable that runs given task and returns given result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public class RunnableAdapter<T> : ICallable<T>
		{
			private readonly T result;
			private readonly IRunnable task;


			public RunnableAdapter(IRunnable task, T result)
			{
				this.result = result;
				this.task = task;
			}

			#region ICallable<T> Members

			/// <summary>
			/// Computes a result, or throws an exception if unable to do so.
			/// </summary>
			/// <returns></returns>
			public T Call()
			{
				task.Run();
				return result;
			}

			#endregion
		}

		#endregion
	}
}