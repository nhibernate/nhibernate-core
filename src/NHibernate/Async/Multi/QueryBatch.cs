﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Type;

namespace NHibernate.Multi
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class QueryBatch : IQueryBatch
	{

		/// <inheritdoc />
		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (_queries.Count == 0)
				return;
			using (Session.BeginProcess())
			{
				var sessionFlushMode = Session.FlushMode;
				if (FlushMode.HasValue)
					Session.FlushMode = FlushMode.Value;
				try
				{
					Init();

					if (!Session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
					{
						foreach (var query in _queries)
						{
							await (query.ExecuteNonBatchedAsync(cancellationToken)).ConfigureAwait(false);
						}

						return;
					}

					await (ExecuteBatchedAsync(cancellationToken)).ConfigureAwait(false);
				}
				finally
				{
					if (_autoReset)
					{
						_queries.Clear();
						_queriesByKey.Clear();
					}
					else
						_executed = true;

					if (FlushMode.HasValue)
						Session.FlushMode = sessionFlushMode;
				}
			}
		}

		/// <inheritdoc />
		public Task<IList<TResult>> GetResultAsync<TResult>(int queryIndex, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<IList<TResult>>(cancellationToken);
			}
			return GetResultsAsync<TResult>(_queries[queryIndex], cancellationToken);
		}

		/// <inheritdoc />
		public Task<IList<TResult>> GetResultAsync<TResult>(string querykey, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<IList<TResult>>(cancellationToken);
			}
			return GetResultsAsync<TResult>(_queriesByKey[querykey], cancellationToken);
		}

		private async Task<IList<TResult>> GetResultsAsync<TResult>(IQueryBatchItem query, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!_executed)
				await (ExecuteAsync(cancellationToken)).ConfigureAwait(false);
			return ((IQueryBatchItem<TResult>) query).GetResults();
		}

		protected async Task ExecuteBatchedAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var querySpaces = new HashSet<string>(_queries.SelectMany(t => t.GetQuerySpaces()));
			if (querySpaces.Count > 0)
			{
				// The auto-flush must be handled before querying the cache, because an auto-flush may
				// have to invalidate cached data, data which otherwise would cause a command to be skipped.
				await (Session.AutoFlushIfRequiredAsync(querySpaces, cancellationToken)).ConfigureAwait(false);
			}

			await (GetCachedResultsAsync(cancellationToken)).ConfigureAwait(false);

			var resultSetsCommand = Session.Factory.ConnectionProvider.Driver.GetResultSetsCommand(Session);
			CombineQueries(resultSetsCommand);

			var statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopWatch = null;
			if (statsEnabled)
			{
				stopWatch = new Stopwatch();
				stopWatch.Start();
			}

			if (Log.IsDebugEnabled())
			{
				Log.Debug("Multi query with {0} queries: {1}", _queries.Count, resultSetsCommand.Sql);
			}

			var rowCount = 0;
			try
			{
				if (resultSetsCommand.HasQueries)
				{
					using (var reader = await (resultSetsCommand.GetReaderAsync(Timeout, cancellationToken)).ConfigureAwait(false))
					{
						var cacheBatcher = new CacheBatcher(Session);
						foreach (var query in _queries)
						{
							if (query.CachingInformation != null)
							{
								foreach (var cachingInfo in query.CachingInformation.Where(ci => ci.IsCacheable))
								{
									cachingInfo.SetCacheBatcher(cacheBatcher);
								}
							}

							rowCount += await (query.ProcessResultsSetAsync(reader, cancellationToken)).ConfigureAwait(false);
						}
						await (cacheBatcher.ExecuteBatchAsync(cancellationToken)).ConfigureAwait(false);
					}
				}

				// Query cacheable results must be cached untransformed: the put does not need to wait for
				// the ProcessResults.
				await (PutCacheableResultsAsync(cancellationToken)).ConfigureAwait(false);

				foreach (var query in _queries)
				{
					query.ProcessResults();
				}
			}
			catch (OperationCanceledException) { throw; }
			catch (Exception sqle)
			{
				Log.Error(sqle, "Failed to execute query batch: [{0}]", resultSetsCommand.Sql);
				throw ADOExceptionHelper.Convert(
					Session.Factory.SQLExceptionConverter,
					sqle,
					"Failed to execute query batch",
					resultSetsCommand.Sql);
			}

			if (statsEnabled)
			{
				stopWatch.Stop();
				Session.Factory.StatisticsImplementor.QueryExecuted(
					resultSetsCommand.Sql.ToString(),
					rowCount,
					stopWatch.Elapsed);
			}
		}

		private async Task GetCachedResultsAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var statisticsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			var queriesByCaches = GetQueriesByCaches(ci => ci.CanGetFromCache);
			foreach (var queriesByCache in queriesByCaches)
			{
				var queryInfos = queriesByCache.ToArray();
				var cache = queriesByCache.Key;
				var keys = new QueryKey[queryInfos.Length];
				var parameters = new QueryParameters[queryInfos.Length];
				var returnTypes = new ICacheAssembler[queryInfos.Length][];
				var spaces = new ISet<string>[queryInfos.Length];
				for (var i = 0; i < queryInfos.Length; i++)
				{
					var queryInfo = queryInfos[i];
					keys[i] = queryInfo.CacheKey;
					parameters[i] = queryInfo.Parameters;
					returnTypes[i] = queryInfo.Parameters.HasAutoDiscoverScalarTypes
						? null
						: queryInfo.CacheKey.ResultTransformer.GetCachedResultTypes(queryInfo.ResultTypes);
					spaces[i] = queryInfo.QuerySpaces;
				}

				var results = await (cache.GetManyAsync(keys, parameters, returnTypes, spaces, Session, cancellationToken)).ConfigureAwait(false);

				for (var i = 0; i < queryInfos.Length; i++)
				{
					queryInfos[i].SetCachedResult(results[i]);

					if (statisticsEnabled)
					{
						var queryIdentifier = queryInfos[i].QueryIdentifier;
						if (results[i] == null)
						{
							Session.Factory.StatisticsImplementor.QueryCacheMiss(queryIdentifier, cache.RegionName);
						}
						else
						{
							Session.Factory.StatisticsImplementor.QueryCacheHit(queryIdentifier, cache.RegionName);
						}
					}
				}
			}
		}

		private async Task PutCacheableResultsAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var statisticsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			var queriesByCaches = GetQueriesByCaches(ci => ci.ResultToCache != null);
			foreach (var queriesByCache in queriesByCaches)
			{
				var queryInfos = queriesByCache.ToArray();
				var cache = queriesByCache.Key;
				var keys = new QueryKey[queryInfos.Length];
				var parameters = new QueryParameters[queryInfos.Length];
				var returnTypes = new ICacheAssembler[queryInfos.Length][];
				var results = new IList[queryInfos.Length];
				for (var i = 0; i < queryInfos.Length; i++)
				{
					var queryInfo = queryInfos[i];
					keys[i] = queryInfo.CacheKey;
					parameters[i] = queryInfo.Parameters;
					returnTypes[i] = queryInfo.CacheKey.ResultTransformer.GetCachedResultTypes(queryInfo.ResultTypes);
					results[i] = queryInfo.ResultToCache;
				}

				var putted = await (cache.PutManyAsync(keys, parameters, returnTypes, results, Session, cancellationToken)).ConfigureAwait(false);

				if (!statisticsEnabled)
					continue;

				for (var i = 0; i < queryInfos.Length; i++)
				{
					if (putted[i])
					{
						Session.Factory.StatisticsImplementor.QueryCachePut(
							queryInfos[i].QueryIdentifier, cache.RegionName);
					}
				}
			}
		}
	}
}
