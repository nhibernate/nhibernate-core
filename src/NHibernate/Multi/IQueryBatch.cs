using System;
using System.Collections.Generic;

namespace NHibernate.Multi
{
	/// <summary>
	/// Universal query batcher
	/// </summary>
	public partial interface IQueryBatch
	{
		/// <summary>
		/// Executes the batch.
		/// </summary>
		void Execute();

		/// <summary>
		/// Returns true if batch is already executed or empty
		/// </summary>
		bool IsExecutedOrEmpty { get; }

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		void Add(IQueryBatchItem query);

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		void Add(string key, IQueryBatchItem query);

		/// <summary>
		/// Gets a query result, triggering execution of the batch if it was not already executed.
		/// </summary>
		/// <param name="queryIndex">The index of the query for which results are to be obtained.</param>
		/// <typeparam name="TResult">The type of the result elements of the query.</typeparam>
		/// <returns>A query result.</returns>
		/// <remarks><paramref name="queryIndex"/> is <c>0</c> based and matches the order in which queries have been
		/// added into the batch.</remarks>
		IList<TResult> GetResult<TResult>(int queryIndex);

		/// <summary>
		/// Gets a query result, triggering execution of the batch if it was not already executed.
		/// </summary>
		/// <param name="querykey">The key of the query for which results are to be obtained.</param>
		/// <typeparam name="TResult">The type of the result elements of the query.</typeparam>
		/// <returns>A query result.</returns>
		IList<TResult> GetResult<TResult>(string querykey);

		/// <summary>
		/// The timeout in seconds for the underlying ADO.NET query.
		/// </summary>
		int? Timeout { get; set; }

		/// <summary>
		/// The session flush mode to use during the batch execution.
		/// </summary>
		FlushMode? FlushMode { get; set; }
	}
}
