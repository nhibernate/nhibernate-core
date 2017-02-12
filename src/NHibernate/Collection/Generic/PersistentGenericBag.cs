using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .NET collections API, has no <c>Bag</c>.
	/// Most developers seem to use <see cref="IList{T}"/> to represent bag semantics,
	/// so NHibernate follows this practice.
	/// </summary>
	/// <typeparam name="T">The type of the element the bag should hold.</typeparam>
	/// <remarks>The underlying collection used is an <see cref="List{T}"/></remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy<>))]
	public class PersistentGenericBag<T> : AbstractPersistentCollection, IList<T>, IList
	{
		// TODO NH: find a way to writeonce (no duplicated code from PersistentBag)

		/* NH considerations:
		 * For various reason we know that the underlining type will be a List<T> or a 
		 * PersistentGenericBag<T>; in both cases the class implement all we need to don't duplicate
		 * all code from PersistentBag.
		 * In the explicit implementation of IList<T> we need to duplicate 
		 * code to take advantage from the better performance the use of generic implementation have 
		 * (mean .NET implementation of the underlining list).
		 * In other cases, where PersistentBag use for example bag.Add, a cast, probably, is more
		 * expensive than .NET original implementation.
		 */

		/* For a one-to-many, a <bag> is not really a bag;
		 * it is *really* a set, since it can't contain the
		 * same element twice. It could be considered a bug
		 * in the mapping dtd that <bag> allows <one-to-many>.
		 * Anyway, here we implement <set> semantics for a
		 * <one-to-many> <bag>!
		 */
		private IList<T> _gbag;

		public PersistentGenericBag()
		{
		}

		public PersistentGenericBag(ISessionImplementor session)
			: base(session)
		{
		}

		public PersistentGenericBag(ISessionImplementor session, IEnumerable<T> coll)
			: base(session)
		{
			_gbag = coll as IList<T> ?? new List<T>(coll);
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		protected IList<T> InternalBag
		{
			get { return _gbag; }
			set { _gbag = value; }
		}

		public override bool Empty
		{
			get { return _gbag.Count == 0; }
		}

		public override bool RowUpdatePossible
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			for (var i = index; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T) value);
		}

		int IList.Add(object value)
		{
			Add((T) value);

			//TODO: take a look at this - I don't like it because it changes the 
			// meaning of Add - instead of returning the index it was added at 
			// returns a "fake" index - not consistent with IList interface...
			var count = !IsOperationQueueEnabled
							? _gbag.Count
							: 0;
			return count - 1;
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T) value);
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		bool IList.Contains(object value)
		{
			return Contains((T) value);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T) value; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			Read();
			return _gbag.GetEnumerator();
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : _gbag.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(T item)
		{
			if (!IsOperationQueueEnabled)
			{
				Write();
				_gbag.Add(item);
			}
			else
			{
				QueueOperation(new SimpleAddDelayedOperation(this, item));
			}
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
				if (_gbag.Count != 0)
				{
					_gbag.Clear();
					Dirty();
				}
			}
		}

		public bool Contains(T item)
		{
			var exists = ReadElementExistence(item);
			return !exists.HasValue ? _gbag.Contains(item) : exists.Value;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (var i = arrayIndex; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		public bool Remove(T item)
		{
			Initialize(true);
			var result = _gbag.Remove(item);
			if (result)
			{
				Dirty();
			}
			return result;
		}

		public T this[int index]
		{
			get
			{
				Read();
				return _gbag[index];
			}
			set
			{
				Write();
				_gbag[index] = value;
			}
		}

		public int IndexOf(T item)
		{
			Read();
			return _gbag.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			Write();
			_gbag.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Write();
			_gbag.RemoveAt(index);
		}

		public override bool AfterInitialize(ICollectionPersister persister)
		{
			// NH Different behavior : NH-739
			// would be nice to prevent this overhead but the operation is managed where the ICollectionPersister is not available
			bool result;
			if (persister.IsOneToMany && HasQueuedOperations)
			{
				var additionStartFrom = _gbag.Count;
				IList additionQueue = new List<object>(additionStartFrom);
				foreach (var o in QueuedAdditionIterator)
				{
					if (o != null)
					{
						for (var i = 0; i < _gbag.Count; i++)
						{
							// we are using ReferenceEquals to be sure that is exactly the same queued instance 
							if (ReferenceEquals(o, _gbag[i]))
							{
								additionQueue.Add(o);
								break;
							}
						}
					}
				}

				result = base.AfterInitialize(persister);

				if (!result)
				{
					// removing duplicated additions
					foreach (var o in additionQueue)
					{
						for (var i = additionStartFrom; i < _gbag.Count; i++)
						{
							if (ReferenceEquals(o, _gbag[i]))
							{
								_gbag.RemoveAt(i);
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

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			_gbag = (IList<T>) persister.CollectionType.Instantiate(anticipatedSize);
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			var length = _gbag.Count;
			var result = new object[length];

			for (var i = 0; i < length; i++)
			{
				result[i] = persister.ElementType.Disassemble(_gbag[i], Session, null);
			}

			return result;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return _gbag;
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			var elementType = persister.ElementType;
			var entityMode = Session.EntityMode;

			var sn = (IList) GetSnapshot();
			if (sn.Count != _gbag.Count)
			{
				return false;
			}

			foreach (var elt in _gbag)
			{
				if (CountOccurrences(elt, _gbag, elementType, entityMode) != CountOccurrences(elt, sn, elementType, entityMode))
				{
					return false;
				}
			}

			return true;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			var elementType = persister.ElementType;
			var entityMode = Session.EntityMode;
			var deletes = new List<object>();
			var sn = (IList) GetSnapshot();
			var i = 0;
			foreach (var old in sn)
			{
				var found = false;
				if (_gbag.Count > i && elementType.IsSame(old, _gbag[i++], entityMode))
				{
					//a shortcut if its location didn't change!
					found = true;
				}
				else
				{
					foreach (object newObject in _gbag)
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

		public override object GetElement(object entry)
		{
			return entry;
		}

		public override object GetIndex(object entry, int i, ICollectionPersister persister)
		{
			throw new NotSupportedException("Bags don't have indexes");
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			var sn = (ICollection) snapshot;
			return GetOrphans(sn, (ICollection) _gbag, entityName, Session);
		}

		public override object GetSnapshot(ICollectionPersister persister)
		{
			var entityMode = Session.EntityMode;
			var clonedList = new List<object>(_gbag.Count);
			foreach (object current in _gbag)
			{
				clonedList.Add(persister.ElementType.DeepCopy(current, entityMode, persister.Factory));
			}
			return clonedList;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			var sn = (IList) GetSnapshot();
			return sn[i];
		}

		/// <summary>
		/// Initializes this PersistentBag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentBag.</param>
		/// <param name="disassembled">The disassembled PersistentBag.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			var array = (object[]) disassembled;
			var size = array.Length;
			BeforeInitialize(persister, size);
			for (var i = 0; i < size; i++)
			{
				var element = persister.ElementType.Assemble(array[i], Session, owner);
				if (element != null)
				{
					_gbag.Add((T) element);
				}
			}
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((ICollection) snapshot).Count == 0;
		}

		public override bool IsWrapper(object collection)
		{
			return _gbag == collection;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			var sn = (IList) GetSnapshot();
			var entityMode = Session.EntityMode;

			if (sn.Count > i && elemType.IsSame(sn[i], entry, entityMode))
			{
				// a shortcut if its location didn't change
				return false;
			}
			//search for it
			foreach (var old in sn)
			{
				if (elemType.IsEqual(old, entry, entityMode))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if this PersistentBag needs to be recreated
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

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			return false;
		}

		public override object ReadFrom(DbDataReader reader, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			// note that if we load this collection from a cartesian product
			// the multiplicity would be broken ... so use an idbag instead
			var element = role.ReadElement(reader, owner, descriptor.SuffixedElementAliases, Session);
			// NH Different behavior : we don't check for null
			// The NH-750 test show how checking for null we are ignoring the not-found tag and
			// the DB may have some records ignored by NH. This issue may need some more deep consideration.
			//if (element != null)
			_gbag.Add((T) element);
			return element;
		}

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(_gbag);
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
		private static int CountOccurrences(object element, IEnumerable list, IType elementType, EntityMode entityMode)
		{
			var result = 0;
			foreach (var obj in list)
			{
				if (elementType.IsSame(element, obj, entityMode))
				{
					result++;
				}
			}

			return result;
		}

		private sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericBag<T> _enclosingInstance;

			public ClearDelayedOperation(PersistentGenericBag<T> enclosingInstance)
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
				_enclosingInstance._gbag.Clear();
			}
		}

		private sealed class SimpleAddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentGenericBag<T> _enclosingInstance;
			private readonly T _value;

			public SimpleAddDelayedOperation(PersistentGenericBag<T> enclosingInstance, T value)
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
				_enclosingInstance._gbag.Add(_value);
			}
		}
	}
}
