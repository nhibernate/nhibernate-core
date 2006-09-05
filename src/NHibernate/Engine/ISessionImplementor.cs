using System;
using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif

using NHibernate.Collection;
using NHibernate.Engine.Query;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

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
		object GetLoadedCollectionKey( IPersistentCollection collection );

		/// <summary>
		/// Get the snapshot of the pre-flush collection state
		/// </summary>
		object GetSnapshot( IPersistentCollection collection );

		/// <summary>
		/// Get the <see cref="IPersistentCollection" /> object for an array
		/// </summary>
		PersistentArrayHolder GetArrayHolder( object array );

		/// <summary>
		/// Register a <see cref="IPersistentCollection" /> object for an array
		/// </summary>
		void AddArrayHolder( PersistentArrayHolder holder );

		/// <summary>
		/// Initialize the collection (if not already initialized)
		/// </summary>
		/// <param name="coolection"></param>
		/// <param name="writing"></param>
		void InitializeCollection( IPersistentCollection coolection, bool writing );

		/// <summary>
		/// Is this the "inverse" end of a bidirectional association?
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		bool IsInverseCollection( IPersistentCollection collection );

		/// <summary>
		/// new in h2.1 and no javadoc
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="resultSetId"></param>
		/// <returns></returns>
		IPersistentCollection GetLoadingCollection( ICollectionPersister persister, object id, object resultSetId );

		/// <summary>
		/// new in h2.1 and no javadoc
		/// </summary>
		void EndLoadingCollections( ICollectionPersister persister, object resultSetId );

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
		object GetCollection( string role, object id, object owner );

		// NH-268
		/// <summary>
		/// Load an instance without checking if it was deleted. If it does not exist and isn't nullable, throw an exception.
		/// This method may create a new proxy or return an existing proxy.
		/// </summary>
		/// <param name="persistentClass">The <see cref="System.Type"/> to load.</param>
		/// <param name="id">The identifier of the object in the database.</param>
		/// <param name="isNullable">Allow null instance</param>
		/// <returns>
		/// A proxy of the object or an instance of the object if the <c>persistentClass</c> does not have a proxy.
		/// </returns>
		/// <exception cref="ObjectNotFoundException">No object could be found with that <c>id</c>.</exception>
		object InternalLoad( System.Type persistentClass, object id, bool isNullable );

		/// <summary>
		/// Load an instance immediately. Do not return a proxy.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object ImmediateLoad( System.Type persistentClass, object id );

		/// <summary>
		/// Load an instance by a unique key that is not the primary key.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object LoadByUniqueKey( System.Type persistentClass, string uniqueKeyPropertyName, object id );

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
		/// After actually inserting a row, record the fact that the instance exists on the database
		/// (needed for identity-column key generation)
		/// </summary>
		/// <param name="obj"></param>
		void PostInsert( object obj );

		/// <summary>
		/// After actually deleting a row, record the fact that the instance no longer exists on the
		/// database (needed for identity-column key generation)
		/// </summary>
		/// <param name="obj"></param>
		void PostDelete( object obj );

		/// <summary>
		/// After actually updating a row, record the fact that the database state has been updated.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> instance that was saved.</param>
		/// <param name="updatedState">A updated snapshot of the values in the object.</param>
		/// <param name="nextVersion">The new version to assign to the <c>obj</c>.</param>
		void PostUpdate( object obj, object[ ] updatedState, object nextVersion );

		/// <summary>
		/// Execute a <c>Find()</c> query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IList Find( string query, QueryParameters parameters );

		void Find( string query, QueryParameters parameters, IList results );

#if NET_2_0
		/// <summary>
		/// Strongly-typed version of <see cref="Find(string, QueryParameters)" />
		/// </summary>
		IList<T> Find<T>( string query, QueryParameters queryParameters );
#endif

		/// <summary>
		/// Execute an <c>Iterate()</c> query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IEnumerable Enumerable( string query, QueryParameters parameters );

#if NET_2_0
		/// <summary>
		/// Strongly-typed version of <see cref="Enumerable(string, QueryParameters)" />
		/// </summary>
		IEnumerable<T> Enumerable<T>( string query, QueryParameters queryParameters );
#endif

		/// <summary>
		/// Execute a filter
		/// </summary>
		IList Filter( object collection, string filter, QueryParameters parameters );

#if NET_2_0
		/// <summary>
		/// Execute a filter (strongly-typed version).
		/// </summary>
		IList<T> Filter<T>( object collection, string filter, QueryParameters parameters );
#endif

		/// <summary>
		/// Collection from a filter
		/// </summary>
		IEnumerable EnumerableFilter( object collection, string filter, QueryParameters parameters );

#if NET_2_0
		/// <summary>
		/// Strongly-typed version of <see cref="EnumerableFilter(object, string, QueryParameters)" />
		/// </summary>
		IEnumerable<T> EnumerableFilter<T>( object collection, string filter, QueryParameters parameters );
