using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using NHibernate.Collection;
using NHibernate.Engine.Loading;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Util;

namespace NHibernate.Engine
{
	/// <summary>
	/// A <see cref="IPersistenceContext"/> represents the state of persistent "stuff" which
	/// NHibernate is tracking.  This includes persistent entities, collections,
	/// as well as proxies generated.
	/// </summary>
	/// <remarks>
	/// There is meant to be a one-to-one correspondence between a SessionImpl and
	/// a PersistentContext.  The SessionImpl uses the PersistentContext to track
	/// the current state of its context.  Event-listeners then use the
	/// PersistentContext to drive their processing.
	/// </remarks>
	[Serializable]
	public class StatefulPersistenceContext : IPersistenceContext, ISerializable, IDeserializationCallback
	{
		private const int InitCollectionSize = 8;
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(StatefulPersistenceContext));
		private static readonly IInternalLogger ProxyWarnLog = LoggerProvider.LoggerFor(typeof(StatefulPersistenceContext).FullName + ".ProxyWarnLog");

		public static readonly object NoRow = new object();

		[NonSerialized]
		private ISessionImplementor session;

		// Loaded entity instances, by EntityKey
		private readonly Dictionary<EntityKey, object> entitiesByKey;

		// Loaded entity instances, by EntityUniqueKey
		private readonly Dictionary<EntityUniqueKey, object> entitiesByUniqueKey;

		// Identity map of EntityEntry instances, by the entity instance
		private readonly IDictionary entityEntries;

		// Entity proxies, by EntityKey
		private readonly Dictionary<EntityKey, INHibernateProxy> proxiesByKey;

		// Snapshots of current database state for entities
		// that have *not* been loaded
		private readonly Dictionary<EntityKey, object> entitySnapshotsByKey;

		// Identity map of array holder ArrayHolder instances, by the array instance
		private readonly IDictionary arrayHolders;

		// Identity map of CollectionEntry instances, by the collection wrapper
		private readonly IDictionary collectionEntries;

		// Collection wrappers, by the CollectionKey
		private readonly Dictionary<CollectionKey, IPersistentCollection> collectionsByKey;

		// Set of EntityKeys of deleted objects
		private readonly ISet<EntityKey> nullifiableEntityKeys;

		// properties that we have tried to load, and not found in the database
		private ISet<AssociationKey> nullAssociations;

		// A list of collection wrappers that were instantiating during result set
		// processing, that we will need to initialize at the end of the query
		[NonSerialized]
		private List<IPersistentCollection> nonlazyCollections;

		// A container for collections we load up when the owning entity is not
		// yet loaded ... for now, this is purely transient!
		private Dictionary<CollectionKey, IPersistentCollection> unownedCollections;

		private bool hasNonReadOnlyEntities;
		
		// Parent entities cache by their child for cascading
		// May be empty or not contains all relation
		[NonSerialized]
		private IDictionary parentsByChild;

		[NonSerialized]
		private int cascading;

		[NonSerialized]
		private bool flushing;

		[NonSerialized]
		private int loadCounter;

		[NonSerialized]
		private LoadContexts loadContexts;

		[NonSerialized]
		private BatchFetchQueue batchFetchQueue;

		private bool defaultReadOnly;
		
		/// <summary> Constructs a PersistentContext, bound to the given session. </summary>
		/// <param name="session">The session "owning" this context. </param>
		public StatefulPersistenceContext(ISessionImplementor session)
		{
			loadCounter = 0;
			flushing = false;
			cascading = 0;
			this.session = session;

			entitiesByKey = new Dictionary<EntityKey, object>(InitCollectionSize);
			entitiesByUniqueKey = new Dictionary<EntityUniqueKey, object>(InitCollectionSize);
			proxiesByKey = new Dictionary<EntityKey, INHibernateProxy>(InitCollectionSize);
			entitySnapshotsByKey = new Dictionary<EntityKey, object>(InitCollectionSize);
			entityEntries = IdentityMap.InstantiateSequenced(InitCollectionSize);
			collectionEntries = IdentityMap.InstantiateSequenced(InitCollectionSize);
			collectionsByKey = new Dictionary<CollectionKey, IPersistentCollection>(InitCollectionSize);
			arrayHolders = IdentityMap.Instantiate(InitCollectionSize);
			parentsByChild = IdentityMap.Instantiate(InitCollectionSize);
			nullifiableEntityKeys = new HashSet<EntityKey>();
			InitTransientState();
		}

		private void InitTransientState()
		{
			loadContexts = null;
			nullAssociations = new HashSet<AssociationKey>();
			nonlazyCollections = new List<IPersistentCollection>(InitCollectionSize);
		}

		#region IPersistenceContext Members

		public bool IsStateless
		{
			get { return false; }
		}

		/// <summary>
		/// Get the session to which this persistence context is bound.
		/// </summary>
		public ISessionImplementor Session
		{
			get { return session; }
		}

		/// <summary>
		/// Retrieve this persistence context's managed load context.
		/// </summary>
		public LoadContexts LoadContexts
		{
			get
			{
				if (loadContexts == null)
					loadContexts = new LoadContexts(this);

				return loadContexts;
			}
		}

		/// <summary>
		/// Get the <tt>BatchFetchQueue</tt>, instantiating one if necessary.
		/// </summary>
		public BatchFetchQueue BatchFetchQueue
		{
			get
			{
				if (batchFetchQueue == null)
					batchFetchQueue = new BatchFetchQueue(this);

				return batchFetchQueue;
			}
		}

		/// <summary> Retrieve the set of EntityKeys representing nullifiable references</summary>
		public ISet<EntityKey> NullifiableEntityKeys
		{
			get { return nullifiableEntityKeys; }
		}

		/// <summary> Get the mapping from key value to entity instance</summary>
		public IDictionary<EntityKey, object> EntitiesByKey
		{
			get { return entitiesByKey; }
		}

		/// <summary> Get the mapping from entity instance to entity entry</summary>
		public IDictionary EntityEntries
		{
			get { return entityEntries; }
		}

		/// <summary> Get the mapping from collection instance to collection entry</summary>
		public IDictionary CollectionEntries
		{
			get { return collectionEntries; }
		}

		/// <summary> Get the mapping from collection key to collection instance</summary>
		public IDictionary<CollectionKey, IPersistentCollection> CollectionsByKey
		{
			get { return collectionsByKey; }
		}

