using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Custom;
using NHibernate.Multi;
using NHibernate.Persister.Entity;
using NHibernate.Transaction;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	// 6.0 TODO: Convert to interface methods, excepted SwitchCacheMode
	public static partial class SessionImplementorExtensions
	{
		//6.0 TODO: Expose as TenantIdentifier property
		/// <summary>
		/// Obtain the tenant identifier associated with this session.
		/// </summary>
		/// <returns> The tenant identifier associated with this session or null </returns>
		public static string GetTenantIdentifier(this ISessionImplementor session)
		{
			if (session is AbstractSessionImpl sessionImpl)
			{
				return sessionImpl.TenantIdentifier;
			}

			return null;
		}

		/// <summary>
		/// Instantiate the entity class, initializing with the given identifier
		/// </summary>
		internal static object Instantiate(this ISessionImplementor session, IEntityPersister persister, object id)
		{
			if(session is AbstractSessionImpl impl)
				return impl.Instantiate(persister, id);
			return session.Instantiate(persister.EntityName, id);
		}

		internal static IDisposable BeginContext(this ISessionImplementor session)
		{
			if (session == null)
				return null;
			return session is AbstractSessionImpl impl
				? impl.BeginContext()
				: SessionIdLoggingContext.CreateOrNull(session.SessionId);
		}

		internal static IDisposable BeginProcess(this ISessionImplementor session)
		{
			if (session == null)
				return null;
			return session is AbstractSessionImpl impl
				? impl.BeginProcess()
				// This method has only replaced bare call to setting the id, so this fallback is enough for avoiding a
				// breaking change in case a custom session implementation is used.
				: SessionIdLoggingContext.CreateOrNull(session.SessionId);
		}

		//6.0 TODO: Expose as ISessionImplementor.FutureBatch and replace method usages with property
		internal static IQueryBatch GetFutureBatch(this ISessionImplementor session)
		{
			return ReflectHelper.CastOrThrow<AbstractSessionImpl>(session, "future batch").FutureBatch;
		}

		internal static void AutoFlushIfRequired(this ISessionImplementor implementor, ISet<string> querySpaces)
		{
			(implementor as AbstractSessionImpl)?.AutoFlushIfRequired(querySpaces);
		}

		/// <summary>
		/// Switch the session current cache mode.
		/// </summary>
		/// <param name="session">The session for which the cache mode has to be switched.</param>
		/// <param name="cacheMode">The desired cache mode. <see langword="null" /> for not actually switching.</param>
		/// <returns><see langword="null" /> if no switch is required, otherwise an <see cref="IDisposable"/> which
		/// dispose will set the session cache mode back to its original value.</returns>
		internal static IDisposable SwitchCacheMode(this ISessionImplementor session, CacheMode? cacheMode)
		{
			if (!cacheMode.HasValue || cacheMode == session.CacheMode)
				return null;
			return new CacheModeSwitch(session, cacheMode.Value);
		}

		private sealed class CacheModeSwitch : IDisposable
		{
			private ISessionImplementor _session;
			private readonly CacheMode _originalCacheMode;

			public CacheModeSwitch(ISessionImplementor session, CacheMode cacheMode)
			{
				_session = session;
				_originalCacheMode = session.CacheMode;
				_session.CacheMode = cacheMode;
			}

			public void Dispose()
			{
				if (_session == null)
					throw new ObjectDisposedException("The session cache mode switch has been disposed already");
				_session.CacheMode = _originalCacheMode;
				_session = null;
			}
		}
	}

	/// <summary>
	/// Defines the internal contract between the <c>Session</c> and other parts of NHibernate
	/// such as implementors of <c>Type</c> or <c>ClassPersister</c>
	/// </summary>
	public partial interface ISessionImplementor
	{
		/// <summary>
		/// Initialize the session after its construction was complete
		/// </summary>
		// Since v5.1
		[Obsolete("This method has no more usages in NHibernate and will be removed.")]
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
		/// Execute a <c>List()</c> expression query
		/// </summary>
		/// <param name="queryExpression"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IList List(IQueryExpression queryExpression, QueryParameters parameters);

		/// <summary>
		/// Create a new instance of <c>Query</c> for the given query expression
		/// <param name="queryExpression">A hibernate query expression</param>
		/// <returns>The query</returns>
		/// </summary>
		IQuery CreateQuery(IQueryExpression queryExpression);

		void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results);

		/// <summary>
		/// Strongly-typed version of <see cref="List(IQueryExpression,QueryParameters)" />
		/// </summary>
		IList<T> List<T>(IQueryExpression queryExpression, QueryParameters queryParameters);

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
		IEnumerable Enumerable(IQueryExpression query, QueryParameters parameters);

		/// <summary>
		/// Strongly-typed version of <see cref="Enumerable(IQueryExpression, QueryParameters)" />
		/// </summary>
		IEnumerable<T> Enumerable<T>(IQueryExpression query, QueryParameters queryParameters);

		/// <summary>
		/// Execute a filter
		/// </summary>
		IList ListFilter(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Execute a filter
		/// </summary>
		IList ListFilter(object collection, IQueryExpression queryExpression, QueryParameters parameters);

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
		/// 
		/// </summary>
		void FlushBeforeTransactionCompletion();

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

		//Since 5.3
		//TODO 6.0 Remove (see SessionImplementorExtensions.Instantiate for replacement)
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
		
		// Since v5.2
		[Obsolete("This method has no usages and will be removed in a future version")]
		IQueryTranslator[] GetQueries(IQueryExpression query, bool scalar); // NH specific for MultiQuery

		IInterceptor Interceptor { get; }

		/// <summary> Retrieves the configured event listeners from this event source. </summary>
		EventListeners Listeners { get; }

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
		/// Is the session connected?
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the session is connected.
		/// </value>
		/// <remarks>
		/// A session is considered connected if there is a <see cref="DbConnection"/> (regardless
		/// of its state) or if the field <c>connect</c> is true. Meaning that it will connect
		/// at the next operation that requires a connection.
		/// </remarks>
		bool IsConnected { get; }

		FlushMode FlushMode { get; set; }

		string FetchProfile { get; set; }

		/// <summary> The best guess entity name for an entity not in an association</summary>
		string BestGuessEntityName(object entity);

		/// <summary> The guessed entity name for an entity not in an association</summary>
		string GuessEntityName(object entity);

		DbConnection Connection { get; }

		IQuery GetNamedQuery(string queryName);

		/// <summary>
		/// Determine whether the session is closed. Provided separately from
		/// <c>IsOpen</c> as this method does not attempt any system transaction sync
		/// registration, whereas <c>IsOpen</c> is allowed to (does not currently, but may do
		/// in a future version as it is the case in Hibernate); which makes this one
		/// nicer to use for most internal purposes.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the session is closed; <see langword="false" /> otherwise.
		/// </returns>
		bool IsClosed { get; }

		void Flush();

		/// <summary>
		/// Does this <c>ISession</c> have an active NHibernate transaction
		/// or is there a system transaction in progress in which the session is enlisted?
		/// </summary>
		bool TransactionInProgress { get; }

		/// <summary> Execute a native SQL update or delete query</summary>
		int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters);

		/// <summary> Execute a HQL update or delete query</summary>
		int ExecuteUpdate(IQueryExpression query, QueryParameters queryParameters);

		//Since 5.2
		[Obsolete("Replaced by FutureBatch")]
		FutureCriteriaBatch FutureCriteriaBatch { get; }

		//Since 5.2
		[Obsolete("Replaced by FutureBatch")]
		FutureQueryBatch FutureQueryBatch { get; }

		Guid SessionId { get; }

		ITransactionContext TransactionContext { get; set; }

		/// <summary>
		/// Join the <see cref="System.Transactions.Transaction.Current"/> system transaction.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Sessions auto-join current transaction by default on their first usage within a scope.
		/// This can be disabled with <see cref="ISessionBuilder{T}.AutoJoinTransaction(bool)"/> from
		/// a session builder obtained with <see cref="ISessionFactory.WithOptions()"/>.
		/// </para>
		/// <para>
		/// This method allows to explicitly join the current transaction. It does nothing if it is already
		/// joined.
		/// </para>
		/// </remarks>
		/// <exception cref="HibernateException">Thrown if there is no current transaction.</exception>
		void JoinTransaction();

		void CloseSessionFromSystemTransaction();

		IQuery CreateFilter(object collection, IQueryExpression queryExpression);

		EntityKey GenerateEntityKey(object id, IEntityPersister persister);

		CacheKey GenerateCacheKey(object id, IType type, string entityOrRoleName);
	}
}
