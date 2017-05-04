using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.AdoNet;
using NHibernate.Collection;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Intercept;
using NHibernate.Loader.Criteria;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Stat;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	/// <summary>
	/// Concrete implementation of an <see cref="ISession" />, also the central, organizing component
	/// of NHibernate's internal implementation.
	/// </summary>
	/// <remarks>
	/// Exposes two interfaces: <see cref="ISession" /> itself, to the application and 
	/// <see cref="ISessionImplementor" /> to other components of NHibernate. This is where the 
	/// hard stuff is... This class is NOT THREADSAFE.
	/// </remarks>
	[Serializable]
	public sealed partial class SessionImpl : AbstractSessionImpl, IEventSource, ISerializable, IDeserializationCallback
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(SessionImpl));

		private CacheMode cacheMode = CacheMode.Normal;

		[NonSerialized]
		private FutureCriteriaBatch futureCriteriaBatch;
		[NonSerialized]
		private FutureQueryBatch futureQueryBatch;

		[NonSerialized]
		private readonly EventListeners listeners;

		[NonSerialized]
		private readonly ActionQueue actionQueue;

		private readonly ConnectionManager connectionManager;

		[NonSerialized]
		private int dontFlushFromFind;

		[NonSerialized]
		private readonly IDictionary<string, IFilter> enabledFilters = new Dictionary<string, IFilter>();

		[NonSerialized]
		private readonly List<string> enabledFilterNames = new List<string>();

		[NonSerialized]
		private readonly StatefulPersistenceContext persistenceContext;

		[NonSerialized]
		private readonly bool autoCloseSessionEnabled;
		[NonSerialized]
		private readonly ConnectionReleaseMode connectionReleaseMode;
		[NonSerialized]
		private readonly bool _transactionCoordinatorShared;

		#region System.Runtime.Serialization.ISerializable Members

		/// <summary>
		/// Constructor used to recreate the Session during the deserialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <remarks>
		/// This is needed because we have to do some checking before the serialization process
		/// begins.  I don't know how to add logic in ISerializable.GetObjectData and have .net
		/// write all of the serializable fields out.
		/// </remarks>
		private SessionImpl(SerializationInfo info, StreamingContext context)
		{
			Timestamp = info.GetInt64("timestamp");
			SessionFactoryImpl fact = (SessionFactoryImpl)info.GetValue("factory", typeof(SessionFactoryImpl));
			Factory = fact;
			listeners = fact.EventListeners;
			persistenceContext = (StatefulPersistenceContext)info.GetValue("persistenceContext", typeof(StatefulPersistenceContext));

			actionQueue = (ActionQueue)info.GetValue("actionQueue", typeof(ActionQueue));

			FlushMode = (FlushMode)info.GetValue("flushMode", typeof(FlushMode));
			cacheMode = (CacheMode)info.GetValue("cacheMode", typeof(CacheMode));

			Interceptor = (IInterceptor)info.GetValue("interceptor", typeof(IInterceptor));

			enabledFilters = (IDictionary<string, IFilter>)info.GetValue("enabledFilters", typeof(Dictionary<string, IFilter>));
			enabledFilterNames = (List<string>)info.GetValue("enabledFilterNames", typeof(List<string>));

			connectionManager = (ConnectionManager)info.GetValue("connectionManager", typeof(ConnectionManager));
		}

		/// <summary>
		/// Verify the ISession can be serialized and write the fields to the Serializer.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <remarks>
		/// The fields are marked with [NonSerializable] as just a point of reference.  This method
		/// has complete control and what is serialized and those attributes are ignored.  However,
		/// this method should be in sync with the attributes for easy readability.
		/// </remarks>
		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			log.Debug("writting session to serializer");

			if (!connectionManager.IsReadyForSerialization)
			{
				throw new InvalidOperationException("Cannot serialize a Session while connected");
			}
			if (_transactionCoordinatorShared)
			{
				throw new InvalidOperationException("Cannot serialize a Session sharing its transaction coordinator");
			}

			info.AddValue("factory", Factory, typeof(SessionFactoryImpl));
			info.AddValue("persistenceContext", persistenceContext, typeof(StatefulPersistenceContext));
			info.AddValue("actionQueue", actionQueue, typeof(ActionQueue));
			info.AddValue("timestamp", Timestamp);
			info.AddValue("flushMode", FlushMode);
			info.AddValue("cacheMode", cacheMode);

			info.AddValue("interceptor", Interceptor, typeof(IInterceptor));

			info.AddValue("enabledFilters", enabledFilters, typeof(IDictionary<string, IFilter>));
			info.AddValue("enabledFilterNames", enabledFilterNames, typeof(List<string>));

			info.AddValue("connectionManager", connectionManager, typeof(ConnectionManager));
		}

		#endregion

		#region System.Runtime.Serialization.IDeserializationCallback Members

		/// <summary>
		/// Once the entire object graph has been deserialized then we can hook the
		/// collections, proxies, and entities back up to the ISession.
		/// </summary>
		/// <param name="sender"></param>
		void IDeserializationCallback.OnDeserialization(object sender)
		{
			log.Debug("OnDeserialization of the session.");

			persistenceContext.SetSession(this);

			// OnDeserialization() must be called manually on all Dictionaries and Hashtables,
			// otherwise they are still empty at this point (the .NET deserialization code calls
			// OnDeserialization() on them AFTER it calls the current method).
			((IDeserializationCallback)enabledFilters).OnDeserialization(sender);

			foreach (string filterName in enabledFilterNames)
			{
				FilterImpl filter = (FilterImpl)enabledFilters[filterName];
				filter.AfterDeserialize(Factory.GetFilterDefinition(filterName));
			}
		}

		#endregion

		/// <summary>
		/// Constructor used for OpenSession(...) processing, as well as construction
		/// of sessions for GetCurrentSession().
		/// </summary>
		/// <param name="factory">The factory from which this session was obtained.</param>
		/// <param name="options">The options of the session.</param>
		internal SessionImpl(SessionFactoryImpl factory, ISessionCreationOptions options)
			: base(factory, options)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				actionQueue = new ActionQueue(this);
				persistenceContext = new StatefulPersistenceContext(this);

				autoCloseSessionEnabled = options.ShouldAutoClose;

				listeners = factory.EventListeners;
				connectionReleaseMode = options.SessionConnectionReleaseMode;

				if (options is ISharedSessionCreationOptions sharedOptions && sharedOptions.IsTransactionCoordinatorShared)
				{
					// NH specific implementation: need to port Hibernate transaction management.
					_transactionCoordinatorShared = true;
					if (options.UserSuppliedConnection != null)
						throw new SessionException("Cannot simultaneously share transaction context and specify connection");
					connectionManager = sharedOptions.ConnectionManager;
				}
				else
				{
					connectionManager = new ConnectionManager(this, options.UserSuppliedConnection, connectionReleaseMode, Interceptor);
				}

				if (factory.Statistics.IsStatisticsEnabled)
				{
					factory.StatisticsImplementor.OpenSession();
				}

				log.DebugFormat("[session-id={0}] opened session at timestamp: {1}, for session factory: [{2}/{3}]",
					SessionId, Timestamp, factory.Name, factory.Uuid);

				CheckAndUpdateSessionStatus();
			}
		}

		public override FutureCriteriaBatch FutureCriteriaBatch
		{
			get
			{
				if (futureCriteriaBatch == null)
					futureCriteriaBatch = new FutureCriteriaBatch(this);
				return futureCriteriaBatch;
			}
			protected internal set
			{
				futureCriteriaBatch = value;
			}
		}

		public override FutureQueryBatch FutureQueryBatch
		{
			get
			{
				if (futureQueryBatch == null)
					futureQueryBatch = new FutureQueryBatch(this);
				return futureQueryBatch;
			}
			protected internal set
			{
				futureQueryBatch = value;
			}
		}

		/// <summary></summary>
		public override IBatcher Batcher
		{
			get
			{
				CheckAndUpdateSessionStatus();
				return connectionManager.Batcher;
			}
		}

		public ConnectionReleaseMode ConnectionReleaseMode
		{
			get { return connectionReleaseMode; }
		}

		public bool IsAutoCloseSessionEnabled
		{
			get { return autoCloseSessionEnabled; }
		}

		public bool ShouldAutoClose
		{
			get { return IsAutoCloseSessionEnabled && !IsClosed; }
		}

		/// <summary>
		/// Close the session and release all resources
		/// <remarks>
		/// Do not call this method inside a transaction scope, use <c>Dispose</c> instead, since
		/// Close() is not aware of distributed transactions
		/// </remarks>
		/// </summary>
		public DbConnection Close()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				log.Debug("closing session");
				if (IsClosed)
				{
					throw new SessionException("Session was already closed");
				}

				if (Factory.Statistics.IsStatisticsEnabled)
				{
					Factory.StatisticsImplementor.CloseSession();
				}

				try
				{
					if (!_transactionCoordinatorShared)
						return connectionManager.Close();
					else
						return null;
				}
				finally
				{
					SetClosed();
					Cleanup();
				}
			}
		}

		/// <summary>
		/// Ensure that the locks are downgraded to <see cref="LockMode.None"/>
		/// and that all of the softlocks in the <see cref="Cache"/> have
		/// been released.
		/// </summary>
		public override void AfterTransactionCompletion(bool success, ITransaction tx)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				log.Debug("transaction completion");
				if (Factory.Statistics.IsStatisticsEnabled)
				{
					Factory.StatisticsImplementor.EndTransaction(success);
				}

				connectionManager.AfterTransaction();
				persistenceContext.AfterTransactionCompletion();
				actionQueue.AfterTransactionCompletion(success);
				if (!_transactionCoordinatorShared)
				{
					try
					{
						Interceptor.AfterTransactionCompletion(tx);
					}
					catch (Exception t)
					{
						log.Error("exception in interceptor afterTransactionCompletion()", t);
					}
				}


				//if (autoClear)
				//	Clear();
			}
		}

		private void Cleanup()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				persistenceContext.Clear();
			}
		}

		public LockMode GetCurrentLockMode(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				if (obj == null)
				{
					throw new ArgumentNullException("obj", "null object passed to GetCurrentLockMode");
				}

				if (obj.IsProxy())
				{
					var proxy = obj as INHibernateProxy;
					obj = proxy.HibernateLazyInitializer.GetImplementation(this);
					if (obj == null)
					{
						return LockMode.None;
					}
				}

				EntityEntry e = persistenceContext.GetEntry(obj);
				if (e == null)
				{
					throw new TransientObjectException("Given object not associated with the session");
				}

				if (e.Status != Status.Loaded)
				{
					throw new ObjectDeletedException("The given object was deleted", e.Id, e.EntityName);
				}
				return e.LockMode;
			}
		}

		public override bool IsOpen
		{
			get { return !IsClosed; }
		}

		/// <summary>
		/// Save a transient object. An id is generated, assigned to the object and returned
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Save(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return FireSave(new SaveOrUpdateEvent(null, obj, this));
			}
		}

		public object Save(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return FireSave(new SaveOrUpdateEvent(entityName, obj, this));
			}
		}

		public void Save(string entityName, object obj, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireSave(new SaveOrUpdateEvent(entityName, obj, id, this));
			}
		}

		/// <summary>
		/// Save a transient object with a manually assigned ID
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Save(object obj, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireSave(new SaveOrUpdateEvent(null, obj, id, this));
			}
		}

		/// <summary>
		/// Delete a persistent object
		/// </summary>
		/// <param name="obj"></param>
		public void Delete(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireDelete(new DeleteEvent(obj, this));
			}
		}

		/// <summary> Delete a persistent object (by explicit entity name)</summary>
		public void Delete(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireDelete(new DeleteEvent(entityName, obj, this));
			}
		}

		public void Update(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireUpdate(new SaveOrUpdateEvent(null, obj, this));
			}
		}

		public void Update(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireUpdate(new SaveOrUpdateEvent(entityName, obj, this));
			}
		}

		public void Update(string entityName, object obj, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireUpdate(new SaveOrUpdateEvent(entityName, obj, id, this));
			}
		}

		public void SaveOrUpdate(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireSaveOrUpdate(new SaveOrUpdateEvent(null, obj, this));
			}
		}

		public void SaveOrUpdate(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireSaveOrUpdate(new SaveOrUpdateEvent(entityName, obj, this));
			}
		}

		public void SaveOrUpdate(string entityName, object obj, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireSaveOrUpdate(new SaveOrUpdateEvent(entityName, obj, id, this));
			}
		}

		public void Update(object obj, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireUpdate(new SaveOrUpdateEvent(null, obj, id, this));
			}
		}

		private static readonly object[] NoArgs = new object[0];
		private static readonly IType[] NoTypes = new IType[0];

		IList Find(string query, object[] values, IType[] types)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return List(query.ToQueryExpression(), new QueryParameters(types, values));
			}
		}

		public override void CloseSessionFromDistributedTransaction()
		{
			Dispose(true);
		}

		public override void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				queryParameters.ValidateParameters();
				var plan = GetHQLQueryPlan(queryExpression, false);
				AutoFlushIfRequired(plan.QuerySpaces);

				bool success = false;
				dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called
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
					dontFlushFromFind--;
					AfterOperation(success);
				}
			}
		}

		public override IQueryTranslator[] GetQueries(IQueryExpression query, bool scalar)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				var plan = Factory.QueryPlanCache.GetHQLQueryPlan(query, scalar, enabledFilters);
				AutoFlushIfRequired(plan.QuerySpaces);
				return plan.Translators;
			}
		}

		public override IEnumerable<T> Enumerable<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				queryParameters.ValidateParameters();
				var plan = GetHQLQueryPlan(queryExpression, true);
				AutoFlushIfRequired(plan.QuerySpaces);

				dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called
				try
				{
					return plan.PerformIterate<T>(queryParameters, this);
				}
				finally
				{
					dontFlushFromFind--;
				}
			}
		}

		public override IEnumerable Enumerable(IQueryExpression queryExpression, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				queryParameters.ValidateParameters();
				var plan = GetHQLQueryPlan(queryExpression, true);
				AutoFlushIfRequired(plan.QuerySpaces);

				dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called
				try
				{
					return plan.PerformIterate(queryParameters, this);
				}
				finally
				{
					dontFlushFromFind--;
				}
			}
		}

		// TODO: Scroll(string query, QueryParameters queryParameters)

		public int Delete(string query)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Delete(query, NoArgs, NoTypes);
			}
		}

		public int Delete(string query, object value, IType type)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Delete(query, new[] { value }, new[] { type });
			}
		}

		public int Delete(string query, object[] values, IType[] types)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (string.IsNullOrEmpty(query))
				{
					throw new ArgumentNullException("query", "attempt to perform delete-by-query with null query");
				}

				CheckAndUpdateSessionStatus();

				if (log.IsDebugEnabled)
				{
					log.Debug("delete: " + query);
					if (values.Length != 0)
					{
						log.Debug("parameters: " + StringHelper.ToString(values));
					}
				}

				IList list = Find(query, values, types);
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					Delete(list[i]);
				}
				return count;
			}
		}

		public void Lock(object obj, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireLock(new LockEvent(obj, lockMode, this));
			}
		}

		public void Lock(string entityName, object obj, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireLock(new LockEvent(entityName, obj, lockMode, this));
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public IQuery CreateFilter(object collection, string queryString)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				CollectionFilterImpl filter =
					new CollectionFilterImpl(queryString, collection, this,
											 GetFilterQueryPlan(collection, queryString, null, false).ParameterMetadata);
				//filter.SetComment(queryString);
				return filter;
			}
		}

		private FilterQueryPlan GetFilterQueryPlan(object collection, string filter, QueryParameters parameters, bool shallow)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (collection == null)
				{
					throw new ArgumentNullException("collection", "null collection passed to filter");
				}

				CollectionEntry entry = persistenceContext.GetCollectionEntryOrNull(collection);
				ICollectionPersister roleBeforeFlush = (entry == null) ? null : entry.LoadedPersister;

				FilterQueryPlan plan;
				if (roleBeforeFlush == null)
				{
					// if it was previously unreferenced, we need to flush in order to
					// get its state into the database in order to execute query
					Flush();
					entry = persistenceContext.GetCollectionEntryOrNull(collection);
					ICollectionPersister roleAfterFlush = (entry == null) ? null : entry.LoadedPersister;
					if (roleAfterFlush == null)
					{
						throw new QueryException("The collection was unreferenced");
					}
					plan = Factory.QueryPlanCache.GetFilterQueryPlan(filter, roleAfterFlush.Role, shallow, EnabledFilters);
				}
				else
				{
					// otherwise, we only need to flush if there are in-memory changes
					// to the queried tables
					plan = Factory.QueryPlanCache.GetFilterQueryPlan(filter, roleBeforeFlush.Role, shallow, EnabledFilters);
					if (AutoFlushIfRequired(plan.QuerySpaces))
					{
						// might need to run a different filter entirely after the flush
						// because the collection role may have changed
						entry = persistenceContext.GetCollectionEntryOrNull(collection);
						ICollectionPersister roleAfterFlush = (entry == null) ? null : entry.LoadedPersister;
						if (roleBeforeFlush != roleAfterFlush)
						{
							if (roleAfterFlush == null)
							{
								throw new QueryException("The collection was dereferenced");
							}
							plan = Factory.QueryPlanCache.GetFilterQueryPlan(filter, roleAfterFlush.Role, shallow, EnabledFilters);
						}
					}
				}

				if (parameters != null)
				{
					parameters.PositionalParameterValues[0] = entry.LoadedKey;
					parameters.PositionalParameterTypes[0] = entry.LoadedPersister.KeyType;
				}

				return plan;
			}
		}

		public override object Instantiate(string clazz, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Instantiate(Factory.GetEntityPersister(clazz), id);
			}
		}

		/// <summary> Get the ActionQueue for this session</summary>
		public ActionQueue ActionQueue
		{
			get
			{
				CheckAndUpdateSessionStatus();
				return actionQueue;
			}
		}

		/// <summary>
		/// Give the interceptor an opportunity to override the default instantiation
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object Instantiate(IEntityPersister persister, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				ErrorIfClosed();
				object result = Interceptor.Instantiate(persister.EntityName, id);
				if (result == null)
				{
					result = persister.Instantiate(id);
				}
				return result;
			}
		}

		#region IEventSource Members
		/// <summary> Force an immediate flush</summary>
		public void ForceFlush(EntityEntry entityEntry)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				if (log.IsDebugEnabled)
				{
					log.Debug("flushing to force deletion of re-saved object: " +
							  MessageHelper.InfoString(entityEntry.Persister, entityEntry.Id, Factory));
				}

				if (persistenceContext.CascadeLevel > 0)
				{
					throw new ObjectDeletedException(
						"deleted object would be re-saved by cascade (remove deleted object from associations)",
						entityEntry.Id,
						entityEntry.EntityName);
				}

				Flush();
			}
		}

		/// <summary> Cascade merge an entity instance</summary>
		public void Merge(string entityName, object obj, IDictionary copiedAlready)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireMerge(copiedAlready, new MergeEvent(entityName, obj, this));
			}
		}

		/// <summary> Cascade persist an entity instance</summary>
		public void Persist(string entityName, object obj, IDictionary createdAlready)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FirePersist(createdAlready, new PersistEvent(entityName, obj, this));
			}
		}

		/// <summary> Cascade persist an entity instance during the flush process</summary>
		public void PersistOnFlush(string entityName, object obj, IDictionary copiedAlready)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FirePersistOnFlush(copiedAlready, new PersistEvent(entityName, obj, this));
			}
		}

		/// <summary> Cascade refresh an entity instance</summary>
		public void Refresh(object obj, IDictionary refreshedAlready)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireRefresh(refreshedAlready, new RefreshEvent(obj, this));
			}
		}

		/// <summary> Cascade delete an entity instance</summary>
		public void Delete(string entityName, object child, bool isCascadeDeleteEnabled, ISet<object> transientEntities)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireDelete(new DeleteEvent(entityName, child, isCascadeDeleteEnabled, this), transientEntities);
			}
		}

		#endregion

		public object Merge(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return FireMerge(new MergeEvent(entityName, obj, this));
			}
		}

		public T Merge<T>(T entity) where T : class
		{
			return (T)Merge((object)entity);
		}

		public T Merge<T>(string entityName, T entity) where T : class
		{
			return (T)Merge(entityName, (object)entity);
		}

		public object Merge(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Merge(null, obj);
			}
		}

		public void Persist(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FirePersist(new PersistEvent(entityName, obj, this));
			}
		}

		public void Persist(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				Persist(null, obj);
			}
		}

		public void PersistOnFlush(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FirePersistOnFlush(new PersistEvent(entityName, obj, this));
			}
		}

		public void PersistOnFlush(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				Persist(null, obj);
			}
		}

		// Obsolete in v5, and was already having no usages previously.
		[Obsolete("Please use FlushMode instead.")]
		public bool FlushBeforeCompletionEnabled => FlushMode >= FlushMode.Commit;

		public override string BestGuessEntityName(object entity)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (entity.IsProxy())
				{
					INHibernateProxy proxy = entity as INHibernateProxy;
					ILazyInitializer initializer = proxy.HibernateLazyInitializer;

					// it is possible for this method to be called during flush processing,
					// so make certain that we do not accidentally initialize an uninitialized proxy
					if (initializer.IsUninitialized)
					{
						return initializer.PersistentClass.FullName;
					}
					entity = initializer.GetImplementation();
				}
				if (FieldInterceptionHelper.IsInstrumented(entity))
				{
					// NH: support of field-interceptor-proxy
					IFieldInterceptor interceptor = FieldInterceptionHelper.ExtractFieldInterceptor(entity);
					return interceptor.EntityName;
				}
				EntityEntry entry = persistenceContext.GetEntry(entity);
				if (entry == null)
				{
					return GuessEntityName(entity);
				}
				else
				{
					return entry.Persister.EntityName;
				}
			}
		}

		public override string GuessEntityName(object entity)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				string entityName = Interceptor.GetEntityName(entity);
				if (entityName == null)
				{
					System.Type t = entity.GetType();
					entityName = Factory.TryGetGuessEntityName(t) ?? t.FullName;
				}
				return entityName;
			}
		}

		public override bool IsEventSource
		{
			get
			{
				return true;
			}
		}

		public override object GetEntityUsingInterceptor(EntityKey key)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				// todo : should this get moved to PersistentContext?
				// logically, is PersistentContext the "thing" to which an interceptor gets attached?
				object result = persistenceContext.GetEntity(key);

				if (result == null)
				{
					object newObject = Interceptor.GetEntity(key.EntityName, key.Identifier);
					if (newObject != null)
					{
						Lock(newObject, LockMode.None);
					}
					return newObject;
				}
				else
				{
					return result;
				}
			}
		}

		public override IPersistenceContext PersistenceContext
		{
			get
			{
				CheckAndUpdateSessionStatus();
				return persistenceContext;
			}
		}

		/// <summary>
		/// detect in-memory changes, determine if the changes are to tables
		/// named in the query and, if so, complete execution the flush
		/// </summary>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
		private bool AutoFlushIfRequired(ISet<string> querySpaces)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				if (!ConnectionManager.IsInActiveTransaction)
				{
					// do not auto-flush while outside a transaction
					return false;
				}
				AutoFlushEvent autoFlushEvent = new AutoFlushEvent(querySpaces, this);
				IAutoFlushEventListener[] autoFlushEventListener = listeners.AutoFlushEventListeners;
				for (int i = 0; i < autoFlushEventListener.Length; i++)
				{
					autoFlushEventListener[i].OnAutoFlush(autoFlushEvent);
				}
				return autoFlushEvent.FlushRequired;
			}
		}

		#region load()/get() operations

		public void Load(object obj, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				LoadEvent loadEvent = new LoadEvent(id, obj, this);
				FireLoad(loadEvent, LoadEventListener.Reload);
			}
		}

		public T Load<T>(object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return (T)Load(typeof(T), id);
			}
		}

		public T Load<T>(object id, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return (T)Load(typeof(T), id, lockMode);
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This should return an existing proxy where appropriate.
		///
		/// If the object does not exist in the database, an exception is thrown.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		/// <exception cref="ObjectNotFoundException">
		/// Thrown when the object with the specified id does not exist in the database.
		/// </exception>
		public object Load(System.Type entityClass, object id, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Load(entityClass.FullName, id, lockMode);
			}
		}

		public object Load(string entityName, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (id == null)
				{
					throw new ArgumentNullException("id", "null is not a valid identifier");
				}

				var @event = new LoadEvent(id, entityName, false, this);
				bool success = false;
				try
				{
					FireLoad(@event, LoadEventListener.Load);
					if (@event.Result == null)
					{
						Factory.EntityNotFoundDelegate.HandleEntityNotFound(entityName, id);
					}
					success = true;
					return @event.Result;
				}
				finally
				{
					AfterOperation(success);
				}
			}
		}

		public object Load(string entityName, object id, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				var @event = new LoadEvent(id, entityName, lockMode, this);
				FireLoad(@event, LoadEventListener.Load);
				return @event.Result;
			}
		}

		public object Load(System.Type entityClass, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Load(entityClass.FullName, id);
			}
		}

		public T Get<T>(object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return (T)Get(typeof(T), id);
			}
		}

		public T Get<T>(object id, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return (T)Get(typeof(T), id, lockMode);
			}
		}

		public object Get(System.Type entityClass, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Get(entityClass.FullName, id);
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This should return an existing proxy where appropriate.
		///
		/// If the object does not exist in the database, null is returned.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		public object Get(System.Type clazz, object id, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				LoadEvent loadEvent = new LoadEvent(id, clazz.FullName, lockMode, this);
				FireLoad(loadEvent, LoadEventListener.Get);
				return loadEvent.Result;
			}
		}

		public string GetEntityName(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				if (obj.IsProxy())
				{
					var proxy = obj as INHibernateProxy;

					if (!persistenceContext.ContainsProxy(proxy))
					{
						throw new TransientObjectException("proxy was not associated with the session");
					}
					ILazyInitializer li = proxy.HibernateLazyInitializer;

					obj = li.GetImplementation();
				}

				EntityEntry entry = persistenceContext.GetEntry(obj);
				if (entry == null)
				{
					throw new TransientObjectException(
						"object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave: "
						+ obj.GetType().FullName);
				}
				return entry.Persister.EntityName;
			}
		}

		public object Get(string entityName, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				LoadEvent loadEvent = new LoadEvent(id, entityName, false, this);
				bool success = false;
				try
				{
					FireLoad(loadEvent, LoadEventListener.Get);
					success = true;
					return loadEvent.Result;
				}
				finally
				{
					AfterOperation(success);
				}
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object.
		/// This is only called when lazily initializing a proxy.
		/// Do NOT return a proxy.
		/// </summary>
		public override object ImmediateLoad(string entityName, object id)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (log.IsDebugEnabled)
				{
					IEntityPersister persister = Factory.GetEntityPersister(entityName);
					log.Debug("initializing proxy: " + MessageHelper.InfoString(persister, id, Factory));
				}

				LoadEvent loadEvent = new LoadEvent(id, entityName, true, this);
				FireLoad(loadEvent, LoadEventListener.ImmediateLoad);
				return loadEvent.Result;
			}
		}


		/// <summary>
		/// Return the object with the specified id or throw exception if no row with that id exists. Defer the load,
		/// return a new proxy or return an existing proxy if possible. Do not check if the object was deleted.
		/// </summary>
		public override object InternalLoad(string entityName, object id, bool eager, bool isNullable)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				// todo : remove
				LoadType type = isNullable
									? LoadEventListener.InternalLoadNullable
									: (eager ? LoadEventListener.InternalLoadEager : LoadEventListener.InternalLoadLazy);
				LoadEvent loadEvent = new LoadEvent(id, entityName, true, this);
				FireLoad(loadEvent, type);
				if (!isNullable)
				{
					UnresolvableObjectException.ThrowIfNull(loadEvent.Result, id, entityName);
				}
				return loadEvent.Result;
			}
		}

		#endregion

		public void Refresh(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireRefresh(new RefreshEvent(obj, this));
			}
		}

		public void Refresh(object obj, LockMode lockMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireRefresh(new RefreshEvent(obj, lockMode, this));
			}
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (_transactionCoordinatorShared)
				{
					// Todo : should seriously consider not allowing a txn to begin from a child session
					//      can always route the request to the root session...
					log.Warn("Transaction started on non-root session");
				}

				CheckAndUpdateSessionStatus();
				return connectionManager.BeginTransaction(isolationLevel);
			}
		}

		public ITransaction BeginTransaction()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (_transactionCoordinatorShared)
				{
					// Todo : should seriously consider not allowing a txn to begin from a child session
					//      can always route the request to the root session...
					log.Warn("Transaction started on non-root session");
				}

				CheckAndUpdateSessionStatus();
				return connectionManager.BeginTransaction();
			}
		}

		public ITransaction Transaction
		{
			get { return connectionManager.Transaction; }
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>
		/// This can be called from commit() or at the start of a List() method.
		/// <para>
		/// Perform all the necessary SQL statements in a sensible order, to allow
		/// users to respect foreign key constraints:
		/// <list type="">
		///		<item>Inserts, in the order they were performed</item>
		///		<item>Updates</item>
		///		<item>Deletion of collection elements</item>
		///		<item>Insertion of collection elements</item>
		///		<item>Deletes, in the order they were performed</item>
		/// </list>
		/// </para>
		/// <para>
		/// Go through all the persistent objects and look for collections they might be
		/// holding. If they had a nonpersistable collection, substitute a persistable one
		/// </para>
		/// </remarks>
		public override void Flush()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				if (persistenceContext.CascadeLevel > 0)
				{
					throw new HibernateException("Flush during cascade is dangerous");
				}
				IFlushEventListener[] flushEventListener = listeners.FlushEventListeners;
				for (int i = 0; i < flushEventListener.Length; i++)
				{
					flushEventListener[i].OnFlush(new FlushEvent(this));
				}
			}
		}

		public override bool TransactionInProgress
		{
			get { return ConnectionManager.IsInActiveTransaction; }
		}

		public bool IsDirty()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				log.Debug("checking session dirtiness");
				if (actionQueue.AreInsertionsOrDeletionsQueued)
				{
					log.Debug("session dirty (scheduled updates and insertions)");
					return true;
				}
				else
				{
					DirtyCheckEvent dcEvent = new DirtyCheckEvent(this);
					IDirtyCheckEventListener[] dirtyCheckEventListener = listeners.DirtyCheckEventListeners;
					for (int i = 0; i < dirtyCheckEventListener.Length; i++)
					{
						dirtyCheckEventListener[i].OnDirtyCheck(dcEvent);
					}
					return dcEvent.Dirty;
				}
			}
		}

		/// <summary>
		/// Not for internal use
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetIdentifier(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				// Actually the case for proxies will probably work even with
				// the session closed, but do the check here anyway, so that
				// the behavior is uniform.

				if (obj.IsProxy())
				{
					var proxy = obj as INHibernateProxy;

					ILazyInitializer li = proxy.HibernateLazyInitializer;
					if (li.Session != this)
					{
						throw new TransientObjectException("The proxy was not associated with this session");
					}
					return li.Identifier;
				}

				EntityEntry entry = persistenceContext.GetEntry(obj);
				if (entry == null)
				{
					throw new TransientObjectException("the instance was not associated with this session");
				}
				return entry.Id;
			}
		}

		/// <summary>
		/// Get the id value for an object that is actually associated with the session.
		/// This is a bit stricter than GetEntityIdentifierIfNotUnsaved().
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override object GetContextEntityIdentifier(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (obj.IsProxy())
				{
					INHibernateProxy proxy = obj as INHibernateProxy;

					return proxy.HibernateLazyInitializer.Identifier;
				}
				else
				{
					EntityEntry entry = persistenceContext.GetEntry(obj);
					return (entry != null) ? entry.Id : null;
				}
			}
		}

		internal ICollectionPersister GetCollectionPersister(string role)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return Factory.GetCollectionPersister(role);
			}
		}

		/// <summary>
		/// called by a collection that wants to initialize itself
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="writing"></param>
		public override void InitializeCollection(IPersistentCollection collection, bool writing)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IInitializeCollectionEventListener[] listener = listeners.InitializeCollectionEventListeners;
				for (int i = 0; i < listener.Length; i++)
				{
					listener[i].OnInitializeCollection(new InitializeCollectionEvent(collection, this));
				}
			}
		}

		public override DbConnection Connection
		{
			get { return connectionManager.GetConnection(); }
		}

		/// <summary>
		/// Gets if the ISession is connected.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the ISession is connected.
		/// </value>
		/// <remarks>
		/// An ISession is considered connected if there is an <see cref="DbConnection"/> (regardless
		/// of its state) or if it the field <c>connect</c> is true.  Meaning that it will connect
		/// at the next operation that requires a connection.
		/// </remarks>
		public override bool IsConnected
		{
			get { return connectionManager.IsConnected; }
		}

		/// <summary></summary>
		public DbConnection Disconnect()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				log.Debug("disconnecting session");
				return connectionManager.Disconnect();
			}
		}

		public void Reconnect()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				log.Debug("reconnecting session");
				connectionManager.Reconnect();
			}
		}

		public void Reconnect(DbConnection conn)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				log.Debug("reconnecting session");
				connectionManager.Reconnect(conn);
			}
		}

		#region System.IDisposable Members

		private string fetchProfile;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~SessionImpl()
		{
			Dispose(false);
		}

		/// <summary>
		/// Perform a soft (distributed transaction aware) close of the session
		/// </summary>
		public void Dispose()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				log.Debug(string.Format("[session-id={0}] running ISession.Dispose()", SessionId));
				if (TransactionContext != null)
				{
					TransactionContext.ShouldCloseSessionOnDistributedTransactionCompleted = true;
					return;
				}
				Dispose(true);
			}
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this Session is being Disposed of or Finalized.</param>
		/// <remarks>
		/// If this Session is being Finalized (<c>isDisposing==false</c>) then make sure not
		/// to call any methods that could potentially bring this Session back to life.
		/// </remarks>
		private void Dispose(bool isDisposing)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				if (IsAlreadyDisposed)
				{
					// don't dispose of multiple times.
					return;
				}

				log.Debug(string.Format("[session-id={0}] executing real Dispose({1})", SessionId, isDisposing));

				// free managed resources that are being managed by the session if we
				// know this call came through Dispose()
				if (isDisposing && !IsClosed)
				{
					Close();
				}

				// free unmanaged resources here

				IsAlreadyDisposed = true;

				// nothing for Finalizer to do - so tell the GC to ignore it
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		private void Filter(object collection, string filter, QueryParameters queryParameters, IList results)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				FilterQueryPlan plan = GetFilterQueryPlan(collection, filter, queryParameters, false);

				bool success = false;
				dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called
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
					throw Convert(e, "could not execute query");
				}
				finally
				{
					dontFlushFromFind--;
					AfterOperation(success);
				}
			}
		}

		public override IList ListFilter(object collection, string filter, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				var results = new List<object>();
				Filter(collection, filter, queryParameters, results);
				return results;
			}
		}

		public override IList<T> ListFilter<T>(object collection, string filter, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				List<T> results = new List<T>();
				Filter(collection, filter, queryParameters, results);
				return results;
			}
		}

		public override IEnumerable EnumerableFilter(object collection, string filter, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				FilterQueryPlan plan = GetFilterQueryPlan(collection, filter, queryParameters, true);
				return plan.PerformIterate(queryParameters, this);
			}
		}

		public override IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				FilterQueryPlan plan = GetFilterQueryPlan(collection, filter, queryParameters, true);
				return plan.PerformIterate<T>(queryParameters, this);
			}
		}

		public ICriteria CreateCriteria<T>() where T : class
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return CreateCriteria(typeof(T));
			}
		}

		public ICriteria CreateCriteria(System.Type persistentClass)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				return new CriteriaImpl(persistentClass, this);
			}
		}

		public ICriteria CreateCriteria<T>(string alias) where T : class
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return CreateCriteria(typeof(T), alias);
			}
		}

		public ICriteria CreateCriteria(System.Type persistentClass, string alias)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				return new CriteriaImpl(persistentClass, alias, this);
			}
		}

		public ICriteria CreateCriteria(string entityName, string alias)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				return new CriteriaImpl(entityName, alias, this);
			}
		}

		public ICriteria CreateCriteria(string entityName)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				return new CriteriaImpl(entityName, this);
			}
		}

		public IQueryOver<T, T> QueryOver<T>() where T : class
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				return new QueryOver<T, T>(new CriteriaImpl(typeof(T), this));
			}
		}

		public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				string aliasPath = ExpressionProcessor.FindMemberExpression(alias.Body);
				return new QueryOver<T, T>(new CriteriaImpl(typeof(T), aliasPath, this));
			}
		}

		public IQueryOver<T, T> QueryOver<T>(string entityName) where T : class
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				return new QueryOver<T, T>(new CriteriaImpl(entityName, this));
			}
		}

		public IQueryOver<T, T> QueryOver<T>(string entityName, Expression<Func<T>> alias) where T : class
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				string aliasPath = ExpressionProcessor.FindMemberExpression(alias.Body);
				return new QueryOver<T, T>(new CriteriaImpl(entityName, aliasPath, this));
			}
		}

		public override void List(CriteriaImpl criteria, IList results)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				string[] implementors = Factory.GetImplementors(criteria.EntityOrClassName);
				int size = implementors.Length;

				CriteriaLoader[] loaders = new CriteriaLoader[size];
				ISet<string> spaces = new HashSet<string>();

				for (int i = 0; i < size; i++)
				{
					loaders[i] = new CriteriaLoader(
						GetOuterJoinLoadable(implementors[i]),
						Factory,
						criteria,
						implementors[i],
						enabledFilters
						);

					spaces.UnionWith(loaders[i].QuerySpaces);
				}

				AutoFlushIfRequired(spaces);

				dontFlushFromFind++;

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
					dontFlushFromFind--;
					AfterOperation(success);
				}
			}
		}

		public bool Contains(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				if (obj.IsProxy())
				{
					var proxy = obj as INHibernateProxy;

					//do not use proxiesByKey, since not all
					//proxies that point to this session's
					//instances are in that collection!
					ILazyInitializer li = proxy.HibernateLazyInitializer;
					if (li.IsUninitialized)
					{
						//if it is an uninitialized proxy, pointing
						//with this session, then when it is accessed,
						//the underlying instance will be "contained"
						return li.Session == this;
					}
					else
					{
						//if it is initialized, see if the underlying
						//instance is contained, since we need to
						//account for the fact that it might have been
						//evicted
						obj = li.GetImplementation();
					}
				}
				// A session is considered to contain an entity only if the entity has
				// an entry in the session's persistence context and the entry reports
				// that the entity has not been removed
				EntityEntry entry = persistenceContext.GetEntry(obj);
				return entry != null && entry.Status != Status.Deleted && entry.Status != Status.Gone;
			}
		}

		/// <summary>
		/// remove any hard references to the entity that are held by the infrastructure
		/// (references held by application or other persistant instances are okay)
		/// </summary>
		/// <param name="obj"></param>
		public void Evict(object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireEvict(new EvictEvent(obj, this));
			}
		}

		public override ISQLQuery CreateSQLQuery(string sql)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				return base.CreateSQLQuery(sql);
			}
		}

		public override void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				CustomLoader loader = new CustomLoader(customQuery, Factory);
				AutoFlushIfRequired(loader.QuerySpaces);

				bool success = false;
				dontFlushFromFind++;
				try
				{
					ArrayHelper.AddAll(results, loader.List(this, queryParameters));
					success = true;
				}
				finally
				{
					dontFlushFromFind--;
					AfterOperation(success);
				}
			}
		}

		public ISharedSessionBuilder SessionWithOptions()
		{
			return new SharedSessionBuilderImpl(this);
		}

		public void Clear()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				actionQueue.Clear();
				persistenceContext.Clear();
			}
		}

		public void Replicate(object obj, ReplicationMode replicationMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireReplicate(new ReplicateEvent(obj, replicationMode, this));
			}
		}

		public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				FireReplicate(new ReplicateEvent(entityName, obj, replicationMode, this));
			}
		}

		public ISessionFactory SessionFactory
		{
			get { return Factory; }
		}

		public void CancelQuery()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();

				Batcher.CancelLastQuery();
			}
		}

		public IFilter GetEnabledFilter(string filterName)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IFilter result;
				enabledFilters.TryGetValue(filterName, out result);
				return result;
			}
		}

		public IFilter EnableFilter(string filterName)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				FilterImpl filter = new FilterImpl(Factory.GetFilterDefinition(filterName));
				enabledFilters[filterName] = filter;
				if (!enabledFilterNames.Contains(filterName))
				{
					enabledFilterNames.Add(filterName);
				}
				return filter;
			}
		}

		public void DisableFilter(string filterName)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				enabledFilters.Remove(filterName);
				enabledFilterNames.Remove(filterName);
			}
		}

		public override Object GetFilterParameterValue(string filterParameterName)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				string[] parsed = ParseFilterParameterName(filterParameterName);
				IFilter ifilter;
				enabledFilters.TryGetValue(parsed[0], out ifilter);
				FilterImpl filter = ifilter as FilterImpl;
				if (filter == null)
				{
					throw new ArgumentException("Filter [" + parsed[0] + "] currently not enabled");
				}
				return filter.GetParameter(parsed[1]);
			}
		}

		public override IType GetFilterParameterType(string filterParameterName)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				string[] parsed = ParseFilterParameterName(filterParameterName);
				FilterDefinition filterDef = Factory.GetFilterDefinition(parsed[0]);
				if (filterDef == null)
				{
					throw new ArgumentNullException(parsed[0], "Filter [" + parsed[0] + "] not defined");
				}
				IType type = filterDef.GetParameterType(parsed[1]);
				if (type == null)
				{
					// this is an internal error of some sort...
					throw new ArgumentNullException(parsed[1], "Unable to locate type for filter parameter");
				}
				return type;
			}
		}

		public override IDictionary<string, IFilter> EnabledFilters
		{
			get
			{
				CheckAndUpdateSessionStatus();

				foreach (IFilter filter in enabledFilters.Values)
				{
					filter.Validate();
				}

				return enabledFilters;
			}
		}

		private string[] ParseFilterParameterName(string filterParameterName)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				int dot = filterParameterName.IndexOf(".");
				if (dot <= 0)
				{
					throw new ArgumentException("Invalid filter-parameter name format", "filterParameterName");
				}
				string filterName = filterParameterName.Substring(0, dot);
				string parameterName = filterParameterName.Substring(dot + 1);
				return new[] { filterName, parameterName };
			}
		}

		public override ConnectionManager ConnectionManager
		{
			get { return connectionManager; }
		}

		public IMultiQuery CreateMultiQuery()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return new MultiQueryImpl(this);
			}
		}

		public IMultiCriteria CreateMultiCriteria()
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				return new MultiCriteriaImpl(this, Factory);
			}
		}

		/// <summary> Get the statistics for this session.</summary>
		public ISessionStatistics Statistics
		{
			get
			{
				return new SessionStatisticsImpl(this);
			}
		}

		public override void AfterTransactionBegin(ITransaction tx)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				Interceptor.AfterTransactionBegin(tx);
			}
		}

		public override void BeforeTransactionCompletion(ITransaction tx)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				log.Debug("before transaction completion");
				actionQueue.BeforeTransactionCompletion();
				if (!_transactionCoordinatorShared)
				{
					try
					{
						Interceptor.BeforeTransactionCompletion(tx);
					}
					catch (Exception e)
					{
						log.Error("exception in interceptor BeforeTransactionCompletion()", e);

						throw;
					}
				}
			}
		}

		public ISession SetBatchSize(int batchSize)
		{
			Batcher.BatchSize = batchSize;
			return this;
		}

		public ISessionImplementor GetSessionImplementation()
		{
			return this;
		}

		// Obsolete since v5
		[Obsolete("Please use SessionWithOptions instead.")]
		public ISession GetSession(EntityMode entityMode)
		{
			return SessionWithOptions()
				.Connection()
				.ConnectionReleaseMode()
				.FlushMode()
				.Interceptor()
				.OpenSession();
		}

		/// <summary> Retrieves the configured event listeners from this event source. </summary>
		public override EventListeners Listeners
		{
			get { return listeners; }
		}

		public override int DontFlushFromFind
		{
			get { return dontFlushFromFind; }
		}

		public override CacheMode CacheMode
		{
			get { return cacheMode; }
			set
			{
				CheckAndUpdateSessionStatus();
				if (log.IsDebugEnabled)
				{
					log.Debug("setting cache mode to: " + value);
				}
				cacheMode = value;
			}
		}

		public override string FetchProfile
		{
			get { return fetchProfile; }
			set
			{
				CheckAndUpdateSessionStatus();
				fetchProfile = value;
			}
		}

		/// <inheritdoc />
		public void SetReadOnly(object entityOrProxy, bool readOnly)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				persistenceContext.SetReadOnly(entityOrProxy, readOnly);
			}
		}

		/// <inheritdoc />
		public bool DefaultReadOnly
		{
			get { return persistenceContext.DefaultReadOnly; }
			set { persistenceContext.DefaultReadOnly = value; }
		}

		/// <inheritdoc />
		public bool IsReadOnly(object entityOrProxy)
		{
			ErrorIfClosed();
			// CheckTransactionSynchStatus();
			return persistenceContext.IsReadOnly(entityOrProxy);
		}

		private void FireDelete(DeleteEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IDeleteEventListener[] deleteEventListener = listeners.DeleteEventListeners;
				for (int i = 0; i < deleteEventListener.Length; i++)
				{
					deleteEventListener[i].OnDelete(@event);
				}
			}
		}

		private void FireDelete(DeleteEvent @event, ISet<object> transientEntities)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IDeleteEventListener[] deleteEventListener = listeners.DeleteEventListeners;
				for (int i = 0; i < deleteEventListener.Length; i++)
				{
					deleteEventListener[i].OnDelete(@event, transientEntities);
				}
			}
		}

		private void FireEvict(EvictEvent evictEvent)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IEvictEventListener[] evictEventListener = listeners.EvictEventListeners;
				for (int i = 0; i < evictEventListener.Length; i++)
				{
					evictEventListener[i].OnEvict(evictEvent);
				}
			}
		}

		private void FireLoad(LoadEvent @event, LoadType loadType)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				ILoadEventListener[] loadEventListener = listeners.LoadEventListeners;
				for (int i = 0; i < loadEventListener.Length; i++)
				{
					loadEventListener[i].OnLoad(@event, loadType);
				}
			}
		}

		private void FireLock(LockEvent lockEvent)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				ILockEventListener[] lockEventListener = listeners.LockEventListeners;
				for (int i = 0; i < lockEventListener.Length; i++)
				{
					lockEventListener[i].OnLock(lockEvent);
				}
			}
		}

		private object FireMerge(MergeEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IMergeEventListener[] mergeEventListener = listeners.MergeEventListeners;
				for (int i = 0; i < mergeEventListener.Length; i++)
				{
					mergeEventListener[i].OnMerge(@event);
				}
				return @event.Result;
			}
		}

		private void FireMerge(IDictionary copiedAlready, MergeEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IMergeEventListener[] mergeEventListener = listeners.MergeEventListeners;
				for (int i = 0; i < mergeEventListener.Length; i++)
				{
					mergeEventListener[i].OnMerge(@event, copiedAlready);
				}
			}
		}

		private void FirePersist(IDictionary copiedAlready, PersistEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IPersistEventListener[] persistEventListener = listeners.PersistEventListeners;
				for (int i = 0; i < persistEventListener.Length; i++)
				{
					persistEventListener[i].OnPersist(@event, copiedAlready);
				}
			}
		}

		private void FirePersist(PersistEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IPersistEventListener[] createEventListener = listeners.PersistEventListeners;
				for (int i = 0; i < createEventListener.Length; i++)
				{
					createEventListener[i].OnPersist(@event);
				}
			}
		}

		private void FirePersistOnFlush(IDictionary copiedAlready, PersistEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IPersistEventListener[] persistEventListener = listeners.PersistOnFlushEventListeners;
				for (int i = 0; i < persistEventListener.Length; i++)
				{
					persistEventListener[i].OnPersist(@event, copiedAlready);
				}
			}
		}

		private void FirePersistOnFlush(PersistEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IPersistEventListener[] createEventListener = listeners.PersistOnFlushEventListeners;
				for (int i = 0; i < createEventListener.Length; i++)
				{
					createEventListener[i].OnPersist(@event);
				}
			}
		}

		private void FireRefresh(RefreshEvent refreshEvent)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IRefreshEventListener[] refreshEventListener = listeners.RefreshEventListeners;
				for (int i = 0; i < refreshEventListener.Length; i++)
				{
					refreshEventListener[i].OnRefresh(refreshEvent);
				}
			}
		}

		private void FireRefresh(IDictionary refreshedAlready, RefreshEvent refreshEvent)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IRefreshEventListener[] refreshEventListener = listeners.RefreshEventListeners;
				for (int i = 0; i < refreshEventListener.Length; i++)
				{
					refreshEventListener[i].OnRefresh(refreshEvent, refreshedAlready);
				}
			}
		}

		private void FireReplicate(ReplicateEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				IReplicateEventListener[] replicateEventListener = listeners.ReplicateEventListeners;
				for (int i = 0; i < replicateEventListener.Length; i++)
				{
					replicateEventListener[i].OnReplicate(@event);
				}
			}
		}

		private object FireSave(SaveOrUpdateEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				ISaveOrUpdateEventListener[] saveEventListener = listeners.SaveEventListeners;
				for (int i = 0; i < saveEventListener.Length; i++)
				{
					saveEventListener[i].OnSaveOrUpdate(@event);
				}
				return @event.ResultId;
			}
		}

		private void FireSaveOrUpdate(SaveOrUpdateEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				ISaveOrUpdateEventListener[] saveOrUpdateEventListener = listeners.SaveOrUpdateEventListeners;
				for (int i = 0; i < saveOrUpdateEventListener.Length; i++)
				{
					saveOrUpdateEventListener[i].OnSaveOrUpdate(@event);
				}
			}
		}

		private void FireUpdate(SaveOrUpdateEvent @event)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				ISaveOrUpdateEventListener[] updateEventListener = listeners.UpdateEventListeners;
				for (int i = 0; i < updateEventListener.Length; i++)
				{
					updateEventListener[i].OnSaveOrUpdate(@event);
				}
			}
		}

		public override int ExecuteNativeUpdate(NativeSQLQuerySpecification nativeQuerySpecification, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				queryParameters.ValidateParameters();
				NativeSQLQueryPlan plan = GetNativeSQLQueryPlan(nativeQuerySpecification);

				AutoFlushIfRequired(plan.CustomQuery.QuerySpaces);

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
				return result;
			}
		}

		public override int ExecuteUpdate(IQueryExpression queryExpression, QueryParameters queryParameters)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				queryParameters.ValidateParameters();
				var plan = GetHQLQueryPlan(queryExpression, false);
				AutoFlushIfRequired(plan.QuerySpaces);

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
				return result;
			}
		}

		public override IEntityPersister GetEntityPersister(string entityName, object obj)
		{
			using (new SessionIdLoggingContext(SessionId))
			{
				CheckAndUpdateSessionStatus();
				if (entityName == null)
				{
					return Factory.GetEntityPersister(GuessEntityName(obj));
				}
				else
				{
					// try block is a hack around fact that currently tuplizers are not
					// given the opportunity to resolve a subclass entity name.  this
					// allows the (we assume custom) interceptor the ability to
					// influence this decision if we were not able to based on the
					// given entityName
					try
					{
						return Factory.GetEntityPersister(entityName).GetSubclassEntityPersister(obj, Factory);
					}
					catch (HibernateException)
					{
						try
						{
							return GetEntityPersister(null, obj);
						}
						catch (HibernateException)
						{
							// we ignore this exception and re-throw the
							// original one
						}
						throw;
					}
				}
			}
		}

		// NH different implementation: will not try to support covariant return type for specializations
		// of SharedSessionBuilderImpl until they need to exist.
		private class SharedSessionBuilderImpl : SessionFactoryImpl.SessionBuilderImpl<ISharedSessionBuilder>,
			ISharedSessionBuilder, ISharedSessionCreationOptions
		{
			private readonly SessionImpl _session;
			private bool _shareTransactionContext;

			public SharedSessionBuilderImpl(SessionImpl session)
				: base((SessionFactoryImpl)session.Factory)
			{
				_session = session;
				SetSelf(this);
			}

			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// SharedSessionBuilder

			public virtual ISharedSessionBuilder Interceptor() => Interceptor(_session.Interceptor);

			public virtual ISharedSessionBuilder Connection()
			{
				// Ensure any previously user supplied connection is removed.
				base.Connection(null);
				// We share the connection manager
				_shareTransactionContext = true; 
				return this;
			}

			public virtual ISharedSessionBuilder ConnectionReleaseMode() => ConnectionReleaseMode(_session.ConnectionReleaseMode);

			public virtual ISharedSessionBuilder FlushMode() => FlushMode(_session.FlushMode);

			public virtual ISharedSessionBuilder AutoClose() => AutoClose(_session.autoCloseSessionEnabled);

			// NH different implementation, avoid an error case.
			public override ISharedSessionBuilder Connection(DbConnection connection)
			{
				_shareTransactionContext = false;
				return base.Connection(connection);
			}

			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// SharedSessionCreationOptions

			public virtual bool IsTransactionCoordinatorShared => _shareTransactionContext;

			// NH different implementation: need to port Hibernate transaction management.
			public ConnectionManager ConnectionManager => _shareTransactionContext ? _session.ConnectionManager : null;
		}
	}
}
