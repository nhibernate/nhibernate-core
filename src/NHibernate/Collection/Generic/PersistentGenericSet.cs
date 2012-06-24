using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Iesi.Collections.Generic;
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
	[DebuggerTypeProxy(typeof(CollectionProxy<>))]
	public class PersistentGenericSet<T> : AbstractPersistentCollection, ISet<T>
	{
		// TODO NH: find a way to writeonce (no duplicated code from PersistentSet)

		/* NH considerations:
		 * The implementation of Set<T> in Iesi collections don't have any particular behavior
		 * for strongly typed. BTW we use the same technique used for other collection.
		 */
		/// <summary>
		/// The <see cref="ISet{T}"/> that NHibernate is wrapping.
		/// </summary>
		protected ISet<T> set;

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
		private IList<T> tempList;

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
			var entityMode = Session.EntityMode;
			var clonedSet = new SetSnapShot<T>(set.Count);
			var enumerable = from object current in set
							 select persister.ElementType.DeepCopy(current, entityMode, persister.Factory);
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
			if (set.Count == 0) return sn;
			if (((ICollection)sn).Count == 0) return sn;
			return GetOrphans(sn, set.ToArray(), entityName, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			var elementType = persister.ElementType;
			var snapshot = (ISetSnapshot<T>)GetSnapshot();
			if (((ICollection)snapshot).Count != set.Count)
			{
				return false;
			}

			return !(from object obj in set
					 let oldValue = snapshot[(T)obj]
					 where oldValue == null || elementType.IsDirty(oldValue, obj, Session)
					 select obj).Any();
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((ICollection)snapshot).Count == 0;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			set = (ISet<T>)persister.CollectionType.Instantiate(anticipatedSize);
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
				var element = (T)persister.ElementType.Assemble(array[i], Session, owner);
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
			var element = (T)role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
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
			tempList = new List<T>();
		}

		/// <summary>
		/// Takes the contents stored in the temporary list created during <c>BeginRead()</c>
		/// that was populated during <c>ReadFrom()</c> and write it to the underlying 
		/// PersistentSet.
		/// </summary>
		public override bool EndRead(ICollectionPersister persister)
		{
			foreach (T item in tempList)
			{
				set.Add(item);
			}
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
			var result = new object[set.Count];
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
			var sn = (ISetSnapshot<T>)GetSnapshot();
			var deletes = new List<T>(((ICollection<T>)sn).Count);

			deletes.AddRange(sn.Where(obj => !set.Contains(obj)));

			deletes.AddRange(from obj in set
							 let oldValue = sn[obj]
							 where oldValue != null && elementType.IsDirty(obj, oldValue, Session)
							 select oldValue);

			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			var sn = (ISetSnapshot<T>)GetSnapshot();
			object oldKey = sn[(T)entry];
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
			var that = other as ISet<T>;
			if (that == null)
			{
				return false;
			}
			Read();
			return set.SequenceEqual(that);
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

		#region ISet<T> Members

		public ISet<T> Union(ISet<T> a)
		{
			Read();
			return set.Union(a);
		}

		public ISet<T> Intersect(ISet<T> a)
		{
			Read();
			return set.Intersect(a);
		}

		public ISet<T> Minus(ISet<T> a)
		{
			Read();
			return set.Minus(a);
		}

		public ISet<T> ExclusiveOr(ISet<T> a)
		{
			Read();
			return set.ExclusiveOr(a);
		}

		public bool Contains(T item)
		{
			bool? exists = ReadElementExistence(item);
			return exists == null ? set.Contains(item) : exists.Value;
		}

		public bool ContainsAll(ICollection<T> c)
		{
			Read();
			return set.ContainsAll(c);
		}

		public bool Add(T o)
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
				return false;
			}

			if (exists.Value)
			{
				return false;
			}
			QueueOperation(new SimpleAddDelayedOperation(this, o));
			return true;
		}

		public bool AddAll(ICollection<T> c)
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

		public bool Remove(T o)
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
				return false;
			}

			if (exists.Value)
			{
				QueueOperation(new SimpleRemoveDelayedOperation(this, o));
				return true;
			}
			return false;
		}

		public bool RemoveAll(ICollection<T> c)
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

		public bool RetainAll(ICollection<T> c)
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
				if (set.Count != 0)
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

		#region ICollection<T> Members

		public void CopyTo(T[] array, int arrayIndex)
		{
			// NH : we really need to initialize the set ?
			Read();
			set.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : set.Count; }
		}

		bool ICollection<T>.IsReadOnly
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
			return set.GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
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
			private readonly PersistentGenericSet<T> enclosingInstance;

			public ClearDelayedOperation(PersistentGenericSet<T> enclosingInstance)
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
			private readonly PersistentGenericSet<T> enclosingInstance;
			private readonly T value;

			public SimpleAddDelayedOperation(PersistentGenericSet<T> enclosingInstance, T value)
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
			private readonly PersistentGenericSet<T> enclosingInstance;
			private readonly T value;

			public SimpleRemoveDelayedOperation(PersistentGenericSet<T> enclosingInstance, T value)
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