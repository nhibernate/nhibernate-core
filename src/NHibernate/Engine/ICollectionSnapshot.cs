namespace NHibernate.Engine
{
	/// <summary>
	/// Defines a complete "snapshot" of a particular collection.
	/// </summary>
	public interface ICollectionSnapshot
	{
		/// <summary>
		/// Gets the identifier of the Entity that owns this Collection.
		/// </summary>
		object Key { get; }

		/// <summary>
		/// Gets the role that identifies this Collection.
		/// </summary>
		string Role { get; }

		/// <summary>
		/// Gets the snapshot copy of the Collection's elements.
		/// </summary>
		/// <remarks>
		/// In most cases this is the same collection type as the one being snapshotted. 
		/// ie - the snapshot of an IList will return an IList.
		/// </remarks>
		object Snapshot { get; }

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if some action has been performed on the
		/// actual collection instance that has modified it.
		/// </summary>
		bool Dirty { get; }

		/// <summary>
		/// Marks the <see cref="ICollectionSnapshot"/> as being dirty.
		/// </summary>
		void SetDirty();

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if the underlying collection has been
		/// initialized yet.
		/// </summary>
		bool IsInitialized { get; }
	}
}