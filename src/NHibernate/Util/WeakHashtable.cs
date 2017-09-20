using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Util
{
	// This class does not inherit from WeakReference but uses composition
	// instead to avoid requiring UnmanagedCode permission.
	[Serializable]
	public class WeakRefWrapper<T> : IEquatable<WeakRefWrapper<T>> where T : class
	{
		private WeakReference<T> _reference;
		private int _hashCode;

		internal WeakRefWrapper(T target)
		{
			_reference = new WeakReference<T>(target);
			_hashCode = target.GetHashCode();
		}

		public override bool Equals(object obj)
			=> Equals(obj as WeakRefWrapper<T>);

		public bool Equals(WeakRefWrapper<T> other)
		{
			if (other == null)
				return false;
			if (this == other)
				return true;

			if (!TryGetTarget(out var target))
			{
				// If the reference is dead, we are only equal to ourselves,
				// and this was checked above.
				return false;
			}

			return _hashCode == other._hashCode &&
				other.TryGetTarget(out var otherTarget) &&
				Equals(target, otherTarget);
		}

		public override int GetHashCode()
			=> _hashCode;

		public bool TryGetTarget(out T target)
			=> _reference.TryGetTarget(out target);

		public static WeakRefWrapper<T> Wrap(T value)
			=> new WeakRefWrapper<T>(value);

		public static T Unwrap(WeakRefWrapper<T> value)
		{
			if (value != null && value.TryGetTarget(out var target))
				return target;
			return null;
		}
	}

	internal class WeakEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>> where TKey : class where TValue : class
	{
		private IEnumerator<KeyValuePair<WeakRefWrapper<TKey>, WeakRefWrapper<TValue>>> _innerEnumerator;

		public WeakEnumerator(IEnumerator<KeyValuePair<WeakRefWrapper<TKey>, WeakRefWrapper<TValue>>> innerEnumerator)
		{
			_innerEnumerator = innerEnumerator;
		}

		// Unwrapped key and value stored here so that they
		// do not get collected between calls to MoveNext and Current.
		private KeyValuePair<TKey, TValue> _current;

		public bool MoveNext()
		{
			// Skip any garbage-collected key/value pairs
			while (true)
			{

				if (!_innerEnumerator.MoveNext())
				{
					_current = default(KeyValuePair<TKey, TValue>);
					return false;
				}

				var currentKey = WeakRefWrapper<TKey>.Unwrap(_innerEnumerator.Current.Key);
				var currentValue = WeakRefWrapper<TValue>.Unwrap(_innerEnumerator.Current.Value);

				if (currentKey != null && currentValue != null)
				{
					_current = new KeyValuePair<TKey, TValue>(currentKey, currentValue);
					break;
				}
			}

			return true;
		}

		public void Reset()
		{
			_innerEnumerator.Reset();
			_current = default(KeyValuePair<TKey, TValue>);
		}

		public KeyValuePair<TKey, TValue> Current => _current;

		object IEnumerator.Current => _current;

		#region IDisposable Support
		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_innerEnumerator.Dispose();
					_current = default(KeyValuePair<TKey, TValue>);
				}

				_disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}

	[Serializable]
	public class WeakHashtable<TKey, TValue> : IDictionary<TKey, TValue> where TKey : class where TValue : class
	{
		private Dictionary<WeakRefWrapper<TKey>, WeakRefWrapper<TValue>> _innerHashtable =
			new Dictionary<WeakRefWrapper<TKey>, WeakRefWrapper<TValue>>();

		public void Scavenge()
		{
			var deadKeys = new List<WeakRefWrapper<TKey>>();

			foreach (var de in _innerHashtable)
			{
				if (!de.Key.TryGetTarget(out _) || !de.Value.TryGetTarget(out _))
				{
					deadKeys.Add(de.Key);
				}
			}

			foreach (var key in deadKeys)
			{
				_innerHashtable.Remove(key);
			}
		}

		public bool ContainsKey(TKey key)
		{
			return _innerHashtable.ContainsKey(WeakRefWrapper<TKey>.Wrap(key));
		}

		public void Add(TKey key, TValue value)
		{
			Scavenge();
			_innerHashtable.Add(WeakRefWrapper<TKey>.Wrap(key), WeakRefWrapper<TValue>.Wrap(value));
		}

		public void Clear()
		{
			_innerHashtable.Clear();
		}

		public bool Remove(TKey key)
		{
			return _innerHashtable.Remove(WeakRefWrapper<TKey>.Wrap(key));
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (_innerHashtable.TryGetValue(WeakRefWrapper<TKey>.Wrap(key), out var wrappedValue))
			{
				value = WeakRefWrapper<TValue>.Unwrap(wrappedValue);
				// If it was no more alive, it will be unwrapped as null.
				return value != null;
			}
			value = null;
			return false;
		}

		public TValue this[TKey key]
		{
			get { return WeakRefWrapper<TValue>.Unwrap(_innerHashtable[WeakRefWrapper<TKey>.Wrap(key)]); }
			set
			{
				Scavenge();
				_innerHashtable[WeakRefWrapper<TKey>.Wrap(key)] = WeakRefWrapper<TValue>.Wrap(value);
			}
		}

		public ICollection<TKey> Keys
			=> _innerHashtable.Keys.Select(k => WeakRefWrapper<TKey>.Unwrap(k)).ToArray();

		public ICollection<TValue> Values
			=> _innerHashtable.Values.Select(k => WeakRefWrapper<TValue>.Unwrap(k)).ToArray();

		#region ICollection<TKey, TValue>

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Scavenge();
			_innerHashtable.Add(WeakRefWrapper<TKey>.Wrap(item.Key), WeakRefWrapper<TValue>.Wrap(item.Value));
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return TryGetValue(item.Key, out var value) && value == item.Value;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			foreach (var pair in this)
			{
				array[arrayIndex] = pair;
				arrayIndex++;
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Contains(item) && Remove(item.Key);
		}

		/// <summary>
		/// Count of elements in the collection. Unreliable: does not exclude dead references, contrary to the enumerator.
		/// </summary>
		public int Count => _innerHashtable.Count;

		public bool IsReadOnly => false;

		#endregion

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			=> new WeakEnumerator<TKey, TValue>(_innerHashtable.GetEnumerator());

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
