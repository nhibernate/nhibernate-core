using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;

namespace NHibernate.Impl
{
	public abstract partial class FutureBatch<TQueryApproach, TMultiApproach>
	{
		private class BatchedQuery
		{
			public TQueryApproach Query { get; set; }
			public System.Type ResultType { get; set; }
			public IDelayedValue Future { get; set; }
		}

		private readonly List<BatchedQuery> queries = new List<BatchedQuery>();
		private int index;
		private IList results;
		private bool isCacheable = true;
		private string cacheRegion;

		protected readonly SessionImpl session;

		protected FutureBatch(SessionImpl session)
		{
			this.session = session;
		}

		public void Add<TResult>(TQueryApproach query)
		{
			if (queries.Count == 0)
			{
				cacheRegion = CacheRegion(query);
			}

			queries.Add(new BatchedQuery { Query = query, ResultType = typeof(TResult) });
			index = queries.Count - 1;
			isCacheable = isCacheable && IsQueryCacheable(query);
			isCacheable = isCacheable && (cacheRegion == CacheRegion(query));
		}

		public void Add(TQueryApproach query)
		{
			Add<object>(query);
		}

		public IFutureValue<TResult> GetFutureValue<TResult>()
		{
			int currentIndex = index;
			var future = new FutureValue<TResult>(
				() => GetCurrentResult<TResult>(currentIndex),
				cancellationToken => GetCurrentResultAsync<TResult>(currentIndex, cancellationToken));
			var query = queries[currentIndex];
			query.Future = future;
			return future;
		}

		public IFutureEnumerable<TResult> GetEnumerator<TResult>()
		{
			var currentIndex = index;
			var future = new DelayedEnumerator<TResult>(
				() => GetCurrentResult<TResult>(currentIndex),
				cancellationToken => GetCurrentResultAsync<TResult>(currentIndex, cancellationToken));
			var query = queries[currentIndex];
			query.Future = future;
			return future;
		}

		private IList GetResults()
		{
			if (results != null)
				return results;

			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				results = new List<object>();
				foreach (var query in queries)
				{
					var result = List(query.Query);
					if (query.Future != null)
						result = query.Future.TransformList(result);
					results.Add(result);
				}
			}
			else
			{
				var multiApproach = CreateMultiApproach(isCacheable, cacheRegion);
				var needTransformer = false;
				foreach (var query in queries)
				{
					AddTo(multiApproach, query.Query, query.ResultType);
					if (query.Future?.ExecuteOnEval != null)
						needTransformer = true;
				}

				if (needTransformer)
					AddResultTransformer(
						multiApproach, 
						new FutureResultsTransformer(queries));

				results = GetResultsFrom(multiApproach);
			}

			ClearCurrentFutureBatch();
			return results;
		}

		private IEnumerable<TResult> GetCurrentResult<TResult>(int currentIndex)
		{
			return ((IList) GetResults()[currentIndex]).Cast<TResult>();
		}

		// 6.0 TODO: switch to abstract
		protected virtual IList List(TQueryApproach query)
		{
			throw new NotSupportedException("This FutureBatch implementation does not support executing queries when multiple queries are not supported");
		}

		protected abstract TMultiApproach CreateMultiApproach(bool isCacheable, string cacheRegion);
		protected abstract void AddTo(TMultiApproach multiApproach, TQueryApproach query, System.Type resultType);
		protected abstract IList GetResultsFrom(TMultiApproach multiApproach);
		protected abstract void ClearCurrentFutureBatch();
		protected abstract bool IsQueryCacheable(TQueryApproach query);
		protected abstract string CacheRegion(TQueryApproach query);

		protected virtual void AddResultTransformer(
			TMultiApproach multiApproach,
			IResultTransformer futureResulsTransformer)
		{
			// Only Linq set ExecuteOnEval, so only FutureQueryBatch needs to support it, not FutureCriteriaBatch.
			throw new NotSupportedException();
		}

		// ResultTransformer are usually re-usable, this is not the case of this one, which will
		// be built for each multi-query requiring it.
		// It also usually ends in query cache, but this is not the case either for multi-query.
		[Serializable]
		private class FutureResultsTransformer : IResultTransformer
		{
			private readonly List<BatchedQuery> _batchedQueries;
			private int _currentIndex;

			public FutureResultsTransformer(List<BatchedQuery> batchedQueries)
			{
				_batchedQueries = batchedQueries;
			}

			public object TransformTuple(object[] tuple, string[] aliases)
			{
				return tuple.Length == 1 ? tuple[0] : tuple;
			}

			public IList TransformList(IList collection)
			{
				if (_currentIndex >= _batchedQueries.Count)
					throw new InvalidOperationException(
						$"Transformer have been called more times ({_currentIndex + 1}) than it has queries to transform.");

				var batchedQuery = _batchedQueries[_currentIndex];
				_currentIndex++;

				return batchedQuery.Future?.TransformList(collection) ?? collection;
			}

			// We do not really need to override them since this one does not ends in query cache, but a test forces us to.
			public override bool Equals(object obj)
			{
				return ReferenceEquals(this, obj);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}
	}
}
