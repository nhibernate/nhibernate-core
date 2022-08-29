using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace NHibernate.Proxy.Map
{
	/// <summary> Proxy for "dynamic-map" entity representations. </summary>
	[Serializable]
	public class MapProxy : DynamicObject, INHibernateProxy, IDictionary, IDictionary<string, object>
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

		#region DynamecObject overrides

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return li.GenericMap.TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			li.GenericMap[binder.Name] = value;
			return true;
		}

		public override bool TryDeleteMember(DeleteMemberBinder binder)
		{
			return li.GenericMap.Remove(binder.Name);
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return li.GenericMap.Keys;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			if (indexes.Length == 1 && indexes[0] is string key)
				return li.GenericMap.TryGetValue(key, out result);
			return base.TryGetIndex(binder, indexes, out result);
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			if (indexes.Length == 1 && indexes[0] is string key)
			{
				li.GenericMap[key] = value;
				return true;
			}
			return base.TrySetIndex(binder, indexes, value);
		}

		public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
		{
			if (indexes.Length == 1 && indexes[0] is string key)
				return li.GenericMap.Remove(key);

			return base.TryDeleteIndex(binder, indexes);
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			if (li.GenericMap.TryGetValue(binder.Name, out var possibleDeligate) && possibleDeligate is Delegate @delegate)
			{
				result = @delegate.DynamicInvoke(args);
				return true;
			}

			return base.TryInvokeMember(binder, args, out result);
		}

		#endregion

		#region IDictionary Members

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
			set { li.Map[key] = value; }
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

		#region IDictionary<string, object> Members

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return li.GenericMap.ContainsKey(key);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			li.GenericMap.Add(key, value);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			return li.GenericMap.Remove(key);
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			return li.GenericMap.TryGetValue(key, out value);
		}

		object IDictionary<string, object>.this[string key]
		{
			get => li.GenericMap[key];
			set => li.GenericMap[key] = value;
		}

		ICollection<object> IDictionary<string, object>.Values => li.GenericMap.Values;

		ICollection<string> IDictionary<string, object>.Keys => li.GenericMap.Keys;

		#endregion

		#region ICollection<KeyValuePair<string, object>> Members

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			li.GenericMap.Add(item);
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return li.GenericMap.Contains(item);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			li.GenericMap.CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			return li.GenericMap.Remove(item);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string, object>> Members

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return li.GenericMap.GetEnumerator();
		}

		#endregion
	}
}
