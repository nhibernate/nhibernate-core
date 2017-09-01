using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NHibernate.Collection.Generic.SetHelpers;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// A persistent wrapper for an <see cref="ISet{T}"/>.
	/// </summary>
	[Serializable]
	[DebuggerTypeProxy(typeof(CollectionProxy<>))]
	public partial class PersistentGenericSet<T> : AbstractPersistentCollection, ISet<T>
	{
		/// <summary>
		/// The <see cref="ISet{T}"/> that NHibernate is wrapping.
		/// </summary>
		protected ISet<T> WrappedSet;

		/// <summary>
		/// A temporary list that holds the objects while the PersistentSet is being
		/// populated from the database.  
		/// </summary>
		/// <remarks>
		/// This is necessary to ensure that the object being added to the PersistentSet doesn't
		/// have its' <c>GetHashCode()</c> and <c>Equals()</c> methods called during the load
		/// process.
		/// </remarks>
		[NonSerialized]
		private IList<T> _tempList;

		// needed for serialization
		public PersistentGenericSet()
		{
		}


		/// <summary> 
		/// Constructor matching super.
		/// Instantiates a lazy set (the underlying set is un-initialized).
		/// </summary>
		/// <param name="session">The session to which this set will belong. </param>
		public PersistentGenericSet(ISessionImplementor session)
			: base(session)
		{
		}

		/// <summary> 
		/// Instantiates a non-lazy set (the underlying set is constructed
		/// from the incoming set reference).
		/// </summary>
		/// <param name="session">The session to which this set will belong. </param>
		/// <param name="original">The underlying set data. </param>
		public PersistentGenericSet(ISessionImplementor session, ISet<T> original)
			: base(session)
		{
			// Sets can be just a view of a part of another collection.
			// do we need to copy it to be sure it won't be changing
			// underneath us?
			// ie. this.set.addAll(set);
			WrappedSet = original;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override bool RowUpdatePossible
		{
			get { return false; }
		}

		public override object GetSnapshot(ICollectionPersister persister)
		{
			var clonedSet = new SetSnapShot<T>(WrappedSet.Count);
			var enumerable = from object current in WrappedSet
							 select persister.ElementType.DeepCopy(current, persister.Factory);
			foreach (var copied in enumerable)
			{
				clonedSet.Add((T)copied);
			}
			return clonedSet;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			var sn = new SetSnapShot<T>((IEnumerable<T>)snapshot);

			// TODO: Avoid duplicating shortcuts and array copy, by making base class GetOrphans() more flexible
			if (WrappedSet.Count == 0) return sn;
			if (((ICollection)sn).Count == 0) return sn;
			return GetOrphans(sn, WrappedSet.ToArray(), entityName, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			var elementType = persister.ElementType;
			var snapshot = (ISetSnapshot<T>)GetSnapshot();
			if (((ICollection)snapshot).Count != WrappedSet.Count)
			{
				return false;
			}


			foreach (T obj in WrappedSet)
			{
				T oldValue;
				if (!snapshot.TryGetValue(obj, out oldValue) || elementType.IsDirty(oldValue, obj, Session))
					return false;
			}

			return true;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((ICollection)snapshot).Count == 0;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			WrappedSet = (ISet<T>)persister.CollectionType.Instantiate(anticipatedSize);
		}

		/// <summary>
		/// Initializes this PersistentSet from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentSet.</param>
		/// <param name="disassembled">The disassembled PersistentSet.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			var array = (object[])disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i++)
			{
				var element = persister.ElementType.Assemble(array[i], Session, owner);
				if (element != null)
				{
					WrappedSet.Add((T) element);
				}
			}
			SetInitialized();
		}

		public override bool Empty
		{
			get { return WrappedSet.Count == 0; }
		}

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(WrappedSet);
		}

		public override object ReadFrom(DbDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			var element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			if (element != null)
			{
				_tempList.Add((T) element);
			}
			return element;
		}

		/// <summary>
		/// Set up the temporary List that will be used in the EndRead() 
		/// to fully create the set.
		/// </summary>
		public override void BeginRead()
		{
			base.BeginRead();
			_tempList = new List<T>();
		}

		/// <summary>
		/// Takes the contents stored in the temporary list created during <c>BeginRead()</c>
		/// that was populated during <c>ReadFrom()</c> and write it to the underlying 
		/// PersistentSet.
		/// </summary>
		public override bool EndRead(ICollectionPersister persister)
		{
			foreach (T item in _tempList)
			{
				WrappedSet.Add(item);
			}
			_tempList = null;
			SetInitialized();
			return true;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return WrappedSet;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			var result = new object[WrappedSet.Count];
			int i = 0;

			foreach (object obj in WrappedSet)
			{
				result[i++] = persister.ElementType.Disassemble(obj, Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IType elementType = persister.ElementType;
			var sn = (ISetSnapshot<T>)GetSnapshot();
			var deletes = new List<T>(((ICollection<T>)sn).Count);

			deletes.AddRange(sn.Where(obj => !WrappedSet.Contains(obj)));


			foreach (var obj in WrappedSet)
			{
				T oldValue;
				if (sn.TryGetValue(obj, out oldValue) && elementType.IsDirty(obj, oldValue, Session))
					deletes.Add(oldValue);
			}

			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			var sn = (ISetSnapshot<T>)GetSnapshot();
			T oldKey;

			// note that it might be better to iterate the snapshot but this is safe,
			// assuming the user implements equals() properly, as required by the PersistentSet
			// contract!
			return !sn.TryGetValue((T) entry, out oldKey) || elemType.IsDirty(oldKey, entry, Session);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			return false;
		}

		public override object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			throw new NotSupportedException("Sets don't have indexes");
		}

		public override object GetElement(object entry)
		{
			return entry;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			throw new NotSupportedException("Sets don't support updating by element");
		}

		public override bool Equals(object other)
		{
			var that = other as ISet<T>;
			if (that == null)
			{
				return false;
			}
			Read();
			return WrappedSet.SequenceEqual(that);
		}

		public override int GetHashCode()
		{
			Read();
			return WrappedSet.GetHashCode();
		}

		public override bool EntryExists(object entry, int i)
		{
			return true;
		}

		public override bool IsWrapper(object collection)
		{
			return WrappedSet == collection;
		}

		#region ISet<T> Members


		public bool Contains(T item)
		{
			bool? exists = ReadElementExistence(item);
			return exists == null ? WrappedSet.Contains(item) : exists.Value;
		}


		public bool Add(T o)
		{
			bool? exists = IsOperationQueueEnabled ? ReadElementExistence(o) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				if (WrappedSet.Add(o))
				{
					Dirty();
					return true;
				}
				return false;
			}

			if (exists.Value)
			{
				return false;
			}
			QueueOperation(new SimpleAddDelayedOperation(this, o));
			return true;
		}

		public void UnionWith(IEnumerable<T> other)
		{
			var collection = other as ICollection<T> ?? other.ToList();
			if (collection.Count == 0)
				return;

			Initialize(true);

			var oldCount = WrappedSet.Count;
			WrappedSet.UnionWith(collection);
			var newCount = WrappedSet.Count;

			// Union can only add, so if the set was modified the count must increase.
			if (oldCount != newCount)
				Dirty();
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			Initialize(true);

			var oldCount = WrappedSet.Count;
			WrappedSet.IntersectWith(other);
			var newCount = WrappedSet.Count;

			// Intersect can only remove, so if the set was modified the count must decrease.
			if (oldCount != newCount)
				Dirty();
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			var collection = other as ICollection<T> ?? other.ToList();
			if (collection.Count == 0)
				return;

			Initialize(true);

			var oldCount = WrappedSet.Count;
			WrappedSet.ExceptWith(collection);
			var newCount = WrappedSet.Count;

			// Except can only remove, so if the set was modified the count must decrease.
			if (oldCount != newCount)
				Dirty();
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			var collection = other as ICollection<T> ?? other.ToList();
			if (collection.Count == 0)
				return;

			Initialize(true);

			WrappedSet.SymmetricExceptWith(collection);

			// If the other collection is non-empty, we are guaranteed to 
			// remove or add at least one element.
			Dirty();
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			Read();
			return WrappedSet.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			Read();
			return WrappedSet.IsSupersetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			Read();
			return WrappedSet.IsProperSupersetOf(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			Read();
			return WrappedSet.IsProperSubsetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			Read();
			return WrappedSet.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			Read();
			return WrappedSet.SetEquals(other);
		}

		public bool Remove(T o)
		{
			bool? exists = PutQueueEnabled ? ReadElementExistence(o) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				if (WrappedSet.Remove(o))
				{
					Dirty();
					return true;
				}
				return false;
			}

			if (exists.Value)
			{
				QueueOperation(new SimpleRemoveDelayedOperation(this, o));
				return true;
			}
			return false;
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
				if (WrappedSet.Count != 0)
				{
					WrappedSet.Clear();
					Dirty();
				}
			}
		}

		#endregion

		#region ICollection<T> Members

		public void CopyTo(T[] array, int arrayIndex)
		{
			// NH : we really need to initialize the set ?
			Read();
			WrappedSet.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : WrappedSet.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}


		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return WrappedSet.GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			Read();
			return WrappedSet.GetEnumerator();
		}

		#endregion


		#region DelayedOperations

		protected sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericSet<T> _enclosingInstance;

			public ClearDelayedOperation(PersistentGenericSet<T> enclosingInstance)
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
				_enclosingInstance.WrappedSet.Clear();
			}
		}

		protected sealed class SimpleAddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericSet<T> _enclosingInstance;
			private readonly T _value;

			public SimpleAddDelayedOperation(PersistentGenericSet<T> enclosingInstance, T value)
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
				_enclosingInstance.WrappedSet.Add(_value);
			}
		}

		protected sealed class SimpleRemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericSet<T> _enclosingInstance;
			private readonly T _value;

			public SimpleRemoveDelayedOperation(PersistentGenericSet<T> enclosingInstance, T value)
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
				_enclosingInstance.WrappedSet.Remove(_value);
			}
		}

		#endregion
	}
}