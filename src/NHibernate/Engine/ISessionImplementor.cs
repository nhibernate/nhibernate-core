using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Collection;
using NHibernate.Engine.Query;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using System.Collections.Generic;

namespace NHibernate.Engine
{
	/// <summary>
	/// Defines the internal contract between the <c>Session</c> and other parts of Hibernate
	/// such as implementors of <c>Type</c> or <c>ClassPersister</c>
	/// </summary>
	public interface ISessionImplementor : ISession
	{
		/// <summary>
		/// Get the pre-flush identifier of the collection
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		object GetLoadedCollectionKey(IPersistentCollection collection);

		/// <summary>
		/// Get the snapshot of the pre-flush collection state
		/// </summary>
		object GetSnapshot(IPersistentCollection collection);

		/// <summary>
		/// Initialize the collection (if not already initialized)
		/// </summary>
		/// <param name="coolection"></param>
		/// <param name="writing"></param>
		void InitializeCollection(IPersistentCollection coolection, bool writing);

		/// <summary>
		/// Is this the "inverse" end of a bidirectional association?
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		bool IsInverseCollection(IPersistentCollection collection);

		/// <summary>
		/// new in h2.1 and no javadoc
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="resultSetId"></param>
		/// <returns></returns>
		IPersistentCollection GetLoadingCollection(ICollectionPersister persister, object id, object resultSetId);

		/// <summary>
		/// new in h2.1 and no javadoc
		/// </summary>
		void EndLoadingCollections(ICollectionPersister persister, object resultSetId);

		/// <summary>
		/// new in h2.1 and no javadoc
		/// </summary>
		void AfterLoad();

		/// <summary>
		/// new in h2.1 and no javadoc
		/// </summary>
		void BeforeLoad();

		/// <summary>
		/// new in h2.1 and no javadoc
		/// </summary>
		void InitializeNonLazyCollections();

		/// <summary>
		/// Gets the NHibernate collection wrapper from the ISession.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <returns>
		/// A NHibernate wrapped collection.
		/// </returns>
		object GetCollection(string role, object id, object owner);

		// NH-268
		/// <summary>
		/// Load an instance without checking if it was deleted. If it does not exist and isn't nullable, throw an exception.
		/// This method may create a new proxy or return an existing proxy.
		/// </summary>
		/// <param name="persistentClass">The <see cref="System.Type"/> to load.</param>
		/// <param name="id">The identifier of the object in the database.</param>
		/// <param name="isNullable">Allow null instance</param>
		/// <param name="eager">When enabled, the object is eagerly fetched.</param>
		/// <returns>
		/// A proxy of the object or an instance of the object if the <c>persistentClass</c> does not have a proxy.
		/// </returns>
		/// <exception cref="ObjectNotFoundException">No object could be found with that <c>id</c>.</exception>
		object InternalLoad(System.Type persistentClass, object id, bool eager, bool isNullable);

		/// <summary>
		/// Load an instance immediately. Do not return a proxy.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object ImmediateLoad(System.Type persistentClass, object id);

		/// <summary>
		/// Load an instance by a unique key that is not the primary key.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object LoadByUniqueKey(System.Type persistentClass, string uniqueKeyPropertyName, object id);

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
		/// After actually deleting a row, record the fact that the instance no longer exists on the
		/// database (needed for identity-column key generation)
		/// </summary>
		/// <param name="obj"></param>
		void PostDelete(object obj);

		/// <summary>
		/// Execute a <c>Find()</c> query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IList Find(string query, QueryParameters parameters);

		void Find(string query, QueryParameters parameters, IList results);

		/// <summary>
		/// Strongly-typed version of <see cref="Find(string, QueryParameters)" />
		/// </summary>
		IList<T> Find<T>(string query, QueryParameters queryParameters);

		/// <summary>
		/// Strongly-typed version of <see cref="Find(CriteriaImpl)" />
		/// </summary>
		IList<T> Find<T>(CriteriaImpl criteria);

		void Find(CriteriaImpl criteria, IList results);

