using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Loader
{
	public partial interface ILoader
	{
		bool IsSubselectLoadingEnabled { get; }

		/// <summary>
		/// The result types of the result set, for query loaders.
		/// </summary>
		IType[] ResultTypes { get; }

		IType[] CacheTypes { get; }
		Loader.QueryCacheInfo CacheInfo { get; }
		ISessionFactoryImplementor Factory { get; }

		/// <summary>
		/// The SqlString to be called; implemented by all subclasses
		/// </summary>
		SqlString SqlString { get; }

		/// <summary>
		/// An array of persisters of entity classes contained in each row of results;
		/// implemented by all subclasses
		/// </summary>
		/// <remarks>
		/// The <c>setter</c> was added so that classes inheriting from Loader could write a 
		/// value using the Property instead of directly to the field.
		/// </remarks>
		ILoadable[] EntityPersisters { get; }

		/// <summary> 
		/// Identifies the query for statistics reporting, if null,
		/// no statistics will be reported
		/// </summary>
		string QueryIdentifier { get; }

		/// <summary>
		/// What lock mode does this load entities with?
		/// </summary>
		/// <param name="lockModes">A Collection of lock modes specified dynamically via the Query Interface</param>
		/// <returns></returns>
		LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes);

		object GetRowFromResultSet(DbDataReader resultSet, ISessionImplementor session,
		                           QueryParameters queryParameters, LockMode[] lockModeArray,
		                           EntityKey optionalObjectKey, IList hydratedObjects, EntityKey[] keys,
		                           bool returnProxies, IResultTransformer forcedResultTransformer,
		                           QueryCacheResultBuilder queryCacheResultBuilder,
		                           Action<IEntityPersister, CachePutData> cacheBatchingHandler);

		void CreateSubselects(List<EntityKey[]> keys, QueryParameters queryParameters, ISessionImplementor session);

		void InitializeEntitiesAndCollections(IList hydratedObjects, 
		                                      DbDataReader reader, 
		                                      ISessionImplementor session, 
		                                      bool readOnly,
		                                      CacheBatcher cacheBatcher);

		IList GetResultList(IList results, IResultTransformer resultTransformer);

		/// <summary>
		/// Should we pre-process the SQL string, adding a dialect-specific
		/// LIMIT clause.
		/// </summary>
		/// <param name="selection"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		bool UseLimit(RowSelection selection, Dialect.Dialect dialect);

		/// <summary>
		/// Called by subclasses that load collections
		/// </summary>
		void LoadCollection(ISessionImplementor session, 
		                    object id, 
		                    IType type);

		/// <summary>
		/// Called by wrappers that batch initialize collections
		/// </summary>
		void LoadCollectionBatch(ISessionImplementor session, 
		                         object[] ids, 
		                         IType type);

		bool IsCacheable(QueryParameters queryParameters);
		
		bool IsCacheable(QueryParameters queryParameters,
		                 bool supportsQueryCache, 
		                 IEnumerable<IPersister> persisters);
		
		string ToString();
		
		ISqlCommand CreateSqlCommand(QueryParameters queryParameters, 
		                             ISessionImplementor session);

		void AutoDiscoverTypes(DbDataReader rs, 
		                       QueryParameters queryParameters, 
		                       IResultTransformer forcedResultTransformer);
		
		IList TransformCacheableResults(QueryParameters queryParameters, 
		                                CacheableResultTransformer transformer,
		                                IList result);

		void HandleEmptyCollections(object[] keys, 
		                            object resultSetId, 
		                            ISessionImplementor session);

		void StopLoadingCollections(ISessionImplementor session, 
		                            DbDataReader reader);

		QueryKey GenerateQueryKey(ISessionImplementor session, 
		                          QueryParameters queryParameters);
	}
}
