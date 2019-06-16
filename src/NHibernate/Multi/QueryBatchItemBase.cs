using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Multi
{
	/// <summary>
	/// Base class for both ICriteria and IQuery queries
	/// </summary>
	public abstract partial class QueryBatchItemBase<TResult> : IQueryBatchItem<TResult>
	{
		protected ISessionImplementor Session;
		private List<EntityKey[]>[] _subselectResultKeys;
		private List<QueryInfo> _queryInfos;
		private CacheMode? _cacheMode;
		private IList<TResult> _finalResults;

		protected class QueryInfo : ICachingInformation, ICachingInformationWithFetches
		{
			/// <summary>
			/// The query loader.
			/// </summary>
			public Loader.Loader Loader { get; set; }

			/// <summary>
			/// The query result.
			/// </summary>
			public IList Result { get; set; }

			/// <inheritdoc />
			public QueryParameters Parameters { get; }

			/// <inheritdoc />
			public ISet<string> QuerySpaces { get; }

			//Cache related properties:

			/// <inheritdoc />
			public bool IsCacheable { get; }

			/// <inheritdoc />
			public QueryKey CacheKey { get;}

			/// <inheritdoc />
			public bool CanGetFromCache { get; }

			// Do not store but forward instead: Loader.ResultTypes can be null initially (if AutoDiscoverTypes
			// is enabled).
			/// <inheritdoc />
			public IType[] ResultTypes => Loader.ResultTypes;

			/// <inheritdoc />
			public IType[] CacheTypes => Loader.CacheTypes;

			/// <inheritdoc />
			public string QueryIdentifier => Loader.QueryIdentifier;

			/// <inheritdoc />
			public IList ResultToCache { get; set; }

			/// <summary>
			/// Indicates if the query result was obtained from the cache.
			/// </summary>
			public bool IsResultFromCache { get; private set; }

			/// <summary>
			/// Should a result retrieved from database be cached?
			/// </summary>
			public bool CanPutToCache { get; }

			/// <summary>
			/// The cache batcher to use for entities and collections puts.
			/// </summary>
			public CacheBatcher CacheBatcher { get; private set; }

			/// <summary>
			/// Create a new <c>QueryInfo</c>.
			/// </summary>
			/// <param name="parameters">The query parameters.</param>
			/// <param name="loader">The loader.</param>
			/// <param name="querySpaces">The query spaces.</param>
			/// <param name="session">The session of the query.</param>
			public QueryInfo(
				QueryParameters parameters, Loader.Loader loader, ISet<string> querySpaces,
				ISessionImplementor session)
			{
				Parameters = parameters;
				Loader = loader;
				QuerySpaces = querySpaces;

				IsCacheable = loader.IsCacheable(parameters);
				if (!IsCacheable)
					return;

				CacheKey = Loader.GenerateQueryKey(session, Parameters);
				CanGetFromCache = Parameters.CanGetFromCache(session);
				CanPutToCache = Parameters.CanPutToCache(session);
			}

			/// <inheritdoc />
			public void SetCachedResult(IList result)
			{
				if (!IsCacheable)
					throw new InvalidOperationException("Cannot set cached result on a non cacheable query");
				if (Result != null)
					throw new InvalidOperationException("Result is already set");
				Result = result;
				IsResultFromCache = result != null;
			}

			/// <inheritdoc />
			public void SetCacheBatcher(CacheBatcher cacheBatcher)
			{
				CacheBatcher = cacheBatcher;
			}
		}

		protected abstract List<QueryInfo> GetQueryInformation(ISessionImplementor session);

		/// <inheritdoc />
		public IEnumerable<ICachingInformation> CachingInformation
		{
			get
			{
				ThrowIfNotInitialized();
				return _queryInfos;
			}
		}

		/// <inheritdoc />
		public virtual void Init(ISessionImplementor session)
		{
			Session = session;

			_queryInfos = GetQueryInformation(session);
			// Cache and readonly parameters are the same for all translators
			_cacheMode = _queryInfos.First().Parameters.CacheMode;

			var count = _queryInfos.Count;
			_subselectResultKeys = new List<EntityKey[]>[count];

			_finalResults = null;
		}

		/// <inheritdoc />
		public IEnumerable<string> GetQuerySpaces()
		{
			return _queryInfos.SelectMany(q => q.QuerySpaces);
		}

		/// <inheritdoc />
		public IEnumerable<ISqlCommand> GetCommands()
		{
			ThrowIfNotInitialized();

			foreach (var qi in _queryInfos)
			{
				if (qi.IsResultFromCache)
					continue;

				yield return qi.Loader.CreateSqlCommand(qi.Parameters, Session);
			}
		}

		/// <inheritdoc />
		public int ProcessResultsSet(DbDataReader reader)
		{
			ThrowIfNotInitialized();

			var dialect = Session.Factory.Dialect;
			var hydratedObjects = new List<object>[_queryInfos.Count];

			using (Session.SwitchCacheMode(_cacheMode))
			{
				var rowCount = 0;
				for (var i = 0; i < _queryInfos.Count; i++)
				{
					var queryInfo = _queryInfos[i];
					var loader = queryInfo.Loader;
					var queryParameters = queryInfo.Parameters;

					//Skip processing for items already loaded from cache
					if (queryInfo.IsResultFromCache)
					{
						continue;
					}

					var entitySpan = loader.EntityPersisters.Length;
					hydratedObjects[i] = entitySpan == 0 ? null : new List<object>(entitySpan);
					var keys = new EntityKey[entitySpan];

					var selection = queryParameters.RowSelection;
					var createSubselects = loader.IsSubselectLoadingEnabled;

					_subselectResultKeys[i] = createSubselects ? new List<EntityKey[]>() : null;
					var maxRows = Loader.Loader.HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;
					var advanceSelection = !dialect.SupportsLimitOffset || !loader.UseLimit(selection, dialect);

					if (advanceSelection)
					{
						Loader.Loader.Advance(reader, selection);
					}

					var forcedResultTransformer = queryInfo.CacheKey?.ResultTransformer;
					if (queryParameters.HasAutoDiscoverScalarTypes)
					{
						loader.AutoDiscoverTypes(reader, queryParameters, forcedResultTransformer);
					}

					var lockModeArray = loader.GetLockModes(queryParameters.LockModes);
					var optionalObjectKey = Loader.Loader.GetOptionalObjectKey(queryParameters, Session);
					var tmpResults = new List<object>();
					var queryCacheBuilder = new QueryCacheResultBuilder(loader);
					var cacheBatcher = queryInfo.CacheBatcher;
					var ownCacheBatcher = cacheBatcher == null;
					if (ownCacheBatcher)
						cacheBatcher = new CacheBatcher(Session);

					for (var count = 0; count < maxRows && reader.Read(); count++)
					{
						rowCount++;

						var o =
							loader.GetRowFromResultSet(
								reader,
								Session,
								queryParameters,
								lockModeArray,
								optionalObjectKey,
								hydratedObjects[i],
								keys,
								true,
								forcedResultTransformer,
								queryCacheBuilder,
								(persister, data) => cacheBatcher.AddToBatch(persister, data)
							);
						if (loader.IsSubselectLoadingEnabled)
						{
							_subselectResultKeys[i].Add(keys);
							keys = new EntityKey[entitySpan]; //can't reuse in this case
						}

						tmpResults.Add(o);
					}

					queryInfo.Result = tmpResults;
					if (queryInfo.CanPutToCache)
						queryInfo.ResultToCache = queryCacheBuilder.Result;

					if (ownCacheBatcher)
						cacheBatcher.ExecuteBatch();

					reader.NextResult();
				}

				InitializeEntitiesAndCollections(reader, hydratedObjects);

				return rowCount;
			}
		}

		/// <inheritdoc />
		public void ProcessResults()
		{
			ThrowIfNotInitialized();

			for (var i = 0; i < _queryInfos.Count; i++)
			{
				var queryInfo = _queryInfos[i];
				if (_subselectResultKeys[i] != null)
				{
					queryInfo.Loader.CreateSubselects(_subselectResultKeys[i], queryInfo.Parameters, Session);
				}

				if (queryInfo.IsCacheable)
				{
					if (queryInfo.IsResultFromCache)
					{
						var queryCacheBuilder = new QueryCacheResultBuilder(queryInfo.Loader);
						queryInfo.Result = queryCacheBuilder.GetResultList(queryInfo.Result);
					}

					// This transformation must not be applied to ResultToCache.
					queryInfo.Result =
						queryInfo.Loader.TransformCacheableResults(
							queryInfo.Parameters, queryInfo.CacheKey.ResultTransformer, queryInfo.Result);
				}
			}
			AfterLoadCallback?.Invoke(GetResults());
		}

		/// <inheritdoc />
		public void ExecuteNonBatched()
		{
			_finalResults = GetResultsNonBatched();
			AfterLoadCallback?.Invoke(_finalResults);
		}

		protected abstract IList<TResult> GetResultsNonBatched();

		protected List<T> GetTypedResults<T>()
		{
			ThrowIfNotInitialized();
			if (_queryInfos.Any(qi => qi.Result == null))
			{
				throw new InvalidOperationException("Some query results are missing, batch is likely not fully executed yet.");
			}
			var results = new List<T>(_queryInfos.Sum(qi => qi.Result.Count));
			foreach (var queryInfo in _queryInfos)
			{
				var list = queryInfo.Loader.GetResultList(
					queryInfo.Result,
					queryInfo.Parameters.ResultTransformer);
				ArrayHelper.AddAll(results, list);
			}

			return results;
		}

		/// <inheritdoc />
		public IList<TResult> GetResults()
		{
			return _finalResults ?? (_finalResults = DoGetResults());
		}

		/// <inheritdoc />
		public Action<IList<TResult>> AfterLoadCallback { get; set; }

		protected abstract List<TResult> DoGetResults();

		private void InitializeEntitiesAndCollections(DbDataReader reader, List<object>[] hydratedObjects)
		{
			for (var i = 0; i < _queryInfos.Count; i++)
			{
				var queryInfo = _queryInfos[i];
				if (queryInfo.IsResultFromCache)
					continue;
				queryInfo.Loader.InitializeEntitiesAndCollections(
					hydratedObjects[i], reader, Session, queryInfo.Parameters.IsReadOnly(Session),
					queryInfo.CacheBatcher);
			}
		}

		private void ThrowIfNotInitialized()
		{
			if (_queryInfos == null)
				throw new InvalidOperationException(
					"The query item has not been initialized. A query item must belong to a batch " +
					$"({nameof(IQueryBatch)}) and the batch must be executed ({nameof(IQueryBatch)}." +
					$"{nameof(IQueryBatch.Execute)} or {nameof(IQueryBatch)}.{nameof(IQueryBatch.GetResult)}) " +
					"before retrieving the item result.");
		}
	}
}
