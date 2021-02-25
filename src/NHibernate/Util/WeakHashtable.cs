using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using NHibernate.DebugHelpers;

namespace NHibernate.Util
{
	// This class does not inherit from WeakReference but uses composition
	// instead to avoid requiring UnmanagedCode permission.
	[DebuggerTypeProxy(typeof(DictionaryProxy))]
	[Serializable]
	public class WeakRefWrapper : IDeserializationCallback
	{
		private WeakReference reference;
		// hashcode may vary among processes, they cannot be stored and have to be re-computed after deserialization
		[NonSerialized]
		private int? _hashCode;

		public WeakRefWrapper(object target)
		{
			reference = new WeakReference(target);

			// No need to delay computation here, but we need the lazy for the deserialization case.
			_hashCode = GenerateHashCode();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}

			WeakRefWrapper that = obj as WeakRefWrapper;
			if (that == null)
			{
				return false;
			}

			object target = Target;
			if (target == null)
			{
				// If the reference is dead, we are only equal to ourselves,
				// and this was checked above.
				return false;
			}

			return GetHashCode() == that.GetHashCode() &&
				   Equals(target, that.Target);
		}

		public override int GetHashCode()
		{
			// If the object is put in a set or dictionary during deserialization, the hashcode will not yet be
			// computed. Compute the hashcode on the fly. So long as this happens only during deserialization, there
			// will be no thread safety issues. For the hashcode to be always defined after deserialization, the
			// deserialization callback is used.
			// As the reference may (unlikely) die between the computation here and the deserialization callback,
			// better also store the computed on the fly value.
			return _hashCode ?? (_hashCode = GenerateHashCode()).Value;
		}

		/// <inheritdoc />
		public void OnDeserialization(object sender)
		{
			// If the value has already been computed for some reasons, keep-it. Otherwise the hashcode may change
			// due to the reference having died.
			if (!_hashCode.HasValue)
				_hashCode = GenerateHashCode();
		}

		private int GenerateHashCode()
		{
			// Defaulting to the base value is not a trouble, because a dead reference is considered only equal to
			// itself.
			return reference.Target?.GetHashCode() ?? base.GetHashCode();
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

		public static object Unwrap(object value)
		{
			if (value == null)
			{
				return null;
			}
			return ((WeakRefWrapper) value).Target;
		}
	}

	public class WeakEnumerator : IDictionaryEnumerator
	{
		private IDictionaryEnumerator innerEnumerator;

		public WeakEnumerator(IDictionaryEnumerator innerEnumerator)
		{
			this.innerEnumerator = innerEnumerator;
		}

		// Unwrapped key and value stored here so that they
		// do not get collected between calls to MoveNext and Current.
		private object currentKey, currentValue;

		public object Key
		{
			get { return currentKey; }
		}

		public object Value
		{
			get { return currentValue; }
		}

		public DictionaryEntry Entry
		{
			get { return new DictionaryEntry(currentKey, currentValue); }
		}

		public bool MoveNext()
		{
			// Skip any garbage-collected key/value pairs
			while (true)
			{
				currentKey = null;
				currentValue = null;

				if (!innerEnumerator.MoveNext())
				{
					return false;
				}

				currentKey = WeakRefWrapper.Unwrap(innerEnumerator.Key);
				currentValue = WeakRefWrapper.Unwrap(innerEnumerator.Value);

				if (currentKey != null && currentValue != null)
				{
					break;
				}
			}

			return true;
		}

		public void Reset()
		{
			innerEnumerator.Reset();
			currentKey = currentValue = null;
		}

		public object Current
		{
			get { return Entry; }
		}
	}

	[Serializable]
	public class WeakHashtable : IDictionary
	{
		private Hashtable innerHashtable = new Hashtable();

		public void Scavenge()
		{
			var deadKeys = new List<object>();

			foreach (DictionaryEntry de in innerHashtable)
			{
				WeakRefWrapper key = (WeakRefWrapper) de.Key;
				WeakRefWrapper value = (WeakRefWrapper) de.Value;

				if (!key.IsAlive || !value.IsAlive)
				{
					deadKeys.Add(key);
				}
			}

			foreach (object key in deadKeys)
			{
				innerHashtable.Remove(key);
			}
		}

		public bool Contains(object key)
		{
			return innerHashtable.Contains(WeakRefWrapper.Wrap(key));
		}

		public void Add(object key, object value)
		{
			Scavenge();
			innerHashtable.Add(WeakRefWrapper.Wrap(key), WeakRefWrapper.Wrap(value));
		}

		public void Clear()
		{
			innerHashtable.Clear();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new WeakEnumerator(innerHashtable.GetEnumerator());
		}

		public void Remove(object key)
		{
			innerHashtable.Remove(WeakRefWrapper.Wrap(key));
		}

		public object this[object key]
		{
			get { return WeakRefWrapper.Unwrap(innerHashtable[WeakRefWrapper.Wrap(key)]); }
			set
			{
				Scavenge();
				innerHashtable[WeakRefWrapper.Wrap(key)] = WeakRefWrapper.Wrap(value);
			}
		}

		public ICollection Keys
		{
			get { throw new NotImplementedException(); }
		}

		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { return innerHashtable.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return innerHashtable.IsFixedSize; }
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Count of elements in the collection. Unreliable!
		/// </summary>
		public int Count
		{
			get { return innerHashtable.Count; }
		}

		public object SyncRoot
		{
			get { return innerHashtable.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return innerHashtable.IsSynchronized; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
