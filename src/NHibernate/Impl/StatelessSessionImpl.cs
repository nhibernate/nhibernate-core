using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Loader.Criteria;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	[Serializable]
	public partial class StatelessSessionImpl : AbstractSessionImpl, IStatelessSession
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(StatelessSessionImpl));

		[NonSerialized]
		private readonly StatefulPersistenceContext temporaryPersistenceContext;

		internal StatelessSessionImpl(SessionFactoryImpl factory, ISessionCreationOptions options)
			: base(factory, options)
		{
			// This context is disposed only on session own disposal. This greatly reduces the number of context switches
			// for most usual session usages. It may cause an irrelevant session id to be set back on disposal, but since all
			// session entry points are supposed to set it, it should not have any consequences.
			_context = SessionIdLoggingContext.CreateOrNull(SessionId);
			try
			{
				temporaryPersistenceContext = new StatefulPersistenceContext(this);

				if (log.IsDebugEnabled())
				{
					log.Debug("[session-id={0}] opened session for session factory: [{1}/{2}]",
						SessionId, factory.Name, factory.Uuid);
				}

				CheckAndUpdateSessionStatus();
			}
			catch
			{
				_context?.Dispose();
				throw;
			}
		}

		public override void InitializeCollection(IPersistentCollection collection, bool writing)
		{
			if (temporaryPersistenceContext.IsLoadFinished)
			{
				throw new SessionException("Collections cannot be fetched by a stateless session. You can eager load it through specific query.");
			}
			CollectionEntry ce = temporaryPersistenceContext.GetCollectionEntry(collection);
			if (!collection.WasInitialized)
			{
				ce.LoadedPersister.Initialize(ce.LoadedKey, this);
			}
		}

		public override object InternalLoad(string entityName, object id, bool eager, bool isNullable)
		{
			using (BeginProcess())
			{
				IEntityPersister persister = Factory.GetEntityPersister(entityName);
				object loaded = temporaryPersistenceContext.GetEntity(GenerateEntityKey(id, persister));
				if (loaded != null)
				{
					return loaded;
				}
				if (!eager && persister.HasProxy)
				{
					return persister.CreateProxy(id, this);
				}
				//TODO: if not loaded, throw an exception
				return Get(entityName, id);
			}
		}

		public override object ImmediateLoad(string entityName, object id)
		{
			throw new SessionException("proxies cannot be fetched by a stateless session");
		}

		public override long Timestamp
		{
			get { throw new NotSupportedException(); }
		}

		public override void CloseSessionFromSystemTransaction()
		{
			Dispose(true);
		}

		public override IQuery CreateFilter(object collection, IQueryExpression queryExpression)
		{
			throw new NotSupportedException();
		}

		public override void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
		{
			using (BeginProcess())
			{
				queryParameters.ValidateParameters();
				var plan = GetHQLQueryPlan(queryExpression, false);

				bool success = false;
				try
				{
					plan.PerformList(queryParameters, this, results);
					success = true;
				}
				catch (HibernateException)
				{
					// Do not call Convert on HibernateExceptions
					throw;
				}
				catch (Exception e)
				{
					throw Convert(e, "Could not execute query");
				}
				finally
				{
					AfterOperation(success);
				}
				temporaryPersistenceContext.Clear();
			}
		}

		public override void List(CriteriaImpl criteria, IList results)
		{
			using (BeginProcess())
			{
				string[] implementors = Factory.GetImplementors(criteria.EntityOrClassName);
				int size = implementors.Length;

				CriteriaLoader[] loaders = new CriteriaLoader[size];
				for (int i = 0; i < size; i++)
				{
					loaders[i] = new CriteriaLoader(GetOuterJoinLoadable(implementors[i]), Factory,
													criteria, implementors[i], EnabledFilters);
				}

				bool success = false;
				try
				{
					for (int i = size - 1; i >= 0; i--)
					{
						ArrayHelper.AddAll(results, loaders[i].List(this));
					}
					success = true;
				}
				catch (HibernateException)
				{
					// Do not call Convert on HibernateExceptions
					throw;
				}
				catch (Exception sqle)
				{
					throw Convert(sqle, "Unable to perform find");
				}
				finally
				{
					AfterOperation(success);
				}
				temporaryPersistenceContext.Clear();
			}
		}

		public override IEnumerable Enumerable(IQueryExpression queryExpression, QueryParameters queryParameters)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<T> Enumerable<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
		{
			throw new NotImplementedException();
		}

		public override IList ListFilter(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		protected override void ListFilter(object collection, IQueryExpression queryExpression, QueryParameters parameters, IList results)
		{
			throw new NotSupportedException();
		}

		public override IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override void AfterTransactionBegin(ITransaction tx)
		{
		}

		public override void BeforeTransactionCompletion(ITransaction tx)
		{
			var context = TransactionContext;
			if (tx == null && context == null)
				throw new InvalidOperationException("Cannot complete a transaction without neither an explicit transaction nor an ambient one.");
			// Always allow flushing from explicit transactions, otherwise check if flushing from scope is enabled.
			if (tx != null || context.CanFlushOnSystemTransactionCompleted)
				FlushBeforeTransactionCompletion();
		}

		public override void FlushBeforeTransactionCompletion()
		{
			if (FlushMode != FlushMode.Manual)
				Flush();
		}

		public override void AfterTransactionCompletion(bool successful, ITransaction tx)
		{
		}

		public override object GetContextEntityIdentifier(object obj)
		{
			using (BeginProcess())
			{
				EntityEntry entry = temporaryPersistenceContext.GetEntry(obj);
				return (entry != null) ? entry.Id : null;
			}
		}

		//Since 5.3
		[Obsolete("Use override with persister parameter")]
		public override object Instantiate(string clazz, object id)
		{
		  return Instantiate(Factory.GetEntityPersister(clazz), id);
		}

		public override object Instantiate(IEntityPersister persister, object id)
		{
			using (BeginProcess())
			{
				return persister.Instantiate(id);
			}
		}

		public override void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			using (BeginProcess())
			{
				var loader = new CustomLoader(customQuery, Factory);

				var success = false;
				try
				{
					ArrayHelper.AddAll(results, loader.List(this, queryParameters));
					success = true;
				}
				finally
				{
					AfterOperation(success);
				}
				temporaryPersistenceContext.Clear();
			}
		}

		public override object GetFilterParameterValue(string filterParameterName)
		{
			throw new NotSupportedException();
		}

		public override IType GetFilterParameterType(string filterParameterName)
		{
			throw new NotSupportedException();
		}

		public override IDictionary<string, IFilter> EnabledFilters
		{
			get { return CollectionHelper.EmptyDictionary<string, IFilter>(); }
		}

		// Since v5.2
		[Obsolete("This method has no usages and will be removed in a future version")]
		public override IQueryTranslator[] GetQueries(IQueryExpression query, bool scalar)
		{
			using (BeginContext())
			{
				// take the union of the query spaces (ie the queried tables)
				var plan = Factory.QueryPlanCache.GetHQLQueryPlan(query, scalar, EnabledFilters);
				return plan.Translators;
			}
		}

		public override IInterceptor Interceptor
		{
			get { return EmptyInterceptor.Instance; }
		}

		public override EventListeners Listeners
		{
			get { throw new NotSupportedException(); }
		}

		public override bool IsEventSource
		{
			get { return false; }
		}

		public override object GetEntityUsingInterceptor(EntityKey key)
		{
			using (BeginProcess())
			{
				// while a pending Query we should use existing temporary entities so a join fetch does not create multiple instances
				// of the same parent item (NH-3015, NH-3705).
				object obj;
				if (temporaryPersistenceContext.EntitiesByKey.TryGetValue(key, out obj))
					return obj;

				return null;
			}
		}

		public override IPersistenceContext PersistenceContext
		{
			get { return temporaryPersistenceContext; }
		}

		public override bool IsOpen
		{
			get { return !IsClosed; }
		}

		public override FlushMode FlushMode
		{
			get { return FlushMode.Commit; }
			set { throw new NotSupportedException(); }
		}

		public override string BestGuessEntityName(object entity)
		{
			using (BeginContext())
			{
				if (entity.IsProxy())
				{
					var proxy = entity as INHibernateProxy;
					entity = proxy.HibernateLazyInitializer.GetImplementation();
				}
				return GuessEntityName(entity);
			}
		}

		public override string GuessEntityName(object entity)
		{
			using (BeginContext())
			{
				return entity.GetType().FullName;
			}
		}

		public IStatelessSession SetBatchSize(int batchSize)
		{
			Batcher.BatchSize = batchSize;
			return this;
		}

		public override void Flush()
		{
			ManagedFlush(); // NH Different behavior since ADOContext.Context is not implemented
		}

		public void ManagedFlush()
		{
			using (BeginProcess())
			{
				Batcher.ExecuteBatch();
			}
		}

		#region IStatelessSession Members

		public override CacheMode CacheMode
		{
			get { return CacheMode.Ignore; }
			set { throw new NotSupportedException(); }
		}

		public override string FetchProfile
		{
			get { return null; }
			set { }
		}

		/// <summary>
		/// Gets the stateless session implementation.
		/// </summary>
		/// <remarks>
		/// This method is provided in order to get the <b>NHibernate</b> implementation of the session from wrapper implementations.
		/// Implementors of the <seealso cref="IStatelessSession"/> interface should return the NHibernate implementation of this method.
		/// </remarks>
		/// <returns>
		/// An NHibernate implementation of the <seealso cref="ISessionImplementor"/> interface
		/// </returns>
		public ISessionImplementor GetSessionImplementation()
		{
			return this;
		}

		/// <summary> Close the stateless session and release the ADO.NET connection.</summary>
		public void Close()
		{
			ManagedClose();
		}

		public void ManagedClose()
		{
			using (BeginContext())
			{
				if (IsClosed)
				{
					throw new SessionException("Session was already closed!");
				}
				CloseConnectionManager();
				SetClosed();
			}
		}

		/// <summary> Insert a entity.</summary>
		/// <param name="entity">A new transient instance </param>
		/// <returns> the identifier of the instance </returns>
		public object Insert(object entity)
		{
			return Insert(null, entity);
		}

		/// <summary> Insert a row. </summary>
		/// <param name="entityName">The entityName for the entity to be inserted </param>
		/// <param name="entity">a new transient instance </param>
		/// <returns> the identifier of the instance </returns>
		public object Insert(string entityName, object entity)
		{
			using (BeginProcess())
			{
				IEntityPersister persister = GetEntityPersister(entityName, entity);
				object id = persister.IdentifierGenerator.Generate(this, entity);
				object[] state = persister.GetPropertyValues(entity);
				if (persister.IsVersioned)
				{
					object versionValue = state[persister.VersionProperty];
					bool substitute = Versioning.SeedVersion(state, persister.VersionProperty, persister.VersionType,
															 persister.IsUnsavedVersion(versionValue), this);
					if (substitute)
					{
						persister.SetPropertyValues(entity, state);
					}
				}
				if (id == IdentifierGeneratorFactory.PostInsertIndicator)
				{
					id = persister.Insert(state, entity, this);
				}
				else
				{
					persister.Insert(id, state, entity, this);
				}
				persister.SetIdentifier(entity, id);
				return id;
			}
		}

		/// <summary> Update a entity.</summary>
		/// <param name="entity">a detached entity instance </param>
		public void Update(object entity)
		{
			Update(null, entity);
		}

		/// <summary>Update a entity.</summary>
		/// <param name="entityName">The entityName for the entity to be updated </param>
		/// <param name="entity">a detached entity instance </param>
		public void Update(string entityName, object entity)
		{
			using (BeginProcess())
			{
				IEntityPersister persister = GetEntityPersister(entityName, entity);
				object id = persister.GetIdentifier(entity);
				object[] state = persister.GetPropertyValues(entity);
				object oldVersion;
				if (persister.IsVersioned)
				{
					oldVersion = persister.GetVersion(entity);
					object newVersion = Versioning.Increment(oldVersion, persister.VersionType, this);
					Versioning.SetVersion(state, newVersion, persister);
					persister.SetPropertyValues(entity, state);
				}
				else
				{
					oldVersion = null;
				}
				persister.Update(id, state, null, false, null, oldVersion, entity, null, this);
			}
		}

		/// <summary> Delete a entity. </summary>
		/// <param name="entity">a detached entity instance </param>
		public void Delete(object entity)
		{
			Delete(null, entity);
		}

		/// <summary> Delete a entity. </summary>
		/// <param name="entityName">The entityName for the entity to be deleted </param>
		/// <param name="entity">a detached entity instance </param>
		public void Delete(string entityName, object entity)
		{
			using (BeginProcess())
			{
				IEntityPersister persister = GetEntityPersister(entityName, entity);
				object id = persister.GetIdentifier(entity);
				object version = persister.GetVersion(entity);
				persister.Delete(id, version, entity, this);
			}
		}

		/// <summary> Retrieve a entity. </summary>
		/// <returns> a detached entity instance </returns>
		public object Get(string entityName, object id)
		{
			return Get(entityName, id, LockMode.None);
		}

		/// <summary> Retrieve a entity.
		///
		/// </summary>
		/// <returns> a detached entity instance
		/// </returns>
		public T Get<T>(object id)
		{
			using (BeginProcess())
			{
				return (T)Get(typeof(T), id);
			}
		}

		private object Get(System.Type persistentClass, object id)
		{
			return Get(persistentClass.FullName, id);
		}

		/// <summary>
		/// Retrieve a entity, obtaining the specified lock mode.
		/// </summary>
		/// <returns> a detached entity instance </returns>
		public object Get(string entityName, object id, LockMode lockMode)
		{
			using (BeginProcess())
			{
				object result = Factory.GetEntityPersister(entityName).Load(id, null, lockMode, this);
				if (temporaryPersistenceContext.IsLoadFinished)
				{
					temporaryPersistenceContext.Clear();
				}
				return result;
			}
		}

		/// <summary>
		/// Retrieve a entity, obtaining the specified lock mode.
		/// </summary>
		/// <returns> a detached entity instance </returns>
		public T Get<T>(object id, LockMode lockMode)
		{
			using (BeginProcess())
			{
				return (T)Get(typeof(T).FullName, id, lockMode);
			}
		}

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entity">The entity to be refreshed. </param>
		public void Refresh(object entity)
		{
			using (BeginProcess())
			{
				Refresh(BestGuessEntityName(entity), entity, LockMode.None);
			}
		}

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entityName">The entityName for the entity to be refreshed. </param>
		/// <param name="entity">The entity to be refreshed.</param>
		public void Refresh(string entityName, object entity)
		{
			Refresh(entityName, entity, LockMode.None);
		}

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entity">The entity to be refreshed. </param>
		/// <param name="lockMode">The LockMode to be applied.</param>
		public void Refresh(object entity, LockMode lockMode)
		{
			using (BeginProcess())
			{
				Refresh(BestGuessEntityName(entity), entity, lockMode);
			}
		}

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entityName">The entityName for the entity to be refreshed. </param>
		/// <param name="entity">The entity to be refreshed. </param>
		/// <param name="lockMode">The LockMode to be applied. </param>
		public void Refresh(string entityName, object entity, LockMode lockMode)
		{
			using (BeginProcess())
			{
				IEntityPersister persister = GetEntityPersister(entityName, entity);
				object id = persister.GetIdentifier(entity);
				if (log.IsDebugEnabled())
				{
					log.Debug("refreshing transient {0}", MessageHelper.InfoString(persister, id, Factory));
				}
				//from H3.2 TODO : can this ever happen???
				//		EntityKey key = new EntityKey( id, persister, source.getEntityMode() );
				//		if ( source.getPersistenceContext().getEntry( key ) != null ) {
				//			throw new PersistentObjectException(
				//					"attempted to refresh transient instance when persistent " +
				//					"instance was already associated with the Session: " +
				//					MessageHelper.infoString( persister, id, source.getFactory() )
				//			);
				//		}

				CacheKey ck = this.GetCacheAndKey(id, persister, out var cache);
				cache?.Remove(ck);

				string previousFetchProfile = FetchProfile;
				object result;
				try
				{
					FetchProfile = "refresh";
					result = persister.Load(id, entity, lockMode, this);
				}
				finally
				{
					FetchProfile = previousFetchProfile;
				}
				UnresolvableObjectException.ThrowIfNull(result, id, persister.EntityName);
			}
		}

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class.
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria<T>() where T : class
		{
			return CreateCriteria(typeof(T));
		}

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class, with the given alias.
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <param name="alias">The alias of the entity</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria<T>(string alias) where T : class
		{
			return CreateCriteria(typeof(T), alias);
		}

		public ICriteria CreateCriteria(System.Type entityType)
		{
			using (BeginProcess())
			{
				return new CriteriaImpl(entityType, this);
			}
		}

		public ICriteria CreateCriteria(System.Type entityType, string alias)
		{
			using (BeginProcess())
			{
				return new CriteriaImpl(entityType, alias, this);
			}
		}

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity name.
		/// </summary>
		/// <param name="entityName">The entity name. </param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria(string entityName)
		{
			using (BeginProcess())
			{
				return new CriteriaImpl(entityName, this);
			}
		}

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity name,
		/// with the given alias.
		/// </summary>
		/// <param name="entityName">The entity name. </param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria(string entityName, string alias)
		{
			using (BeginProcess())
			{
				return new CriteriaImpl(entityName, alias, this);
			}
		}

		public IQueryOver<T, T> QueryOver<T>() where T : class
		{
			using (BeginProcess())
			{
				return new QueryOver<T, T>(new CriteriaImpl(typeof(T), this));
			}
		}

		public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class
		{
			using (BeginProcess())
			{
				string aliasPath = ExpressionProcessor.FindMemberExpression(alias.Body);
				return new QueryOver<T, T>(new CriteriaImpl(typeof(T), aliasPath, this));
			}
		}

		#endregion

		#region IDisposable Members

		private bool _isAlreadyDisposed;
		private IDisposable _context;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~StatelessSessionImpl()
		{
			Dispose(false);
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			using (BeginContext())
			{
				log.Debug("running IStatelessSession.Dispose()");
				// Ensure we are not disposing concurrently to transaction completion, which would
				// remove the context. (Do not store it into a local variable before the Wait.)
				TransactionContext?.Wait();
				// If the synchronization above is bugged and lets a race condition remaining, we may
				// blow here with a null ref exception after the null check. We could introduce
				// a local variable for avoiding it, but that would turn a failure causing an exception
				// into a failure causing a session and connection leak. So do not do it, better blow away
				// with a null ref rather than silently leaking a session. And then fix the synchronization.
				if (TransactionContext != null && TransactionContext.CanFlushOnSystemTransactionCompleted)
				{
					TransactionContext.ShouldCloseSessionOnSystemTransactionCompleted = true;
					return;
				}
				Dispose(true);
			}
		}

		protected void Dispose(bool isDisposing)
		{
			using (BeginContext())
			{
				if (_isAlreadyDisposed)
				{
					// don't dispose of multiple times.
					return;
				}

				// free managed resources that are being managed by the session if we
				// know this call came through Dispose()
				if (isDisposing)
				{
					if (!IsClosed)
					{
						Close();
					}

					// nothing for Finalizer to do - so tell the GC to ignore it
					GC.SuppressFinalize(this);
				}

				// free unmanaged resources here
				_isAlreadyDisposed = true;
			}
			_context?.Dispose();
		}

		#endregion

		public override int ExecuteNativeUpdate(NativeSQLQuerySpecification nativeSQLQuerySpecification, QueryParameters queryParameters)
		{
			using (BeginProcess())
			{
				queryParameters.ValidateParameters();
				NativeSQLQueryPlan plan = GetNativeSQLQueryPlan(nativeSQLQuerySpecification);

				bool success = false;
				int result;
				try
				{
					result = plan.PerformExecuteUpdate(queryParameters, this);
					success = true;
				}
				finally
				{
					AfterOperation(success);
				}
				temporaryPersistenceContext.Clear();
				return result;
			}
		}

		public override int ExecuteUpdate(IQueryExpression queryExpression, QueryParameters queryParameters)
		{
			using (BeginProcess())
			{
				queryParameters.ValidateParameters();
				var plan = GetHQLQueryPlan(queryExpression, false);
				bool success = false;
				int result;
				try
				{
					result = plan.PerformExecuteUpdate(queryParameters, this);
					success = true;
				}
				finally
				{
					AfterOperation(success);
				}
				temporaryPersistenceContext.Clear();
				return result;
			}
		}

		//Since 5.2
		[Obsolete("Replaced by QueryBatch")]
		public override FutureCriteriaBatch FutureCriteriaBatch
		{
			get { throw new NotSupportedException("future queries are not supported for stateless session"); }
			protected internal set { throw new NotSupportedException("future queries are not supported for stateless session"); }
		}

		//Since 5.2
		[Obsolete("Replaced by QueryBatch")]
		public override FutureQueryBatch FutureQueryBatch
		{
			get { throw new NotSupportedException("future queries are not supported for stateless session"); }
			protected internal set { throw new NotSupportedException("future queries are not supported for stateless session"); }
		}

		public override IEntityPersister GetEntityPersister(string entityName, object obj)
		{
			using (BeginProcess())
			{
				if (entityName == null)
				{
					return Factory.GetEntityPersister(GuessEntityName(obj));
				}
				else
				{
					return Factory.GetEntityPersister(entityName).GetSubclassEntityPersister(obj, Factory);
				}
			}
		}
	}
}
