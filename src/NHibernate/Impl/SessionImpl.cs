using System;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Iesi.Collections;

using log4net;

using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql.Classic;
using NHibernate.Id;
using NHibernate.Loader.Criteria;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;
#if NET_2_0
using System.Collections.Generic;
#endif
using NHibernate.Engine.Query;

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
	public sealed class SessionImpl : ISessionImplementor, ISerializable, IDeserializationCallback
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SessionImpl));

		[NonSerialized]
		private SessionFactoryImpl factory;

		private bool autoClose;
		private readonly long timestamp;

		/// <summary>
		/// Indicates if the Session has been closed.
		/// </summary>
		/// <value>
		/// <c>false</c> (by default) if the Session is Open and can be used, 
		/// <c>true</c> if the Session has had the methods <c>Close()</c> or
		/// <c>Dispose()</c> invoked.</value>
		private bool closed = false;

		private FlushMode flushMode = FlushMode.Auto;

		/// <summary>
		/// An <see cref="IDictionary"/> with the <see cref="EntityKey"/> as the key
		/// and an <see cref="Object"/> as the value.
		/// </summary>
		private readonly IDictionary entitiesByKey;

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

		private IInterceptor interceptor;

		[NonSerialized]
		private IDbConnection connection;

		/// <summary>
		/// A boolean indicating if the Session should automattically connect to the
		/// database - ie, open a new <see cref="IDbConnection"/> for the operation if
		/// <c>this.Connection==null</c>.
		/// </summary>
		/// <remarks>
		/// <p>
		/// This will be initialzed to <c>true</c> by the ctor if NHibernate is managing
		/// the Connections, <c>false</c> if the user passes in their own connection - 
		/// indicating they will be responsible for managing connections.
		/// </p>
		/// <p>
		/// This can also be set to <c>false</c> when NHibernate has opened the connection
		/// on its own and the Session has had the methods <c>Close()</c> or 
		/// <c>Disconnect()</c> invoked.
		/// </p>
		/// <p>
		/// This can also be set to <c>true</c> when the Session has had the method 
		/// <c>Reconnect()</c> invoked.
		/// </p>
		/// </remarks>
		[NonSerialized]
		private bool connect;

		[NonSerialized]
		private ITransaction transaction;

		// We keep scheduled insertions, deletions and updates in collections
		// and actually execute them as part of the flush() process. Actually,
		// not every Flush() ends in execution of the scheduled actions. Auto-
		// flushes initiated by a query execution might be "shortcircuited".

		// Object insertions and deletions have list semantics because they
		// must happen in the right order so as to respect referential integrity
		private ArrayList insertions;
		private ArrayList deletions;

		// updates are kept in a Map because successive flushes might need to add
		// extra, new changes for an object thats already scheduled for update.
		// Note: we *could* treat updates the same way we treat collection actions
		// (discarding them at the end of a "shortcircuited" auto-flush) and then
		// we would keep them in a list
		private ArrayList updates;

		// Actually the semantics of the next three are really "Bag"
		// Note that, unlike objects, collection insertions, updates,
		// deletions are not really remembered between flushes. We
		// just re-use the same Lists for convenience.
		private ArrayList collectionCreations;
		private ArrayList collectionUpdates;
		private ArrayList collectionRemovals;

		[NonSerialized]
		private ArrayList executions;

		// The collections we are currently loading
		[NonSerialized]
		private IDictionary loadingCollections = new Hashtable();

		[NonSerialized]
		private IList nonlazyCollections;

		// A set of entity keys that we predict might be needed for
		// loading soon
		[NonSerialized]
		private IDictionary batchLoadableEntityKeys; // actually, a Set
		private static readonly object Marker = new object();

		[NonSerialized]
		private int dontFlushFromFind = 0;

		[NonSerialized]
		private int cascading = 0;

		[NonSerialized]
		private int loadCounter = 0;

		[NonSerialized]
		private bool flushing;

		[NonSerialized]
		private IBatcher batcher;

		[NonSerialized]
		private IDictionary enabledFilters = new Hashtable();

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
			this.autoClose = info.GetBoolean("autoClose");
			this.timestamp = info.GetInt64("timestamp");

			this.factory = (SessionFactoryImpl) info.GetValue("factory", typeof(SessionFactoryImpl));

			this.entitiesByKey = (IDictionary) info.GetValue("entitiesByKey", typeof(IDictionary));
			// we did not actually serializing the IDictionary but instead the proxies in an arraylist
			//this.proxiesByKey = (IDictionary)info.GetValue( "proxiesByKey", typeof(IDictionary) );
			tmpProxiesKey = (ArrayList) info.GetValue("tmpProxiesKey", typeof(ArrayList));
			tmpProxiesProxy = (ArrayList) info.GetValue("tmpProxiesProxy", typeof(ArrayList));
			this.entityEntries = (IdentityMap) info.GetValue("entityEntries", typeof(IdentityMap));
			this.collectionEntries = (IdentityMap) info.GetValue("collectionEntries", typeof(IdentityMap));
			this.collectionsByKey = (IDictionary) info.GetValue("collectionsByKey", typeof(IDictionary));
			this.arrayHolders = (IdentityMap) info.GetValue("arrayHolders", typeof(IdentityMap));
			this.nonExists = (ISet) info.GetValue("nonExists", typeof(ISet));

			this.closed = info.GetBoolean("closed");
			this.flushMode = (FlushMode) info.GetValue("flushMode", typeof(FlushMode));

			this.nullifiables = (ISet) info.GetValue("nullifiables", typeof(ISet));
			this.interceptor = (IInterceptor) info.GetValue("interceptor", typeof(IInterceptor));

			this.insertions = (ArrayList) info.GetValue("insertions", typeof(ArrayList));
			this.deletions = (ArrayList) info.GetValue("deletions", typeof(ArrayList));
			this.updates = (ArrayList) info.GetValue("updates", typeof(ArrayList));

			this.collectionCreations = (ArrayList) info.GetValue("collectionCreations", typeof(ArrayList));
			this.collectionUpdates = (ArrayList) info.GetValue("collectionUpdates", typeof(ArrayList));
			this.collectionRemovals = (ArrayList) info.GetValue("collectionRemovals", typeof(ArrayList));

			//this.enabledFilters = (IDictionary) info.GetValue("enabledFilters", typeof(IDictionary));
			tmpEnabledFiltersKey = (ArrayList) info.GetValue("tmpEnabledFiltersKey", typeof(ArrayList));
			tmpEnabledFiltersValue = (ArrayList) info.GetValue("tmpEnabledFiltersValue", typeof(ArrayList));
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
		[SecurityPermissionAttribute(SecurityAction.LinkDemand,
									 Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			log.Debug("writting session to serializer");

			if (IsConnected)
			{
				throw new InvalidOperationException("Cannot serialize a Session while connected");
			}

			info.AddValue("factory", factory, typeof(SessionFactoryImpl));
			info.AddValue("autoClose", autoClose);
			info.AddValue("timestamp", timestamp);
			info.AddValue("closed", closed);
			info.AddValue("flushMode", flushMode);
			info.AddValue("entitiesByKey", entitiesByKey, typeof(IDictionary));

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

			info.AddValue("insertions", insertions);
			info.AddValue("deletions", deletions);
			info.AddValue("updates", updates);
			info.AddValue("collectionCreations", collectionCreations);
			info.AddValue("collectionRemovals", collectionRemovals);
			info.AddValue("collectionUpdates", collectionUpdates);

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
					((IPersistentCollection) e.Key).SetCurrentSession(this);
					CollectionEntry ce = (CollectionEntry) e.Value;
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
					NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) proxy).Session = this;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="factory"></param>
		/// <param name="autoClose"></param>
		/// <param name="timestamp"></param>
		/// <param name="interceptor"></param>
		internal SessionImpl(IDbConnection connection, SessionFactoryImpl factory, bool autoClose, long timestamp, IInterceptor interceptor)
		{
			if (interceptor == null)
				throw new ArgumentNullException("interceptor", "The interceptor can not be null");

			this.connection = connection;
			connect = connection == null;
			this.interceptor = interceptor;

			this.autoClose = autoClose;
			this.timestamp = timestamp;

			this.factory = factory;

			entitiesByKey = new Hashtable(50);
			proxiesByKey = new Hashtable(10);
			nonExists = new HashedSet();
			//TODO: hack with this cast
			entityEntries = (IdentityMap) IdentityMap.InstantiateSequenced(50);
			collectionEntries = (IdentityMap) IdentityMap.InstantiateSequenced(30);
			collectionsByKey = new Hashtable(30);
			arrayHolders = (IdentityMap) IdentityMap.Instantiate(10);

			insertions = new ArrayList(20);
			deletions = new ArrayList(20);
			updates = new ArrayList(20);
			collectionCreations = new ArrayList(20);
			collectionRemovals = new ArrayList(20);
			collectionUpdates = new ArrayList(20);

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
				// when the connection is null nothing needs to be done - if there
				// is a value for connection then Disconnect() was not called - so we
				// need to ensure it gets called.
				if (connection == null)
				{
					connect = false;
					return null;
				}
				else
				{
					return Disconnect();
				}
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
		/// <param name="success"></param>
		public void AfterTransactionCompletion(bool success)
		{
			transaction = null;
			log.Debug("transaction completion");

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
				catch (System.Exception e)
				{
					throw new AssertionFailure("Exception releasing cache locks", e);
				}
			}
			executions.Clear();
		}

		private void InitTransientState()
		{
			executions = new ArrayList(50);
			batchLoadableEntityKeys = new SequencedHashMap(30);
			loadingCollections = new Hashtable();
			nonlazyCollections = new ArrayList(20);

			batcher = SessionFactory.ConnectionProvider.Driver.CreateBatcher(this);
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
			proxiesByKey.Clear();
			entityEntries.Clear();
			arrayHolders.Clear();
			collectionEntries.Clear();
			nullifiables.Clear();
			batchLoadableEntityKeys.Clear();
			collectionsByKey.Clear();
			nonExists.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public LockMode GetCurrentLockMode(object obj)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "null object passed to GetCurrentLockMode");
			}
			if (obj is INHibernateProxy)
			{
				obj = (NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj)).GetImplementation(this);
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public LockMode GetLockMode(object entity)
		{
			return GetEntry(entity).LockMode;
		}

		private void AddEntity(EntityKey key, object obj)
		{
			entitiesByKey[key] = obj;
			if (key.IsBatchLoadable)
			{
				batchLoadableEntityKeys.Remove(key);
			}
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

		private object RemoveEntity(EntityKey key)
		{
			object retVal = entitiesByKey[key];
			entitiesByKey.Remove(key);
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
			EntityEntry e = new EntityEntry(status, loadedState, id, version, lockMode, existsInDatabase, persister, disableVersionIncrement);
			entityEntries[obj] = e;
			return e;
		}

		public EntityEntry GetEntry(object obj)
		{
			return (EntityEntry) entityEntries[obj];
		}

		private EntityEntry RemoveEntry(object obj)
		{
			object retVal = entityEntries[obj];
			entityEntries.Remove(obj);
			return (EntityEntry) retVal;
		}

		private bool IsEntryFor(object obj)
		{
			return entityEntries.Contains(obj);
		}

		public CollectionEntry GetCollectionEntry(IPersistentCollection coll)
		{
			return (CollectionEntry) collectionEntries[coll];
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
			CheckIsOpen();

			if (obj == null)
			{
				throw new NullReferenceException("attempted to save null");
			}

			object theObj = Unproxy(obj);

			EntityEntry e = GetEntry(theObj);
			if (e != null)
			{
				if (e.Status == Status.Deleted)
				{
					ForceFlush(e);
				}
				else
				{
					log.Debug("object already associated with session");
					return e.Id;
				}
			}

			//id might be generated by SQL insert
			object id = SaveWithGeneratedIdentifier(theObj, Cascades.CascadingAction.ActionSaveUpdate, null);

			// NH-552
			IEntityPersister persister = GetEntityPersister(theObj);
			nullifiables.Remove(new EntityKey(id, persister));

			ReassociateProxy(obj, id);
			return id;
		}

		private void ForceFlush(EntityEntry e)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("flushing to force deletion of re-saved object" + MessageHelper.InfoString(e.Persister, e.Id));
			}

			if (cascading > 0)
			{
				throw new ObjectDeletedException(
					"deleted object would be re-saved by cascade (remove deleted object from associations)",
					e.Id,
					e.Persister.MappedClass);
			}

			Flush();
		}

		private object SaveWithGeneratedIdentifier(object obj, Cascades.CascadingAction action, object anything)
		{
			IEntityPersister persister = GetEntityPersister(obj);
			try
			{
				object id = persister.IdentifierGenerator.Generate(this, obj);

				if (id == null)
				{
					throw new IdentifierGenerationException(string.Format("null id generated for: {0}", obj.GetType().FullName));
				}
				else if (id == IdentifierGeneratorFactory.ShortCircuitIndicator)
				{
					return GetIdentifier(obj); //yick!!
				}
				else if (id == IdentifierGeneratorFactory.IdentityColumnIndicator)
				{
					return DoSave(obj, null, persister, true, action, anything);
				}
				else
				{
					if (log.IsDebugEnabled)
					{
						log.Debug(string.Format("generated identifier: {0}", id));
					}
					return DoSave(obj, id, persister, false, action, anything);
				}
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception ex)
			{
				throw Convert(ex, "Could not save object");
			}
		}

		/// <summary>
		/// Save a transient object with a manually assigned ID
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Save(object obj, object id)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attemted to insert null");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id", "null identifier passed to Insert()");
			}

			object theObj = Unproxy(obj);

			EntityEntry e = GetEntry(theObj);
			if (e != null)
			{
				if (e.Status == Status.Deleted)
				{
					ForceFlush(e);
				}
				else
				{
					if (!id.Equals(e.Id))
					{
						throw new PersistentObjectException(
							"object passed to Save() was already persistent: " +
								MessageHelper.InfoString(e.Persister, id)
							);
					}
					log.Debug("object already associated with session");
				}
			}

			DoSave(theObj, id, GetEntityPersister(theObj), false, Cascades.CascadingAction.ActionSaveUpdate, null);

			ReassociateProxy(obj, id);
		}

		private object DoSave(
			object obj,
			object id,
			IEntityPersister persister,
			bool useIdentityColumn,
			Cascades.CascadingAction cascadeAction,
			object anything)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("saving " + MessageHelper.InfoString(persister, id));
			}

			EntityKey key;
			if (useIdentityColumn)
			{
				// if the id is generated by the database, we assign the key later
				key = null;
			}
			else
			{
				key = new EntityKey(id, persister);

				object old = GetEntity(key);
				if (old != null)
				{
					EntityEntry e = GetEntry(old);
					if (e.Status == Status.Deleted)
					{
						ForceFlush(e);
					}
					else
					{
						throw new NonUniqueObjectException(id, persister.MappedClass);
					}
				}

				persister.SetIdentifier(obj, id);
			}

			// Sub-insertions should occur before containing insertsion so
			// Try to do the callback not
			if (persister.ImplementsLifecycle)
			{
				log.Debug("calling OnSave()");
				if (((ILifecycle) obj).OnSave(this) == LifecycleVeto.Veto)
				{
					log.Debug("insertion vetoed by OnSave()");
					return id;
				}
			}

			return DoSave(obj, key, persister, false, useIdentityColumn, cascadeAction, anything);
		}

		private object DoSave(
			object theObj,
			EntityKey key,
			IEntityPersister persister,
			bool replicate,
			bool useIdentityColumn,
			Cascades.CascadingAction cascadeAction,
			object anything)
		{
			if (persister.ImplementsValidatable)
			{
				((IValidatable) theObj).Validate();
			}

			object id;
			if (useIdentityColumn)
			{
				id = null;
				ExecuteInserts();
			}
			else
			{
				id = key.Identifier;
			}

			// Put a placeholder in entries, so we don't recurse back to try and Save() the
			// same object again. QUESTION: Should this be done before OnSave() is called?
			// likewise, should it be done before OnUpdate()?
			AddEntry(theObj, Status.Saving, null, id, null, LockMode.Write, useIdentityColumn, persister, false); // okay if id is null here

			// cascade-save to many-to-one BEFORE the parent is saved
			cascading++;
			try
			{
				Cascades.Cascade(this, persister, theObj, cascadeAction, CascadePoint.CascadeBeforeInsertAfterDelete, anything);
			}
			finally
			{
				cascading--;
			}

			object[] values = persister.GetPropertyValues(theObj);
			IType[] types = persister.PropertyTypes;

			bool substitute = false;
			if (!replicate)
			{
				substitute = interceptor.OnSave(theObj, id, values, persister.PropertyNames, types);

				// Keep the existing version number in the case of replicate!
				if (persister.IsVersioned)
				{
					// IsUnsavedVersion bit below is NHibernate-specific
					substitute = Versioning.SeedVersion(
						values, persister.VersionProperty, persister.VersionType, persister.IsUnsavedVersion(values), this
						) || substitute;
				}
			}

			if (persister.HasCollections)
			{
				// TODO: make OnReplicateVisitor extend WrapVisitor
				if (replicate)
				{
					OnReplicateVisitor visitor = new OnReplicateVisitor(this, id);
					visitor.ProcessValues(values, types);
				}
				WrapVisitor visitor2 = new WrapVisitor(this);
				// substitutes into values by side-effect
				visitor2.ProcessValues(values, types);
				substitute = substitute || visitor2.IsSubstitutionRequired;
			}

			if (substitute)
			{
				persister.SetPropertyValues(theObj, values);
			}

			TypeFactory.DeepCopy(values, types, persister.PropertyUpdateability, values);
			NullifyTransientReferences(values, types, useIdentityColumn, theObj);
			CheckNullability(values, persister, false);

			if (useIdentityColumn)
			{
				ScheduledIdentityInsertion insert = new ScheduledIdentityInsertion(values, theObj, persister, this);
				Execute(insert);
				id = insert.GeneratedId;
				persister.SetIdentifier(theObj, id);
				key = new EntityKey(id, persister);
				CheckUniqueness(key, theObj);
			}

			object version = Versioning.GetVersion(values, persister);
			AddEntity(key, theObj);
			AddEntry(theObj, Status.Loaded, values, id, version, LockMode.Write, useIdentityColumn, persister, replicate);
			nonExists.Remove(key);

			if (!useIdentityColumn)
			{
				insertions.Add(new ScheduledInsertion(id, values, theObj, version, persister, this));
			}

			// cascade-save to collections AFTER the collection owner was saved
			cascading++;
			try
			{
				Cascades.Cascade(this, persister, theObj, cascadeAction, CascadePoint.CascadeAfterInsertBeforeDelete, anything);
			}
			finally
			{
				cascading--;
			}

			return id;
		}

		/// <summary>
		/// If the parameter <c>value</c> is an unitialized proxy then it will be reassociated
		/// with the session. 
		/// </summary>
		/// <param name="value">A persistable object, proxy, persistent collection or null</param>
		/// <returns>
		/// <c>true</c> when an uninitialized proxy was passed into this method, <c>false</c> otherwise.
		/// </returns>
		internal bool ReassociateIfUninitializedProxy(object value)
		{
			if (!NHibernateUtil.IsInitialized(value))
			{
				INHibernateProxy proxy = (INHibernateProxy) value;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
				ReassociateProxy(li, proxy);
				return true;
			}
			else
			{
				return false;
			}
		}

		private void ReassociateProxy(Object value, object id)
		{
			if (value is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy) value;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
				li.Identifier = id;
				ReassociateProxy(li, proxy);
			}
		}

		private object Unproxy(object maybeProxy)
		{
			if (maybeProxy is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy) maybeProxy;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
				if (li.IsUninitialized)
				{
					throw new PersistentObjectException(string.Format("object was an uninitialized proxy for: {0}", li.PersistentClass.Name));
				}
				return li.GetImplementation(); //unwrap the object 
			}
			else
			{
				return maybeProxy;
			}
		}

		private object UnproxyAndReassociate(object maybeProxy)
		{
			if (maybeProxy is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy) maybeProxy;
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

		private void NullifyTransientReferences(object[] values, IType[] types, bool earlyInsert, object self)
		{
			for (int i = 0; i < types.Length; i++)
			{
				values[i] = NullifyTransientReferences(values[i], types[i], earlyInsert, self);
			}
		}

		/// <summary>
		/// Return null if the argument is an "unsaved" entity (ie. one with no existing database row), 
		/// or the input argument otherwise. This is how Hibernate avoids foreign key constraint violations.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <param name="earlyInsert"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		private object NullifyTransientReferences(object value, IType type, bool earlyInsert, object self)
		{
			if (value == null)
			{
				return null;
			}
			else if (type.IsEntityType || type.IsAnyType)
			{
				return (IsUnsaved(value, earlyInsert, self)) ? null : value;
			}
			else if (type.IsComponentType)
			{
				IAbstractComponentType actype = (IAbstractComponentType) type;
				object[] subvalues = actype.GetPropertyValues(value, this);
				IType[] subtypes = actype.Subtypes;
				bool substitute = false;
				for (int i = 0; i < subvalues.Length; i++)
				{
					object replacement = NullifyTransientReferences(subvalues[i], subtypes[i], earlyInsert, self);
					if (replacement != subvalues[i])
					{
						substitute = true;
						subvalues[i] = replacement;
					}
				}
				if (substitute)
				{
					actype.SetPropertyValues(value, subvalues);
				}
				return value;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// determine if the object already exists in the database, using a "best guess"
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="earlyInsert"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		private bool IsUnsaved(object obj, bool earlyInsert, object self)
		{
			if (obj is INHibernateProxy)
			{
				// if its an uninitialized proxy, it can't be transient
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj);
				if (li.GetImplementation(this) == null)
				{
					return false;
					// ie we never have to null out a reference to an uninitialized proxy
				}
				else
				{
					//unwrap it
					obj = li.GetImplementation(this);
				}
			}

			// if it was a reference to self, don't need to nullify
			// unless we are using native id generation, in which
			// case we definitely need to nullify
			if (obj == self)
			{
				return earlyInsert;
			}

			// See if the entity is already bound to this session, if not look at the
			// entity identifier and assume that the entity is persistent if the
			// id is not "unsaved" (that is, we rely on foreign keys to keep
			// database integrity)
			EntityEntry e = GetEntry(obj);
			if (e == null)
			{
				return GetEntityPersister(obj).IsUnsaved(obj);
			}

			return e.Status == Status.Saving || (
				earlyInsert ? !e.ExistsInDatabase : nullifiables.Contains(new EntityKey(e.Id, e.Persister))
				);
		}

		/// <summary>
		/// Delete a persistent object
		/// </summary>
		/// <param name="obj"></param>
		public void Delete(object obj)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attempted to delete null");
			}

			obj = UnproxyAndReassociate(obj);

			EntityEntry entry = GetEntry(obj);
			IEntityPersister persister;
			if (entry == null)
			{
				log.Debug("deleting a transient instance");

				persister = GetEntityPersister(obj);
				object id = persister.GetIdentifier(obj);

				if (id == null)
				{
					throw new TransientObjectException("the transient instance passed to Delete() has a null identifier");
				}

				object old = GetEntity(new EntityKey(id, persister));

				if (old != null)
				{
					throw new NonUniqueObjectException(id, persister.MappedClass);
				}

				new OnUpdateVisitor(this, id).Process(obj, persister);

				AddEntity(new EntityKey(id, persister), obj);
				entry = AddEntry(
					obj,
					Status.Loaded,
					persister.GetPropertyValues(obj),
					id,
					persister.GetVersion(obj),
					LockMode.None,
					true,
					persister,
					false
					);
				// not worth worrying about the proxy
			}
			else
			{
				log.Debug("deleting a persistent instance");

				if (entry.Status == Status.Deleted || entry.Status == Status.Gone)
				{
					log.Debug("object was already deleted");
					return;
				}
				persister = entry.Persister;
			}

			if (!persister.IsMutable)
			{
				throw new HibernateException(
					"attempted to delete an object of immutable class: " +
						MessageHelper.InfoString(persister));
			}

			DoDelete(obj, entry, persister);
		}

		private void DoDelete(object obj, EntityEntry entry, IEntityPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("deleting " + MessageHelper.InfoString(persister, entry.Id));
			}

			IType[] propTypes = persister.PropertyTypes;

			object version = entry.Version;

			object[] loadedState;
			if (entry.LoadedState == null)
			{
				//ie the object came in from Update()
				loadedState = persister.GetPropertyValues(obj);
			}
			else
			{
				loadedState = entry.LoadedState;
			}
			entry.DeletedState = new object[loadedState.Length];
			TypeFactory.DeepCopy(loadedState, propTypes, persister.PropertyUpdateability, entry.DeletedState);

			interceptor.OnDelete(obj, entry.Id, entry.DeletedState, persister.PropertyNames, propTypes);

			entry.Status = Status.Deleted; // before cascade and Lifecycle callback, so we are circular-reference safe
			EntityKey key = new EntityKey(entry.Id, persister);

			IList deletionsByOnDelete = null;
			ISet nullifiablesAfterOnDelete = null;

			// do Lifecycle callback before cascades, since this can veto
			if (persister.ImplementsLifecycle)
			{
				ISet oldNullifiables = (ISet) nullifiables.Clone();
				ArrayList oldDeletions = (ArrayList) deletions.Clone();

				nullifiables.Add(key); //the deletion of the parent is actually executed BEFORE any deletion from onDelete()

				try
				{
					log.Debug("calling onDelete()");
					if (((ILifecycle) obj).OnDelete(this) == LifecycleVeto.Veto)
					{
						//rollback deletion
						entry.Status = Status.Loaded;
						entry.DeletedState = null;
						nullifiables = oldNullifiables;
						log.Debug("deletion vetoed by OnDelete()");
						return; // don't let it cascade
					}
				}
				catch (Exception)
				{
					//rollback deletion
					entry.Status = Status.Loaded;
					entry.DeletedState = null;
					nullifiables = oldNullifiables;
					throw;
				}

				//note, the following assumes that onDelete() didn't cause the session to be flushed! 
				// TODO: add a better check that it doesn't
				if (oldDeletions.Count > deletions.Count)
				{
					throw new HibernateException("session was flushed during onDelete()");
				}
				deletionsByOnDelete = Sublist(deletions, oldDeletions.Count, deletions.Count);
				deletions = oldDeletions;
				nullifiablesAfterOnDelete = nullifiables;
				nullifiables = oldNullifiables;
			}

			cascading++;
			try
			{
				// cascade-delete to collections "BEFORE" the collection owner is deleted
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionDelete, CascadePoint.CascadeAfterInsertBeforeDelete, null);
			}
			finally
			{
				cascading--;
			}

			NullifyTransientReferences(entry.DeletedState, propTypes, false, obj);
			// NH: commented out, seems illogical to check for nulls here
			//CheckNullability( entry.DeletedState, persister, true );
			nullifiables.Add(key);

			ScheduledDeletion delete = new ScheduledDeletion(entry.Id, version, obj, persister, this);
			deletions.Add(delete); // Ensures that containing deletions happen before sub-deletions

			if (persister.ImplementsLifecycle)
			{
				// after nullify, because we don't want to nullify references to subdeletions
				nullifiables.AddAll(nullifiablesAfterOnDelete);
				// after deletions.add(), to respect foreign key constraints
				deletions.AddRange(deletionsByOnDelete);
			}

			cascading++;
			try
			{
				// cascade-delete to many-to-one AFTER the parent was deleted
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionDelete, CascadePoint.CascadeBeforeInsertAfterDelete);
			}
			finally
			{
				cascading--;
			}
		}

		private static IList Sublist(IList list, int fromIx, int toIx)
		{
			IList newList = new ArrayList(toIx - fromIx);

			for (int i = fromIx; i < toIx; i++)
			{
				newList.Add(list[i]);
			}

			return newList;
		}

		/// <summary>
		/// Checks to see if there are any Properties that should not be null 
		/// are references to null or to a transient object.
		/// </summary>
		/// <param name="values">An object array of values that should be validated.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that describes which values can be null.</param>
		/// <param name="isUpdate">A <see cref="Boolean"/> indicating if this is an Update operation.</param>
		/// <exception cref="HibernateException">
		/// Thrown when a non-nullable property contains a value that would
		/// persist the value of null to the database.
		/// </exception>
		private static void CheckNullability(object[] values, IEntityPersister persister, bool isUpdate)
		{
			bool[] nullability = persister.PropertyNullability;
			bool[] checkability = isUpdate ? persister.PropertyUpdateability : persister.PropertyInsertability;

			for (int i = 0; i < values.Length; i++)
			{
				if (!nullability[i] && checkability[i] && values[i] == null)
				{
					throw new PropertyValueException(
						"not-null property references a null or transient value: ",
						persister.MappedClass,
						persister.PropertyNames[i]);
				}
			}
		}

		internal void RemoveCollection(ICollectionPersister role, object id)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("collection dereferenced while transient " + MessageHelper.InfoString(role, id));
			}
			// 2.1 comments this out
			/*
			if( role.HasOrphanDelete )
			{
				throw new HibernateException( "You may not dereference a collection with cascade=\"all-delete-orphan\"" );
			}
			*/
			collectionRemovals.Add(new ScheduledCollectionRemove(role, id, false, this));
		}

		private static bool IsCollectionSnapshotValid(ICollectionSnapshot snapshot)
		{
			return snapshot != null &&
				snapshot.Role != null &&
				snapshot.Key != null;
		}

		internal static bool IsOwnerUnchanged(ICollectionSnapshot snapshot, ICollectionPersister persister, object id)
		{
			return IsCollectionSnapshotValid(snapshot) &&
				persister.Role.Equals(snapshot.Role) &&
				id.Equals(snapshot.Key);
		}

		/// <summary>
		/// Reattach a detached (disassociated) initialized or uninitialized collection wrapper
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="snapshot"></param>
		internal void ReattachCollection(IPersistentCollection collection, ICollectionSnapshot snapshot)
		{
			if (collection.WasInitialized)
			{
				AddInitializedDetachedCollection(collection, snapshot);
			}
			else
			{
				if (!IsCollectionSnapshotValid(snapshot))
				{
					throw new HibernateException("could not reassociate uninitialized transient collection");
				}
				AddUninitializedDetachedCollection(
					collection,
					GetCollectionPersister(snapshot.Role),
					snapshot.Key);
			}
		}

		public void Update(object obj)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attempted to update null");
			}

			if (ReassociateIfUninitializedProxy(obj))
			{
				return;
			}

			object theObj = UnproxyAndReassociate(obj);

			IEntityPersister persister = GetEntityPersister(theObj);

			if (IsEntryFor(theObj))
			{
				log.Debug("object already associated with session");
				// do nothing
			}
			else
			{
				// the object is transient
				object id = persister.GetIdentifier(theObj);

				if (id == null)
				{
					// assume this is a newly instantiated transient object 
					throw new HibernateException("The given object has a null identifier property " + MessageHelper.InfoString(persister));
				}
				else
				{
					DoUpdate(theObj, id, persister);
				}
			}
		}

		public void SaveOrUpdate(object obj)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attempted to update null");
			}

			if (ReassociateIfUninitializedProxy(obj))
			{
				return;
			}

			object theObj = UnproxyAndReassociate(obj); //a proxy is always "update", never "save"

			EntityEntry e = GetEntry(theObj);
			if (e != null && e.Status != Status.Deleted)
			{
				// do nothing for persistent instances
				log.Debug("SaveOrUpdate() persistent instance");
			}
			else if (e != null)
			{
				//ie status==DELETED
				log.Debug("SaveOrUpdate() deleted instance");
				Save(obj);
			}
			else
			{
				// the object is transient
				object isUnsaved = interceptor.IsUnsaved(theObj);
				IEntityPersister persister = GetEntityPersister(theObj);
				if (isUnsaved == null)
				{
					// use unsaved-value
					if (persister.IsUnsaved(theObj))
					{
						log.Debug("SaveOrUpdate() unsaved instance");
						Save(obj);
					}
					else
					{
						object id = persister.GetIdentifier(theObj);
						if (log.IsDebugEnabled)
						{
							log.Debug("SaveOrUpdate() previously saved instance with id: " + id);
						}
						DoUpdate(theObj, id, persister);
					}
				}
				else
				{
					if (true.Equals(isUnsaved))
					{
						log.Debug("SaveOrUpdate() unsaved instance");
						Save(obj);
					}
					else
					{
						log.Debug("SaveOrUpdate() previously saved instance");
						DoUpdate(theObj, persister.GetIdentifier(theObj), persister);
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Update(object obj, object id)
		{
			CheckIsOpen();

			if (id == null)
			{
				throw new ArgumentNullException("id", "null is not a valid identifier");
			}
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attempted to update null");
			}

			if (obj is INHibernateProxy)
			{
				NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj).Identifier = id;
			}

			if (ReassociateIfUninitializedProxy(obj))
			{
				return;
			}

			object theObj = UnproxyAndReassociate(obj);

			EntityEntry e = GetEntry(theObj);
			if (e == null)
			{
				IEntityPersister persister = GetEntityPersister(theObj);
				persister.SetIdentifier(theObj, id);
				DoUpdate(theObj, id, persister);
			}
			else
			{
				if (!e.Id.Equals(id))
				{
					throw new PersistentObjectException(
						"The instance passed to Update() was already persistent: " +
							MessageHelper.InfoString(e.Persister, id)
						);
				}
			}
		}

		private void DoUpdateMutable(object obj, object id, IEntityPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("updating " + MessageHelper.InfoString(persister, id));
			}

			EntityKey key = new EntityKey(id, persister);
			CheckUniqueness(key, obj);

			if (persister.ImplementsLifecycle)
			{
				log.Debug("calling onUpdate()");
				if (((ILifecycle) obj).OnUpdate(this) == LifecycleVeto.Veto)
				{
					log.Debug("update vetoed by onUpdate()");
					Reassociate(obj, id, persister);
					return;
				}
			}

			// this is a transient object with existing persistent state not loaded by the session

			new OnUpdateVisitor(this, id).Process(obj, persister);

			AddEntity(key, obj);
			AddEntry(obj, Status.Loaded, null, id, persister.GetVersion(obj), LockMode.None, true, persister, false);
		}

		private void DoUpdate(object obj, object id, IEntityPersister persister)
		{
			if (!persister.IsMutable)
			{
				log.Debug("immutable instance passed to doUpdate(), locking");
				Reassociate(obj, id, persister);
			}
			else
			{
				DoUpdateMutable(obj, id, persister);
			}

			cascading++;
			try
			{
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeOnUpdate, null); // do cascade
			}
			finally
			{
				cascading--;
			}
		}

		/// <summary>
		/// Used only by Replicate
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="replicationMode"></param>
		/// <param name="persister"></param>
		private void DoReplicate(object obj, object id, object version, ReplicationMode replicationMode, IEntityPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("replicating changes to " + MessageHelper.InfoString(persister, id));
			}

			new OnReplicateVisitor(this, id).Process(obj, persister);
			EntityKey key = new EntityKey(id, persister);
			AddEntity(key, obj);
			AddEntry(obj, Status.Loaded, null, id, version, LockMode.None, true, persister, true);

			cascading++;
			try
			{
				// do cascade
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionReplicate, CascadePoint.CascadeOnUpdate, replicationMode);
			}
			finally
			{
				cascading--;
			}
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

