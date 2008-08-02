using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Iesi.Collections;
using log4net;
using NHibernate.AdoNet;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Loader.Criteria;
using NHibernate.Loader.Custom;
using NHibernate.Loader.Custom.Sql;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Stat;
using NHibernate.Type;
using NHibernate.Util;
using Iesi.Collections.Generic;

namespace NHibernate.Impl
{
	/// <summary>
	/// Concrete implementation of a Session, also the central, organizing component
	/// of Hibernate's internal implementation.
	/// </summary>
	/// <remarks>
	/// Exposes two interfaces: ISession itself, to the application and ISessionImplementor
	/// to other components of hibernate. This is where the hard stuff is...
	/// NOT THREADSAFE
	/// </remarks>
	[Serializable]
	public sealed class SessionImpl : AbstractSessionImpl, IEventSource, ISerializable, IDeserializationCallback
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SessionImpl));

		private readonly long timestamp;

		private CacheMode cacheMode= CacheMode.Normal;
		private FlushMode flushMode = FlushMode.Auto;

		private readonly IInterceptor interceptor;

		[NonSerialized] 
		private EntityMode entityMode = NHibernate.EntityMode.Poco;

		[NonSerialized]
		private readonly EventListeners listeners;

		[NonSerialized]
		private readonly ActionQueue actionQueue;

		private readonly ConnectionManager connectionManager;

		[NonSerialized]
		private int dontFlushFromFind = 0;

		[NonSerialized]
		private IDictionary<string, IFilter> enabledFilters = new Dictionary<string, IFilter>();

		[NonSerialized]
		private readonly StatefulPersistenceContext persistenceContext;
		
		[NonSerialized]
		private ISession rootSession;

		[NonSerialized]
		private IDictionary<EntityMode,ISession> childSessionsByEntityMode;


		//[NonSerialized] private bool flushBeforeCompletionEnabled;
		[NonSerialized] private bool autoCloseSessionEnabled;
		//[NonSerialized] private ConnectionReleaseMode connectionReleaseMode;

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
			timestamp = info.GetInt64("timestamp");

			factory = (SessionFactoryImpl)info.GetValue("factory", typeof(SessionFactoryImpl));
			listeners = factory.EventListeners;
			persistenceContext = (StatefulPersistenceContext)info.GetValue("persistenceContext", typeof(StatefulPersistenceContext));

			actionQueue = (ActionQueue)info.GetValue("actionQueue", typeof(ActionQueue));

			flushMode = (FlushMode)info.GetValue("flushMode", typeof(FlushMode));
			cacheMode = (CacheMode)info.GetValue("cacheMode", typeof(CacheMode));

			interceptor = (IInterceptor)info.GetValue("interceptor", typeof(IInterceptor));

			enabledFilters = (IDictionary<string, IFilter>)info.GetValue("enabledFilters", typeof(Dictionary<string, IFilter>));

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
		/// this method should be in synch with the attributes for easy readability.
		/// </remarks>
		[SecurityPermission(SecurityAction.LinkDemand,
			Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			log.Debug("writting session to serializer");

			if (!connectionManager.IsReadyForSerialization)
			{
				throw new InvalidOperationException("Cannot serialize a Session while connected");
			}

			info.AddValue("factory", factory, typeof(SessionFactoryImpl));
			info.AddValue("persistenceContext", persistenceContext, typeof (StatefulPersistenceContext));
			info.AddValue("actionQueue", actionQueue, typeof(ActionQueue));
			info.AddValue("timestamp", timestamp);
			info.AddValue("flushMode", flushMode);
			info.AddValue("cacheMode", cacheMode);

			info.AddValue("interceptor", interceptor, typeof(IInterceptor));

			info.AddValue("enabledFilters", enabledFilters, typeof(IDictionary<string, IFilter>));

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

			// don't need any section for IdentityMaps because .net does not have a problem
			// serializing them like java does.
			persistenceContext.SetSession(this);
			foreach (FilterImpl filter in enabledFilters.Values)
			{
				filter.AfterDeserialize(factory.GetFilterDefinition(filter.Name));
			}
		}

		#endregion

		internal SessionImpl(
			IDbConnection connection,
			SessionFactoryImpl factory,
			long timestamp,
			IInterceptor interceptor,
			ConnectionReleaseMode connectionReleaseMode):base(factory)
		{
			if (interceptor == null)
				throw new ArgumentNullException("interceptor", "The interceptor can not be null");

			connectionManager = new ConnectionManager(this, connection, connectionReleaseMode, interceptor);
			this.interceptor = interceptor;
			this.timestamp = timestamp;
			listeners = factory.EventListeners;
			actionQueue = new ActionQueue(this);
			persistenceContext = new StatefulPersistenceContext(this);

			if (factory.Statistics.IsStatisticsEnabled)
			{
				factory.StatisticsImplementor.OpenSession();
			}

			log.Debug("opened session");
		}

		/// <summary>
		/// Constructor used for OpenSession(...) processing, as well as construction
		/// of sessions for GetCurrentSession().
		/// </summary>
		/// <param name="connection">The user-supplied connection to use for this session.</param>
		/// <param name="factory">The factory from which this session was obtained</param>
		/// <param name="autoclose">NOT USED</param>
		/// <param name="timestamp">The timestamp for this session</param>
		/// <param name="interceptor">The interceptor to be applied to this session</param>
		/// <param name="entityMode">The entity-mode for this session</param>
		/// <param name="flushBeforeCompletionEnabled">Should we auto flush before completion of transaction</param>
		/// <param name="autoCloseSessionEnabled">Should we auto close after completion of transaction</param>
		/// <param name="connectionReleaseMode">The mode by which we should release JDBC connections.</param>
		public SessionImpl(
			IDbConnection connection,
			SessionFactoryImpl factory,
			bool autoclose,
			long timestamp,
			IInterceptor interceptor,
			EntityMode entityMode,
			bool flushBeforeCompletionEnabled,
			bool autoCloseSessionEnabled,
			ConnectionReleaseMode connectionReleaseMode)
			: base(factory)
		{
			rootSession = null;
			this.timestamp = timestamp;
			this.entityMode = entityMode;
			this.interceptor = interceptor;
			listeners = factory.EventListeners;
			actionQueue = new ActionQueue(this);
			persistenceContext = new StatefulPersistenceContext(this);
			//this.flushBeforeCompletionEnabled = flushBeforeCompletionEnabled;
			this.autoCloseSessionEnabled = autoCloseSessionEnabled;
			//this.connectionReleaseMode = connectionReleaseMode;
			//this.jdbcContext = new JDBCContext(this, connection, interceptor);
			connectionManager = new ConnectionManager(this, connection, connectionReleaseMode, interceptor);
		}

		/// <summary>
		/// Constructor used in building "child sessions".
		/// </summary>
		/// <param name="parent">The parent Session</param>
		/// <param name="entityMode">The entity mode</param>
		private SessionImpl(SessionImpl parent, EntityMode entityMode)
			:base (parent.factory)
		{
			rootSession = parent;
			timestamp = parent.timestamp;
			connectionManager = parent.connectionManager; //this.jdbcContext = parent.jdbcContext;
			interceptor = parent.interceptor;
			listeners = parent.listeners;
			actionQueue = new ActionQueue(this);
			this.entityMode = entityMode;
			persistenceContext = new StatefulPersistenceContext(this);
			//this.flushBeforeCompletionEnabled = false;
			autoCloseSessionEnabled = false;
			//this.connectionReleaseMode = null;

			if (factory.Statistics.IsStatisticsEnabled)
				factory.StatisticsImplementor.OpenSession();

			log.Debug("opened session [" + entityMode + "]");
		}

		/// <summary></summary>
		public override IBatcher Batcher
		{
			get
			{
				ErrorIfClosed();
				return connectionManager.Batcher;
			}
		}

		/// <summary></summary>
		public override long Timestamp
		{
			get { return timestamp; }
		}

		public bool IsAutoCloseSessionEnabled
		{
			get { return autoCloseSessionEnabled; }
		}

		public bool ShouldAutoClose
		{
			get { return IsAutoCloseSessionEnabled && !IsClosed; }
		}

		/// <summary></summary>
		public IDbConnection Close()
		{
			log.Debug("closing session");
			if (IsClosed)
			{
				throw new SessionException("Session was already closed");
			}

			if (factory.Statistics.IsStatisticsEnabled)
			{
				factory.StatisticsImplementor.CloseSession();
			}

			try
			{
				try
				{
					if (childSessionsByEntityMode != null)
					{
						foreach (KeyValuePair<EntityMode, ISession> pair in childSessionsByEntityMode)
						{
							pair.Value.Close();
						}
					}
				}
				catch {}

				if (rootSession == null)
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

		/// <summary>
		/// Ensure that the locks are downgraded to <see cref="LockMode.None"/>
		/// and that all of the softlocks in the <see cref="Cache"/> have
		/// been released.
		/// </summary>
		public override void AfterTransactionCompletion(bool success, ITransaction tx)
		{
			log.Debug("transaction completion");
			if (Factory.Statistics.IsStatisticsEnabled)
			{
				Factory.StatisticsImplementor.EndTransaction(success);
			}

			connectionManager.AfterTransaction();
			persistenceContext.AfterTransactionCompletion();
			actionQueue.AfterTransactionCompletion(success);
			if (rootSession == null && tx != null)
			{
				try
				{
					interceptor.AfterTransactionCompletion(tx);
				}
				catch (Exception t)
				{
					log.Error("exception in interceptor afterTransactionCompletion()", t);
				}
			}


			//if (autoClear)
			//	Clear();
		}

		private void Cleanup()
		{
			persistenceContext.Clear();
		}

		public LockMode GetCurrentLockMode(object obj)
		{
			ErrorIfClosed();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "null object passed to GetCurrentLockMode");
			}
			if (obj is INHibernateProxy)
			{
				obj = ((INHibernateProxy)obj).HibernateLazyInitializer.GetImplementation(this);
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
			return FireSave(new SaveOrUpdateEvent(null, obj, this));
		}

		public object Save(string entityName, object obj)
		{
			return FireSave(new SaveOrUpdateEvent(entityName, obj, this));
		}

		/// <summary>
		/// Save a transient object with a manually assigned ID
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Save(object obj, object id)
		{
			FireSave(new SaveOrUpdateEvent(null, obj, id, this));
		}

		/// <summary>
		/// Delete a persistent object
		/// </summary>
		/// <param name="obj"></param>
		public void Delete(object obj)
		{
			FireDelete(new DeleteEvent(obj, this));
		}

		/// <summary> Delete a persistent object (by explicit entity name)</summary>
		public void Delete(string entityName, object obj)
		{
			FireDelete(new DeleteEvent(entityName, obj, this));
		}

		public void Update(object obj)
		{
			FireUpdate(new SaveOrUpdateEvent(null, obj, this));
		}

		public void Update(string entityName, object obj)
		{
			FireUpdate(new SaveOrUpdateEvent(entityName, obj, this));
		}

		public void SaveOrUpdate(object obj)
		{
			FireSaveOrUpdate(new SaveOrUpdateEvent(null, obj, this));
		}

		public void SaveOrUpdate(string entityName, object obj)
		{
			FireSaveOrUpdate(new SaveOrUpdateEvent(entityName, obj, this));
		}

		public void Update(object obj, object id)
		{
			FireUpdate(new SaveOrUpdateEvent(null, obj, id, this));
		}

		private static readonly object[] NoArgs = new object[0];
		private static readonly IType[] NoTypes = new IType[0];

		/// <summary>
		/// Retrieve a list of persistent objects using a Hibernate query
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IList Find(string query)
		{
			return List(query, new QueryParameters());
		}

		public IList Find(string query, object value, IType type)
		{
			return List(query, new QueryParameters(type, value));
		}

		public IList Find(string query, object[] values, IType[] types)
		{
			return List(query, new QueryParameters(types, values));
		}

		public override IList List(string query, QueryParameters parameters)
		{
			IList results = new ArrayList();
			List(query, parameters, results);
			return results;
		}

		public override IList<T> List<T>(string query, QueryParameters parameters)
		{
			List<T> results = new List<T>();
			List(query, parameters, results);
			return results;
		}

		public override void List(string query, QueryParameters queryParameters, IList results)
		{
			ErrorIfClosed();
			queryParameters.ValidateParameters();
			HQLQueryPlan plan = GetHQLQueryPlan(query, false);
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

		public override IQueryTranslator[] GetQueries(string query, bool scalar)
		{
			// take the union of the query spaces (ie the queried tables)
			IQueryTranslator[] q = factory.GetQuery(query, scalar, enabledFilters);
			HashedSet<string> qs = new HashedSet<string>();
			for (int i = 0; i < q.Length; i++)
			{
				qs.AddAll(q[i].QuerySpaces);
			}

			AutoFlushIfRequired(qs);

			return q;
		}

		public IEnumerable Enumerable(string query)
		{
			return Enumerable(query, NoArgs, NoTypes);
		}

		public IEnumerable Enumerable(string query, object value, IType type)
		{
			return Enumerable(query, new object[] { value }, new IType[] { type });
		}

		public IEnumerable Enumerable(string query, object[] values, IType[] types)
		{
			return Enumerable(query, new QueryParameters(types, values));
		}

		public override IEnumerable<T> Enumerable<T>(string query, QueryParameters queryParameters)
		{
			ErrorIfClosed();
			queryParameters.ValidateParameters();
			HQLQueryPlan plan = GetHQLQueryPlan(query, true);
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

		public override IEnumerable Enumerable(string query, QueryParameters queryParameters)
		{
			ErrorIfClosed();
			queryParameters.ValidateParameters();
			HQLQueryPlan plan = GetHQLQueryPlan(query, true);
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

		// TODO: Scroll(string query, QueryParameters queryParameters)

		public int Delete(string query)
		{
			return Delete(query, NoArgs, NoTypes);
		}

		public int Delete(string query, object value, IType type)
		{
			return Delete(query, new object[] { value }, new IType[] { type });
		}

		public int Delete(string query, object[] values, IType[] types)
		{
			if (string.IsNullOrEmpty(query))
			{
				throw new ArgumentNullException("query", "attempt to perform delete-by-query with null query");
			}

			ErrorIfClosed();

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

		public void Lock(object obj, LockMode lockMode)
		{
			FireLock(new LockEvent(obj, lockMode, this));
		}

		public void Lock(string entityName, object obj, LockMode lockMode)
		{
			FireLock(new LockEvent(entityName, obj, lockMode, this));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public IQuery CreateFilter(object collection, string queryString)
		{
			ErrorIfClosed();

			ErrorIfClosed();
			CollectionFilterImpl filter =
				new CollectionFilterImpl(queryString, collection, this,
				                         GetFilterQueryPlan(collection, queryString, null, false).ParameterMetadata);
			//filter.SetComment(queryString);
			return filter;
		}

		private FilterQueryPlan GetFilterQueryPlan(object collection, string filter, QueryParameters parameters, bool shallow)
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
				plan = factory.QueryPlanCache.GetFilterQueryPlan(filter, roleAfterFlush.Role, shallow, EnabledFilters);
			}
			else
			{
				// otherwise, we only need to flush if there are in-memory changes
				// to the queried tables
				plan = factory.QueryPlanCache.GetFilterQueryPlan(filter, roleBeforeFlush.Role, shallow, EnabledFilters);
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
						plan = factory.QueryPlanCache.GetFilterQueryPlan(filter, roleAfterFlush.Role, shallow, EnabledFilters);
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

		public override object Instantiate(string clazz, object id)
		{
			return Instantiate(factory.GetEntityPersister(clazz), id);
		}

		/// <summary> Get the ActionQueue for this session</summary>
		public ActionQueue ActionQueue
		{
			get
			{
				ErrorIfClosed();
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
			object result = interceptor.Instantiate(persister.EntityName, entityMode, id);
			if (result == null)
			{
				result = persister.Instantiate(id, entityMode);
			}
			return result;
		}

		#region IEventSource Members
		/// <summary> Force an immediate flush</summary>
		public void ForceFlush(EntityEntry entityEntry)
		{
			ErrorIfClosed();
			if (log.IsDebugEnabled)
			{
				log.Debug("flushing to force deletion of re-saved object: " + MessageHelper.InfoString(entityEntry.Persister, entityEntry.Id, Factory));
			}

			if (persistenceContext.CascadeLevel > 0)
			{
				throw new ObjectDeletedException(
					"deleted object would be re-saved by cascade (remove deleted object from associations)", entityEntry.Id,
					entityEntry.EntityName);
			}

			Flush();
		}

		/// <summary> Cascade merge an entity instance</summary>
		public void Merge(string entityName, object obj, IDictionary copiedAlready)
		{
			FireMerge(copiedAlready, new MergeEvent(entityName, obj, this));
		}

		/// <summary> Cascade persist an entity instance</summary>
		public void Persist(string entityName, object obj, IDictionary createdAlready)
		{
			FirePersist(createdAlready, new PersistEvent(entityName, obj, this));
		}

		/// <summary> Cascade persist an entity instance during the flush process</summary>
		public void PersistOnFlush(string entityName, object obj, IDictionary copiedAlready)
		{
			FirePersistOnFlush(copiedAlready, new PersistEvent(entityName, obj, this));
		}

		/// <summary> Cascade refesh an entity instance</summary>
		public void Refresh(object obj, IDictionary refreshedAlready)
		{
			FireRefresh(refreshedAlready, new RefreshEvent(obj, this));
		}

		/// <summary> Cascade copy an entity instance</summary>
		public void SaveOrUpdateCopy(string entityName, object obj, IDictionary copiedAlready)
		{
			FireSaveOrUpdateCopy(copiedAlready, new MergeEvent(entityName, obj, this));
		}

		/// <summary> Cascade delete an entity instance</summary>
		public void Delete(string entityName, object child, bool isCascadeDeleteEnabled, ISet transientEntities)
		{
			FireDelete(new DeleteEvent(entityName, child, isCascadeDeleteEnabled, this), transientEntities);
		}

		#endregion

		public object Merge(string entityName, object obj)
		{
			return FireMerge(new MergeEvent(entityName, obj, this));
		}

		public object Merge(object obj)
		{
			return Merge(null, obj);
		}

		public void Persist(string entityName, object obj)
		{
			FirePersist(new PersistEvent(entityName, obj, this));
		}

		public void Persist(object obj)
		{
			Persist(null, obj);
		}

		public void PersistOnFlush(string entityName, object obj)
		{
			FirePersistOnFlush(new PersistEvent(entityName, obj, this));
		}

		public void PersistOnFlush(object obj)
		{
			Persist(null, obj);
		}

		/// <summary></summary>
		public override FlushMode FlushMode
		{
			get { return flushMode; }
			set { flushMode = value; }
		}

		public override string BestGuessEntityName(object entity)
		{
			INHibernateProxy proxy = entity as INHibernateProxy;
			if (proxy != null)
			{
				ILazyInitializer initializer = proxy.HibernateLazyInitializer;

				// it is possible for this method to be called during flush processing,
				// so make certain that we do not accidently initialize an uninitialized proxy
				if (initializer.IsUninitialized)
				{
					return initializer.PersistentClass.FullName;
				}
				entity = initializer.GetImplementation();
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

		public override string GuessEntityName(object entity)
		{
			string entityName = interceptor.GetEntityName(entity);
			if (entityName == null)
			{
				entityName = entity.GetType().FullName;
			}
			return entityName;
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
			ErrorIfClosed();
			// todo : should this get moved to PersistentContext?
			// logically, is PersistentContext the "thing" to which an interceptor gets attached?
			object result = persistenceContext.GetEntity(key);

			if (result == null)
			{
				object newObject = interceptor.GetEntity(key.EntityName, key.Identifier);
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

		public override IPersistenceContext PersistenceContext
		{
			get
			{
				ErrorIfClosed();
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
			ErrorIfClosed();
			if (!TransactionInProgress)
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

		public void Load(object obj, object id)
		{
			LoadEvent loadEvent = new LoadEvent(id, obj, this);
			FireLoad(loadEvent, LoadEventListener.Reload);
		}

		public object Load(System.Type clazz, object id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id", "null is not a valid identifier");
			}
			LoadEvent loadEvent = new LoadEvent(id, clazz.FullName, false, this);
			bool success = false;
			try
			{
				FireLoad(loadEvent, LoadEventListener.Load);
				if (loadEvent.Result == null)
					factory.EntityNotFoundDelegate.HandleEntityNotFound(clazz.FullName, id);

				success = true;
				return loadEvent.Result;
			}
			finally
			{
				AfterOperation(success);
			}
		}

		public T Load<T>(object id)
		{
			return (T)Load(typeof(T), id);
		}

		public T Load<T>(object id, LockMode lockMode)
		{
			return (T)Load(typeof(T), id, lockMode);
		}

		public T Get<T>(object id)
		{
			return (T)Get(typeof(T), id);
		}

		public T Get<T>(object id, LockMode lockMode)
		{
			return (T)Get(typeof(T), id, lockMode);
		}

		public string GetEntityName(object obj)
		{
			ErrorIfClosed();
			INHibernateProxy proxy = obj as INHibernateProxy;
			if (proxy != null)
			{
				if (!persistenceContext.ContainsProxy(proxy))
				{
					throw new TransientObjectException("proxy was not associated with the session");
				}
				ILazyInitializer li = ((INHibernateProxy)obj).HibernateLazyInitializer;

				obj = li.GetImplementation();
			}

			EntityEntry entry = persistenceContext.GetEntry(obj);
			if (entry == null)
			{
				throw new TransientObjectException(
					"object references an unsaved transient instance - save the transient instance before flushing: "
					+ obj.GetType().FullName);
			}
			return entry.Persister.EntityName;
		}

		public object Get(System.Type entityClass, object id)
		{
			return Get(entityClass.FullName, id);
		}

		public object Get(string entityName, object id)
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

		/// <summary> 
		/// Load the data for the object with the specified id into a newly created object.
		/// This is only called when lazily initializing a proxy.
		/// Do NOT return a proxy.
		/// </summary>
		public override object ImmediateLoad(string entityName, object id)
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


		/// <summary>
		/// Return the object with the specified id or throw exception if no row with that id exists. Defer the load,
		/// return a new proxy or return an existing proxy if possible. Do not check if the object was deleted.
		/// </summary>
		public override object InternalLoad(string entityName, object id, bool eager, bool isNullable)
		{
			// todo : remove
			LoadType type = isNullable ? LoadEventListener.InternalLoadNullable: (eager ? LoadEventListener.InternalLoadEager: LoadEventListener.InternalLoadLazy);
			LoadEvent loadEvent = new LoadEvent(id, entityName, true, this);
			FireLoad(loadEvent, type);
			if (!isNullable)
			{
				UnresolvableObjectException.ThrowIfNull(loadEvent.Result, id, entityName);
			}
			return loadEvent.Result; 
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This should return an existing proxy where appropriate.
		/// 
		/// If the object does not exist in the database, an exception is thrown.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		/// <exception cref="ObjectNotFoundException">
		/// Thrown when the object with the specified id does not exist in the database.
		/// </exception>
		public object Load(System.Type clazz, object id, LockMode lockMode)
		{
			LoadEvent loadEvent = new LoadEvent(id, clazz.FullName, lockMode, this);
			FireLoad(loadEvent, LoadEventListener.Load);
			return loadEvent.Result;
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
			LoadEvent loadEvent = new LoadEvent(id, clazz.FullName, lockMode, this);
			FireLoad(loadEvent, LoadEventListener.Get);
			return loadEvent.Result;
		}

		public void Refresh(object obj)
		{
			FireRefresh(new RefreshEvent(obj, this));
		}

		public void Refresh(object obj, LockMode lockMode)
		{
			FireRefresh(new RefreshEvent(obj, lockMode, this));
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			if (rootSession != null)
			{
				// Todo : should seriously consider not allowing a txn to begin from a child session
				//      can always route the request to the root session...
				log.Warn("Transaction started on non-root session");
			}

			ErrorIfClosed();
			return connectionManager.BeginTransaction(isolationLevel);
		}

		public ITransaction BeginTransaction()
		{
			if (rootSession != null)
			{
				// Todo : should seriously consider not allowing a txn to begin from a child session
				//      can always route the request to the root session...
				log.Warn("Transaction started on non-root session");
			}

			ErrorIfClosed();
			return connectionManager.BeginTransaction();
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
		/// users to repect foreign key constraints:
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
			ErrorIfClosed();
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

		public override bool TransactionInProgress
		{
			get
			{
				return !IsClosed && Transaction.IsActive;
			}
		}

		public bool IsDirty()
		{
			ErrorIfClosed();

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

		[NonSerialized]
		private System.Type lastClass;

		[NonSerialized]
		private IEntityPersister lastResultForClass;

		private IEntityPersister GetClassPersister(System.Type theClass)
		{
			if (lastClass != theClass)
			{
				lastResultForClass = factory.GetEntityPersister(theClass);
				lastClass = theClass;
			}
			return lastResultForClass;
		}

		public override IEntityPersister GetEntityPersister(object obj)
		{
			return GetClassPersister(obj.GetType());
		}

		/// <summary>
		/// Not for internal use
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetIdentifier(object obj)
		{
			ErrorIfClosed();
			// Actually the case for proxies will probably work even with
			// the session closed, but do the check here anyway, so that
			// the behavior is uniform.

			if (obj is INHibernateProxy)
			{
				ILazyInitializer li = ((INHibernateProxy)obj).HibernateLazyInitializer;
				if (li.Session != this)
				{
					throw new TransientObjectException("The proxy was not associated with this session");
				}
				return li.Identifier;
			}
			else
			{
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
			INHibernateProxy proxy = obj as INHibernateProxy;
			if (proxy != null)
			{
				return proxy.HibernateLazyInitializer.Identifier;
			}
			else
			{
				EntityEntry entry = persistenceContext.GetEntry(obj);
				return (entry != null) ? entry.Id : null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool IsSaved(object obj)
		{
			if (obj is INHibernateProxy)
			{
				return true; // NH sure ???
			}

			EntityEntry entry = persistenceContext.GetEntry(obj);
			if (entry != null)
			{
				return true;
			}

			bool? isUnsaved = interceptor.IsTransient(obj);
			if (isUnsaved.HasValue)
			{
				return !isUnsaved.Value;
			}
			isUnsaved = GetEntityPersister(obj).IsTransient(obj, this);
			if (isUnsaved.HasValue)
			{
				return !isUnsaved.Value;
			}

			return false;
		}

		internal ICollectionPersister GetCollectionPersister(string role)
		{
			return factory.GetCollectionPersister(role);
		}

		/// <summary>
		/// called by a collection that wants to initialize itself
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="writing"></param>
		public override void InitializeCollection(IPersistentCollection collection, bool writing)
		{
			ErrorIfClosed();
			IInitializeCollectionEventListener[] listener = listeners.InitializeCollectionEventListeners;
			for (int i = 0; i < listener.Length; i++)
			{
				listener[i].OnInitializeCollection(new InitializeCollectionEvent(collection, this));
			}
		}

		public override IDbConnection Connection
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
		/// An ISession is considered connected if there is an <see cref="IDbConnection"/> (regardless
		/// of its state) or if it the field <c>connect</c> is true.  Meaning that it will connect
		/// at the next operation that requires a connection.
		/// </remarks>
		public override bool IsConnected
		{
			get { return connectionManager.IsConnected; }
		}

		/// <summary></summary>
		public IDbConnection Disconnect()
		{
			ErrorIfClosed();
			log.Debug("disconnecting session");
			return connectionManager.Disconnect();
		}

		public void Reconnect()
		{
			ErrorIfClosed();
			log.Debug("reconnecting session");
			connectionManager.Reconnect();
		}

		public void Reconnect(IDbConnection conn)
		{
			ErrorIfClosed();
			log.Debug("reconnecting session");
			connectionManager.Reconnect(conn);
		}

		#region System.IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Dispose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		private string fetchProfile;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~SessionImpl()
		{
			Dispose(false);
		}

		/// <summary>
		/// Just in case the user forgot to Commit() or Close()
		/// </summary>
		public void Dispose()
		{
			log.Debug("running ISession.Dispose()");
			Dispose(true);
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
			if (_isAlreadyDisposed)
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the session if we
			// know this call came through Dispose()
			if (isDisposing && !IsClosed)
			{
				Close();
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize(this);
		}

		#endregion

		public ICollection Filter(object collection, string filter)
		{
			QueryParameters qp = new QueryParameters(new IType[1], new object[1]);
			return ListFilter(collection, filter, qp);
		}

		public ICollection Filter(object collection, string filter, object value, IType type)
		{
			QueryParameters qp = new QueryParameters(new IType[] { null, type }, new object[] { null, value });
			return ListFilter(collection, filter, qp);
		}

		public ICollection Filter(object collection, string filter, object[] values, IType[] types)
		{
			ErrorIfClosed();

			object[] vals = new object[values.Length + 1];
			IType[] typs = new IType[values.Length + 1];
			Array.Copy(values, 0, vals, 1, values.Length);
			Array.Copy(types, 0, typs, 1, types.Length);
			QueryParameters qp = new QueryParameters(typs, vals);
			return ListFilter(collection, filter, qp);
		}

		private void Filter(object collection, string filter, QueryParameters queryParameters, IList results)
		{
			ErrorIfClosed();
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

		public override IList ListFilter(object collection, string filter, QueryParameters queryParameters)
		{
			IList results = new ArrayList();
			Filter(collection, filter, queryParameters, results);
			return results;
		}

		public override IList<T> ListFilter<T>(object collection, string filter, QueryParameters queryParameters)
		{
			List<T> results = new List<T>();
			Filter(collection, filter, queryParameters, results);
			return results;
		}

		public override IEnumerable EnumerableFilter(object collection, string filter, QueryParameters queryParameters)
		{
			ErrorIfClosed();
			FilterQueryPlan plan = GetFilterQueryPlan(collection, filter, queryParameters, true);
			return plan.PerformIterate(queryParameters, this);
		}

		public override IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters queryParameters)
		{
			ErrorIfClosed();
			FilterQueryPlan plan = GetFilterQueryPlan(collection, filter, queryParameters, true);
			return plan.PerformIterate<T>(queryParameters, this);
		}

		public ICriteria CreateCriteria(System.Type persistentClass)
		{
			ErrorIfClosed();

			return new CriteriaImpl(persistentClass, this);
		}

		public ICriteria CreateCriteria(System.Type persistentClass, string alias)
		{
			ErrorIfClosed();

			return new CriteriaImpl(persistentClass, alias, this);
		}

		public override IList List(CriteriaImpl criteria)
		{
			ArrayList results = new ArrayList();
			List(criteria, results);
			return results;
		}

		public override IList<T> List<T>(CriteriaImpl criteria)
		{
			List<T> results = new List<T>();
			List(criteria, results);
			return results;
		}

		public override void List(CriteriaImpl criteria, IList results)
		{
			ErrorIfClosed();

			string[] implementors = factory.GetImplementors(criteria.EntityOrClassName);
			int size = implementors.Length;

			CriteriaLoader[] loaders = new CriteriaLoader[size];
			ISet<string> spaces = new HashedSet<string>();

			for (int i = 0; i < size; i++)
			{
				loaders[i] = new CriteriaLoader(
					GetOuterJoinLoadable(implementors[i]),
					factory,
					criteria,
					implementors[i],
					enabledFilters
					);

				spaces.AddAll(loaders[i].QuerySpaces);
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

		internal IOuterJoinLoadable GetOuterJoinLoadable(string entityName)
		{
			IEntityPersister persister = factory.GetEntityPersister(entityName);
			if (!(persister is IOuterJoinLoadable))
			{
				throw new MappingException("class persister is not OuterJoinLoadable: " + entityName);
			}
			return (IOuterJoinLoadable)persister;
		}

		public bool Contains(object obj)
		{
			ErrorIfClosed();

			if (obj is INHibernateProxy)
			{
				//do not use proxiesByKey, since not all
				//proxies that point to this session's
				//instances are in that collection!
				ILazyInitializer li = ((INHibernateProxy)obj).HibernateLazyInitializer;
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

		/// <summary>
		/// remove any hard references to the entity that are held by the infrastructure
		/// (references held by application or other persistant instances are okay)
		/// </summary>
		/// <param name="obj"></param>
		public void Evict(object obj)
		{
			FireEvict(new EvictEvent(obj, this));
		}

		public override ISQLQuery CreateSQLQuery(string sql)
		{
			ErrorIfClosed();
			return base.CreateSQLQuery(sql);
		}

		public IQuery CreateSQLQuery(string sql, string returnAlias, System.Type returnClass)
		{
			ErrorIfClosed();
			return new SqlQueryImpl(sql, new string[] { returnAlias }, new System.Type[] { returnClass }, this, factory.QueryPlanCache.GetSQLParameterMetadata(sql));
		}

		public IQuery CreateSQLQuery(string sql, string[] returnAliases, System.Type[] returnClasses)
		{
			ErrorIfClosed();
			return new SqlQueryImpl(sql, returnAliases, returnClasses, this, factory.QueryPlanCache.GetSQLParameterMetadata(sql));
		}

		public override IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			ArrayList results = new ArrayList();
			List(spec, queryParameters, results);
			return results;
		}

		public override IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			List<T> results = new List<T>();
			List(spec, queryParameters, results);
			return results;
		}

		public override void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
		{
			SQLCustomQuery query = new SQLCustomQuery(
				spec.SqlQueryReturns,
				spec.QueryString,
				spec.QuerySpaces,
				factory);
			ListCustomQuery(query, queryParameters, results);
		}

		public override void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			ErrorIfClosed();

			CustomLoader loader = new CustomLoader(customQuery, factory);
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

		public override IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
		{
			List<T> results = new List<T>();
			ListCustomQuery(customQuery, queryParameters, results);
			return results;
		}

		/// <summary></summary>
		public void Clear()
		{
			ErrorIfClosed();
			actionQueue.Clear();
			persistenceContext.Clear();
		}

		public void Replicate(object obj, ReplicationMode replicationMode)
		{
			FireReplicate(new ReplicateEvent(obj, replicationMode, this));
		}

		public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
		{
			FireReplicate(new ReplicateEvent(entityName, obj, replicationMode, this));
		}

		public ISessionFactory SessionFactory
		{
			get { return factory; }
		}

		public void CancelQuery()
		{
			ErrorIfClosed();

			Batcher.CancelLastQuery();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object SaveOrUpdateCopy(object obj)
		{
			return FireSaveOrUpdateCopy(new MergeEvent(null, obj, this));
		}

		public object SaveOrUpdateCopy(object obj, object id)
		{
			return FireSaveOrUpdateCopy(new MergeEvent(null, obj, id, this));
		}

		protected internal override void ErrorIfClosed()
		{
			if (_isAlreadyDisposed || IsClosed)
			{
				throw new ObjectDisposedException("ISession", "Session was disposed of or closed");
			}
		}

		public IFilter GetEnabledFilter(string filterName)
		{
			ErrorIfClosed();
			return enabledFilters[filterName];
		}

		public IFilter EnableFilter(string filterName)
		{
			ErrorIfClosed();
			FilterImpl filter = new FilterImpl(factory.GetFilterDefinition(filterName));
			enabledFilters[filterName] = filter;
			return filter;
		}

		public void DisableFilter(string filterName)
		{
			ErrorIfClosed();
			enabledFilters.Remove(filterName);
		}

		public override Object GetFilterParameterValue(string filterParameterName)
		{
			ErrorIfClosed();
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

		public override IType GetFilterParameterType(string filterParameterName)
		{
			ErrorIfClosed();
			string[] parsed = ParseFilterParameterName(filterParameterName);
			FilterDefinition filterDef = factory.GetFilterDefinition(parsed[0]);
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

		public override IDictionary<string, IFilter> EnabledFilters
		{
			get
			{
				ErrorIfClosed();

				foreach (IFilter filter in enabledFilters.Values)
				{
					filter.Validate();
				}

				return enabledFilters;
			}
		}

		private string[] ParseFilterParameterName(string filterParameterName)
		{
			int dot = filterParameterName.IndexOf(".");
			if (dot <= 0)
			{
				throw new ArgumentException("Invalid filter-parameter name format","filterParameterName");
			}
			string filterName = filterParameterName.Substring(0, dot);
			string parameterName = filterParameterName.Substring(dot + 1);
			return new string[] { filterName, parameterName };
		}

		public override ConnectionManager ConnectionManager
		{
			get { return connectionManager; }
		}

		public IMultiQuery CreateMultiQuery()
		{
			return new MultiQueryImpl(this);
		}

		public IMultiCriteria CreateMultiCriteria()
		{
			return new MultiCriteriaImpl(this, factory);
		}

		/// <summary> Get the statistics for this session.</summary>
		public ISessionStatistics Statistics
		{
			get
			{
				return new SessionStatisticsImpl(this);
			}
		}

		private void AfterOperation(bool success)
		{
			if (!connectionManager.IsInActiveTransaction)
			{
				connectionManager.AfterNonTransactionalQuery(success);
			}
		}

		public override void AfterTransactionBegin(ITransaction tx)
		{
			ErrorIfClosed();
			interceptor.AfterTransactionBegin(tx);
		}

		public override void BeforeTransactionCompletion(ITransaction tx)
		{
			log.Debug("before transaction completion");
			if ( rootSession == null ) 
			{
				try
				{
					interceptor.BeforeTransactionCompletion(tx);
				}
				catch (Exception e)
				{
					log.Error("exception in interceptor BeforeTransactionCompletion()", e);
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

		public override ISession GetSession()
		{
			return this;
		}

		public ISession GetSession(EntityMode entityMode)
		{
			if(this.entityMode.Equals(entityMode))
			{
				return this;
			}

			if(rootSession !=null)
			{
				return rootSession.GetSession(entityMode);
			}

			ErrorIfClosed();

			ISession rtn = null;
			if (childSessionsByEntityMode == null)
			{
				childSessionsByEntityMode = new Dictionary<EntityMode, ISession>();
			}
			else
			{
				if(childSessionsByEntityMode.ContainsKey(entityMode))
					rtn = childSessionsByEntityMode[entityMode];
			}

			if(rtn == null)
			{
				rtn = new SessionImpl(this, entityMode);
				childSessionsByEntityMode.Add(entityMode,rtn);
			}
			
			return rtn;
		}

		public override IInterceptor Interceptor
		{
			get { return interceptor; }
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
				ErrorIfClosed();
				if (log.IsDebugEnabled)
				{
					log.Debug("setting cache mode to: " + value);
				}
				cacheMode = value;
			}
		}

		public override EntityMode EntityMode
		{
			get { return entityMode; }
		}

		public override string FetchProfile
		{
			get { return fetchProfile; }
			set
			{
				ErrorIfClosed();
				fetchProfile = value;
			}
		}

		public void SetReadOnly(object entity, bool readOnly)
		{
			ErrorIfClosed();
			persistenceContext.SetReadOnly(entity, readOnly);
		}

		private void FireDelete(DeleteEvent @event)
		{
			ErrorIfClosed();
			IDeleteEventListener[] deleteEventListener = listeners.DeleteEventListeners;
			for (int i = 0; i < deleteEventListener.Length; i++)
			{
				deleteEventListener[i].OnDelete(@event);
			}
		}

		private void FireDelete(DeleteEvent @event, ISet transientEntities)
		{
			ErrorIfClosed();
			IDeleteEventListener[] deleteEventListener = listeners.DeleteEventListeners;
			for (int i = 0; i < deleteEventListener.Length; i++)
			{
				deleteEventListener[i].OnDelete(@event, transientEntities);
			}
		}

		private void FireEvict(EvictEvent evictEvent)
		{
			ErrorIfClosed();
			IEvictEventListener[] evictEventListener = listeners.EvictEventListeners;
			for (int i = 0; i < evictEventListener.Length; i++)
			{
				evictEventListener[i].OnEvict(evictEvent);
			}
		}

		private void FireLoad(LoadEvent @event, LoadType loadType)
		{
			ErrorIfClosed();
			ILoadEventListener[] loadEventListener = listeners.LoadEventListeners;
			for (int i = 0; i < loadEventListener.Length; i++)
			{
				loadEventListener[i].OnLoad(@event, loadType);
			}
		}

		private void FireLock(LockEvent lockEvent)
		{
			ErrorIfClosed();
			ILockEventListener[] lockEventListener = listeners.LockEventListeners;
			for (int i = 0; i < lockEventListener.Length; i++)
			{
				lockEventListener[i].OnLock(lockEvent);
			}
		}

		private object FireMerge(MergeEvent @event)
		{
			ErrorIfClosed();
			IMergeEventListener[] mergeEventListener = listeners.MergeEventListeners;
			for (int i = 0; i < mergeEventListener.Length; i++)
			{
				mergeEventListener[i].OnMerge(@event);
			}
			return @event.Result;
		}

		private void FireMerge(IDictionary copiedAlready, MergeEvent @event)
		{
			ErrorIfClosed();
			IMergeEventListener[] mergeEventListener = listeners.MergeEventListeners;
			for (int i = 0; i < mergeEventListener.Length; i++)
			{
				mergeEventListener[i].OnMerge(@event, copiedAlready);
			}
		}

		private void FirePersist(IDictionary copiedAlready, PersistEvent @event)
		{
			ErrorIfClosed();
			IPersistEventListener[] persistEventListener = listeners.PersistEventListeners;
			for (int i = 0; i < persistEventListener.Length; i++)
			{
				persistEventListener[i].OnPersist(@event, copiedAlready);
			}
		}

		private void FirePersist(PersistEvent @event)
		{
			ErrorIfClosed();
			IPersistEventListener[] createEventListener = listeners.PersistEventListeners;
			for (int i = 0; i < createEventListener.Length; i++)
			{
				createEventListener[i].OnPersist(@event);
			}
		}

		private void FirePersistOnFlush(IDictionary copiedAlready, PersistEvent @event)
		{
			ErrorIfClosed();
			IPersistEventListener[] persistEventListener = listeners.PersistOnFlushEventListeners;
			for (int i = 0; i < persistEventListener.Length; i++)
			{
				persistEventListener[i].OnPersist(@event, copiedAlready);
			}
		}

		private void FirePersistOnFlush(PersistEvent @event)
		{
			ErrorIfClosed();
			IPersistEventListener[] createEventListener = listeners.PersistOnFlushEventListeners;
			for (int i = 0; i < createEventListener.Length; i++)
			{
				createEventListener[i].OnPersist(@event);
			}
		}

		private void FireRefresh(RefreshEvent refreshEvent)
		{
			ErrorIfClosed();
			IRefreshEventListener[] refreshEventListener = listeners.RefreshEventListeners;
			for (int i = 0; i < refreshEventListener.Length; i++)
			{
				refreshEventListener[i].OnRefresh(refreshEvent);
			}
		}

		private void FireRefresh(IDictionary refreshedAlready, RefreshEvent refreshEvent)
		{
			ErrorIfClosed();
			IRefreshEventListener[] refreshEventListener = listeners.RefreshEventListeners;
			for (int i = 0; i < refreshEventListener.Length; i++)
			{
				refreshEventListener[i].OnRefresh(refreshEvent, refreshedAlready);
			}
		}

		private void FireReplicate(ReplicateEvent @event)
		{
			ErrorIfClosed();
			IReplicateEventListener[] replicateEventListener = listeners.ReplicateEventListeners;
			for (int i = 0; i < replicateEventListener.Length; i++)
			{
				replicateEventListener[i].OnReplicate(@event);
			}
		}

		private object FireSave(SaveOrUpdateEvent @event)
		{
			ErrorIfClosed();
			ISaveOrUpdateEventListener[] saveEventListener = listeners.SaveEventListeners;
			for (int i = 0; i < saveEventListener.Length; i++)
			{
				saveEventListener[i].OnSaveOrUpdate(@event);
			}
			return @event.ResultId;
		}

		private void FireSaveOrUpdate(SaveOrUpdateEvent @event)
		{
			ErrorIfClosed();
			ISaveOrUpdateEventListener[] saveOrUpdateEventListener = listeners.SaveOrUpdateEventListeners;
			for (int i = 0; i < saveOrUpdateEventListener.Length; i++)
			{
				saveOrUpdateEventListener[i].OnSaveOrUpdate(@event);
			}
		}

		private void FireSaveOrUpdateCopy(IDictionary copiedAlready, MergeEvent @event)
		{
			ErrorIfClosed();
			IMergeEventListener[] saveOrUpdateCopyEventListener = listeners.SaveOrUpdateCopyEventListeners;
			for (int i = 0; i < saveOrUpdateCopyEventListener.Length; i++)
			{
				saveOrUpdateCopyEventListener[i].OnMerge(@event, copiedAlready);
			}
		}

		private object FireSaveOrUpdateCopy(MergeEvent @event)
		{
			ErrorIfClosed();
			IMergeEventListener[] saveOrUpdateCopyEventListener = listeners.SaveOrUpdateCopyEventListeners;
			for (int i = 0; i < saveOrUpdateCopyEventListener.Length; i++)
			{
				saveOrUpdateCopyEventListener[i].OnMerge(@event);
			}
			return @event.Result;
		}

		private void FireUpdate(SaveOrUpdateEvent @event)
		{
			ErrorIfClosed();
			ISaveOrUpdateEventListener[] updateEventListener = listeners.UpdateEventListeners;
			for (int i = 0; i < updateEventListener.Length; i++)
			{
				updateEventListener[i].OnSaveOrUpdate(@event);
			}
		}

		public override int ExecuteNativeUpdate(NativeSQLQuerySpecification nativeQuerySpecification, QueryParameters queryParameters)
		{
			ErrorIfClosed();
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

		public override int ExecuteUpdate(string query, QueryParameters queryParameters)
		{
			ErrorIfClosed();
			queryParameters.ValidateParameters();
			HQLQueryPlan plan = GetHQLQueryPlan(query, false);
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

		public override IEntityPersister GetEntityPersister(string entityName, object obj)
		{
			ErrorIfClosed();
			if (entityName == null)
			{
				return factory.GetEntityPersister(GuessEntityName(obj));
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
					return factory.GetEntityPersister(entityName).GetSubclassEntityPersister(obj, Factory, entityMode);
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
}
