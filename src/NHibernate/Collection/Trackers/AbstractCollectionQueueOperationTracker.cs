using System.Collections.Generic;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal abstract class AbstractCollectionQueueOperationTracker<T> : AbstractQueueOperationTracker
	{
		/// <summary>
		/// A method that is called when an element is added to the collection.
		/// </summary>
		/// <param name="element">The element to add.</param>
		/// <returns>True whether the element was successfully added to the queue, false otherwise</returns>
		public abstract bool AddElement(T element);

		/// <summary>
		///  A method that is called when an existing element is removed from the collection.
		/// </summary>
		/// <param name="element">The element to remove.</param>
		/// <param name="existsInDb">Whether the element exists in the database.</param>
		public abstract void RemoveExistingElement(T element, bool? existsInDb);

		/// <summary>
		/// Checks whether the element exists in the queue.
		/// </summary>
		/// <param name="element">The element to check.</param>
		/// <returns>True whether the element exists in the queue, false otherwise.</returns>
		public abstract bool ContainsElement(T element);

		/// <summary>
		/// Checks whether the element is queued for removal.
		/// </summary>
		/// <param name="element">The element to check.</param>
		/// <returns>True whether the element is queued for removal, false otherwise.</returns>
		public abstract bool IsElementQueuedForDelete(T element);

		#region Indexed operations

		/// <summary>
		/// A method that is called when an element is removed by its index from the collection.
		/// </summary>
		/// <param name="index">The index of the element.</param>
		/// <param name="element">The element to remove.</param>
		public abstract void RemoveElementAtIndex(int index, T element);

		/// <summary>
		/// A method that is called when an element is added at a specific index of the collection.
		/// </summary>
		/// <param name="index">The index to put the element.</param>
		/// <param name="element">The element to add.</param>
		public abstract void AddElementAtIndex(int index, T element);

		/// <summary>
		/// A method that is called when an element is set at a specific index of the collection.
		/// </summary>
		/// <param name="index">The index to set the new element.</param>
		/// <param name="element">The element to set.</param>
		/// <param name="oldElement">The element that currently occupies the <paramref name="index"/>.</param>
		public abstract void SetElementAtIndex(int index, T element, T oldElement);

		/// <summary>
		/// Tries to retrieve the element by a specific index of the collection.
		/// </summary>
		/// <param name="index">The index to put the element.</param>
		/// <param name="element">The output variable for the element.</param>
		/// <returns>True whether the element was found, false otherwise.</returns>
		public abstract bool TryGetElementAtIndex(int index, out T element);

		/// <summary>
		/// Gets the element index where it currently lies in the database by taking into the consideration the queued operations.
		/// </summary>
		/// <param name="index">The effective index that will be when all operations would be flushed.</param>
		/// <returns>The element index in the database or -1 if the index represents a transient element.</returns>
		public abstract int? GetDatabaseElementIndex(int index);

		#endregion
	}

	internal abstract class AbstractCollectionQueueOperationTracker<T, TCollection> : AbstractCollectionQueueOperationTracker<T>
		where TCollection : ICollection<T>
	{
		/// <summary>
		/// Applies all the queued changes to the loaded collection.
		/// </summary>
		/// <param name="loadedCollection">The loaded collection.</param>
		public abstract void ApplyChanges(TCollection loadedCollection);
	}
}
