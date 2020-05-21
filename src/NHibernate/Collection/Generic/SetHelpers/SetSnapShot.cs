using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Collection.Generic.SetHelpers
{
	[Serializable]
	internal class SetSnapShot<T> : ISetSnapshot<T>
	{
		private readonly List<T> _elements;

		/// <summary>
		/// Lazy cache for TryGetValue calls
		/// </summary>
		private Dictionary<T, T> _lookupCache;

		/// <summary>
		/// Index of the first item that is not yet in the lookup cache
		/// </summary>
		private int _lookupCacheUnprocessed;

		/// <summary>
		/// <see langword="true" /> if the snapshot contains a <see langword="null" /> element
		/// </summary>
		private bool _containsNull;

		public SetSnapShot()
		{
			_elements = new List<T>();
		}

		public SetSnapShot(int capacity)
		{
			_elements = new List<T>(capacity);
		}

		public SetSnapShot(IEnumerable<T> collection)
		{
			_elements = new List<T>(collection);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			_elements.Add(item);
			// we don't need to invalidate the cache if it was already created,
			// as items are always added at the end and will be caught by
			// _lookupCacheUnprocessed

			if (item == null)
			{
				// keep track if the collection contains a null item
				// these are processed separately in TryGetValue
				_containsNull = true;
			}
		}

		public void Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(T item)
		{
			// use the caching implementation in TryGetValue
			T discard;
			return TryGetValue(item, out discard);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_elements.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new InvalidOperationException();
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_elements).CopyTo(array, index);
		}

		int ICollection.Count
		{
			get { return _elements.Count; }
		}

		public object SyncRoot
		{
			get { return ((ICollection)_elements).SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return ((ICollection)_elements).IsSynchronized; }
		}

		int ICollection<T>.Count
		{
			get { return _elements.Count; }
		}

		int IReadOnlyCollection<T>.Count
		{
			get { return _elements.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<T>)_elements).IsReadOnly; }
		}

		public bool TryGetValue(T element, out T value)
		{
			if (element == null)
			{
				// null-s cannot be handled by the cache,
				// but we keep track of them separately
				value = default(T);
				return _containsNull;
			}

			if (_lookupCache == null)
			{
				_lookupCache = new Dictionary<T, T>();
			}

			if (_lookupCache.TryGetValue(element, out value))
			{
				return true;
			}

			// look at elements not yet in the cache
			while (_lookupCacheUnprocessed < _elements.Count)
			{
				T snapshotElement = _elements[_lookupCacheUnprocessed++];
				if (snapshotElement != null)
				{
					// be careful not to replace cache entries to preserve
					// original semantics and return the first matching object
					if (!_lookupCache.ContainsKey(snapshotElement))
					{
						_lookupCache.Add(snapshotElement, snapshotElement);
					}

					if (snapshotElement.Equals(element))
					{
						// found a match, no need to continue filling the cache
						// at this point
						value = snapshotElement;
						return true;
					}
				}
			}

			value = default(T);
			return false;
		}
	}
}
