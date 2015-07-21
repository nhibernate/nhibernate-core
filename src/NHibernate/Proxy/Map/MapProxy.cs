using System;
using System.Collections;

namespace NHibernate.Proxy.Map
{
	/// <summary> Proxy for "dynamic-map" entity representations. </summary>
	[Serializable]
	public class MapProxy : INHibernateProxy, IDictionary
	{
		private readonly MapLazyInitializer li;

		internal MapProxy(MapLazyInitializer li)
		{
			this.li = li;
		}

		public ILazyInitializer HibernateLazyInitializer
		{
			get { return li; }
		}

		public bool Contains(object key)
		{
			return li.Map.Contains(key);
		}

		public void Add(object key, object value)
		{
			li.Map.Add(key, value);
		}

		public void Clear()
		{
			li.Map.Clear();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return li.Map.GetEnumerator();
		}

		public void Remove(object key)
		{
			li.Map.Remove(key);
		}

		public object this[object key]
		{
			get { return li.Map[key]; }
			set { li.Map[key] = value;}
		}

		public ICollection Keys
		{
			get { return li.Map.Keys; }
		}

		public ICollection Values
		{
			get { return li.Map.Values; }
		}

		public bool IsReadOnly
		{
			get { return li.Map.IsReadOnly; }
		}

		#region IDictionary Members

		public bool IsFixedSize
		{
			get { return li.Map.IsFixedSize; }
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			object[] keys = new object[Count];
			object[] values = new object[Count];
			if (Keys != null)
				Keys.CopyTo(keys, index);
			if (Values != null)
				Values.CopyTo(values, index);
			for (int i = index; i < Count; i++)
				if (keys[i] != null || values[i] != null)
					array.SetValue(new DictionaryEntry(keys[i], values[i]), i);
		}

		public int Count
		{
			get { return li.Map.Count; }
		}

		public object SyncRoot
		{
			get { return li.Map.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return li.Map.IsSynchronized; }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return li.Map.GetEnumerator();
		}

		#endregion
	}
}
