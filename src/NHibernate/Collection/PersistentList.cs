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
	/// A persistent wrapper for an <see cref="IList"/>
	/// </summary>	
	/// <remarks>
	/// The underlying collection used in an <see cref="ArrayList"/>.
	/// </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy))]
	public class PersistentList : AbstractPersistentCollection, IList
	{
		protected IList list;

		protected virtual object DefaultForType
		{
			get { return null; }
		}

		public PersistentList() {}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentList"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		public PersistentList(ISessionImplementor session) : base(session) {}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentList"/>
		/// that wraps an existing <see cref="IList"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		/// <param name="list">The <see cref="IList"/> to wrap.</param>
		public PersistentList(ISessionImplementor session, IList list) : base(session)
		{
			this.list = list;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override ICollection GetSnapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;

			List<object> clonedList = new List<object>(list.Count);
			foreach (object current in list)
			{
				object deepCopy = persister.ElementType.DeepCopy(current, entityMode, persister.Factory);
				clonedList.Add(deepCopy);
			}
			return clonedList;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IList sn = (IList) snapshot;
			return GetOrphans(sn, list, entityName, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IList sn = (IList) GetSnapshot();
			if (sn.Count != list.Count)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (elementType.IsDirty(list[i], sn[i], Session))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((ICollection) snapshot).Count == 0;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			list = (IList) persister.CollectionType.Instantiate(anticipatedSize);
		}

		public override bool IsWrapper(object collection)
		{
			return list == collection;
		}

		public override bool Empty
		{
			get { return (list.Count == 0); }
		}

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(list);
		}

		public override object ReadFrom(IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			object element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			int index = (int) role.ReadIndex(rs, descriptor.SuffixedIndexAliases, Session);

			//pad with nulls from the current last element up to the new index
			for (int i = list.Count; i <= index; i++)
			{
				list.Insert(i, DefaultForType);
			}

			list[index] = element;
			return element;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return list;
		}

		/// <summary>
		/// Initializes this PersistentList from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentList.</param>
		/// <param name="disassembled">The disassembled PersistentList.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] array = (object[]) disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i++)
			{
				object element = persister.ElementType.Assemble(array[i], Session, owner);
				list.Add(element ?? DefaultForType);
			}
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			int length = list.Count;
			object[] result = new object[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = persister.ElementType.Disassemble(list[i], Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IList deletes = new List<object>();
			IList sn = (IList) GetSnapshot();
			int end;
			if (sn.Count > list.Count)
			{
				for (int i = list.Count; i < sn.Count; i++)
				{
					deletes.Add(indexIsFormula ? sn[i] : i);
				}
				end = list.Count;
			}
			else
			{
				end = sn.Count;
			}
			for (int i = 0; i < end; i++)
			{
				if (list[i] == null && sn[i] != null)
				{
					deletes.Add(indexIsFormula ? sn[i] : i);
				}
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IList sn = (IList) GetSnapshot();
			return list[i] != null && (i >= sn.Count || sn[i] == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			IList sn = (IList) GetSnapshot();
			return i < sn.Count && sn[i] != null && list[i] != null && elemType.IsDirty(list[i], sn[i], Session);
		}

		public override object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			return i;
		}

		public override object GetElement(object entry)
		{
			return entry;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			IList sn = (IList) GetSnapshot();
			return sn[i];
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		public override bool Equals(object obj)
		{
			ICollection that = obj as ICollection;
			if (that == null)
			{
				return false;
			}
			Read();
			return CollectionHelper.CollectionEquals(list, that);
		}

		public override int GetHashCode()
		{
			Read();
			return list.GetHashCode();
		}

		#region IList Members

		public int Add(object value)
		{
			if (!IsOperationQueueEnabled)
			{
				Write();
				return list.Add(value);
			}
			else
			{
				QueueOperation(new SimpleAddDelayedOperation(this, value));
				//TODO: take a look at this - I don't like it because it changes the 
				// meaning of Add - instead of returning the index it was added at 
				// returns a "fake" index - not consistent with IList interface...
				return -1;
			}
		}

		public bool Contains(object value)
		{
			bool? exists = ReadElementExistence(value);
			return !exists.HasValue ? list.Contains(value) : exists.Value;
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
				if (!(list.Count == 0))
				{
					list.Clear();
					Dirty();
				}
			}
		}

		public int IndexOf(object value)
		{
			Read();
			return list.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException("negative index");
			}
			if (!IsOperationQueueEnabled)
			{
				Write();
				list.Insert(index, value);
			}
			else
			{
				QueueOperation(new AddDelayedOperation(this, index, value));
			}
		}

		public void Remove(object value)
		{
			bool? exists = PutQueueEnabled ? ReadElementExistence(value) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				// NH: Different implementation: we use the count to know if the value was removed (better performance)
				int contained = list.Count;
				list.Remove(value);
				if (contained != list.Count)
				{
					Dirty();
				}
			}
			else if (exists.Value)
			{
				QueueOperation(new SimpleRemoveDelayedOperation(this, value));
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException("negative index");
			}
			object old = PutQueueEnabled ? ReadElementByIndex(index) : Unknown;
			if (old == Unknown)
			{
				Write();
				list.RemoveAt(index);
			}
			else
			{
				QueueOperation(new RemoveDelayedOperation(this, index, old));
			}
		}

		public object this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new IndexOutOfRangeException("negative index");
				}
				object result = ReadElementByIndex(index);
				return result == Unknown ? list[index] : result;
			}
			set
			{
				if (index < 0)
				{
					throw new IndexOutOfRangeException("negative index");
				}
				object old = PutQueueEnabled ? ReadElementByIndex(index) : Unknown;
				if (old == Unknown)
				{
					Write();
					list[index] = value;
				}
				else
				{
					QueueOperation(new SetDelayedOperation(this, index, value, old));
				}
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
			for (int i = index; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : list.Count; }
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
			return list.GetEnumerator();
		}

		#endregion

		#region DelayedOperations

		protected sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentList enclosingInstance;

			public ClearDelayedOperation(PersistentList enclosingInstance)
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
				enclosingInstance.list.Clear();
			}
		}

		protected sealed class SimpleAddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentList enclosingInstance;
			private readonly object value;

			public SimpleAddDelayedOperation(PersistentList enclosingInstance, object value)
			{
				this.enclosingInstance = enclosingInstance;
				this.value = value;
			}

			public object AddedInstance
			{
				get { return value; }
			}

			public object Orphan
			{
				get { return null; }
			}

			public void Operate()
			{
				enclosingInstance.list.Add(value);
			}
		}

		protected sealed class AddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentList enclosingInstance;
			private readonly int index;
			private readonly object value;

			public AddDelayedOperation(PersistentList enclosingInstance, int index, object value)
			{
				this.enclosingInstance = enclosingInstance;
				this.index = index;
				this.value = value;
			}

			public object AddedInstance
			{
				get { return value; }
			}

			public object Orphan
			{
				get { return null; }
			}

			public void Operate()
			{
				enclosingInstance.list.Insert(index, value);
			}
		}

		protected sealed class SetDelayedOperation : IDelayedOperation
		{
			private readonly PersistentList enclosingInstance;
			private readonly int index;
			private readonly object value;
			private readonly object old;

			public SetDelayedOperation(PersistentList enclosingInstance, int index, object value, object old)
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
				enclosingInstance.list[index] = value;
			}
		}

		protected sealed class RemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentList enclosingInstance;
			private readonly int index;
			private readonly object old;

			public RemoveDelayedOperation(PersistentList enclosingInstance, int index, object old)
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
				enclosingInstance.list.RemoveAt(index);
			}
		}

		protected sealed class SimpleRemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentList enclosingInstance;
			private readonly object value;

			public SimpleRemoveDelayedOperation(PersistentList enclosingInstance, object value)
			{
				this.enclosingInstance = enclosingInstance;
				this.value = value;
			}

			public object AddedInstance
			{
				get { return null; }
			}

			public object Orphan
			{
				get { return value; }
			}

			public void Operate()
			{
				enclosingInstance.list.Remove(value);
			}
		}

		#endregion
	}
}
