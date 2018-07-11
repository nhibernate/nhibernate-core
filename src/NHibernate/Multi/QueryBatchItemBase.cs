using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.SqlCommand;
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
		private IList[] _loaderResults;

		private List<QueryLoadInfo> _queryInfos;
		private IList<TResult> _finalResults;

		protected class QueryLoadInfo
		{
			public Loader.Loader Loader;
			public QueryParameters Parameters;
			
			//Cache related properties:
			public bool IsCacheable;
			public ISet<string> QuerySpaces;
			public IQueryCache Cache;
			public QueryKey CacheKey;
			public bool IsResultFromCache;
		}

		protected abstract List<QueryLoadInfo> GetQueryLoadInfo();

		/// <inheritdoc />
		public virtual void Init(ISessionImplementor session)
		{
			Session = session;

			_queryInfos = GetQueryLoadInfo();

			var count = _queryInfos.Count;
			_subselectResultKeys = new List<EntityKey[]>[count];
			_loaderResults = new IList[count];

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
			for (var index = 0; index < _queryInfos.Count; index++)
			{
				var qi = _queryInfos[index];

				if (qi.Loader.IsCacheable(qi.Parameters))
				{
					qi.IsCacheable = true;
					// Check if the results are available in the cache
					qi.Cache = Session.Factory.GetQueryCache(qi.Parameters.CacheRegion);
					qi.CacheKey = qi.Loader.GenerateQueryKey(Session, qi.Parameters);
					var resultsFromCache = qi.Loader.GetResultFromQueryCache(Session, qi.Parameters, qi.QuerySpaces, qi.Cache, qi.CacheKey);

					if (resultsFromCache != null)
					{
						// Cached results available, skip the command for them and stores them.
						_loaderResults[index] = resultsFromCache;
						qi.IsResultFromCache = true;
						continue;
					}
				}

				yield return qi.Loader.CreateSqlCommand(qi.Parameters, Session);
			}
		}

		/// <inheritdoc />
		public int ProcessResultsSet(DbDataReader reader)
		{
			var dialect = Session.Factory.Dialect;
			var hydratedObjects = new List<object>[_queryInfos.Count];

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
							forcedResultTransformer
						);
					if (loader.IsSubselectLoadingEnabled)
					{
						_subselectResultKeys[i].Add(keys);
						keys = new EntityKey[entitySpan]; //can't reuse in this case
					}

					tmpResults.Add(o);
				}

				_loaderResults[i] = tmpResults;

				reader.NextResult();
			}

			InitializeEntitiesAndCollections(reader, hydratedObjects);

			return rowCount;
		}

		/// <inheritdoc />
		public void ProcessResults()
		{
			for (var i = 0; i < _queryInfos.Count; i++)
			{
				var queryInfo = _queryInfos[i];
				if (_subselectResultKeys[i] != null)
				{
					queryInfo.Loader.CreateSubselects(_subselectResultKeys[i], queryInfo.Parameters, Session);
				}

				// Handle cache if cacheable.
				if (queryInfo.IsCacheable)
				{
					if (!queryInfo.IsResultFromCache)
					{
						queryInfo.Loader.PutResultInQueryCache(
							Session,
							queryInfo.Parameters,
							queryInfo.Cache,
							queryInfo.CacheKey,
							_loaderResults[i]);
					}

					_loaderResults[i] =
						queryInfo.Loader.TransformCacheableResults(
							queryInfo.Parameters, queryInfo.CacheKey.ResultTransformer, _loaderResults[i]);
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
			if (_loaderResults == null)
			{
				throw new HibernateException("Batch wasn't executed. You must call IQueryBatch.Execute() before accessing results.");
			}
			var results = new List<T>(_loaderResults.Sum(tr => tr.Count));
			for (var i = 0; i < _queryInfos.Count; i++)
			{
				var list = _queryInfos[i].Loader.GetResultList(
					_loaderResults[i],
					_queryInfos[i].Parameters.ResultTransformer);
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
					hydratedObjects[i], reader, Session, Session.PersistenceContext.DefaultReadOnly);
			}
		}
	}
}
