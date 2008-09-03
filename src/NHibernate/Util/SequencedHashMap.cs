#region The Apache Software License, Version 1.1

/* This is a port from the Jakarta commons project */

/*
 * ====================================================================
 *
 * The Apache Software License, Version 1.1
 *
 * Copyright (c) 1999-2002 The Apache Software Foundation.  All rights
 * reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution.
 *
 * 3. The end-user documentation included with the redistribution, if
 *    any, must include the following acknowledgement:
 *       "This product includes software developed by the
 *        Apache Software Foundation (http://www.apache.org/)."
 *    Alternately, this acknowledgement may appear in the software itself,
 *    if and wherever such third-party acknowledgements normally appear.
 *
 * 4. The names "The Jakarta Project", "Commons", and "Apache Software
 *    Foundation" must not be used to endorse or promote products derived
 *    from this software without prior written permission. For written
 *    permission, please contact apache@apache.org.
 *
 * 5. Products derived from this software may not be called "Apache"
 *    nor may "Apache" appear in their names without prior written
 *    permission of the Apache Group.
 *
 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 * ====================================================================
 *
 * This software consists of voluntary contributions made by many
 * individuals on behalf of the Apache Software Foundation.  For more
 * information on the Apache Software Foundation, please see
 * <http://www.apache.org/>.
 *
 */

#endregion

