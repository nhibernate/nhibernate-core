/* Copyright © 2012 Oskar Berggren */

#if FEATURE_INTERNALIZE_IESI

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace Iesi.Collections.Generic
{
	/// <summary>
	/// Implementation of ISet that also maintains a linked list over all elements.
	/// Enumeration of this set is guaranteed to return the elements in the order
	/// they were added.
	/// </summary>
	[Serializable]
	public class LinkedHashSet<T> : ISet<T>
	{
		private readonly Dictionary<T, LinkedHashNode<T>> _elements;
		private LinkedHashNode<T> _first, _last;

		public LinkedHashSet()
		{
			_elements = new Dictionary<T, LinkedHashNode<T>>();
		}


		public LinkedHashSet(IEnumerable<T> initialValues)
			: this()
		{
			UnionWith(initialValues);
		}



		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			var current = _first;
			while (current != null)
			{
				yield return current.Value;
				current = current.Next;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion



		#region Implementation of ICollection<T>

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		void ICollection<T>.Add(T item)
		{
			Add(item);
		}


		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		public int Count
		{
			get { return _elements.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		/// </returns>
		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
		public void Clear()
		{
			_elements.Clear();
			_first = null;
			_last = null;
		}


		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		public bool Contains(T item)
		{
			return _elements.ContainsKey(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			int index = arrayIndex;

			foreach (var item in this)
				array[index++] = item;
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public bool Remove(T item)
		{
			LinkedHashNode<T> node;
			if (_elements.TryGetValue(item, out node))
			{
				_elements.Remove(item);
				Unlink(node);
				return true;
			}

			return false;
		}

		#endregion


		#region Implementation of ISet<T>

		/// <summary>
		/// Modifies the current set so that it contains all elements that are present in either the current set or the specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public void UnionWith(IEnumerable<T> other)
		{
			foreach (var item in other)
				Add(item);
		}

		/// <summary>
		/// Modifies the current set so that it contains only elements that are also in a specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public void IntersectWith(IEnumerable<T> other)
		{
			var otherSet = AsSet(other);

			var current = _first;
			while (current != null)
			{
				if (!otherSet.Contains(current.Value))
				{
					_elements.Remove(current.Value);
					Unlink(current);
				}
				current = current.Next;
			}
		}

		/// <summary>
		/// Removes all elements in the specified collection from the current set.
		/// </summary>
		/// <param name="other">The collection of items to remove from the set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public void ExceptWith(IEnumerable<T> other)
		{
			foreach (var item in other)
				Remove(item);
		}

		/// <summary>
		/// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both. 
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			foreach (var item in other)
			{
				LinkedHashNode<T> node;
				if (_elements.TryGetValue(item, out node))
				{
					_elements.Remove(item);
					Unlink(node);
				}
				else
					Add(item);
			}
		}

		/// <summary>
		/// Determines whether a set is a subset of a specified collection.
		/// </summary>
		/// <returns>
		/// true if the current set is a subset of <paramref name="other"/>; otherwise, false.
		/// </returns>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			var otherSet = AsSet(other);

			if (Count > otherSet.Count)
				return false;

			return _elements.Keys.All(otherSet.Contains);
		}


		/// <summary>
		/// Determines whether the current set is a superset of a specified collection.
		/// </summary>
		/// <returns>
		/// true if the current set is a superset of <paramref name="other"/>; otherwise, false.
		/// </returns>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			int numberOfOthersPresent;
			var numberOfOthers = CountOthers(other, out numberOfOthersPresent);

			// All others must be present.
			return numberOfOthersPresent == numberOfOthers;
		}

		/// <summary>
		/// Determines whether the current set is a correct superset of a specified collection.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.ISet`1"/> object is a correct superset of <paramref name="other"/>; otherwise, false.
		/// </returns>
		/// <param name="other">The collection to compare to the current set. </param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			int numberOfOthersPresent;
			var numberOfOthers = CountOthers(other, out numberOfOthersPresent);

			// All others must be present, plus we need to have at least one additional item.
			return numberOfOthersPresent == numberOfOthers && numberOfOthers < Count;
		}

		/// <summary>
		/// Determines whether the current set is a proper (strict) subset of a specified collection.
		/// </summary>
		/// <returns>
		/// true if the current set is a correct subset of <paramref name="other"/>; otherwise, false.
		/// </returns>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			var otherSet = AsSet(other);

			if (Count >= otherSet.Count)
				return false;

			return _elements.Keys.All(otherSet.Contains);
		}

		/// <summary>
		/// Determines whether the current set overlaps with the specified collection.
		/// </summary>
		/// <returns>
		/// true if the current set and <paramref name="other"/> share at least one common element; otherwise, false.
		/// </returns>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public bool Overlaps(IEnumerable<T> other)
		{
			return other.Any(Contains);
		}

		/// <summary>
		/// Determines whether the current set and the specified collection contain the same elements.
		/// </summary>
		/// <returns>
		/// true if the current set is equal to <paramref name="other"/>; otherwise, false.
		/// </returns>
		/// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
		public bool SetEquals(IEnumerable<T> other)
		{
			int numberOfOthersPresent;
			var numberOfOthers = CountOthers(other, out numberOfOthersPresent);

			return numberOfOthers == Count && numberOfOthersPresent == Count;
		}


		/// <summary>
		/// Adds an element to the current set and returns a value to indicate if the element was successfully added. 
		/// </summary>
		/// <returns>
		/// true if the element is added to the set; false if the element is already in the set.
		/// </returns>
		/// <param name="item">The element to add to the set.</param>
		public bool Add(T item)
		{
			if (_elements.ContainsKey(item))
				return false;

			var node = new LinkedHashNode<T>(item) { Previous = _last };

			if (_first == null)
				_first = node;

			if (_last != null)
				_last.Next = node;

			_last = node;

			_elements.Add(item, node);

			return true;
		}

		#endregion


		/// <summary>
		/// Count the elements in the given collection and determine both the total
		/// count and how many of the elements that are present in the current set.
		/// </summary>
		private int CountOthers(IEnumerable<T> items, out int numberOfOthersPresent)
		{
			numberOfOthersPresent = 0;
			int numberOfOthers = 0;

			foreach (var item in items)
			{
				numberOfOthers++;
				if (Contains(item))
					numberOfOthersPresent++;
			}
			return numberOfOthers;
		}


		/// <summary>
		/// Cast the given collection to an ISet&lt;T&gt; if possible. If not,
		/// return a new set containing the items.
		/// </summary>
		private static ISet<T> AsSet(IEnumerable<T> items)
		{
			return items as ISet<T> ?? new HashSet<T>(items);
		}


		/// <summary>
		/// Unlink a node from the linked list by updating the node pointers in
		/// its preceeding and subsequent node. Also update the _first and _last
		/// pointers if necessary.
		/// </summary>
		private void Unlink(LinkedHashNode<T> node)
		{
			if (node.Previous != null)
				node.Previous.Next = node.Next;

			if (node.Next != null)
				node.Next.Previous = node.Previous;

			if (ReferenceEquals(node, _first))
				_first = node.Next;
			if (ReferenceEquals(node, _last))
				_last = node.Previous;
		}

		[Serializable]
		private class LinkedHashNode<TElement>
		{
			public LinkedHashNode(TElement value)
			{
				Value = value;
			}

			public TElement Value { get; private set; }

			public LinkedHashNode<TElement> Next { get; set; }
			public LinkedHashNode<TElement> Previous { get; set; }
		}
	}
}

#endif
