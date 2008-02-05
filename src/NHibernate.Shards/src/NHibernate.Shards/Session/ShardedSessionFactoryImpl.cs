using System.Collections;
using System.Collections.Generic;
using System.Data;
using Iesi.Collections;
using Iesi.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Context;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Shards.Engine;
using NHibernate.Shards.Strategy;
using NHibernate.Stat;
using NHibernate.Transaction;
using NHibernate.Type;

namespace NHibernate.Shards.Session
{
	public class ShardedSessionFactoryImpl : IShardedSessionFactoryImplementor, IControlSessionProvider
	{
		#region IControlSessionProvider Members

		/// <summary>
		/// Opens control session.
		/// </summary>
		/// <returns>control session</returns>
		public ISessionImplementor OpenControlSession()
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region IShardedSessionFactoryImplementor Members

		public IDictionary<ISessionFactoryImplementor, Set<ShardId>> GetSessionFactoryShardIdMap()
		{
			throw new System.NotImplementedException();
		}

		public bool ContainsFactory(ISessionFactoryImplementor factory)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// All an unmodifiable list of the <see cref="ISessionFactory"/> objects contained within.
		/// </summary>
		public IList<ISessionFactory> SessionFactories
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// This method is provided to allow a client to work on a subset of
		/// shards or a specialized <see cref="IShardStrategyFactory"/>.  By providing
		/// the desired shardIds, the client can limit operations to these shards.
		/// Alternatively, this method can be used to create a ShardedSessionFactory
		/// with different strategies that might be appropriate for a specific operation.
		///
		/// The factory returned will not be stored as one of the factories that would
		/// be returned by a call to getSessionFactories.
		/// </summary>
		/// <param name="shardIds"></param>
		/// <param name="shardStrategyFactory"></param>
		/// <returns>specially configured ShardedSessionFactory</returns>
		public IShardedSessionFactory GetSessionFactory(IList<ShardId> shardIds, IShardStrategyFactory shardStrategyFactory)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Create database connection(s) and open a ShardedSession on it,
		/// specifying an interceptor.
		/// </summary>
		/// <param name="interceptor">a session-scoped interceptor</param>
		/// <returns></returns>
		/// Throws <see cref="HibernateException"/>
		IShardedSession IShardedSessionFactory.OpenSession(IInterceptor interceptor)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Create database connection(s) and open a ShardedSession on it.
		/// </summary>
		/// <returns></returns>
		/// Throws <see cref="HibernateException"/>
		public IShardedSession openSession()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Open a <c>ISession</c> on the given connection
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <returns>A session</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="IConnectionProvider" />.
		/// </remarks>
		public ISession OpenSession(IDbConnection conn)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Create database connection and open a <c>ISession</c> on it, specifying an interceptor
		/// </summary>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		public ISession OpenSession(IInterceptor interceptor)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Open a <c>ISession</c> on the given connection, specifying an interceptor
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="IConnectionProvider" />.
		/// </remarks>
		public ISession OpenSession(IDbConnection conn, IInterceptor interceptor)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Create a database connection and open a <c>ISession</c> on it
		/// </summary>
		/// <returns></returns>
		public ISession OpenSession()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Create a new databinder.
		/// </summary>
		/// <returns></returns>
		public IDatabinder OpenDatabinder()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the <c>ClassMetadata</c> associated with the given entity class
		/// </summary>
		/// <param name="persistentType"></param>
		/// <returns></returns>
		public IClassMetadata GetClassMetadata(System.Type persistentType)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the <c>CollectionMetadata</c> associated with the named collection role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public ICollectionMetadata GetCollectionMetadata(string roleName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get all <c>ClassMetadata</c> as a <c>IDictionary</c> from <c>Type</c>
		/// to metadata object
		/// </summary>
		/// <returns></returns>
		public IDictionary GetAllClassMetadata()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get all <c>CollectionMetadata</c> as a <c>IDictionary</c> from role name
		/// to metadata object
		/// </summary>
		/// <returns></returns>
		public IDictionary GetAllCollectionMetadata()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Destroy this <c>SessionFactory</c> and release all resources 
		/// connection pools, etc). It is the responsibility of the application
		/// to ensure that there are no open <c>Session</c>s before calling
		/// <c>close()</c>. 
		/// </summary>
		public void Close()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="persistentClass"></param>
		public void Evict(System.Type persistentClass)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		public void Evict(System.Type persistentClass, object id)
		{
			throw new System.NotImplementedException();
		}

		/// <summary> 
		/// Evict all entries from the second-level cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		public void EvictEntity(string entityName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		public void EvictCollection(string roleName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="id"></param>
		public void EvictCollection(string roleName, object id)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Evict any query result sets cached in the default query cache region.
		/// </summary>
		public void EvictQueries()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Evict any query result sets cached in the named query cache region.
		/// </summary>
		/// <param name="cacheRegion"></param>
		public void EvictQueries(string cacheRegion)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the <see cref="IConnectionProvider" /> used.
		/// </summary>
		public IConnectionProvider ConnectionProvider
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Get the SQL <c>Dialect</c>
		/// </summary>
		public Dialect.Dialect Dialect
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Obtain a set of the names of all filters defined on this SessionFactory.
		/// </summary>
		/// <return>The set of filter names.</return>
		public ICollection<string> DefinedFilterNames
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Obtain the definition of a filter by name.
		/// </summary>
		/// <param name="filterName">The name of the filter for which to obtain the definition.</param>
		/// <return>The filter definition.</return>
		public FilterDefinition GetFilterDefinition(string filterName)
		{
			throw new System.NotImplementedException();
		}

		public Settings Settings
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// This collections allows external libraries
		/// to add their own configuration to the NHibernate session factory.
		/// This is needed in such cases where the library is tightly coupled to NHibernate, such
		/// as the case of NHibernate Search
		/// </summary>
		public IDictionary Items
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Obtains the current session.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The definition of what exactly "current" means is controlled by the <see cref="NHibernate.Context.ICurrentSessionContext" />
		/// implementation configured for use.
		/// </para>
		/// </remarks>
		/// <returns>The current session.</returns>
		/// <exception cref="HibernateException">Indicates an issue locating a suitable current session.</exception>
		public ISession GetCurrentSession()
		{
			throw new System.NotImplementedException();
		}

		/// <summary> Get a new stateless session.</summary>
		public IStatelessSession OpenStatelessSession()
		{
			throw new System.NotImplementedException();
		}

		/// <summary> Get a new stateless session for the given ADO.NET connection.</summary>
		public IStatelessSession OpenStatelessSession(IDbConnection connection)
		{
			throw new System.NotImplementedException();
		}

		/// <summary> Get the statistics for this session factory</summary>
		public IStatistics Statistics
		{
			get { throw new System.NotImplementedException(); }
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the persister for a class
		/// </summary>
		public IEntityPersister GetEntityPersister(System.Type clazz)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <returns>The <see cref="IEntityPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IEntityPersister"/> can be found.</exception>
		public IEntityPersister GetEntityPersister(string className)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <param name="throwIfNotFound">Whether to throw an exception if the class is not found,
		/// or just return <see langword="null" /></param>
		/// <returns>The <see cref="IEntityPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IEntityPersister"/> can be found
		/// and throwIfNotFound is true.</exception>
		public IEntityPersister GetEntityPersister(string className, bool throwIfNotFound)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the persister object for a collection role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public ICollectionPersister GetCollectionPersister(string role)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Is outerjoin fetching enabled?
		/// </summary>
		public bool IsOuterJoinedFetchEnabled
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Are scrollable <c>ResultSet</c>s supported?
		/// </summary>
		public bool IsScrollableResultSetsEnabled
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Is <c>PreparedStatement.getGeneratedKeys</c> supported (Java-specific?)
		/// </summary>
		public bool IsGetGeneratedKeysEnabled
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Get the database schema specified in <c>default_schema</c>
		/// </summary>
		public string DefaultSchema
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Get the return types of a query
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public IType[] GetReturnTypes(string queryString)
		{
			throw new System.NotImplementedException();
		}

		/// <summary> Get the return aliases of a query</summary>
		public string[] GetReturnAliases(string queryString)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get the names of all persistent classes that implement/extend the given interface/class
		/// </summary>
		/// <param name="clazz"></param>
		/// <returns></returns>
		public string[] GetImplementors(System.Type clazz)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Get a class name, using query language imports
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetImportedClassName(string name)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Maximum depth of outer join fetching
		/// </summary>
		public int MaximumFetchDepth
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Get the default query cache
		/// </summary>
		public IQueryCache QueryCache
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Get a particular named query cache, or the default cache
		/// </summary>
		/// <param name="regionName">the name of the cache region, or null for the default
		/// query cache</param>
		/// <returns>the existing cache, or a newly created cache if none by that
		/// region name</returns>
		public IQueryCache GetQueryCache(string regionName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Is query caching enabled?
		/// </summary>
		public bool IsQueryCacheEnabled
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Obtain an ADO.NET connection
		/// </summary>
		/// <returns></returns>
		public IDbConnection OpenConnection()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Release an ADO.NET connection
		/// </summary>
		/// <param name="conn"></param>
		public void CloseConnection(IDbConnection conn)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Gets the IsolationLevel an IDbTransaction should be set to.
		/// </summary>
		/// <remarks>
		/// This is only applicable to manually controlled NHibernate Transactions.
		/// </remarks>
		public IsolationLevel Isolation
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Get the identifier generator for the hierarchy
		/// </summary>
		public IIdentifierGenerator GetIdentifierGenerator(System.Type rootClass)
		{
			throw new System.NotImplementedException();
		}

		public ResultSetMappingDefinition GetResultSetMapping(string resultSetRef)
		{
			throw new System.NotImplementedException();
		}

		public ITransactionFactory TransactionFactory
		{
			get { throw new System.NotImplementedException(); }
		}

		public SQLFunctionRegistry SQLFunctionRegistry
		{
			get { throw new System.NotImplementedException(); }
		}

		public IEntityNotFoundDelegate EntityNotFoundDelegate
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Gets the ICurrentSessionContext instance attached to this session factory.
		/// </summary>
		public ICurrentSessionContext CurrentSessionContext
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Open a session conforming to the given parameters. For use mainly by
		/// <see cref="Context.ICurrentSessionContext" /> implementations.
		/// </summary>
		/// <param name="connection">The external ADO.NET connection to use, if any (i.e., optional).</param>
		/// <param name="connectionReleaseMode">The release mode for managed database connections.</param>
		/// <returns>An appropriate session.</returns>
		/// <exception cref="HibernateException" />
		public ISession OpenSession(IDbConnection connection, ConnectionReleaseMode connectionReleaseMode)
		{
			throw new System.NotImplementedException();
		}

		/// <summary> 
		/// Retrieves a set of all the collection roles in which the given entity
		/// is a participant, as either an index or an element.
		/// </summary>
		/// <param name="entityName">The entity name for which to get the collection roles.</param>
		/// <returns> 
		/// Set of all the collection roles in which the given entityName participates.
		/// </returns>
		public ISet GetCollectionRolesByEntityParticipant(string entityName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary> The cache of table update timestamps</summary>
		public UpdateTimestampsCache UpdateTimestampsCache
		{
			get { throw new System.NotImplementedException(); }
		}

		public IDictionary GetAllSecondLevelCacheRegions()
		{
			throw new System.NotImplementedException();
		}

		/// <summary> Get a named second-level cache region</summary>
		public ICache GetSecondLevelCacheRegion(string regionName)
		{
			throw new System.NotImplementedException();
		}

		/// <summary> Statistics SPI</summary>
		public IStatisticsImplementor StatisticsImplementor
		{
			get { throw new System.NotImplementedException(); }
		}

		public QueryPlanCache QueryPlanCache
		{
			get { throw new System.NotImplementedException(); }
		}

		public IType GetIdentifierType(string className)
		{
			throw new System.NotImplementedException();
		}

		public string GetIdentifierPropertyName(string className)
		{
			throw new System.NotImplementedException();
		}

		public IType GetReferencedPropertyType(string className, string propertyName)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}