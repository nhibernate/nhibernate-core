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
	/// A persistent wrapper for an <see cref="IList{T}"/>
	/// </summary>
	/// <typeparam name="T">The type of the element the list should hold.</typeparam>
	/// <remarks>The underlying collection used is a <see cref="List{T}"/></remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy<>))]
	public partial class PersistentGenericList<T> : AbstractPersistentCollection, IList<T>, IList, IQueryable<T>
	{
		protected IList<T> WrappedList;

		protected virtual T DefaultForType
		{
			get { return default(T); }
		}

		public PersistentGenericList() {}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericList&lt;T&gt;"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		public PersistentGenericList(ISessionImplementor session) : base(session) {}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericList&lt;T&gt;"/>
		/// that wraps an existing <see cref="IList"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		/// <param name="list">The <see cref="IList"/> to wrap.</param>
		public PersistentGenericList(ISessionImplementor session, IList<T> list) : base(session)
		{
			WrappedList = list;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		internal override AbstractQueueOperationTracker CreateQueueOperationTracker()
		{
			var entry = Session.PersistenceContext.GetCollectionEntry(this);
			return new ListQueueOperationTracker<T>(entry.LoadedPersister);
		}

		public override object GetSnapshot(ICollectionPersister persister)
		{
			var clonedList = new List<T>(WrappedList.Count);
			foreach (T current in WrappedList)
			{
				var deepCopy = (T)persister.ElementType.DeepCopy(current, persister.Factory);
				clonedList.Add(deepCopy);
			}

			return clonedList;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			var sn = (IList<T>)snapshot;
			return GetOrphans((ICollection)sn, (ICollection) WrappedList, entityName, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			var sn = (IList<T>) GetSnapshot();
			if (sn.Count != WrappedList.Count)
			{
				return false;
			}
			for (int i = 0; i < WrappedList.Count; i++)
			{
				if (elementType.IsDirty(WrappedList[i], sn[i], Session))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((ICollection)snapshot).Count == 0;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			WrappedList = (IList<T>) persister.CollectionType.Instantiate(anticipatedSize);
		}

		public override void ApplyQueuedOperations()
		{
			var queueOperation = (ListQueueOperationTracker<T>) QueueOperationTracker;
			queueOperation?.ApplyChanges(WrappedList);
			QueueOperationTracker = null;
		}

		public override bool IsWrapper(object collection)
		{
			return ReferenceEquals(WrappedList, collection);
		}

		public override bool Empty
		{
			get { return (WrappedList.Count == 0); }
		}

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(WrappedList);
		}

		public override object ReadFrom(DbDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			var element = (T)role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			int index = (int)role.ReadIndex(rs, descriptor.SuffixedIndexAliases, Session);

			//pad with nulls from the current last element up to the new index
			for (int i = WrappedList.Count; i <= index; i++)
			{
				WrappedList.Insert(i, DefaultForType);
			}

			WrappedList[index] = element;
			return element;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return WrappedList;
		}

		/// <summary>
		/// Initializes this PersistentGenericList from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentGenericList.</param>
		/// <param name="disassembled">The disassembled PersistentList.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] array = (object[])disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i++)
			{
				var element = persister.ElementType.Assemble(array[i], Session, owner);
				WrappedList.Add((T) (element ?? DefaultForType));
			}
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			int length = WrappedList.Count;
			object[] result = new object[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = persister.ElementType.Disassemble(WrappedList[i], Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IList deletes = new List<object>();
			var sn = (IList<T>)GetSnapshot();
			int end;
			if (sn.Count > WrappedList.Count)
			{
				for (int i = WrappedList.Count; i < sn.Count; i++)
				{
					deletes.Add(indexIsFormula ? (object) sn[i] : i);
				}
				end = WrappedList.Count;
			}
			else
			{
				end = sn.Count;
			}
			for (int i = 0; i < end; i++)
			{
				if (WrappedList[i] == null && sn[i] != null)
				{
					deletes.Add(indexIsFormula ? (object) sn[i] : i);
				}
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			var sn = (IList<T>)GetSnapshot();
			return WrappedList[i] != null && (i >= sn.Count || sn[i] == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			var sn = (IList<T>)GetSnapshot();
			return i < sn.Count && sn[i] != null && WrappedList[i] != null && elemType.IsDirty(WrappedList[i], sn[i], Session);
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
			var sn = (IList<T>)GetSnapshot();
			return sn[i];
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		public override bool Equals(object obj)
		{
			var that = obj as ICollection<T>;
			if (that == null)
			{
				return false;
			}
			Read();
			return CollectionHelper.SequenceEquals(WrappedList, that);
		}

		public override int GetHashCode()
		{
			Read();
			return WrappedList.GetHashCode();
		}


		#region IList Members

		int IList.Add(object value)
		{
			if (!IsOperationQueueEnabled || !ReadSize())
			{
				Write();
				return ((IList)WrappedList).Add(value);
			}

			var val = (T) value;
			var queueOperationTracker = GetOrCreateQueueOperationTracker();
			if (queueOperationTracker != null)
			{
				QueueAddElement(val);
			}
			else
			{
#pragma warning disable 618
				QueueOperation(new SimpleAddDelayedOperation(this, val));
#pragma warning restore 618
			}

			return CachedSize - 1;
		}

		bool IList.Contains(object value)
		{
			return Contains((T) value);
		}

		public void Clear()
		{
			if (ClearQueueEnabled)
			{
				var queueOperationTracker = GetOrCreateQueueOperationTracker();
				if (queueOperationTracker != null)
				{
					QueueClearCollection();
				}
				else
				{
#pragma warning disable 618
					QueueOperation(new ClearDelayedOperation(this));
#pragma warning restore 618
				}
			}
			else
			{
				Initialize(true);
				if (WrappedList.Count != 0)
				{
					WrappedList.Clear();
					Dirty();
				}
			}
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T) value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T) value);
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		public void RemoveAt(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(
					nameof(index),
					"Index was out of range. Must be non-negative and less than the size of the collection.");
			}

			var found = TryReadElementAtIndex<T>(index, out var element);
			if (!found.HasValue)
			{
				Write();
				WrappedList.RemoveAt(index);
			}
			else
			{
				var queueOperationTracker = GetOrCreateQueueOperationTracker();
				if (queueOperationTracker != null)
				{
					QueueRemoveElementAtIndex<T>(index, element);
				}
				else
				{
#pragma warning disable 618
					QueueOperation(new RemoveDelayedOperation(this, index, element));
#pragma warning restore 618
				}
			}
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T) value; }
		}

		bool IList.IsReadOnly
		{
			get { return ((ICollection<T>) this).IsReadOnly; }
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		#endregion


		#region IList<T> Members

		public int IndexOf(T item)
		{
			Read();
			return WrappedList.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the List.");
			}
			if (!IsOperationQueueEnabled)
			{
				Write();
				WrappedList.Insert(index, item);
			}
			else
			{
				var queueOperationTracker = GetOrCreateQueueOperationTracker();
				if (queueOperationTracker != null)
				{
					QueueAddElementAtIndex(index, item);
				}
				else
				{
#pragma warning disable 618
					QueueOperation(new AddDelayedOperation(this, index, item));
#pragma warning restore 618
				}
			}
		}

		public T this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException(
						nameof(index),
						"Index was out of range. Must be non-negative and less than the size of the collection.");
				}

				var found = TryReadElementAtIndex<T>(index, out var element);
				if (!found.HasValue)
				{
					return WrappedList[index];
				}
				if (!found.Value)
				{
					// check if the index is valid
					if (index >= Count)
					{
						throw new ArgumentOutOfRangeException(
							nameof(index),
							"Index was out of range. Must be non-negative and less than the size of the collection.");
					}
					return default(T);
				}
				return element;
			}
			set
			{
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException(
						nameof(index),
						"Index was out of range. Must be non-negative and less than the size of the collection.");
				}

				var old = default(T);
				var found = PutQueueEnabled ? TryReadElementAtIndex(index, out old) : null;
				if (!found.HasValue)
				{
					Write();
					WrappedList[index] = value;
				}
				else
				{
					if (EqualityComparer<T>.Default.Equals(value, old))
					{
						return;
					}

					var queueOperationTracker = GetOrCreateQueueOperationTracker();
					if (queueOperationTracker != null)
					{
						QueueSetElementAtIndex(index, value, old);
					}
					else
					{
#pragma warning disable 618
						QueueOperation(new SetDelayedOperation(this, index, value, old));
#pragma warning restore 618
					}
				}
			}
		}

		#endregion


		#region ICollection Members

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			Read();
			if (WrappedList is ICollection collection)
			{
				collection.CopyTo(array, arrayIndex);
			}
			else
			{
				foreach (var item in WrappedList)
					array.SetValue(item, arrayIndex++);
			}
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : WrappedList.Count; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		#endregion


		#region ICollection<T> Members

		public void Add(T item)
		{
			if (!IsOperationQueueEnabled)
			{
				Write();
				WrappedList.Add(item);
			}
			else
			{
				var queueOperationTracker = GetOrCreateQueueOperationTracker();
				if (queueOperationTracker != null)
				{
					QueueAddElement(item);
				}
				else
				{
#pragma warning disable 618
					QueueOperation(new SimpleAddDelayedOperation(this, item));
#pragma warning restore 618
				}
			}
		}

		public bool Contains(T item)
		{
			return ReadElementExistence(item, out _) ?? WrappedList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Read();
			WrappedList.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}

		public bool Remove(T item)
		{
			bool? existsInDb = null;
			bool? exists = PutQueueEnabled ? ReadElementExistence(item, out existsInDb) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				bool contained = WrappedList.Remove(item);
				if (contained)
				{
					Dirty();
					return true;
				}
			}
			else if (exists.Value)
			{
				var queueOperationTracker = GetOrCreateQueueOperationTracker();
				if (queueOperationTracker != null)
				{
					QueueRemoveExistingElement(item, existsInDb);
				}
				else
				{
#pragma warning disable 618
					QueueOperation(new SimpleRemoveDelayedOperation(this, item));
#pragma warning restore 618
				}

				return true;
			}

			return false;
		}

		#endregion


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return WrappedList.GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return WrappedList.GetEnumerator();
		}

		#endregion

		#region IQueryable<T> Members

		[NonSerialized]
		IQueryable<T> _queryable;

		Expression IQueryable.Expression => InnerQueryable.Expression;

		System.Type IQueryable.ElementType => InnerQueryable.ElementType;

		IQueryProvider IQueryable.Provider => InnerQueryable.Provider;

		IQueryable<T> InnerQueryable => _queryable ?? (_queryable = new NhQueryable<T>(Session, this));

		#endregion

		#region DelayedOperations

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericList<T> _enclosingInstance;

			public ClearDelayedOperation(PersistentGenericList<T> enclosingInstance)
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
				_enclosingInstance.WrappedList.Clear();
			}
		}

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class SimpleAddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericList<T> _enclosingInstance;
			private readonly T _value;

			public SimpleAddDelayedOperation(PersistentGenericList<T> enclosingInstance, T value)
			{
				_enclosingInstance = enclosingInstance;
				_value = value;
			}

			public object AddedInstance
			{
				get { return _value; }
			}

			public object Orphan
			{
				get { return null; }
			}

			public void Operate()
			{
				_enclosingInstance.WrappedList.Add(_value);
			}
		}

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class AddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericList<T> _enclosingInstance;
			private readonly int _index;
			private readonly T _value;

			public AddDelayedOperation(PersistentGenericList<T> enclosingInstance, int index, T value)
			{
				_enclosingInstance = enclosingInstance;
				_index = index;
				_value = value;
			}

			public object AddedInstance
			{
				get { return _value; }
			}

			public object Orphan
			{
				get { return null; }
			}

			public void Operate()
			{
				_enclosingInstance.WrappedList.Insert(_index, _value);
			}
		}

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class SetDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericList<T> _enclosingInstance;
			private readonly int _index;
			private readonly T _value;
			private readonly object _old;

			public SetDelayedOperation(PersistentGenericList<T> enclosingInstance, int index, T value, object old)
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
				_enclosingInstance.WrappedList[_index] = _value;
			}
		}

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class RemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericList<T> _enclosingInstance;
			private readonly int _index;
			private readonly object _old;

			public RemoveDelayedOperation(PersistentGenericList<T> enclosingInstance, int index, object old)
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
				_enclosingInstance.WrappedList.RemoveAt(_index);
			}
		}

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected sealed class SimpleRemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericList<T> _enclosingInstance;
			private readonly T _value;

			public SimpleRemoveDelayedOperation(PersistentGenericList<T> enclosingInstance, T value)
			{
				_enclosingInstance = enclosingInstance;
				_value = value;
			}

			public object AddedInstance
			{
				get { return null; }
			}

			public object Orphan
			{
				get { return _value; }
			}

			public void Operate()
			{
				_enclosingInstance.WrappedList.Remove(_value);
			}
		}

		#endregion
	}
}