		IList Find(CriteriaImpl criteria);

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
		IList Filter(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Execute a filter (strongly-typed version).
		/// </summary>
		IList<T> Filter<T>(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Collection from a filter
		/// </summary>
		IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Strongly-typed version of <see cref="EnumerableFilter(object, string, QueryParameters)" />
		/// </summary>
		IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters);

		/// <summary>
		/// Get the <c>IEntityPersister</c> for an object
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		IEntityPersister GetEntityPersister(object obj);

		/// <summary>
		/// Add an uninitialized instance of an entity class, as a placeholder to ensure object identity.
		/// Must be called before <c>PostHydrate()</c>
		/// </summary>
		void AddUninitializedEntity(EntityKey key, object obj, LockMode lockMode);

		/// <summary>
		/// Register the "hydrated" state of an entity instance, after the first step of 2-phase loading
		/// </summary>
		void PostHydrate(IEntityPersister persister, object id, object[] values, object obj, LockMode lockMode);

		/// <summary>
		/// Perform the second step of 2-phase load (ie. fully initialize the entity instance)
		/// </summary>
		void InitializeEntity(object obj);

		/// <summary>
		/// Get the entity instance associated with the given <c>EntityKey</c>
		/// </summary>
		object GetEntity(EntityKey key);

		/// <summary>
		/// Return the existing proxy associated with the given <c>EntityKey</c>, or the second
		/// argument (the entity associated with the key) if no proxy exists.
		/// </summary>
		/// <param name="persister">The <see cref="IEntityPersister"/> to see if it should be Proxied.</param>
		/// <param name="key">The <see cref="EntityKey"/> that identifies the entity.</param>
		/// <param name="impl"></param>
		/// <returns>Returns a the Proxy for the class or the parameter impl.</returns>
		object ProxyFor(IEntityPersister persister, EntityKey key, object impl);

		/// <summary>
		/// Return the existing proxy associated with the given object. (Slower than the form above)
		/// </summary>
		object ProxyFor(object impl);

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
		/// (Also we shold release cache softlocks). May be called multiple times during the transaction
		/// completion process.
		/// </summary>
		void AfterTransactionCompletion(bool successful, ITransaction tx);

		/// <summary>
		/// Return the identifier of the persistent object, or null if transient
		/// </summary>
		object GetEntityIdentifier(object obj);

		/// <summary>
		/// Return the identifer of the persistent or transient object, or throw
		/// an exception if the instance is "unsaved"
		/// </summary>
		object GetEntityIdentifierIfNotUnsaved(object obj);

		bool IsSaved(object obj);

		/// <summary>
		/// Instantiate the entity class, initializing with the given identifier
		/// </summary>
		object Instantiate(System.Type clazz, object id);

		/// <summary>
		/// Set the lock mode of the entity to the given lock mode
		/// </summary>
		void SetLockMode(object entity, LockMode lockMode);

		/// <summary>
		/// Get the current version of the entity
		/// </summary>
		object GetVersion(object entity);

		/// <summary>
		/// Get the lock mode of the entity
		/// </summary>
		LockMode GetLockMode(object entity);

		BatchFetchQueue BatchFetchQueue { get; }

		/// <summary>
		/// Execute an SQL Query
		/// </summary>
		IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters);

		void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results);

