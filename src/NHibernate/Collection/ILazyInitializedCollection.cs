namespace NHibernate.Collection
{
	/// <summary>
	/// This interface allows to check if a lazy collection is already initialized and to force its initialization.
	/// </summary>
	/// <remarks>
	/// This interface is provided to allow implementing lazy initialized collections which do not implement
	/// <see cref="IPersistentCollection" />.
	/// That is e.g. needed for NHibernate.Envers which can't load its collections as PersistentCollections.
	/// </remarks>
	// 6.0 TODO: set as ancestor of IPersistentCollection
	public partial interface ILazyInitializedCollection
	{
		/// <summary>
		/// Return <see langword="true"/> if the proxy has already been initialized.
		/// If <see langword="false"/>, accessing the collection or calling <see cref="ForceInitialization" />
		/// initializes the collection.
		/// </summary>
		bool WasInitialized { get; }

		/// <summary>
		/// Force immediate initialization.
		/// </summary>
		void ForceInitialization();

	}
}
