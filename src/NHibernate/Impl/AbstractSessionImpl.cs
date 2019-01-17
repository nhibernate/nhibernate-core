using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Linq;
using NHibernate.Loader.Custom;
using NHibernate.Loader.Custom.Sql;
using NHibernate.Multi;
using NHibernate.Persister.Entity;
using NHibernate.Transaction;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary> Functionality common to stateless and stateful sessions </summary>
	[Serializable]
	public abstract partial class AbstractSessionImpl : ISessionImplementor
	{
		[NonSerialized]
		private ISessionFactoryImplementor _factory;
		private FlushMode _flushMode;

		[NonSerialized]
		private IQueryBatch _futureMultiBatch;

		private bool closed;

		/// <summary>Get the current NHibernate transaction.</summary>
		public ITransaction Transaction => ConnectionManager.Transaction;

		protected bool IsTransactionCoordinatorShared { get; }

		public ITransactionContext TransactionContext
		{
			get; set;
		}

		private bool isAlreadyDisposed;

		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(AbstractSessionImpl));

		public Guid SessionId { get; }

		internal AbstractSessionImpl() { }

		protected internal AbstractSessionImpl(ISessionFactoryImplementor factory, ISessionCreationOptions options)
		{
			SessionId = factory.Settings.TrackSessionId ? Guid.NewGuid() : Guid.Empty;
			using (BeginContext())
			{
				_factory = factory;
				Timestamp = factory.Settings.CacheProvider.NextTimestamp();
				_flushMode = options.InitialSessionFlushMode;
				Interceptor = options.SessionInterceptor ?? EmptyInterceptor.Instance;

				if (options is ISharedSessionCreationOptions sharedOptions && sharedOptions.IsTransactionCoordinatorShared)
				{
					// NH specific implementation: need to port Hibernate transaction management.
					IsTransactionCoordinatorShared = true;
					if (options.UserSuppliedConnection != null)
						throw new SessionException("Cannot simultaneously share transaction context and specify connection");
					var connectionManager = sharedOptions.ConnectionManager;
					connectionManager.AddDependentSession(this);
					ConnectionManager = connectionManager;
				}
				else
				{
					ConnectionManager = new ConnectionManager(
						this,
						options.UserSuppliedConnection,
						options.SessionConnectionReleaseMode,
						Interceptor,
						options.ShouldAutoJoinTransaction);
				}
			}
		}

		#region ISessionImplementor Members

		// Since v5.1
		[Obsolete("This method has no more usages in NHibernate and will be removed.")]
		public void Initialize()
		{
			BeginProcess()?.Dispose();
		}

		public abstract void InitializeCollection(IPersistentCollection collection, bool writing);
		public abstract object InternalLoad(string entityName, object id, bool eager, bool isNullable);
		public abstract object ImmediateLoad(string entityName, object id);

		public EntityKey GenerateEntityKey(object id, IEntityPersister persister)
		{
			return new EntityKey(id, persister);
		}

		public CacheKey GenerateCacheKey(object id, IType type, string entityOrRoleName)
		{
			return new CacheKey(id, type, entityOrRoleName, Factory);
		}

		public ISessionFactoryImplementor Factory
		{
			get => _factory;
			protected set => _factory = value;
		}

		// 6.0 TODO: remove virtual.
		/// <inheritdoc />
		public virtual IBatcher Batcher
		{
			get
			{
				CheckAndUpdateSessionStatus();
				return ConnectionManager.Batcher;
			}
		}
		public abstract void CloseSessionFromSystemTransaction();

		public virtual IList List(IQueryExpression queryExpression, QueryParameters parameters)
		{
			var results = (IList)typeof(List<>)
				.MakeGenericType(queryExpression.Type)
				.GetConstructor(System.Type.EmptyTypes)
				.Invoke(null);
			List(queryExpression, parameters, results);
			return results;
		}

		public abstract void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results);

		public virtual IList<T> List<T>(IQueryExpression query, QueryParameters parameters)
		{
			using (BeginProcess())
			{
				var results = new List<T>();
				List(query, parameters, results);
				return results;
			}
		}

		public virtual IList<T> List<T>(CriteriaImpl criteria)
		{
			using (BeginProcess())
			{
				var results = new List<T>();
				List(criteria, results);
				return results;
			}
		}

		public abstract void List(CriteriaImpl criteria, IList results);

		public virtual IList List(CriteriaImpl criteria)
		{
			using (BeginProcess())
			{
				var results = new List<object>();
				List(criteria, results);
				return results;
			}
		}

		public abstract IList ListFilter(object collection, string filter, QueryParameters parameters);
		public IList ListFilter(object collection, IQueryExpression queryExpression, QueryParameters parameters)
		{
			var results = (IList)typeof(List<>).MakeGenericType(queryExpression.Type)
									.GetConstructor(System.Type.EmptyTypes)
									.Invoke(null);

			ListFilter(collection, queryExpression, parameters, results);
			return results;
		}
		protected abstract void ListFilter(object collection, IQueryExpression queryExpression, QueryParameters parameters, IList results);

		public abstract IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters);
		public abstract IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters);
		public abstract IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters);
		public abstract IEntityPersister GetEntityPersister(string entityName, object obj);
		public abstract void AfterTransactionBegin(ITransaction tx);
		public abstract void BeforeTransactionCompletion(ITransaction tx);
		public abstract void FlushBeforeTransactionCompletion();
		public abstract void AfterTransactionCompletion(bool successful, ITransaction tx);
		public abstract object GetContextEntityIdentifier(object obj);
		public abstract object Instantiate(string clazz, object id);

		public virtual IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			using (BeginProcess())
			{
				var results = new List<object>();
				List(spec, queryParameters, results);
				return results;
			}
		}

		public virtual void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
		{
			using (BeginProcess())
			{
				var query = new SQLCustomQuery(
					spec.SqlQueryReturns,
					spec.QueryString,
					spec.QuerySpaces,
					Factory);
				ListCustomQuery(query, queryParameters, results);
			}
		}

		public virtual IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			using (BeginProcess())
			{
				var results = new List<T>();
				List(spec, queryParameters, results);
				return results;
			}
		}

		public abstract void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results);

		public virtual IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
		{
			using (BeginProcess())
			{
				var results = new List<T>();
				ListCustomQuery(customQuery, queryParameters, results);
				return results;
			}
		}

		public abstract object GetFilterParameterValue(string filterParameterName);
		public abstract IType GetFilterParameterType(string filterParameterName);
		public abstract IDictionary<string, IFilter> EnabledFilters { get; }

		public virtual IQuery GetNamedSQLQuery(string name)
		{
			using (BeginProcess())
			{
				var nsqlqd = _factory.GetNamedSQLQuery(name);
				if (nsqlqd == null)
				{
					throw new MappingException("Named SQL query not known: " + name);
				}
				var query = new SqlQueryImpl(nsqlqd, this,
					_factory.QueryPlanCache.GetSQLParameterMetadata(nsqlqd.QueryString));
				query.SetComment("named native SQL query " + name);
				InitQuery(query, nsqlqd);
				return query;
			}
		}

		// 6.0 TODO: remove virtual from below properties.
		/// <inheritdoc />
		public virtual ConnectionManager ConnectionManager { get; protected set; }
		/// <inheritdoc />
		public virtual bool IsConnected => ConnectionManager.IsConnected;
		/// <inheritdoc />
		public virtual DbConnection Connection => ConnectionManager.GetConnection();

		// Since v5.2
		[Obsolete("This method has no usages and will be removed in a future version")]
		public abstract IQueryTranslator[] GetQueries(IQueryExpression query, bool scalar);
		public abstract EventListeners Listeners { get; }
		public abstract bool IsEventSource { get; }
		public abstract object GetEntityUsingInterceptor(EntityKey key);
		public abstract IPersistenceContext PersistenceContext { get; }
		public abstract CacheMode CacheMode { get; set; }
		public abstract bool IsOpen { get; }
		public abstract string FetchProfile { get; set; }
		public abstract string BestGuessEntityName(object entity);
		public abstract string GuessEntityName(object entity);
		public abstract int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters);

		//Since 5.2
		[Obsolete("Replaced by FutureBatch")]
		public abstract FutureCriteriaBatch FutureCriteriaBatch { get; protected internal set; }
		//Since 5.2
		[Obsolete("Replaced by FutureBatch")]
		public abstract FutureQueryBatch FutureQueryBatch { get; protected internal set; }
	
		public virtual IQueryBatch FutureBatch
			=>_futureMultiBatch ?? (_futureMultiBatch = new QueryBatch(this, true));

		public virtual IInterceptor Interceptor { get; protected set; }

		public virtual FlushMode FlushMode
		{
			get => _flushMode;
			set => _flushMode = value;
		}

		//6.0 TODO: Make abstract
		/// <summary>
		/// detect in-memory changes, determine if the changes are to tables
		/// named in the query and, if so, complete execution the flush
		/// </summary>
		/// <param name="querySpaces"></param>
		/// <returns>Returns true if flush was executed</returns>
		public virtual bool AutoFlushIfRequired(ISet<string> querySpaces)
		{
			return false;
		}

		public virtual IQuery GetNamedQuery(string queryName)
		{
			using (BeginProcess())
			{
				var nqd = _factory.GetNamedQuery(queryName);
				IQuery query;
				if (nqd != null)
				{
					var queryString = nqd.QueryString;
					query = new QueryImpl(queryString, nqd.FlushMode, this, GetHQLQueryPlan(queryString.ToQueryExpression(), false).ParameterMetadata);
					query.SetComment("named HQL query " + queryName);
				}
				else
				{
					var nsqlqd = _factory.GetNamedSQLQuery(queryName);
					if (nsqlqd == null)
					{
						throw new MappingException("Named query not known: " + queryName);
					}
					query = new SqlQueryImpl(nsqlqd, this,
						_factory.QueryPlanCache.GetSQLParameterMetadata(nsqlqd.QueryString));
					query.SetComment("named native SQL query " + queryName);
					nqd = nsqlqd;
				}
				InitQuery(query, nqd);
				return query;
			}
		}

		public virtual long Timestamp { get; protected set; }

		public bool IsClosed
		{
			get { return closed; }
		}

		/// <summary>
		/// If not nested in another call to <c>BeginProcess</c> on this session, check and update the
		/// session status and set its session id in context.
		/// </summary>
		/// <returns>
		/// If not already processing, an object to dispose for signaling the end of the process.
		/// Otherwise, <see langword="null" />.
		/// </returns>
		public IDisposable BeginProcess()
		{
			return _processing ? null : new ProcessHelper(this);
		}

		/// <summary>
		/// If not nested in a call to <c>BeginProcess</c> on this session, set its session id in context.
		/// </summary>
		/// <returns>
		/// If not already processing, an object to dispose for restoring the previous session id.
		/// Otherwise, <see langword="null" />.
		/// </returns>
		public IDisposable BeginContext()
		{
			return _processing ? null : SessionIdLoggingContext.CreateOrNull(SessionId);
		}

		[NonSerialized]
		private bool _processing;

		private sealed class ProcessHelper : IDisposable
		{
			private AbstractSessionImpl _session;
			private IDisposable _context;

			public ProcessHelper(AbstractSessionImpl session)
			{
				_session = session;
				_context = SessionIdLoggingContext.CreateOrNull(session.SessionId);
				try
				{
					_session.CheckAndUpdateSessionStatus();
					_session._processing = true;
				}
				catch
				{
					_context?.Dispose();
					_context = null;
					throw;
				}
			}

			public void Dispose()
			{
				_context?.Dispose();
				_context = null;
				if (_session == null)
					throw new ObjectDisposedException("The session process helper has been disposed already");
				_session._processing = false;
				_session = null;
			}
		}

		protected internal virtual void CheckAndUpdateSessionStatus()
		{
			if (_processing)
				return;

			ErrorIfClosed();

			// Ensure the session does not run on a thread supposed to be blocked, waiting
			// for transaction completion.
			TransactionContext?.Wait();

			EnlistInAmbientTransactionIfNeeded();
		}

		protected virtual void ErrorIfClosed()
		{
			if (IsClosed || IsAlreadyDisposed)
			{
				throw new ObjectDisposedException(nameof(ISession), "Session is closed!");
			}
		}

		protected bool IsAlreadyDisposed
		{
			get { return isAlreadyDisposed; }
			set { isAlreadyDisposed = value; }
		}

		public abstract void Flush();

		// 6.0 TODO: remove virtual.
		/// <inheritdoc />
		public virtual bool TransactionInProgress => ConnectionManager.IsInActiveTransaction;

		#endregion

		protected internal void SetClosed()
		{
			closed = true;
		}

		protected DbConnection CloseConnectionManager()
		{
			if (!IsTransactionCoordinatorShared)
				return ConnectionManager.Close();
			ConnectionManager.RemoveDependentSession(this);
			return null;
		}

		private void InitQuery(IQuery query, NamedQueryDefinition nqd)
		{
			query.SetCacheable(nqd.IsCacheable);
			query.SetCacheRegion(nqd.CacheRegion);
			if (nqd.Timeout != -1)
			{
				query.SetTimeout(nqd.Timeout);
			}
			if (nqd.FetchSize != -1)
			{
				query.SetFetchSize(nqd.FetchSize);
			}
			if (nqd.CacheMode.HasValue)
				query.SetCacheMode(nqd.CacheMode.Value);

			query.SetReadOnly(nqd.IsReadOnly);
			if (nqd.Comment != null)
			{
				query.SetComment(nqd.Comment);
			}
			query.SetFlushMode(nqd.FlushMode);
		}

		public virtual IQuery CreateQuery(IQueryExpression queryExpression)
		{
			using (BeginProcess())
			{
				var queryPlan = GetHQLQueryPlan(queryExpression, false);
				var query = new ExpressionQueryImpl(queryPlan.QueryExpression, this, queryPlan.ParameterMetadata);
				query.SetComment("[expression]");
				return query;
			}
		}

		public virtual IQuery CreateQuery(string queryString)
		{
			using (BeginProcess())
			{
				var queryPlan = GetHQLQueryPlan(queryString.ToQueryExpression(), false);
				var query = new QueryImpl(queryString, this, queryPlan.ParameterMetadata);
				query.SetComment(queryString);
				return query;
			}
		}

		public virtual ISQLQuery CreateSQLQuery(string sql)
		{
			using (BeginProcess())
			{
				var query = new SqlQueryImpl(sql, this, _factory.QueryPlanCache.GetSQLParameterMetadata(sql));
				query.SetComment("dynamic native SQL query");
				return query;
			}
		}

		protected internal virtual IQueryExpressionPlan GetHQLQueryPlan(IQueryExpression queryExpression, bool shallow)
		{
			using (BeginProcess())
			{
				return _factory.QueryPlanCache.GetHQLQueryPlan(queryExpression, shallow, EnabledFilters);
			}
		}

		protected internal virtual NativeSQLQueryPlan GetNativeSQLQueryPlan(NativeSQLQuerySpecification spec)
		{
			using (BeginContext())
			{
				return _factory.QueryPlanCache.GetNativeSQLQueryPlan(spec);
			}
		}

		protected Exception Convert(Exception sqlException, string message)
		{
			using (BeginContext())
			{
				return ADOExceptionHelper.Convert(_factory.SQLExceptionConverter, sqlException, message);
			}
		}

		protected void AfterOperation(bool success)
		{
			using (BeginContext())
			{
				if (!ConnectionManager.IsInActiveTransaction)
				{
					ConnectionManager.AfterNonTransactionalQuery(success);
					ConnectionManager.AfterTransaction();
					AfterTransactionCompletion(success, null);
				}
			}
		}

		/// <summary>
		/// Begin a NHibernate transaction
		/// </summary>
		/// <returns>A NHibernate transaction</returns>
		public ITransaction BeginTransaction()
		{
			using (BeginProcess())
			{
				if (IsTransactionCoordinatorShared)
				{
					// Todo : should seriously consider not allowing a txn to begin from a child session
					//      can always route the request to the root session...
					Log.Warn("Transaction started on non-root session");
				}

				return ConnectionManager.BeginTransaction();
			}
		}

		/// <summary>
		/// Begin a NHibernate transaction with the specified isolation level
		/// </summary>
		/// <param name="isolationLevel">The isolation level</param>
		/// <returns>A NHibernate transaction</returns>
		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			using (BeginProcess())
			{
				if (IsTransactionCoordinatorShared)
				{
					// Todo : should seriously consider not allowing a txn to begin from a child session
					//      can always route the request to the root session...
					Log.Warn("Transaction started on non-root session");
				}

				return ConnectionManager.BeginTransaction(isolationLevel);
			}
		}

		protected void EnlistInAmbientTransactionIfNeeded()
		{
			_factory.TransactionFactory.EnlistInSystemTransactionIfNeeded(this);
		}

		public void JoinTransaction()
		{
			using (BeginProcess())
				_factory.TransactionFactory.ExplicitJoinSystemTransaction(this);
		}

		public abstract IQuery CreateFilter(object collection, IQueryExpression queryExpression);

		internal IOuterJoinLoadable GetOuterJoinLoadable(string entityName)
		{
			using (BeginContext())
			{
				var persister = Factory.GetEntityPersister(entityName) as IOuterJoinLoadable;
				if (persister == null)
				{
					throw new MappingException("class persister is not OuterJoinLoadable: " + entityName);
				}
				return persister;
			}
		}

		public abstract IEnumerable Enumerable(IQueryExpression queryExpression, QueryParameters queryParameters);

		public abstract IEnumerable<T> Enumerable<T>(IQueryExpression queryExpression, QueryParameters queryParameters);

		public abstract int ExecuteUpdate(IQueryExpression queryExpression, QueryParameters queryParameters);
		
		/// <summary>
		/// Creates a new Linq <see cref="IQueryable{T}"/> for the entity class.
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <returns>An <see cref="IQueryable{T}"/> instance</returns>
		public IQueryable<T> Query<T>()
		{
			return new NhQueryable<T>(this);
		}

		/// <summary>
		/// Creates a new Linq <see cref="IQueryable{T}"/> for the entity class and with given entity name.
		/// </summary>
		/// <typeparam name="T">The type of entity to query.</typeparam>
		/// <param name="entityName">The entity name.</param>
		/// <returns>An <see cref="IQueryable{T}"/> instance</returns>
		public IQueryable<T> Query<T>(string entityName)
		{
			return new NhQueryable<T>(this, entityName);
		}

		public virtual IQueryBatch CreateQueryBatch()
		{
			return new QueryBatch(this, false);
		}
	}
}
