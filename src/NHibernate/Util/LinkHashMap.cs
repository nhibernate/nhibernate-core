using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using NHibernate.DebugHelpers;

namespace NHibernate.Util
{
	/// <summary>
	/// A map of objects whose mapping entries are sequenced based on the order in which they were
	/// added. This data structure has fast <c>O(1)</c> search time, deletion time, and insertion time
	/// </summary>
	/// <remarks>
	/// This class is not thread safe.
	/// This class is not really a replication of JDK LinkedHashMap{K, V}, 
	/// this class is an adaptation of SequencedHashMap with generics.
	/// </remarks>
	[DebuggerTypeProxy(typeof(CollectionProxy<>))]
	[Serializable]
	internal class LinkHashMap<TKey, TValue> : IDictionary<TKey, TValue>, IDeserializationCallback
	{
		[Serializable]
		protected class Entry
		{
			public Entry(TKey key, TValue value)
			{
				Key = key;
				Value = value;
			}

			public TKey Key { get; }

			public TValue Value { get; set; }

			public Entry Next { get; set; }

			public Entry Prev { get; set; }

			#region System.Object Members

			public override int GetHashCode()
			{
				return Key == null ? 0 : Key.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				var other = obj as Entry;
				if (other == null) return false;
				if (other == this) return true;

				return (Key == null ? other.Key == null : Key.Equals(other.Key)) &&
						(Value == null ? other.Value == null : Value.Equals(other.Value));
			}

			public override string ToString()
			{
				return "[" + Key + "=" + Value + "]";
			}

			#endregion
		}

