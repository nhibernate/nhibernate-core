using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Context;
using NHibernate.Dialect.Function;
using NHibernate.Engine.Query;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Stat;
using NHibernate.Transaction;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// Defines the internal contract between the <c>ISessionFactory</c> and other parts of NHibernate
	/// such as implementors of <c>IType</c>.
	/// </summary>
	public interface ISessionFactoryImplementor : IMapping, ISessionFactory
	{
		/// <summary>
		/// Get the SQL <see cref="NHibernate.Dialect.Dialect"/>.
		/// </summary>
		Dialect.Dialect Dialect { get; }

		IInterceptor Interceptor { get; }

		QueryPlanCache QueryPlanCache { get; }

		/// <summary>
		/// Get the <see cref="IConnectionProvider" /> used.
		/// </summary>
		IConnectionProvider ConnectionProvider { get; }

		ITransactionFactory TransactionFactory { get; }

		/// <summary> The cache of table update timestamps</summary>
		UpdateTimestampsCache UpdateTimestampsCache { get; }

		/// <summary> Statistics SPI</summary>
		IStatisticsImplementor StatisticsImplementor { get; }

		/// <summary> Retrieves the SQLExceptionConverter in effect for this SessionFactory. </summary>
		/// <returns> The SQLExceptionConverter for this SessionFactory. </returns>
		ISQLExceptionConverter SQLExceptionConverter { get; }

		Settings Settings { get; }

		IEntityNotFoundDelegate EntityNotFoundDelegate { get; }

		SQLFunctionRegistry SQLFunctionRegistry { get; }

		IDictionary<string, ICache> GetAllSecondLevelCacheRegions();

		/// <summary>
		/// Get the persister for the named entity
		/// </summary>
		/// <param name="entityName">The name of the entity that is persisted.</param>
		/// <returns>The <see cref="IEntityPersister"/> for the entity.</returns>
		/// <exception cref="MappingException">If no <see cref="IEntityPersister"/> can be found.</exception>
		IEntityPersister GetEntityPersister(string entityName);

		/// <summary>
		/// Get the persister object for a collection role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		ICollectionPersister GetCollectionPersister(string role);

		/// <summary>
		/// Get the return types of a query
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		IType[] GetReturnTypes(string queryString);

		/// <summary> Get the return aliases of a query</summary>
		string[] GetReturnAliases(string queryString);

		/// <summary>
		/// Get the names of all persistent classes that implement/extend the given interface/class
		/// </summary>
		/// <param name="entityOrClassName">The entity-name, the class name or full name, the imported class name.</param>
		/// <returns>All implementors class names.</returns>
		string[] GetImplementors(string entityOrClassName);

		/// <summary>
		/// Get a class name, using query language imports
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string GetImportedClassName(string name);

		/// <summary>
		/// Get the default query cache
		/// </summary>
		IQueryCache QueryCache { get; }

		/// <summary>
		/// Get a particular named query cache, or the default cache
		/// </summary>
		/// <param name="regionName">the name of the cache region, or null for the default
		/// query cache</param>
		/// <returns>the existing cache, or a newly created cache if none by that
		/// region name</returns>
		IQueryCache GetQueryCache(string regionName);

		/// <summary>
		/// Gets the <c>hql</c> query identified by the <c>name</c>.
		/// </summary>
		/// <param name="queryName">The name of that identifies the query.</param>
		/// <returns>
		/// A <c>hql</c> query or <see langword="null" /> if the named
		/// query does not exist.
		/// </returns>
		NamedQueryDefinition GetNamedQuery(string queryName);

		NamedSQLQueryDefinition GetNamedSQLQuery(string queryName);

		ResultSetMappingDefinition GetResultSetMapping(string resultSetRef);

		/// <summary>
		/// Get the identifier generator for the hierarchy
		/// </summary>
		IIdentifierGenerator GetIdentifierGenerator(string rootEntityName);

		/// <summary> Get a named second-level cache region</summary>
		ICache GetSecondLevelCacheRegion(string regionName);

		/// <summary>
		/// Open a session conforming to the given parameters. Used mainly
		/// for current session processing.
		/// </summary>
		/// <param name="connection">The external ado.net connection to use, if one (i.e., optional).</param>
		/// <param name="flushBeforeCompletionEnabled">
		/// Should the session be auto-flushed 
		/// prior to transaction completion?
		/// </param>
		/// <param name="autoCloseSessionEnabled">
		/// Should the session be auto-closed after
		/// transaction completion?
		/// </param>
		/// <param name="connectionReleaseMode">The release mode for managed jdbc connections.</param>
		/// <returns>An appropriate session.</returns>
		ISession OpenSession(DbConnection connection, bool flushBeforeCompletionEnabled, bool autoCloseSessionEnabled,
		                     ConnectionReleaseMode connectionReleaseMode);

		/// <summary> 
		/// Retrieves a set of all the collection roles in which the given entity
		/// is a participant, as either an index or an element.
		/// </summary>
		/// <param name="entityName">The entity name for which to get the collection roles.</param>
		/// <returns> 
		/// Set of all the collection roles in which the given entityName participates.
		/// </returns>
		ISet<string> GetCollectionRolesByEntityParticipant(string entityName);

		#region NHibernate specific

		/// <summary>
		/// Gets the ICurrentSessionContext instance attached to this session factory.
		/// </summary>
		ICurrentSessionContext CurrentSessionContext { get; } // TODO NH : Remove

		/// <summary>
		/// Get the persister for the named entity
		/// </summary>
		/// <param name="entityName">The name of the entity that is persisted.</param>
		/// <returns>
		/// The <see cref="IEntityPersister"/> for the entity or <see langword="null"/> is the name was not found.
		/// </returns>
		IEntityPersister TryGetEntityPersister(string entityName);

		/// <summary>
		/// Get the entity-name for a given mapped class.
		/// </summary>
		/// <param name="implementor">the mapped class</param>
		/// <returns>the entity name where available or null</returns>
		string TryGetGuessEntityName(System.Type implementor);
		#endregion
	}
}