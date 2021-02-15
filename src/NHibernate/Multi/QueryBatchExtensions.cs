using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Criterion;
using NHibernate.Engine;

namespace NHibernate.Multi
{
	public static partial class QueryBatchExtensions
	{
		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="afterLoad">Callback to execute when query is loaded. Loaded results are provided as action parameter.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, IQueryOver query, Action<IList<TResult>> afterLoad = null)
		{
			return batch.Add(For<TResult>(query), afterLoad);
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, string key, IQueryOver query)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Add(key, For<TResult>(query));
			return batch;
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="afterLoad">Callback to execute when query is loaded. Loaded results are provided as action parameter.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, IQueryOver<TResult> query, Action<IList<TResult>> afterLoad = null)
		{
			return batch.Add(For<TResult>(query), afterLoad);
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, string key, IQueryOver<TResult> query)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Add(key, For<TResult>(query));
			return batch;
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="afterLoad">Callback to execute when query is loaded. Loaded results are provided as action parameter.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, ICriteria query, Action<IList<TResult>> afterLoad = null)
		{
			return batch.Add(For<TResult>(query), afterLoad);
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, string key, ICriteria query)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Add(key, For<TResult>(query));
			return batch;
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="afterLoad">Callback to execute when query is loaded. Loaded results are provided as action parameter.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, DetachedCriteria query, Action<IList<TResult>> afterLoad = null)
		{
			return batch.Add(For<TResult>(query), afterLoad);
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, string key, DetachedCriteria query)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Add(key, For<TResult>(query));
			return batch;
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="afterLoad">Callback to execute when query is loaded. Loaded results are provided as action parameter.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, IQuery query, Action<IList<TResult>> afterLoad = null)
		{
			return batch.Add(For<TResult>(query), afterLoad);
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, string key, IQuery query)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Add(key, For<TResult>(query));
			return batch;
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="afterLoad">Callback to execute when query is loaded. Loaded results are provided as action parameter.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, IQueryable<TResult> query, Action<IList<TResult>> afterLoad = null)
		{
			return batch.Add(For(query), afterLoad);
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TResult>(this IQueryBatch batch, string key, IQueryable<TResult> query)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Add(key, For(query));
			return batch;
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="selector">An aggregation function to apply to <paramref name="query"/>.</param>
		/// <param name="afterLoad">Callback to execute when query is loaded. Loaded results are provided as action parameter.</param>
		/// <typeparam name="TSource">The type of the query elements before aggregation.</typeparam>
		/// <typeparam name="TResult">The type resulting of the query result aggregation.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TSource, TResult>(this IQueryBatch batch, IQueryable<TSource> query, Expression<Func<IQueryable<TSource>, TResult>> selector, Action<TResult> afterLoad = null)
		{
			return batch.Add(For(query, selector), afterLoad == null ? (Action<IList<TResult>>) null : list => afterLoad(list.FirstOrDefault()));
		}

		/// <summary>
		/// Adds a query to the batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="key">A key for retrieval of the query result.</param>
		/// <param name="query">The query.</param>
		/// <param name="selector">An aggregation function to apply to <paramref name="query"/>.</param>
		/// <typeparam name="TSource">The type of the query elements before aggregation.</typeparam>
		/// <typeparam name="TResult">The type resulting of the query result aggregation.</typeparam>
		/// <exception cref="InvalidOperationException">Thrown if the batch has already been executed.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch Add<TSource, TResult>(this IQueryBatch batch, string key, IQueryable<TSource> query, Expression<Func<IQueryable<TSource>, TResult>> selector)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Add(key, For(query, selector));
			return batch;
		}

		/// <summary>
		/// Sets the timeout in seconds for the underlying ADO.NET query.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="timeout">The timeout for the batch.</param>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch SetTimeout(this IQueryBatch batch, int? timeout)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.Timeout = timeout == RowSelection.NoValue ? null : timeout;
			return batch;
		}

		/// <summary>
		/// Overrides the current session flush mode, just for this query batch.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="mode">The flush mode for the batch.</param>
		/// <returns>The batch instance for method chain.</returns>
		public static IQueryBatch SetFlushMode(this IQueryBatch batch, FlushMode mode)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			batch.FlushMode = mode;
			return batch;
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureEnumerable{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureEnumerable<TResult> AddAsFuture<TResult>(this IQueryBatch batch, IQueryOver query)
		{
			return AddAsFuture(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureEnumerable{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureEnumerable<TResult> AddAsFuture<TResult>(this IQueryBatch batch, IQueryOver<TResult> query)
		{
			return AddAsFuture(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureEnumerable{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureEnumerable<TResult> AddAsFuture<TResult>(this IQueryBatch batch, ICriteria query)
		{
			return AddAsFuture(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureEnumerable{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureEnumerable<TResult> AddAsFuture<TResult>(this IQueryBatch batch, DetachedCriteria query)
		{
			return AddAsFuture(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureEnumerable{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureEnumerable<TResult> AddAsFuture<TResult>(this IQueryBatch batch, IQuery query)
		{
			return AddAsFuture(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureEnumerable{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureEnumerable<TResult> AddAsFuture<TResult>(this IQueryBatch batch, IQueryable<TResult> query)
		{
			return AddAsFuture(batch, For(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureEnumerable{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureEnumerable<TResult> AddAsFuture<TResult>(this IQueryBatch batch, IQueryBatchItem<TResult> query)
		{
			batch.Add(query);
			return new FutureEnumerable<TResult>(batch, query);
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <param name="selector">An aggregation function to apply to <paramref name="query"/>.</param>
		/// <typeparam name="TSource">The type of the query elements before aggregation.</typeparam>
		/// <typeparam name="TResult">The type resulting of the query result aggregation.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TSource, TResult>(this IQueryBatch batch, IQueryable<TSource> query, Expression<Func<IQueryable<TSource>, TResult>> selector)
		{
			return AddAsFutureValue(batch, For(query, selector));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TResult>(this IQueryBatch batch, IQueryable<TResult> query)
		{
			return AddAsFutureValue(batch, For(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TResult>(this IQueryBatch batch, ICriteria query)
		{
			return AddAsFutureValue(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TResult>(this IQueryBatch batch, DetachedCriteria query)
		{
			return AddAsFutureValue(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TResult>(this IQueryBatch batch, IQueryOver query)
		{
			return AddAsFutureValue(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TResult>(this IQueryBatch batch, IQueryOver<TResult> query)
		{
			return AddAsFutureValue(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TResult>(this IQueryBatch batch, IQuery query)
		{
			return AddAsFutureValue(batch, For<TResult>(query));
		}

		/// <summary>
		/// Adds a query to the batch, returning it as an <see cref="IFutureValue{T}"/>.
		/// </summary>
		/// <param name="batch">The batch.</param>
		/// <param name="query">The query.</param>
		/// <typeparam name="TResult">The type of the query result elements.</typeparam>
		/// <returns>A future query which execution will be handled by the batch.</returns>
		public static IFutureValue<TResult> AddAsFutureValue<TResult>(this IQueryBatch batch, IQueryBatchItem<TResult> query)
		{
			batch.Add(query);
			return new FutureValue<TResult>(batch, query);
		}

		private static LinqBatchItem<TResult> For<TResult>(IQueryable<TResult> query)
		{
			return LinqBatchItem.Create(query);
		}

		private static LinqBatchItem<TResult> For<TSource, TResult>(IQueryable<TSource> query, Expression<Func<IQueryable<TSource>, TResult>> selector)
		{
			return LinqBatchItem.Create(query, selector);
		}

		private static QueryBatchItem<TResult> For<TResult>(IQuery query)
		{
			return new QueryBatchItem<TResult>(query);
		}

		private static CriteriaBatchItem<TResult> For<TResult>(ICriteria query)
		{
			return new CriteriaBatchItem<TResult>(query);
		}

		private static CriteriaBatchItem<TResult> For<TResult>(DetachedCriteria query)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			return new CriteriaBatchItem<TResult>(query.GetCriteriaImpl());
		}

		private static CriteriaBatchItem<TResult> For<TResult>(IQueryOver query)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			return For<TResult>(query.RootCriteria);
		}

		private static IQueryBatch Add<TResult>(this IQueryBatch batch, IQueryBatchItem<TResult> query, Action<IList<TResult>> afterLoad)
		{
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			if (afterLoad != null)
			{
				query.AfterLoadCallback += afterLoad;
			}
			batch.Add(query);
			return batch;
		}

		#region Helper classes

		partial class FutureValue<TResult> : IFutureValue<TResult>
		{
			private FutureList<TResult> _futureList;

			private TResult _result;

			public FutureValue(IQueryBatch batch, IQueryBatchItem<TResult> query)
			{
				_futureList = new FutureList<TResult>(batch, query);
			}

			public TResult Value
			{
				get
				{
					if (_futureList == null)
						return _result;

					_result = _futureList.Value.FirstOrDefault();
					_futureList = null;

					return _result;
				}
			}
		}

		partial class FutureList<TResult> : IFutureList<TResult>
		{
			private IQueryBatch _batch;
			private IQueryBatchItem<TResult> _query;

			private IList<TResult> _list;

			public FutureList(IQueryBatch batch, IQueryBatchItem<TResult> query)
			{
				_batch = batch;
				_query = query;
			}

			public IList<TResult> Value
			{
				get
				{
					if (_batch == null)
						return _list;

					if (!_batch.IsExecutedOrEmpty)
						_batch.Execute();
					_list = _query.GetResults();

					_batch = null;
					_query = null;

					return _list;
				}
			}
		}

		class FutureEnumerable<TResult> : IFutureEnumerable<TResult>
		{
			private readonly IFutureList<TResult> _result;

			public FutureEnumerable(IQueryBatch batch, IQueryBatchItem<TResult> query)
			{
				_result = new FutureList<TResult>(batch, query);
			}

			public async Task<IEnumerable<TResult>> GetEnumerableAsync(CancellationToken cancellationToken = default(CancellationToken))
			{
				return await _result.GetValueAsync(cancellationToken).ConfigureAwait(false);
			}

			public IEnumerable<TResult> GetEnumerable()
			{
				return _result.Value;
			}

			IEnumerator<TResult> IFutureEnumerable<TResult>.GetEnumerator()
			{
				return GetEnumerable().GetEnumerator();
			}

			IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
			{
				return GetEnumerable().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerable().GetEnumerator();
			}
		}

		#endregion Helper classes
	}
}
