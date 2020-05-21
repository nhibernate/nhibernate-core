using System;
using System.Collections;
using System.Collections.Generic;
#if NETCOREAPP2_0
using System.Runtime.Serialization;
using System.Threading;
#endif

namespace NHibernate.Collection.Generic.SetHelpers
{
#if NETFX
	// TODO 6.0: Consider removing this class in case we upgrade to NET 4.7.2, which has HashSet<T>.TryGetValue
	[Serializable]
	internal class SetSnapShot<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
	{
		private readonly Dictionary<T, T> _values;
		private bool _hasNull;

		public SetSnapShot(int capacity)
		{
			_values = new Dictionary<T, T>(capacity);
		}

		public SetSnapShot(IEnumerable<T> collection)
		{
			_values = new Dictionary<T, T>();
			foreach (var item in collection)
			{
				if (item == null)
				{
					_hasNull = true;
				}
				else
				{
					_values.Add(item, item);
				}
			}
		}

		public bool TryGetValue(T equalValue, out T actualValue)
		{
			if (equalValue != null)
			{
				return _values.TryGetValue(equalValue, out actualValue);
			}

			actualValue = default(T);
			return _hasNull;
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_hasNull)
			{
				yield return default(T);
			}

			foreach (var item in _values.Keys)
			{
				yield return item;
			}
		}

		public void Add(T item)
		{
			if (item == null)
			{
				_hasNull = true;
				return;
			}

			_values.Add(item, item);
		}

		public void Clear()
		{
			_values.Clear();
			_hasNull = false;
		}

		public bool Contains(T item)
		{
			return item == null ? _hasNull : _values.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_values.Keys.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			if (item != null)
			{
				return _values.Remove(item);
			}

			if (!_hasNull)
			{
				return false;
			}

			_hasNull = false;
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (!(array is T[] typedArray))
			{
				throw new ArgumentException($"Array must be of type {typeof(T[])}.", nameof(array));
			}

			CopyTo(typedArray, index);
		}

		public int Count => _values.Count;

		public bool IsReadOnly => ((ICollection<KeyValuePair<T, T>>) _values).IsReadOnly;

		public object SyncRoot => ((ICollection) _values).SyncRoot;

		public bool IsSynchronized => ((ICollection) _values).IsSynchronized;
	}
#endif

#if NETCOREAPP2_0
	[Serializable]
	internal class SetSnapShot<T> : HashSet<T>, ICollection
	{
		[NonSerialized]
		private object _syncRoot;

		public SetSnapShot(int capacity) : base(capacity)
		{
		}

		public SetSnapShot(IEnumerable<T> collection) : base(collection)
		{
		}

		protected SetSnapShot(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (!(array is T[] typedArray))
			{
				throw new ArgumentException($"Array must be of type {typeof(T[])}.", nameof(array));
			}

			CopyTo(typedArray, index);
		}

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot
		{
			get
			{
				if (_syncRoot == null)
				{
					Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
				}

				return _syncRoot;
			}
		}
	}
#endif
}
