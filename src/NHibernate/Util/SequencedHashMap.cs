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
 *    any, must include the following acknowlegement:
 *       "This product includes software developed by the
 *        Apache Software Foundation (http://www.apache.org/)."
 *    Alternately, this acknowlegement may appear in the software itself,
 *    if and wherever such third-party acknowlegements normally appear.
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

using System;
using System.Collections;

namespace NHibernate.Util {
	/// <summary>
	/// A map of objects whose mapping entries are sequenced based on the order in which they were
	/// added. This data structure has fast <c>O(1)</c> search time, deletion time, and insertion time
	/// </summary>
	/// <remarks>
	/// This class is not thread safe.
	/// </remarks>
	public class SequencedHashMap : IDictionary {
		
		private class Entry  {
			private object key;
			private object value;

			public Entry next = null;
			public Entry prev = null;

			public Entry(object key, object value) {
				this.key = key;
				this.value = value;
			}

			public object Key {
				get { return key; }
			}
			public object Value {
				get { return value; }
				set { this.value = value; }
			}

			public override int GetHashCode() {
				return ((Key == null ? 0 : Key.GetHashCode()) ^ (Value == null ? 0 : Value.GetHashCode()) );
			}

			public override bool Equals(object obj) {
				Entry other = obj as Entry;
				if (other == null) return false;
				if (other == this) return true;

				return ( (Key == null ? other.Key == null : Key.Equals(other.Key)) &&
					(Value == null ? other.Value == null : Value.Equals(other.Value)) );
			}

			public override string ToString() {
				return "[" + Key + "=" + Value + "]";
			}

		}

		/// <summary>
		/// Construct an empty sentinel used to hold the head (sentinel.next) and the tail (sentinal.prev)
		/// of the list. The sentinal has a <c>null</c> key and value
		/// </summary>
		/// <returns></returns>
		private static Entry CreateSentinel() {
			Entry s = new Entry(null, null);
			s.prev = s;
			s.next = s;
			return s;
		}

		/// <summary>
		/// Sentinel used to hold the head and tail of the list of entries
		/// </summary>
		private Entry sentinel;

		/// <summary>
		/// Map of keys to entries
		/// </summary>
		private Hashtable entries;

		/// <summary>
		/// Holds the number of modifications that have occurred to the map, excluding modifications
		/// made through a collection view's iterator.
		/// </summary>
		private long modCount = 0;

		/// <summary>
		/// Construct a new sequenced hash map with default initial size and load factor
		/// </summary>
		public SequencedHashMap() {
			sentinel = CreateSentinel();
			entries = new Hashtable();
		}

		/// <summary>
		/// Construct a new sequenced hash map with the specified initial size and default load factor
		/// </summary>
		/// <param name="initialSize">the initial size for the hash table</param>
		public SequencedHashMap(int initialSize) {
			sentinel = CreateSentinel();
			entries = new Hashtable(initialSize);
		}

		/// <summary>
		/// Construct a new sequenced hash map with the specified initial size and load factor
		/// </summary>
		/// <param name="initialSize">the initial size for the hashtable</param>
		/// <param name="loadFactor">the load factor for the hash table</param>
		public SequencedHashMap(int initialSize, float loadFactor) {
			sentinel = CreateSentinel();
			entries = new Hashtable(initialSize, loadFactor);
		}

		
		/// <summary>
		/// Removes an internal entry from the linked list. THis does not remove it from the underlying
		/// map.
		/// </summary>
		/// <param name="entry"></param>
		private void RemoveEntry(Entry entry) {
			entry.next.prev = entry.prev;
			entry.prev.next = entry.next;
		}
		
		/// <summary>
		/// Inserts a new internal entry to the tail of the linked list. This does not add the 
		/// entry to the underlying map.
		/// </summary>
		/// <param name="entry"></param>
		private void InsertEntry(Entry entry) {
			entry.next = sentinel;
			entry.prev = sentinel.prev;
			sentinel.prev.next = entry;
			sentinel.prev = entry;
		}

		public void CopyTo(Array array, int i) {
			//TODO: implement
		}

		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return this; }
		}

		public int Count {
			get { return entries.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool IsFixedSize {
			get { return false; }
		}

		public bool IsEmpty {
			get { return sentinel.next == sentinel; }
		}

		public bool ContainsKey(object key) {
			return entries.ContainsKey(key);
		}

		public bool ContainsValue(object value) {
			if (value == null) {
				for(Entry pos = sentinel.next; pos != sentinel; pos = pos.next) {
					if (pos.Value == null) return true;
				}
			} else {
				for(Entry pos = sentinel.next; pos != sentinel; pos = pos.next) {
					if (value.Equals(pos.Value)) return true;
				}
			}
			return false;
		}

		public bool Contains(object key) {
			return ContainsKey(key); //This is probably wrong....
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException("this operation is not yet implemented");
		}

		IDictionaryEnumerator IDictionary.GetEnumerator() {
			throw new NotImplementedException("this operation is not yet implemented");
		}

		public void Add(object key, object value) {
			this[key] = value; 
		}

		public object this [ object o ] {
			get {
				Entry entry = (Entry) entries[o];
				if (entry == null) return null;

				return entry.Value;
			}
			set {
				modCount++;

				//object oldValue = null;

				Entry e = (Entry)entries[o];
				if (e != null) {
					RemoveEntry(e);

					e.Value = value;
					//oldValue = value;
				} else {
					e = new Entry(o, value);
					entries[o] = e;
				}
				InsertEntry(e);
			}
		}

		private Entry First {
			get { return (IsEmpty) ? null : sentinel.next; }
		}

		public object FirstKey {
			get { return sentinel.next.Key; }
		}

		public object FirstValue {
			get { return sentinel.next.Value; }
		}

		private Entry Last {
			get { return (IsEmpty) ? null : sentinel.prev; }
		}

		public object LastKey {
			get { return sentinel.prev.Key; }
		}

		public object LastValue {
			get { return sentinel.prev.Value; }
		}

		public void Remove(object key) {
			Entry e = RemoveImpl(key);
		}

		private Entry RemoveImpl(object key) {
			Entry e = (Entry) entries[key];
			entries.Remove(key);
			modCount++;
			RemoveEntry(e);
			return e;

		}

		public void Clear() {
			modCount++;

			entries.Clear();

			sentinel.next = sentinel;
			sentinel.prev = sentinel;
		}

		public override bool Equals(object obj) {
			if (obj == null) return false;
			if (obj == this) return true;

			if (!(obj is IDictionary)) return false;

			return Keys.Equals(((IDictionary)obj).Keys);
		}

		public override int GetHashCode() {
			return Keys.GetHashCode();
		}

		public override string ToString() {
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			buf.Append('[');
			for (Entry pos = sentinel.next; pos != sentinel; pos = pos.next) {
				buf.Append(pos.Key);
				buf.Append('=');
				buf.Append(pos.Value);
				if (pos.next != sentinel) {
					buf.Append(',');
				}
			}
			buf.Append(']');

			return buf.ToString();
		}

		public ICollection Keys {
			get { return new KeyCollection(this); }
		}
		private class KeyCollection : ICollection {
			private SequencedHashMap parent;

			public void CopyTo(Array array, int i) {
				//TODO: implement
			}

			public object SyncRoot {
				get { return this; }
			}

			public bool IsSynchronized {
				get { return false; }
			}

			public KeyCollection(SequencedHashMap parent) {
				this.parent = parent;
			}

			public IEnumerator GetEnumerator() {
				return new OrderedEnumerator(parent, KEY);
			}

			public int Count {
				get { return parent.Count; }
			}
			public bool Contains(object o) {
				return parent.ContainsKey(o);
			}
		}
		public ICollection Values {
			get { return new ValuesCollection(this); }
		}
		private class ValuesCollection : ICollection {
			private SequencedHashMap parent;

			public void CopyTo(Array array, int i) {
				//TODO: implement
			}

			public object SyncRoot {
				get { return this; }
			}

			public bool IsSynchronized {
				get { return false; }
			}

			public ValuesCollection(SequencedHashMap parent) {
				this.parent = parent;
			}

			public IEnumerator GetEnumerator() {
				return new OrderedEnumerator(parent, VALUE); 
			}

			public int Count {
				get { return parent.Count; }
			}

			public bool Contains(object o) {
				return parent.ContainsValue(o);
			}
		}
			
		private const int KEY = 0;
		private const int VALUE = 1;
		private const int ENTRY = 2;

		private class OrderedEnumerator : IEnumerator {
			private SequencedHashMap parent;

			public OrderedEnumerator(SequencedHashMap parent, int returnType) {
				this.parent = parent;
				this.returnType = returnType;
				pos = parent.sentinel;
				expectedModCount = parent.modCount;
			}

			private int returnType;
			private Entry pos;
			private long expectedModCount;

			public object Current {
				get { 
					if (parent.modCount != expectedModCount) {
						throw new InvalidOperationException("Enumerator was modified");
					}
					//if (pos.next == parent.sentinel) {
					//	throw new InvalidOperationException("moved past end");
					//}

					switch(returnType) {
						case KEY:
							return pos.Key;
						case VALUE:
							return pos.Value;
						case ENTRY:
							return pos;
					}
					return null;
				}
			}
			public bool MoveNext() {


				if (parent.modCount != expectedModCount) {
					throw new InvalidOperationException("Enumerator was modified");
				}
				if (pos.next == parent.sentinel) {
					return false;
				}

				pos = pos.next;

				return true;
			}
			public void Reset() {
				pos = parent.sentinel;
			}
		}

		

		 
	}
}
