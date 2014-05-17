using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Collection.Generic.SetHelpers
{
	[Serializable]
	internal class SetSnapShot<T> : ISetSnapshot<T>
	{
		private readonly List<T> _elements;
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
		}

		public void Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(T item)
		{
			return _elements.Contains(item);
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

		public bool IsReadOnly
		{
			get { return ((ICollection<T>)_elements).IsReadOnly; }
		}

		public bool TryGetValue(T element, out T value)
		{
			var idx = _elements.IndexOf(element);
			if (idx >= 0)
			{
				value = _elements[idx];
				return true;
			}

			value = default(T);
			return false;
		}
	}
}