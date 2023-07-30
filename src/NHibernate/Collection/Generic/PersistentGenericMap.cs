using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Collection.Trackers;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;

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
	public partial class PersistentGenericMap<TKey, TValue> : AbstractPersistentCollection, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ICollection
	{
		protected IDictionary<TKey, TValue> WrappedMap;
		private readonly ICollection<TValue> _wrappedValues;

		public PersistentGenericMap()
		{
			_wrappedValues = new ValuesWrapper(this);
		}

		/// <summary>
		/// Construct an uninitialized PersistentGenericMap.
		/// </summary>
		/// <param name="session">The ISession the PersistentGenericMap should be a part of.</param>
		public PersistentGenericMap(ISessionImplementor session) : base(session)
		{
			_wrappedValues = new ValuesWrapper(this);
		}

		/// <summary>
		/// Construct an initialized PersistentGenericMap based off the values from the existing IDictionary.
		/// </summary>
		/// <param name="session">The ISession the PersistentGenericMap should be a part of.</param>
		/// <param name="map">The IDictionary that contains the initial values.</param>
		public PersistentGenericMap(ISessionImplementor session, IDictionary<TKey, TValue> map)
			: base(session)
		{
			WrappedMap = map;
			_wrappedValues = new ValuesWrapper(this);
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		internal override AbstractQueueOperationTracker CreateQueueOperationTracker()
		{
			var entry = Session.PersistenceContext.GetCollectionEntry(this);
			return new MapQueueOperationTracker<TKey, TValue>(entry.LoadedPersister);
		}

		public override object GetSnapshot(ICollectionPersister persister)
		{
			Dictionary<TKey, TValue> clonedMap = new Dictionary<TKey, TValue>(WrappedMap.Count);
			foreach (KeyValuePair<TKey, TValue> e in WrappedMap)
			{
				object copy = persister.ElementType.DeepCopy(e.Value, persister.Factory);
				clonedMap[e.Key] = (TValue)copy;
			}
			return clonedMap;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			var sn = (IDictionary<TKey, TValue>) snapshot;
			return GetOrphans((ICollection)sn.Values, (ICollection)WrappedMap.Values, entityName, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			var xmap = (IDictionary<TKey, TValue>)GetSnapshot();
			if (xmap.Count != WrappedMap.Count)
			{
				return false;
			}
			foreach (KeyValuePair<TKey, TValue> entry in WrappedMap)
			{
				// This method is not currently called if a key has been removed/added, but better be on the safe side.
				if (!xmap.TryGetValue(entry.Key, out var value) ||
					elementType.IsDirty(value, entry.Value, Session))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((IDictionary)snapshot).Count == 0;
		}

		public override bool IsWrapper(object collection)
		{
			return WrappedMap == collection;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			WrappedMap = (IDictionary<TKey, TValue>)persister.CollectionType.Instantiate(anticipatedSize);
		}

		public override void ApplyQueuedOperations()
		{
			var queueOperation = (AbstractMapQueueOperationTracker<TKey, TValue>) QueueOperationTracker;
			queueOperation?.ApplyChanges(WrappedMap);
			QueueOperationTracker = null;
		}

		public override bool Empty
		{
			get { return (WrappedMap.Count == 0); }
		}

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(WrappedMap);
		}

		public override object ReadFrom(DbDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			object element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			object index = role.ReadIndex(rs, descriptor.SuffixedIndexAliases, Session);

			AddDuringInitialize(index, element);
			return element;
		}

		protected virtual void AddDuringInitialize(object index, object element)
		{
			WrappedMap[(TKey)index] = (TValue)element;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return WrappedMap;
		}

		/// <summary>
		/// Initializes this PersistentGenericMap from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentGenericMap.</param>
		/// <param name="disassembled">The disassembled PersistentGenericMap.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] array = (object[])disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);

			var indexType = persister.IndexType;
			var elementType = persister.ElementType;
			for (int i = 0; i < size; i += 2)
			{
				indexType.BeforeAssemble(array[i], Session);
				elementType.BeforeAssemble(array[i + 1], Session);
			}

			for (int i = 0; i < size; i += 2)
			{
				WrappedMap[(TKey)indexType.Assemble(array[i], Session, owner)] =
					(TValue)elementType.Assemble(array[i + 1], Session, owner);
			}
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[WrappedMap.Count * 2];
			int i = 0;
			foreach (KeyValuePair<TKey, TValue> e in WrappedMap)
			{
				result[i++] = persister.IndexType.Disassemble(e.Key, Session, null);
				result[i++] = persister.ElementType.Disassemble(e.Value, Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IList deletes = new List<object>();
			var sn = (IDictionary<TKey, TValue>)GetSnapshot();
			foreach (var e in sn)
			{
				if (!WrappedMap.ContainsKey(e.Key))
				{
					object key = e.Key;
					deletes.Add(indexIsFormula ? e.Value : key);
				}
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			var sn = (IDictionary)GetSnapshot();
			var e = (KeyValuePair<TKey, TValue>)entry;
			return !sn.Contains(e.Key);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			var sn = (IDictionary)GetSnapshot();
			var e = (KeyValuePair<TKey, TValue>)entry;
			var snValue = sn[e.Key];
			var isNew = !sn.Contains(e.Key);
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
			var sn = (IDictionary)GetSnapshot();
			return sn[((KeyValuePair<TKey, TValue>)entry).Key];
		}

		public override bool EntryExists(object entry, int i)
		{
			return WrappedMap.ContainsKey(((KeyValuePair<TKey, TValue>)entry).Key);
		}

		#region IDictionary<TKey,TValue> Members

		public bool ContainsKey(TKey key)
		{
			return ReadKeyExistence<TKey, TValue>(key) ?? WrappedMap.ContainsKey(key);
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (PutQueueEnabled)
			{
				var found = TryReadElementByKey<TKey, TValue>(key, out _, out _);
				if (found.HasValue)
				{
					if (found.Value)
					{
						throw new ArgumentException("An item with the same key has already been added."); // Mimic dictionary behavior
					}

					QueueAddElementByKey(key, value);

					return;
				}
			}

			Initialize(true);
			WrappedMap.Add(key, value);
			Dirty();
		}

		public bool Remove(TKey key)
		{
			var oldValue = default(TValue);
			var existsInDb = default(bool?);
			var found = PutQueueEnabled ? TryReadElementByKey(key, out oldValue, out existsInDb) : null;
			if (!found.HasValue) // queue is not enabled for 'puts' or collection was initialized
			{
				Initialize(true);
				bool contained = WrappedMap.Remove(key);
				if (contained)
				{
					Dirty();
				}
				return contained;
			}

			return QueueRemoveElementByKey(key, oldValue, existsInDb);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			var found = TryReadElementByKey(key, out value, out _);
			if (!found.HasValue) // collection was initialized
			{
				return WrappedMap.TryGetValue(key, out value);
			}

			if (found.Value)
			{
				return true;
			}

			value = default(TValue);
			return false;
		}

		public TValue this[TKey key]
		{
			get
			{
				var found = TryReadElementByKey<TKey, TValue>(key, out var value, out _);
				if (!found.HasValue) // collection was initialized
				{
					return WrappedMap[key];
				}

				if (!found.Value)
				{
					throw new KeyNotFoundException();
				}

				return value;
			}
			set
			{
				// NH Note: the assignment in NET work like the put method in JAVA (mean assign or add)
				if (PutQueueEnabled)
				{
					var found = TryReadElementByKey<TKey, TValue>(key, out var oldValue, out var existsInDb);
					if (found.HasValue)
					{
						QueueSetElementByKey(key, value, oldValue, existsInDb);

						return;
					}
				}

				Initialize(true);
				if (!WrappedMap.TryGetValue(key, out var old))
				{
					WrappedMap.Add(key, value);
					Dirty();
				}
				else
				{
					WrappedMap[key] = value;
					if (!EqualityComparer<TValue>.Default.Equals(value, old))
					{
						Dirty();
					}
				}
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				Read();
				return WrappedMap.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return _wrappedValues;
			}
		}

		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
		{
			get { return Keys; }
		}

		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
		{
			get { return Values; }
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			if (ClearQueueEnabled)
			{
				QueueClearCollection();
			}
			else
			{
				Initialize(true);
				if (WrappedMap.Count != 0)
				{
					Dirty();
					WrappedMap.Clear();
				}
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			bool? exists = ReadKeyExistence<TKey, TValue>(item.Key);
			if (!exists.HasValue)
			{
				return WrappedMap.Contains(item);
			}

			if (exists.Value)
			{
				TValue x = ((IDictionary<TKey, TValue>)this)[item.Key];
				TValue y = item.Value;
				return EqualityComparer<TValue>.Default.Equals(x, y);
			}

			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			Read();
			WrappedMap.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item))
			{
				Remove(item.Key);
				return true;
			}

			return false;
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : WrappedMap.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int arrayIndex)
		{
			Read();
			if (WrappedMap is ICollection collection)
			{
				collection.CopyTo(array, arrayIndex);
			}
			else
			{
				foreach (var item in WrappedMap)
					array.SetValue(item, arrayIndex++);
			}
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			Read();
			return WrappedMap.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return WrappedMap.GetEnumerator();
		}

		#endregion

		#region DelayedOperations

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericMap<TKey, TValue> _enclosingInstance;

			public ClearDelayedOperation(PersistentGenericMap<TKey, TValue> enclosingInstance)
			{
				_enclosingInstance = enclosingInstance;
			}

			public object AddedInstance
			{
				get { return null; }
			}

			public object Orphan
			{
				get { throw new NotSupportedException("queued clear cannot be used with orphan delete"); }
			}

			public void Operate()
			{
				_enclosingInstance.WrappedMap.Clear();
			}
		}

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class PutDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericMap<TKey, TValue> _enclosingInstance;
			private readonly TKey _index;
			private readonly TValue _value;
			private readonly object _old;

			public PutDelayedOperation(PersistentGenericMap<TKey, TValue> enclosingInstance, TKey index, TValue value, object old)
			{
				_enclosingInstance = enclosingInstance;
				_index = index;
				_value = value;
				_old = old;
			}

			public object AddedInstance
			{
				get { return _value; }
			}

			public object Orphan
			{
				get { return _old; }
			}

			public void Operate()
			{
				_enclosingInstance.WrappedMap[_index] = _value;
			}
		}

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class RemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericMap<TKey, TValue> _enclosingInstance;
			private readonly TKey _index;
			private readonly object _old;

			public RemoveDelayedOperation(PersistentGenericMap<TKey, TValue> enclosingInstance, TKey index, object old)
			{
				_enclosingInstance = enclosingInstance;
				_index = index;
				_old = old;
			}

			public object AddedInstance
			{
				get { return null; }
			}

			public object Orphan
			{
				get { return _old; }
			}

			public void Operate()
			{
				_enclosingInstance.WrappedMap.Remove(_index);
			}
		}

		#endregion

		[Serializable]
		private class ValuesWrapper : ICollection<TValue>, IQueryable<TValue>
		{
			private readonly PersistentGenericMap<TKey, TValue> _map;

			public ValuesWrapper(PersistentGenericMap<TKey, TValue> map)
			{
				_map = map;
			}

			#region IQueryable<TValue> Members

			[NonSerialized]
			private IQueryable<TValue> _queryable;

			Expression IQueryable.Expression => InnerQueryable.Expression;

			System.Type IQueryable.ElementType => InnerQueryable.ElementType;

			IQueryProvider IQueryable.Provider => InnerQueryable.Provider;

			private IQueryable<TValue> InnerQueryable => _queryable ?? (_queryable = new NhQueryable<TValue>(_map.Session, _map));

			#endregion

			#region ICollection<TValue> Members

			public IEnumerator<TValue> GetEnumerator()
			{
				_map.Read();
				return _map.WrappedMap.Values.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				_map.Read();
				return GetEnumerator();
			}

			public void Add(TValue item)
			{
				throw new NotSupportedException("Values collection is readonly");
			}

			public void Clear()
			{
				throw new NotSupportedException("Values collection is readonly");
			}

			public bool Contains(TValue item)
			{
				_map.Read();
				return _map.WrappedMap.Values.Contains(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				_map.Read();
				_map.WrappedMap.Values.CopyTo(array, arrayIndex);
			}

			public bool Remove(TValue item)
			{
				throw new NotSupportedException("Values collection is readonly");
			}

			public int Count
			{
				get
				{
					_map.Read();
					return _map.WrappedMap.Values.Count;
				}
			}

			public bool IsReadOnly => true;

			#endregion
		}
	}
}
