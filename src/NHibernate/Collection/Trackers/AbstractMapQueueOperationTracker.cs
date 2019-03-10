using System.Collections.Generic;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal abstract class AbstractMapQueueOperationTracker<TKey, TValue> : AbstractQueueOperationTracker
	{
		/// <summary>
		/// Tries to retrieve a queued element by its key.
		/// </summary>
		/// <param name="elementKey">The element key.</param>
		/// <param name="element">The output variable for the element.</param>
		/// <returns>True whether the element was found, false otherwise.</returns>
		public abstract bool TryGetElementByKey(TKey elementKey, out TValue element);

		/// <summary>
		/// Tries to retrieve a element that exists in the database by its key.
		/// </summary>
		/// <param name="elementKey">The element key.</param>
		/// <param name="element">The output variable for the element.</param>
		/// <returns>True whether the element was found, false otherwise.</returns>
		public abstract bool TryGetDatabaseElementByKey(TKey elementKey, out TValue element);

		/// <summary>
		/// Checks whether the key exist in the queue.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>True whether it exists, false otherwise.</returns>
		public abstract bool ContainsKey(TKey key);

		/// <summary>
		/// A method that is called when the map <see cref="IDictionary{TKey,TValue}.Add(TKey,TValue)"/> method is called.
		/// </summary>
		/// <param name="elementKey">The key to add.</param>
		/// <param name="element">The element to add </param>
		/// <param name="exists">Whether the key exists in the database or in the queue.</param>
		public abstract void AddElementByKey(TKey elementKey, TValue element, bool exists);

		/// <summary>
		/// A method that is called when the map <see cref="IDictionary{TKey,TValue}.this"/> is set.
		/// </summary>
		/// <param name="elementKey">The key to set.</param>
		/// <param name="element">The element to set.</param>
		/// <param name="oldElement">The element that currently occupies the <paramref name="elementKey"/>.</param>
		/// <param name="existsInDb">Whether the element exists in the database.</param>
		public abstract void SetElementByKey(TKey elementKey, TValue element, TValue oldElement, bool? existsInDb);

		/// <summary>
		/// A method that is called when the map <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/> is called.
		/// </summary>
		/// <param name="elementKey">The key to remove.</param>
		/// <param name="oldElement">The element that currently occupies the <paramref name="elementKey"/>.</param>
		/// <param name="existsInDb">Whether the element exists in the database.</param>
		/// <returns>True whether the key was successfully removed from the queue.</returns>
		public abstract bool RemoveElementByKey(TKey elementKey, TValue oldElement, bool? existsInDb);

		/// <summary>
		/// Checks whether the element key is queued for removal.
		/// </summary>
		/// <param name="elementKey">The element key to check.</param>
		/// <returns>True whether the element key is queued for removal, false otherwise.</returns>
		public abstract bool IsElementKeyQueuedForDelete(TKey elementKey);

		/// <summary>
		/// Applies all the queued changes to the loaded map.
		/// </summary>
		/// <param name="loadedMap">The loaded map.</param>
		public abstract void ApplyChanges(IDictionary<TKey, TValue> loadedMap);
	}
}
