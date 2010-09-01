using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// Implements "bag" semantics more efficiently than a regular <see cref="PersistentBag" />
	/// by adding a synthetic identifier column to the table.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The identifier is unique for all rows in the table, allowing very efficient
	/// updates and deletes. The value of the identifier is never exposed to the 
	/// application. 
	/// </para>
	/// <para>
	/// PersistentIdentifierBags may not be used for a many-to-one association.
	/// Furthermore, there is no reason to use <c>inverse="true"</c>.
	/// </para>
	/// </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy))]
	public class PersistentIdentifierBag : AbstractPersistentCollection, IList
	{
		protected IList values; //element
		protected Dictionary<int, object> identifiers; //index -> id 

		public PersistentIdentifierBag() {} // needed for serialization

		public PersistentIdentifierBag(ISessionImplementor session) : base(session) {}

		public PersistentIdentifierBag(ISessionImplementor session, ICollection coll) : base(session)
		{
			IList list = coll as IList;

			if (list != null)
			{
				values = list;
			}
			else
			{
				values = new ArrayList(coll);
			}

			SetInitialized();
			IsDirectlyAccessible = true;
			identifiers = new Dictionary<int, object>();
		}

		/// <summary>
		/// Initializes this Bag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentIdentifierBag.</param>
		/// <param name="disassembled">The disassembled PersistentIdentifierBag.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] array = (object[]) disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i += 2)
			{
				identifiers[i / 2] = persister.IdentifierType.Assemble(array[i], Session, owner);
				values.Add(persister.ElementType.Assemble(array[i + 1], Session, owner));
			}
		}

		private object GetIdentifier(int index)
		{
			// NH specific : To emulate IDictionary behavior but using Dictionary<int, object> (without boxing/unboxing for index)
			object result = null;
			identifiers.TryGetValue(index, out result);
			return result;
		}

		public override object GetIdentifier(object entry, int i)
		{
			return GetIdentifier(i);
		}

		public override bool IsWrapper(object collection)
		{
			return values == collection;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			identifiers = anticipatedSize <= 0 ? new Dictionary<int, object>() : new Dictionary<int, object>(anticipatedSize + 1);
			values = anticipatedSize <= 0 ? new List<object>() : new List<object>(anticipatedSize);
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[values.Count * 2];

			int i = 0;
			for (int j = 0; j < values.Count; j++)
			{
				object val = values[j];
				result[i++] = persister.IdentifierType.Disassemble(identifiers[j], Session, null);
				result[i++] = persister.ElementType.Disassemble(val, Session, null);
			}

			return result;
		}

		public override bool Empty
		{
			get { return values.Count == 0; }
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return values;
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			IDictionary snap = (IDictionary) GetSnapshot();
			if (snap.Count != values.Count)
			{
				return false;
			}
			for (int i = 0; i < values.Count; i++)
			{
				object val = values[i];
				object id = GetIdentifier(i);
				if (id == null)
				{
					return false;
				}

				object old = snap[id];
				if (elementType.IsDirty(old, val, Session))
				{
					return false;
				}
			}

			return true;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((IDictionary) snapshot).Count == 0;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			IDictionary snap = (IDictionary) GetSnapshot();
			ArrayList deletes = new ArrayList(snap.Keys);
			for (int i = 0; i < values.Count; i++)
			{
				if (values[i] != null)
				{
					deletes.Remove(GetIdentifier(i));
				}
			}
			return deletes;
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
			IDictionary snap = (IDictionary) GetSnapshot();
			object id = GetIdentifier(i);
			return snap[id];
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			IDictionary snap = (IDictionary) GetSnapshot();
			object id = GetIdentifier(i);

			return entry != null && (id == null || snap[id] == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			if (entry == null)
			{
				return false;
			}
			IDictionary snap = (IDictionary) GetSnapshot();

			object id = GetIdentifier(i);
			if (id == null)
			{
				return false;
			}

			object old = snap[id];
			return old != null && elemType.IsDirty(old, entry, Session);
		}

		public override object ReadFrom(IDataReader reader, ICollectionPersister persister, ICollectionAliases descriptor, object owner)
		{
			object element = persister.ReadElement(reader, owner, descriptor.SuffixedElementAliases, Session);
			object id = persister.ReadIdentifier(reader, descriptor.SuffixedIdentifierAlias, Session);
			if (!identifiers.ContainsValue(id))
			{
				identifiers[values.Count] = id;
				values.Add(element);
			}
			return element;
		}

		public override ICollection GetSnapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;

			Hashtable map = new Hashtable(values.Count);
			int i = 0;
			foreach (object value in values)
			{
				// NH Different behavior : in Hb they use directly identifiers[i++]
				// probably we have some different behavior in some other place because a Snapshot before save the collection
				// when the identifiers dictionary is empty (in this case the Snapshot is unneeded before save)
				object id;
				if (identifiers.TryGetValue(i++, out id))
				{
					map[id] = persister.ElementType.DeepCopy(value, entityMode, persister.Factory);
				}
			}
			return map;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			IDictionary sn = (IDictionary) snapshot;
			return GetOrphans(sn.Values, values, entityName, Session);
		}

		public override void PreInsert(ICollectionPersister persister)
		{
			if ((persister.IdentifierGenerator as IPostInsertIdentifierGenerator) != null)
			{
				// NH Different behavior (NH specific) : if we are using IdentityGenerator the PreInsert have no effect
				return;
			}
			try
			{
				int i = 0;
				foreach (object entry in values)
				{
					int loc = i++;
					if (!identifiers.ContainsKey(loc)) // TODO: native ids
					{
						object id = persister.IdentifierGenerator.Generate(Session, entry);
						identifiers[loc] = id;
					}
				}
			}
			catch (Exception sqle)
			{
				throw new ADOException("Could not generate idbag row id.", sqle);
			}
		}

		public override void AfterRowInsert(ICollectionPersister persister, object entry, int i, object id)
		{
			identifiers[i] = id;
		}

		protected void BeforeRemove(int index)
		{
			int last = values.Count - 1;
			for (int i = index; i < last; i++)
			{
				object id = GetIdentifier(i + 1);
				if (id == null)
				{
					identifiers.Remove(i);
				}
				else
				{
					identifiers[i] = id;
				}
			}
			identifiers.Remove(last);
		}

		protected void BeforeInsert(int index)
		{
			for (int i = values.Count - 1; i >= index; i--)
			{
				object id = GetIdentifier(i);
				if (id == null)
				{
					identifiers.Remove(i + 1);
				}
				else
				{
					identifiers[i + 1] = id;
				}
			}
			identifiers.Remove(index);
		}

		#region IList Members

		public int Add(object value)
		{
			Write();
			return values.Add(value);
		}

		public void Clear()
		{
			Initialize(true);
			if (values.Count > 0 || identifiers.Count > 0)
			{
				values.Clear();
				identifiers.Clear();
				Dirty();
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public object this[int index]
		{
			get
			{
				Read();
				return values[index];
			}
			set
			{
				Write();
				identifiers.Remove(index);
				values[index] = value;
			}
		}

		public void Insert(int index, object value)
		{
			Write();
			BeforeInsert(index);
			values.Insert(index, value);
		}

		public void RemoveAt(int index)
		{
			Write();
			BeforeRemove(index);
			values.RemoveAt(index);
		}

		public void Remove(object value)
		{
			Initialize(true);
			int index = values.IndexOf(value);
			if (index >= 0)
			{
				BeforeRemove(index);
				values.RemoveAt(index);
				Dirty();
			}
		}

		public bool Contains(object value)
		{
			Read();
			return values.Contains(value);
		}

		public int IndexOf(object value)
		{
			Read();
			return values.IndexOf(value);
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
			get { return ReadSize() ? CachedSize : values.Count; }
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
			return values.GetEnumerator();
		}

		#endregion
	}
}
