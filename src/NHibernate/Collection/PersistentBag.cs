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
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .NET collections API has no Bag class.
	/// Most developers seem to use <see cref="IList" />s to represent bag semantics,
	/// so NHibernate follows this practice.
	/// </summary>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy))]
	public class PersistentBag : AbstractPersistentCollection, IList
	{
		protected IList bag;

		public PersistentBag() {} // needed for serialization

		public PersistentBag(ISessionImplementor session) : base(session) {}

		public PersistentBag(ISessionImplementor session, ICollection coll) : base(session)
		{
			bag = coll as IList;

			if (bag == null)
			{
				bag = new ArrayList(coll);
			}

			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override bool RowUpdatePossible
		{
			get { return false; }
		}

		public override bool IsWrapper(object collection)
		{
			return bag == collection;
		}

		public override bool Empty
		{
			get { return bag.Count == 0; }
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return bag;
		}

		public override object ReadFrom(IDataReader reader, ICollectionPersister role, ICollectionAliases descriptor,
		                                object owner)
		{
			// note that if we load this collection from a cartesian product
			// the multiplicity would be broken ... so use an idbag instead
			object element = role.ReadElement(reader, owner, descriptor.SuffixedElementAliases, Session);
			// NH Different behavior : we don't check for null
			// The NH-750 test show how checking for null we are ignoring the not-found tag and
			// the DB may have some records ignored by NH. This issue may need some more deep consideration.
			//if (element != null)
				bag.Add(element);
			return element;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			bag = (IList) persister.CollectionType.Instantiate(anticipatedSize);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			EntityMode entityMode = Session.EntityMode;

			IList sn = (IList) GetSnapshot();
			if (sn.Count != bag.Count)
			{
				return false;
			}

			foreach (object elt in bag)
			{
				if (CountOccurrences(elt, bag, elementType, entityMode) != CountOccurrences(elt, sn, elementType, entityMode))
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

		/// <summary>
		/// Counts the number of times that the <paramref name="element"/> occurs
		/// in the <paramref name="list"/>.
		/// </summary>
		/// <param name="element">The element to find in the list.</param>
		/// <param name="list">The <see cref="IList"/> to search.</param>
		/// <param name="elementType">The <see cref="IType"/> that can determine equality.</param>
		/// <param name="entityMode">The entity mode.</param>
		/// <returns>
		/// The number of occurrences of the element in the list.
		/// </returns>
		private static int CountOccurrences(object element, IList list, IType elementType, EntityMode entityMode)
		{
			int result = 0;
			foreach (object obj in list)
			{
				if (elementType.IsSame(element, obj, entityMode))
				{
					result++;
				}
			}

			return result;
		}

		public override ICollection GetSnapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;
			List<object> clonedList = new List<object>(bag.Count);
			foreach (object current in bag)
			{
				clonedList.Add(persister.ElementType.DeepCopy(current, entityMode, persister.Factory));
			}
			return clonedList;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IList sn = (IList) snapshot;
			return GetOrphans(sn, bag, entityName, Session);
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			int length = bag.Count;
			object[] result = new object[length];

			for (int i = 0; i < length; i++)
			{
				result[i] = persister.ElementType.Disassemble(bag[i], Session, null);
			}

			return result;
		}

		/// <summary>
		/// Initializes this PersistentBag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentBag.</param>
		/// <param name="disassembled">The disassembled PersistentBag.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] array = (object[]) disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i++)
			{
				object element = persister.ElementType.Assemble(array[i], Session, owner);
				if (element != null)
				{
					bag.Add(element);
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if this PersistentBag needs to be recreated
		/// in the database.
		/// </summary>
		/// <param name="persister"></param>
		/// <returns>
		/// <see langword="false" /> if this is a <c>one-to-many</c> Bag, <see langword="true" /> if this is not
		/// a <c>one-to-many</c> Bag.  Since a Bag is an unordered, unindexed collection 
		/// that permits duplicates it is not possible to determine what has changed in a
		/// <c>many-to-many</c> so it is just recreated.
		/// </returns>
		public override bool NeedsRecreate(ICollectionPersister persister)
		{
			return !persister.IsOneToMany;
		}

		// For a one-to-many, a <bag> is not really a bag;
		// it is *really* a set, since it can't contain the
		// same element twice. It could be considered a bug
		// in the mapping dtd that <bag> allows <one-to-many>.

		// Anyway, here we implement <set> semantics for a
		// <one-to-many> <bag>!

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IType elementType = persister.ElementType;
			EntityMode entityMode = Session.EntityMode;
			List<object> deletes = new List<object>();
			IList sn = (IList) GetSnapshot();
			int i = 0;
			foreach (object old in sn)
			{
				bool found = false;
				if (bag.Count > i && elementType.IsSame(old, bag[i++], entityMode))
				{
					//a shortcut if its location didn't change!
					found = true;
				}
				else
				{
					foreach (object newObject in bag)
					{
						if (elementType.IsSame(old, newObject, entityMode))
						{
							found = true;
							break;
						}
					}
				}
				if (!found)
				{
					deletes.Add(old);
				}
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IList sn = (IList) GetSnapshot();
			EntityMode entityMode = Session.EntityMode;

			if (sn.Count > i && elemType.IsSame(sn[i], entry, entityMode))
			{
				// a shortcut if its location didn't change
				return false;
			}
			else
			{
				//search for it
				foreach (object old in sn)
				{
					if (elemType.IsEqual(old, entry, entityMode))
					{
						return false;
					}
				}
				return true;
			}
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			return false;
		}

		public override object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			throw new NotSupportedException("Bags don't have indexes");
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

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(bag);
		}

		#region IList Members

		public bool IsReadOnly
		{
			get { return false; }
		}

		public object this[int index]
		{
			get
			{
				Read();
				return bag[index];
			}
			set
			{
				Write();
				bag[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			Write();
			bag.RemoveAt(index);
		}

		public void Insert(int index, object value)
		{
			Write();
			bag.Insert(index, value);
		}

		public void Remove(object value)
		{
			Initialize(true);
			// NH: Different implementation: we use the count to know if the value was removed (better performance)
			int contained = bag.Count;
			bag.Remove(value);
			if (contained != bag.Count)
			{
				Dirty();
			}
		}

		public bool Contains(object value)
		{
			bool? exists = ReadElementExistence(value);
			return !exists.HasValue ? bag.Contains(value) : exists.Value;
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
				if (!(bag.Count == 0))
				{
					bag.Clear();
					Dirty();
				}
			}
		}

		public int IndexOf(object value)
		{
			Read();
			return bag.IndexOf(value);
		}

		public int Add(object value)
		{
			if (!IsOperationQueueEnabled)
			{
				Write();
				return bag.Add(value);
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

		public bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get { return false; }
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : bag.Count; }
		}

		public void CopyTo(Array array, int index)
		{
			for (int i = index; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		public object SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			Read();
			return bag.GetEnumerator();
		}

		#endregion

		public override bool AfterInitialize(ICollectionPersister persister)
		{
			// NH Different behavior : NH-739
			// would be nice to prevent this overhead but the operation is managed where the ICollectionPersister is not available
			bool result;
			if (persister.IsOneToMany && HasQueuedOperations)
			{
				int additionStartFrom = bag.Count;
				IList additionQueue = new List<object>(additionStartFrom);
				foreach (object o in QueuedAdditionIterator)
				{
					if (o != null)
					{
						for (int i = 0; i < bag.Count; i++)
						{
							// we are using ReferenceEquals to be sure that is exactly the same queued instance 
							if (ReferenceEquals(o, bag[i]))
							{
								additionQueue.Add(o);
								break;
							}
						}
					}
				}

				result = base.AfterInitialize(persister);

				if(!result)
				{
					// removing duplicated additions
					foreach (object o in additionQueue)
					{
						for (int i = additionStartFrom; i < bag.Count; i++)
						{
							if (ReferenceEquals(o, bag[i]))
							{
								bag.RemoveAt(i);
								break;
							}
						}
					}
				}
			}
			else
			{
				result = base.AfterInitialize(persister);
			}
			return result;
		}

		#region DelayedOperations

		protected sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentBag enclosingInstance;

			public ClearDelayedOperation(PersistentBag enclosingInstance)
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
				enclosingInstance.bag.Clear();
			}
		}

		protected sealed class SimpleAddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentBag enclosingInstance;
			private readonly object value;

			public SimpleAddDelayedOperation(PersistentBag enclosingInstance, object value)
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
				enclosingInstance.bag.Add(value);
			}
		}

		#endregion
	}
}