using System;
using System.Collections;
using System.Diagnostics;
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
	/// </remarks>
	[DebuggerTypeProxy(typeof(CollectionProxy<>))]
	[Serializable]
	public class SequencedHashMap : IDictionary
	{
		[Serializable]
		private class Entry
		{
			private object _key;
			private object _value;

			private Entry _next = null;
			private Entry _prev = null;

			public Entry(object key, object value)
			{
				_key = key;
				_value = value;
			}

			public object Key
			{
				get { return _key; }
			}

			public object Value
			{
				get { return _value; }
				set { _value = value; }
			}

			public Entry Next
			{
				get { return _next; }
				set { _next = value; }
			}

			public Entry Prev
			{
				get { return _prev; }
				set { _prev = value; }
			}

			#region System.Object Members

			public override int GetHashCode()
			{
				return ((_key == null ? 0 : _key.GetHashCode()) ^ (_value == null ? 0 : _value.GetHashCode()));
			}

			public override bool Equals(object obj)
			{
				Entry other = obj as Entry;
				if (other == null) return false;
				if (other == this) return true;

				return ((_key == null ? other.Key == null : _key.Equals(other.Key)) &&
				        (_value == null ? other.Value == null : _value.Equals(other.Value)));
			}

			public override string ToString()
			{
				return "[" + _key + "=" + _value + "]";
			}

			#endregion
		}


		/// <summary>
		/// Construct an empty sentinel used to hold the head (sentinel.next) and the tail (sentinal.prev)
		/// of the list. The sentinal has a <see langword="null" /> key and value
		/// </summary>
		/// <returns></returns>
		private static Entry CreateSentinel()
		{
			Entry s = new Entry(null, null);
			s.Prev = s;
			s.Next = s;
			return s;
		}

		/// <summary>
		/// Sentinel used to hold the head and tail of the list of entries
		/// </summary>
		private Entry _sentinel;

		/// <summary>
		/// Map of keys to entries
		/// </summary>
		private Hashtable _entries;

		/// <summary>
		/// Holds the number of modifications that have occurred to the map, excluding modifications
		/// made through a collection view's iterator.
		/// </summary>
		private long _modCount = 0;

		/// <summary>
		/// Construct a new sequenced hash map with default initial size and load factor
		/// </summary>
		public SequencedHashMap() : this(0, 1.0F, null)
		{
		}

		/// <summary>
		/// Construct a new sequenced hash map with the specified initial size and default load factor
		/// </summary>
		/// <param name="capacity">the initial size for the hash table</param>
		public SequencedHashMap(int capacity) : this(capacity, 1.0F, null)
		{
		}

		/// <summary>
		/// Construct a new sequenced hash map with the specified initial size and load factor
		/// </summary>
		/// <param name="capacity">the initial size for the hashtable</param>
		/// <param name="loadFactor">the load factor for the hash table</param>
		public SequencedHashMap(int capacity, float loadFactor) : this(capacity, loadFactor, null)
		{
		}

		/// <summary>
		/// Construct a new sequenced hash map with the specified initial size, hash code provider
		/// and comparer
		/// </summary>
		/// <param name="capacity">the initial size for the hashtable</param>
		/// <param name="equalityComparer"></param>
		public SequencedHashMap(int capacity, IEqualityComparer equalityComparer)
			: this(capacity, 1.0F, equalityComparer)
		{
		}

		/// <summary>
		/// Creates an empty Hashtable with the default initial capacity and using the default load factor, 
		/// the specified hash code provider and the specified comparer
		/// </summary>
		/// <param name="equalityComparer"></param>
		public SequencedHashMap(IEqualityComparer equalityComparer)
			: this(0, 1.0F, equalityComparer)
		{
		}

		/// <summary>
		/// Creates an empty Hashtable with the default initial capacity and using the default load factor, 
		/// the specified hash code provider and the specified comparer
		/// </summary>
		/// <param name="capacity">the initial size for the hashtable</param>
		/// <param name="loadFactor">the load factor for the hash table</param>
		/// <param name="equalityComparer"></param>
		public SequencedHashMap(int capacity, float loadFactor, IEqualityComparer equalityComparer)
		{
			_sentinel = CreateSentinel();
			_entries = new Hashtable(capacity, loadFactor, equalityComparer);
		}

		/// <summary>
		/// Removes an internal entry from the linked list. THis does not remove it from the underlying
		/// map.
		/// </summary>
		/// <param name="entry"></param>
		private void RemoveEntry(Entry entry)
		{
			entry.Next.Prev = entry.Prev;
			entry.Prev.Next = entry.Next;
		}

		/// <summary>
		/// Inserts a new internal entry to the tail of the linked list. This does not add the 
		/// entry to the underlying map.
		/// </summary>
		/// <param name="entry"></param>
		private void InsertEntry(Entry entry)
		{
			entry.Next = _sentinel;
			entry.Prev = _sentinel.Prev;
			_sentinel.Prev.Next = entry;
			_sentinel.Prev = entry;
		}

		#region System.Collections.IDictionary Members

		/// <summary></summary>
		public virtual bool IsFixedSize
		{
			get { return false; }
		}

		/// <summary></summary>
		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary></summary>
		public virtual object this[object o]
		{
			get
			{
				Entry entry = (Entry) _entries[o];
				if (entry == null) return null;

				return entry.Value;
			}
			set
			{
				_modCount++;

				Entry e = (Entry) _entries[o];
				if (e != null)
				{
					RemoveEntry(e);
					e.Value = value;
				}
				else
				{
					e = new Entry(o, value);
					_entries[o] = e;
				}

				InsertEntry(e);
			}
		}

		/// <summary></summary>
		public virtual ICollection Keys
		{
			get { return new KeyCollection(this); }
		}

		/// <summary></summary>
		public virtual ICollection Values
		{
			get { return new ValuesCollection(this); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public virtual void Add(object key, object value)
		{
			this[key] = value;
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			_modCount++;

			_entries.Clear();

			_sentinel.Next = _sentinel;
			_sentinel.Prev = _sentinel;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual bool Contains(object key)
		{
			return ContainsKey(key);
		}

		/// <summary></summary>
		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new OrderedEnumerator(this, ReturnType.ReturnEntry);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public virtual void Remove(object key)
		{
			RemoveImpl(key);
		}

		#endregion

		#region System.Collections.ICollection Members

		/// <summary></summary>
		public virtual int Count
		{
			get { return _entries.Count; }
		}

		/// <summary></summary>
		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary></summary>
		public virtual object SyncRoot
		{
			get { return this; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public virtual void CopyTo(Array array, int index)
		{
			foreach (DictionaryEntry de in this)
			{
				array.SetValue(de, index++);
			}
		}

		#endregion

		#region System.Collections.IEnumerable Members

		/// <summary></summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new OrderedEnumerator(this, ReturnType.ReturnEntry);
		}

		#endregion

		private bool IsEmpty
		{
			get { return _sentinel.Next == _sentinel; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual bool ContainsKey(object key)
		{
			return _entries.ContainsKey(key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual bool ContainsValue(object value)
		{
			if (value == null)
			{
				for (Entry pos = _sentinel.Next; pos != _sentinel; pos = pos.Next)
				{
					if (pos.Value == null) return true;
				}
			}
			else
			{
				for (Entry pos = _sentinel.Next; pos != _sentinel; pos = pos.Next)
				{
					if (value.Equals(pos.Value)) return true;
				}
			}
			return false;
		}


		private Entry First
		{
			get { return (IsEmpty) ? null : _sentinel.Next; }
		}

		/// <summary></summary>
		public virtual object FirstKey
		{
			get { return (First == null) ? null : First.Key; }
		}

		/// <summary></summary>
		public virtual object FirstValue
		{
			get { return (First == null) ? null : First.Value; }
		}

		private Entry Last
		{
			get { return (IsEmpty) ? null : _sentinel.Prev; }
		}

		/// <summary></summary>
		public virtual object LastKey
		{
			get { return (Last == null) ? null : Last.Key; }
		}

		/// <summary></summary>
		public virtual object LastValue
		{
			get { return (Last == null) ? null : Last.Value; }
		}


		/// <summary>
		/// Remove the Entry identified by the Key if it exists.
		/// </summary>
		/// <param name="key">The Key to remove.</param>
		private void RemoveImpl(object key)
		{
			Entry e = (Entry) _entries[key];
			if (e != null)
			{
				_entries.Remove(key);
				_modCount++;
				RemoveEntry(e);
			}
		}

		#region System.Object Members

		/// <summary></summary>
		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append('[');
			for (Entry pos = _sentinel.Next; pos != _sentinel; pos = pos.Next)
			{
				buf.Append(pos.Key);
				buf.Append('=');
				buf.Append(pos.Value);
				if (pos.Next != _sentinel)
				{
					buf.Append(',');
				}
			}
			buf.Append(']');

			return buf.ToString();
		}

		#endregion

		private class KeyCollection : ICollection
		{
			private SequencedHashMap _parent;

			public KeyCollection(SequencedHashMap parent)
			{
				_parent = parent;
			}

			#region System.Collections.ICollection Members

			public int Count
			{
				get { return _parent.Count; }
			}

			public bool IsSynchronized
			{
				get { return false; }
			}

			public object SyncRoot
			{
				get { return this; }
			}

			public void CopyTo(Array array, int index)
			{
				foreach (object obj in this)
				{
					array.SetValue(obj, index++);
				}
			}

			#endregion

			#region System.Collections.IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return new OrderedEnumerator(_parent, ReturnType.ReturnKey);
			}

			#endregion

			public bool Contains(object o)
			{
				return _parent.ContainsKey(o);
			}
		}


		private class ValuesCollection : ICollection
		{
			private SequencedHashMap _parent;

			public ValuesCollection(SequencedHashMap parent)
			{
				_parent = parent;
			}

			#region System.Collections.ICollection Members

			public int Count
			{
				get { return _parent.Count; }
			}

			public bool IsSynchronized
			{
				get { return false; }
			}

			public object SyncRoot
			{
				get { return this; }
			}

			public void CopyTo(Array array, int index)
			{
				foreach (object obj in this)
				{
					array.SetValue(obj, index++);
				}
			}

			#endregion

			#region System.Collections.IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return new OrderedEnumerator(_parent, ReturnType.ReturnValue);
			}

			#endregion

			public bool Contains(object o)
			{
				return _parent.ContainsValue(o);
			}
		}


		private enum ReturnType
		{
			/// <summary>
			/// Return only the Key of the DictionaryEntry
			/// </summary>
			ReturnKey,

			/// <summary>
			/// Return only the Value of the DictionaryEntry
			/// </summary>
			ReturnValue,

			/// <summary>
			/// Return the full DictionaryEntry
			/// </summary>
			ReturnEntry
		}


		private class OrderedEnumerator : IDictionaryEnumerator
		{
			private SequencedHashMap _parent;
			private ReturnType _returnType;
			private Entry _pos;
			private long _expectedModCount;

			public OrderedEnumerator(SequencedHashMap parent, ReturnType returnType)
			{
				_parent = parent;
				_returnType = returnType;
				_pos = _parent._sentinel;
				_expectedModCount = _parent._modCount;
			}

			#region System.Collections.IEnumerator Members

			public object Current
			{
				get
				{
					if (_parent._modCount != _expectedModCount)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}


					switch (_returnType)
					{
						case ReturnType.ReturnKey:
							return _pos.Key;
						case ReturnType.ReturnValue:
							return _pos.Value;
						case ReturnType.ReturnEntry:
							return new DictionaryEntry(_pos.Key, _pos.Value);
					}
					return null;
				}
			}

			public bool MoveNext()
			{
				if (_parent._modCount != _expectedModCount)
				{
					throw new InvalidOperationException("Enumerator was modified");
				}
				if (_pos.Next == _parent._sentinel)
				{
					return false;
				}

				_pos = _pos.Next;

				return true;
			}

			public void Reset()
			{
				_pos = _parent._sentinel;
			}

			#endregion

			#region System.Collection.IDictionaryEnumerator Members

			public DictionaryEntry Entry
			{
				get { return new DictionaryEntry(_pos.Key, _pos.Value); }
			}

			public object Key
			{
				get { return _pos.Key; }
			}

			public object Value
			{
				get { return _pos.Value; }
			}

			#endregion
		}
	}
}
