using System;
using NHibernate.Collection;

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
		/// Is this the readonly end of a bidirectional association?
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		bool IsCollectionReadOnly(PersistentCollection collection);		
	
		/// <summary>
		/// Notify the session that the transaction completed, so we no longer own the old locks.
		/// (Also we should release cache softlocks.)
		/// </summary>
		void AfterTransactionCompletion();

		/// <summary>
		/// Return the identifier of the persistent object, or null if transient
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object GetEntityIdentifier(object obj);

		/// <summary>
		/// Return the identifier of the persistent or transient object, or throw
		/// an exception if the instance is "unsaved"
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object GetEntityIdentifierIfNotUnsaved(object obj);

		/// <summary>
		/// Instantiate the entity class, initilizing with the given identifier
		/// </summary>
		/// <param name="?"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		object Instantiate(System.Type clazz, object id);

		/// <summary>
		/// Get the creating SessionFactoryImplementor
		/// </summary>
		/// <returns></returns>
		ISessionFactoryImplementor GetFactory();

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
		/// Get the prepared statement <c>Batcher</c> for this session
		/// </summary>
		IBatcher Batcher { get; }

		/// <summary>
		/// Get the entity instance associated with the given <c>Key</c>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object GetEntity(Key key);
	
	}
}
