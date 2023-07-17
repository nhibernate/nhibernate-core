using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Loader.Hql;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Test.QueryTranslator
{
	/// <summary>
	/// Custom query loader to test the functionality of custom query translator factory
	/// with a custom query loader factory.
	/// </summary>
	internal sealed class CustomQueryLoader: IQueryLoader
	{
		private readonly IQueryLoader _queryLoader;

		public CustomQueryLoader(IQueryLoader queryLoader)
		{
			_queryLoader = queryLoader;
		}

		public Task<object> GetRowFromResultSetAsync(
			DbDataReader resultSet,
			ISessionImplementor session,
			QueryParameters queryParameters,
			LockMode[] lockModeArray,
			EntityKey optionalObjectKey,
			IList hydratedObjects,
			EntityKey[] keys,
			bool returnProxies,
			IResultTransformer forcedResultTransformer,
			QueryCacheResultBuilder queryCacheResultBuilder,
			Action<IEntityPersister, CachePutData> cacheBatchingHandler,
			CancellationToken cancellationToken)
		{
			return _queryLoader.GetRowFromResultSetAsync(resultSet, session, queryParameters, lockModeArray, optionalObjectKey, hydratedObjects, keys, returnProxies, forcedResultTransformer, queryCacheResultBuilder, cacheBatchingHandler, cancellationToken);
		}

		public Task InitializeEntitiesAndCollectionsAsync(
			IList hydratedObjects,
			DbDataReader reader,
			ISessionImplementor session,
			bool readOnly,
			CacheBatcher cacheBatcher,
			CancellationToken cancellationToken)
		{
			return _queryLoader.InitializeEntitiesAndCollectionsAsync(hydratedObjects, reader, session, readOnly, cacheBatcher, cancellationToken);
		}

		public Task LoadCollectionAsync(ISessionImplementor session, object id, IType type, CancellationToken cancellationToken)
		{
			return _queryLoader.LoadCollectionAsync(session, id, type, cancellationToken);
		}

		public Task LoadCollectionBatchAsync(
			ISessionImplementor session,
			object[] ids,
			IType type,
			CancellationToken cancellationToken)
		{
			return _queryLoader.LoadCollectionBatchAsync(session, ids, type, cancellationToken);
		}

		public bool IsSubselectLoadingEnabled => _queryLoader.IsSubselectLoadingEnabled;

		public IType[] ResultTypes => _queryLoader.ResultTypes;

		public IType[] CacheTypes => _queryLoader.CacheTypes;

		public Loader.Loader.QueryCacheInfo CacheInfo => _queryLoader.CacheInfo;

		public ISessionFactoryImplementor Factory => _queryLoader.Factory;

		public SqlString SqlString => _queryLoader.SqlString;

		public ILoadable[] EntityPersisters => _queryLoader.EntityPersisters;

		public string QueryIdentifier => _queryLoader.QueryIdentifier;

		public LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes)
		{
			return _queryLoader.GetLockModes(lockModes);
		}

		public object GetRowFromResultSet(
			DbDataReader resultSet,
			ISessionImplementor session,
			QueryParameters queryParameters,
			LockMode[] lockModeArray,
			EntityKey optionalObjectKey,
			IList hydratedObjects,
			EntityKey[] keys,
			bool returnProxies,
			IResultTransformer forcedResultTransformer,
			QueryCacheResultBuilder queryCacheResultBuilder,
			Action<IEntityPersister, CachePutData> cacheBatchingHandler)
		{
			return _queryLoader.GetRowFromResultSet(resultSet, session, queryParameters, lockModeArray, optionalObjectKey, hydratedObjects, keys, returnProxies, forcedResultTransformer, queryCacheResultBuilder, cacheBatchingHandler);
		}

		public void CreateSubselects(List<EntityKey[]> keys, QueryParameters queryParameters, ISessionImplementor session)
		{
			_queryLoader.CreateSubselects(keys, queryParameters, session);
		}

		public void InitializeEntitiesAndCollections(
			IList hydratedObjects,
			DbDataReader reader,
			ISessionImplementor session,
			bool readOnly,
			CacheBatcher cacheBatcher)
		{
			_queryLoader.InitializeEntitiesAndCollections(hydratedObjects, reader, session, readOnly, cacheBatcher);
		}

		public IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			return _queryLoader.GetResultList(results, resultTransformer);
		}

		public bool UseLimit(RowSelection selection, Dialect.Dialect dialect)
		{
			return _queryLoader.UseLimit(selection, dialect);
		}

		public void LoadCollection(ISessionImplementor session, object id, IType type)
		{
			_queryLoader.LoadCollection(session, id, type);
		}

		public void LoadCollectionBatch(ISessionImplementor session, object[] ids, IType type)
		{
			_queryLoader.LoadCollectionBatch(session, ids, type);
		}

		public bool IsCacheable(QueryParameters queryParameters)
		{
			return _queryLoader.IsCacheable(queryParameters);
		}

		public bool IsCacheable(QueryParameters queryParameters, bool supportsQueryCache, IEnumerable<IPersister> persisters)
		{
			return _queryLoader.IsCacheable(queryParameters, supportsQueryCache, persisters);
		}

		public ISqlCommand CreateSqlCommand(QueryParameters queryParameters, ISessionImplementor session)
		{
			return _queryLoader.CreateSqlCommand(queryParameters, session);
		}

		public void AutoDiscoverTypes(DbDataReader rs, QueryParameters queryParameters, IResultTransformer forcedResultTransformer)
		{
			_queryLoader.AutoDiscoverTypes(rs, queryParameters, forcedResultTransformer);
		}

		public IList TransformCacheableResults(QueryParameters queryParameters, CacheableResultTransformer transformer, IList result)
		{
			return _queryLoader.TransformCacheableResults(queryParameters, transformer, result);
		}

		public void HandleEmptyCollections(object[] keys, object resultSetId, ISessionImplementor session)
		{
			_queryLoader.HandleEmptyCollections(keys, resultSetId, session);
		}

		public void StopLoadingCollections(ISessionImplementor session, DbDataReader reader)
		{
			_queryLoader.StopLoadingCollections(session, reader);
		}

		public QueryKey GenerateQueryKey(ISessionImplementor session, QueryParameters queryParameters)
		{
			return _queryLoader.GenerateQueryKey(session, queryParameters);
		}

		public IList List(ISessionImplementor session, QueryParameters queryParameters)
		{
			return _queryLoader.List(session, queryParameters);
		}

		public IEnumerable GetEnumerable(QueryParameters queryParameters, IEventSource session)
		{
			return _queryLoader.GetEnumerable(queryParameters, session);
		}

		public Task<IList> ListAsync(ISessionImplementor session, QueryParameters queryParameters, CancellationToken cancellationToken)
		{
			return _queryLoader.ListAsync(session, queryParameters, cancellationToken);
		}

		public Task<IEnumerable> GetEnumerableAsync(QueryParameters queryParameters, IEventSource session, CancellationToken cancellationToken)
		{
			return _queryLoader.GetEnumerableAsync(queryParameters, session, cancellationToken);
		}
	}
}
