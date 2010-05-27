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
using NHibernate.Util;

namespace NHibernate.Collection
{
	/// <summary>
	/// A persistent wrapper for a <see cref="IDictionary" />. Underlying collection
	/// is a <see cref="Hashtable" />.
	/// </summary>
	[Serializable]
	[DebuggerTypeProxy(typeof (DictionaryProxy))]
	public class PersistentMap : AbstractPersistentCollection, IDictionary
	{
		protected IDictionary map;

		public PersistentMap() {}

		/// <summary>
		/// Construct an uninitialized PersistentMap.
		/// </summary>
		/// <param name="session">The ISession the PersistentMap should be a part of.</param>
		public PersistentMap(ISessionImplementor session) : base(session) {}

		/// <summary>
		/// Construct an initialized PersistentMap based off the values from the existing IDictionary.
		/// </summary>
		/// <param name="session">The ISession the PersistentMap should be a part of.</param>
		/// <param name="map">The IDictionary that contains the initial values.</param>
		public PersistentMap(ISessionImplementor session, IDictionary map) : base(session)
		{
			this.map = map;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override ICollection GetSnapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;
			Hashtable clonedMap = new Hashtable(map.Count);
			foreach (DictionaryEntry e in map)
			{
				object copy = persister.ElementType.DeepCopy(e.Value, entityMode, persister.Factory);
				clonedMap[e.Key] = copy;
			}
			return clonedMap;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IDictionary sn = (IDictionary) snapshot;
			return GetOrphans(sn.Values, map.Values, entityName, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IDictionary xmap = (IDictionary) GetSnapshot();
			if (xmap.Count != map.Count)
			{
				return false;
			}
			foreach (DictionaryEntry entry in map)
			{
				if (elementType.IsDirty(entry.Value, xmap[entry.Key], Session))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((IDictionary) snapshot).Count == 0;
		}

		public override bool IsWrapper(object collection)
		{
			return map == collection;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			map = (IDictionary) persister.CollectionType.Instantiate(anticipatedSize);
		}

		public override bool Empty
		{
			get { return (map.Count == 0); }
		}

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(map);
		}

		public override object ReadFrom(IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			object element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			object index = role.ReadIndex(rs, descriptor.SuffixedIndexAliases, Session);

			AddDuringInitialize(index, element);
			return element;
		}

		protected virtual void AddDuringInitialize(object index, object element)
		{
			map[index] = element;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return map;
		}

		/// <summary>
		/// Initializes this PersistentMap from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentMap.</param>
		/// <param name="disassembled">The disassembled PersistentMap.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] array = (object[]) disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i += 2)
			{
				map[persister.IndexType.Assemble(array[i], Session, owner)] =
					persister.ElementType.Assemble(array[i + 1], Session, owner);
			}
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[map.Count * 2];
			int i = 0;
			foreach (DictionaryEntry e in map)
			{
				result[i++] = persister.IndexType.Disassemble(e.Key, Session, null);
				result[i++] = persister.ElementType.Disassemble(e.Value, Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IList deletes = new List<object>();
			IDictionary sn = (IDictionary) GetSnapshot();
			foreach (DictionaryEntry e in sn)
			{
				object key = e.Key;
				if (!map.Contains(key))
				{
					deletes.Add(indexIsFormula ? e.Value : key);
				}
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IDictionary sn = (IDictionary) GetSnapshot();
			DictionaryEntry e = (DictionaryEntry) entry;
			return !sn.Contains(e.Key);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			IDictionary sn = (IDictionary) GetSnapshot();
			DictionaryEntry e = (DictionaryEntry) entry;
			object snValue = sn[e.Key];
			bool isNew = !sn.Contains(e.Key);
			return e.Value != null && snValue != null && elemType.IsDirty(snValue, e.Value, Session)
				|| (!isNew && ((e.Value == null) != (snValue == null)));
		}

		public override object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			return ((DictionaryEntry) entry).Key;
		}

		public override object GetElement(object entry)
		{
			return ((DictionaryEntry) entry).Value;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			IDictionary sn = (IDictionary) GetSnapshot();
			return sn[((DictionaryEntry) entry).Key];
		}

		public override bool Equals(object other)
		{
			IDictionary that = other as IDictionary;
			if (that == null)
			{
				return false;
			}
			Read();
			return CollectionHelper.DictionaryEquals(map, that);
		}

		public override int GetHashCode()
		{
			Read();
			return map.GetHashCode();
		}

		public override bool EntryExists(object entry, int i)
		{
			return map.Contains(((DictionaryEntry) entry).Key);
		}

		#region IDictionary Members

		public bool Contains(object key)
		{
			bool? exists = ReadIndexExistence(key);
			return !exists.HasValue ? map.Contains(key) : exists.Value;
		}

		public void Add(object key, object value)
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
			// NH Different behavior: we are using same NET behavior where Add is different than put method in JAVA
			map.Add(key, value);
			Dirty();
		}

		public void Clear()
		{
			if (ClearQueueEnabled)
			{
				QueueOperation(new ClearDelayedOperation(this));
			}
			else
			{
				Initialize(true);
				if (!(map.Count == 0))
				{
					Dirty();
					map.Clear();
				}
			}
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			Read();
			return map.GetEnumerator();
		}

		public void Remove(object key)
		{
			if (PutQueueEnabled)
			{
				object old = ReadElementByIndex(key);
				QueueOperation(new RemoveDelayedOperation(this, key, old));
				return;
			}
			else
			{
				// TODO : safe to interpret "map.remove(key) == null" as non-dirty?
				Initialize(true);
				// NH: Different implementation: we use the count to know if the value was removed (better performance)
				int contained = map.Count;
				map.Remove(key);
				if (contained != map.Count)
				{
					Dirty();
				}
			}
		}

		public object this[object key]
		{
			get
			{
				object result = ReadElementByIndex(key);
				return result == Unknown ? map[key] : result;
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
				object tempObject = map[key];
				map[key] = value;
				object old2 = tempObject;
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

		public ICollection Keys
		{
			get
			{
				Read();
				return map.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				Read();
				return map.Values;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			int c = Count;
			object[] keys = new object[c];
			object[] values = new object[c];
			if (Keys != null)
			{
				Keys.CopyTo(keys, index);
			}
			if (Values != null)
			{
				Values.CopyTo(values, index);
			}
			for (int i = index; i < c; i++)
			{
				if (keys[i] != null || values[i] != null)
				{
					array.SetValue(new DictionaryEntry(keys[i], values[i]), i);
				}
			}
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : map.Count; }
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

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			Read();
			return map.GetEnumerator();
		}

		#endregion

		#region DelayedOperations

		protected sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentMap enclosingInstance;

			public ClearDelayedOperation(PersistentMap enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
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
				enclosingInstance.map.Clear();
			}
		}

		protected sealed class PutDelayedOperation : IDelayedOperation
		{
			private readonly PersistentMap enclosingInstance;
			private readonly object index;
			private readonly object value;
			private readonly object old;

			public PutDelayedOperation(PersistentMap enclosingInstance, object index, object value, object old)
			{
				this.enclosingInstance = enclosingInstance;
				this.index = index;
				this.value = value;
				this.old = old;
			}

			public object AddedInstance
			{
				get { return value; }
			}

			public object Orphan
			{
				get { return old; }
			}

			public void Operate()
			{
				enclosingInstance.map[index] = value;
			}
		}

		protected sealed class RemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentMap enclosingInstance;
			private readonly object index;
			private readonly object old;

			public RemoveDelayedOperation(PersistentMap enclosingInstance, Object index, Object old)
			{
				this.enclosingInstance = enclosingInstance;
				this.index = index;
				this.old = old;
			}

			public object AddedInstance
			{
				get { return null; }
			}

			public object Orphan
			{
				get { return old; }
			}

			public void Operate()
			{
				enclosingInstance.map.Remove(index);
			}
		}

		#endregion
	}
}