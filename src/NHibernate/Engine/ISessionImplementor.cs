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
	}
}