#if NET_2_0
		public IList<T> Find<T>(string query, QueryParameters parameters)
		{
			List<T> results = new List<T>();
			Find(query, parameters, results);
			return results;
		}
#endif

		public void Find(string query, QueryParameters parameters, IList results)
		{
			CheckIsOpen();

			if (log.IsDebugEnabled)
			{
				log.Debug("find: " + query);
				parameters.LogParameters(factory);
			}

			parameters.ValidateParameters();

			QueryTranslator[] q = GetQueries(query, false);

			dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called

			//execute the queries and return all result lists as a single list
			try
			{
				for (int i = q.Length - 1; i >= 0; i--)
				{
					ArrayHelper.AddAll(results, q[i].List(this, parameters));
				}
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
			}
		}

		private QueryTranslator[] GetQueries(string query, bool scalar)
		{
			// take the union of the query spaces (ie the queried tables)
			QueryTranslator[] q = factory.GetQuery(query, scalar);
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

#if NET_2_0
		public IEnumerable<T> Enumerable<T>(string query, QueryParameters parameters)
		{
			CheckIsOpen();

			if (log.IsDebugEnabled)
			{
				log.Debug("GetEnumerable: " + query);
				parameters.LogParameters(factory);
			}

			QueryTranslator[] q = GetQueries(query, true);

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
#endif

		public IEnumerable Enumerable(string query, QueryParameters parameters)
		{
			CheckIsOpen();

			if (log.IsDebugEnabled)
			{
				log.Debug("GetEnumerable: " + query);
				parameters.LogParameters(factory);
			}

			QueryTranslator[] q = GetQueries(query, true);

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

		private void CheckUniqueness(EntityKey key, object obj)
		{
			object entity = GetEntity(key);
			if (entity == obj)
			{
				throw new AssertionFailure("object already associated in DoSave()");
			}
			if (entity != null)
			{
				throw new NonUniqueObjectException(key.Identifier, key.MappedClass);
			}
		}

		private EntityEntry Reassociate(object obj, object id, IEntityPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("reassociating transient instance: " + MessageHelper.InfoString(persister, id));
			}
			EntityKey key = new EntityKey(id, persister);
			CheckUniqueness(key, obj);
			AddEntity(key, obj);
			Object[] values = persister.GetPropertyValues(obj);
			TypeFactory.DeepCopy(values, persister.PropertyTypes, persister.PropertyUpdateability, values);
			object version = Versioning.GetVersion(values, persister);
			EntityEntry newEntry = AddEntry(obj, Status.Loaded, values, id, version, LockMode.None, true, persister, false);
			new OnLockVisitor(this, id).Process(obj, persister);
			return newEntry;
		}

		public void Lock(object obj, LockMode lockMode)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attempted to lock null");
			}

			if (lockMode == LockMode.Write)
			{
				throw new HibernateException("Invalid lock mode for Lock()");
			}

			if (lockMode == LockMode.None && ReassociateIfUninitializedProxy(obj))
			{
				// NH-specific: shortcut for uninitialized proxies - reassociate
				// without initialization
				return;
			}

			obj = UnproxyAndReassociate(obj);
			//TODO: if object was an uninitialized proxy, this is inefficient, 
			//resulting in two SQL selects 

			EntityEntry e = GetEntry(obj);
			if (e == null)
			{
				IEntityPersister objPersister = GetEntityPersister(obj);
				object id = objPersister.GetIdentifier(obj);

				if (!IsSaved(obj))
				{
					throw new TransientObjectException(
						"cannot lock an unsaved transient instance: "
							+ MessageHelper.InfoString(objPersister));
				}

				e = Reassociate(obj, id, objPersister);

				cascading++;
				try
				{
					Cascades.Cascade(this, objPersister, obj, Cascades.CascadingAction.ActionLock,
									  CascadePoint.CascadeOnLock, lockMode);
				}
				finally
				{
					cascading--;
				}
			}

			UpgradeLock(obj, e, lockMode);
		}

		private void UpgradeLock(object obj, EntityEntry entry, LockMode lockMode)
		{
			if (lockMode.GreaterThan(entry.LockMode))
			{
				if (entry.Status != Status.Loaded)
				{
					throw new ObjectDeletedException("attempted to lock a deleted instance", entry.Id, obj.GetType());
				}

				IEntityPersister persister = entry.Persister;

				if (log.IsDebugEnabled)
				{
					log.Debug("locking " + MessageHelper.InfoString(persister, entry.Id) + " in mode: " + lockMode);
				}

				ISoftLock myLock = null;
				CacheKey ck = null;
				if (persister.HasCache)
				{
					ck = new CacheKey(
						entry.Id,
						persister.IdentifierType,
						(string)persister.IdentifierSpace,
						Factory
					);
					myLock = persister.Cache.Lock(ck, entry.Version);
				}
				try
				{
					persister.Lock(entry.Id, entry.Version, obj, lockMode, this);
					entry.LockMode = lockMode;
				}
				finally
				{
					// the database now holds a lock + the object is flushed from the cache,
					// so release the soft lock
					if (persister.HasCache)
					{
						persister.Cache.Release(ck, myLock);
					}
				}
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
						this//,
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
						this//,
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
				this//,
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

		/// <summary></summary>
		public FlushMode FlushMode
		{
			get { return flushMode; }
			set { flushMode = value; }
		}

		/// <summary>
		/// detect in-memory changes, determine if the changes are to tables
		/// named in the query and, if so, complete execution the flush
		/// </summary>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
		private bool AutoFlushIfRequired(ISet querySpaces)
		{
			if (flushMode == FlushMode.Auto && dontFlushFromFind == 0)
			{
				int oldSize = collectionRemovals.Count;

				FlushEverything();

				if (AreTablesToBeUpdated(querySpaces))
				{
					log.Debug("Need to execute flush");

					Execute();
					PostFlush();
					// note: Execute() clears all collectionXxxxtion collections
					return true;
				}
				else
				{
					log.Debug("dont need to execute flush");

					// sort of don't like this: we re-use the same collections each flush
					// even though their state is not kept between flushes. However, its
					// nice for performance since the collection sizes will be "nearly"
					// what we need them to be next time.
					collectionCreations.Clear();
					collectionUpdates.Clear();
					updates.Clear();
					// collection deletes are a special case since Update() can add
					// deletions of collections not loaded by the session.
					for (int i = collectionRemovals.Count - 1; i >= oldSize; i--)
					{
						collectionRemovals.RemoveAt(i);
					}
				}
			}

			return false;
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
					LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) proxy);
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
			CheckIsOpen();

			if (id == null)
			{
				throw new ArgumentNullException("id", "null is not a valid identifier");
			}
			DoLoadByObject(obj, id, LockMode.None);
		}

		public object Load(System.Type clazz, object id)
		{
			CheckIsOpen();

			if (id == null)
			{
				throw new ArgumentNullException("id", "null is not a valid identifier");
			}
			object result = DoLoadByClass(clazz, id, true, true);
			ObjectNotFoundException.ThrowIfNull(result, id, clazz);
			return result;
		}

