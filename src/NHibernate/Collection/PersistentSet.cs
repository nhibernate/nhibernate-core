using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using Iesi.Collections;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection
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
	[DebuggerTypeProxy(typeof(CollectionProxy))]
	public class PersistentSet : AbstractPersistentCollection, ISet
	{
		/// <summary>
		/// The <see cref="ISet"/> that NHibernate is wrapping.
		/// </summary>
		protected ISet internalSet;

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
		protected IList tempList;

		/// <summary>
		/// Returns a Hashtable where the Key &amp; the Value are both a Copy of the
		/// same object.
		/// <see cref="AbstractPersistentCollection.Snapshot(ICollectionPersister)"/>
		/// </summary>
		/// <param name="persister"></param>
		protected override ICollection Snapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;

			Hashtable clonedMap = new Hashtable(internalSet.Count);
			foreach (object obj in internalSet)
			{
				object copied = persister.ElementType.DeepCopy(obj, entityMode, persister.Factory);
				clonedMap[copied] = copied;
			}
			return clonedMap;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			/*
			IDictionary sn = ( IDictionary ) snapshot;
			ArrayList result = new ArrayList( sn.Keys.Count );
			result.AddRange( sn.Keys );
			AbstractPersistentCollection.IdentityRemoveAll( result, internalSet, Session );
			return result;
			*/
			IDictionary sn = (IDictionary) snapshot;
			return GetOrphans(sn.Keys, internalSet, Session);
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IDictionary snapshot = (IDictionary)GetSnapshot();
			if (snapshot.Count != internalSet.Count)
			{
				return false;
			}
			else
			{
				foreach (object obj in internalSet)
				{
					object oldValue = snapshot[obj];
					if (oldValue == null || elementType.IsDirty(oldValue, obj, Session))
					{
						return false;
					}
				}
			}

			return true;
		}

		public override bool IsWrapper(object collection)
		{
			return internalSet == collection;
		}

		/// <summary>
		/// This constructor is NOT meant to be called from user code.
		/// </summary>
		public PersistentSet(ISessionImplementor session)
			: base(session)
		{
		}

		/// <summary>
		/// Creates a new PersistentSet initialized to the values in the Map.
		/// This constructor is NOT meant to be called from user code.
		/// </summary>
		/// <remarks>
		/// Only call this constructor if you consider the map initialized.
		/// </remarks>
		public PersistentSet(ISessionImplementor session, ISet collection)
			: base(session)
		{
			internalSet = collection;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		/// <summary>
		/// Initializes this PersistentSet from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentSet.</param>
		/// <param name="disassembled">The disassembled PersistentSet.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;
			for (int i = 0; i < array.Length; i++)
			{
				object element = persister.ElementType.Assemble(array[i], Session, owner);
				if (element != null)
				{
					internalSet.Add(element);
				}
			}
			SetInitialized();
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
			internalSet = (ISet)persister.CollectionType.Instantiate(-1);
		}

		#region System.Collections.ICollection Members

		/// <summary>
		/// <see cref="ICollection.CopyTo"/>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			Read();
			internalSet.CopyTo(array, index);
		}

		/// <summary>
		/// <see cref="ICollection.Count"/>
		/// </summary>
		public int Count
		{
			get
			{
				Read();
				return internalSet.Count;
			}
		}

		/// <summary>
		/// <see cref="ICollection.IsSynchronized"/>
		/// </summary>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// <see cref="ICollection.SyncRoot"/>
		/// </summary>
		public object SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region Iesi.Collections.ISet Memebers

		public bool Add(object value)
		{
			Initialize(true);
			if (internalSet.Add(value))
			{
				Dirty();
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool AddAll(ICollection coll)
		{
			if (coll.Count < 0)
			{
				return false;
			}

			Initialize(true);
			if (internalSet.AddAll(coll))
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
			Initialize(true);
			if (internalSet.Count > 0)
			{
				internalSet.Clear();
				Dirty();
			}
		}

		public bool Contains(object key)
		{
			Read();
			return internalSet.Contains(key);
		}

		public bool ContainsAll(ICollection c)
		{
			Read();
			return internalSet.ContainsAll(c);
		}

		public ISet ExclusiveOr(ISet a)
		{
			Read();
			return internalSet.ExclusiveOr(a);
		}

		public ISet Intersect(ISet a)
		{
			Read();
			return internalSet.Intersect(a);
		}

		public bool IsEmpty
		{
			get
			{
				Read();
				return internalSet.IsEmpty;
			}
		}

		public ISet Minus(ISet a)
		{
			Read();
			return internalSet.Minus(a);
		}

		public bool Remove(object key)
		{
			Initialize(true);
			if (internalSet.Remove(key))
			{
				Dirty();
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveAll(ICollection c)
		{
			Initialize(true);
			if (internalSet.RemoveAll(c))
			{
				Dirty();
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RetainAll(ICollection c)
		{
			Initialize(true);
			if (internalSet.RetainAll(c))
			{
				Dirty();
				return true;
			}
			else
			{
				return false;
			}
		}

		public ISet Union(ISet a)
		{
			Read();
			return internalSet.Union(a);
		}

		#endregion

		#region System.Collections.IEnumerable Members

		/// <summary>
		/// <see cref="IEnumerable.GetEnumerator"/>
		/// </summary>
		public IEnumerator GetEnumerator()
		{
			Read();
			return internalSet.GetEnumerator();
		}

		#endregion

		#region System.Collections.ICloneable Members

		public object Clone()
		{
			Read();
			return internalSet.Clone();
		}

		#endregion

		public override bool Empty
		{
			get { return internalSet.Count == 0; }
		}

		public override string ToString()
		{
			Read();
			return internalSet.ToString();
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
			tempList = new ArrayList();
		}

		/// <summary>
		/// Takes the contents stored in the temporary list created during <c>BeginRead()</c>
		/// that was populated during <c>ReadFrom()</c> and write it to the underlying 
		/// PersistentSet.
		/// </summary>
		public override bool EndRead(ICollectionPersister persister)
		{
			internalSet.AddAll(tempList);
			tempList = null;
			SetInitialized();
			return true;
		}

		public override IEnumerable Entries()
		{
			return internalSet;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[internalSet.Count];
			int i = 0;

			foreach (object obj in internalSet)
			{
				result[i++] = persister.ElementType.Disassemble(obj, Session, null);
			}
			return result;
		}

		public override IEnumerable GetDeletes(IType elemType, bool indexIsFormula)
		{
			IList deletes = new ArrayList();
			IDictionary snapshot = (IDictionary) GetSnapshot();

			foreach (DictionaryEntry e in snapshot)
			{
				object test = e.Key;

				if (internalSet.Contains(test) == false)
				{
					deletes.Add(test);
				}
			}

			foreach (object obj in internalSet)
			{
				//object testKey = e.Key;
				object oldKey = snapshot[obj];

				if (oldKey != null && elemType.IsDirty(obj, oldKey, Session))
				{
					deletes.Add(obj);
				}
			}

			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IDictionary sn = (IDictionary) GetSnapshot();
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

		public override object GetIndex(object entry, int i)
		{
			throw new NotImplementedException("Sets don't have indexes");
		}

		public override object GetElement(object entry)
		{
			return entry;
		}

		public override object GetSnapshotElement(object entry, int i)
		{
			throw new NotSupportedException("Sets don't support updating by element");
		}

		public override bool EntryExists(object entry, int i)
		{
			return true;
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return internalSet;
		}

		public override bool RowUpdatePossible
		{
			get { return false; }
		}
	}
}
