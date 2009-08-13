using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.Collection;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Entity;
using NHibernate.Transaction;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// Defines the internal contract between the <c>Session</c> and other parts of Hibernate
	/// such as implementors of <c>Type</c> or <c>ClassPersister</c>
	/// </summary>
	public interface ISessionImplementor
	{
		/// <summary>
		/// Initialize the session after its construction was complete
		/// </summary>
		void Initialize();

		/// <summary>
		/// Initialize the collection (if not already initialized)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="writing"></param>
		void InitializeCollection(IPersistentCollection collection, bool writing);

		// NH-268
		/// <summary>
		/// Load an instance without checking if it was deleted. If it does not exist and isn't nullable, throw an exception.
		/// This method may create a new proxy or return an existing proxy.
		/// </summary>
		/// <param name="entityName">The entityName (or class full name) to load.</param>
		/// <param name="id">The identifier of the object in the database.</param>
		/// <param name="isNullable">Allow null instance</param>
		/// <param name="eager">When enabled, the object is eagerly fetched.</param>
		/// <returns>
		/// A proxy of the object or an instance of the object if the <c>persistentClass</c> does not have a proxy.
		/// </returns>
		/// <exception cref="ObjectNotFoundException">No object could be found with that <c>id</c>.</exception>
		object InternalLoad(string entityName, object id, bool eager, bool isNullable);

		/// <summary>
		/// Load an instance immediately. Do not return a proxy.
		/// </summary>
		/// <param name="entityName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object ImmediateLoad(string entityName, object id);

		/// <summary>
		/// System time before the start of the transaction
		/// </summary>
		/// <returns></returns>
		long Timestamp { get; }

		/// <summary>
		/// Get the creating SessionFactoryImplementor
		/// </summary>
		/// <returns></returns>
		ISessionFactoryImplementor Factory { get; }

		/// <summary>
		/// Get the prepared statement <c>Batcher</c> for this session
		/// </summary>
		IBatcher Batcher { get; }

		/// <summary>
		/// Execute a <c>List()</c> query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IList List(string query, QueryParameters parameters);

        /// <summary>
        /// Execute a <c>List()</c> expression query
        /// </summary>
        /// <param name="queryExpression"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IList List(IQueryExpression queryExpression, QueryParameters parameters);

		void List(string query, QueryParameters parameters, IList results);

		/// <summary>
		/// Strongly-typed version of <see cref="List(string,QueryParameters)" />
		/// </summary>
		IList<T> List<T>(string query, QueryParameters queryParameters);

		/// <summary>
		/// Strongly-typed version of <see cref="List(CriteriaImpl)" />
		/// </summary>
		IList<T> List<T>(CriteriaImpl criteria);

		void List(CriteriaImpl criteria, IList results);

		IList List(CriteriaImpl criteria);

		/// <summary>
		/// Execute an <c>Iterate()</c> query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IEnumerable Enumerable(string query, QueryParameters parameters);

		/// <summary>
		/// Strongly-typed version of <see cref="Enumerable(string, QueryParameters)" />
		/// </summary>
		IEnumerable<T> Enumerable<T>(string query, QueryParameters queryParameters);

		/// <summary>
		/// Execute a filter
		/// </summary>
		IList ListFilter(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Execute a filter (strongly-typed version).
		/// </summary>
		IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Collection from a filter
		/// </summary>
		IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Strongly-typed version of <see cref="EnumerableFilter(object, string, QueryParameters)" />
		/// </summary>
		IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters);

		/// <summary> Get the <see cref="IEntityPersister"/> for any instance</summary>
		/// <param name="entityName">optional entity name </param>
		/// <param name="obj">the entity instance </param>
		IEntityPersister GetEntityPersister(string entityName, object obj);

		/// <summary>
		/// Notify the session that an NHibernate transaction has begun.
		/// </summary>
		void AfterTransactionBegin(ITransaction tx);

		/// <summary>
		/// Notify the session that the transaction is about to complete
		/// </summary>
		void BeforeTransactionCompletion(ITransaction tx);

		/// <summary>
		/// Notify the session that the transaction completed, so we no longer own the old locks.
		/// (Also we should release cache softlocks). May be called multiple times during the transaction
		/// completion process.
		/// </summary>
		void AfterTransactionCompletion(bool successful, ITransaction tx);

		/// <summary>
		/// Return the identifier of the persistent object, or null if transient
		/// </summary>
		object GetContextEntityIdentifier(object obj);

		/// <summary>
		/// Instantiate the entity class, initializing with the given identifier
		/// </summary>
		object Instantiate(string entityName, object id);

		/// <summary>
		/// Execute an SQL Query
		/// </summary>
		IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters);

		void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results);

		/// <summary>
		/// Strongly-typed version of <see cref="List(NativeSQLQuerySpecification, QueryParameters)" />
		/// </summary>
		IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters);

		/// <summary> Execute an SQL Query</summary>
		void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results);

		IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters);

		/// <summary>
		/// Retrieve the currently set value for a filter parameter.
		/// </summary>
		/// <param name="filterParameterName">The filter parameter name in the format 
		/// {FILTER_NAME.PARAMETER_NAME}.</param>
		/// <returns>The filter parameter value.</returns>
		object GetFilterParameterValue(string filterParameterName);

		/// <summary>
		/// Retrieve the type for a given filter parameter.
		/// </summary>
		/// <param name="filterParameterName">The filter parameter name in the format 
		/// {FILTER_NAME.PARAMETER_NAME}.</param>
		/// <returns>The filter parameter type.</returns>
		IType GetFilterParameterType(string filterParameterName);

		/// <summary>
		/// Return the currently enabled filters.  The filter map is keyed by filter
		/// name, with values corresponding to the <see cref="FilterImpl"/>
		/// instance.
		/// </summary>
		/// <returns>The currently enabled filters.</returns>
		IDictionary<string, IFilter> EnabledFilters { get; }

		IQuery GetNamedSQLQuery(string name);

		IQueryTranslator[] GetQueries(string query, bool scalar); // NH specific for MultiQuery

		IInterceptor Interceptor { get; }

		/// <summary> Retrieves the configured event listeners from this event source. </summary>
		EventListeners Listeners { get; }

		int DontFlushFromFind { get; }

		ConnectionManager ConnectionManager { get; }

		bool IsEventSource { get; }

		/// <summary> 
		/// Get the entity instance associated with the given <tt>Key</tt>,
		/// calling the Interceptor if necessary
		/// </summary>
		object GetEntityUsingInterceptor(EntityKey key);

		/// <summary> Get the persistence context for this session</summary>
		IPersistenceContext PersistenceContext { get; }

		CacheMode CacheMode { get; set; }

		/// <summary>
		/// Is the <c>ISession</c> still open?
		/// </summary>
		bool IsOpen { get; }

		/// <summary>
		/// Is the <c>ISession</c> currently connected?
		/// </summary>
		bool IsConnected { get; }

		FlushMode FlushMode { get; set; }

		string FetchProfile { get; set; }

		/// <summary> The best guess entity name for an entity not in an association</summary>
		string BestGuessEntityName(object entity);

		/// <summary> The guessed entity name for an entity not in an association</summary>
		string GuessEntityName(object entity);

		IDbConnection Connection { get; }

		IQuery GetNamedQuery(string queryName);

		/// <summary> Determine whether the session is closed.  Provided separately from
		/// {@link #isOpen()} as this method does not attempt any JTA synch
		/// registration, where as {@link #isOpen()} does; which makes this one
		/// nicer to use for most internal purposes. 
		/// </summary>
		/// <returns> True if the session is closed; false otherwise.
		/// </returns>
		bool IsClosed { get; }

		void Flush();

		/// <summary> 
		/// Does this <tt>Session</tt> have an active Hibernate transaction
		/// or is there a JTA transaction in progress?
		/// </summary>
		bool TransactionInProgress { get; }

		/// <summary> Retrieve the entity mode in effect for this session. </summary>
		EntityMode EntityMode { get; }

		/// <summary> Execute a native SQL update or delete query</summary>
		int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters);

		/// <summary> Execute a HQL update or delete query</summary>
		int ExecuteUpdate(string query, QueryParameters queryParameters);

		FutureCriteriaBatch FutureCriteriaBatch { get; }

		FutureQueryBatch FutureQueryBatch { get; }

		Guid SessionId { get; }

		ITransactionContext TransactionContext { get; set; }

		void CloseSessionFromDistributedTransaction();
	}
}
