using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
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
	public class PersistentGenericMap<TKey, TValue> : AbstractPersistentCollection, IDictionary<TKey, TValue>, IDictionary
	{
		protected IDictionary<TKey, TValue> map;

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericMap&lt;TKey,TValue&gt;"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the map is in.</param>
		public PersistentGenericMap(ISessionImplementor session) : base(session)
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericMap&lt;TKey,TValue&gt;"/>
		/// that wraps an existing <see cref="IDictionary&lt;TKey,TValue&gt;"/> in the 
		/// <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
		/// <param name="map">The <see cref="IDictionary&lt;TKey,TValue&gt;"/> to wrap.</param>
		public PersistentGenericMap(ISessionImplementor session, IDictionary<TKey, TValue> map)
			: base(session)
		{
			this.map = map;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		#region AbstractPersistentCollection Members

		public override bool Empty
		{
			get { return map.Count == 0; }
		}

		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;

			for (int i = 0; i < array.Length; i += 2)
			{
				TKey index = (TKey) persister.IndexType.Assemble(array[i], Session, owner);
				object element = persister.ElementType.Assemble(array[i + 1], Session, owner);
				map[index] = (element == null ? default(TValue) : (TValue) element);
			}

			SetInitialized();
		}

		public override IEnumerable Entries()
		{
			return map;
		}

		public override object ReadFrom(IDataReader reader, ICollectionPersister persister, ICollectionAliases descriptor,
		                                object owner)
		{
			// this really negates the value of generics - need to get the IPersistentCollection and
			// ICollectionPersister to be generic versions themselves to get the benefits of generics
			object element = persister.ReadElement(reader, owner, descriptor.SuffixedElementAliases, Session);
			object index = persister.ReadIndex(reader, descriptor.SuffixedIndexAliases, Session);

			TValue typedElement = (element != null ? (TValue) element : default(TValue));
			TKey typedIndex = (index != null ? (TKey) index : default(TKey));

			map[typedIndex] = typedElement;
			return typedElement;
		}

		//		public override void WriteTo(IDbCommand st, ICollectionPersister persister, object entry, int i, bool writeOrder)
		//		{
		//			KeyValuePair<TKey, TValue> e = (KeyValuePair<TKey, TValue>)entry;
		//			persister.WriteElement(st, e.Value, writeOrder, this.Session);
		//			persister.WriteIndex(st, e.Key, writeOrder, this.Session);
		//		}

		public override object GetIndex(object entry, int i)
		{
			return ((KeyValuePair<TKey, TValue>) entry).Key;
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
			this.map = (IDictionary<TKey, TValue>) persister.CollectionType.Instantiate(-1);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IDictionary<TKey, TValue> xmap = (IDictionary<TKey, TValue>) GetSnapshot();
			if (xmap.Count != this.map.Count)
			{
				return false;
			}
			foreach (KeyValuePair<TKey, TValue> entry in map)
			{
				if (elementType.IsDirty(entry.Value, xmap[entry.Key], Session))
				{
					return false;
				}
			}
			return true;
		}

		protected override ICollection Snapshot(ICollectionPersister persister)
		{
			Dictionary<TKey, TValue> clonedMap = new Dictionary<TKey, TValue>(map.Count);
			foreach (KeyValuePair<TKey, TValue> e in map)
			{
				object copy = persister.ElementType.DeepCopy(e.Value, EntityMode.Poco, Session.Factory);
				clonedMap[e.Key] = (copy == null ? default(TValue) : (TValue) copy);
			}
			return clonedMap;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[map.Count * 2];
			int i = 0;
			foreach (KeyValuePair<TKey, TValue> e in map)
			{
				result[i++] = persister.IndexType.Disassemble(e.Key, Session, null);
				result[i++] = persister.ElementType.Disassemble(e.Value, Session, null);
			}

			return result;
		}

		public override bool EntryExists(object entry, int i)
		{
			return ((KeyValuePair<TKey, TValue>) entry).Value != null;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IDictionary<TKey, TValue> sn = (IDictionary<TKey, TValue>) GetSnapshot();
			KeyValuePair<TKey, TValue> e = (KeyValuePair<TKey, TValue>) entry;
			return (e.Value != null && sn.ContainsKey(e.Key) == false);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			IDictionary<TKey, TValue> sn = (IDictionary<TKey, TValue>) GetSnapshot();
			KeyValuePair<TKey, TValue> e = (KeyValuePair<TKey, TValue>) entry;
			TValue snValue;
			bool existedBefore = sn.TryGetValue(e.Key, out snValue);
			return (e.Value != null && existedBefore && elemType.IsDirty(snValue, e.Value, Session));
		}

		public override IEnumerable GetDeletes(IType elemType, bool indexIsFormula)
		{
			IList deletes = new ArrayList();
			foreach (KeyValuePair<TKey, TValue> e in (IDictionary<TKey, TValue>) GetSnapshot())
			{
				TKey key = e.Key;
				if (e.Value != null && !map.ContainsKey(key))
				{
					deletes.Add(indexIsFormula ? (object) e.Value : key);
				}
			}
			return deletes;
		}

		public override bool IsWrapper(object collection)
		{
			return map == collection;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IDictionary<TKey, TValue> sn = (IDictionary<TKey, TValue>) snapshot;
			List<TValue> result = new List<TValue>(sn.Values.Count);
			result.AddRange(sn.Values);
			IdentityRemoveAll(result, (ICollection) map.Values, entityName, Session);
			return result;
		}

		public override object GetElement(object entry)
		{
			return ((KeyValuePair<TKey, TValue>) entry).Value;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			// The snapshot is a Dictionary<TKey, TValue> which is an IDictionary as well.
			// IDictionary is used here so that the indexer returns null if the key
			// is not found, instead of throwing an exception.
			IDictionary sn = (IDictionary) GetSnapshot();
			TKey key = ((KeyValuePair<TKey, TValue>) entry).Key;
			return sn[key];
		}

		#endregion

		#region IDictionary<TKey,TValue> Members

		public void Add(TKey key, TValue value)
		{
			Initialize(true);
			map.Add(key, value);
			Dirty();
		}

		public bool ContainsKey(TKey key)
		{
			Read();
			return map.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get
			{
				Read();
				return map.Keys;
			}
		}

		public bool Remove(TKey key)
		{
			Initialize(true);
			return MakeDirtyIfTrue(map.Remove(key));
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			Read();
			return map.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get
			{
				Read();
				return map.Values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				Read();
				return map[key];
			}
			set
			{
				Write();
				map[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Initialize(true);
			map.Add(item);
			Dirty();
		}

		public void Clear()
		{
			Initialize(true);
			if (map.Count > 0)
			{
				map.Clear();
				Dirty();
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			Read();
			return map.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			Read();
			map.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				Read();
				return map.Count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			Initialize(true);
			return MakeDirtyIfTrue(map.Remove(item));
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			Read();
			return map.GetEnumerator();
		}

		#endregion

		#region IDictionary Members

		void IDictionary.Add(object key, object value)
		{
			Initialize(true);
			((IDictionary) map).Add(key, value);
			Dirty();
		}

		void IDictionary.Clear()
		{
			Initialize(true);
			if (map.Count > 0)
			{
				map.Clear();
				Dirty();
			}
		}

		bool IDictionary.Contains(object key)
		{
			Read();
			return ((IDictionary) map).Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			Read();
			return ((IDictionary) map).GetEnumerator();
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				Read();
				return ((IDictionary) map).IsFixedSize;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get { return false; }
		}

		ICollection IDictionary.Keys
		{
			get
			{
				Read();
				return ((IDictionary) map).Keys;
			}
		}

		void IDictionary.Remove(object key)
		{
			Initialize(true);
			int oldCount = map.Count;
			((IDictionary) map).Remove(key);
			if (oldCount != map.Count)
			{
				Dirty();
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				Read();
				return ((IDictionary) map).Values;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				Read();
				return ((IDictionary) map)[key];
			}
			set
			{
				Write();
				((IDictionary) map)[key] = value;
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			Read();
			((IDictionary) map).CopyTo(array, index);
		}

		int ICollection.Count
		{
			get
			{
				Read();
				return map.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return ((IEnumerable) map).GetEnumerator();
		}

		#endregion

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return map;
		}
	}
}
