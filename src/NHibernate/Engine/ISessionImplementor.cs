using System;
using NHibernate.Collection;

namespace NHibernate.Engine {
	/// <summary>
	/// Defines the internal contract between the <c>Session</c> and other parts of Hibernate
	/// such as implementors of <c>Type</c> or <c>ClassPersister</c>
	/// </summary>
	public interface ISessionImplementor : ISession {

		/// <summary>
		/// Get the snapshot of the pre-flush collection state
		/// </summary>
		object GetSnapshot(PersistentCollection collection);
		
		/// <summary>
		/// Set the "shallow dirty" status of the collection. Called when the collection detects
		/// taht the client is modifying it
		/// </summary>
		void Dirty(PersistentCollection collection);

		/// <summary>
		/// Is this the readonly end of a bidirectional association?
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		bool IsCollectionReadOnly(PersistentCollection collection);

		/// <summary>
		/// Initialize the collection (if not already initialized)
		/// </summary>
		/// <param name="coolection"></param>
		/// <param name="writing"></param>
		void Initialize(PersistentCollection coolection, bool writing);
	
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
	}
}
