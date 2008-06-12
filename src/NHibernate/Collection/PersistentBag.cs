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
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .NET collections API has no Bag class.
	/// Most developers seem to use <see cref="IList" />s to represent bag semantics,
	/// so NHibernate follows this practice.
	/// </summary>
	[Serializable]
	[DebuggerTypeProxy(typeof(CollectionProxy))]
	public class PersistentBag : AbstractPersistentCollection, IList
	{
		protected IList bag;

		public PersistentBag(ISessionImplementor session) : base(session)
		{
		}

		public PersistentBag(ISessionImplementor session, ICollection coll) : base(session)
		{
			bag = coll as IList;

			if (bag == null)
			{
				bag = new ArrayList();
				((ArrayList) bag).AddRange(coll);
			}

			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override bool Empty
		{
			get { return bag.Count == 0; }
		}

		public override bool IsWrapper(object collection)
		{
			return bag == collection;
		}

		public override IEnumerable Entries()
		{
			return bag;
		}

		public override object ReadFrom(IDataReader reader, ICollectionPersister role, ICollectionAliases descriptor,
		                                object owner)
		{
			object element = role.ReadElement(reader, owner, descriptor.SuffixedElementAliases, Session);
			bag.Add(element);
			return element;
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
			bag = (IList) persister.CollectionType.Instantiate(-1);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IList sn = (IList)GetSnapshot();
			if (sn.Count != bag.Count)
			{
				return false;
			}

			foreach (object elt in bag)
			{
				if (CountOccurrences(elt, bag, elementType) != CountOccurrences(elt, sn, elementType))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Counts the number of times that the <paramref name="element"/> occurs
		/// in the <paramref name="list"/>.
		/// </summary>
		/// <param name="element">The element to find in the list.</param>
		/// <param name="list">The <see cref="IList"/> to search.</param>
		/// <param name="elementType">The <see cref="IType"/> that can determine equality.</param>
		/// <returns>
		/// The number of occurrences of the element in the list.
		/// </returns>
		private int CountOccurrences(object element, IList list, IType elementType)
		{
			int result = 0;
			foreach (object obj in list)
			{
				if (elementType.IsEqual(element, obj, EntityMode.Poco))
				{
					result++;
				}
			}

			return result;
		}

		protected override ICollection Snapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;
			ArrayList clonedList = new ArrayList(bag.Count);
			foreach (object obj in bag)
			{
				clonedList.Add(persister.ElementType.DeepCopy(obj, entityMode, persister.Factory));
			}
			return clonedList;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IList sn = (IList) snapshot;
			ArrayList result = new ArrayList();
			result.AddRange(sn);
			IdentityRemoveAll(result, bag, entityName, Session);
			return result;
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
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;
			for (int i = 0; i < array.Length; i++)
			{
				bag.Add(persister.ElementType.Assemble(array[i], Session, owner));
			}
			SetInitialized();
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

		public override IEnumerable GetDeletes(IType elemType, bool indexIsFormula)
		{
			ArrayList deletes = new ArrayList();
			IList sn = (IList) GetSnapshot();

			int i = 0;

			foreach (object oldObject in sn)
			{
				bool found = false;
				if (bag.Count > i && elemType.IsEqual(oldObject, bag[i++], EntityMode.Poco))
				{
					//a shortcut if its location didn't change!
					found = true;
				}
				else
				{
					//search for it
					foreach (object newObject in bag)
					{
						if (elemType.IsEqual(oldObject, newObject, EntityMode.Poco))
						{
							found = true;
							break;
						}
					}
				}
				if (!found)
				{
					deletes.Add(oldObject);
				}
			}

			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IList sn = (IList) GetSnapshot();
			if (sn.Count > i && elemType.IsEqual(sn[i], entry, EntityMode.Poco))
			{
				// a shortcut if its location didn't change
				return false;
			}
			else
			{
				//search for it
				foreach (object oldObject in sn)
				{
					if (elemType.IsEqual(oldObject, entry, EntityMode.Poco))
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
			Initialize(true);
			bag.RemoveAt(index);
			Dirty();
		}

		public void Insert(int index, object value)
		{
			Initialize(true);
			bag.Insert(index, value);
			Dirty();
		}

		public void Remove(object value)
		{
			Initialize(true);
			int oldCount = bag.Count;
			bag.Remove(value);
			if (oldCount != bag.Count)
			{
				Dirty();
			}
		}

		public bool Contains(object value)
		{
			Read();
			return bag.Contains(value);
		}

		public void Clear()
		{
			Initialize(true);
			if (bag.Count > 0)
			{
				Dirty();
				bag.Clear();
			}
		}

		public int IndexOf(object value)
		{
			Read();
			return bag.IndexOf(value);
		}

		public int Add(object value)
		{
			if (!QueueAdd(value))
			{
				Write();
				return bag.Add(value);
			}
			else
			{
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
			get
			{
				Read();
				return bag.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			Read();
			bag.CopyTo(array, index);
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

		public override void DelayedAddAll(ICollection coll, ICollectionPersister persister)
		{
			bool isOneToMany = persister.IsOneToMany;
			foreach (object obj in coll)
			{
				if (isOneToMany && bag.Contains(obj))
				{
					// Skip this
					continue;
				}
				bag.Add(obj);
			}
		}

		public override object GetIndex(object entry, int i)
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

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return bag;
		}

		public override bool RowUpdatePossible
		{
			get { return false; }
		}
	}
}
