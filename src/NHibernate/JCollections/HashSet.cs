using System;

using System.Collections;

namespace NHibernate.JCollections {
	/// <summary>
	/// An implementation of the Java HashSet class
	/// Uses an internal hashtable for implementation
	/// </summary>
	public class HashSet : ISet {
		Hashtable _hashTable;
		bool _hasNullItem = false;

		// initial construction values, used for certain operations 
		// that recreate the internal hashTable
		int _capacity;
		float _loadFactor;

		public HashSet() : this(0) {
		}

		public HashSet(int capacity) : this(capacity, 1.0f) {
		}

		public HashSet(int capacity, float loadFactor) {
			this._capacity = capacity;
			this._loadFactor = loadFactor;
			this._hashTable = new Hashtable(capacity, loadFactor);
		}

		public HashSet(ICollection c) : this(c.Count, 1.0f) {
			foreach (object o in c) {
				this.Add(o);
			}
		}

		public bool Add(object o) {
			// A hashset does take a (single) null item, as .NET Hashtable doesn't we use a flag
			if (o == null) {
				if (this._hasNullItem)
					return false;
				this._hasNullItem = true;
				return true;
			}

			if (this._hashTable.ContainsKey(o.GetHashCode()))
				return false;
			this._hashTable.Add(o.GetHashCode(), o);
			return true;
		}

		public bool AddAll(ICollection c) {
			bool itemAdded = false;
			foreach (object o in c) {
				itemAdded |= this.Add(o);
			}
			return itemAdded;
		}

		public void Clear() {
			this._hashTable.Clear();
			this._hasNullItem = false;
		}

		public bool Contains(object o) {
			if (o == null)
				return this._hasNullItem;
			// unefficient implementation as ContainsValue() does a linear search
			// think about using the hashcode or something else to optimize the execution
			return this._hashTable.ContainsValue(o);
		}

		public bool ContainsAll(ICollection c) {
			foreach (object o in c) {
				if (!this.Contains(o)) {
					return false;
				}
			}
			return true;
		}

		public bool IsEmpty() {
			return this._hashTable.Count == 0;
		}

		public bool Remove(object objToRemove) {
			if (!this.Contains(objToRemove))
				return false;
			if (objToRemove == null) {
				this._hasNullItem = false;
				return true;
			}
			Hashtable newHashTable = new Hashtable(this.Count - 1, this._loadFactor);
			foreach (object o in this._hashTable.Values) {
				if (!objToRemove.Equals(o)) {
					newHashTable.Add(o.GetHashCode(), o);
				}
			}
			this._hashTable = newHashTable;
			return true;
		}

		public bool RemoveAll(ICollection c) {
			bool itemsDeleted = false;
			foreach (object objToRemove in c) {
				itemsDeleted |= this.Remove(objToRemove);
			}
			return itemsDeleted;
		}

		public bool RetainAll(ICollection c) {
			Hashtable newHashTable = new Hashtable(this._capacity, this._loadFactor);
			foreach (object o in c) {
				if (this.Contains(o)) {
					newHashTable.Add(o.GetHashCode(), o);
				}
			}
			bool setChanged = newHashTable.Count != this._hashTable.Count;
			if (setChanged)
				this._hashTable = newHashTable;
			return setChanged;
		}

		public object[] ToArray() {
			object[] array = new object[this.Count];
			this._hashTable.Values.CopyTo(array, 0);
			return array;
		}

        public Array ToArray(System.Type type){
			System.Array array = Array.CreateInstance(type, this.Count);
			this._hashTable.Values.CopyTo(array, 0);
			return array;
		}

		public void CopyTo(Array array, int index) {
			this._hashTable.Values.CopyTo(array, index);
		}

		public bool IsSynchronized {
			get { return this._hashTable.IsSynchronized; }
		}

		public object SyncRoot {
			get { return this._hashTable.SyncRoot; }
		}

		public int Count {
			get { return this._hashTable.Count + (this._hasNullItem ? 1 : 0); }
		}

		public IEnumerator GetEnumerator() {
			return this._hashTable.Values.GetEnumerator();
		}

	}
}
