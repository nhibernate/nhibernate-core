using System;

namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// <c>From Javadocs</c>: A Future represents the result of an asynchronous computation. 
	/// Methods are provided to check if the computation is complete, 
	/// to wait for its completion, and to retrieve the result of the computation. 
	/// The result can only be retrieved using method get when the computation 
	/// has completed, blocking if necessary until it is ready. 
	/// Cancellation is performed by the cancel method. Additional methods are 
	/// provided to determine if the task completed normally or was cancelled. 
	/// Once a computation has completed, the computation cannot be cancelled. 
	/// If you would like to use a Future for the sake of cancellability but not 
	/// provide a usable result, you can declare types of the form Future<?> and 
	/// return null as a result of the underlying task.
	/// </summary>
	/// <typeparam name="T">The result type returned by this IFuture's get method</typeparam>
	public interface IFuture<T>
	{
		/// <summary>
		/// Returns true if this task was cancelled before it completed normally.
		/// </summary>
		bool IsCancelled { get; }

		/// <summary>
		/// Returns true if this task completed.
		/// </summary>
		bool IsDone { get; }

		/// <summary>
		/// Attempts to cancel execution of this task.
		/// </summary>
		/// <param name="mayInterruptIfRunning"></param>
		/// <returns></returns>
		bool Cancel(bool mayInterruptIfRunning);


		/// <summary>
		/// Waits if necessary for the computation to complete, and then retrieves its result.
		/// </summary>
		/// <returns></returns>
		T Get();

		/// <summary>
		/// Waits if necessary for at most the given time for the computation to complete, 
		/// and then retrieves its result, if available.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		T Get(TimeSpan timeout);
	}
}