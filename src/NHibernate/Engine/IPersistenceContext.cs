using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine.Loading;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace NHibernate.Engine
{
	/// <summary>
	/// Holds the state of the persistence context, including the
	/// first-level cache, entries, snapshots, proxies, etc.
	/// </summary>
	public partial interface IPersistenceContext
	{
		bool IsStateless { get;}

		/// <summary>
		/// Get the session to which this persistence context is bound.
		/// </summary>
		ISessionImplementor Session { get;}

		/// <summary>
		/// Retrieve this persistence context's managed load context.
		/// </summary>
		LoadContexts LoadContexts { get;}

		/// <summary>
		/// Get the <tt>BatchFetchQueue</tt>, instantiating one if necessary.
		/// </summary>
		BatchFetchQueue BatchFetchQueue { get;}

		/// <summary> Retrieve the set of EntityKeys representing nullifiable references</summary>
		ISet<EntityKey> NullifiableEntityKeys { get;}

		/// <summary> Get the mapping from key value to entity instance</summary>
		IDictionary<EntityKey, object> EntitiesByKey { get;}

		/// <summary> Get the mapping from entity instance to entity entry</summary>
		IDictionary EntityEntries { get;}

		/// <summary> Get the mapping from collection instance to collection entry</summary>
		IDictionary CollectionEntries { get;}

		/// <summary> Get the mapping from collection key to collection instance</summary>
		IDictionary<CollectionKey, IPersistentCollection> CollectionsByKey { get;}

		/// <summary> How deep are we cascaded?</summary>
		int CascadeLevel { get;}

		/// <summary>Is a flush cycle currently in process?</summary>
		/// <remarks>Called before and after the flushcycle</remarks>
		bool Flushing { get; set;}
		
		/// <summary>
		/// The read-only status for entities (and proxies) loaded into this persistence context.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When a proxy is initialized, the loaded entity will have the same read-only
		/// setting as the uninitialized proxy has, regardless of the persistence context's
		/// current setting.
		/// </para>
		/// <para>
		/// To change the read-only setting for a particular entity or proxy that is already
		/// in the current persistence context, use <see cref="IPersistenceContext.SetReadOnly(object, bool)" />.
		/// </para>
		/// </remarks>
		/// <seealso cref="IPersistenceContext.IsReadOnly(object)" />
		/// <seealso cref="IPersistenceContext.SetReadOnly(object, bool)" />
		bool DefaultReadOnly { get; set; }

		/// <summary> Add a collection which has no owner loaded</summary>
		void AddUnownedCollection(CollectionKey key, IPersistentCollection collection);

		/// <summary>
		/// Get and remove a collection whose owner is not yet loaded,
		/// when its owner is being loaded
		/// </summary>
		IPersistentCollection UseUnownedCollection(CollectionKey key);

		/// <summary> Clear the state of the persistence context</summary>
		void Clear();

		/// <summary>False if we know for certain that all the entities are read-only</summary>
		bool HasNonReadOnlyEntities { get;}

		/// <summary> Set the status of an entry</summary>
		void SetEntryStatus(EntityEntry entry, Status status);

		/// <summary> Called after transactions end</summary>
		void AfterTransactionCompletion();

		/// <summary>
		/// Get the current state of the entity as known to the underlying
		/// database, or null if there is no corresponding row
		/// </summary>
		object[] GetDatabaseSnapshot(object id, IEntityPersister persister);

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
		object[] GetCachedDatabaseSnapshot(EntityKey key);

		/// <summary>
		/// Get the values of the natural id fields as known to the underlying
		/// database, or null if the entity has no natural id or there is no
		/// corresponding row.
		/// </summary>
		object[] GetNaturalIdSnapshot(object id, IEntityPersister persister);

		/// <summary> Add a canonical mapping from entity key to entity instance</summary>
		void AddEntity(EntityKey key, object entity);

		/// <summary>
		/// Get the entity instance associated with the given <tt>EntityKey</tt>
		/// </summary>
		object GetEntity(EntityKey key);

		/// <summary> Is there an entity with the given key in the persistence context</summary>
		bool ContainsEntity(EntityKey key);

		/// <summary>
		/// Remove an entity from the session cache, also clear
		/// up other state associated with the entity, all except
		/// for the <tt>EntityEntry</tt>
		/// </summary>
		object RemoveEntity(EntityKey key);

		/// <summary> Get an entity cached by unique key</summary>
		object GetEntity(EntityUniqueKey euk);

		/// <summary> Add an entity to the cache by unique key</summary>
		void AddEntity(EntityUniqueKey euk, object entity);

		/// <summary>
		/// Retrieve the EntityEntry representation of the given entity.
		/// </summary>
		/// <param name="entity">The entity for which to locate the EntityEntry. </param>
		/// <returns> The EntityEntry for the given entity. </returns>
		EntityEntry GetEntry(object entity);

		/// <summary> Remove an entity entry from the session cache</summary>
		EntityEntry RemoveEntry(object entity);

		/// <summary> Is there an EntityEntry for this instance?</summary>
		bool IsEntryFor(object entity);

		/// <summary> Get the collection entry for a persistent collection</summary>
		CollectionEntry GetCollectionEntry(IPersistentCollection coll);

		/// <summary> Adds an entity to the internal caches.</summary>
		EntityEntry AddEntity(object entity, Status status, object[] loadedState, EntityKey entityKey, object version,
													LockMode lockMode, bool existsInDatabase, IEntityPersister persister,
													bool disableVersionIncrement, bool lazyPropertiesAreUnfetched);

		/// <summary>
		/// Generates an appropriate EntityEntry instance and adds it
		/// to the event source's internal caches.
		/// </summary>
		EntityEntry AddEntry(object entity, Status status, object[] loadedState, object rowId, object id, object version,
		                     LockMode lockMode, bool existsInDatabase, IEntityPersister persister, bool disableVersionIncrement,
		                     bool lazyPropertiesAreUnfetched);

		/// <summary> Is the given collection associated with this persistence context?</summary>
		bool ContainsCollection(IPersistentCollection collection);

		/// <summary> Is the given proxy associated with this persistence context?</summary>
		bool ContainsProxy(INHibernateProxy proxy);

		/// <summary>
		/// Takes the given object and, if it represents a proxy, reassociates it with this event source.
		/// </summary>
		/// <param name="value">The possible proxy to be reassociated. </param>
		/// <returns> Whether the passed value represented an actual proxy which got initialized. </returns>
		bool ReassociateIfUninitializedProxy(object value);

		/// <summary>
		/// If a deleted entity instance is re-saved, and it has a proxy, we need to
		/// reset the identifier of the proxy
		/// </summary>
		void ReassociateProxy(object value, object id);

		/// <summary>
		/// Get the entity instance underlying the given proxy, throwing
		/// an exception if the proxy is uninitialized. If the given object
		/// is not a proxy, simply return the argument.
		/// </summary>
		object Unproxy(object maybeProxy);

		/// <summary>
		/// Possibly unproxy the given reference and reassociate it with the current session.
		/// </summary>
		/// <param name="maybeProxy">The reference to be unproxied if it currently represents a proxy. </param>
		/// <returns> The unproxied instance. </returns>
		object UnproxyAndReassociate(object maybeProxy);

		/// <summary>
		/// Attempts to check whether the given key represents an entity already loaded within the
		/// current session.
		/// </summary>
		/// <param name="obj">The entity reference against which to perform the uniqueness check.</param>
		/// <param name="key">The entity key.</param>
		void CheckUniqueness(EntityKey key, object obj);

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
		object NarrowProxy(INHibernateProxy proxy, IEntityPersister persister, EntityKey key, object obj);

		/// <summary>
		/// Return the existing proxy associated with the given <tt>EntityKey</tt>, or the
		/// third argument (the entity associated with the key) if no proxy exists. Init
		/// the proxy to the target implementation, if necessary.
		/// </summary>
		object ProxyFor(IEntityPersister persister, EntityKey key, object impl);

		/// <summary>
		/// Return the existing proxy associated with the given <tt>EntityKey</tt>, or the
		/// argument (the entity associated with the key) if no proxy exists.
		/// (slower than the form above)
		/// </summary>
		object ProxyFor(object impl);

		/// <summary> Get the entity that owns this persistent collection</summary>
		object GetCollectionOwner(object key, ICollectionPersister collectionPersister);

		/// <summary> Get the entity that owned this persistent collection when it was loaded </summary>
		/// <param name="collection">The persistent collection </param>
		/// <returns>
		/// The owner if its entity ID is available from the collection's loaded key
		/// and the owner entity is in the persistence context; otherwise, returns null
		/// </returns>
		object GetLoadedCollectionOwnerOrNull(IPersistentCollection collection);

		/// <summary> Get the ID for the entity that owned this persistent collection when it was loaded </summary>
		/// <param name="collection">The persistent collection </param>
		/// <returns> the owner ID if available from the collection's loaded key; otherwise, returns null </returns>
		object GetLoadedCollectionOwnerIdOrNull(IPersistentCollection collection);

		/// <summary> add a collection we just loaded up (still needs initializing)</summary>
		void AddUninitializedCollection(ICollectionPersister persister, IPersistentCollection collection, object id);

		/// <summary> add a detached uninitialized collection</summary>
		void AddUninitializedDetachedCollection(ICollectionPersister persister, IPersistentCollection collection);

		/// <summary>
		/// Add a new collection (ie. a newly created one, just instantiated by the
		/// application, with no database state or snapshot)
		/// </summary>
		/// <param name="collection">The collection to be associated with the persistence context </param>
		/// <param name="persister"></param>
		void AddNewCollection(ICollectionPersister persister, IPersistentCollection collection);

		/// <summary>
		/// add an (initialized) collection that was created by another session and passed
		/// into update() (ie. one with a snapshot and existing state on the database)
		/// </summary>
		void AddInitializedDetachedCollection(ICollectionPersister collectionPersister, IPersistentCollection collection);

		/// <summary> add a collection we just pulled out of the cache (does not need initializing)</summary>
		CollectionEntry AddInitializedCollection(ICollectionPersister persister, IPersistentCollection collection, object id);

		/// <summary> Get the collection instance associated with the <tt>CollectionKey</tt></summary>
		IPersistentCollection GetCollection(CollectionKey collectionKey);

		/// <summary>
		/// Register a collection for non-lazy loading at the end of the two-phase load
		/// </summary>
		void AddNonLazyCollection(IPersistentCollection collection);

		/// <summary>
		/// Force initialization of all non-lazy collections encountered during
		/// the current two-phase load (actually, this is a no-op, unless this
		/// is the "outermost" load)
		/// </summary>
		void InitializeNonLazyCollections();

		/// <summary> Get the <tt>PersistentCollection</tt> object for an array</summary>
		IPersistentCollection GetCollectionHolder(object array);

		/// <summary> Register a <tt>PersistentCollection</tt> object for an array.
		/// Associates a holder with an array - MUST be called after loading
		/// array, since the array instance is not created until endLoad().
		/// </summary>
		void AddCollectionHolder(IPersistentCollection holder);

		/// <summary>
		/// Remove the mapping of collection to holder during eviction of the owning entity
		/// </summary>
		IPersistentCollection RemoveCollectionHolder(object array);

		/// <summary> Get the snapshot of the pre-flush collection state</summary>
		object GetSnapshot(IPersistentCollection coll);

		/// <summary>
		/// Get the collection entry for a collection passed to filter,
		/// which might be a collection wrapper, an array, or an unwrapped
		/// collection. Return null if there is no entry.
		/// </summary>
		CollectionEntry GetCollectionEntryOrNull(object collection);

		/// <summary> Get an existing proxy by key</summary>
		object GetProxy(EntityKey key);

		/// <summary> Add a proxy to the session cache</summary>
		void AddProxy(EntityKey key, INHibernateProxy proxy);

		/// <summary> Remove a proxy from the session cache</summary>
		object RemoveProxy(EntityKey key);

		/// <summary> Called before cascading</summary>
		int IncrementCascadeLevel();

		/// <summary> Called after cascading</summary>
		int DecrementCascadeLevel();

		/// <summary> Call this before beginning a two-phase load</summary>
		void BeforeLoad();

		/// <summary> Call this after finishing a two-phase load</summary>
		void AfterLoad();

		/// <summary>
		/// Search the persistence context for an owner for the child object,
		/// given a collection role
		/// </summary>
		object GetOwnerId(string entity, string property, object childObject, IDictionary mergeMap);

		/// <summary>
		/// Search the persistence context for an index of the child object, given a collection role
		/// </summary>
		object GetIndexInOwner(string entity, string property, object childObject, IDictionary mergeMap);

		/// <summary>
		/// Record the fact that the association belonging to the keyed entity is null.
		/// </summary>
		void AddNullProperty(EntityKey ownerKey, string propertyName);

		/// <summary> Is the association property belonging to the keyed entity null?</summary>
		bool IsPropertyNull(EntityKey ownerKey, string propertyName);

		/// <summary>
		/// Change the read-only status of an entity (or proxy).
		/// </summary>
		/// <remarks>
		/// <para>
		/// Read-only entities can be modified, but changes are not persisted. They are not dirty-checked 
		/// and snapshots of persistent state are not maintained. 
		/// </para>
		/// <para>
		/// Immutable entities cannot be made read-only.
		/// </para>
		/// <para>
		/// To set the <em>default</em> read-only setting for entities and proxies that are loaded 
		/// into the persistence context, see <see cref="IPersistenceContext.DefaultReadOnly" />.
		/// </para>
		/// </remarks>
		/// <param name="entityOrProxy">An entity (or <see cref="NHibernate.Proxy.INHibernateProxy" />).</param>
		/// <param name="readOnly">If <c>true</c>, the entity or proxy is made read-only; if <c>false</c>, it is made modifiable.</param>
		/// <seealso cref="IPersistenceContext.DefaultReadOnly" />
		/// <seealso cref="IPersistenceContext.IsReadOnly(object)" />
		void SetReadOnly(object entityOrProxy, bool readOnly);

		/// <summary>
		/// Is the specified entity (or proxy) read-only?
		/// </summary>
		/// <param name="entityOrProxy">An entity (or <see cref="NHibernate.Proxy.INHibernateProxy" />)</param>
		/// <returns>
		/// <c>true</c> if the entity or proxy is read-only, otherwise <c>false</c>.
		/// </returns>
		/// <seealso cref="IPersistenceContext.DefaultReadOnly" />
		/// <seealso cref="IPersistenceContext.SetReadOnly(object, bool)" />
		bool IsReadOnly(object entityOrProxy);
		
		void ReplaceDelayedEntityIdentityInsertKeys(EntityKey oldKey, object generatedId);

		/// <summary>Is in a two-phase load? </summary>
		bool IsLoadFinished { get; }

		/// <summary>
		/// Add child/parent relation to cache for cascading operations
		/// </summary>
		/// <param name="child">The child.</param>
		/// <param name="parent">The parent.</param>
		void AddChildParent(object child, object parent);

		/// <summary>
		/// Remove child/parent relation from cache
		/// </summary>
		/// <param name="child">The child.</param>
		void RemoveChildParent(object child);
	}
}