		private readonly Entry _header;
		private readonly Dictionary<TKey, Entry> _entries;
		private long _version;

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkHashMap{K,V}"/> class that is empty, 
		/// has the default initial capacity, and uses the default equality comparer for the key type.
		/// </summary>
		public LinkHashMap()
			: this(0, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkHashMap{K,V}"/> class that is empty, 
		/// has the specified initial capacity, and uses the default equality comparer for the key type.
		/// </summary>
		/// <param name="capacity">The initial number of elements that the <see cref="LinkHashMap{K,V}"/> can contain.</param>
		public LinkHashMap(int capacity)
			: this(capacity, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkHashMap{K,V}"/> class that is empty, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{K}"/>.
		/// </summary>
		/// <param name="equalityComparer">The <see cref="IEqualityComparer{K}"/> implementation to use when comparing keys, or null to use the default EqualityComparer for the type of the key.</param>
		public LinkHashMap(IEqualityComparer<TKey> equalityComparer)
			: this(0, equalityComparer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkHashMap{K,V}"/> class that is empty, has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{K}"/>.
		/// </summary>
		/// <param name="capacity">The initial number of elements that the <see cref="LinkHashMap{K,V}"/> can contain.</param>
		/// <param name="equalityComparer">The <see cref="IEqualityComparer{K}"/> implementation to use when comparing keys, or null to use the default EqualityComparer for the type of the key.</param>
		public LinkHashMap(int capacity, IEqualityComparer<TKey> equalityComparer)
		{
			_header = CreateSentinel();
			_entries = new Dictionary<TKey, Entry>(capacity, equalityComparer);
		}

		#region IDictionary<TKey,TValue> Members

		public virtual bool ContainsKey(TKey key)
		{
			return _entries.ContainsKey(key);
		}

		public virtual void Add(TKey key, TValue value)
		{
			var e = new Entry(key, value);
			_entries.Add(key, e);
			_version++;
			InsertEntry(e);
		}

		public virtual bool Remove(TKey key)
		{
			return RemoveImpl(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			var result = _entries.TryGetValue(key, out var entry);
			if (result)
				value = entry.Value;
			else
				value = default;

			return result;
		}

		public TValue this[TKey key]
		{
			get
			{
				return _entries[key].Value;
			}
			set
			{
				if (_entries.TryGetValue(key, out var e))
					OverrideEntry(e, value);
				else
					Add(key, value);
			}
		}

		private void OverrideEntry(Entry e, TValue value)
		{
			_version++;
			RemoveEntry(e);
			e.Value = value;
			InsertEntry(e);
		}

		public KeyCollection Keys => new KeyCollection(this);

		ICollection<TKey> IDictionary<TKey, TValue>.Keys => new KeyCollection(this);

		public virtual ValueCollection Values => new ValueCollection(this);

		ICollection<TValue> IDictionary<TKey, TValue>.Values => new ValueCollection(this);

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public virtual void Clear()
		{
			_version++;

			_entries.Clear();

			_header.Next = _header;
			_header.Prev = _header;
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return Contains(item.Key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			foreach (var pair in this)
				array.SetValue(pair, arrayIndex++);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		public virtual int Count => _entries.Count;

		public virtual bool IsReadOnly => false;

		#endregion

		public Enumerator GetEnumerator() => new Enumerator(this);

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region LinkHashMap Members

		private bool IsEmpty
		{
			get { return _header.Next == _header; }
		}

		public virtual bool IsFixedSize => false;

		public virtual TKey FirstKey
		{
			get { return First == null ? default : First.Key; }
		}

		public virtual TValue FirstValue
		{
			get { return First == null ? default : First.Value; }
		}

		public virtual TKey LastKey
		{
			get { return Last == null ? default : Last.Key; }
		}

		public virtual TValue LastValue
		{
			get { return Last == null ? default : Last.Value; }
		}

		public virtual bool Contains(TKey key)
		{
			return ContainsKey(key);
		}

		public virtual bool ContainsValue(TValue value)
		{
			if (value == null)
			{
				for (var entry = _header.Next; entry != _header; entry = entry.Next)
				{
					if (entry.Value == null) return true;
				}
			}
			else
			{
				for (var entry = _header.Next; entry != _header; entry = entry.Next)
				{
					if (value.Equals(entry.Value)) return true;
				}
			}
			return false;
		}

		#endregion

		private static Entry CreateSentinel()
		{
			var s = new Entry(default, default);
			s.Prev = s;
			s.Next = s;
			return s;
		}

		private static void RemoveEntry(Entry entry)
		{
			entry.Next.Prev = entry.Prev;
			entry.Prev.Next = entry.Next;
		}

		private void InsertEntry(Entry entry)
		{
			entry.Next = _header;
			entry.Prev = _header.Prev;
			_header.Prev.Next = entry;
			_header.Prev = entry;
		}

		private Entry First
		{
			get { return IsEmpty ? null : _header.Next; }
		}

		private Entry Last
		{
			get { return IsEmpty ? null : _header.Prev; }
		}

		private bool RemoveImpl(TKey key)
		{
			if (!_entries.Remove(key, out var e)) 
				return false;

			_version++;
			RemoveEntry(e);
			return true;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			((IDeserializationCallback)_entries).OnDeserialization(sender);
		}

		#region System.Object Members

		public override string ToString()
		{
			var buf = new StringBuilder();
			buf.Append('[');
			for (Entry pos = _header.Next; pos != _header; pos = pos.Next)
			{
				buf.Append(pos.Key);
				buf.Append('=');
				buf.Append(pos.Value);
				if (pos.Next != _header)
				{
					buf.Append(',');
				}
			}
			buf.Append(']');

			return buf.ToString();
		}

		#endregion

		public class KeyCollection : ICollection<TKey>
		{
			private readonly LinkHashMap<TKey, TValue> _dictionary;

			public KeyCollection(LinkHashMap<TKey, TValue> dictionary)
			{
				_dictionary = dictionary;
			}

			#region ICollection<TKey> Members

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException($"{nameof(LinkHashMap<TKey,TValue>)}+{nameof(KeyCollection)} is readonly.");
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException($"{nameof(LinkHashMap<TKey,TValue>)}+{nameof(KeyCollection)} is readonly.");
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				foreach (var key in this)
				{
					if (key.Equals(item))
						return true;
				}
				return false;
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				foreach (var key in this)
					array.SetValue(key, arrayIndex++);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException($"{nameof(LinkHashMap<TKey,TValue>)}+{nameof(KeyCollection)} is readonly.");
			}

			public int Count => _dictionary.Count;

			bool ICollection<TKey>.IsReadOnly => true;

			#endregion

			public Enumerator GetEnumerator() => new(_dictionary);

			#region IEnumerable<TKey> Members

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => new Enumerator(_dictionary);

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_dictionary);

			#endregion

			public struct Enumerator : IEnumerator<TKey>
			{
				private readonly LinkHashMap<TKey, TValue> _dictionary;
				private Entry _current;
				private readonly long _version;

				public Enumerator(LinkHashMap<TKey, TValue> dictionary)
				{
					_dictionary = dictionary;
					_version = dictionary._version;
					_current = dictionary._header;
				}

				public bool MoveNext()
				{
					if (_dictionary._version != _version)
						throw new InvalidOperationException("Enumerator was modified");

					if (_current.Next == _dictionary._header)
						return false;

					_current = _current.Next;

					return true;
				}

				public TKey Current
				{
					get
					{
						if (_dictionary._version != _version)
							throw new InvalidOperationException("Enumerator was modified");

						return _current.Key;
					}
				}

				object IEnumerator.Current => Current;

				void IEnumerator.Reset()
				{
					_current = _dictionary._header;
				}

				void IDisposable.Dispose() { }
			}
		}

		public class ValueCollection : ICollection<TValue>
		{
			private readonly LinkHashMap<TKey, TValue> _dictionary;

			public ValueCollection(LinkHashMap<TKey, TValue> dictionary)
			{
				_dictionary = dictionary;
			}

			#region ICollection<TValue> Members

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException($"{nameof(LinkHashMap<TKey,TValue>)}+{nameof(ValueCollection)} is readonly.");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException($"{nameof(LinkHashMap<TKey,TValue>)}+{nameof(ValueCollection)} is readonly.");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				foreach (var value in this)
				{
					if (value.Equals(item))
						return true;
				}
				return false;
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				foreach (var value in this)
					array.SetValue(value, arrayIndex++);
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException($"{nameof(LinkHashMap<TKey,TValue>)}+{nameof(ValueCollection)} is readonly.");
			}

			public int Count => _dictionary.Count;

			bool ICollection<TValue>.IsReadOnly => true;

			#endregion

			public Enumerator GetEnumerator() => new Enumerator(_dictionary);

			#region IEnumerable<TKey> Members

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => new Enumerator(_dictionary);

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_dictionary);

			#endregion

			public struct Enumerator : IEnumerator<TValue>
			{
				private readonly LinkHashMap<TKey, TValue> _dictionary;
				private Entry _current;
				private readonly long _version;

				public Enumerator(LinkHashMap<TKey, TValue> dictionary)
				{
					_dictionary = dictionary;
					_version = dictionary._version;
					_current = dictionary._header;
				}

				public bool MoveNext()
				{
					if (_dictionary._version != _version)
						throw new InvalidOperationException("Enumerator was modified");

					if (_current.Next == _dictionary._header)
						return false;

					_current = _current.Next;

					return true;
				}

				public TValue Current
				{
					get
					{
						if (_dictionary._version != _version)
							throw new InvalidOperationException("Enumerator was modified");

						return _current.Value;
					}
				}

				object IEnumerator.Current => Current;

				void IEnumerator.Reset()
				{
					_current = _dictionary._header;
				}

				void IDisposable.Dispose() { }
			}
		}

		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			private readonly LinkHashMap<TKey, TValue> _dictionary;
			private Entry _current;
			private readonly long _version;

			public Enumerator(LinkHashMap<TKey, TValue> dictionary)
			{
				_dictionary = dictionary;
				_version = dictionary._version;
				_current = dictionary._header;
			}

			public bool MoveNext()
			{
				if (_dictionary._version != _version)
					throw new InvalidOperationException("Enumerator was modified");

				if (_current.Next == _dictionary._header)
					return false;

				_current = _current.Next;

				return true;
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (_dictionary._version != _version)
						throw new InvalidOperationException("Enumerator was modified");

					return new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
				}
			}

			object IEnumerator.Current => Current;

			void IEnumerator.Reset()
			{
				_current = _dictionary._header;
			}

			void IDisposable.Dispose() { }
		}
	}
}
