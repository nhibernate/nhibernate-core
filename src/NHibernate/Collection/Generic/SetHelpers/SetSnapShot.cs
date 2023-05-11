using System;
using System.Collections;
using System.Collections.Generic;
#if NETCOREAPP2_0_OR_GREATER
using System.Runtime.Serialization;
using System.Threading;
#endif

namespace NHibernate.Collection.Generic.SetHelpers
{
#if NETFX || NETSTANDARD2_0
	// TODO 6.0: Consider removing this class in case we upgrade to .NET 4.7.2 and NET Standard 2.1,
	// which have HashSet<T>.TryGetValue
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
					_values[item] = item;
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

			_values[item] = item;
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
			if (_hasNull)
				array[arrayIndex] = default(T);
			_values.Keys.CopyTo(array, arrayIndex + (_hasNull ? 1 : 0));
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
			if (array is T[] typedArray)
			{
				CopyTo(typedArray, index);
				return;
			}

			if (array.GetType().GetElementType().IsAssignableFrom(typeof(T)))
			{
				if (_hasNull)
					array.SetValue(default(T), index);
				ICollection keysCollection = _values.Keys;
				keysCollection.CopyTo(array, index + (_hasNull ? 1 : 0));
				return;
			}

			throw new ArgumentException($"Array must be of type {typeof(T[])}.", nameof(array));
		}

		public int Count => _values.Count + (_hasNull ? 1 : 0);

		public bool IsReadOnly => ((ICollection<KeyValuePair<T, T>>) _values).IsReadOnly;

		public object SyncRoot => ((ICollection) _values).SyncRoot;

		public bool IsSynchronized => ((ICollection) _values).IsSynchronized;
	}
#endif

#if NETCOREAPP2_0_OR_GREATER
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
