using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// A persistent wrapper for a <see cref="IDictionary{TKey,TValue}"/>.  Underlying
	/// collection is a <see cref="Dictionary{TKey,TValue}"/>
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the IDictionary.</typeparam>
	/// <typeparam name="TValue">The type of the elements in the IDictionary.</typeparam>
	[Serializable]
	[DebuggerTypeProxy(typeof(DictionaryProxy<,>))]
	public class PersistentGenericMap<TKey, TValue> : PersistentMap, IDictionary<TKey, TValue>
	{
		// TODO NH: find a way to writeonce (no duplicated code from PersistentMap)
		protected IDictionary<TKey, TValue> gmap;
		public PersistentGenericMap() { }
		public PersistentGenericMap(ISessionImplementor session) : base(session) { }

		public PersistentGenericMap(ISessionImplementor session, IDictionary<TKey, TValue> map)
			: base(session, map as IDictionary)
		{
			gmap = map;
		}

		public override ICollection GetSnapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;
			Dictionary<TKey, TValue> clonedMap = new Dictionary<TKey, TValue>(map.Count);
			foreach (KeyValuePair<TKey, TValue> e in gmap)
			{
				object copy = persister.ElementType.DeepCopy(e.Value, entityMode, persister.Factory);
				clonedMap[e.Key] = (TValue)copy;
			}
			return clonedMap;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			gmap = (IDictionary<TKey, TValue>)persister.CollectionType.Instantiate(anticipatedSize);
			map = (IDictionary)gmap;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IList deletes = new List<object>();
			IDictionary<TKey, TValue> sn = (IDictionary<TKey, TValue>)GetSnapshot();
			foreach (KeyValuePair<TKey, TValue> e in sn)
			{
				if (!gmap.ContainsKey(e.Key))
				{
					object key = e.Key;
					deletes.Add(indexIsFormula ? e.Value : key);
				}
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IDictionary sn = (IDictionary)GetSnapshot();
			KeyValuePair<TKey, TValue> e = (KeyValuePair<TKey, TValue>)entry;
			return !sn.Contains(e.Key);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			IDictionary sn = (IDictionary)GetSnapshot();
			KeyValuePair<TKey, TValue> e = (KeyValuePair<TKey, TValue>)entry;
			object snValue = sn[e.Key];
			bool isNew = !sn.Contains(e.Key);
			return e.Value != null && snValue != null && elemType.IsDirty(snValue, e.Value, Session)
				|| (!isNew && ((e.Value == null) != (snValue == null)));
		}

		public override object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			return ((KeyValuePair<TKey, TValue>)entry).Key;
		}

		public override object GetElement(object entry)
		{
			return ((KeyValuePair<TKey, TValue>)entry).Value;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			IDictionary sn = (IDictionary)GetSnapshot();
			return sn[((KeyValuePair<TKey, TValue>)entry).Key];
		}

		public override bool EntryExists(object entry, int i)
		{
			return gmap.ContainsKey(((KeyValuePair<TKey, TValue>)entry).Key);
		}

		protected override void AddDuringInitialize(object index, object element)
		{
			gmap[(TKey)index] = (TValue)element;
		}

		#region IDictionary<TKey,TValue> Members

		bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
		{
			bool? exists = ReadIndexExistence(key);
			return !exists.HasValue ? gmap.ContainsKey(key) : exists.Value;
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (PutQueueEnabled)
			{
				object old = ReadElementByIndex(key);
				if (old != Unknown)
				{
					QueueOperation(new PutDelayedOperation(this, key, value, old));
					return;
				}
			}
			Initialize(true);
			gmap.Add(key, value);
			Dirty();
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			if (PutQueueEnabled)
			{
				object old = ReadElementByIndex(key);
				QueueOperation(new RemoveDelayedOperation(this, key, old));
				return true;
			}
			else
			{
				Initialize(true);
				bool contained = gmap.Remove(key);
				if (contained)
				{
					Dirty();
				}
				return contained;
			}
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			object result = ReadElementByIndex(key);
			if (result == Unknown)
			{
				return gmap.TryGetValue(key, out value);
			}
			else
			{
				value = (TValue)result;
				return true;
			}
		}

		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get
			{
				object result = ReadElementByIndex(key);
				return result == Unknown ? gmap[key] : (TValue)result;
			}
			set
			{
				// NH Note: the assignment in NET work like the put method in JAVA (mean assign or add)
				if (PutQueueEnabled)
				{
					object old = ReadElementByIndex(key);
					if (old != Unknown)
					{
						QueueOperation(new PutDelayedOperation(this, key, value, old));
						return;
					}
				}
				Initialize(true);
				TValue tempObject;
				gmap.TryGetValue(key, out tempObject);
				gmap[key] = value;
				TValue old2 = tempObject;
				// would be better to use the element-type to determine
				// whether the old and the new are equal here; the problem being
				// we do not necessarily have access to the element type in all
				// cases
				if (!ReferenceEquals(value, old2))
				{
					Dirty();
				}
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				Read();
				return gmap.Keys;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				Read();
				return gmap.Values;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			bool? exists = ReadIndexExistence(item.Key);
			if (!exists.HasValue)
			{
				return gmap.Contains(item);
			}
			else
			{
				if (exists.Value)
				{
					TValue x = ((IDictionary<TKey, TValue>)this)[item.Key];
					TValue y = item.Value;
					return EqualityComparer<TValue>.Default.Equals(x, y);
				}
				else
				{
					return false;
				}
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			int c = Count;
			TKey[] keys = new TKey[c];
			TValue[] values = new TValue[c];
			if (Keys != null)
			{
				Keys.CopyTo(keys, arrayIndex);
			}
			if (Values != null)
			{
				Values.CopyTo(values, arrayIndex);
			}
			for (int i = arrayIndex; i < c; i++)
			{
				if (keys[i] != null || values[i] != null)
				{
					array.SetValue(new KeyValuePair<TKey, TValue>(keys[i], values[i]), i);
				}
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item))
			{
				Remove(item.Key);
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			Read();
			return gmap.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return gmap.GetEnumerator();
		}

		#endregion
	}
}