		/// <summary> How deep are we cascaded?</summary>
		public int CascadeLevel
		{
			get { return cascading; }
		}

		/// <summary>Is a flush cycle currently in process?</summary>
		/// <remarks>Called before and after the flushcycle</remarks>
		public bool Flushing
		{
			get { return flushing; }
			set { flushing = value; }
		}

		/// <summary> Add a collection which has no owner loaded</summary>
		public void AddUnownedCollection(CollectionKey key, IPersistentCollection collection)
		{
			if (unownedCollections == null)
				unownedCollections = new Dictionary<CollectionKey, IPersistentCollection>(8);

			unownedCollections[key] = collection;
		}

		/// <summary>
		/// Get and remove a collection whose owner is not yet loaded,
		/// when its owner is being loaded
		/// </summary>
		public IPersistentCollection UseUnownedCollection(CollectionKey key)
		{
			if (unownedCollections == null)
			{
				return null;
			}
			else
			{
				IPersistentCollection tempObject;
				if (unownedCollections.TryGetValue(key, out tempObject))
					unownedCollections.Remove(key);
				return tempObject;
			}
		}

		/// <summary> Clear the state of the persistence context</summary>
		public void Clear()
		{
			foreach (INHibernateProxy proxy in proxiesByKey.Values)
			{
				ILazyInitializer li = proxy.HibernateLazyInitializer;
				li.UnsetSession();
			}

			var collectionEntryArray = IdentityMap.ConcurrentEntries(collectionEntries);
			foreach (DictionaryEntry entry in collectionEntryArray)
			{
				((IPersistentCollection)entry.Key).UnsetSession(Session);
			}

			arrayHolders.Clear();
			entitiesByKey.Clear();
			entitiesByUniqueKey.Clear();
			entityEntries.Clear();
			entitySnapshotsByKey.Clear();
			collectionsByKey.Clear();
			collectionEntries.Clear();
			if (unownedCollections != null)
			{
				unownedCollections.Clear();
			}
			proxiesByKey.Clear();
			nullifiableEntityKeys.Clear();
			if (batchFetchQueue != null)
			{
				batchFetchQueue.Clear();
			}
			hasNonReadOnlyEntities = false;
			if (loadContexts != null)
			{
				loadContexts.Cleanup();
			}
			parentsByChild.Clear();
		}

		/// <summary>False if we know for certain that all the entities are read-only</summary>
		public bool HasNonReadOnlyEntities
		{
			get { return hasNonReadOnlyEntities; }
		}
		
		/// <inheritdoc />
		public bool DefaultReadOnly
		{
			get { return defaultReadOnly; }
			set { defaultReadOnly = value; }
		}

		private void SetHasNonReadOnlyEnties(Status value)
		{
			if (value == Status.Deleted || value == Status.Loaded || value == Status.Saving)
			{
				hasNonReadOnlyEntities = true;
			}
		}

		/// <summary> Set the status of an entry</summary>
		public void SetEntryStatus(EntityEntry entry, Status status)
		{
			entry.Status = status;
			SetHasNonReadOnlyEnties(status);
		}

		/// <summary> Called after transactions end</summary>
		public void AfterTransactionCompletion()
		{
			// Downgrade locks
			foreach (EntityEntry entityEntry in entityEntries.Values)
				entityEntry.LockMode = LockMode.None;
		}

