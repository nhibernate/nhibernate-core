using System;
using System.Collections;

namespace NHibernate.Util {

	/// <summary>
	/// An <c>IDictionary</c> where keys are compared by object identity, rather than <c>equals</c>.
	/// </summary>
	/// <remarks>
	/// For now, we are just using a hashtable
	/// </remarks>
	[Serializable]
	public sealed class IdentityMap : IDictionary {
		Hashtable map = new Hashtable();

		public static IDictionary Instantiate() {
			return new IdentityMap();
		}

		public int Count {
			get { return map.Count; }
		}

		public bool IsSynchronized {
			get { return map.IsSynchronized; }
		}

		public object SyncRoot {
			get { return map.SyncRoot; }
		}

		public void Add(object key, object val) {
			map.Add(key, val);
		}

		public void Clear() {
			map.Clear();
		}

		public bool Contains(object key) {
			return map.Contains(key);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return map.GetEnumerator();
		}

		public IDictionaryEnumerator GetEnumerator() {
			return map.GetEnumerator();
		}

		public bool IsFixedSize {
			get { return map.IsFixedSize; }
		}

		public bool IsReadOnly {
			get { return map.IsReadOnly; }
		}

		public ICollection Keys {
			get { return map.Keys; }
		}

		public void Remove(object key) {
			map.Remove(key);
		}

		public object this [object key] {
			get { return map[key]; }
			set { map[key] = value; }
		}

		public ICollection Values {
			get { return map.Values; }
		}

		public void CopyTo(Array array, int i) {
			map.CopyTo(array, i);
		}
		

		
	}
}
