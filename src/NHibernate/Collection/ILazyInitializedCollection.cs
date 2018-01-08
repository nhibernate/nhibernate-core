﻿namespace NHibernate.Collection
{
	/// <summary>
	/// This interface allows to check if a collection is already initialized and if not to force initialize it.
	/// </summary>
	/// <remarks>
	/// This interface is provided to allow implementing lazy initialized collections which do not implement IPersistentCollection.
	/// That is e.g. needed for NHibernate.Envers which can't load it's collections as PersistentCollections.
	/// </remarks>
	// 6.0 TODO: set as ancestor of IPersistentCollection
	public partial interface ILazyInitializedCollection
	{
		/// <summary>
		/// returns true, if the proxy already has been initialized. 
		/// If false, accessing the collection or calling ForceInitialization()
		/// initializes the collection. 
		/// </summary>
		bool WasInitialized { get; }

		/// <summary>
		/// To be called internally by the session, forcing
		/// immediate initalization.
		/// </summary>
		void ForceInitialization();

	}
}
