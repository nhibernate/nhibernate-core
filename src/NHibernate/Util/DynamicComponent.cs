using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;

namespace NHibernate.Util
{
	[Serializable]
	public sealed class DynamicComponent : DynamicObject, 
		IDictionary, 
		IDictionary<string, object>, 
		ISerializable,
		IDeserializationCallback
	{
		private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

		public DynamicComponent()
		{
		}

		private DynamicComponent(SerializationInfo info, StreamingContext context)
		{
			_data = info.GetValue<Dictionary<string, object>>("Data");
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return _data.TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			_data[binder.Name] = value;
			return true;
		}

		public override bool TryDeleteMember(DeleteMemberBinder binder)
		{
			return _data.Remove(binder.Name);
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return _data.Keys;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			if (indexes.Length == 1 && indexes[0] is string key)
				return _data.TryGetValue(key, out result);
			return base.TryGetIndex(binder, indexes, out result);
		}
		
		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			if (indexes.Length == 1 && indexes[0] is string key)
			{
				_data[key] = value;
				return true;
			}
			return base.TrySetIndex(binder, indexes, value);
		}

		public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
		{
			if (indexes.Length == 1 && indexes[0] is string key)
				return _data.Remove(key);

			return base.TryDeleteIndex(binder, indexes);
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			if (_data.TryGetValue(binder.Name, out var possibleDeligate) && possibleDeligate is Delegate @delegate)
			{
				result = @delegate.DynamicInvoke(args);
				return true;
			}

			return base.TryInvokeMember(binder, args, out result);
		}

		void IDictionary.Add(object key, object value)
		{
			((IDictionary) _data).Add(key, value);
		}

		void IDictionary.Clear()
		{
			((IDictionary) _data).Clear();
		}

		bool IDictionary.Contains(object key)
		{
			return ((IDictionary) _data).Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary) _data).GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			((IDictionary) _data).Remove(key);
		}

		bool IDictionary.IsFixedSize => ((IDictionary) _data).IsFixedSize;

		bool IDictionary.IsReadOnly => ((IDictionary) _data).IsReadOnly;

		object IDictionary.this[object key]
		{
			get => ((IDictionary) _data)[key];
			set => ((IDictionary) _data)[key] = value;
		}

		ICollection IDictionary.Keys => ((IDictionary) _data).Keys;

		ICollection IDictionary.Values => ((IDictionary) _data).Values;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _data).GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection) _data).CopyTo(array, index);
		}

		int ICollection.Count => ((ICollection) _data).Count;

		bool ICollection.IsSynchronized => ((ICollection) _data).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection) _data).SyncRoot;

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			((ICollection<KeyValuePair<string, object>>) _data).Add(item);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			((ICollection<KeyValuePair<string, object>>) _data).Clear();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>) _data).Contains(item);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>) _data).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>) _data).Remove(item);
		}

		int ICollection<KeyValuePair<string, object>>.Count =>
			((ICollection<KeyValuePair<string, object>>) _data).Count;

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly =>
			((ICollection<KeyValuePair<string, object>>) _data).IsReadOnly;

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>) _data).GetEnumerator();
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			((IDictionary<string, object>) _data).Add(key, value);
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return ((IDictionary<string, object>) _data).ContainsKey(key);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			return ((IDictionary<string, object>) _data).Remove(key);
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			return ((IDictionary<string, object>) _data).TryGetValue(key, out value);
		}

		object IDictionary<string, object>.this[string key]
		{
			get => ((IDictionary<string, object>) _data)[key];
			set => ((IDictionary<string, object>) _data)[key] = value;
		}

		ICollection<object> IDictionary<string, object>.Values => ((IDictionary<string, object>) _data).Values;

		ICollection<string> IDictionary<string, object>.Keys => ((IDictionary<string, object>) _data).Keys;

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Data", _data);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			_data.OnDeserialization(sender);
		}
	}
}
