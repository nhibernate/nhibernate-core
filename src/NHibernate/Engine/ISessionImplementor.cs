using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Persister;
using NHibernate.Type;
namespace NHibernate.Engine {
	/// <summary>
	/// Defines the internal contract between the <c>Session</c> and other parts of Hibernate
	/// such as implementors of <c>Type</c> or <c>ClassPersister</c>
	/// </summary>
	public interface ISessionImplementor : ISession {

		/// <summary>
		/// Get the pre-flush identifier of the collection
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		object GetLoadedCollectionKey(PersistentCollection collection);

		/// <summary>
		/// Get the snapshot of the pre-flush collection state
		/// </summary>
		object GetSnapshot(PersistentCollection collection);

		/// <summary>
		/// Get the <c>PersistentCollection</c> object for an array
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		ArrayHolder GetArrayHolder(object array);

		/// <summary>
		/// Register a <c>PersistentCollection</c> object for an array
		/// </summary>
		/// <param name="holder"></param>
		void AddArrayHolder(ArrayHolder holder);

		/// <summary>
		/// Register an uninitialized <c>PersistentColleciton</c> that will be lazily initialized
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		void AddUninitializedCollection(PersistentCollection collection, CollectionPersister persister, object id);
		
		/// <summary>
		/// Register an initialized <c>PersistentCollection</c>
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		void AddInitializedCollection(PersistentCollection collection, CollectionPersister persister, object id);

		/// <summary>
		/// Set the "shallow dirty" status of the collection. Called when the collection detects
		/// taht the client is modifying it
		/// </summary>
		void Dirty(PersistentCollection collection);

		/// <summary>
		/// Initialize the collection (if not already initialized)
		/// </summary>
		/// <param name="coolection"></param>
		/// <param name="writing"></param>
		void Initialize(PersistentCollection coolection, bool writing);

		/// <summary>
		/// Is this the "inverse" end of a bidirectional association?
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		bool IsInverseCollection(PersistentCollection collection);

		/// <summary>
		/// new in h2.0.3 and no javadoc
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		PersistentCollection GetLoadingCollection(CollectionPersister persister, object id);
		
		/// <summary>
		/// new in h2.0.3 and no javadoc
		/// 
		/// MikeD added to help with EndRead of Collections...
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		PersistentCollection GetLoadingCollection(CollectionPersister persister, object id, object owner);
		
		/// <summary>
		/// new in h2.0.3 and no javadoc
		/// </summary>
		/// <param name="role"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		PersistentCollection GetLoadingCollection(String role, object id);
		
		/// <summary>
		/// new in h2.0.3 and no javadoc
		/// </summary>
		void EndLoadingCollections();
	

		/// <summary>
		/// Load an instance without checking if it was deleted. If it does not exist, throw an exception.
		/// This method may create a new proxy or return an existing proxy.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object InternalLoad(System.Type persistentClass, object id);

		/// <summary>
		/// Load an instance without checking if it was deleted. If it does not exist, return <tt>null</tt>.
		/// Do not create a proxy (but do return any existing proxy).
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object InternalLoadOneToOne(System.Type persistentClass, object id);

		/// <summary>
		/// Load an instance immediately. Do not return a proxy.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object ImmediateLoad(System.Type persistentClass, object id);

		/// <summary>
		/// Load an instance immediately. Do not return a proxy
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		/// <returns></returns>
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
		/// Get the NHibernate Command Preparer for this Session.
		/// </summary>
		IPreparer Preparer {get; }
		//TODO: this will eventually replace the Batcher...

		/// <summary>
		/// After actually inserting a row, record the fact taht the instance exists on the database
		/// (needed for identity-column key generation)
		/// </summary>
		/// <param name="obj"></param>
		void PostInsert(object obj);

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
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="selection"></param>
		/// <param name="namedParams"></param>
		/// <returns></returns>
		IList Find(string query, object[] values, IType[] types, RowSelection selection, IDictionary namedParams);
	
		/// <summary>
		/// Execute an <c>Iterate()</c> query
		/// </summary>
		/// <param name="query"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="selection"></param>
		/// <param name="namedParams"></param>
		/// <returns></returns>
		IEnumerable Enumerable(string query, object[] values, IType[] types, RowSelection selection, IDictionary namedParams);

		/// <summary>
		/// Execute a filter
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="selection"></param>
		/// <param name="namedParams"></param>
		/// <returns></returns>
		IList Filter(object collection, string filter, object[] values, IType[] types, RowSelection selection, IDictionary namedParams);

		/// <summary>
		/// Collection from a filter
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="selection"></param>
		/// <param name="namedParams"></param>
		/// <returns></returns>
		IEnumerable EnumerableFilter(object collection, string filter, object[] values, IType[] types, RowSelection selection, IDictionary namedParams);
		
		/// <summary>
		/// Get the <c>IClassPersister</c> for an object
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		IClassPersister GetPersister(object obj);

		/// <summary>
		/// Add an uninitialized instance of an entity class, as a placeholder to ensure object identity.
		/// Must be called before <c>PostHydrate()</c>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		void AddUninitializedEntity(Key key, object obj, LockMode lockMode);

		/// <summary>
		/// Register the "hydrated" state of an entity instance, after the first step of 2-phase loading
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="values"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		void PostHydrate(IClassPersister persister, object id, object[] values, object obj, LockMode lockMode);
	
		/// <summary>
		/// Perform the second step of 2-phase load (ie. fully initialize the entity instance)
		/// </summary>
		/// <param name="obj"></param>
		void InitializeEntity(object obj);

		/// <summary>
		/// Get the entity instance associated with the given <c>Key</c>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object GetEntity(Key key);

		/// <summary>
		/// Return the existing proxy associated with the given <c>Key</c>, or the second
		/// argument (the entity associated with the key) if no proxy exists.
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="key"></param>
		/// <param name="impl"></param>
		/// <returns></returns>
		object ProxyFor(IClassPersister persister, Key key, object impl);

		/// <summary>
		/// Return the existing proxy associated with the given object. (Slower than the form above)
		/// </summary>
		/// <param name="impl"></param>
		/// <returns></returns>
		object ProxyFor(object impl);

		/// <summary>
		/// Notify the session that the transaction completed, so we no longer own the old locks.
		/// (Also we shold release cache softlocks).
		/// </summary>
		void AfterTransactionCompletion();

		/// <summary>
		/// Return the identifier of the persistent object, or null if transient
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object GetEntityIdentifier(object obj);

		/// <summary>
		/// Return the identifer of the persistent or transient object, or throw
		/// an exception if the instance is "unsaved"
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object GetEntityIdentifierIfNotUnsaved(object obj);

		/// <summary>
		/// Instantiate the entity class, initializing with the given identifier
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object Instantiate(System.Type clazz, object id);

		/// <summary>
		/// Set the lock mode of the entity to the given lock mode
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="lockMode"></param>
		void SetLockMode(object entity, LockMode lockMode);

		/// <summary>
		/// Get the current version of the entity
		/// </summary>
		/// <param name="entity"></param>
		//TODO: remove this comment
		object GetVersion(object entity);

		/// <summary>
		/// Get the lock mode of the entity
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		LockMode GetLockMode(object entity);

		/// <summary>
		/// Get the collection orphans (entities which were removed from
		/// the collection
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		//TODO: remove this comment
		//ICollection GetOrphans(PersistentCollection coll);

	}
}