		/// <summary>
		/// Get the current state of the entity as known to the underlying
		/// database, or null if there is no corresponding row
		/// </summary>
		public object[] GetDatabaseSnapshot(object id, IEntityPersister persister)
		{
			EntityKey key = session.GenerateEntityKey(id, persister);
			object cached;
			if (entitySnapshotsByKey.TryGetValue(key, out cached))
			{
				return cached == NoRow ? null : (object[])cached;
			}
			else
			{
				object[] snapshot = persister.GetDatabaseSnapshot(id, session);
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
		/// <item><description>an entry of NO_ROW here is interpreted as an exception</description></item>
		/// </list>
		/// </remarks>
		public object[] GetCachedDatabaseSnapshot(EntityKey key)
		{
			object snapshot;
			if (!entitySnapshotsByKey.TryGetValue(key, out snapshot))
				return null;

			if (snapshot == NoRow)
			{
				throw new HibernateException("persistence context reported no row snapshot for " + MessageHelper.InfoString(key.EntityName, key.Identifier));
			}
			return (object[])snapshot;
		}

		/// <summary>
		/// Get the values of the natural id fields as known to the underlying
		/// database, or null if the entity has no natural id or there is no
		/// corresponding row.
		/// </summary>
		public object[] GetNaturalIdSnapshot(object id, IEntityPersister persister)
		{
			if (!persister.HasNaturalIdentifier)
			{
				return null;
			}

			// if the natural-id is marked as non-mutable, it is not retrieved during a
			// normal database-snapshot operation...
			int[] props = persister.NaturalIdentifierProperties;
			bool[] updateable = persister.PropertyUpdateability;
			bool allNatualIdPropsAreUpdateable = true;
			for (int i = 0; i < props.Length; i++)
			{
				if (!updateable[props[i]])
				{
					allNatualIdPropsAreUpdateable = false;
					break;
				}
			}

			if (allNatualIdPropsAreUpdateable)
			{
				// do this when all the properties are updateable since there is
				// a certain likelihood that the information will already be
				// snapshot-cached.
				object[] entitySnapshot = GetDatabaseSnapshot(id, persister);
				if (entitySnapshot == NoRow)
				{
					return null;
				}
				object[] naturalIdSnapshot = new object[props.Length];
				for (int i = 0; i < props.Length; i++)
				{
					naturalIdSnapshot[i] = entitySnapshot[props[i]];
				}
				return naturalIdSnapshot;
			}
			else
			{
				return persister.GetNaturalIdentifierSnapshot(id, session);
			}
		}

		/// <summary> Add a canonical mapping from entity key to entity instance</summary>
		public void AddEntity(EntityKey key, object entity)
		{
			entitiesByKey[key] = entity;
			BatchFetchQueue.RemoveBatchLoadableEntityKey(key);
		}

		/// <summary>
		/// Get the entity instance associated with the given <tt>EntityKey</tt>
		/// </summary>
		public object GetEntity(EntityKey key)
		{
			object result;
			entitiesByKey.TryGetValue(key, out result);
			return result;
		}

		/// <summary> Is there an entity with the given key in the persistence context</summary>
		public bool ContainsEntity(EntityKey key)
		{
			return entitiesByKey.ContainsKey(key);
		}

		/// <summary>
		/// Remove an entity from the session cache, also clear
		/// up other state associated with the entity, all except
		/// for the <tt>EntityEntry</tt>
		/// </summary>
		public object RemoveEntity(EntityKey key)
		{
			object tempObject = entitiesByKey[key];
			entitiesByKey.Remove(key);
			object entity = tempObject;
			List<EntityUniqueKey> toRemove = new List<EntityUniqueKey>();
			foreach (KeyValuePair<EntityUniqueKey, object> pair in entitiesByUniqueKey)
			{
				if (pair.Value == entity) toRemove.Add(pair.Key);
			}
			foreach (EntityUniqueKey uniqueKey in toRemove)
			{
				entitiesByUniqueKey.Remove(uniqueKey);
			}

			entitySnapshotsByKey.Remove(key);
			nullifiableEntityKeys.Remove(key);
			BatchFetchQueue.RemoveBatchLoadableEntityKey(key);
			BatchFetchQueue.RemoveSubselect(key);
			parentsByChild.Clear();
			return entity;
		}

		/// <summary> Get an entity cached by unique key</summary>
		public object GetEntity(EntityUniqueKey euk)
		{
			object result;
			entitiesByUniqueKey.TryGetValue(euk, out result);
			return result;
		}

		/// <summary> Add an entity to the cache by unique key</summary>
		public void AddEntity(EntityUniqueKey euk, object entity)
		{
			entitiesByUniqueKey[euk] = entity;
		}

		/// <summary>
		/// Retrieve the EntityEntry representation of the given entity.
		/// </summary>
		/// <param name="entity">The entity for which to locate the EntityEntry. </param>
		/// <returns> The EntityEntry for the given entity. </returns>
		public EntityEntry GetEntry(object entity)
		{
			return (EntityEntry)entityEntries[entity];
		}

		/// <summary> Remove an entity entry from the session cache</summary>
		public EntityEntry RemoveEntry(object entity)
		{
			EntityEntry tempObject = (EntityEntry)entityEntries[entity];
			entityEntries.Remove(entity);
			return tempObject;
		}

		/// <summary> Is there an EntityEntry for this instance?</summary>
		public bool IsEntryFor(object entity)
		{
			return entityEntries.Contains(entity);
		}

		/// <summary> Get the collection entry for a persistent collection</summary>
		public CollectionEntry GetCollectionEntry(IPersistentCollection coll)
		{
			return (CollectionEntry)collectionEntries[coll];
		}

		/// <summary> Adds an entity to the internal caches.</summary>
		public EntityEntry AddEntity(object entity, Status status, object[] loadedState, EntityKey entityKey, object version,
																 LockMode lockMode, bool existsInDatabase, IEntityPersister persister,
																 bool disableVersionIncrement, bool lazyPropertiesAreUnfetched)
		{
			AddEntity(entityKey, entity);

			return AddEntry(entity, status, loadedState, null, entityKey.Identifier, version, lockMode, existsInDatabase, persister, disableVersionIncrement, lazyPropertiesAreUnfetched);
		}

		/// <summary>
		/// Generates an appropriate EntityEntry instance and adds it
		/// to the event source's internal caches.
		/// </summary>
		public EntityEntry AddEntry(object entity, Status status, object[] loadedState, object rowId, object id,
																object version, LockMode lockMode, bool existsInDatabase, IEntityPersister persister,
																bool disableVersionIncrement, bool lazyPropertiesAreUnfetched)
		{
			EntityEntry e =
				new EntityEntry(status, loadedState, rowId, id, version, lockMode, existsInDatabase, persister, session.EntityMode,
								disableVersionIncrement, lazyPropertiesAreUnfetched);
			entityEntries[entity] = e;

			SetHasNonReadOnlyEnties(status);
			return e;
		}

		/// <summary> Is the given collection associated with this persistence context?</summary>
		public bool ContainsCollection(IPersistentCollection collection)
		{
			return collectionEntries.Contains(collection);
		}

		/// <summary> Is the given proxy associated with this persistence context?</summary>
		public bool ContainsProxy(INHibernateProxy proxy)
		{
			return proxiesByKey.ContainsValue(proxy);
		}

		/// <summary>
		/// Takes the given object and, if it represents a proxy, reassociates it with this event source.
		/// </summary>
		/// <param name="value">The possible proxy to be reassociated. </param>
		/// <returns> Whether the passed value represented an actual proxy which got initialized. </returns>
		public bool ReassociateIfUninitializedProxy(object value)
		{
			// TODO H3.2 Not ported
			//ElementWrapper wrapper = value as ElementWrapper;
			//if (wrapper != null)
			//{
			//  value = wrapper.Element;
			//}

			if (!NHibernateUtil.IsInitialized(value))
			{
				INHibernateProxy proxy = (INHibernateProxy)value;
				ReassociateProxy(proxy.HibernateLazyInitializer, proxy);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// If a deleted entity instance is re-saved, and it has a proxy, we need to
		/// reset the identifier of the proxy
		/// </summary>
		public void ReassociateProxy(object value, object id)
		{
			// TODO H3.2 Not ported
			//ElementWrapper wrapper = value as ElementWrapper;
			//if (wrapper != null)
			//{
			//  value = wrapper.Element;
			//}
			if (value.IsProxy())
			{
				var proxy = value as INHibernateProxy; 
				
				if (log.IsDebugEnabled)
				{
					log.Debug("setting proxy identifier: " + id);
				}
				ILazyInitializer li = proxy.HibernateLazyInitializer;
				li.Identifier = id;
				ReassociateProxy(li, proxy);
			}
		}

		/// <summary>
		/// Associate a proxy that was instantiated by another session with this session
		/// </summary>
		/// <param name="li">The proxy initializer. </param>
		/// <param name="proxy">The proxy to reassociate. </param>
		private void ReassociateProxy(ILazyInitializer li, INHibernateProxy proxy)
		{
			if (li.Session != Session)
			{
				IEntityPersister persister = session.Factory.GetEntityPersister(li.EntityName);
				EntityKey key = session.GenerateEntityKey(li.Identifier, persister);
				// any earlier proxy takes precedence
				if (!proxiesByKey.ContainsKey(key))
				{
					proxiesByKey[key] = proxy;
				}
				proxy.HibernateLazyInitializer.Session = Session;
			}
		}

		/// <summary>
		/// Get the entity instance underlying the given proxy, throwing
		/// an exception if the proxy is uninitialized. If the given object
		/// is not a proxy, simply return the argument.
		/// </summary>
		public object Unproxy(object maybeProxy)
		{
			// TODO H3.2 Not ported
			//ElementWrapper wrapper = maybeProxy as ElementWrapper;
			//if (wrapper != null)
			//{
			//  maybeProxy = wrapper.Element;
			//}

			if (maybeProxy.IsProxy())
			{
				INHibernateProxy proxy = maybeProxy as INHibernateProxy; 
				
				ILazyInitializer li = proxy.HibernateLazyInitializer;
				if (li.IsUninitialized)
					throw new PersistentObjectException("object was an uninitialized proxy for " + li.PersistentClass.FullName);

				return li.GetImplementation(); // unwrap the object
			}
			else
			{
				return maybeProxy;
			}
		}

		/// <summary>
		/// Possibly unproxy the given reference and reassociate it with the current session.
		/// </summary>
		/// <param name="maybeProxy">The reference to be unproxied if it currently represents a proxy. </param>
		/// <returns> The unproxied instance. </returns>
		public object UnproxyAndReassociate(object maybeProxy)
		{
			// TODO H3.2 Not ported
			//ElementWrapper wrapper = maybeProxy as ElementWrapper;
			//if (wrapper != null)
			//{
			//  maybeProxy = wrapper.Element;
			//}
			if (maybeProxy.IsProxy())
			{
				var proxy = maybeProxy as INHibernateProxy; 
				
				ILazyInitializer li = proxy.HibernateLazyInitializer;
				ReassociateProxy(li, proxy);
				return li.GetImplementation(); //initialize + unwrap the object
			}
			return maybeProxy;
		}

		/// <summary>
		/// Attempts to check whether the given key represents an entity already loaded within the
		/// current session.
		/// </summary>
		/// <param name="obj">The entity reference against which to perform the uniqueness check.</param>
		/// <param name="key">The entity key.</param>
		public void CheckUniqueness(EntityKey key, object obj)
		{
			object entity = GetEntity(key);
			if (entity == obj)
			{
				throw new AssertionFailure("object already associated, but no entry was found");
			}
			if (entity != null)
			{
				throw new NonUniqueObjectException(key.Identifier, key.EntityName);
			}
		}

		/// <summary>
		/// If the existing proxy is insufficiently "narrow" (derived), instantiate a new proxy
		/// and overwrite the registration of the old one. This breaks == and occurs only for
		/// "class" proxies rather than "interface" proxies. Also init the proxy to point to
		/// the given target implementation if necessary.
		/// </summary>
		/// <param name="proxy">The proxy instance to be narrowed. </param>
		/// <param name="persister">The persister for the proxied entity. </param>
		/// <param name="key">The internal cache key for the proxied entity. </param>
		/// <param name="obj">(optional) the actual proxied entity instance. </param>
		/// <returns> An appropriately narrowed instance. </returns>
		public object NarrowProxy(INHibernateProxy proxy, IEntityPersister persister, EntityKey key, object obj)
		{
			bool alreadyNarrow = persister.GetConcreteProxyClass(session.EntityMode).IsInstanceOfType(proxy);

			if (!alreadyNarrow)
			{
				if (ProxyWarnLog.IsWarnEnabled)
				{
					ProxyWarnLog.Warn("Narrowing proxy to " + persister.GetConcreteProxyClass(session.EntityMode) + " - this operation breaks ==");
				}

				if (obj != null)
				{
					proxiesByKey.Remove(key);
					return obj; //return the proxied object
				}
				else
				{
					proxy = (INHibernateProxy)persister.CreateProxy(key.Identifier, session);
					INHibernateProxy proxyOrig = proxiesByKey[key];
					proxiesByKey[key] = proxy; //overwrite old proxy
					if (proxyOrig != null)
					{
						bool readOnlyOrig = proxyOrig.HibernateLazyInitializer.ReadOnly;
						proxy.HibernateLazyInitializer.ReadOnly = readOnlyOrig;
					}
					return proxy;
				}
			}
			else
			{
				if (obj != null)
				{
					proxy.HibernateLazyInitializer.SetImplementation(obj);
				}
				return proxy;
			}
		}

		/// <summary>
		/// Return the existing proxy associated with the given <tt>EntityKey</tt>, or the
		/// third argument (the entity associated with the key) if no proxy exists. Init
		/// the proxy to the target implementation, if necessary.
		/// </summary>
		public object ProxyFor(IEntityPersister persister, EntityKey key, object impl)
		{
			if (!persister.HasProxy || key == null)
				return impl;

			INHibernateProxy proxy;
			if (proxiesByKey.TryGetValue(key, out proxy))
			{
				return NarrowProxy(proxy, persister, key, impl);
			}
			else
			{
				return impl;
			}
		}

		/// <summary>
		/// Return the existing proxy associated with the given <tt>EntityKey</tt>, or the
		/// argument (the entity associated with the key) if no proxy exists.
		/// (slower than the form above)
		/// </summary>
		public object ProxyFor(object impl)
		{
			EntityEntry e = GetEntry(impl);
			IEntityPersister p = e.Persister;
			return ProxyFor(p, session.GenerateEntityKey(e.Id, p), impl);
		}

		/// <summary> Get the entity that owns this persistent collection</summary>
		public object GetCollectionOwner(object key, ICollectionPersister collectionPersister)
		{
			return GetEntity(session.GenerateEntityKey(key, collectionPersister.OwnerEntityPersister));
		}

		/// <summary> Get the entity that owned this persistent collection when it was loaded </summary>
		/// <param name="collection">The persistent collection </param>
		/// <returns>
		/// The owner, if its entity ID is available from the collection's loaded key
		/// and the owner entity is in the persistence context; otherwise, returns null
		/// </returns>
		public virtual object GetLoadedCollectionOwnerOrNull(IPersistentCollection collection)
		{
			CollectionEntry ce = GetCollectionEntry(collection);
			if (ce.LoadedPersister == null)
			{
				return null; // early exit...
			}
			object loadedOwner = null;
			// TODO: an alternative is to check if the owner has changed; if it hasn't then
			// return collection.getOwner()
			object entityId = GetLoadedCollectionOwnerIdOrNull(ce);
			if (entityId != null)
			{
				loadedOwner = GetCollectionOwner(entityId, ce.LoadedPersister);
			}
			return loadedOwner;
		}

		/// <summary> Get the ID for the entity that owned this persistent collection when it was loaded </summary>
		/// <param name="collection">The persistent collection </param>
		/// <returns> the owner ID if available from the collection's loaded key; otherwise, returns null </returns>
		public virtual object GetLoadedCollectionOwnerIdOrNull(IPersistentCollection collection)
		{
			return GetLoadedCollectionOwnerIdOrNull(GetCollectionEntry(collection));
		}

		/// <summary> Get the ID for the entity that owned this persistent collection when it was loaded </summary>
		/// <param name="ce">The collection entry </param>
		/// <returns> the owner ID if available from the collection's loaded key; otherwise, returns null </returns>
		private object GetLoadedCollectionOwnerIdOrNull(CollectionEntry ce)
		{
			if (ce == null || ce.LoadedKey == null || ce.LoadedPersister == null)
			{
				return null;
			}
			// TODO: an alternative is to check if the owner has changed; if it hasn't then
			// get the ID from collection.getOwner()
			return ce.LoadedPersister.CollectionType.GetIdOfOwnerOrNull(ce.LoadedKey, session);
		}

		/// <summary> add a collection we just loaded up (still needs initializing)</summary>
		public void AddUninitializedCollection(ICollectionPersister persister, IPersistentCollection collection, object id)
		{
			CollectionEntry ce = new CollectionEntry(collection, persister, id, flushing);
			AddCollection(collection, ce, id);
		}

		/// <summary> add a detached uninitialized collection</summary>
		public void AddUninitializedDetachedCollection(ICollectionPersister persister, IPersistentCollection collection)
		{
			CollectionEntry ce = new CollectionEntry(persister, collection.Key);
			AddCollection(collection, ce, collection.Key);
		}

		/// <summary>
		/// Add a new collection (ie. a newly created one, just instantiated by the
		/// application, with no database state or snapshot)
		/// </summary>
		/// <param name="collection">The collection to be associated with the persistence context </param>
		/// <param name="persister"></param>
		public void AddNewCollection(ICollectionPersister persister, IPersistentCollection collection)
		{
			AddCollection(collection, persister);
		}

		/// <summary> Add an collection to the cache, with a given collection entry. </summary>
		/// <param name="coll">The collection for which we are adding an entry.</param>
		/// <param name="entry">The entry representing the collection. </param>
		/// <param name="key">The key of the collection's entry. </param>
		private void AddCollection(IPersistentCollection coll, CollectionEntry entry, object key)
		{
			collectionEntries[coll] = entry;
			CollectionKey collectionKey = new CollectionKey(entry.LoadedPersister, key, session.EntityMode);
			IPersistentCollection tempObject;
			collectionsByKey.TryGetValue(collectionKey, out tempObject);
			collectionsByKey[collectionKey] = coll;
			IPersistentCollection old = tempObject;
			if (old != null)
			{
				if (old == coll)
				{
					throw new AssertionFailure("bug adding collection twice");
				}
				// or should it actually throw an exception?
				old.UnsetSession(session);
				collectionEntries.Remove(old);
				// watch out for a case where old is still referenced
				// somewhere in the object graph! (which is a user error)
			}
		}

		/// <summary> Add a collection to the cache, creating a new collection entry for it </summary>
		/// <param name="collection">The collection for which we are adding an entry. </param>
		/// <param name="persister">The collection persister </param>
		private void AddCollection(IPersistentCollection collection, ICollectionPersister persister)
		{
			CollectionEntry ce = new CollectionEntry(persister, collection);
			collectionEntries[collection] = ce;
		}

		/// <summary>
		/// add an (initialized) collection that was created by another session and passed
		/// into update() (ie. one with a snapshot and existing state on the database)
		/// </summary>
		public void AddInitializedDetachedCollection(ICollectionPersister collectionPersister, IPersistentCollection collection)
		{
			if (collection.IsUnreferenced)
			{
				//treat it just like a new collection
				AddCollection(collection, collectionPersister);
			}
			else
			{
				CollectionEntry ce = new CollectionEntry(collection, session.Factory);
				AddCollection(collection, ce, collection.Key);
			}
		}

		/// <summary> add a collection we just pulled out of the cache (does not need initializing)</summary>
		public CollectionEntry AddInitializedCollection(ICollectionPersister persister, IPersistentCollection collection,
																										object id)
		{
			CollectionEntry ce = new CollectionEntry(collection, persister, id, flushing);
			ce.PostInitialize(collection);
			AddCollection(collection, ce, id);
			return ce;
		}

		/// <summary> Get the collection instance associated with the <tt>CollectionKey</tt></summary>
		public IPersistentCollection GetCollection(CollectionKey collectionKey)
		{
			IPersistentCollection result;
			if (collectionsByKey.TryGetValue(collectionKey, out result))
				return result;
			else
				return null;
		}

		/// <summary>
		/// Register a collection for non-lazy loading at the end of the two-phase load
		/// </summary>
		public void AddNonLazyCollection(IPersistentCollection collection)
		{
			nonlazyCollections.Add(collection);
		}

		/// <summary>
		/// Force initialization of all non-lazy collections encountered during
		/// the current two-phase load (actually, this is a no-op, unless this
		/// is the "outermost" load)
		/// </summary>
		public void InitializeNonLazyCollections()
		{
			if (loadCounter == 0)
			{
				log.Debug("initializing non-lazy collections");
				//do this work only at the very highest level of the load
				loadCounter++; //don't let this method be called recursively
				try
				{
					while (nonlazyCollections.Count > 0)
					{
						//note that each iteration of the loop may add new elements
						IPersistentCollection tempObject = nonlazyCollections[nonlazyCollections.Count - 1];
						nonlazyCollections.RemoveAt(nonlazyCollections.Count - 1);
						tempObject.ForceInitialization();
					}
				}
				finally
				{
					loadCounter--;
					ClearNullProperties();
				}
			}
		}

		private void ClearNullProperties()
		{
			nullAssociations.Clear();
		}

		/// <summary> Get the <tt>PersistentCollection</tt> object for an array</summary>
		public IPersistentCollection GetCollectionHolder(object array)
		{
			return (IPersistentCollection)arrayHolders[array];
		}

		/// <summary> Register a <tt>PersistentCollection</tt> object for an array.
		/// Associates a holder with an array - MUST be called after loading
		/// array, since the array instance is not created until endLoad().
		/// </summary>
		public void AddCollectionHolder(IPersistentCollection holder)
		{
			//TODO:refactor + make this method private
			arrayHolders[holder.GetValue()] = holder;
		}

		/// <summary>
		/// Remove the mapping of collection to holder during eviction of the owning entity
		/// </summary>
		public IPersistentCollection RemoveCollectionHolder(object array)
		{
			IPersistentCollection tempObject = (IPersistentCollection)arrayHolders[array];
			arrayHolders.Remove(array);
			return tempObject;
		}

		/// <summary> Get the snapshot of the pre-flush collection state</summary>
		public object GetSnapshot(IPersistentCollection coll)
		{
			return GetCollectionEntry(coll).Snapshot;
		}

		/// <summary>
		/// Get the collection entry for a collection passed to filter,
		/// which might be a collection wrapper, an array, or an unwrapped
		/// collection. Return null if there is no entry.
		/// </summary>
		public CollectionEntry GetCollectionEntryOrNull(object collection)
		{
			//commented in H3.2 if (collection==null) throw new TransientObjectException("Collection was not yet persistent");
			IPersistentCollection coll = collection as IPersistentCollection;
			if (coll == null)
			{
				coll = GetCollectionHolder(collection);
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

		/// <summary> Get an existing proxy by key</summary>
		public object GetProxy(EntityKey key)
		{
			INHibernateProxy result;
			if (proxiesByKey.TryGetValue(key, out result))
				return result;
			else
				return null;
		}

		/// <summary> Add a proxy to the session cache</summary>
		public void AddProxy(EntityKey key, INHibernateProxy proxy)
		{
			proxiesByKey[key] = proxy;
		}

		/// <summary> Remove a proxy from the session cache</summary>
		public object RemoveProxy(EntityKey key)
		{
			if (batchFetchQueue != null)
			{
				batchFetchQueue.RemoveBatchLoadableEntityKey(key);
				batchFetchQueue.RemoveSubselect(key);
			}
			INHibernateProxy tempObject;
			if (proxiesByKey.TryGetValue(key, out tempObject))
				proxiesByKey.Remove(key);
			return tempObject;
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

		/// <summary> Call this before begining a two-phase load</summary>
		public void BeforeLoad()
		{
			loadCounter++;
		}

		/// <summary> Call this after finishing a two-phase load</summary>
		public void AfterLoad()
		{
			loadCounter--;
		}

		/// <summary>
		/// Search the persistence context for an owner for the child object,
		/// given a collection role
		/// </summary>
		public object GetOwnerId(string entityName, string propertyName, object childEntity, IDictionary mergeMap)
		{
			string collectionRole = entityName + '.' + propertyName;
			IEntityPersister persister = session.Factory.GetEntityPersister(entityName);
			ICollectionPersister collectionPersister = session.Factory.GetCollectionPersister(collectionRole);

			object parent = parentsByChild[childEntity];
			if (parent != null)
			{
				var entityEntry = (EntityEntry) entityEntries[parent];
				//there maybe more than one parent, filter by type
				if (persister.IsSubclassEntityName(entityEntry.EntityName) && IsFoundInParent(propertyName, childEntity, persister, collectionPersister, parent))
				{
					return GetEntry(parent).Id;
				}
				parentsByChild.Remove(childEntity); // remove wrong entry
			}

			// iterate all the entities currently associated with the persistence context.
			foreach (DictionaryEntry entry in entityEntries)
			{
				var entityEntry = (EntityEntry) entry.Value;
				// does this entity entry pertain to the entity persister in which we are interested (owner)?
				if (persister.IsSubclassEntityName(entityEntry.EntityName))
				{
					object entityEntryInstance = entry.Key;

					//check if the managed object is the parent
					bool found = IsFoundInParent(propertyName, childEntity, persister, collectionPersister, entityEntryInstance);

					if (!found && mergeMap != null)
					{
						//check if the detached object being merged is the parent
						object unmergedInstance = mergeMap[entityEntryInstance];
						object unmergedChild = mergeMap[childEntity];
						if (unmergedInstance != null && unmergedChild != null)
						{
							found = IsFoundInParent(propertyName, unmergedChild, persister, collectionPersister, unmergedInstance);
						}
					}

					if (found)
					{
						return entityEntry.Id;
					}
				}
			}

			// if we get here, it is possible that we have a proxy 'in the way' of the merge map resolution...
			// 		NOTE: decided to put this here rather than in the above loop as I was nervous about the performance
			//		of the loop-in-loop especially considering this is far more likely the 'edge case'
			if (mergeMap != null)
			{
				foreach (DictionaryEntry mergeMapEntry in mergeMap)
				{
					var proxy = mergeMapEntry.Key as INHibernateProxy;
					if (proxy != null)
					{
						if (persister.IsSubclassEntityName(proxy.HibernateLazyInitializer.EntityName))
						{
							bool found = IsFoundInParent(propertyName, childEntity, persister, collectionPersister, mergeMap[proxy]);
							if (!found)
							{
								found = IsFoundInParent(propertyName, mergeMap[childEntity], persister, collectionPersister, mergeMap[proxy]);
							}
							if (found)
							{
								return proxy.HibernateLazyInitializer.Identifier;
							}
						}
					}
				}
			}

			return null;
		}

		private bool IsFoundInParent(string property, object childEntity, IEntityPersister persister, ICollectionPersister collectionPersister, object potentialParent)
		{
			object collection = persister.GetPropertyValue(potentialParent, property, session.EntityMode);
			return collection != null && NHibernateUtil.IsInitialized(collection) && collectionPersister.CollectionType.Contains(collection, childEntity, session);
		}

		/// <summary>
		/// Search the persistence context for an index of the child object, given a collection role
		/// </summary>
		public object GetIndexInOwner(string entity, string property, object childEntity, IDictionary mergeMap)
		{
			IEntityPersister persister = session.Factory.GetEntityPersister(entity);
			ICollectionPersister cp = session.Factory.GetCollectionPersister(entity + '.' + property);

			// try cache lookup first
			object parent = parentsByChild[childEntity];
			if (parent != null)
			{
				var entityEntry = (EntityEntry) entityEntries[parent];
				//there maybe more than one parent, filter by type
				if (persister.IsSubclassEntityName(entityEntry.EntityName))
				{
					object index = GetIndexInParent(property, childEntity, persister, cp, parent);

					if (index == null && mergeMap != null)
					{
						object unmergedInstance = mergeMap[parent];
						object unmergedChild = mergeMap[childEntity];
						if (unmergedInstance != null && unmergedChild != null)
						{
							index = GetIndexInParent(property, unmergedChild, persister, cp, unmergedInstance);
						}
					}
					if (index != null)
					{
						return index;
					}
				}
				else
				{
					parentsByChild.Remove(childEntity); // remove wrong entry
				}
			}

			//Not found in cache, proceed
			foreach (DictionaryEntry me in entityEntries)
			{
				var ee = (EntityEntry) me.Value;
				if (persister.IsSubclassEntityName(ee.EntityName))
				{
					object instance = me.Key;
					object index = GetIndexInParent(property, childEntity, persister, cp, instance);

					if (index == null && mergeMap != null)
					{
						object unmergedInstance = mergeMap[instance];
						object unmergedChild = mergeMap[childEntity];
						if (unmergedInstance != null && unmergedChild != null)
						{
							index = GetIndexInParent(property, unmergedChild, persister, cp, unmergedInstance);
						}
					}

					if (index != null)
					{
						return index;
					}
				}
			}
			return null;
		}

		private object GetIndexInParent(string property, object childEntity, IEntityPersister persister, ICollectionPersister collectionPersister, object potentialParent)
		{
			object collection = persister.GetPropertyValue(potentialParent, property, session.EntityMode);
			if (collection != null && NHibernateUtil.IsInitialized(collection))
			{
				return collectionPersister.CollectionType.IndexOf(collection, childEntity);
			}
			return null;
		}

		/// <summary>
		/// Record the fact that the association belonging to the keyed entity is null.
		/// </summary>
		public void AddNullProperty(EntityKey ownerKey, string propertyName)
		{
			nullAssociations.Add(new AssociationKey(ownerKey, propertyName));
		}

		/// <summary> Is the association property belonging to the keyed entity null?</summary>
		public bool IsPropertyNull(EntityKey ownerKey, string propertyName)
		{
			return nullAssociations.Contains(new AssociationKey(ownerKey, propertyName));
		}

		/// <inheritdoc />
		public void SetReadOnly(object entityOrProxy, bool readOnly)
		{
			if (entityOrProxy == null)
				throw new ArgumentNullException("entityOrProxy");
		
			if (IsReadOnly(entityOrProxy) == readOnly)
				return;
	
			if (entityOrProxy is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy)entityOrProxy;
				SetProxyReadOnly(proxy, readOnly);
				if (NHibernateUtil.IsInitialized(proxy))
				{
					SetEntityReadOnly(proxy.HibernateLazyInitializer.GetImplementation(), readOnly);
				}
			}
			else
			{
				SetEntityReadOnly(entityOrProxy, readOnly);
				
				// PersistenceContext.proxyFor( entity ) returns entity if there is no proxy for that entity
				// so need to check the return value to be sure it is really a proxy
				object maybeProxy = this.Session.PersistenceContext.ProxyFor(entityOrProxy);
				if (maybeProxy is INHibernateProxy )
				{
					SetProxyReadOnly((INHibernateProxy)maybeProxy, readOnly);
				}
			}
		}
		
		private void SetProxyReadOnly(INHibernateProxy proxy, bool readOnly)
		{
			if (proxy.HibernateLazyInitializer.Session != this.Session)
			{
				throw new AssertionFailure("Attempt to set a proxy to read-only that is associated with a different session");
			}
			proxy.HibernateLazyInitializer.ReadOnly = readOnly;
		}

		private void SetEntityReadOnly(object entity, bool readOnly)
		{
			EntityEntry entry = GetEntry(entity);
			if (entry == null)
			{
				throw new TransientObjectException("Instance was not associated with this persistence context");
			}
			entry.SetReadOnly(readOnly, entity);
			hasNonReadOnlyEntities |= !readOnly;
		}
		
		/// <inheritdoc />
		public bool IsReadOnly(object entityOrProxy)
		{
			if (entityOrProxy == null)
			{
				throw new AssertionFailure("object must be non-null.");
			}
			bool isReadOnly;
			if (entityOrProxy is INHibernateProxy)
			{
				isReadOnly = ((INHibernateProxy)entityOrProxy).HibernateLazyInitializer.ReadOnly;
			}
			else
			{
				EntityEntry ee = GetEntry(entityOrProxy);
				if (ee == null)
				{
					throw new TransientObjectException("Instance was not associated with this persistence context" );
				}
				isReadOnly = ee.IsReadOnly;
			}
			return isReadOnly;
		}
		
		public void ReplaceDelayedEntityIdentityInsertKeys(EntityKey oldKey, object generatedId)
		{
			object tempObject = entitiesByKey[oldKey];
			entitiesByKey.Remove(oldKey);
			object entity = tempObject;
			object tempObject2 = entityEntries[entity];
			entityEntries.Remove(entity);
			var oldEntry = (EntityEntry) tempObject2;
			parentsByChild.Clear();

			var newKey = Session.GenerateEntityKey(generatedId, oldEntry.Persister);
			AddEntity(newKey, entity);
			AddEntry(entity, oldEntry.Status, oldEntry.LoadedState, oldEntry.RowId, generatedId, oldEntry.Version,
					 oldEntry.LockMode, oldEntry.ExistsInDatabase, oldEntry.Persister, oldEntry.IsBeingReplicated,
					 oldEntry.LoadedWithLazyPropertiesUnfetched);
		}

		public bool IsLoadFinished
		{
			get
			{
				return loadCounter == 0;
			}
		}

		public void AddChildParent(object child, object parent)
		{
			parentsByChild[child] = parent;
		}

		public void RemoveChildParent(object child)
		{
			parentsByChild.Remove(child);
		}

		#endregion

		public override string ToString()
		{
			return new StringBuilder()
				.Append("PersistenceContext[entityKeys=")
				.Append(CollectionPrinter.ToString(entitiesByKey.Keys))
				.Append(",collectionKeys=")
				.Append(CollectionPrinter.ToString(collectionsByKey.Keys))
				.Append("]")
				.ToString();
		}

		#region IDeserializationCallback Members
		internal void SetSession(ISessionImplementor session)
		{
			this.session = session;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			log.Debug("Deserialization callback persistent-context");

			// during deserialization, we need to reconnect all proxies and
			// collections to this session, as well as the EntityEntry and
			// CollectionEntry instances; these associations are transient
			// because serialization is used for different things.
			parentsByChild = IdentityMap.Instantiate(InitCollectionSize);

			// OnDeserialization() must be called manually on all Dictionaries and Hashtables,
			// otherwise they are still empty at this point (the .NET deserialization code calls
			// OnDeserialization() on them AFTER it calls the current method).
			entitiesByKey.OnDeserialization(sender);
			entitiesByUniqueKey.OnDeserialization(sender);
			((IDeserializationCallback)entityEntries).OnDeserialization(sender);
			proxiesByKey.OnDeserialization(sender);
			entitySnapshotsByKey.OnDeserialization(sender);
			((IDeserializationCallback)arrayHolders).OnDeserialization(sender);
			((IDeserializationCallback)collectionEntries).OnDeserialization(sender);
			collectionsByKey.OnDeserialization(sender);

			// If nullifiableEntityKeys is once used in the current method, HashSets will need
			// an OnDeserialization() method.
			//nullifiableEntityKeys.OnDeserialization(sender);

			if (unownedCollections != null)
			{
				unownedCollections.OnDeserialization(sender);
			}

			// TODO NH: "reconnect" EntityKey with session.factory and create a test for serialization of StatefulPersistenceContext
			foreach (DictionaryEntry collectionEntry in collectionEntries)
			{
				try
				{
					((IPersistentCollection)collectionEntry.Key).SetCurrentSession(session);
					CollectionEntry ce = (CollectionEntry)collectionEntry.Value;
					if (ce.Role != null)
					{
						ce.AfterDeserialize(Session.Factory);
					}

				}
				catch (HibernateException he)
				{
					throw new InvalidOperationException(he.Message);
				}
			}

			List<EntityKey> keysToRemove = new List<EntityKey>();
			foreach (KeyValuePair<EntityKey, INHibernateProxy> p in proxiesByKey)
			{
				if (p.Value != null)
				{
					(p.Value).HibernateLazyInitializer.Session = session;
				}
				else
				{
					// the proxy was pruned during the serialization process because the target had been instantiated.
					keysToRemove.Add(p.Key);
				}
			}
			for (int i = 0; i < keysToRemove.Count; i++)
				proxiesByKey.Remove(keysToRemove[i]);

			foreach (EntityEntry e in entityEntries.Values)
			{
				try
				{
					e.Persister = session.Factory.GetEntityPersister(e.EntityName);
				}
				catch (MappingException me)
				{
					throw new InvalidOperationException(me.Message);
				}
			}
		}

		#endregion

		#region ISerializable Members
		internal StatefulPersistenceContext(SerializationInfo info, StreamingContext context)
		{
			loadCounter = 0;
			flushing = false;
			cascading = 0;
			entitiesByKey = (Dictionary<EntityKey, object>)info.GetValue("context.entitiesByKey", typeof(Dictionary<EntityKey, object>));
			entitiesByUniqueKey = (Dictionary<EntityUniqueKey, object>)info.GetValue("context.entitiesByUniqueKey", typeof(Dictionary<EntityUniqueKey, object>));
			entityEntries = (IdentityMap)info.GetValue("context.entityEntries", typeof(IdentityMap));
			proxiesByKey = (Dictionary<EntityKey, INHibernateProxy>)info.GetValue("context.proxiesByKey", typeof(Dictionary<EntityKey, INHibernateProxy>));
			entitySnapshotsByKey = (Dictionary<EntityKey, object>)info.GetValue("context.entitySnapshotsByKey", typeof(Dictionary<EntityKey, object>));
			arrayHolders = (IdentityMap)info.GetValue("context.arrayHolders", typeof(IdentityMap));
			collectionEntries = (IdentityMap)info.GetValue("context.collectionEntries", typeof(IdentityMap));
			collectionsByKey = (Dictionary<CollectionKey, IPersistentCollection>)info.GetValue("context.collectionsByKey", typeof(Dictionary<CollectionKey, IPersistentCollection>));
			nullifiableEntityKeys = (HashSet<EntityKey>)info.GetValue("context.nullifiableEntityKeys", typeof(HashSet<EntityKey>));
			unownedCollections = (Dictionary<CollectionKey, IPersistentCollection>)info.GetValue("context.unownedCollections", typeof(Dictionary<CollectionKey, IPersistentCollection>));
			hasNonReadOnlyEntities = info.GetBoolean("context.hasNonReadOnlyEntities");
			defaultReadOnly = info.GetBoolean("context.defaultReadOnly");
			InitTransientState();
		}

#if NET_4_0
		[SecurityCritical]
#else
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			log.Debug("serializing persistent-context");

			info.AddValue("context.entitiesByKey", entitiesByKey, typeof(Dictionary<EntityKey, object>));
			info.AddValue("context.entitiesByUniqueKey", entitiesByUniqueKey, typeof(Dictionary<EntityUniqueKey, object>));
			info.AddValue("context.entityEntries", entityEntries, typeof(IdentityMap));
			info.AddValue("context.proxiesByKey", proxiesByKey, typeof(Dictionary<EntityKey, INHibernateProxy>));
			info.AddValue("context.entitySnapshotsByKey", entitySnapshotsByKey, typeof(Dictionary<EntityKey, object>));
			info.AddValue("context.arrayHolders", arrayHolders, typeof(IdentityMap));
			info.AddValue("context.collectionEntries", collectionEntries, typeof(IdentityMap));
			info.AddValue("context.collectionsByKey", collectionsByKey, typeof(Dictionary<CollectionKey, IPersistentCollection>));
			info.AddValue("context.nullifiableEntityKeys", nullifiableEntityKeys, typeof(HashSet<EntityKey>));
			info.AddValue("context.unownedCollections", unownedCollections, typeof(Dictionary<CollectionKey, IPersistentCollection>));
			info.AddValue("context.hasNonReadOnlyEntities", hasNonReadOnlyEntities);
			info.AddValue("context.defaultReadOnly", defaultReadOnly);
		}
		#endregion
	}
}
