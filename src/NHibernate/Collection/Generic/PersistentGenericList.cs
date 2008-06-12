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
	/// A persistent wrapper for an <see cref="IList{T}"/>
	/// </summary>
	/// <typeparam name="T">The type of the element the list should hold.</typeparam>
	/// <remarks>The underlying collection used is a <see cref="List{T}"/></remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof(CollectionProxy<>))]
	public class PersistentGenericList<T> : AbstractPersistentCollection, IList<T>, IList
	{
		private IList<T> list;

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericList{T}"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		public PersistentGenericList(ISessionImplementor session) : base(session)
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericList{T}"/>
		/// that wraps an existing <see cref="IList{T}"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
		/// <param name="coll">The <see cref="IList{T}"/> to wrap.</param>
		public PersistentGenericList(ISessionImplementor session, IList<T> coll)
			: base(session)
		{
			list = coll;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			Read();
			return list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			Initialize(true);
			list.Insert(index, item);
			Dirty();
		}

		public void RemoveAt(int index)
		{
			Initialize(true);
			list.RemoveAt(index);
			Dirty();
		}

		public T this[int index]
		{
			get
			{
				Read();
				return list[index];
			}
			set
			{
				Write();
				list[index] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			// we could potentially do a QueueAdd here but essentially
			// won't be <list> mappings don't support inverse collections
			if (!QueueAdd(item))
			{
				Write();
				list.Add(item);
			}
		}

		public void Clear()
		{
			Initialize(true);
			if (list.Count > 0)
			{
				Dirty();
				list.Clear();
			}
		}

		public bool Contains(T item)
		{
			Read();
			return list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Read();
			list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				Read();
				return list.Count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			Initialize(true);
			return MakeDirtyIfTrue(list.Remove(item));
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return list.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return (IEnumerator) list.GetEnumerator();
		}

		#endregion

		#region AbstractPersistentCollection Members

		public override void DelayedAddAll(ICollection coll, ICollectionPersister persister)
		{
			foreach (object obj in coll)
			{
				list.Add((T) obj);
			}
		}

		public override bool Empty
		{
			get { return list.Count == 0; }
		}

		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;
			for (int i = 0; i < array.Length; i++)
			{
				object element = persister.ElementType.Assemble(array[i], this.Session, owner);
				list.Add((element == null ? default(T) : (T) element));
			}
			SetInitialized();
		}

		public override IEnumerable Entries()
		{
			return (IEnumerable) list;
		}

		public override object ReadFrom(IDataReader reader, ICollectionPersister persister, ICollectionAliases descriptor,
		                                object owner)
		{
			object element = persister.ReadElement(reader, owner, descriptor.SuffixedElementAliases, this.Session);
			int index = (int) persister.ReadIndex(reader, descriptor.SuffixedIndexAliases, this.Session);

			// pad with null from the current last element up to the new index
			for (int i = list.Count; i <= index; i++)
			{
				list.Insert(i, default(T));
			}
			list[index] = (element == null ? default(T) : (T) element);
			return element;
		}

//		public override void WriteTo(System.Data.IDbCommand st, ICollectionPersister persister, object entry, int i, bool writeOrder)
//		{
//			persister.WriteElement(st, entry, writeOrder, this.Session);
//			persister.WriteIndex(st, i, writeOrder, this.Session);
//		}

		public override object GetElement(object entry)
		{
			return entry;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			IList<T> sn = (IList<T>) GetSnapshot();
			return sn[i];
		}

		public override object GetIndex(object entry, int i)
		{
			return i;
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
			list = (IList<T>) persister.CollectionType.Instantiate(-1);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IList<T> sn = (IList<T>)GetSnapshot();
			if (sn.Count != list.Count)
			{
				return false;
			}

			for (int i = 0; i < list.Count; i++)
			{
				if (elementType.IsDirty(list[i], sn[i], this.Session))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Return a new snapshot of the current state.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns>
		/// A new <see cref="IList{T}"/> that contains Deep Copies of the 
		/// Elements stored in this wrapped collection.
		/// </returns>
		protected override ICollection Snapshot(ICollectionPersister persister)
		{
			List<T> clonedList = new List<T>(list.Count);
			foreach (T obj in list)
			{
				clonedList.Add((T) persister.ElementType.DeepCopy(obj, EntityMode.Poco, Session.Factory));
			}
			return clonedList;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			int length = list.Count;
			object[] result = new object[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = persister.ElementType.Disassemble(list[i], this.Session, null);
			}
			return result;
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IList<T> sn = (IList<T>) GetSnapshot();
			return list[i] != null && (i >= sn.Count || sn[i] == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			IList<T> sn = (IList<T>) GetSnapshot();
			return i < sn.Count && sn[i] != null && list[i] != null && elemType.IsDirty(list[i], sn[i], this.Session);
		}

		public override IEnumerable GetDeletes(IType elemType, bool indexIsFormula)
		{
			IList deletes = new ArrayList();
			IList<T> sn = (IList<T>) GetSnapshot();
			int end;

			if (sn.Count > list.Count)
			{
				for (int i = list.Count; i < sn.Count; i++)
				{
					deletes.Add(indexIsFormula ? (object) sn[i] : i);
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
					deletes.Add(indexIsFormula ? (object) sn[i] : i);
				}
			}
			return deletes;
		}

		public override bool IsWrapper(object collection)
		{
			return list == collection;
		}

		/// <summary>
		/// Get all "orphaned" elements.
		/// </summary>
		/// <param name="snapshot">The snapshot of the collection.</param>
		/// <param name="entityName">The type of the entities the collection
		/// is supposed to contain.</param>
		/// <returns>
		/// An <see cref="System.Collections.ICollection"/> that contains all of the elements
		/// that have been orphaned.
		/// </returns>
		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IList<T> sn = (IList<T>) snapshot;
			List<T> result = new List<T>(sn.Count);
			result.AddRange(sn);
			IdentityRemoveAll(result, (ICollection) list, entityName, this.Session);
			return result;
		}

		#endregion

		#region IList Members

		// when the method/property takes an "object" parameter then 
		// make sure to use a reference to the non-generic interface
		// so we can ensure that the same exception gets thrown as if
		// there was no NHibernate wrapper around the collection.  For
		// the methods that don't take an "object" parameter then we
		// can just use "this" so we don't duplicate the Read/Write 
		// logic.

		int IList.Add(object value)
		{
			// can't perform a Queued Addition because the non-generic
			// IList interface requires the index the object was added
			// at to be returned
			Write();
			return ((IList) list).Add(value);
		}

		void IList.Clear()
		{
			this.Clear();
		}

		bool IList.Contains(object value)
		{
			Read();
			return ((IList) list).Contains(value);
		}

		int IList.IndexOf(object value)
		{
			Read();
			return ((IList) list).IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			Initialize(true);
			((IList) list).Insert(index, value);
			Dirty();
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		void IList.Remove(object value)
		{
			Initialize(true);
			int oldCount = list.Count;
			((IList) list).Remove(value);
			if (oldCount != list.Count)
			{
				Dirty();
			}
		}

		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				Write();
				((IList) list)[index] = value;
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			Read();
			((IList) list).CopyTo(array, index);
		}

		int ICollection.Count
		{
			get { return this.Count; }
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

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return list;
		}
	}
}
