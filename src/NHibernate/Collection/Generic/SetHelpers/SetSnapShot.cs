using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Collection.Generic.SetHelpers
{
	[Serializable]
	internal class SetSnapShot<T> : ISetSnapshot<T>
	{
		private readonly List<T> elements;
		public SetSnapShot()
		{
			elements = new List<T>();
		}

		public SetSnapShot(int capacity)
		{
			elements = new List<T>(capacity);
		}

		public SetSnapShot(IEnumerable<T> collection)
		{
			elements = new List<T>(collection);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			elements.Add(item);
		}

		public void Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(T item)
		{
			return elements.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			elements.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new InvalidOperationException();
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)elements).CopyTo(array, index);
		}

		int ICollection.Count
		{
			get { return elements.Count; }
		}

		public object SyncRoot
		{
			get { return ((ICollection)elements).SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return ((ICollection)elements).IsSynchronized; }
		}

		int ICollection<T>.Count
		{
			get { return elements.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<T>)elements).IsReadOnly; }
		}

		public T this[T element]
		{
			get
			{
				var idx = elements.IndexOf(element);
				if (idx >= 0)
				{
					return elements[idx];
				}
				return default(T);
			}
		}
	}
}