		/// <summary>
		/// Strongly-typed version of <see cref="List(NativeSQLQuerySpecification, QueryParameters)" />
		/// </summary>
		IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters);

		/// <summary>
		/// new in 2.1 no javadoc
		/// </summary>
		/// <param name="key"></param>
		void AddNonExist(EntityKey key);

		/// <summary>
		/// new in 2.1 no javadoc
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		object Copy(object obj, IDictionary copiedAlready);

		/// <summary>
		/// new in 2.1 no javadoc
		/// </summary>
		/// <param name="key"></param>
		/// <param name="collectionPersister"></param>
		/// <returns></returns>
		object GetCollectionOwner(object key, ICollectionPersister collectionPersister);

		/// <summary>
		/// Retrieve the <see cref="EntityEntry" /> representation of the given entity.
		/// </summary>
		/// <param name="entity">The entity for which to locate the EntityEntry.</param>
		/// <returns>The EntityEntry for the given entity.</returns>
		EntityEntry GetEntry(object entity);

		CollectionEntry GetCollectionEntry(IPersistentCollection collection);


		/// <summary>
		/// Retreive the currently set value for a filter parameter.
		/// </summary>
		/// <param name="filterParameterName">The filter parameter name in the format 
		/// {FILTER_NAME.PARAMETER_NAME}.</param>
		/// <returns>The filter parameter value.</returns>
		object GetFilterParameterValue(string filterParameterName);

		/// <summary>
		/// Retreive the type for a given filter parrameter.
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
		IDictionary EnabledFilters { get; }

		IQuery GetNamedSQLQuery(string name);

		IQueryTranslator[] GetQueries(string query, bool scalar);

		bool ContainsEntity(EntityKey key);

		IInterceptor Interceptor { get; }

		/// <summary> Retrieves the configured event listeners from this event source. </summary>
		EventListeners Listeners { get;}

		int DontFlushFromFind { get;}

		ConnectionManager ConnectionManager { get;}

		#region Feature IPersistenceContext
		// TODO H3.2: Move these methods from ISessionImplementor to IPersistenceContext
		// These methods was added here to use the existing implementation, in SessionImpl,
		// for events-listener scope. In H3.2 these methods are in IPersistenceContext

		/// <summary> 
		/// Remove an entity from the session cache, also clear
		/// up other state associated with the entity, all except
		/// for the <see cref="EntityEntry"/>.
		/// </summary>
		/// <param name="key"></param>
		object RemoveEntity(EntityKey key);

		/// <summary> Remove a proxy from the session cache</summary>
		/// <param name="key"></param>
		void RemoveProxy(EntityKey key);

		/// <summary> Remove an entity entry from the session cache</summary>
		/// <param name="entity"></param>
		EntityEntry RemoveEntry(object entity);

		/// <summary> Is there an EntityEntry for this instance?</summary>
		bool IsEntryFor(object entity);

		/// <summary> Get the mapping from key value to entity instance</summary>
		IDictionary EntitiesByKey { get;}

		/// <summary> Is a flush cycle currently in process?</summary>
		/// <remarks> Called before and after the flushcycle</remarks>
		bool Flushing { get;set;}

		/// <summary> Get the mapping from key value to entity instance</summary>
		IDictionary EntityEntries{ get;}

		/// <summary> Get the mapping from collection instance to collection entry</summary>
		IDictionary CollectionEntries { get; }

		/// <summary> Get the mapping from collection key to collection instance</summary>
		IDictionary CollectionsByKey { get;}

		/// <summary> Called before cascading</summary>
		int IncrementCascadeLevel();

		/// <summary> Called after cascading</summary>
		int DecrementCascadeLevel();

		/// <summary> 
		/// Attempts to check whether the given key represents an entity already loaded within the
		/// current session.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj">The entity reference against which to perform the uniqueness check. </param>
		void CheckUniqueness(EntityKey key, object obj);

		/// <summary> Adds an entity to the internal caches.</summary>
		EntityEntry AddEntity(object entity, Impl.Status status, object[] loadedState, EntityKey entityKey, 
			object version, LockMode lockMode, bool existsInDatabase, IEntityPersister persister, 
			bool disableVersionIncrement, bool lazyPropertiesAreUnfetched);

		/// <summary> 
		/// Generates an appropriate EntityEntry instance and adds it to the event source's internal caches.
		/// </summary>
		EntityEntry AddEntry(object entity, Impl.Status status, object[] loadedState, 
			object id, object version, LockMode lockMode, bool existsInDatabase, 
			IEntityPersister persister, bool disableVersionIncrement, bool lazyPropertiesAreUnfetched);

		/// <summary> 
		/// Takes the given object and, if it represents a proxy, reassociates it with this event source.
		/// </summary>
		/// <param name="value">The possible proxy to be reassociated. </param>
		/// <returns> Whether the passed value represented an actual proxy which got initialized. </returns>
		bool ReassociateIfUninitializedProxy(object value);

		/// <summary> 
		/// Add an (initialized) collection that was created by another session and passed
		/// into update() (ie. one with a snapshot and existing state on the database)
		/// </summary>
		void AddInitializedDetachedCollection(IPersistentCollection collection, ICollectionSnapshot snapshot);

		/// <summary>Add a detached uninitialized collection</summary>
		void AddUninitializedDetachedCollection(IPersistentCollection collection, ICollectionPersister persister, object id);

		/// <summary> 
		/// Add a new collection (ie. a newly created one, just instantiated by the
		/// application, with no database state or snapshot)
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="collection">The collection to be associated with the persistence context </param>
		void AddNewCollection(ICollectionPersister persister, IPersistentCollection collection);

		/// <summary> Get the <see cref="PersistentArrayHolder"/> object for an array</summary>
		PersistentArrayHolder GetCollectionHolder(object array);

		/// <summary> 
		/// Register a <see cref="PersistentArrayHolder"/> object for an array.
		/// Associates a holder with an array - MUST be called after loading 
		/// array, since the array instance is not created until endLoad().
		/// </summary>
		void AddCollectionHolder(PersistentArrayHolder holder);

		/// <summary> 
		/// Remove the mapping of collection to holder during eviction
		/// of the owning entity
		/// </summary>
		IPersistentCollection RemoveCollectionHolder(object array);

		/// <summary> 
		/// False if we know for certain that all the entities are read-only.
		/// </summary>
		bool HasNonReadOnlyEntities { get;}

		/// <summary> Set the status of an entry</summary>
		void SetEntryStatus(EntityEntry entry, Impl.Status status);

		ISet NullifiableEntityKeys { get;}

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
		object[] GetCachedDatabaseSnapshot(EntityKey key);

		/// <summary> 
		/// Possibly unproxy the given reference and reassociate it with the current session.
		/// </summary>
		/// <param name="maybeProxy">The reference to be unproxied if it currently represents a proxy. </param>
		/// <returns> The unproxied instance. </returns>
		object UnproxyAndReassociate(object maybeProxy);

		/// <summary> Is the given proxy associated with this persistence context?</summary>
		bool ContainsProxy(object proxy);

		/// <summary> 
		/// Get the entity instance underlying the given proxy, throwing
		/// an exception if the proxy is uninitialized. If the given object
		/// is not a proxy, simply return the argument.
		/// </summary>
		object Unproxy(object maybeProxy);

		/// <summary> 
		/// If a deleted entity instance is re-saved, and it has a proxy, we need to
		/// reset the identifier of the proxy 
		/// </summary>
		void ReassociateProxy(object value, object id);


		void ReplaceDelayedEntityIdentityInsertKeys(EntityKey oldKey, object generatedId);

		#endregion
	}
}
