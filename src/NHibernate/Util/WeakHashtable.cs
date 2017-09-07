using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.DebugHelpers;

namespace NHibernate.Util
{
	// This class does not inherit from WeakReference but uses composition
	// instead to avoid requiring UnmanagedCode permission.
	[DebuggerTypeProxy(typeof(DictionaryProxy))]
	[Serializable]
	public class WeakRefWrapper
	{
		private WeakReference reference;
		private int hashCode;

		private WeakRefWrapper(object target)
		{
			reference = new WeakReference(target);
			hashCode = target.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}

			var that = obj as WeakRefWrapper;
			if (that == null)
			{
				return false;
			}

			var target = Target;
			if (target == null)
			{
				// If the reference is dead, we are only equal to ourselves,
				// and this was checked above.
				return false;
			}

			return hashCode == that.hashCode &&
				   Equals(target, that.Target);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public object Target
		{
			get { return reference.Target; }
		}

		public bool IsAlive
		{
			get { return reference.IsAlive; }
		}

		public static WeakRefWrapper Wrap(object value)
		{
			return new WeakRefWrapper(value);
		}

		public static object Unwrap(WeakRefWrapper value)
		{
			if (value == null)
			{
				return null;
			}
			return value.Target;
		}
	}

	public class WeakEnumerator : IEnumerator<KeyValuePair<object, object>>
	{
		private IEnumerator<KeyValuePair<WeakRefWrapper, WeakRefWrapper>> _innerEnumerator;

		public WeakEnumerator(IEnumerator<KeyValuePair<WeakRefWrapper, WeakRefWrapper>> innerEnumerator)
		{
			_innerEnumerator = innerEnumerator;
		}

		// Unwrapped key and value stored here so that they
		// do not get collected between calls to MoveNext and Current.
		private KeyValuePair<object, object> _current;

		public bool MoveNext()
		{
			// Skip any garbage-collected key/value pairs
			while (true)
			{

				if (!_innerEnumerator.MoveNext())
				{
					_current = default(KeyValuePair<object, object>);
					return false;
				}

				var currentKey = WeakRefWrapper.Unwrap(_innerEnumerator.Current.Key);
				var currentValue = WeakRefWrapper.Unwrap(_innerEnumerator.Current.Value);

				if (currentKey != null && currentValue != null)
				{
					_current = new KeyValuePair<object, object>(currentKey, currentValue);
					break;
				}
			}

			return true;
		}

		public void Reset()
		{
			_innerEnumerator.Reset();
			_current = default(KeyValuePair<object, object>);
		}

		public KeyValuePair<object, object> Current => _current;

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
					_current = default(KeyValuePair<object, object>);
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
	public class WeakHashtable : IDictionary<object, object>
	{
		private Dictionary<WeakRefWrapper, WeakRefWrapper> _innerHashtable = new Dictionary<WeakRefWrapper, WeakRefWrapper>();

		public void Scavenge()
		{
			var deadKeys = new List<WeakRefWrapper>();

			foreach (var de in _innerHashtable)
			{
				if (!de.Key.IsAlive || !de.Value.IsAlive)
				{
					deadKeys.Add(de.Key);
				}
			}

			foreach (var key in deadKeys)
			{
				_innerHashtable.Remove(key);
			}
		}

		public bool ContainsKey(object key)
		{
			return _innerHashtable.ContainsKey(WeakRefWrapper.Wrap(key));
		}

		public void Add(object key, object value)
		{
			Scavenge();
			_innerHashtable.Add(WeakRefWrapper.Wrap(key), WeakRefWrapper.Wrap(value));
		}

		public void Clear()
		{
			_innerHashtable.Clear();
		}

		public bool Remove(object key)
		{
			return _innerHashtable.Remove(WeakRefWrapper.Wrap(key));
		}

		public bool TryGetValue(object key, out object value)
		{
			if (_innerHashtable.TryGetValue(WeakRefWrapper.Wrap(key), out var wrappedValue))
			{
				value = WeakRefWrapper.Unwrap(wrappedValue);
				// If it was no more alive, it will be unwrapped as null.
				return value != null;
			}
			value = null;
			return false;
		}

		public object this[object key]
		{
			get { return WeakRefWrapper.Unwrap(_innerHashtable[WeakRefWrapper.Wrap(key)]); }
			set
			{
				Scavenge();
				_innerHashtable[WeakRefWrapper.Wrap(key)] = WeakRefWrapper.Wrap(value);
			}
		}

		public ICollection<object> Keys
			=> _innerHashtable.Keys.Select(k => WeakRefWrapper.Unwrap(k)).ToArray();

		public ICollection<object> Values
			=> _innerHashtable.Values.Select(k => WeakRefWrapper.Unwrap(k)).ToArray();

		#region ICollection<object, object>

		public void Add(KeyValuePair<object, object> item)
		{
			Scavenge();
			_innerHashtable.Add(WeakRefWrapper.Wrap(item.Key), WeakRefWrapper.Wrap(item.Value));
		}

		public bool Contains(KeyValuePair<object, object> item)
		{
			return TryGetValue(item.Key, out var value) && value == item.Value;
		}

		public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			foreach(var pair in this)
			{
				array[arrayIndex] = pair;
				arrayIndex++;
			}
		}

		public bool Remove(KeyValuePair<object, object> item)
		{
			return Contains(item) && Remove(item.Key);
		}

		/// <summary>
		/// Count of elements in the collection. Unreliable: does not exclude dead references, contrary to the enumerator.
		/// </summary>
		public int Count => _innerHashtable.Count;

		public bool IsReadOnly => false;

		#endregion

		public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
			=> new WeakEnumerator(_innerHashtable.GetEnumerator());

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
