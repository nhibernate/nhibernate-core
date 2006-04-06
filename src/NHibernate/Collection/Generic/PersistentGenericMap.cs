//#if NET_2_0

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Persister.Collection;
using NHibernate.Loader;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// A persistent wrapper for a <see cref="IDictionary&lt;TKey,TValue&gt;"/>.  Underlying
	/// collection is a <see cref="Dictionary&lt;TKey,TValue&gt;"/>
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the IDictionary.</typeparam>
	/// <typeparam name="TValue">The type of the elements in the IDictionary.</typeparam>
	[Serializable]
	public class PersistentGenericMap<TKey, TValue> : PersistentCollection, IDictionary<TKey, TValue>, System.Collections.IDictionary
	{
		protected IDictionary<TKey, TValue> map;

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericMap&lt;TKey,TValue&gt;"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the map is in.</param>
		internal PersistentGenericMap(ISessionImplementor session)
			: base(session)
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericMap&lt;TKey,TValue&gt;"/>
		/// that wraps an existing <see cref="IDictionary&lt;TKey,TValue&gt;"/> in the 
		/// <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
		/// <param name="map">The <see cref="IDictionary&lt;TKey,TValue&gt;"/> to wrap.</param>
		internal PersistentGenericMap(ISessionImplementor session, IDictionary<TKey, TValue> map)
			: base(session)
		{
			this.map = map;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		#region PersistentCollection Members

		public override bool Empty
		{
			get { return map.Count==0; }
		}

		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			BeforeInitialize(persister);
			object[] array = (object[])disassembled;

			for (int i = 0; i < array.Length; i += 2)
			{
				TKey index = (TKey)persister.IndexType.Assemble(array[i], Session, owner);
				object element = persister.ElementType.Assemble(array[i + 1], Session, owner);
				map[ index ] = (element==null ? default(TValue) : (TValue)element);
			}

			SetInitialized();
		}

		public override System.Collections.IEnumerable Entries()
		{
			return map as System.Collections.IEnumerable;
		}

        public override object ReadFrom(IDataReader reader, ICollectionPersister persister, ICollectionAliases descriptor, object owner)
		{
			// this really negates the value of generics - need to get the IPersistentCollection and
			// ICollectionPersister to be generic versions themselves to get the benefits of generics
			object element = persister.ReadElement(reader, owner, descriptor.SuffixedElementAliases, Session);
			object index = persister.ReadIndex(reader, descriptor.SuffixedIndexAliases, Session);

			TValue typedElement = (element!=null ? (TValue)element : default(TValue));
			TKey typedIndex = (index!=null ? (TKey)index : default(TKey));

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
			return ((KeyValuePair<TKey, TValue>)entry).Key;
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{

			if (persister.HasOrdering)
			{
				// TODO: find an implementation of IDictionary<TKey,TValue> that maintains
				// the order the items were inserted
				throw new NotImplementedException("have not coded the IDictionary<TKey,TValue> that maintains order");
				//this.map = new ListDictionary();
			}
			else
			{
				this.map = new Dictionary<TKey, TValue>();
			}
		}

		public override bool EqualsSnapshot(NHibernate.Type.IType elementType)
		{
			IDictionary<TKey, TValue> xmap = (IDictionary<TKey, TValue>)GetSnapshot();
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

		protected override System.Collections.ICollection Snapshot(ICollectionPersister persister)
		{
			Dictionary<TKey, TValue> clonedMap = new Dictionary<TKey, TValue>(map.Count);
			foreach (KeyValuePair<TKey, TValue> e in map)
			{
				object copy = persister.ElementType.DeepCopy(e.Value);
				clonedMap[e.Key] = (copy==null ? default(TValue) : (TValue)copy);
			}
			return clonedMap;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[map.Count * 2];
			int i = 0;
			foreach (KeyValuePair<TKey, TValue> e in map)
			{
				result[i++] = persister.IndexType.Disassemble(e.Key, Session);
				result[i++] = persister.ElementType.Disassemble(e.Value, Session);
			}

			return result;
		}

		public override bool EntryExists(object entry, int i)
		{
			return ((KeyValuePair<TKey, TValue>)entry).Value != null;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IDictionary<TKey, TValue> sn = (IDictionary<TKey, TValue>)GetSnapshot();
			KeyValuePair<TKey, TValue> e = (KeyValuePair<TKey, TValue>)entry;
			return ( e.Value!=null && sn.ContainsKey(e.Key)==false );
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			IDictionary<TKey, TValue> sn = (IDictionary<TKey, TValue>)GetSnapshot();
			KeyValuePair<TKey, TValue> e = (KeyValuePair<TKey, TValue>)entry;
			TValue snValue;
			sn.TryGetValue(e.Key, out snValue);
			return (e.Value != null && snValue != null && elemType.IsDirty(snValue, e.Value, Session));
		}

		public override System.Collections.ICollection GetDeletes(IType elemType, bool indexIsFormula)
		{
            System.Collections.Generic.IList<TKey> deletes = new System.Collections.Generic.List<TKey>();
			foreach (KeyValuePair<TKey, TValue> e in (IDictionary<TKey, TValue>)GetSnapshot())
			{
				TKey key = e.Key;
				if ( e.Value != null && map.ContainsKey(key)==false )
				{
					deletes.Add(key);
				}
			}
			return (System.Collections.ICollection)deletes;
		}

		public override bool IsWrapper(object collection)
		{
			return map == collection;
		}

		public override System.Collections.ICollection GetOrphans(object snapshot)
		{
			IDictionary<TKey, TValue> sn = (IDictionary<TKey, TValue>)snapshot;
            System.Collections.Generic.List<TValue> result = new System.Collections.Generic.List<TValue>(sn.Values.Count);
			result.AddRange(sn.Values);
			PersistentCollection.IdentityRemoveAll(result, (System.Collections.ICollection)map.Values, Session);
			return result;
		}
        public override object GetElement(object entry)
        {
            return entry;
        }

        public override object GetSnapshotElement(object entry, int i)
        {
            IList<TValue> sn = (IList<TValue>)GetSnapshot();
            return sn[i];
        }

		#endregion

		#region IDictionary<TKey,TValue> Members

		public void Add(TKey key, TValue value)
		{
			Write();
			map.Add(key, value);
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
			Write();
			return map.Remove(key);
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
			Write();
			map.Add(item);
		}

		public void Clear()
		{
			Write();
			map.Clear();
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
			Write();
			return map.Remove(item);
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>>  IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator()
		{
			Read();
			return map.GetEnumerator();
		}

		#endregion

		#region IDictionary Members

		void System.Collections.IDictionary.Add(object key, object value)
		{
			Write();
			((System.Collections.IDictionary)map).Add(key, value);
		}

		void System.Collections.IDictionary.Clear()
		{
			Write();
			map.Clear();
		}

		bool System.Collections.IDictionary.Contains(object key)
		{
			Read();
			return ((System.Collections.IDictionary)map).Contains(key);
		}

		System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
		{
			Read();
			return ((System.Collections.IDictionary)map).GetEnumerator();
		}

		bool System.Collections.IDictionary.IsFixedSize
		{
			get 
			{
				Read();
				return ((System.Collections.IDictionary)map).IsFixedSize;
			}
		}

		bool System.Collections.IDictionary.IsReadOnly
		{
			get { return false; }
		}

		System.Collections.ICollection System.Collections.IDictionary.Keys
		{
			get 
			{
				Read();
				return ((System.Collections.IDictionary)map).Keys;
			}
		}

		void System.Collections.IDictionary.Remove(object key)
		{
			Write();
			((System.Collections.IDictionary)map).Remove(key);
		}

		System.Collections.ICollection System.Collections.IDictionary.Values
		{
			get 
			{
				Read();
				return ((System.Collections.IDictionary)map).Values;
			}
		}

		object System.Collections.IDictionary.this[object key]
		{
			get
			{
				Read();
				return ((System.Collections.IDictionary)map)[key];
			}
			set
			{
				Write();
				((System.Collections.IDictionary)map)[key] = value;
			}
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			Read();
			((System.Collections.IDictionary)map).CopyTo(array, index);
		}

		int System.Collections.ICollection.Count
		{
			get 
			{
				Read();
				return map.Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get { return false; }
		}

		object System.Collections.ICollection.SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			Read();
			return ((System.Collections.IEnumerable)map).GetEnumerator();
		}

		#endregion
	}
}
//#endif