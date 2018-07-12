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
	/// <inheritdoc />
	public partial class QueryBatch : IQueryBatch
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(QueryBatch));

		private readonly bool _autoReset;
		private readonly List<IQueryBatchItem> _queries = new List<IQueryBatchItem>();
		private readonly Dictionary<string, IQueryBatchItem> _queriesByKey = new Dictionary<string, IQueryBatchItem>();
		private bool _executed;

		public QueryBatch(ISessionImplementor session, bool autoReset)
		{
			Session = session;
			_autoReset = autoReset;
		}

		protected ISessionImplementor Session { get; }

		/// <inheritdoc />
		public int? Timeout { get; set; }

		/// <inheritdoc />
		public FlushMode? FlushMode { get; set; }

		/// <inheritdoc />
		public void Execute()
		{
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
							query.ExecuteNonBatched();
						}

						return;
					}

					ExecuteBatched();
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
		public bool IsExecutedOrEmpty => _executed || _queries.Count == 0;

		/// <inheritdoc />
		public void Add(IQueryBatchItem query)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			if (_executed)
				throw new InvalidOperationException("The batch has already been executed, use another batch");
			_queries.Add(query);
		}

		/// <inheritdoc />
		public void Add(string key, IQueryBatchItem query)
		{
			Add(query);
			_queriesByKey.Add(key, query);
		}

		/// <inheritdoc />
		public IList<TResult> GetResult<TResult>(int queryIndex)
		{
			return GetResults<TResult>(_queries[queryIndex]);
		}

		/// <inheritdoc />
		public IList<TResult> GetResult<TResult>(string querykey)
		{
			return GetResults<TResult>(_queriesByKey[querykey]);
		}

		private IList<TResult> GetResults<TResult>(IQueryBatchItem query)
		{
			if (!_executed)
				Execute();
			return ((IQueryBatchItem<TResult>) query).GetResults();
		}

		private void Init()
		{
			foreach (var query in _queries)
			{
				query.Init(Session);
			}
		}

		protected void ExecuteBatched()
		{
			var querySpaces = new HashSet<string>(_queries.SelectMany(t => t.GetQuerySpaces()));
			if (querySpaces.Count > 0)
			{
				// The auto-flush must be handled before querying the cache, because an auto-flush may
				// have to invalidate cached data, data which otherwise would cause a command to be skipped.
				Session.AutoFlushIfRequired(querySpaces);
			}

			GetCachedResults();

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
					using (var reader = resultSetsCommand.GetReader(Timeout))
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

							rowCount += query.ProcessResultsSet(reader);
						}
						cacheBatcher.ExecuteBatch();
					}
				}

				// Query cacheable results must be cached untransformed: the put does not need to wait for
				// the ProcessResults.
				PutCacheableResults();

				foreach (var query in _queries)
				{
					query.ProcessResults();
				}
			}
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

		private void GetCachedResults()
		{
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

				var results = cache.GetMany(keys, parameters, returnTypes, spaces, Session);

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

		private void CombineQueries(IResultSetsCommand resultSetsCommand)
		{
			foreach (var query in _queries)
			foreach (var cmd in query.GetCommands())
			{
				resultSetsCommand.Append(cmd);
			}
		}

		private void PutCacheableResults()
		{
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

				var putted = cache.PutMany(keys, parameters, returnTypes, results, Session);

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

		private IEnumerable<IGrouping<IQueryCache, ICachingInformation>> GetQueriesByCaches(Func<ICachingInformation, bool> cachingInformationFilter)
		{
			return
				_queries
					.Where(q => q.CachingInformation != null)
					.SelectMany(q => q.CachingInformation)
					.Where(ci => ci != null && cachingInformationFilter(ci))
					.GroupBy(
						ci => Session.Factory.GetQueryCache(ci.Parameters.CacheRegion));
		}
	}
}
