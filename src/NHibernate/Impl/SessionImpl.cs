using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Iesi.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Classic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Loader.Criteria;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

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
	public sealed class SessionImpl : IEventSource, ISerializable, IDeserializationCallback
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SessionImpl));

		[NonSerialized]
		private SessionFactoryImpl factory;

		private readonly long timestamp;

		/// <summary>
		/// Indicates if the Session has been closed.
		/// </summary>
		/// <value>
		/// <see langword="false" /> (by default) if the Session is Open and can be used, 
		/// <see langword="true" /> if the Session has had the methods <c>Close()</c> or
		/// <c>Dispose()</c> invoked.</value>
		private bool closed = false;

		private FlushMode flushMode = FlushMode.Auto;

		/// <summary>
		/// An <see cref="IDictionary"/> with the <see cref="EntityKey"/> as the key
		/// and an <see cref="Object"/> as the value.
		/// </summary>
		private readonly IDictionary entitiesByKey;

		// Snapshots of current database state for entities
		// that have *not* been loaded
		private IDictionary entitySnapshotsByKey;

		/// <summary>
		/// An <see cref="IDictionary"/> with the <see cref="EntityKey"/> as the key
		/// and an <see cref="INHibernateProxy"/> as the value.
		/// </summary>
		private IDictionary proxiesByKey;

		// these are used to serialize hashtables because .NET's Hashtable
		// implements IDeserializationCallback as we do, and .NET runtime calls our
		// OnDeserialize method before that of our hashtables which means that
		// the hashtables are not usable in our OnDeserialize.
		private ArrayList tmpProxiesKey;
		private ArrayList tmpProxiesProxy;
		private ArrayList tmpEnabledFiltersKey;
		private ArrayList tmpEnabledFiltersValue;

		//IdentityMaps are serializable in NH 
		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="Object"/> as the key
		/// and an <see cref="EntityEntry"/> as the value.
		/// </summary>
		private readonly IdentityMap entityEntries;

		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="Array"/> as the key
		/// and an <see cref="PersistentArrayHolder"/> as the value.
		/// </summary>
		private readonly IdentityMap arrayHolders;

		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="IPersistentCollection"/> as the key
		/// and an <see cref="CollectionEntry"/> as the value.
		/// </summary>
		private readonly IdentityMap collectionEntries;

		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="CollectionKey"/> as the key
		/// and an <see cref="IPersistentCollection"/> as the value.
		/// </summary>
		private readonly IDictionary collectionsByKey;

		/// <summary>
		/// An <see cref="ISet"/> of <see cref="EntityKey"/> objects of the deleted entities.
		/// </summary>
		private ISet nullifiables = new HashedSet();

		private readonly ISet nonExists;

		private readonly IInterceptor interceptor;

		// todo-events: implements listeners
		[NonSerialized]
		private EventListeners listeners;

		[NonSerialized]
		private readonly ActionQueue actionQueue;

		private readonly ConnectionManager connectionManager;

		[NonSerialized]
		private ArrayList executions;

		// The collections we are currently loading
		[NonSerialized]
		private IDictionary loadingCollections = new Hashtable();

		[NonSerialized]
		private IList nonlazyCollections;

		[NonSerialized]
		private int dontFlushFromFind = 0;

		[NonSerialized]
		private int cascading = 0;

		[NonSerialized]
		private int loadCounter = 0;

		[NonSerialized]
		private bool flushing;

		[NonSerialized] 
		private bool hasNonReadOnlyEntities = false;

		[NonSerialized]
		private IBatcher batcher;

		[NonSerialized]
		private IDictionary enabledFilters = new Hashtable();

		[NonSerialized]
		private BatchFetchQueue batchFetchQueue;

		[NonSerialized]
		private IDictionary unownedCollections;

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
			this.timestamp = info.GetInt64("timestamp");

			this.factory = (SessionFactoryImpl)info.GetValue("factory", typeof(SessionFactoryImpl));
			listeners = factory.EventListeners;
			actionQueue = (ActionQueue)info.GetValue("actionQueue", typeof(ActionQueue));

			this.entitiesByKey = (IDictionary)info.GetValue("entitiesByKey", typeof(IDictionary));
			entitySnapshotsByKey = (IDictionary)info.GetValue("entitySnapshotsByKey", typeof(IDictionary));
			// we did not actually serializing the IDictionary but instead the proxies in an arraylist
			//this.proxiesByKey = (IDictionary)info.GetValue( "proxiesByKey", typeof(IDictionary) );
			tmpProxiesKey = (ArrayList)info.GetValue("tmpProxiesKey", typeof(ArrayList));
			tmpProxiesProxy = (ArrayList)info.GetValue("tmpProxiesProxy", typeof(ArrayList));
			this.entityEntries = (IdentityMap)info.GetValue("entityEntries", typeof(IdentityMap));
			this.collectionEntries = (IdentityMap)info.GetValue("collectionEntries", typeof(IdentityMap));
			this.collectionsByKey = (IDictionary)info.GetValue("collectionsByKey", typeof(IDictionary));
			this.arrayHolders = (IdentityMap)info.GetValue("arrayHolders", typeof(IdentityMap));
			this.nonExists = (ISet)info.GetValue("nonExists", typeof(ISet));

			this.closed = info.GetBoolean("closed");
			this.flushMode = (FlushMode)info.GetValue("flushMode", typeof(FlushMode));

			this.nullifiables = (ISet)info.GetValue("nullifiables", typeof(ISet));
			this.interceptor = (IInterceptor)info.GetValue("interceptor", typeof(IInterceptor));

			//this.enabledFilters = (IDictionary) info.GetValue("enabledFilters", typeof(IDictionary));
			tmpEnabledFiltersKey = (ArrayList)info.GetValue("tmpEnabledFiltersKey", typeof(ArrayList));
			tmpEnabledFiltersValue = (ArrayList)info.GetValue("tmpEnabledFiltersValue", typeof(ArrayList));

			this.connectionManager = (ConnectionManager)info.GetValue("connectionManager", typeof(ConnectionManager));
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
			info.AddValue("actionQueue", actionQueue, typeof(ActionQueue));
			info.AddValue("timestamp", timestamp);
			info.AddValue("closed", closed);
			info.AddValue("flushMode", flushMode);
			info.AddValue("entitiesByKey", entitiesByKey, typeof(IDictionary));
			info.AddValue("entitySnapshotsByKey", entitySnapshotsByKey, typeof(IDictionary));

			// the IDictionary should not be serialized because the objects inside of it are not
			// fully deserialized until after the session is deserialized. Instead use two ArrayList 
			// to hold the values because they don't have the deserialization complexities that
			// hashtables do.
			tmpProxiesKey = new ArrayList(proxiesByKey.Count);
			tmpProxiesProxy = new ArrayList(proxiesByKey.Count);
			foreach (DictionaryEntry de in proxiesByKey)
			{
				tmpProxiesKey.Add(de.Key);
				tmpProxiesProxy.Add(de.Value);
			}

			info.AddValue("tmpProxiesKey", tmpProxiesKey);
			info.AddValue("tmpProxiesProxy", tmpProxiesProxy);

			info.AddValue("nullifiables", nullifiables, typeof(ISet));
			info.AddValue("interceptor", interceptor, typeof(IInterceptor));

			info.AddValue("entityEntries", entityEntries, typeof(IdentityMap));
			info.AddValue("collectionEntries", collectionEntries, typeof(IdentityMap));
			info.AddValue("collectionsByKey", collectionsByKey, typeof(IDictionary));
			info.AddValue("arrayHolders", arrayHolders, typeof(IdentityMap));
			info.AddValue("nonExists", nonExists, typeof(ISet));

			tmpEnabledFiltersKey = new ArrayList(enabledFilters.Count);
			tmpEnabledFiltersValue = new ArrayList(enabledFilters.Count);
			foreach (DictionaryEntry de in enabledFilters)
			{
				tmpEnabledFiltersKey.Add(de.Key);
				tmpEnabledFiltersValue.Add(de.Value);
			}

			//info.AddValue("enabledFilters", enabledFilters, typeof(IDictionary));
			info.AddValue("tmpEnabledFiltersKey", tmpEnabledFiltersKey);
			info.AddValue("tmpEnabledFiltersValue", tmpEnabledFiltersValue);

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

			InitTransientState();

			// we need to reconnect all proxies and collections to this session
			// the association is transient because serialization is used for
			// different things.

			foreach (DictionaryEntry e in collectionEntries)
			{
				try
				{
					((IPersistentCollection)e.Key).SetCurrentSession(this);
					CollectionEntry ce = (CollectionEntry)e.Value;
					if (ce.Role != null)
					{
						ce.SetLoadedPersister(factory.GetCollectionPersister(ce.Role));
					}
				}
				catch (HibernateException he)
				{
					// Different from h2.0.3
					throw new InvalidOperationException(he.Message);
				}
			}

			// recreate the proxiesByKey hashtable from the two arraylists.
			proxiesByKey = new Hashtable(tmpProxiesKey.Count);
			for (int i = 0; i < tmpProxiesKey.Count; i++)
			{
				proxiesByKey.Add(tmpProxiesKey[i], tmpProxiesProxy[i]);
			}

			// we can't remove an entry from an IDictionary while enumerating so store the ones
			// to remove in this list
			ArrayList keysToRemove = new ArrayList();

			foreach (DictionaryEntry de in proxiesByKey)
			{
				object key = de.Key;
				object proxy = de.Value;

				if (proxy is INHibernateProxy)
				{
					NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)proxy).Session = this;
				}
				else
				{
					// the proxy was pruned during the serialization process because
					// the target had been instantiated.
					keysToRemove.Add(key);
				}
			}

			for (int i = 0; i < keysToRemove.Count; i++)
			{
				proxiesByKey.Remove(keysToRemove[i]);
			}

			foreach (EntityEntry e in entityEntries.Values)
			{
				try
				{
					e.Persister = factory.GetEntityPersister(e.ClassName);
				}
				catch (MappingException me)
				{
					throw new InvalidOperationException(me.Message);
				}
			}

			// recreate the enabledFilters hashtable from the two arraylists.
			enabledFilters = new Hashtable(tmpEnabledFiltersKey.Count);
			for (int i = 0; i < tmpEnabledFiltersKey.Count; i++)
			{
				enabledFilters.Add(tmpEnabledFiltersKey[i], tmpEnabledFiltersValue[i]);
			}

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
			ConnectionReleaseMode connectionReleaseMode)
		{
			if (interceptor == null)
				throw new ArgumentNullException("interceptor", "The interceptor can not be null");

			this.connectionManager = new ConnectionManager(this, connection, connectionReleaseMode);
			this.interceptor = interceptor;
			this.timestamp = timestamp;
			this.factory = factory;
			listeners = factory.EventListeners;
			actionQueue = new ActionQueue(this);

			entitiesByKey = new Hashtable(50);
			proxiesByKey = new Hashtable(10);
			nonExists = new HashedSet();
			entitySnapshotsByKey = new Hashtable(50);
			//TODO: hack with this cast
			entityEntries = (IdentityMap)IdentityMap.InstantiateSequenced(50);
			collectionEntries = (IdentityMap)IdentityMap.InstantiateSequenced(30);
			collectionsByKey = new Hashtable(30);
			arrayHolders = (IdentityMap)IdentityMap.Instantiate(10);

			InitTransientState();

			log.Debug("opened session");
		}

		/// <summary></summary>
		public IBatcher Batcher
		{
			get { return batcher; }
		}

		/// <summary></summary>
		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		/// <summary></summary>
		public long Timestamp
		{
			get { return timestamp; }
		}

		/// <summary></summary>
		public IDbConnection Close()
		{
			log.Debug("closing session");

			try
			{
				return connectionManager.Close();
			}
			finally
			{
				Cleanup();
			}
		}

		/// <summary>
		/// Ensure that the locks are downgraded to <see cref="LockMode.None"/>
		/// and that all of the softlocks in the <see cref="Cache"/> have
		/// been released.
		/// </summary>
		public void AfterTransactionCompletion(bool success, ITransaction tx)
		{
			connectionManager.AfterTransaction();
			log.Debug("transaction completion");
			actionQueue.AfterTransactionCompletion(success);

			// Downgrade locks
			foreach (EntityEntry entry in entityEntries.Values)
			{
				entry.LockMode = LockMode.None;
			}

			// Release cache softlocks
			bool invalidateQueryCache = factory.IsQueryCacheEnabled;
			foreach (IExecutable executable in executions)
			{
				try
				{
					try
					{
						executable.AfterTransactionCompletion(success);
					}
					finally
					{
						if (invalidateQueryCache)
						{
							factory.UpdateTimestampsCache.Invalidate(executable.PropertySpaces);
						}
					}
				}
				catch (CacheException ce)
				{
					log.Error("could not release a cache lock", ce);
					// continue loop
				}
				catch (Exception e)
				{
					throw new AssertionFailure("Exception releasing cache locks", e);
				}
			}
			executions.Clear();

			if ( //rootSession == null &&
				tx != null)
			{
				try
				{
					interceptor.AfterTransactionCompletion(tx);
				}
				catch (Exception e)
				{
					log.Error("exception in interceptor AfterTransactionCompletion()", e);
				}
			}
		}

		private void InitTransientState()
		{
			executions = new ArrayList(50);
			loadingCollections = new Hashtable();
			nonlazyCollections = new ArrayList(20);

			batcher = SessionFactory.ConnectionProvider.Driver.CreateBatcher(connectionManager);
		}

		/// <summary>
		/// Mark the Session as being closed and Clear out the HashTables of
		/// entities and proxies along with the Identity Maps for entries, array
		/// holders, collections, and nullifiables.
		/// </summary>
		private void Cleanup()
		{
			closed = true;
			entitiesByKey.Clear();
			entitySnapshotsByKey.Clear();
			proxiesByKey.Clear();
			entityEntries.Clear();
			arrayHolders.Clear();
			collectionEntries.Clear();
			nullifiables.Clear();
			if (batchFetchQueue != null)
			{
				batchFetchQueue.Clear();
			}
			collectionsByKey.Clear();
			nonExists.Clear();
			if (unownedCollections != null)
			{
				unownedCollections.Clear();
			}
		}

		public LockMode GetCurrentLockMode(object obj)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "null object passed to GetCurrentLockMode");
			}
			if (obj is INHibernateProxy)
			{
				obj = (NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)obj)).GetImplementation(this);
				if (obj == null)
				{
					return LockMode.None;
				}
			}

			EntityEntry e = GetEntry(obj);
			if (e == null)
			{
				throw new TransientObjectException("Given object not associated with the session");
			}

			if (e.Status != Status.Loaded)
			{
				throw new ObjectDeletedException("The given object was deleted", e.Id, obj.GetType());
			}
			return e.LockMode;
		}

		public LockMode GetLockMode(object entity)
		{
			return GetEntry(entity).LockMode;
		}

		private void AddEntity(EntityKey key, object obj)
		{
			entitiesByKey[key] = obj;
			BatchFetchQueue.RemoveBatchLoadableEntityKey(key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object GetEntity(EntityKey key)
		{
			return entitiesByKey[key];
		}

		public object RemoveEntity(EntityKey key)
		{
			object retVal = entitiesByKey[key];
			entitiesByKey.Remove(key);
			entitySnapshotsByKey.Remove(key);
			nullifiables.Remove(key);
			BatchFetchQueue.RemoveBatchLoadableEntityKey(key);
			BatchFetchQueue.RemoveSubselect(key);
			return retVal;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="lockMode"></param>
		public void SetLockMode(object entity, LockMode lockMode)
		{
			GetEntry(entity).LockMode = lockMode;
		}

		private EntityEntry AddEntry(
			object obj,
			Status status,
			object[] loadedState,
			object id,
			object version,
			LockMode lockMode,
			bool existsInDatabase,
			IEntityPersister persister,
			bool disableVersionIncrement)
		{
			EntityEntry e =
				new EntityEntry(status, loadedState, id, version, lockMode, existsInDatabase, persister, disableVersionIncrement);
			entityEntries[obj] = e;
			SetHasNonReadOnlyEnties(status);
			return e;
		}

		public EntityEntry GetEntry(object obj)
		{
			return (EntityEntry)entityEntries[obj];
		}

		public EntityEntry RemoveEntry(object obj)
		{
			object retVal = entityEntries[obj];
			entityEntries.Remove(obj);
			return (EntityEntry)retVal;
		}

		public bool IsEntryFor(object obj)
		{
			return entityEntries.Contains(obj);
		}

		public CollectionEntry GetCollectionEntry(IPersistentCollection coll)
		{
			return (CollectionEntry)collectionEntries[coll];
		}

		public bool IsOpen
		{
			get { return !closed; }
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

		/// <summary>
		/// Save a transient object with a manually assigned ID
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Save(object obj, object id)
		{
			FireSave(new SaveOrUpdateEvent(null, obj, id, this));
		}

		public void ReassociateProxy(Object value, object id)
		{
			if (value is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy)value;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
				li.Identifier = id;
				ReassociateProxy(li, proxy);
			}
		}

		public void AddProxy(EntityKey key, object proxy)
		{
			proxiesByKey[key] = proxy;
		}

		public object GetProxy(EntityKey key)
		{
			return proxiesByKey[key];
		}

		public object Unproxy(object maybeProxy)
		{
			if (maybeProxy is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy)maybeProxy;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
				if (li.IsUninitialized)
				{
					throw new PersistentObjectException(
						string.Format("object was an uninitialized proxy for: {0}", li.PersistentClass.Name));
				}
				return li.GetImplementation(); //unwrap the object 
			}
			else
			{
				return maybeProxy;
			}
		}

		public object UnproxyAndReassociate(object maybeProxy)
		{
			if (maybeProxy is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy)maybeProxy;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
				ReassociateProxy(li, proxy);
				return li.GetImplementation(); //initialize + unwrap the object 
			}
			else
			{
				return maybeProxy;
			}
		}

		/// <summary>
		/// associate a proxy that was instantiated by another session with this session
		/// </summary>
		/// <param name="li"></param>
		/// <param name="proxy"></param>
		private void ReassociateProxy(LazyInitializer li, INHibernateProxy proxy)
		{
			if (li.Session != this)
			{
				IEntityPersister persister = GetClassPersister(li.PersistentClass);
				EntityKey key = new EntityKey(li.Identifier, persister);
				if (!proxiesByKey.Contains(key))
				{
					proxiesByKey[key] = proxy; // any earlier proxy takes precedence 
				}
				NHibernateProxyHelper.GetLazyInitializer(proxy).Session = this;
			}
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

		public void SaveOrUpdate(object obj)
		{
			FireSaveOrUpdate(new SaveOrUpdateEvent(null, obj, this));
		}

		public void Update(object obj, object id)
		{
			FireUpdate(new SaveOrUpdateEvent(null, obj, id, this));
		}

		private static object[] NoArgs = new object[0];
		private static IType[] NoTypes = new IType[0];

		/// <summary>
		/// Retrieve a list of persistent objects using a Hibernate query
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IList Find(string query)
		{
			return Find(query, new QueryParameters());
		}

		public IList Find(string query, object value, IType type)
		{
			return Find(query, new QueryParameters(type, value));
		}

		public IList Find(string query, object[] values, IType[] types)
		{
			return Find(query, new QueryParameters(types, values));
		}

		public IList Find(string query, QueryParameters parameters)
		{
			IList results = new ArrayList();
			Find(query, parameters, results);
			return results;
		}

		public IList<T> Find<T>(string query, QueryParameters parameters)
		{
			List<T> results = new List<T>();
			Find(query, parameters, results);
			return results;
		}

		public void Find(string query, QueryParameters parameters, IList results)
		{
			CheckIsOpen();

			if (log.IsDebugEnabled)
			{
				log.Debug("find: " + query);
				parameters.LogParameters(factory);
			}

			parameters.ValidateParameters();

			IQueryTranslator[] q = GetQueries(query, false);

			dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called

			//execute the queries and return all result lists as a single list
			bool success = false;
			try
			{
				for (int i = q.Length - 1; i >= 0; i--)
				{
					ArrayHelper.AddAll(results, q[i].List(this, parameters));
				}
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

		public IQueryTranslator[] GetQueries(string query, bool scalar)
		{
			// take the union of the query spaces (ie the queried tables)
			IQueryTranslator[] q = factory.GetQuery(query, scalar, enabledFilters);
			HashedSet qs = new HashedSet();
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

		public IEnumerable<T> Enumerable<T>(string query, QueryParameters parameters)
		{
			CheckIsOpen();

			if (log.IsDebugEnabled)
			{
				log.Debug("GetEnumerable: " + query);
				parameters.LogParameters(factory);
			}

			IQueryTranslator[] q = GetQueries(query, true);

			if (q.Length == 0)
			{
				return new List<T>();
			}

			IEnumerable[] results = new IEnumerable[q.Length];

			dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called
			//execute the queries and return all results as a single enumerable
			try
			{
				for (int i = 0; i < q.Length; i++)
				{
					results[i] = q[i].GetEnumerable(parameters, this);
				}

				return new GenericJoinedEnumerable<T>(results);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "Could not execute query");
			}
			finally
			{
				dontFlushFromFind--;
			}
		}

		public IEnumerable Enumerable(string query, QueryParameters parameters)
		{
			CheckIsOpen();

			if (log.IsDebugEnabled)
			{
				log.Debug("GetEnumerable: " + query);
				parameters.LogParameters(factory);
			}

			IQueryTranslator[] q = GetQueries(query, true);

			if (q.Length == 0)
			{
				return new ArrayList();
			}

			IEnumerable result = null;
			IEnumerable[] results = null;
			bool many = q.Length > 1;
			if (many)
			{
				results = new IEnumerable[q.Length];
			}

			dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called
			//execute the queries and return all results as a single enumerable
			try
			{
				for (int i = 0; i < q.Length; i++)
				{
					try
					{
						result = q[i].GetEnumerable(parameters, this);
					}
					catch (HibernateException)
					{
						// Do not call Convert on HibernateExceptions
						throw;
					}
					catch (Exception sqle)
					{
						throw Convert(sqle, "Could not execute query");
					}
					if (many)
					{
						results[i] = result;
					}
				}

				return many ? new JoinedEnumerable(results) : result;
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

			CheckIsOpen();

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public IQuery CreateFilter(object collection, string queryString)
		{
			CheckIsOpen();

			//Had to replace FilterImpl with version consistent with Hibernate3
			//Changed old FilterImpl to QueryFilterImpl
			return new QueryFilterImpl(queryString, collection, this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public IQuery CreateQuery(string queryString)
		{
			CheckIsOpen();

			return new QueryImpl(queryString, FlushMode.Unspecified, this);
		}

		/// <summary>
		/// Obtain an instance of <see cref="IQuery" /> for a named query string defined in the
		/// mapping file.
		/// </summary>
		/// <param name="queryName">The name of a query defined externally.</param>
		/// <returns>An <see cref="IQuery"/> fro a named query string.</returns>
		/// <remarks>
		/// The query can be either in <c>hql</c> or <c>sql</c> format.
		/// </remarks>
		public IQuery GetNamedQuery(string queryName)
		{
			CheckIsOpen();

			NamedQueryDefinition nqd = factory.GetNamedQuery(queryName);

			IQuery query;

			if (nqd != null)
			{
				string queryString = nqd.QueryString;
				query = new QueryImpl(
					queryString,
					nqd.FlushMode,
					this //,
					//GetHQLQueryPlan(queryString, false).ParameterMetadata
					);
				//TODO: query.Comment = "named HQL query " + queryName;
			}
			else
			{
				NamedSQLQueryDefinition nsqlqd = factory.GetNamedSQLQuery(queryName);
				if (nsqlqd == null)
				{
					throw new MappingException("Named query not known: " + queryName);
				}
				query = new SqlQueryImpl(
					nsqlqd,
					this //,
					//factory.QueryPlanCache.GetSQLParameterMetadata(nsqlqd.QueryString)
					);
				//TODO: query.Comment = "named native SQL query " + queryName;
				nqd = nsqlqd;
			}
			InitQuery(query, nqd);
			return query;
		}

		public IQuery GetNamedSQLQuery(string queryName)
		{
			CheckIsOpen();
			NamedSQLQueryDefinition nsqlqd = factory.GetNamedSQLQuery(queryName);
			if (nsqlqd == null)
			{
				throw new MappingException("Named SQL query not known: " + queryName);
			}
			IQuery query = new SqlQueryImpl(
				nsqlqd,
				this //,
				//factory.QueryPlanCache.GetSQLParameterMetadata( nsqlqd.QueryString )
				);
			//query.setComment( "named native SQL query " + queryName );
			InitQuery(query, nsqlqd);
			return query;
		}

		private void InitQuery(IQuery query, NamedQueryDefinition nqd)
		{
			query.SetCacheable(nqd.IsCacheable);
			query.SetCacheRegion(nqd.CacheRegion);
			if (nqd.Timeout != -1)
			{
				query.SetTimeout(nqd.Timeout);
			}
			//if (nqd.FetchSize != -1)
			//{
			//	query.SetFetchSize(nqd.FetchSize);
			//}
			//if ( nqd.getCacheMode() != null ) query.setCacheMode( nqd.getCacheMode() );
			//query.SetReadOnly(nqd.IsReadOnly);
			//if (nqd.Comment != null)
			//{
			//	query.SetComment(nqd.Comment);
			//}
		}

		public object Instantiate(System.Type clazz, object id)
		{
			return Instantiate(factory.GetEntityPersister(clazz), id);
		}

		/// <summary> Get the ActionQueue for this session</summary>
		public ActionQueue ActionQueue
		{
			get
			{
				CheckIsOpen();
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
			object result = interceptor.Instantiate(persister.MappedClass, id);
			if (result == null)
			{
				result = persister.Instantiate(id);
			}
			return result;
		}

		#region IEventSource Members
		/// <summary> Force an immediate flush</summary>
		public void ForceFlush(EntityEntry entityEntry)
		{
			CheckIsOpen();
			if (log.IsDebugEnabled)
			{
				log.Debug("flushing to force deletion of re-saved object: " + MessageHelper.InfoString(entityEntry.Persister, entityEntry.Id, Factory));
			}

			if (cascading > 0) //CascadeLevel
			{
				throw new ObjectDeletedException(
					"deleted object would be re-saved by cascade (remove deleted object from associations)", 
					entityEntry.Id, 
					entityEntry.Persister.MappedClass); // todo entityname: change with next line
					//entityEntry.Persister.EntityName);
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

		/// <summary></summary>
		public FlushMode FlushMode
		{
			get { return flushMode; }
			set { flushMode = value; }
		}

		public bool HasEventSource
		{
			get
			{
				return true;
			}
		}

		public object GetEntityUsingInterceptor(EntityKey key)
		{
			CheckIsOpen();
			// todo : should this get moved to PersistentContext?
			// logically, is PersistentContext the "thing" to which an interceptor gets attached?
			object result = GetEntity(key);
			if (result == null)
			{
				// TODO H3.2 Not ported
				//object newObject = interceptor.GetEntity(key.EntityName, key.Identifier);
				//if (newObject != null)
				//{
				//  Lock(newObject, LockMode.None);
				//}
				//return newObject;
				return null;
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// detect in-memory changes, determine if the changes are to tables
		/// named in the query and, if so, complete execution the flush
		/// </summary>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
		private bool AutoFlushIfRequired(ISet querySpaces)
		{
			CheckIsOpen();
			AutoFlushEvent autoFlushEvent = new AutoFlushEvent(querySpaces, this);
			IAutoFlushEventListener[] autoFlushEventListener = listeners.AutoFlushEventListeners;
			for (int i = 0; i < autoFlushEventListener.Length; i++)
			{
				autoFlushEventListener[i].OnAutoFlush(autoFlushEvent);
			}
			return autoFlushEvent.FlushRequired;
		}

		/// <summary>
		/// If the existing proxy is insufficiently "narrow" (derived), instantiate a 
		/// new proxy and overwrite the registration of the old one. This breaks == and 
		/// occurs only for "class" proxies rather than "interface" proxies.
		/// </summary>
		/// <param name="proxy"></param>
		/// <param name="persister"></param>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object NarrowProxy(object proxy, IEntityPersister persister, EntityKey key, object obj)
		{
			if (!persister.ConcreteProxyClass.IsAssignableFrom(proxy.GetType()))
			{
				if (log.IsWarnEnabled)
				{
					log.Warn(
						"Narrowing proxy to " + persister.ConcreteProxyClass + " - this operation breaks =="
						);
				}

				if (obj != null)
				{
					proxiesByKey.Remove(key);
					return obj;
				}
				else
				{
					proxy = persister.CreateProxy(key.Identifier, this);
					proxiesByKey[key] = proxy;
					return proxy;
				}
			}
			else
			{
				if (obj != null)
				{
					LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)proxy);
					li.SetImplementation(obj);
				}
				return proxy;
			}
		}

		/// <summary>
		/// Grab the existing proxy for an instance, if one exists.
		/// (otherwise return the instance)
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="key"></param>
		/// <param name="impl"></param>
		/// <returns></returns>
		public object ProxyFor(IEntityPersister persister, EntityKey key, object impl)
		{
			if (!persister.HasProxy || key == null)
			{
				return impl;
			}

			object proxy = proxiesByKey[key];
			if (proxy != null)
			{
				return NarrowProxy(proxy, persister, key, impl);
			}
			else
			{
				return impl;
			}
		}

		public object ProxyFor(object impl)
		{
			EntityEntry e = GetEntry(impl);

			// can't use e.persister since it is null after addUninitializedEntity 
			// (when this method is called)
			IEntityPersister p = GetEntityPersister(impl);
			return ProxyFor(p, new EntityKey(e.Id, p), impl);
		}

		/// <summary>
		/// Create a "temporary" entry for a newly instantiated entity. The entity is 
		/// uninitialized, but we need the mapping from id to instance in order to guarantee 
		/// uniqueness.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		public void AddUninitializedEntity(EntityKey key, object obj, LockMode lockMode)
		{
			AddEntity(key, obj);
			AddEntry(obj, Status.Loading, null, key.Identifier, null, lockMode, true, null, false); //temporary
		}

		/// <summary>
		/// Add the "hydrated state" (an array) of an uninitialized entity to the session. 
		/// We don't try to resolve any associations yet, because there might be other entities 
		/// waiting to be read from the ADO datareader we are currently processing
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="values"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		public void PostHydrate(IEntityPersister persister, object id, object[] values, object obj, LockMode lockMode)
		{
			//persister.SetIdentifier( obj, id );
			object version = Versioning.GetVersion(values, persister);
			AddEntry(obj, Status.Loading, values, id, version, lockMode, true, persister, false);

			if (log.IsDebugEnabled && version != null)
			{
				log.Debug("Version: " + version);
			}
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
			CheckIsOpen();
			if (obj is INHibernateProxy)
			{
				if (!ContainsProxy(obj))
				{
					throw new TransientObjectException("proxy was not associated with the session");
				}
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)obj);

				obj = li.GetImplementation();
			}

			EntityEntry entry = GetEntry(obj);
			if (entry == null)
			{
				ThrowTransientObjectException(obj);
			}
			return entry.Persister.EntityName;
		}

		public bool ContainsProxy(object obj)
		{
			foreach (DictionaryEntry entry in proxiesByKey)
			{
				if(entry.Value.Equals(obj)) return true;
			}
			return false;
		}

		public object Get(System.Type clazz, object id)
		{
			LoadEvent loadEvent = new LoadEvent(id, clazz.FullName, false, this);
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

		///<summary> 
		/// Load the data for the object with the specified id into a newly created object.
		/// Do NOT return a proxy.
		///</summary>
		public object ImmediateLoad(System.Type clazz, object id)
		{
			if (log.IsDebugEnabled)
			{
				IEntityPersister persister = Factory.GetEntityPersister(clazz);
				log.Debug("initializing proxy: " + MessageHelper.InfoString(persister, id, Factory));
			}

			LoadEvent loadEvent = new LoadEvent(id, clazz.FullName, true, this);
			FireLoad(loadEvent,LoadEventListener.ImmediateLoad);
			return loadEvent.Result;
		}

		/// <summary>
		/// Return the object with the specified id or throw exception if no row with that id exists. Defer the load,
		/// return a new proxy or return an existing proxy if possible. Do not check if the object was deleted.
		/// </summary>
		public object InternalLoad(System.Type clazz, object id, bool eager, bool isNullable)
		{
			// todo : remove
			LoadType type = isNullable ? LoadEventListener.InternalLoadNullable: (eager ? LoadEventListener.InternalLoadEager: LoadEventListener.InternalLoadLazy);
			LoadEvent loadEvent = new LoadEvent(id, clazz.FullName, true, this);
			FireLoad(loadEvent, type);
			if (!isNullable)
			{
				UnresolvableObjectException.ThrowIfNull(loadEvent.Result, id, clazz);
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

		/// <summary>
		/// After processing a JDBC result set, we "resolve" all the associations
		/// between the entities which were instantiated and had their state
		/// "hydrated" into an array
		/// </summary>
		/// <param name="obj"></param>
		public void InitializeEntity(object obj)
		{
			EntityEntry e = GetEntry(obj);
			if (e == null)
			{
				throw new AssertionFailure("possible non-threadsafe access to the session");
			}
			IEntityPersister persister = e.Persister;
			object id = e.Id;
			object[] hydratedState = e.LoadedState;
			IType[] types = persister.PropertyTypes;

			if (log.IsDebugEnabled)
			{
				log.Debug("resolving associations for: " + MessageHelper.InfoString(persister, id));
			}

			for (int i = 0; i < hydratedState.Length; i++)
			{
				hydratedState[i] = types[i].ResolveIdentifier(hydratedState[i], this, obj);
			}

			interceptor.OnLoad(obj, id, hydratedState, persister.PropertyNames, types);

			persister.SetPropertyValues(obj, hydratedState);

			if (persister.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("adding entity to second-level cache " + MessageHelper.InfoString(persister, id));
				}
				CacheKey ck = new CacheKey(
					id,
					persister.IdentifierType,
					(string)persister.IdentifierSpace,
					factory
					);
				persister.Cache.Put(
					ck,
					new CacheEntry(obj, persister, this),
					Timestamp,
					Versioning.GetVersion(hydratedState, persister),
					persister.IsVersioned ?
					persister.VersionType.Comparator : null,
					UseMinimalPuts(e));
			}

			if (persister.ImplementsLifecycle)
			{
				log.Debug("calling OnLoad()");
				((ILifecycle)obj).OnLoad(this, id);
			}

			// after setting values to object
			TypeFactory.DeepCopy(hydratedState, persister.PropertyTypes, persister.PropertyUpdateability, hydratedState);
			e.Status = Status.Loaded;

			if (log.IsDebugEnabled)
			{
				log.Debug("done materializing entity " + MessageHelper.InfoString(persister, id));
			}
		}

		private bool UseMinimalPuts(EntityEntry entityEntry)
		{
			//TODO port this functionality from Hibernate 3.1
			/*return (factory.Settings.IsMinimalPutsEnabled &&
							CacheMode != CacheMode.REFRESH) ||
							(entityEntry.getPersister().hasLazyProperties() &&
							entityEntry.isLoadedWithLazyPropertiesUnfetched() &&
							entityEntry.getPersister().isLazyPropertiesCacheable());*/
			return factory.Settings.IsMinimalPutsEnabled;
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			CheckIsOpen();
			return connectionManager.BeginTransaction(isolationLevel);
		}

		public ITransaction BeginTransaction()
		{
			CheckIsOpen();
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
		/// This can be called from commit() or at the start of a Find() method.
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
		public void Flush()
		{
			CheckIsOpen();
			if (CascadeLevel > 0)
			{
				throw new HibernateException("Flush during cascade is dangerous");
			}
			IFlushEventListener[] flushEventListener = listeners.FlushEventListeners;
			for (int i = 0; i < flushEventListener.Length; i++)
			{
				flushEventListener[i].OnFlush(new FlushEvent(this));
			}
		}

		public int CascadeLevel
		{
			get { return cascading; }
		}

		public bool IsDirty()
		{
			CheckIsOpen();

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

		public void PostDelete(object obj)
		{
			EntityEntry entry = RemoveEntry(obj);
			if (entry == null)
			{
				throw new AssertionFailure("possible nonthreadsafe access to session");
			}
			entry.Status = Status.Gone;
			entry.ExistsInDatabase = false;
			EntityKey key = new EntityKey(entry.Id, entry.Persister);
			RemoveEntity(key);
			RemoveProxy(key);
		}

		public void RemoveProxy(EntityKey key)
		{
			if (batchFetchQueue != null)
			{
				batchFetchQueue.RemoveSubselect(key);
				batchFetchQueue.RemoveBatchLoadableEntityKey(key);
			}
			proxiesByKey.Remove(key);
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

		public IEntityPersister GetEntityPersister(object obj)
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
			CheckIsOpen();
			// Actually the case for proxies will probably work even with
			// the session closed, but do the check here anyway, so that
			// the behavior is uniform.

			if (obj is INHibernateProxy)
			{
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)obj);
				if (li.Session != this)
				{
					throw new TransientObjectException("The proxy was not associated with this session");
				}
				return li.Identifier;
			}
			else
			{
				EntityEntry entry = GetEntry(obj);
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
		public object GetEntityIdentifier(object obj)
		{
			if (obj is INHibernateProxy)
			{
				return NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)obj).Identifier;
			}
			else
			{
				EntityEntry entry = GetEntry(obj);
				return (entry != null) ? entry.Id : null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsSaved(object obj)
		{
			if (obj is INHibernateProxy)
			{
				return true;
			}

			EntityEntry entry = GetEntry(obj);
			if (entry != null)
			{
				return true;
			}

			bool? isUnsaved = interceptor.IsUnsaved(obj);
			if (isUnsaved.HasValue)
			{
				return !isUnsaved.Value;
			}

			return !GetEntityPersister(obj).IsUnsaved(obj);
		}

		/// <summary>
		/// Used by OneToOneType and ManyToOneType to determine what id value
		/// should be used for an object that may or may not be associated with
		/// the session. This does a "best guess" using any/all info available
		/// to use (not just the EntityEntry).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetEntityIdentifierIfNotUnsaved(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			if (obj is INHibernateProxy)
			{
				return NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)obj).Identifier;
			}
			else
			{
				EntityEntry entry = GetEntry(obj);
				if (entry != null)
				{
					return entry.Id;
				}
				else
				{
					bool? isUnsaved = interceptor.IsUnsaved(obj);

					if (isUnsaved.HasValue && (isUnsaved.Value))
					{
						ThrowTransientObjectException(obj);
					}

					IEntityPersister persister = GetEntityPersister(obj);
					if (persister.IsUnsaved(obj))
					{
						ThrowTransientObjectException(obj);
					}

					return persister.GetIdentifier(obj);
				}
			}
		}

		private static void ThrowTransientObjectException(object obj)
		{
			throw new TransientObjectException(
				"object references an unsaved transient instance - save the transient instance before flushing: " +
				obj.GetType().FullName
				);
		}


		private sealed class LoadingCollectionEntry
		{
			// Key is not present in H2.1, it was added to be able
			// to implement EndLoadingCollections efficiently
			private readonly CollectionKey key;
			private readonly IPersistentCollection collection;
			private readonly object id;
			private readonly object resultSetId;
			private readonly ICollectionPersister persister;

			internal LoadingCollectionEntry(CollectionKey key, IPersistentCollection collection, object id,
											ICollectionPersister persister, object resultSetId)
			{
				this.key = key;
				this.collection = collection;
				this.id = id;
				this.resultSetId = resultSetId;
				this.persister = persister;
			}

			public CollectionKey Key
			{
				get { return key; }
			}

			public IPersistentCollection Collection
			{
				get { return collection; }
			}

			public object Id
			{
				get { return id; }
			}

			public object ResultSetId
			{
				get { return resultSetId; }
			}

			public ICollectionPersister Persister
			{
				get { return persister; }
			}
		}

		private LoadingCollectionEntry GetLoadingCollectionEntry(CollectionKey collectionKey)
		{
			return (LoadingCollectionEntry)loadingCollections[collectionKey];
		}

		private void AddLoadingCollectionEntry(CollectionKey key, IPersistentCollection collection,
											   ICollectionPersister persister, object resultSetId)
		{
			loadingCollections.Add(key, new LoadingCollectionEntry(key, collection, key.Key, persister, resultSetId));
		}

		public IPersistentCollection GetLoadingCollection(ICollectionPersister persister, object id, object resultSetId)
		{
			CollectionKey ckey = new CollectionKey(persister, id);
			LoadingCollectionEntry lce = GetLoadingCollectionEntry(ckey);
			if (lce == null)
			{
				// look for existing collection
				IPersistentCollection pc = GetCollection(ckey);
				if (pc != null)
				{
					if (pc.WasInitialized)
					{
						log.Debug("collection already initialized: ignoring");
						return null; //ignore this row of results! Note the early exit
					}
					else
					{
						log.Debug("uninitialized collection: initializing");
					}
				}
				else
				{
					object entity = GetCollectionOwner(id, persister);
					if (entity != null && GetEntry(entity).Status != Status.Loading)
					{
						// important, to account for newly saved entities in query
						log.Debug("owning entity already loaded: ignoring");
						return null;
					}
					else
					{
						//create one
						log.Debug("new collection: instantiating");
						pc = persister.CollectionType.Instantiate(this, persister);
					}
				}
				pc.BeforeInitialize(persister);
				pc.BeginRead();
				AddLoadingCollectionEntry(ckey, pc, persister, resultSetId);
				return pc;
			}
			else
			{
				if (lce.ResultSetId == resultSetId)
				{
					log.Debug("reading row");
					return lce.Collection;
				}
				else
				{
					//ignore this row, the collection is in process of being loaded somewhere further "up" the stack
					log.Debug("collection is already being initialized: ignoring row");
					return null;
				}
			}
		}

		public void EndLoadingCollections(ICollectionPersister persister, object resultSetId)
		{
			// scan the loading collections for collections from this result set
			// put them in a new temp collection so that we are safe from concurrent
			// modification when the call to endRead() causes a proxy to be
			// initialized
			IList resultSetCollections = null;
			foreach (LoadingCollectionEntry lce in loadingCollections.Values)
			{
				if (lce.ResultSetId == resultSetId && lce.Persister == persister)
				{
					if (resultSetCollections == null)
					{
						resultSetCollections = new ArrayList();
					}
					resultSetCollections.Add(lce);
					if (lce.Collection.Owner == null)
					{
						AddUnownedCollection(
							new CollectionKey(persister, lce.Id),
							lce.Collection);
					}
				}
			}

			if (resultSetCollections != null)
			{
				// Remove them from the original collection - expanded LoadingCollectionEntry to know it's key to do this
				foreach (LoadingCollectionEntry lce in resultSetCollections)
				{
					loadingCollections.Remove(lce.Key);
				}
			}

			EndLoadingCollections(persister, resultSetCollections);
		}

		private void EndLoadingCollections(ICollectionPersister persister, IList resultSetCollections)
		{
			int count = resultSetCollections == null ? 0 : resultSetCollections.Count;

			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("{0} collections were found in result set", count));
			}

			// now finish them
			for (int i = 0; i < count; i++)
			{
				LoadingCollectionEntry lce = (LoadingCollectionEntry)resultSetCollections[i];
				bool noQueueAdds = lce.Collection.EndRead(persister); // warning: can cause recursive query! (proxy initialization)
				CollectionEntry ce = GetCollectionEntry(lce.Collection);
				if (ce == null)
				{
					ce = AddInitializedCollection(lce.Collection, persister, lce.Id);
				}
				else
				{
					ce.PostInitialize(lce.Collection);
				}

				bool addToCache = noQueueAdds && persister.HasCache && !ce.IsDoremove;
				if (addToCache)
				{
					AddCollectionToCache(lce, persister);
				}
				if (log.IsDebugEnabled)
				{
					log.Debug("collection fully initialized: " + MessageHelper.InfoString(persister, lce.Id));
				}
			}

			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("{0} collections initialized", count));
			}
		}

		private void AddCollectionToCache(LoadingCollectionEntry lce, ICollectionPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("caching collection: " + MessageHelper.InfoString(persister, lce.Id));
			}

			if (EnabledFilters.Count > 0 && persister.IsAffectedByEnabledFilters(this))
			{
				// some filters affecting the collection are enabled on the session, so do not do the put into the cache.
				log.Debug("Refusing to add to cache due to enabled filters");
				return; // EARLY EXIT!!!!!
			}

			IEntityPersister ownerPersister = factory.GetEntityPersister(persister.OwnerClass);
			object version;
			IComparer versionComparator;
			if (persister.IsVersioned)
			{
				version = GetEntry(GetCollectionOwner(lce.Id, persister)).Version;
				versionComparator = ownerPersister.VersionType.Comparator;
			}
			else
			{
				version = null;
				versionComparator = null;
			}
			CacheKey ck = new CacheKey(lce.Id, persister.KeyType, persister.Role, factory);
			persister.Cache.Put(ck, lce.Collection.Disassemble(persister), Timestamp, version, versionComparator,
								factory.Settings.IsMinimalPutsEnabled /*&& cacheMode != CacheMode.REFRESH*/);
		}

		private IPersistentCollection GetLoadingCollection(ICollectionPersister persister, object id)
		{
			LoadingCollectionEntry lce = GetLoadingCollectionEntry(new CollectionKey(persister, id));
			return (lce != null) ? lce.Collection : null;
		}

		public void BeforeLoad()
		{
			loadCounter++;
		}

		public void AfterLoad()
		{
			loadCounter--;
		}

		public void InitializeNonLazyCollections()
		{
			if (loadCounter == 0)
			{
				log.Debug("initializing non-lazy collections");
				// Do this work only at the very highest level of the load

				// Don't let this method be called recursively
				loadCounter++;
				try
				{
					while (nonlazyCollections.Count > 0)
					{
						//note that each iteration of the loop may add new elements
						IPersistentCollection collection = (IPersistentCollection)nonlazyCollections[nonlazyCollections.Count - 1];
						nonlazyCollections.RemoveAt(nonlazyCollections.Count - 1);
						collection.ForceInitialization();
					}
				}
				finally
				{
					loadCounter--;
				}
			}
		}

		private void AddCollection(IPersistentCollection collection, CollectionEntry entry, object key)
		{
			collectionEntries[collection] = entry;

			CollectionKey ck = new CollectionKey(entry.LoadedPersister, key);
			IPersistentCollection old = (IPersistentCollection)collectionsByKey[ck];
			collectionsByKey[ck] = collection;

			if (old != null)
			{
				if (old == collection)
				{
					throw new AssertionFailure("collection added twice");
				}
				// or should it actually throw an exception?
				old.UnsetSession(this);
				collectionEntries.Remove(old);
				// watch out for a case where old is still referenced
				// somewhere in the object graph! (which is a user error)
			}
		}

		private IPersistentCollection GetCollection(CollectionKey key)
		{
			return (IPersistentCollection)collectionsByKey[key];
		}

		/// <summary>
		/// add a collection we just loaded up (still needs initializing)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		private void AddUninitializedCollection(IPersistentCollection collection, ICollectionPersister persister, object id)
		{
			CollectionEntry ce = new CollectionEntry(persister, id, flushing);
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);
		}

		public void AddUninitializedDetachedCollection(IPersistentCollection collection, ICollectionPersister persister,
														object id)
		{
			CollectionEntry ce = new CollectionEntry(persister, id);
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);
		}

		/// <summary> 
		/// Add a new collection (ie. a newly created one, just instantiated by the
		/// application, with no database state or snapshot)
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="collection">The collection to be associated with the persistence context </param>
		public void AddNewCollection(ICollectionPersister persister, IPersistentCollection collection)
		{
			AddNewCollection(collection, persister);
		}

		/// <summary> Get the <see cref="PersistentArrayHolder"/> object for an array</summary>
		public PersistentArrayHolder GetCollectionHolder(object array)
		{
			return GetArrayHolder(array);
		}

		/// <summary> 
		/// Register a <see cref="PersistentArrayHolder"/> object for an array.
		/// Associates a holder with an array - MUST be called after loading 
		/// array, since the array instance is not created until endLoad().
		/// </summary>
		public void AddCollectionHolder(PersistentArrayHolder holder)
		{
			AddArrayHolder(holder);
		}

		/// <summary> 
		/// Remove the mapping of collection to holder during eviction
		/// of the owning entity
		/// </summary>
		public IPersistentCollection RemoveCollectionHolder(object array)
		{
			IPersistentCollection result;
			result = (IPersistentCollection)arrayHolders[array];
			arrayHolders.Remove(array);
			return result;
		}

		/// <summary> 
		/// False if we know for certain that all the entities are read-only.
		/// </summary>
		public bool HasNonReadOnlyEntities
		{
			get { return hasNonReadOnlyEntities; }
		}

		public void SetEntryStatus(EntityEntry entry, Status status)
		{
			entry.Status = status;
			SetHasNonReadOnlyEnties(status);
		}

		public ISet NullifiableEntityKeys
		{
			get { return nullifiables; }
		}

		public object[] GetDatabaseSnapshot(object id, IEntityPersister persister)
		{
			EntityKey key = new EntityKey(id, persister);
			object cached = entitySnapshotsByKey[key];
			if (cached != null)
			{
				return cached == NoRow ? null : (object[])cached;
			}
			else
			{
				object[] snapshot = persister.GetDatabaseSnapshot(id, this);
				entitySnapshotsByKey[key] = snapshot ?? NoRow;
				return snapshot;
			}
		}

		/// <summary> 
		/// Retrieve the cached database snapshot for the requested entity key.
		/// </summary>
		/// <param name="key">The entity key for which to retrieve the cached snapshot </param>
		/// <returns> The cached snapshot </returns>
		/// <remarks>
		/// <list type="bullet">
		/// <listheader><description>This differs from <see cref="GetDatabaseSnapshot"/> is two important respects:</description></listheader>
		/// <item><description>no snapshot is obtained from the database if not already cached</description></item>
		/// <item><description>an entry of NO_ROW here is interpretet as an exception</description></item>
		/// </list>
		/// </remarks>
		public object[] GetCachedDatabaseSnapshot(EntityKey key)
		{
			object snapshot = entitySnapshotsByKey[key];
			if (snapshot == NoRow)
			{
				throw new HibernateException("persistence context reported no row snapshot for " + MessageHelper.InfoString(key.MappedClass, key.Identifier));
			}
			return (object[])snapshot;
		}

		private void SetHasNonReadOnlyEnties(Status value)
		{
			if (value == Status.Deleted || value == Status.Loaded || value == Status.Saving)
			{
				hasNonReadOnlyEntities = true;
			}
		}

		/// <summary>
		/// add a collection we just pulled out of the cache (does not need initializing)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		public CollectionEntry AddInitializedCollection(IPersistentCollection collection, ICollectionPersister persister,
														object id)
		{
			CollectionEntry ce = new CollectionEntry(persister, id, flushing);
			ce.PostInitialize(collection);
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);

			return ce;
		}

		private CollectionEntry AddCollection(IPersistentCollection collection)
		{
			CollectionEntry ce = new CollectionEntry(collection);
			collectionEntries[collection] = ce;
			collection.CollectionSnapshot = ce;
			return ce;
		}

		/// <summary>
		/// Add a new collection (i.e. a newly created one, just instantiated by
		/// the application, with no database state or snapshot)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		internal void AddNewCollection(IPersistentCollection collection, ICollectionPersister persister)
		{
			// todo-events Remove (move implementation below)
			CollectionEntry ce = AddCollection(collection);
			if (persister.HasOrphanDelete)
			{
				ce.InitSnapshot(collection, persister);
			}
		}

		/// <summary>
		/// Add an (initialized) collection that was created by another session and passed
		/// into update() (i.e. one with a snapshot and existing state on the database)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="cs"></param>
		public void AddInitializedDetachedCollection(IPersistentCollection collection, ICollectionSnapshot cs)
		{
			if (cs.WasDereferenced)
			{
				AddCollection(collection);
			}
			else
			{
				CollectionEntry ce = new CollectionEntry(cs, factory);
				collection.CollectionSnapshot = ce;
				AddCollection(collection, ce, cs.Key);
			}
		}

		private PersistentArrayHolder GetArrayHolder(object array)
		{
			return (PersistentArrayHolder)arrayHolders[array];
		}

		private void AddArrayHolder(PersistentArrayHolder holder)
		{
			arrayHolders[holder.Array] = holder;
		}

		internal ICollectionPersister GetCollectionPersister(string role)
		{
			return factory.GetCollectionPersister(role);
		}

		public object GetSnapshot(IPersistentCollection coll)
		{
			return GetCollectionEntry(coll).Snapshot;
		}

		public object GetLoadedCollectionKey(IPersistentCollection coll)
		{
			return GetCollectionEntry(coll).LoadedKey;
		}

		public bool IsInverseCollection(IPersistentCollection collection)
		{
			CollectionEntry ce = GetCollectionEntry(collection);
			return ce != null && ce.LoadedPersister.IsInverse;
		}

		/// <summary>
		/// called by a collection that wants to initialize itself
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="writing"></param>
		public void InitializeCollection(IPersistentCollection collection, bool writing)
		{
			CollectionEntry ce = GetCollectionEntry(collection);

			if (ce == null)
			{
				throw new HibernateException("collection was evicted");
			}

			if (!collection.WasInitialized)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("initializing collection " + MessageHelper.InfoString(ce.LoadedPersister, ce.LoadedKey));
				}

				log.Debug("checking second-level cache");

				bool foundInCache =
					InitializeCollectionFromCache(ce.LoadedKey, GetCollectionOwner(ce), ce.LoadedPersister, collection);

				if (foundInCache)
				{
					log.Debug("collection initialized from cache");
				}
				else
				{
					log.Debug("collection not cached");
					ce.LoadedPersister.Initialize(ce.LoadedKey, this);
					log.Debug("collection initialized");
				}
			}
		}

		private object GetCollectionOwner(CollectionEntry ce)
		{
			return GetCollectionOwner(ce.LoadedKey, ce.LoadedPersister);
		}

		public object GetCollectionOwner(object key, ICollectionPersister collectionPersister)
		{
			//TODO: Give collection persister a reference to the owning class persister
			return GetEntity(new EntityKey(key, factory.GetEntityPersister(collectionPersister.OwnerClass)));
		}

		public IDbConnection Connection
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
		public bool IsConnected
		{
			get { return connectionManager.IsConnected; }
		}

		/// <summary></summary>
		public IDbConnection Disconnect()
		{
			CheckIsOpen();
			log.Debug("disconnecting session");
			return connectionManager.Disconnect();
		}

		public void Reconnect()
		{
			CheckIsOpen();
			log.Debug("reconnecting session");
			connectionManager.Reconnect();
		}

		public void Reconnect(IDbConnection conn)
		{
			CheckIsOpen();
			log.Debug("reconnecting session");
			connectionManager.Reconnect(conn);
		}

		#region System.IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Dispose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

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
			if (isDisposing)
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
			return Filter(collection, filter, qp);
		}

		public ICollection Filter(object collection, string filter, object value, IType type)
		{
			QueryParameters qp = new QueryParameters(new IType[] { null, type }, new object[] { null, value });
			return Filter(collection, filter, qp);
		}

		public ICollection Filter(object collection, string filter, object[] values, IType[] types)
		{
			CheckIsOpen();

			object[] vals = new object[values.Length + 1];
			IType[] typs = new IType[values.Length + 1];
			Array.Copy(values, 0, vals, 1, values.Length);
			Array.Copy(types, 0, typs, 1, types.Length);
			QueryParameters qp = new QueryParameters(typs, vals);
			return Filter(collection, filter, qp);
		}

		/// <summary>
		/// 1. determine the collection role of the given collection (this may require a flush, if the collection is recorded as unreferenced)
		/// 2. obtain a compiled filter query
		/// 3. autoflush if necessary
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		private IFilterTranslator GetFilterTranslator(object collection, string filter, QueryParameters parameters,
													  bool scalar)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection", "null collection passed to Filter");
			}

			if (log.IsDebugEnabled)
			{
				log.Debug("filter: " + filter);
				parameters.LogParameters(factory);
			}

			CollectionEntry entry = GetCollectionEntryOrNull(collection);
			ICollectionPersister roleBeforeFlush = (entry == null) ? null : entry.LoadedPersister;

			IFilterTranslator filterTranslator;
			if (roleBeforeFlush == null)
			{
				// if it was previously unreferenced, we need
				// to flush in order to get its state into the
				// database to query
				Flush();
				entry = GetCollectionEntryOrNull(collection);
				ICollectionPersister roleAfterFlush = (entry == null) ? null : entry.LoadedPersister;

				if (roleAfterFlush == null)
				{
					throw new QueryException("the collection was unreferenced");
				}
				filterTranslator = factory.GetFilter(filter, roleAfterFlush.Role, scalar);
			}
			else
			{
				// otherwise, we only need to flush if there are
				// in-memory changes to the queried tables
				filterTranslator = factory.GetFilter(filter, roleBeforeFlush.Role, scalar);
				if (AutoFlushIfRequired(filterTranslator.QuerySpaces))
				{
					// might need to run a different filter entirely after the flush
					// because the collection role may have changed
					entry = GetCollectionEntryOrNull(collection);
					ICollectionPersister roleAfterFlush = (entry == null) ? null : entry.LoadedPersister;
					if (roleBeforeFlush != roleAfterFlush)
					{
						if (roleAfterFlush == null)
						{
							throw new QueryException("The collection was dereferenced");
						}
					}
					filterTranslator = factory.GetFilter(filter, roleAfterFlush.Role, scalar);
				}
			}

			parameters.PositionalParameterValues[0] = entry.LoadedKey;
			parameters.PositionalParameterTypes[0] = entry.LoadedPersister.KeyType;

			return filterTranslator;
		}

		/// <summary>
		/// Get the collection entry for a collection passed to filter,
		/// which might be a collection wrapper, an array, or an unwrapped
		/// collection. Return <see langword="null" /> if there is no entry.
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		private CollectionEntry GetCollectionEntryOrNull(object collection)
		{
			IPersistentCollection coll;
			if (collection is IPersistentCollection)
			{
				coll = (IPersistentCollection)collection;
			}
			else
			{
				coll = GetArrayHolder(collection);
				if (coll == null)
				{
					// it might be an unwrapped collection reference!
					// try to find a wrapper (slowish)
					foreach (IPersistentCollection pc in collectionEntries.Keys)
					{
						if (pc.IsWrapper(collection))
						{
							coll = pc;
							break;
						}
					}
				}
			}

			return (coll == null) ? null : GetCollectionEntry(coll);
		}

		public void Filter(object collection, string filter, QueryParameters parameters, IList results)
		{
			string[] concreteFilters = QuerySplitter.ConcreteQueries(filter, factory);
			IFilterTranslator[] filters = new IFilterTranslator[concreteFilters.Length];

			for (int i = 0; i < concreteFilters.Length; i++)
			{
				filters[i] = GetFilterTranslator(
					collection,
					concreteFilters[i],
					parameters,
					false);
			}

			bool success = false;
			dontFlushFromFind++; // stops flush being called multiple times if this method is recursively called

			try
			{
				for (int i = filters.Length - 1; i >= 0; i--)
				{
					ArrayHelper.AddAll(results, filters[i].List(this, parameters));
				}
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

		public IList Filter(object collection, string filter, QueryParameters parameters)
		{
			ArrayList results = new ArrayList();
			Filter(collection, filter, parameters, results);
			return results;
		}

		public IList<T> Filter<T>(object collection, string filter, QueryParameters parameters)
		{
			List<T> results = new List<T>();
			Filter(collection, filter, parameters, results);
			return results;
		}

		public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
		{
			string[] concreteFilters = QuerySplitter.ConcreteQueries(filter, factory);
			IFilterTranslator[] filters = new IFilterTranslator[concreteFilters.Length];

			for (int i = 0; i < concreteFilters.Length; i++)
			{
				filters[i] = GetFilterTranslator(
					collection,
					concreteFilters[i],
					parameters,
					true);
			}

			if (filters.Length == 0)
			{
				return new ArrayList(0);
			}

			IEnumerable result = null;
			IEnumerable[] results = null;
			bool many = filters.Length > 1;
			if (many)
			{
				results = new IEnumerable[filters.Length];
			}

			// execute the queries and return all results as a single enumerable
			for (int i = 0; i < filters.Length; i++)
			{
				try
				{
					result = filters[i].GetEnumerable(parameters, this);
				}
				catch (Exception e)
				{
					throw new ADOException("could not execute query", e);
				}
				if (many)
				{
					results[i] = result;
				}
			}

			return many ? new JoinedEnumerable(results) : result;
		}

		public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			string[] concreteFilters = QuerySplitter.ConcreteQueries(filter, factory);
			IFilterTranslator[] filters = new IFilterTranslator[concreteFilters.Length];

			for (int i = 0; i < concreteFilters.Length; i++)
			{
				filters[i] = GetFilterTranslator(
					collection,
					concreteFilters[i],
					parameters,
					true);
			}

			if (filters.Length == 0)
			{
				return new List<T>(0);
			}

			IEnumerable[] results = new IEnumerable[filters.Length];

			// execute the queries and return all results as a single enumerable
			for (int i = 0; i < filters.Length; i++)
			{
				try
				{
					results[i] = filters[i].GetEnumerable(parameters, this);
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
			}

			return new GenericJoinedEnumerable<T>(results);
		}

		public ICriteria CreateCriteria(System.Type persistentClass)
		{
			CheckIsOpen();

			return new CriteriaImpl(persistentClass, this);
		}

		public ICriteria CreateCriteria(System.Type persistentClass, string alias)
		{
			CheckIsOpen();

			return new CriteriaImpl(persistentClass, alias, this);
		}

		public IList Find(CriteriaImpl criteria)
		{
			ArrayList results = new ArrayList();
			Find(criteria, results);
			return results;
		}

		public IList<T> Find<T>(CriteriaImpl criteria)
		{
			List<T> results = new List<T>();
			Find(criteria, results);
			return results;
		}

		public void Find(CriteriaImpl criteria, IList results)
		{
			// The body of this method is modified from H2.1 version, because the Java version
			// used factory.GetImplementors which returns a list of implementor class names
			// obtained from IEntityPersister.ClassName property.
			//
			// In Java, calling ReflectHelper.ClassForName( IEntityPersister.ClassName )
			// works, but in .NET it does not, because the class name does not include the assembly
			// name. .NET tries to load the class from NHibernate assembly and fails.
			//
			// The solution was to add SessionFactoryImpl.GetImplementorClasses method
			// which returns an array of System.Types instead of just class names.
			System.Type[] implementors = factory.GetImplementorClasses(criteria.CriteriaClass);
			int size = implementors.Length;

			CriteriaLoader[] loaders = new CriteriaLoader[size];
			ISet spaces = new HashedSet();

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

		internal IOuterJoinLoadable GetOuterJoinLoadable(System.Type clazz)
		{
			IEntityPersister persister = GetClassPersister(clazz);
			if (!(persister is IOuterJoinLoadable))
			{
				throw new MappingException("class persister is not IOuterJoinLoadable: " + clazz.FullName);
			}
			return (IOuterJoinLoadable)persister;
		}

		public bool Contains(object obj)
		{
			CheckIsOpen();

			if (obj is INHibernateProxy)
			{
				//do not use proxiesByKey, since not all
				//proxies that point to this session's
				//instances are in that collection!
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)obj);
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
			return IsEntryFor(obj);
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

		public object GetVersion(object entity)
		{
			return GetEntry(entity).Version;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collectionPersister"></param>
		/// <param name="id"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		public object[] GetCollectionBatch(ICollectionPersister collectionPersister, object id, int batchSize)
		{
			return BatchFetchQueue.GetCollectionBatch(collectionPersister, id, batchSize);
		}

		public object[] GetClassBatch(System.Type clazz, object id, int batchSize)
		{
			return BatchFetchQueue.GetEntityBatch(factory.GetEntityPersister(clazz), id, batchSize);
		}

		public void ScheduleBatchLoad(System.Type clazz, object id)
		{
			IEntityPersister persister = GetClassPersister(clazz);
			BatchFetchQueue.AddBatchLoadableEntityKey(new EntityKey(id, persister));
		}

		public IQuery CreateSQLQuery(string sql, string returnAlias, System.Type returnClass)
		{
			CheckIsOpen();

			return new SqlQueryImpl(sql, new string[] { returnAlias }, new System.Type[] { returnClass }, this, null);
		}

		public IQuery CreateSQLQuery(string sql, string[] returnAliases, System.Type[] returnClasses)
		{
			CheckIsOpen();

			return new SqlQueryImpl(sql, returnAliases, returnClasses, this, null);
		}

		public IQuery CreateSQLQuery(string sql, string[] returnAliases, System.Type[] returnClasses, ICollection querySpaces)
		{
			CheckIsOpen();

			return new SqlQueryImpl(sql, returnAliases, returnClasses, this, querySpaces);
		}

		public IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			ArrayList results = new ArrayList();
			List(spec, queryParameters, results);
			return results;
		}

		public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			List<T> results = new List<T>();
			List(spec, queryParameters, results);
			return results;
		}

		public void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
		{
			SQLCustomQuery query = new SQLCustomQuery(
				spec.SqlQueryReturns,
				spec.QueryString,
				spec.QuerySpaces,
				factory);
			ListCustomQuery(query, queryParameters, results);
		}

		public void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			CheckIsOpen();

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

		public IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
		{
			CheckIsOpen();

			CustomLoader loader = new CustomLoader(customQuery, factory);
			AutoFlushIfRequired(loader.QuerySpaces);

			dontFlushFromFind++;
			try
			{
				IList results = loader.List(this, queryParameters);
				List<T> typedResults = new List<T>();
				ArrayHelper.AddAll(typedResults, results);
				return typedResults;
			}
			finally
			{
				dontFlushFromFind--;
			}
		}

		/// <summary></summary>
		public void Clear()
		{
			CheckIsOpen();
			actionQueue.Clear();

			arrayHolders.Clear();
			entitiesByKey.Clear();
			entityEntries.Clear();
			collectionsByKey.Clear();
			collectionEntries.Clear();
			proxiesByKey.Clear();
			if (batchFetchQueue != null)
			{
				batchFetchQueue.Clear();
			}
			nonExists.Clear();

			nullifiables.Clear();

			if (unownedCollections != null)
			{
				unownedCollections.Clear();
			}
			hasNonReadOnlyEntities = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object LoadByUniqueKey(System.Type clazz, string uniqueKeyPropertyName, object id)
		{
			IUniqueKeyLoadable persister = (IUniqueKeyLoadable)Factory.GetEntityPersister(clazz);
			try
			{
				// TODO: Implement caching?! proxies?!
				object result = persister.LoadByUniqueKey(uniqueKeyPropertyName, id, this);
				return result == null ? null : ProxyFor(result);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "Error performing LoadByUniqueKey");
			}
		}

		public void Replicate(object obj, ReplicationMode replicationMode)
		{
			FireReplicate(new ReplicateEvent(obj, replicationMode, this));
		}

		public ISessionFactory SessionFactory
		{
			get { return factory; }
		}

		/// <summary>
		/// Instantiate a collection wrapper (called when loading an object)
		/// </summary>
		/// <param name="role"></param>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public object GetCollection(string role, object id, object owner)
		{
			// note: there cannot possibly be a collection already registered,
			// because this method is called while first loading the entity
			// that references it

			ICollectionPersister persister = factory.GetCollectionPersister(role);
			IPersistentCollection collection = GetLoadingCollection(persister, id);

			if (collection == null)
			{
				collection = UseUnownedCollection(new CollectionKey(persister, id));

				if (collection == null)
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("creating collection wrapper:" + MessageHelper.InfoString(persister, id));
					}
					//TODO: suck into CollectionPersister.instantiate()
					collection = persister.CollectionType.Instantiate(this, persister);
					collection.Owner = owner;
					AddUninitializedCollection(collection, persister, id);
					if (persister.IsArray)
					{
						InitializeCollection(collection, false);
						AddArrayHolder((PersistentArrayHolder)collection);
					}
					else if (!persister.IsLazy)
					{
						nonlazyCollections.Add(collection);
					}
				}
			}

			collection.Owner = owner;
			return collection.GetValue();
		}

		/// <summary>
		/// Try to initialize a Collection from the cache.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <param name="persister"></param>
		/// <param name="collection"></param>
		/// <returns><see langword="true" /> if the collection was initialized from the cache, otherwise <see langword="false" />.</returns>
		private bool InitializeCollectionFromCache(object id, object owner, ICollectionPersister persister,
												   IPersistentCollection collection)
		{
			//todo-events remove
			if (enabledFilters.Count > 0 && persister.IsAffectedByEnabledFilters(this))
			{
				log.Debug("disregarding cached version (if any) of collection due to enabled filters ");
				return false;
			}

			if (!persister.HasCache)
			{
				return false;
			}
			else
			{
				CacheKey ck = new CacheKey(id, persister.KeyType, persister.Role, factory);
				object cached = persister.Cache.Get(ck, Timestamp);
				if (cached == null)
				{
					return false;
				}
				else
				{
					collection.InitializeFromCache(persister, cached, owner);
					collection.AfterInitialize(persister);
					GetCollectionEntry(collection).PostInitialize(collection);
					//addInitializedCollection(collection, persister, id); h2.1 - commented out
					return true;
				}
			}
		}

		public void CancelQuery()
		{
			CheckIsOpen();

			Batcher.CancelLastQuery();
		}

		public void AddNonExist(EntityKey key)
		{
			nonExists.Add(key);
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

		public object Copy(object obj, IDictionary copiedAlready)
		{
			// TODO H3.2 Different behavior
			MergeEvent mergeEvent = new MergeEvent(null, obj, this);
			FireMerge(copiedAlready, mergeEvent);
			return mergeEvent.Result;
		}

		public object SaveOrUpdateCopy(object obj, object id)
		{
			return FireSaveOrUpdateCopy(new MergeEvent(null, obj, id, this));
		}

		private ADOException Convert(Exception sqlException, string message)
		{
			return ADOExceptionHelper.Convert( /*Factory.SQLExceptionConverter,*/ sqlException, message);
		}

		private void CheckIsOpen()
		{
			if (_isAlreadyDisposed || closed)
			{
				throw new ObjectDisposedException("ISession", "Session was disposed of or closed");
			}
		}

		public IFilter GetEnabledFilter(string filterName)
		{
			CheckIsOpen();
			return (IFilter)enabledFilters[filterName];
		}

		public IFilter EnableFilter(string filterName)
		{
			CheckIsOpen();
			FilterImpl filter = new FilterImpl(factory.GetFilterDefinition(filterName));
			if (enabledFilters[filterName] == null)
				enabledFilters.Add(filterName, filter);
			return filter;
		}

		public void DisableFilter(string filterName)
		{
			CheckIsOpen();
			enabledFilters.Remove(filterName);
		}

		public Object GetFilterParameterValue(string filterParameterName)
		{
			CheckIsOpen();
			string[] parsed = ParseFilterParameterName(filterParameterName);
			FilterImpl filter = (FilterImpl)enabledFilters[parsed[0]];
			if (filter == null)
			{
				throw new ArgumentException("Filter [" + parsed[0] + "] currently not enabled");
			}
			return filter.GetParameter(parsed[1]);
		}

		public IType GetFilterParameterType(string filterParameterName)
		{
			CheckIsOpen();
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

		public IDictionary EnabledFilters
		{
			get
			{
				CheckIsOpen();

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
				throw new ArgumentException("filterParameterName", "Invalid filter-parameter name format");
			}
			string filterName = filterParameterName.Substring(0, dot);
			string parameterName = filterParameterName.Substring(dot + 1);
			return new string[] { filterName, parameterName };
		}

		public ISQLQuery CreateSQLQuery(string sql)
		{
			SqlQueryImpl query = new SqlQueryImpl(sql, this);
			return query;
		}

		public BatchFetchQueue BatchFetchQueue
		{
			get
			{
				if (batchFetchQueue == null)
				{
					batchFetchQueue = new BatchFetchQueue(this);
				}
				return batchFetchQueue;
			}
		}

		public ConnectionManager ConnectionManager
		{
			get { return connectionManager; }
		}

		public bool ContainsEntity(EntityKey key)
		{
			return GetEntity(key) != null;
		}

		private void AddUnownedCollection(CollectionKey key, IPersistentCollection collection)
		{
			if (unownedCollections == null)
			{
				unownedCollections = new Hashtable(8);
			}

			unownedCollections[key] = collection;
		}

		private IPersistentCollection UseUnownedCollection(CollectionKey key)
		{
			if (unownedCollections == null)
			{
				return null;
			}

			IPersistentCollection result = (IPersistentCollection)unownedCollections[key];
			if (result != null)
			{
				unownedCollections.Remove(key);
			}

			return result;
		}

		public IMultiQuery CreateMultiQuery()
		{
			return new MultiQueryImpl(this);
		}

		public IMultiCriteria CreateMultiCriteria()
		{
			return new MultiCriteriaImpl(this, factory);
		}

		private void AfterOperation(bool success)
		{
			if (!connectionManager.IsInActiveTransaction)
			{
				connectionManager.AfterNonTransactionalQuery(success);
			}
		}

		public void AfterTransactionBegin(ITransaction tx)
		{
			CheckIsOpen();
			interceptor.AfterTransactionBegin(tx);
		}

		public void BeforeTransactionCompletion(ITransaction tx)
		{
			log.Debug("before transaction completion");
			//if ( rootSession == null ) {
			try
			{
				interceptor.BeforeTransactionCompletion(tx);
			}
			catch (Exception e)
			{
				log.Error("exception in interceptor BeforeTransactionCompletion()", e);
			}
			//}
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

		public IInterceptor Interceptor
		{
			get { return interceptor; }
		}

		/// <summary> Retrieves the configured event listeners from this event source. </summary>
		public EventListeners Listeners
		{
			get { return listeners; }
		}

		public int DontFlushFromFind
		{
			get { return dontFlushFromFind; }
		}

		#region Feature IPersistenceContext Members

		public static readonly object NoRow = new object();

		public IDictionary EntitiesByKey
		{
			get { return entitiesByKey; }
		}

		public IDictionary EntityEntries
		{
			get { return entityEntries; }
		}

		public IDictionary CollectionEntries
		{
			get { return collectionEntries; }
		}

		/// <summary> Get the mapping from collection key to collection instance</summary>
		public IDictionary CollectionsByKey
		{
			get { return collectionsByKey; }
		}


		public bool Flushing 
		{
			get { return flushing; }
			set { flushing = value; }
		}

		/// <summary> Called before cascading</summary>
		public int IncrementCascadeLevel()
		{
			return ++cascading;
		}

		/// <summary> Called after cascading</summary>
		public int DecrementCascadeLevel()
		{
			return --cascading;
		}

		public void CheckUniqueness(EntityKey key, object obj)
		{
			object entity = GetEntity(key);
			if (entity == obj)
			{
				throw new AssertionFailure("object already associated, but no entry was found");
			}
			if (entity != null)
			{
				throw new NonUniqueObjectException(key.Identifier, key.MappedClass);
			}
		}

		/// <summary> Adds an entity to the internal caches.</summary>
		public EntityEntry AddEntity(object entity, Status status, object[] loadedState, EntityKey entityKey, object version,
																 LockMode lockMode, bool existsInDatabase, IEntityPersister persister,
																 bool disableVersionIncrement, bool lazyPropertiesAreUnfetched)
		{
			AddEntity(entityKey, entity);
			return
				AddEntry(entity, status, loadedState, entityKey.Identifier, version, lockMode,
				         existsInDatabase, persister, disableVersionIncrement, lazyPropertiesAreUnfetched);
		}

		/// <summary> 
		/// Generates an appropriate EntityEntry instance and adds it to the event source's internal caches.
		/// </summary>
		public EntityEntry AddEntry(object entity, Status status, object[] loadedState, object id, object version,
																LockMode lockMode, bool existsInDatabase, IEntityPersister persister,
																bool disableVersionIncrement, bool lazyPropertiesAreUnfetched)
		{
			return
				AddEntry(entity, status, loadedState, id, version, lockMode, existsInDatabase, persister, disableVersionIncrement);
		}

		/// <summary>
		/// If the parameter <c>value</c> is an unitialized proxy then it will be reassociated
		/// with the session. 
		/// </summary>
		/// <param name="value">A persistable object, proxy, persistent collection or null</param>
		/// <returns>
		/// <see langword="true" /> when an uninitialized proxy was passed into this method, <see langword="false" /> otherwise.
		/// </returns>
		public bool ReassociateIfUninitializedProxy(object value)
		{
			if (!NHibernateUtil.IsInitialized(value))
			{
				INHibernateProxy proxy = (INHibernateProxy)value;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
				ReassociateProxy(li, proxy);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void ReplaceDelayedEntityIdentityInsertKeys(EntityKey oldKey, object generatedId)
		{
			object tempObject = entitiesByKey[oldKey];
			entitiesByKey.Remove(oldKey);
			object entity = tempObject;
			object tempObject2 = entityEntries[entity];
			entityEntries.Remove(entity);
			EntityEntry oldEntry = (EntityEntry)tempObject2;

			EntityKey newKey = new EntityKey(generatedId, oldEntry.Persister);
			AddEntity(newKey, entity);
			AddEntry(entity, oldEntry.Status, oldEntry.LoadedState, generatedId, oldEntry.Version, oldEntry.LockMode, oldEntry.ExistsInDatabase, oldEntry.Persister, oldEntry.IsBeingReplicated);
		}
		#endregion

		private void FireDelete(DeleteEvent @event)
		{
			CheckIsOpen();
			IDeleteEventListener[] deleteEventListener = listeners.DeleteEventListeners;
			for (int i = 0; i < deleteEventListener.Length; i++)
			{
				deleteEventListener[i].OnDelete(@event);
			}
		}

		private void FireDelete(DeleteEvent @event, ISet transientEntities)
		{
			CheckIsOpen();
			IDeleteEventListener[] deleteEventListener = listeners.DeleteEventListeners;
			for (int i = 0; i < deleteEventListener.Length; i++)
			{
				deleteEventListener[i].OnDelete(@event, transientEntities);
			}
		}

		private void FireEvict(EvictEvent evictEvent)
		{
			CheckIsOpen();
			IEvictEventListener[] evictEventListener = listeners.EvictEventListeners;
			for (int i = 0; i < evictEventListener.Length; i++)
			{
				evictEventListener[i].OnEvict(evictEvent);
			}
		}

		private void FireLoad(LoadEvent @event, LoadType loadType)
		{
			CheckIsOpen();
			ILoadEventListener[] loadEventListener = listeners.LoadEventListeners;
			for (int i = 0; i < loadEventListener.Length; i++)
			{
				loadEventListener[i].OnLoad(@event, loadType);
			}
		}

		private void FireLock(LockEvent lockEvent)
		{
			CheckIsOpen();
			ILockEventListener[] lockEventListener = listeners.LockEventListeners;
			for (int i = 0; i < lockEventListener.Length; i++)
			{
				lockEventListener[i].OnLock(lockEvent);
			}
		}

		private object FireMerge(MergeEvent @event)
		{
			CheckIsOpen();
			IMergeEventListener[] mergeEventListener = listeners.MergeEventListeners;
			for (int i = 0; i < mergeEventListener.Length; i++)
			{
				mergeEventListener[i].OnMerge(@event);
			}
			return @event.Result;
		}

		private void FireMerge(IDictionary copiedAlready, MergeEvent @event)
		{
			CheckIsOpen();
			IMergeEventListener[] mergeEventListener = listeners.MergeEventListeners;
			for (int i = 0; i < mergeEventListener.Length; i++)
			{
				mergeEventListener[i].OnMerge(@event, copiedAlready);
			}
		}

		private void FirePersist(IDictionary copiedAlready, PersistEvent @event)
		{
			CheckIsOpen();
			IPersistEventListener[] persistEventListener = listeners.PersistEventListeners;
			for (int i = 0; i < persistEventListener.Length; i++)
			{
				persistEventListener[i].OnPersist(@event, copiedAlready);
			}
		}

		private void FirePersist(PersistEvent @event)
		{
			CheckIsOpen();
			IPersistEventListener[] createEventListener = listeners.PersistEventListeners;
			for (int i = 0; i < createEventListener.Length; i++)
			{
				createEventListener[i].OnPersist(@event);
			}
		}

		private void FirePersistOnFlush(IDictionary copiedAlready, PersistEvent @event)
		{
			CheckIsOpen();
			IPersistEventListener[] persistEventListener = listeners.PersistOnFlushEventListeners;
			for (int i = 0; i < persistEventListener.Length; i++)
			{
				persistEventListener[i].OnPersist(@event, copiedAlready);
			}
		}

		private void FirePersistOnFlush(PersistEvent @event)
		{
			CheckIsOpen();
			IPersistEventListener[] createEventListener = listeners.PersistOnFlushEventListeners;
			for (int i = 0; i < createEventListener.Length; i++)
			{
				createEventListener[i].OnPersist(@event);
			}
		}

		private void FireRefresh(RefreshEvent refreshEvent)
		{
			CheckIsOpen();
			IRefreshEventListener[] refreshEventListener = listeners.RefreshEventListeners;
			for (int i = 0; i < refreshEventListener.Length; i++)
			{
				refreshEventListener[i].OnRefresh(refreshEvent);
			}
		}

		private void FireRefresh(IDictionary refreshedAlready, RefreshEvent refreshEvent)
		{
			CheckIsOpen();
			IRefreshEventListener[] refreshEventListener = listeners.RefreshEventListeners;
			for (int i = 0; i < refreshEventListener.Length; i++)
			{
				refreshEventListener[i].OnRefresh(refreshEvent, refreshedAlready);
			}
		}

		private void FireReplicate(ReplicateEvent @event)
		{
			CheckIsOpen();
			IReplicateEventListener[] replicateEventListener = listeners.ReplicateEventListeners;
			for (int i = 0; i < replicateEventListener.Length; i++)
			{
				replicateEventListener[i].OnReplicate(@event);
			}
		}

		private object FireSave(SaveOrUpdateEvent @event)
		{
			CheckIsOpen();
			ISaveOrUpdateEventListener[] saveEventListener = listeners.SaveEventListeners;
			for (int i = 0; i < saveEventListener.Length; i++)
			{
				saveEventListener[i].OnSaveOrUpdate(@event);
			}
			return @event.ResultId;
		}

		private void FireSaveOrUpdate(SaveOrUpdateEvent @event)
		{
			CheckIsOpen();
			ISaveOrUpdateEventListener[] saveOrUpdateEventListener = listeners.SaveOrUpdateEventListeners;
			for (int i = 0; i < saveOrUpdateEventListener.Length; i++)
			{
				saveOrUpdateEventListener[i].OnSaveOrUpdate(@event);
			}
		}

		private void FireSaveOrUpdateCopy(IDictionary copiedAlready, MergeEvent @event)
		{
			CheckIsOpen();
			IMergeEventListener[] saveOrUpdateCopyEventListener = listeners.SaveOrUpdateCopyEventListeners;
			for (int i = 0; i < saveOrUpdateCopyEventListener.Length; i++)
			{
				saveOrUpdateCopyEventListener[i].OnMerge(@event, copiedAlready);
			}
		}

		private object FireSaveOrUpdateCopy(MergeEvent @event)
		{
			CheckIsOpen();
			IMergeEventListener[] saveOrUpdateCopyEventListener = listeners.SaveOrUpdateCopyEventListeners;
			for (int i = 0; i < saveOrUpdateCopyEventListener.Length; i++)
			{
				saveOrUpdateCopyEventListener[i].OnMerge(@event);
			}
			return @event.Result;
		}

		private void FireUpdate(SaveOrUpdateEvent @event)
		{
			CheckIsOpen();
			ISaveOrUpdateEventListener[] updateEventListener = listeners.UpdateEventListeners;
			for (int i = 0; i < updateEventListener.Length; i++)
			{
				updateEventListener[i].OnSaveOrUpdate(@event);
			}
		}
	}
}
