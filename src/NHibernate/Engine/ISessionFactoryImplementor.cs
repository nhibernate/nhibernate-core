using System.Data;
using NHibernate.Cache;
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
using System.Collections;
using Iesi.Collections;

namespace NHibernate.Engine
{
	/// <summary>
	/// Defines the internal contract between the <c>ISessionFactory</c> and other parts of NHibernate
	/// such as implementors of <c>IType</c>.
	/// </summary>
	public interface ISessionFactoryImplementor : IMapping, ISessionFactory
	{
		/// <summary>
		/// Get the persister for a class
		/// </summary>
		IEntityPersister GetEntityPersister(System.Type clazz);

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <returns>The <see cref="IEntityPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IEntityPersister"/> can be found.</exception>
		IEntityPersister GetEntityPersister(string className);

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <param name="throwIfNotFound">Whether to throw an exception if the class is not found,
		/// or just return <see langword="null" /></param>
		/// <returns>The <see cref="IEntityPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IEntityPersister"/> can be found
		/// and throwIfNotFound is true.</exception>
		IEntityPersister GetEntityPersister(string className, bool throwIfNotFound);

		/// <summary>
		/// Get the persister object for a collection role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		ICollectionPersister GetCollectionPersister(string role);

		/// <summary>
		/// Is outerjoin fetching enabled?
		/// </summary>
		bool IsOuterJoinedFetchEnabled { get; }

		/// <summary>
		/// Are scrollable <c>ResultSet</c>s supported?
		/// </summary>
		bool IsScrollableResultSetsEnabled { get; }

		/// <summary>
		/// Is <c>PreparedStatement.getGeneratedKeys</c> supported (Java-specific?)
		/// </summary>
		bool IsGetGeneratedKeysEnabled { get; }

		/// <summary>
		/// Get the database schema specified in <c>default_schema</c>
		/// </summary>
		string DefaultSchema { get; }

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
		/// <param name="className"></param>
		/// <returns></returns>
		string[] GetImplementors(string className);

		/// <summary>
		/// Get a class name, using query language imports
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string GetImportedClassName(string name);

		/// <summary>
		/// Maximum depth of outer join fetching
		/// </summary>
		int MaximumFetchDepth { get; }

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
		/// Is query caching enabled?
		/// </summary>
		bool IsQueryCacheEnabled { get; }

		/// <summary>
		/// Obtain an ADO.NET connection
		/// </summary>
		/// <returns></returns>
		IDbConnection OpenConnection();

		/// <summary>
		/// Release an ADO.NET connection
		/// </summary>
		/// <param name="conn"></param>
		void CloseConnection(IDbConnection conn);

		/// <summary>
		/// Gets the IsolationLevel an IDbTransaction should be set to.
		/// </summary>
		/// <remarks>
		/// This is only applicable to manually controlled NHibernate Transactions.
		/// </remarks>
		IsolationLevel Isolation { get; }

		/// <summary>
		/// Get the identifier generator for the hierarchy
		/// </summary>
		IIdentifierGenerator GetIdentifierGenerator(System.Type rootClass);

		ResultSetMappingDefinition GetResultSetMapping(string resultSetRef);

		ITransactionFactory TransactionFactory { get; }

		/// <summary> Retrieves the SQLExceptionConverter in effect for this SessionFactory. </summary>
		/// <returns> The SQLExceptionConverter for this SessionFactory. </returns>
		ISQLExceptionConverter SQLExceptionConverter { get;}

		SQLFunctionRegistry SQLFunctionRegistry { get; }

		IEntityNotFoundDelegate EntityNotFoundDelegate { get;}

		/// <summary>
		/// Gets the ICurrentSessionContext instance attached to this session factory.
		/// </summary>
		ICurrentSessionContext CurrentSessionContext { get; }

		/// <summary>
		/// Open a session conforming to the given parameters. For use mainly by
		/// <see cref="Context.ICurrentSessionContext" /> implementations.
		/// </summary>
		/// <param name="connection">The external ADO.NET connection to use, if any (i.e., optional).</param>
		/// <param name="connectionReleaseMode">The release mode for managed database connections.</param>
		/// <returns>An appropriate session.</returns>
		/// <exception cref="HibernateException" />
		ISession OpenSession(
			IDbConnection connection,
			ConnectionReleaseMode connectionReleaseMode);

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
		ISession OpenSession(
			IDbConnection connection,
			bool flushBeforeCompletionEnabled,
			bool autoCloseSessionEnabled,
			ConnectionReleaseMode connectionReleaseMode);

		/// <summary> 
		/// Retrieves a set of all the collection roles in which the given entity
		/// is a participant, as either an index or an element.
		/// </summary>
		/// <param name="entityName">The entity name for which to get the collection roles.</param>
		/// <returns> 
		/// Set of all the collection roles in which the given entityName participates.
		/// </returns>
		ISet GetCollectionRolesByEntityParticipant(string entityName);

		/// <summary> The cache of table update timestamps</summary>
		UpdateTimestampsCache UpdateTimestampsCache { get;}

		IDictionary GetAllSecondLevelCacheRegions();

		/// <summary> Get a named second-level cache region</summary>
		ICache GetSecondLevelCacheRegion(string regionName);

		/// <summary> Statistics SPI</summary>
		IStatisticsImplementor StatisticsImplementor { get;}

		QueryPlanCache QueryPlanCache { get;}

	}
}
