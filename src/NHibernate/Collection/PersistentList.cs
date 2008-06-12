using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// A persistent wrapper for an <see cref="IList"/>
	/// </summary>	
	/// <remarks>
	/// The underlying collection used in an <see cref="ArrayList"/>.
	/// </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof(CollectionProxy))]
	public class PersistentList : AbstractPersistentCollection, IList
	{
		private IList list;

		/// <summary>
		/// Return a new snapshot of the current state.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns>
		/// A new <see cref="ArrayList"/> that contains Deep Copies of the 
		/// Elements stored in this wrapped collection.
		/// </returns>
		protected override ICollection Snapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;

			ArrayList clonedList = new ArrayList(list.Count);
			foreach (object obj in list)
			{
				clonedList.Add(persister.ElementType.DeepCopy(obj,entityMode, persister.Factory));
			}
			return clonedList;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IList sn = (IList) snapshot;
			ArrayList result = new ArrayList(sn.Count);
			result.AddRange(sn);
			IdentityRemoveAll(result, list, entityName, Session);
			return result;
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IList sn = (IList)GetSnapshot();
			if (sn.Count != this.list.Count)
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

		public override bool IsWrapper(object collection)
		{
			return list == collection;
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentList"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		public PersistentList(ISessionImplementor session)
			: base(session)
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentList"/>
		/// that wraps an existing <see cref="IList"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		/// <param name="list">The <see cref="IList"/> to wrap.</param>
		public PersistentList(ISessionImplementor session, IList list)
			: base(session)
		{
			this.list = list;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
			this.list = (IList) persister.CollectionType.Instantiate(-1);
		}

		public int Count
		{
			get
			{
				Read();
				return list.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			Read();
			list.CopyTo(array, index);
		}

		/// <seealso cref="ICollection.SyncRoot"/>
		public object SyncRoot
		{
			get { return this; }
		}

		/// <seealso cref="ICollection.IsSynchronized"/>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <seealso cref="IList.IsFixedSize"/>
		public bool IsFixedSize
		{
			get { return false; }
		}

		/// <seealso cref="IList.IsReadOnly"/>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <seealso cref="IList.Contains(Object)"/>
		public bool Contains(object obj)
		{
			Read();
			return list.Contains(obj);
		}

		public IEnumerator GetEnumerator()
		{
			Read();
			return list.GetEnumerator();
		}

		public override void DelayedAddAll(ICollection coll, ICollectionPersister persister)
		{
			foreach (object obj in coll)
			{
				list.Add(obj);
			}
		}

		public int Add(object obj)
		{
			// can't perform a Queued Addition because the non-generic
			// IList interface requires the index the object was added
			// at to be returned
			Write();
			return list.Add(obj);
		}

		public void Insert(int index, object obj)
		{
			Initialize(true);
			list.Insert(index, obj);
			Dirty();
		}

		public void Remove(object obj)
		{
			Initialize(true);
			int oldCount = list.Count;
			list.Remove(obj);
			if (oldCount != list.Count)
			{
				Dirty();
			}
		}

		public void Clear()
		{
			Initialize(true);
			if (list.Count > 0)
			{
				list.Clear();
				Dirty();
			}
		}

		public object this[int index]
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

		public void RemoveAt(int index)
		{
			Initialize(true);
			list.RemoveAt(index);
			Dirty();
		}

		public int IndexOf(object obj)
		{
			Read();
			return list.IndexOf(obj);
		}

		public override bool Empty
		{
			get { return list.Count == 0; }
		}

		public override string ToString()
		{
			Read();
			return list.ToString();
		}

		public override object ReadFrom(IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			object element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			int index = (int) role.ReadIndex(rs, descriptor.SuffixedIndexAliases, Session);

			for (int i = list.Count; i <= index; i++)
			{
				list.Insert(i, null);
			}

			list[index] = element;
			return element;
		}

		public override IEnumerable Entries()
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
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(persister.ElementType.Assemble(array[i], Session, owner));
			}
			SetInitialized();
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

		public override IEnumerable GetDeletes(IType elemType, bool indexIsFormula)
		{
			IList deletes = new ArrayList();
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

		public override object GetIndex(object entry, int i)
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

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return list;
		}
	}
}