#endif

		/// <summary>
		/// Get the <c>IEntityPersister</c> for an object
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		IEntityPersister GetEntityPersister( object obj );

		/// <summary>
		/// Add an uninitialized instance of an entity class, as a placeholder to ensure object identity.
		/// Must be called before <c>PostHydrate()</c>
		/// </summary>
		void AddUninitializedEntity( EntityKey key, object obj, LockMode lockMode );

		/// <summary>
		/// Register the "hydrated" state of an entity instance, after the first step of 2-phase loading
		/// </summary>
		void PostHydrate( IEntityPersister persister, object id, object[ ] values, object obj, LockMode lockMode );

		/// <summary>
		/// Perform the second step of 2-phase load (ie. fully initialize the entity instance)
		/// </summary>
		void InitializeEntity( object obj );

		/// <summary>
		/// Get the entity instance associated with the given <c>EntityKey</c>
		/// </summary>
		object GetEntity( EntityKey key );

		/// <summary>
		/// Return the existing proxy associated with the given <c>EntityKey</c>, or the second
		/// argument (the entity associated with the key) if no proxy exists.
		/// </summary>
		/// <param name="persister">The <see cref="IEntityPersister"/> to see if it should be Proxied.</param>
		/// <param name="key">The <see cref="EntityKey"/> that identifies the entity.</param>
		/// <param name="impl"></param>
		/// <returns>Returns a the Proxy for the class or the parameter impl.</returns>
		object ProxyFor( IEntityPersister persister, EntityKey key, object impl );

		/// <summary>
		/// Return the existing proxy associated with the given object. (Slower than the form above)
		/// </summary>
		object ProxyFor( object impl );

		/// <summary>
		/// Notify the session that the transaction completed, so we no longer own the old locks.
		/// (Also we shold release cache softlocks). May be called multiple times during the transaction
		/// completion process.
		/// </summary>
		void AfterTransactionCompletion( bool successful );

		/// <summary>
		/// Return the identifier of the persistent object, or null if transient
		/// </summary>
		object GetEntityIdentifier( object obj );

		/// <summary>
		/// Return the identifer of the persistent or transient object, or throw
		/// an exception if the instance is "unsaved"
		/// </summary>
		object GetEntityIdentifierIfNotUnsaved( object obj );

		bool IsSaved( object obj );

		/// <summary>
		/// Instantiate the entity class, initializing with the given identifier
		/// </summary>
		object Instantiate( System.Type clazz, object id );

		/// <summary>
		/// Set the lock mode of the entity to the given lock mode
		/// </summary>
		void SetLockMode( object entity, LockMode lockMode );

		/// <summary>
		/// Get the current version of the entity
		/// </summary>
		object GetVersion( object entity );

		/// <summary>
		/// Get the lock mode of the entity
		/// </summary>
		LockMode GetLockMode( object entity );

		/// <summary>
		/// Get a batch of uninitialized collection keys for this role
		/// </summary>
		object[] GetCollectionBatch( ICollectionPersister collectionPersister, object id, int batchSize );

		/// <summary>
		/// Get a batch of unloaded identifiers for this class
		/// </summary>
		object[] GetClassBatch( System.Type clazz, object id, int batchSize );

		/// <summary>
		/// Register the entity as batch loadable, if enabled
		/// </summary>
		void ScheduleBatchLoad( System.Type clazz, object id );

		/// <summary>
		/// Execute an SQL Query
		/// </summary>
		IList List( NativeSQLQuerySpecification spec, QueryParameters queryParameters );

		void List( NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results );

#if NET_2_0
		/// <summary>
		/// Strongly-typed version of <see cref="List(NativeSQLQuerySpecification, QueryParameters)" />
		/// </summary>
		IList<T> List<T>( NativeSQLQuerySpecification spec, QueryParameters queryParameters );
#endif

		/// <summary>
		/// new in 2.1 no javadoc
		/// </summary>
		/// <param name="key"></param>
		void AddNonExist( EntityKey key );

		/// <summary>
		/// new in 2.1 no javadoc
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		object Copy( object obj, IDictionary copiedAlready );

		/// <summary>
		/// new in 2.1 no javadoc
		/// </summary>
		/// <param name="key"></param>
		/// <param name="collectionPersister"></param>
		/// <returns></returns>
		object GetCollectionOwner( object key, ICollectionPersister collectionPersister );

		/// <summary>
		/// Retrieve the <see cref="EntityEntry" /> representation of the given entity.
		/// </summary>
		/// <param name="entity">The entity for which to locate the EntityEntry.</param>
		/// <returns>The EntityEntry for the given entity.</returns>
		EntityEntry GetEntry( object entity );

		CollectionEntry GetCollectionEntry( IPersistentCollection collection );


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
        /// name, with values corresponding to the {@link org.hibernate.impl.FilterImpl}
        /// instance.
        /// </summary>
        /// <param name="filterParameterName">The filter parameter name in the format 
        /// {FILTER_NAME.PARAMETER_NAME}.</param>
        /// <returns>The currently enabled filters.</returns>
        IDictionary EnabledFilters { get; }
    }
}