#if NET_2_0
		public T Load<T>(object id)
		{
			return (T) Load(typeof(T), id);
		}

		public T Load<T>(object id, LockMode lockMode)
		{
			return (T) Load(typeof(T), id, lockMode);
		}

		public T Get<T>(object id)
		{
			return (T) Get(typeof(T), id);
		}

		public T Get<T>(object id, LockMode lockMode)
		{
			return (T) Get(typeof(T), id, lockMode);
		}
#endif

		public object Get(System.Type clazz, object id)
		{
			CheckIsOpen();

			if (id == null)
			{
				throw new ArgumentNullException("id", "null is not a valid identifier");
			}
			object result = DoLoadByClass(clazz, id, true, false);
			return result;
		}

		///<summary> 
		/// Load the data for the object with the specified id into a newly created object.
		/// Do NOT return a proxy.
		///</summary>
		public object ImmediateLoad(System.Type clazz, object id)
		{
			object result = DoLoad(clazz, id, null, LockMode.None, false);
			//ObjectNotFoundException.ThrowIfNull(result, id, clazz); (NH-268)break the NH-467 test but work like H3  
			return result;
		}

		/// <summary>
		/// Return the object with the specified id or throw exception if no row with that id exists. Defer the load,
		/// return a new proxy or return an existing proxy if possible. Do not check if the object was deleted.
		/// </summary>
		public object InternalLoad(System.Type clazz, object id, bool isNullable)
		{
			object result = DoLoadByClass(clazz, id, false, true);
			if (!isNullable)
			{
				UnresolvableObjectException.ThrowIfNull(result, id, clazz);
			}
			return result;
		}

		/// <summary>
		/// Load the data for the object with the specified id into the supplied
		/// instance. A new key will be assigned to the object. If there is an
		/// existing uninitialized proxy, this will break identity equals as far
		/// as the application is concerned.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		private void DoLoadByObject(object obj, object id, LockMode lockMode)
		{
			System.Type clazz = obj.GetType();
			if (GetEntry(obj) != null)
			{
				throw new PersistentObjectException(
					"attempted to load into an instance that was already associated with the Session: " +
						MessageHelper.InfoString(clazz, id)
					);
			}
			object result = DoLoad(clazz, id, obj, lockMode, true);
			ObjectNotFoundException.ThrowIfNull(result, id, clazz);
			if (result != obj)
			{
				throw new NonUniqueObjectException(id, clazz);
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created
		/// object. A new key will be assigned to the object. If the class supports
		/// lazy initialization, return a proxy instead, leaving the real work for
		/// later. This should return an existing proxy where appropriate.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> of the object to load.</param>
		/// <param name="id">The identifier of the object in the database.</param>
		/// <param name="checkDeleted">
		/// A boolean indicating if NHiberate should check if the object has or has not been deleted.
		/// </param>
		/// <param name="allowProxyCreation">A boolean indicating if it is allowed to return a Proxy instead of an instance of the <see cref="System.Type"/>.</param>
		/// <returns>
		/// An loaded instance of the object or a proxy of the object is proxies are allowed.
		/// </returns>
		/// <remarks>
		/// If the parameter <c>checkDeleted</c> is <c>false</c> it is possible to return an object that has 
		/// been deleted by the user in this <see cref="ISession"/>.  If the parameter <c>checkDeleted</c> is
		/// <c>true</c> and the object has been deleted then an <see cref="ObjectDeletedException"/> will be
		/// thrown.
		/// </remarks>
		private object DoLoadByClass(System.Type clazz, object id, bool checkDeleted, bool allowProxyCreation)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("loading " + MessageHelper.InfoString(clazz, id));
			}

			IEntityPersister persister = GetClassPersister(clazz);
			if (!persister.HasProxy)
			{
				// this class has no proxies (so do a shortcut)
				return DoLoad(clazz, id, null, LockMode.None, checkDeleted);
			}
			else
			{
				EntityKey key = new EntityKey(id, persister);
				object proxy;

				if (GetEntity(key) != null)
				{
					// return existing object or initialized proxy (unless deleted)
					return ProxyFor(
						persister,
						key,
						DoLoad(clazz, id, null, LockMode.None, checkDeleted)
						);
				}
				else if ((proxy = proxiesByKey[key]) != null)
				{
					object impl = null;
					if (!allowProxyCreation)
					{
						impl = DoLoad(clazz, id, null, LockMode.None, checkDeleted);
						ObjectNotFoundException.ThrowIfNull(impl, id, clazz);
					}
					return NarrowProxy(proxy, persister, key, impl);
				}
				else if (allowProxyCreation)
				{
					// return new uninitailzed proxy
					proxy = persister.CreateProxy(id, this);
					if (persister.IsBatchLoadable)
					{
						batchLoadableEntityKeys[key] = Marker;
					}
					proxiesByKey[key] = proxy;
					return proxy;
				}
				else
				{
					// return a newly loaded object
					return DoLoad(clazz, id, null, LockMode.None, checkDeleted);
				}
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This method always hits the db, and does not create proxies. It should return
		/// an existing proxy where appropriate.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <param name="allowNull"></param>
		/// <returns></returns>
		private object DoLoad(System.Type clazz, object id, LockMode lockMode, bool allowNull)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id", "null is not a valid identifier");
			}
			if (log.IsDebugEnabled)
			{
				log.Debug("loading " + MessageHelper.InfoString(clazz, id) + " in lock mode: " + lockMode);
			}

			IEntityPersister persister = GetClassPersister(clazz);
			ISoftLock myLock = null;
			CacheKey ck = null;
			if (persister.HasCache)
			{
				ck = new CacheKey(
					id,
					persister.IdentifierType,
					(string)persister.IdentifierSpace,
					Factory
				);
				//increments the lock
				myLock = persister.Cache.Lock(ck, null);
			}
			object result;
			try
			{
				result = DoLoad(clazz, id, null, lockMode, true);
			}
			finally
			{
				// the datbase now hold a lock + the object is flushed from the cache,
				// so release the soft lock
				if (persister.HasCache)
				{
					persister.Cache.Release(ck, myLock);
				}
			}

			if (!allowNull)
			{
				ObjectNotFoundException.ThrowIfNull(result, id, persister.MappedClass);
			}

			// return existing proxy (if one exists)
			return ProxyFor(persister, new EntityKey(id, persister), result);
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
			CheckIsOpen();

			if (lockMode == LockMode.Write)
			{
				throw new HibernateException("Invalid lock mode for Load()");
			}

			if (lockMode == LockMode.None)
			{
				// we don't necessarily need to hit the db in this case
				return Load(clazz, id);
			}

			return DoLoad(clazz, id, lockMode, false);
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
			CheckIsOpen();

			if (lockMode == LockMode.Write)
			{
				throw new HibernateException("Invalid lock mode for Get()");
			}

			if (lockMode == LockMode.None)
			{
				// we don't necessarily need to hit the db in this case
				return Get(clazz, id);
			}

			return DoLoad(clazz, id, lockMode, true);
		}

		/// <summary>
		/// Actually do all the hard work of loading up an object
		/// </summary>
		/// <param name="theClass"></param>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="lockMode"></param>
		/// <param name="checkDeleted"></param>
		/// <returns></returns>
		/// <remarks>
		/// 1. see if it is already loaded
		/// 2. see if it is cached
		/// 3. actually go to the database
		/// </remarks>
		private object DoLoad(System.Type theClass, object id, object optionalObject, LockMode lockMode, bool checkDeleted)
		{
			//DONT need to flush before a load by id, because ids are constant

			if (log.IsDebugEnabled)
			{
				log.Debug("attempting to resolve " + MessageHelper.InfoString(theClass, id));
			}

			IEntityPersister persister = GetClassPersister(theClass);
			EntityKey key = new EntityKey(id, persister);

			if (optionalObject != null)
			{
				persister.SetIdentifier(optionalObject, id);
			}

			// LOOK FOR LOADED OBJECT 
			// Look for Status.Loaded object
			object old = GetEntity(key);
			if (old != null)
			{
				//if this object was already loaded
				EntityEntry oldEntry = GetEntry(old);
				Status status = oldEntry.Status;
				if (checkDeleted && (status == Status.Deleted || status == Status.Gone))
				{
					throw new ObjectDeletedException("The object with that id was deleted", id, theClass);
				}
				UpgradeLock(old, oldEntry, lockMode);
				if (log.IsDebugEnabled)
				{
					log.Debug("resolved object in session cache " + MessageHelper.InfoString(persister, id));
				}
				return old;

			}
			else
			{
				// check to see if we know already that it does not exist:
				if (nonExists.Contains(key))
				{
					log.Debug("entity does not exist");
					return null;
				}

				// LOOK IN CACHE
				CacheKey ck = new CacheKey(id, persister.IdentifierType, (string) persister.IdentifierSpace, factory);
				CacheEntry entry = persister.HasCache && lockMode.LessThan(LockMode.Read) ?
					(CacheEntry) persister.Cache.Get(ck, Timestamp) :
					null;

				if (entry != null)
				{
					return AssembleCacheEntry(entry, id, persister, optionalObject);
				}
				else
				{
					//GO TO DATABASE
					if (log.IsDebugEnabled)
					{
						log.Debug("object not resolved in any cache " + MessageHelper.InfoString(persister, id));
					}
					object result = persister.Load(id, optionalObject, lockMode, this);
					if (result == null)
					{
						// remember it doesn't exist, in case of next time
						AddNonExist(key);
					}
					return result;
				}
			}
		}

		private object AssembleCacheEntry(CacheEntry entry, object id, IEntityPersister persister, object optionalObject)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("resolved object in second-level cache " + MessageHelper.InfoString(persister, id));
			}
			IEntityPersister subclassPersister = GetClassPersister(entry.Subclass);
			object result = optionalObject != null ? optionalObject : Instantiate(subclassPersister, id);
			AddEntry(result, Status.Loading, null, id, null, LockMode.None, true, subclassPersister, false);
			AddEntity(new EntityKey(id, persister), result);

			IType[] types = subclassPersister.PropertyTypes;
			object[] values = entry.Assemble(result, id, subclassPersister, interceptor, this); // intializes result by side-effect

			TypeFactory.DeepCopy(values, types, subclassPersister.PropertyUpdateability, values);
			object version = Versioning.GetVersion(values, subclassPersister);

			if (log.IsDebugEnabled)
			{
				log.Debug("Cached Version: " + version);
			}

			AddEntry(result, Status.Loaded, values, id, version, LockMode.None, true, subclassPersister, false);
			InitializeNonLazyCollections();

			// upgrade lock if necessary;
			//Lock( result, lockMode );

			return result;
		}

		public void Refresh(object obj)
		{
			Refresh(obj, LockMode.Read);
		}

		public void Refresh(object obj, LockMode lockMode)
		{
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attempted to refresh null");
			}

			if (ReassociateIfUninitializedProxy(obj))
			{
				return;
			}

			object theObj = UnproxyAndReassociate(obj);
			EntityEntry e = RemoveEntry(theObj);
			IEntityPersister persister;
			object id;

			if (e == null)
			{
				persister = GetEntityPersister(theObj);
				id = persister.GetIdentifier(theObj);
				if (log.IsDebugEnabled)
				{
					log.Debug("refreshing transient " + MessageHelper.InfoString(persister, id));
				}

				if (GetEntry(new EntityKey(id, persister)) != null)
				{
					throw new PersistentObjectException(
						"attempted to refresh transient instance when persistent instance was already associated with the Session: " +
						MessageHelper.InfoString(persister, id));
				}
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("refreshing " + MessageHelper.InfoString(e.Persister, e.Id));
				}

				if (!e.ExistsInDatabase)
				{
					throw new HibernateException("this instance does not yet exist as a row in the database");
				}

				persister = e.Persister;
				id = e.Id;
				EntityKey key = new EntityKey(id, persister);
				RemoveEntity(key);
				if (persister.HasCollections)
				{
					new EvictVisitor(this).Process(obj, persister);
				}
			}

			if (persister.HasCache)
			{
				CacheKey ck = new CacheKey(
					id,
					persister.IdentifierType,
					(string)persister.IdentifierSpace,
					Factory
				);
				persister.Cache.Remove(ck);
			}

			EvictCachedCollections(persister, id);
			object result = persister.Load(id, theObj, lockMode, this);
			UnresolvableObjectException.ThrowIfNull(result, id, persister.MappedClass);
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
				((ILifecycle) obj).OnLoad(this, id);
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
			transaction = Transaction;
			transaction.Begin(isolationLevel);
			return transaction;
		}

		public ITransaction BeginTransaction()
		{
			CheckIsOpen();
			transaction = Transaction;
			transaction.Begin();
			return transaction;
		}

		public ITransaction Transaction
		{
			get
			{
				if (transaction == null)
				{
					transaction = factory.TransactionFactory.CreateTransaction(this);
				}
				return transaction;
			}
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

			if (cascading > 0)
			{
				throw new HibernateException("Flush during cascade is dangerous");
			}
			FlushEverything();
			Execute();
			PostFlush();
		}

		private void FlushEverything()
		{
			log.Debug("flushing session");

			interceptor.PreFlush(entitiesByKey.Values);

			PreFlushEntities();

			// We could move this inside if we wanted to
			// tolerate collection initializations during
			// collection dirty checking:
			PreFlushCollections();

			// Now, any collections that are initialized
			// inside this block do not get updated - they
			// are ignored until the next flush
			flushing = true;
			try
			{
				FlushEntities();
				FlushCollections();
			}
			finally
			{
				flushing = false;
			}

			//some stats

			if (log.IsDebugEnabled)
			{
				log.Debug("Flushed: " +
					insertions.Count + " insertions, " +
					updates.Count + " updates, " +
					deletions.Count + " deletions to " +
					entityEntries.Count + " objects");
				log.Debug("Flushed: " +
					collectionCreations.Count + " (re)creations, " +
					collectionUpdates.Count + " updates, " +
					collectionRemovals.Count + " removals to " +
					collectionEntries.Count + " collections");

				new Printer(factory).ToString(entitiesByKey.Values.GetEnumerator());
			}
		}

		private bool AreTablesToBeUpdated(ISet tables)
		{
			return AreTablesToUpdated(updates, tables) ||
				AreTablesToUpdated(insertions, tables) ||
				AreTablesToUpdated(deletions, tables) ||
				AreTablesToUpdated(collectionUpdates, tables) ||
				AreTablesToUpdated(collectionCreations, tables) ||
				AreTablesToUpdated(collectionRemovals, tables);
		}

		private bool AreTablesToUpdated(ICollection executables, ISet theSet)
		{
			foreach (IExecutable executable in executables)
			{
				object[] spaces = executable.PropertySpaces;
				for (int i = 0; i < spaces.Length; i++)
				{
					if (theSet.Contains(spaces[i]))
					{
						if (log.IsDebugEnabled)
						{
							log.Debug(string.Format("changes must be flushed to space: {0}", spaces[i]));
						}
						return true;
					}
				}
			}
			return false;
		}

		private void ExecuteInserts()
		{
			log.Debug("executing insertions");
			ExecuteAll(insertions);
		}

		public bool IsDirty()
		{
			CheckIsOpen();

			log.Debug("checking session dirtiness");
			if (insertions.Count > 0 || deletions.Count > 0)
			{
				log.Debug("session dirty (scheduled updates and insertions)");
				return true;
			}
			else
			{
				int oldSize = collectionRemovals.Count;
				try
				{
					FlushEverything();
					bool result = updates.Count > 0 ||
						insertions.Count > 0 ||
						deletions.Count > 0 ||
						collectionUpdates.Count > 0 ||
						collectionRemovals.Count > 0 ||
						collectionCreations.Count > 0;
					log.Debug(result ? "session dirty" : "session not dirty");
					return result;
				}
				finally
				{
					collectionCreations.Clear();
					collectionUpdates.Clear();
					updates.Clear();
					// collection deletions are a special case since update() can add
					// deletions of collections not loaded by the session.
					for (int i = collectionRemovals.Count - 1; i >= oldSize; i--)
					{
						collectionRemovals.RemoveAt(i);
					}
				}
			}
		}

		/**
		 * Execute all SQL and second-level cache updates, in a
		 * special order so that foreign-key constraints cannot
		 * be violated:
		 * <ol>
		 * <li> Inserts, in the order they were performed </li>
		 * <li> Updates</li>
		 * <li> Deletion of collection elements</li>
		 * <li> Insertion of collection elements</li>
		 * <li> Deletes, in the order they were performed</li>
		 * </ol>
		 */

		private void Execute()
		{
			log.Debug("executing flush");

			try
			{
				// we need to lock the collection caches before
				// executing entity insert/updates in order to
				// account for bidi associations
				BeforeExecutionsAll(collectionRemovals);
				BeforeExecutionsAll(collectionUpdates);
				BeforeExecutionsAll(collectionCreations);

				// now actually execute SQL and update the
				// second-level cache
				ExecuteAll(insertions);
				ExecuteAll(updates);
				ExecuteAll(collectionRemovals);
				ExecuteAll(collectionUpdates);
				ExecuteAll(collectionCreations);
				ExecuteAll(deletions);
			}
			catch (HibernateException he)
			{
				log.Error("could not synchronize database state with session", he);
				throw;
			}
		}

		public void PostInsert(object obj)
		{
			EntityEntry entry = GetEntry(obj);
			if (entry == null)
			{
				throw new AssertionFailure("possible nonthreadsafe access to session");
			}
			entry.ExistsInDatabase = true;
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
			proxiesByKey.Remove(key);
		}

		public void PostUpdate(object obj, object[] updatedState, object nextVersion)
		{
			EntityEntry entry = GetEntry(obj);
			if (entry == null)
			{
				throw new AssertionFailure("possible nonthreadsafe access to session");
			}
			entry.LoadedState = updatedState;
			entry.LockMode = LockMode.Write;
			if (entry.Persister.IsVersioned)
			{
				entry.Version = nextVersion;
				entry.Persister.SetPropertyValue(obj, entry.Persister.VersionProperty, nextVersion);
			}
		}

		private void ExecuteAll(IList list)
		{
			foreach (IExecutable e in list)
			{
				Execute(e);
			}
			list.Clear();

			if (batcher != null)
			{
				batcher.ExecuteBatch();
			}
		}

		private void Execute(IExecutable executable)
		{
			bool lockQueryCache = factory.IsQueryCacheEnabled;
			if (executable.HasAfterTransactionCompletion || lockQueryCache)
			{
				executions.Add(executable);
			}
			if (lockQueryCache)
			{
				factory.UpdateTimestampsCache.PreInvalidate(executable.PropertySpaces);
			}
			executable.Execute();
		}

		private void BeforeExecutionsAll(IList list)
		{
			foreach (IExecutable e in list)
			{
				e.BeforeExecutions();
			}
		}

		/// <summary>
		/// 1. detect any dirty entities
		/// 2. schedule any entity updates
		/// 3. search out any reachable collections
		/// </summary>
		private void FlushEntities()
		{
			log.Debug("Flushing entities and processing referenced collections");

			// Among other things, UpdateReachables() will recursively load all
			// collections that are moving roles. This might cause entities to
			// be loaded.

			// So this needs to be safe from concurrent modification problems.
			// It is safe because of how IdentityMap implements entrySet()

			ICollection iterSafeCollection = IdentityMap.ConcurrentEntries(entityEntries);

			foreach (DictionaryEntry me in iterSafeCollection)
			{
				// Update the status of the object and if necessary, schedule an update
				EntityEntry entry = (EntityEntry) me.Value;
				Status status = entry.Status;

				if (status != Status.Loading && status != Status.Gone)
				{
					FlushEntity(me.Key, entry);
				}
			}
		}

		private void FlushEntity(object obj, EntityEntry entry)
		{
			IEntityPersister persister = entry.Persister;
			Status status = entry.Status;
			CheckId(obj, persister, entry.Id);

			object[] values;
			if (status == Status.Deleted)
			{
				//grab its state saved at deletion
				values = entry.DeletedState;
			}
			else
			{
				//grab its current state
				values = persister.GetPropertyValues(obj);
			}
			IType[] types = persister.PropertyTypes;

			bool substitute = false;

			if (persister.HasCollections)
			{
				// wrap up any new collections directly referenced by the object
				// or its components

				// NOTE: we need to do the wrap here even if its not "dirty",
				// because nested collections need wrapping but changes to
				// _them_ don't dirty the container. Also, for versioned
				// data, we need to wrap before calling searchForDirtyCollections

				WrapVisitor visitor = new WrapVisitor(this);
				// substitutes into values by side-effect
				visitor.ProcessValues(values, types);
				substitute = visitor.IsSubstitutionRequired;
			}

			bool cannotDirtyCheck;
			bool interceptorHandledDirtyCheck;
			bool dirtyCheckDoneBySelect = false;
			object[] currentPersistentState = null;

			int[] dirtyProperties = interceptor.FindDirty(obj, entry.Id, values, entry.LoadedState, persister.PropertyNames, types);

			if (dirtyProperties == null)
			{
				// interceptor returned null, so do the dirtycheck ourself, if possible
				interceptorHandledDirtyCheck = false;
				cannotDirtyCheck = entry.LoadedState == null; // object loaded by update()
				if (!cannotDirtyCheck)
				{
					dirtyProperties = persister.FindDirty(values, entry.LoadedState, obj, this);
				}
				else
				{
					currentPersistentState = persister.GetDatabaseSnapshot(entry.Id, entry.Version, this);
					if (currentPersistentState != null)
					{
						dirtyProperties = persister.FindModified(currentPersistentState, values, obj, this);
						cannotDirtyCheck = false;
						dirtyCheckDoneBySelect = true;
					}
				}
			}
			else
			{
				// the interceptor handled the dirty checking
				cannotDirtyCheck = false;
				interceptorHandledDirtyCheck = true;
			}

			// compare to cached state (ignoring nested collections)
			if (IsUpdateNecessary(persister, cannotDirtyCheck, status, dirtyProperties, values, types))
			{
				// its dirty!

				if (log.IsDebugEnabled)
				{
					if (status == Status.Deleted)
					{
						log.Debug("Updating deleted entity: " + MessageHelper.InfoString(persister, entry.Id));
					}
					else
					{
						log.Debug("Updating entity: " + MessageHelper.InfoString(persister, entry.Id));
					}
				}

				if (!entry.IsBeingReplicated)
				{
					// give the Interceptor a chance to modify property values
					bool intercepted = interceptor.OnFlushDirty(
						obj, entry.Id, values, entry.LoadedState, persister.PropertyNames, types);

					//now we might need to recalculate the dirtyProperties array
					if (intercepted && !cannotDirtyCheck && !interceptorHandledDirtyCheck)
					{
						if (dirtyCheckDoneBySelect)
						{
							dirtyProperties = persister.FindModified(currentPersistentState, values, obj, this);
						}
						else
						{
							dirtyProperties = persister.FindDirty(values, entry.LoadedState, obj, this);
						}
					}
					// if the properties were modified by the Interceptor, we need to set them back to the object
					substitute = substitute || intercepted;
				}

				// validate() instances of Validatable
				if (status == Status.Loaded && persister.ImplementsValidatable)
				{
					((IValidatable) obj).Validate();
				}

				//increment the version number (if necessary)
				object nextVersion = GetNextVersion(persister, values, entry);

				// get the updated snapshot by cloning current state
				object[] updatedState = null;
				if (status == Status.Loaded)
				{
					updatedState = new object[values.Length];
					TypeFactory.DeepCopy(values, types, persister.PropertyCheckability, updatedState);
				}

				// if it was dirtied by a collection only
				if (!cannotDirtyCheck && dirtyProperties == null)
				{
					dirtyProperties = ArrayHelper.EmptyIntArray;
				}

				CheckNullability(values, persister, true);

                bool hasDirtyCollections = HasDirtyCollections(persister, status, values, types);
				//note that we intentionally did _not_ pass in currentPersistentState!
				updates.Add(
                    new ScheduledUpdate(entry.Id, values, dirtyProperties, hasDirtyCollections, entry.LoadedState, entry.Version, nextVersion, obj, updatedState, persister, this)
					);
			}

			if (status == Status.Deleted)
			{
				//entry.status = Status.Gone;
			}
			else
			{
				// now update the object... has to be outside the main if block above (because of collections)
				if (substitute)
				{
					persister.SetPropertyValues(obj, values);
				}

				// search for collections by reachability, updating their role.
				// we don't want to touch collections reachable from a deleted object.
				if (persister.HasCollections)
				{
					new FlushVisitor(this, obj).ProcessValues(values, types);
				}
			}
		}

        private bool HasDirtyCollections(
            IEntityPersister persister,
            Status status,
            object[] values,
            IType[] types)
        {
            if (status == Status.Loaded && persister.IsVersioned && persister.HasCollections)
            {
                DirtyCollectionSearchVisitor visitor = new DirtyCollectionSearchVisitor(this, persister.PropertyVersionability);
                visitor.ProcessEntityPropertyValues(values, types);
                return visitor.WasDirtyCollectionFound;
            }
            else
            {
                return false;
            }
        }


		private bool IsUpdateNecessary(
			IEntityPersister persister,
			bool cannotDirtyCheck,
			Status status,
			int[] dirtyProperties,
			object[] values,
			IType[] types)
		{
			if (!persister.IsMutable)
			{
				return false;
			}
			if (cannotDirtyCheck)
			{
				return true;
			}

			if (dirtyProperties != null && dirtyProperties.Length != 0)
			{
				return true;
			}

			if (status == Status.Loaded && persister.IsVersioned && persister.HasCollections)
			{
				DirtyCollectionSearchVisitor visitor = new DirtyCollectionSearchVisitor(this, persister.PropertyVersionability);
				visitor.ProcessValues(values, types);
				return visitor.WasDirtyCollectionFound;
			}
			else
			{
				return false;
			}
		}

		private object GetNextVersion(IEntityPersister persister, object[] values, EntityEntry entry)
		{
			if (persister.IsVersioned)
			{
				if (entry.IsBeingReplicated)
				{
					return Versioning.GetVersion(values, persister);
				}
				else
				{
					Object nextVersion = entry.Status == Status.Deleted ?
						entry.Version :
						Versioning.Increment(entry.Version, persister.VersionType, this);
					Versioning.SetVersion(values, nextVersion, persister);
					return nextVersion;
				}
			}
			else
			{
				return null;
			}
		}

		private static void CheckId(object obj, IEntityPersister persister, object id)
		{
			// make sure user didn't mangle the id
			if (persister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier)
			{
				object oid = persister.GetIdentifier(obj);
				if (id == null)
				{
					throw new AssertionFailure("null id in entry (don't flush the Session after an exception occurs)");
				}

				if (!id.Equals(oid))
				{
					throw new HibernateException(
						string.Format(
							"identifier of an instance of {0} altered from {1} ({2}) to {3} ({4})",
							persister.ClassName, id, id.GetType(), oid, oid.GetType())
						);
				}
			}
		}

		/// <summary>
		/// Process cascade save/update at the start of a flush to discover
		/// any newly referenced entity that must be passed to
		/// <see cref="SaveOrUpdate" /> and also apply orphan delete
		/// </summary>
		private void PreFlushEntities()
		{
			ICollection iterSafeCollection = IdentityMap.ConcurrentEntries(entityEntries);

			// so that we can be safe from the enumerator & concurrent modifications
			foreach (DictionaryEntry me in iterSafeCollection)
			{
				EntityEntry entry = (EntityEntry) me.Value;
				Status status = entry.Status;

				if (status != Status.Loading && status != Status.Gone && status != Status.Deleted)
				{
					object obj = me.Key;
					cascading++;
					try
					{
						Cascades.Cascade(this, entry.Persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeOnUpdate);
					}
					finally
					{
						cascading--;
					}
				}
			}
		}

		// This just does a table lookup, but caches the last result

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
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj);
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
				return NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj).Identifier;
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

			object isUnsaved = interceptor.IsUnsaved(obj);
			if (isUnsaved != null)
			{
				return !(bool) isUnsaved;
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
				return NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj).Identifier;
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
					object isUnsaved = interceptor.IsUnsaved(obj);

					if (isUnsaved != null && ((bool) isUnsaved))
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

		/// <summary>
		/// process any unreferenced collections and then inspect all known collections,
		/// scheduling creates/removes/updates
		/// </summary>
		private void FlushCollections()
		{
			log.Debug("Processing unreferenced collections");

			foreach (DictionaryEntry e in IdentityMap.ConcurrentEntries(collectionEntries))
			{
				CollectionEntry ce = (CollectionEntry) e.Value;
				if (!ce.IsReached && !ce.IsIgnore)
				{
					UpdateUnreachableCollection((IPersistentCollection) e.Key);
				}

			}
			// schedule updates to collections:

			log.Debug("scheduling collection removes/(re)creates/updates");

			foreach (DictionaryEntry me in IdentityMap.Entries(collectionEntries))
			{
				IPersistentCollection coll = (IPersistentCollection) me.Key;
				CollectionEntry ce = (CollectionEntry) me.Value;

				if (ce.IsDorecreate)
				{
					collectionCreations.Add(new ScheduledCollectionRecreate(coll, ce.CurrentPersister, ce.CurrentKey, this));
				}
				if (ce.IsDoremove)
				{
					collectionRemovals.Add(new ScheduledCollectionRemove(ce.LoadedPersister, ce.LoadedKey, ce.IsSnapshotEmpty(coll), this));
				}
				if (ce.IsDoupdate)
				{
					collectionUpdates.Add(new ScheduledCollectionUpdate(coll, ce.LoadedPersister, ce.LoadedKey, ce.IsSnapshotEmpty(coll), this));
				}
			}
		}

		/// <summary>
		/// 1. Recreate the collection key -> collection map
		/// 2. rebuild the collection entries
		/// 3. call Interceptor.postFlush()
		/// </summary>
		private void PostFlush()
		{
			log.Debug("post flush");

			ISet keysToRemove = new HashedSet();
			foreach (DictionaryEntry me in collectionEntries)
			{
				CollectionEntry ce = (CollectionEntry) me.Value;
				IPersistentCollection pc = (IPersistentCollection) me.Key;

				if (ce.PostFlush(pc))
				{
					keysToRemove.Add(me.Key);
				}
				else if (ce.IsReached)
				{
					collectionsByKey[new CollectionKey(ce.CurrentPersister, ce.CurrentKey)] = pc;
				}
			}

			foreach (object key in keysToRemove)
			{
				collectionEntries.Remove(key);
			}

			interceptor.PostFlush(entitiesByKey.Values);
		}

		/// <summary>
		/// Initialize the flags of the CollectionEntry, including the
		/// dirty check.
		/// </summary>
		private void PreFlushCollections()
		{
			// Initialize dirty flags for arrays + collections with composte elements
			// and reset reached, doupdate, etc

			foreach (DictionaryEntry de in IdentityMap.Entries(collectionEntries))
			{
				CollectionEntry ce = (CollectionEntry) de.Value;
				IPersistentCollection pc = (IPersistentCollection) de.Key;

				ce.PreFlush(pc);
			}
		}

		/// <summary>
		/// Initialize the role of the collection.
		/// The CollectionEntry.reached stuff is just to detect any silly users who set up
		/// circular or shared references between/to collections.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="type"></param>
		/// <param name="owner"></param>
		public void UpdateReachableCollection(IPersistentCollection coll, IType type, object owner)
		{
			CollectionEntry ce = GetCollectionEntry(coll);

			if (ce == null)
			{
				// refer to comment in AddCollection()
				throw new HibernateException(string.Format("Found two representations of same collection: {0}", coll.CollectionSnapshot.Role));
			}

			if (ce.IsReached)
			{
				// we've been here before
				throw new HibernateException(string.Format("Found shared references to a collection: {0}", coll.CollectionSnapshot.Role));
			}
			ce.IsReached = true;

			ICollectionPersister persister = GetCollectionPersister(((CollectionType) type).Role);
			ce.CurrentPersister = persister;
			ce.CurrentKey = GetEntityIdentifier(owner); //TODO: better to pass the id in as an argument?

			if (log.IsDebugEnabled)
			{
				log.Debug(
					"Collection found: " + MessageHelper.InfoString(persister, ce.CurrentKey) +
						", was: " + MessageHelper.InfoString(ce.LoadedPersister, ce.LoadedKey)
					);
			}

			PrepareCollectionForUpdate(coll, ce);
		}

		/// <summary>
		/// record the fact that this collection was dereferenced
		/// </summary>
		/// <param name="coll"></param>
		private void UpdateUnreachableCollection(IPersistentCollection coll)
		{
			CollectionEntry entry = GetCollectionEntry(coll);

			if (log.IsDebugEnabled && entry.LoadedPersister != null)
			{
				log.Debug("collection dereferenced: " + MessageHelper.InfoString(entry.LoadedPersister, entry.LoadedKey));
			}

			// do a check
			if (entry.LoadedPersister != null && entry.LoadedPersister.HasOrphanDelete)
			{
				EntityKey key = new EntityKey(
					entry.LoadedKey,
					GetClassPersister(entry.LoadedPersister.OwnerClass));

				object owner = GetEntity(key);

				if (owner == null)
				{
					throw new AssertionFailure("owner not associated with session");
				}

				EntityEntry e = GetEntry(owner);

				// only collections belonging to deleted entities are allowed to be dereferenced in 
				// the case of orphan delete
				if (e != null && e.Status != Status.Deleted && e.Status != Status.Gone)
				{
					throw new HibernateException("You may not dereference an collection with cascade=\"all-delete-orphan\"");
				}
			}

			// do the work
			entry.CurrentPersister = null;
			entry.CurrentKey = null;

			PrepareCollectionForUpdate(coll, entry);
		}

		/// <summary>
		/// 1. record the collection role that this collection is referenced by
		/// 2. decide if the collection needs deleting/creating/updating (but
		///    don't actually schedule the action yet)
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="entry"></param>
		private void PrepareCollectionForUpdate(IPersistentCollection coll, CollectionEntry entry)
		{
			if (entry.IsProcessed)
			{
				throw new AssertionFailure("collection was processed twice by flush()");
			}

			entry.IsProcessed = true;

			// it is or was referenced _somewhere_
			if (entry.LoadedPersister != null || entry.CurrentPersister != null)
			{
				if (
					entry.LoadedPersister != entry.CurrentPersister || //if either its role changed,
						!entry.CurrentPersister.KeyType.Equals(entry.LoadedKey, entry.CurrentKey) // or its key changed
					)
				{
					// do a check
					if (
						entry.LoadedPersister != null &&
							entry.CurrentPersister != null &&
							entry.LoadedPersister.HasOrphanDelete)
					{
						throw new HibernateException("Don't dereference a collection with cascade=\"all-delete-orphan\": " + coll.CollectionSnapshot.Role);
					}

					// do the work
					if (entry.CurrentPersister != null)
					{
						entry.IsDorecreate = true; //we will need to create new entries
					}

					if (entry.LoadedPersister != null)
					{
						entry.IsDoremove = true; // we will need to remove the old entres
						if (entry.IsDorecreate)
						{
							log.Debug("forcing collection initialization");
							coll.ForceInitialization();
						}
					}
				}
				else if (entry.Dirty)
				{
					// else if it's elements changed
					entry.IsDoupdate = true;
				}
			}
		}

		/// <summary>
		/// ONLY near the end of the flush process, determine if the collection is dirty
		/// by checking its entry
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		internal bool CollectionIsDirty(IPersistentCollection coll)
		{
			CollectionEntry entry = GetCollectionEntry(coll);
			return coll.WasInitialized && entry.Dirty; //( entry.dirty || coll.hasQueuedAdds() ); 
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

			internal LoadingCollectionEntry(CollectionKey key, IPersistentCollection collection, object id, ICollectionPersister persister, object resultSetId)
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
			return (LoadingCollectionEntry) loadingCollections[collectionKey];
		}

		private void AddLoadingCollectionEntry(CollectionKey key, IPersistentCollection collection, ICollectionPersister persister, object resultSetId)
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
				LoadingCollectionEntry lce = (LoadingCollectionEntry) resultSetCollections[i];
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

				if (noQueueAdds && persister.HasCache && !ce.IsDoremove)
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("caching collection: " + MessageHelper.InfoString(persister, lce.Id));
					}
					IEntityPersister ownerPersister = factory.GetEntityPersister(persister.OwnerClass);
					object version;
					IComparer versionComparator;
					if (ownerPersister.IsVersioned)
					{
						version = GetEntry(GetCollectionOwner(ce)).Version;
						versionComparator = ownerPersister.VersionType.Comparator;
					}
					else
					{
						version = null;
						versionComparator = null;
					}
					CacheKey ck = new CacheKey(lce.Id, persister.KeyType, persister.Role, factory);
					persister.Cache.Put(ck, lce.Collection.Disassemble(persister), Timestamp, version, versionComparator, factory.Settings.IsMinimalPutsEnabled /*&& cacheMode != CacheMode.REFRESH*/);
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
						IPersistentCollection collection = (IPersistentCollection) nonlazyCollections[nonlazyCollections.Count - 1];
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
			IPersistentCollection old = (IPersistentCollection) collectionsByKey[ck];
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
			return (IPersistentCollection) collectionsByKey[key];
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

		private void AddUninitializedDetachedCollection(IPersistentCollection collection, ICollectionPersister persister, object id)
		{
			CollectionEntry ce = new CollectionEntry(persister, id);
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);
		}

		/// <summary>
		/// add a collection we just pulled out of the cache (does not need initializing)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		public CollectionEntry AddInitializedCollection(IPersistentCollection collection, ICollectionPersister persister, object id)
		{
			CollectionEntry ce = new CollectionEntry(persister, id, flushing);
			ce.PostInitialize(collection);
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);

			return ce;
		}

		private CollectionEntry AddCollection(IPersistentCollection collection)
		{
			CollectionEntry ce = new CollectionEntry();
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
		private void AddInitializedDetachedCollection(IPersistentCollection collection, ICollectionSnapshot cs)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public PersistentArrayHolder GetArrayHolder(object array)
		{
			return (PersistentArrayHolder) arrayHolders[array];
		}

		/// <summary>
		/// associate a holder with an array - called after loading an array
		/// </summary>
		/// <param name="holder"></param>
		public void AddArrayHolder(PersistentArrayHolder holder)
		{
			//TODO:refactor + make this method private
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

				bool foundInCache = InitializeCollectionFromCache(ce.LoadedKey, GetCollectionOwner(ce), ce.LoadedPersister, collection);

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
			get
			{
				if (connection == null)
				{
					if (connect)
					{
						Connect();
					}
					else if (IsOpen)
					{
						throw new HibernateException("Session is currently disconnected");
					}
					else
					{
						throw new HibernateException("Session is closed");
					}
				}
				return connection;
			}
		}

		private void Connect()
		{
			// TODO: H2.1 delegates this to batcher, not factory
			connection = factory.OpenConnection();
			connect = false;
		}

		/// <summary>
		/// Gets if the ISession is connected.
		/// </summary>
		/// <value>
		/// <c>true</c> if the ISession is connected.
		/// </value>
		/// <remarks>
		/// An ISession is considered connected if there is an <see cref="IDbConnection"/> (regardless
		/// of its state) or if it the field <c>connect</c> is true.  Meaning that it will connect
		/// at the next operation that requires a connection.
		/// </remarks>
		public bool IsConnected
		{
			get { return connection != null || connect; }
		}

		/// <summary></summary>
		public IDbConnection Disconnect()
		{
			CheckIsOpen();

			log.Debug("disconnecting session");

			try
			{
				// the Session is flagged as needing to create a Connection for the
				// next operation
				if (connect)
				{
					// a Disconnected Session should not automattically "connect"
					connect = false;
					return null;
				}
				else
				{
					if (connection == null)
					{
						throw new HibernateException("Session already disconnected");
					}

					if (batcher != null)
					{
						batcher.CloseCommands();
					}

					// get a new reference to the the Session's connection before 
					// closing it - and set the existing to Session's connection to
					// null but don't close it yet
					IDbConnection c = connection;
					connection = null;

					// if Session is supposed to auto-close the connection then
					// the Sesssion is managing the IDbConnection. 
					if (autoClose)
					{
						// let the SessionFactory close it and return null
						// because the connection is internal to the Session

						// TODO: H2.1 delegates this to batcher, not factory
						factory.CloseConnection(c);
						return null;
					}
					else
					{
						// return the connection the user provided - at this point
						// it has been disassociated with the NHibernate session. 
						return c;
					}
				}
			}
			finally
			{
				// ensure that AfterTransactionCompletion gets called since
				// it takes care of the Locks and Cache.
				if (!IsInActiveTransaction)
				{
					// We don't know the state of the transaction
					AfterTransactionCompletion(false);
				}
			}
		}

		public void Reconnect()
		{
			CheckIsOpen();

			if (IsConnected)
			{
				throw new HibernateException("session already connected");
			}

			log.Debug("reconnecting session");

			connect = true;
			autoClose = true;
		}

		public void Reconnect(IDbConnection conn)
		{
			CheckIsOpen();

			if (IsConnected)
			{
				throw new HibernateException("session already connected");
			}
			this.connection = conn;
			autoClose = false;
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
				if (transaction != null)
				{
					transaction.Dispose();
				}

				if (batcher != null)
				{
					batcher.Dispose();
				}

				// we are not reusing the Close() method because that sets the connection==null
				// during the Close() - if the connection is null we can't get to it to Dispose
				// of it.
				if (connection != null)
				{
					if (connection.State == ConnectionState.Closed)
					{
						log.Warn("finalizing unclosed session with closed connection");
					}
					else
					{
						// if the Session is responsible for managing the connection then make sure
						// the connection is disposed of.
						if (autoClose)
						{
							factory.CloseConnection(connection);
						}
					}
				}
				// it is important to call Cleanup because that marks the Session as being
				// closed - the Session could still be associated with a Proxy that is attempting
				// to be reassociated with another Session.  If the Proxy sees ISession.IsOpen==true
				// then an exception will be thrown for trying to associate it with 2 open sessions.

				Cleanup();
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
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
		private QueryTranslator GetFilterTranslator(object collection, string filter, QueryParameters parameters, bool scalar)
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

			QueryTranslator filterTranslator;
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
		/// collection. Return <c>null</c> if there is no entry.
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		private CollectionEntry GetCollectionEntryOrNull(object collection)
		{
			IPersistentCollection coll;
			if (collection is IPersistentCollection)
			{
				coll = (IPersistentCollection) collection;
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
			string[] concreteFilters = QueryTranslator.ConcreteQueries(filter, factory);
			QueryTranslator[] filters = new QueryTranslator[concreteFilters.Length];

			for (int i = 0; i < concreteFilters.Length; i++)
			{
				filters[i] = GetFilterTranslator(
					collection,
					concreteFilters[i],
					parameters,
					false);

			}

			dontFlushFromFind++; // stops flush being called multiple times if this method is recursively called

			try
			{
				for (int i = filters.Length - 1; i >= 0; i--)
				{
					ArrayHelper.AddAll(results, filters[i].List(this, parameters));
				}
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
			}
		}

		public IList Filter(object collection, string filter, QueryParameters parameters)
		{
			ArrayList results = new ArrayList();
			Filter(collection, filter, parameters, results);
			return results;
		}

#if NET_2_0
		public IList<T> Filter<T>(object collection, string filter, QueryParameters parameters)
		{
			List<T> results = new List<T>();
			Filter(collection, filter, parameters, results);
			return results;
		}
#endif

		public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
		{
			string[] concreteFilters = QueryTranslator.ConcreteQueries(filter, factory);
			QueryTranslator[] filters = new QueryTranslator[concreteFilters.Length];

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

#if NET_2_0
		public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			string[] concreteFilters = QueryTranslator.ConcreteQueries(filter, factory);
			QueryTranslator[] filters = new QueryTranslator[concreteFilters.Length];

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
#endif

		public ICriteria CreateCriteria(System.Type persistentClass)
		{
			CheckIsOpen();

			return new CriteriaImpl(persistentClass, this);
		}

		public IList Find(CriteriaImpl criteria)
		{
			ArrayList results = new ArrayList();
			Find(criteria, results);
			return results;
		}

#if NET_2_0
		public IList<T> Find<T>(CriteriaImpl criteria)
		{
			List<T> results = new List<T>();
			Find(criteria, results);
			return results;
		}
#endif

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

			try
			{
				for (int i = size - 1; i >= 0; i--)
				{
					ArrayHelper.AddAll(results, loaders[i].List(this));
				}
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
			}
		}

		private IOuterJoinLoadable GetOuterJoinLoadable(System.Type clazz)
		{
			IEntityPersister persister = GetClassPersister(clazz);
			if (!(persister is IOuterJoinLoadable))
			{
				throw new MappingException("class persister is not IOuterJoinLoadable: " + clazz.FullName);
			}
			return (IOuterJoinLoadable) persister;
		}

		public bool Contains(object obj)
		{
			CheckIsOpen();

			if (obj is INHibernateProxy)
			{
				//do not use proxiesByKey, since not all
				//proxies that point to this session's
				//instances are in that collection!
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj);
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
			CheckIsOpen();

			if (obj is INHibernateProxy)
			{
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj);
				object id = li.Identifier;
				IEntityPersister persister = GetClassPersister(li.PersistentClass);
				EntityKey key = new EntityKey(id, persister);
				proxiesByKey.Remove(key);
				if (!li.IsUninitialized)
				{
					object entity = RemoveEntity(key);
					if (entity != null)
					{
						EntityEntry e = RemoveEntry(entity);
						DoEvict(e.Persister, entity);
					}
				}
			}
			else
			{
				EntityEntry e = RemoveEntry(obj);
				if (e != null)
				{
					RemoveEntity(new EntityKey(e.Id, e.Persister));
					DoEvict(e.Persister, obj);
				}
			}
		}

		private void DoEvict(IEntityPersister persister, object obj)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("evicting " + MessageHelper.InfoString(persister));
			}

			//remove all collections for the entity from the session-level cache
			if (persister.HasCollections)
			{
				new EvictVisitor(this).Process(obj, persister);
			}

			Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionEvict, CascadePoint.CascadeOnEvict);
		}

		internal void EvictCollection(object value, CollectionType type)
		{
			object pc;
			if (type.IsArrayType)
			{
				pc = arrayHolders[value];
				arrayHolders.Remove(value);
			}
			else if (value is IPersistentCollection)
			{
				pc = value;
			}
			else
			{
				return; //EARLY EXIT!
			}

			IPersistentCollection collection = pc as IPersistentCollection;
			if (collection.UnsetSession(this))
			{
				EvictCollection(collection);
			}

		}

		private void EvictCollection(IPersistentCollection collection)
		{
			CollectionEntry ce = (CollectionEntry) collectionEntries[collection];
			collectionEntries.Remove(collection);
			if (log.IsDebugEnabled)
			{
				log.Debug("evicting collection: " + MessageHelper.InfoString(ce.LoadedPersister, ce.LoadedKey));
			}
			if (ce.LoadedPersister != null && ce.LoadedKey != null)
			{
				//TODO: is this 100% correct?
				collectionsByKey.Remove(new CollectionKey(ce.LoadedPersister, ce.LoadedKey));
			}
		}

		/// <summary>
		/// Evict collections from the factory-level cache
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		private void EvictCachedCollections(IEntityPersister persister, object id)
		{
			EvictCachedCollections(persister.PropertyTypes, id);
		}

		private void EvictCachedCollections(IType[] types, object id)
		{
			foreach (IType type in types)
			{
				if (type.IsCollectionType)
				{
					factory.EvictCollection(((CollectionType) type).Role, id);
				}
				else if (type.IsComponentType)
				{
					IAbstractComponentType acType = (IAbstractComponentType) type;
					EvictCachedCollections(acType.Subtypes, id);
				}
			}
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
			object[] keys = new object[batchSize];
			keys[0] = id;
			int i = 0;
			foreach (DictionaryEntry de in collectionEntries)
			{
				IPersistentCollection collection = (IPersistentCollection) de.Key;
				CollectionEntry ce = (CollectionEntry) de.Value;
				if (!collection.WasInitialized && ce.LoadedPersister == collectionPersister && !id.Equals(ce.LoadedKey))
				{
					keys[++i] = ce.LoadedKey;
					if (i == batchSize - 1)
					{
						return keys;
					}
				}
			}
			return keys;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		public object[] GetClassBatch(System.Type clazz, object id, int batchSize)
		{
			object[] ids = new object[batchSize];
			ids[0] = id;
			int i = 0;
			foreach (EntityKey key in batchLoadableEntityKeys.Keys)
			{
				if (key.MappedClass == clazz && !id.Equals(key.Identifier))
				{
					ids[++i] = key.Identifier;
					if (i == batchSize - 1)
					{
						return ids;
					}
				}
			}
			return ids;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		public void ScheduleBatchLoad(System.Type clazz, object id)
		{
			IEntityPersister persister = GetClassPersister(clazz);
			if (persister.IsBatchLoadable)
			{
				batchLoadableEntityKeys.Add(new EntityKey(id, persister), Marker);
			}
		}

		public ISQLQuery CreateSQLQuery(string sql, string returnAlias, System.Type returnClass)
		{
			CheckIsOpen();

			return new SqlQueryImpl(sql, new string[] { returnAlias }, new System.Type[] { returnClass }, this, null);
		}

		public ISQLQuery CreateSQLQuery(string sql, string[] returnAliases, System.Type[] returnClasses)
		{
			CheckIsOpen();

			return new SqlQueryImpl(sql, returnAliases, returnClasses, this, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="returnAliases"></param>
		/// <param name="returnClasses"></param>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
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

#if NET_2_0
		public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			List<T> results = new List<T>();
			List(spec, queryParameters, results);
			return results;
		}
#endif

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

			dontFlushFromFind++;
			try
			{
				ArrayHelper.AddAll(results, loader.List(this, queryParameters));
			}
			finally
			{
				dontFlushFromFind--;
			}
		}

#if NET_2_0
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
#endif

		/// <summary></summary>
		public void Clear()
		{
			CheckIsOpen();

			arrayHolders.Clear();
			entitiesByKey.Clear();
			entityEntries.Clear();
			collectionsByKey.Clear();
			collectionEntries.Clear();
			proxiesByKey.Clear();
			batchLoadableEntityKeys.Clear();
			nonExists.Clear();

			updates.Clear();
			insertions.Clear();
			deletions.Clear();
			collectionCreations.Clear();
			collectionRemovals.Clear();
			collectionUpdates.Clear();

			// TODO: Find out why this is missing from H2.1
			nullifiables.Clear();
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
			IUniqueKeyLoadable persister = (IUniqueKeyLoadable) Factory.GetEntityPersister(clazz);
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
			CheckIsOpen();

			if (obj == null)
			{
				throw new ArgumentNullException("obj", "attempt to replicate null");
			}

			if (ReassociateIfUninitializedProxy(obj))
			{
				return;
			}

			object theObj = UnproxyAndReassociate(obj);

			if (IsEntryFor(theObj))
			{
				return;
			}

			IEntityPersister persister = GetEntityPersister(theObj);
			if (persister.IsUnsaved(theObj))
			{
				//TODO: generate a new id value for brand new objects
				throw new TransientObjectException("unsaved object passed to Replicate()");
			}

			object id = persister.GetIdentifier(theObj);
			object oldVersion;
			if (replicationMode == ReplicationMode.Exception)
			{
				//always do an INSERT, and let it fail by constraint violation
				oldVersion = null;
			}
			else
			{
				//what is the version on the database?
				oldVersion = persister.GetCurrentVersion(id, this);
			}

			if (oldVersion != null)
			{
				// existing row - do an update if appropriate
				if (replicationMode.ShouldOverwriteCurrentVersion(theObj, oldVersion, persister.GetVersion(obj), persister.VersionType))
				{
					//will result in a SQL UPDATE:
					DoReplicate(theObj, id, oldVersion, replicationMode, persister);
				}
				//else do nothing (don't even reassociate object!)
				//TODO: would it be better to do a refresh from db?
			}
			else
			{
				// no existing row - do an insert
				if (log.IsDebugEnabled)
				{
					log.Debug("replicating " + MessageHelper.InfoString(persister, id));
				}
				bool regenerate = persister.IsIdentifierAssignedByInsert; // prefer re-generation of identity!
				DoSave(
					theObj,
					regenerate ? null : new EntityKey(id, persister),
					persister,
					true, //!persister.isUnsaved(object), //TODO: Do an ordinary save in the case of an "unsaved" object
					// TODO: currently ignores interceptor definition of isUnsaved()
					regenerate,
					Cascades.CascadingAction.ActionReplicate, // not quite an ordinary save(), since we cascade back to replicate()
					replicationMode
					);
			}
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

			if (collection != null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("returning loading collection:"
						+ MessageHelper.InfoString(persister, id));
				}
				return collection.GetValue();
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("creating collection wrapper:" + MessageHelper.InfoString(persister, id));
				}
				//TODO: suck into CollectionPersister.instantiate()
				collection = persister.CollectionType.Instantiate(this, persister);
				AddUninitializedCollection(collection, persister, id);
				if (persister.IsArray)
				{
					InitializeCollection(collection, false);
					AddArrayHolder((PersistentArrayHolder) collection);
				}
				else if (!persister.IsLazy)
				{
					nonlazyCollections.Add(collection);
				}
				return collection.GetValue();
			}
		}

		/// <summary>
		/// Try to initialize a Collection from the cache.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <param name="persister"></param>
		/// <param name="collection"></param>
		/// <returns><c>true</c> if the collection was initialized from the cache, otherwise <c>false</c>.</returns>
		private bool InitializeCollectionFromCache(object id, object owner, ICollectionPersister persister, IPersistentCollection collection)
		{
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
			CheckIsOpen();

			return DoCopy(obj, null, IdentityMap.Instantiate(10));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		public object Copy(object obj, IDictionary copiedAlready)
		{
			return DoCopy(obj, null, copiedAlready);
		}

		public object DoCopy(object obj, object id, IDictionary copiedAlready)
		{
			if (obj == null)
			{
				return null;
			}

			if (obj is INHibernateProxy)
			{
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) obj);
				if (li.IsUninitialized)
				{
					return Load(li.PersistentClass, li.Identifier); // EARLY EXIT!
				}
				else
				{
					obj = li.GetImplementation();
				}
			}

			if (copiedAlready.Contains(obj))
			{
				return obj; // EARLY EXIT!
			}

			EntityEntry entry = GetEntry(obj);
			if (entry != null)
			{
				if (id != null && entry.Id.Equals(id))
				{
					return obj; //EARLY EXIT!
				}
				// else copy from one persistent instance to another!
			}

			System.Type clazz = obj.GetType();
			IEntityPersister persister = GetClassPersister(clazz);

			object result;
			object target;
			if (id == null && persister.IsUnsaved(obj))
			{
				copiedAlready[obj] = obj;
				SaveWithGeneratedIdentifier(obj, Cascades.CascadingAction.ActionCopy, copiedAlready);
				result = obj; // TODO: Handle its proxy (reassociate it, I suppose)
				target = obj;
			}
			else
			{
				if (id == null)
				{
					id = persister.GetIdentifier(obj);
				}
				result = Get(clazz, id);
				if (result == null)
				{
					copiedAlready[obj] = obj;
					SaveWithGeneratedIdentifier(obj, Cascades.CascadingAction.ActionCopy, copiedAlready);
					result = obj; // TODO: Could it have a proxy?
					target = obj;
				}
				else
				{
					// NH: Added to fix NH-523. Result might have been loaded as a proxy earlier,
					// not initializing it here causes Unproxy to throw an exception.
					//
					// H2.1 also has this bug. H3 avoids the bug by first loading all objects
					// to which copy (merge) is going to be cascaded, and then copying. This is
					// more efficient, but not possible in NH currently.
					NHibernateUtil.Initialize(result);

					target = Unproxy(result);
					copiedAlready[obj] = target;
					if (target == obj)
					{
						return result;
					}
					else if (NHibernateUtil.GetClass(result) != clazz)
					{
						throw new WrongClassException("class of the given object did not match class of persistent copy", id, clazz);
					}
					else if (persister.IsVersioned && !persister.VersionType.Equals(persister.GetVersion(result), persister.GetVersion(obj)))
					{
						throw new StaleObjectStateException(clazz, id);
					}

					// cascade first, so that all unsaved objected get saved before we actually copy
					Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionCopy, CascadePoint.CascadeOnCopy, copiedAlready);
				}
			}

			// no need to handle the version differently
			object[] copiedValues = TypeFactory.Copy(
				persister.GetPropertyValues(obj),
				persister.GetPropertyValues(target),
				persister.PropertyTypes,
				this,
				target, copiedAlready);

			persister.SetPropertyValues(target, copiedValues);
			return result;
		}

		public object SaveOrUpdateCopy(object obj, object id)
		{
			CheckIsOpen();

			return DoCopy(obj, id, IdentityMap.Instantiate(10));
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
			return (IFilter) enabledFilters[filterName];
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
			FilterImpl filter = (FilterImpl) enabledFilters[parsed[0]];
			if (filter == null)
			{
				throw new ArgumentNullException(parsed[0], "Filter [" + parsed[0] + "] currently not enabled");
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
		
		private bool IsInActiveTransaction
		{
			get { return transaction != null && transaction.IsActive; }
		}
	}
}
