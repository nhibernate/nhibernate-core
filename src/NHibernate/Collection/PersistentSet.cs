using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Iesi.Collections;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection
{
	internal interface ISetSnapshot<T>: ICollection<T>, ICollection
	{
		T this[T element]{ get;}
	}

	[Serializable]
	internal class SetSnapShot<T> : ISetSnapshot<T>
	{
		private readonly List<T> elements;
		public SetSnapShot()
		{
			elements = new List<T>();
		}

		public SetSnapShot(int capacity)
		{
			elements = new List<T>(capacity);			
		}

		public SetSnapShot(IEnumerable<T> collection)
		{
			elements = new List<T>(collection);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			elements.Add(item);
		}

		public void Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(T item)
		{
			return elements.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			elements.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new InvalidOperationException();
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)elements).CopyTo(array, index);
		}

		int ICollection.Count
		{
			get { return elements.Count; }
		}

		public object SyncRoot
		{
			get { return ((ICollection)elements).SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return ((ICollection)elements).IsSynchronized; }
		}

		int ICollection<T>.Count
		{
			get { return elements.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<T>)elements).IsReadOnly; }
		}

		public T this[T element]
		{
			get
			{
				var idx = elements.IndexOf(element);
				if(idx >=0)
				{
					return elements[idx];
				}
				return default(T);
			}
		}
	}

	/// <summary>
	/// .NET has no design equivalent for Java's Set so we are going to use the
	/// Iesi.Collections library. This class is internal to NHibernate and shouldn't
	/// be used by user code.
	/// </summary>
	/// <remarks>
	/// The code for the Iesi.Collections library was taken from the article
	/// <a href="http://www.codeproject.com/csharp/sets.asp">Add Support for "Set" Collections
	/// to .NET</a> that was written by JasonSmith.
	/// </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy))]
	public class PersistentSet : AbstractPersistentCollection, ISet
	{
		/// <summary>
		/// The <see cref="ISet"/> that NHibernate is wrapping.
		/// </summary>
		protected ISet set;

		/// <summary>
		/// A temporary list that holds the objects while the PersistentSet is being
		/// populated from the database.  
		/// </summary>
		/// <remarks>
		/// This is necessary to ensure that the object being added to the PersistentSet doesn't
		/// have its' <c>GetHashCode()</c> and <c>Equals()</c> methods called during the load
		/// process.
		/// </remarks>
		[NonSerialized] private IList tempList;

		public PersistentSet() {} // needed for serialization

		/// <summary> 
		/// Constructor matching super.
		/// Instantiates a lazy set (the underlying set is un-initialized).
		/// </summary>
		/// <param name="session">The session to which this set will belong. </param>
		public PersistentSet(ISessionImplementor session) : base(session) {}

		/// <summary> 
		/// Instantiates a non-lazy set (the underlying set is constructed
		/// from the incoming set reference).
		/// </summary>
		/// <param name="session">The session to which this set will belong. </param>
		/// <param name="original">The underlying set data. </param>
		public PersistentSet(ISessionImplementor session, ISet original) : base(session)
		{
			// Sets can be just a view of a part of another collection.
			// do we need to copy it to be sure it won't be changing
			// underneath us?
			// ie. this.set.addAll(set);
			set = original;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override bool RowUpdatePossible
		{
			get { return false; }
		}

		public override ICollection GetSnapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;

			// NH Different behavior : for NH-1810 and the way is working the possible underlining collection
			// the Snapshot is represented using a List<T> 
			// (perhaps it has less performance then IDictionary but it is working)
			// TODO : should use ever underlining collection type or something to have same performace and same order
			var clonedSet = new SetSnapShot<object>(set.Count);
			foreach (object current in set)
			{
				object copied = persister.ElementType.DeepCopy(current, entityMode, persister.Factory);
				clonedSet.Add(copied);
			}
			return clonedSet;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			var sn = new SetSnapShot<object>((IEnumerable<object>)snapshot);
			return GetOrphans(sn, set, entityName, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			var snapshot = (ISetSnapshot<object>)GetSnapshot();
			if (((ICollection)snapshot).Count != set.Count)
			{
				return false;
			}

			foreach (object obj in set)
			{
				object oldValue = snapshot[obj];
				if (oldValue == null || elementType.IsDirty(oldValue, obj, Session))
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
			set = (ISet) persister.CollectionType.Instantiate(anticipatedSize);
		}

		/// <summary>
		/// Initializes this PersistentSet from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentSet.</param>
		/// <param name="disassembled">The disassembled PersistentSet.</param>
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
					set.Add(element);
				}
			}
			SetInitialized();
		}

		public override bool Empty
		{
			get { return set.Count == 0; }
		}

		public override string ToString()
		{
			Read();
			return StringHelper.CollectionToString(set);
		}

		public override object ReadFrom(IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			object element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			if (element != null)
			{
				tempList.Add(element);
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
			tempList = new List<object>();
		}

		/// <summary>
		/// Takes the contents stored in the temporary list created during <c>BeginRead()</c>
		/// that was populated during <c>ReadFrom()</c> and write it to the underlying 
		/// PersistentSet.
		/// </summary>
		public override bool EndRead(ICollectionPersister persister)
		{
			set.AddAll(tempList);
			tempList = null;
			SetInitialized();
			return true;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return set;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[set.Count];
			int i = 0;

			foreach (object obj in set)
			{
				result[i++] = persister.ElementType.Disassemble(obj, Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IType elementType = persister.ElementType;
			var sn = (ISetSnapshot<object>)GetSnapshot();
			var deletes = new List<object>(((ICollection)sn).Count);
			foreach (object obj in sn)
			{
				if (!set.Contains(obj))
				{
					// the element has been removed from the set
					deletes.Add(obj);
				}
			}
			foreach (object obj in set)
			{
				object oldValue = sn[obj];
				if (oldValue != null && elementType.IsDirty(obj, oldValue, Session))
				{
					// the element has changed
					deletes.Add(oldValue);
				}
			}

			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			var sn = (ISetSnapshot<object>) GetSnapshot();
			object oldKey = sn[entry];
			// note that it might be better to iterate the snapshot but this is safe,
			// assuming the user implements equals() properly, as required by the PersistentSet
			// contract!
			return oldKey == null || elemType.IsDirty(oldKey, entry, Session);
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
			ICollection that = other as ICollection;
			if (that == null)
			{
				return false;
			}
			Read();
			return CollectionHelper.CollectionEquals(set, that);
		}

		public override int GetHashCode()
		{
			Read();
			return set.GetHashCode();
		}

		public override bool EntryExists(object entry, int i)
		{
			return true;
		}

		public override bool IsWrapper(object collection)
		{
			return set == collection;
		}

		#region ISet Members

		public ISet Union(ISet a)
		{
			Read();
			return set.Union(a);
		}

		public ISet Intersect(ISet a)
		{
			Read();
			return set.Intersect(a);
		}

		public ISet Minus(ISet a)
		{
			Read();
			return set.Minus(a);
		}

		public ISet ExclusiveOr(ISet a)
		{
			Read();
			return set.ExclusiveOr(a);
		}

		public bool Contains(object o)
		{
			bool? exists = ReadElementExistence(o);
			return exists == null ? set.Contains(o) : exists.Value;
		}

		public bool ContainsAll(ICollection c)
		{
			Read();
			return set.ContainsAll(c);
		}

		public bool Add(object o)
		{
			bool? exists = IsOperationQueueEnabled ? ReadElementExistence(o) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				if (set.Add(o))
				{
					Dirty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (exists.Value)
			{
				return false;
			}
			else
			{
				QueueOperation(new SimpleAddDelayedOperation(this, o));
				return true;
			}
		}

		public bool AddAll(ICollection c)
		{
			if (c.Count > 0)
			{
				Initialize(true);
				if (set.AddAll(c))
				{
					Dirty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public bool Remove(object o)
		{
			bool? exists = PutQueueEnabled ? ReadElementExistence(o) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				if (set.Remove(o))
				{
					Dirty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (exists.Value)
			{
				QueueOperation(new SimpleRemoveDelayedOperation(this, o));
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveAll(ICollection c)
		{
			if (c.Count > 0)
			{
				Initialize(true);
				if (set.RemoveAll(c))
				{
					Dirty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public bool RetainAll(ICollection c)
		{
			Initialize(true);
			if (set.RetainAll(c))
			{
				Dirty();
				return true;
			}
			else
			{
				return false;
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
				if (!(set.Count == 0))
				{
					set.Clear();
					Dirty();
				}
			}
		}

		public bool IsEmpty
		{
			get { return ReadSize() ? CachedSize == 0 : (set.Count == 0); }
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			// NH : we really need to initialize the set ?
			Read();
			set.CopyTo(array, index);
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : set.Count; }
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
			return set.GetEnumerator();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			Read();
			return set.Clone();
		}

		#endregion

		#region DelayedOperations

		protected sealed class ClearDelayedOperation : IDelayedOperation
		{
			private readonly PersistentSet enclosingInstance;

			public ClearDelayedOperation(PersistentSet enclosingInstance)
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
				enclosingInstance.set.Clear();
			}
		}

		protected sealed class SimpleAddDelayedOperation : IDelayedOperation
		{
			private readonly PersistentSet enclosingInstance;
			private readonly object value;

			public SimpleAddDelayedOperation(PersistentSet enclosingInstance, object value)
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
				enclosingInstance.set.Add(value);
			}
		}

		protected sealed class SimpleRemoveDelayedOperation : IDelayedOperation
		{
			private readonly PersistentSet enclosingInstance;
			private readonly object value;

			public SimpleRemoveDelayedOperation(PersistentSet enclosingInstance, object value)
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
				enclosingInstance.set.Remove(value);
			}
		}

		#endregion
	}
}