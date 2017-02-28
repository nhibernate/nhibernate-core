using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// Implements "bag" semantics more efficiently than <see cref="PersistentGenericBag{T}" /> by adding
	/// a synthetic identifier column to the table.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The identifier is unique for all rows in the table, allowing very efficient
	/// updates and deletes.  The value of the identifier is never exposed to the 
	/// application. 
	/// </para>
	/// <para>
	/// Identifier bags may not be used for a many-to-one association.  Furthermore,
	/// there is no reason to use <c>inverse="true"</c>.
	/// </para>
	/// </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy<>))]
	public class PersistentIdentifierBag<T> : AbstractPersistentCollection, IList<T>, IList
	{
		/* NH considerations:
		 * For various reason we know that the underlining type will be a List<T> or a 
		 * PersistentGenericBag<T>; in both cases the class implement all we need to don't duplicate
		 * many code from PersistentBag.
		 * In the explicit implementation of IList<T> we need to duplicate code to take advantage
		 * from the better performance the use of generic implementation have.
		 */
		private Dictionary<int, object> _identifiers; //index -> id 

		private IList<T> _values; //element
		
		public PersistentIdentifierBag() {}
		
		public PersistentIdentifierBag(ISessionImplementor session) : base(session) {}

		public PersistentIdentifierBag(ISessionImplementor session, IEnumerable<T> coll) : base(session)
		{
			_values = coll as IList<T> ?? new List<T>(coll);
			SetInitialized();
			IsDirectlyAccessible = true;
			_identifiers = new Dictionary<int, object>();
		}

		protected IList<T> InternalValues
		{
			get { return _values; }
			set { _values = value; }
		}

		/// <summary>
		/// Initializes this Bag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentIdentifierBag.</param>
		/// <param name="disassembled">The disassembled PersistentIdentifierBag.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			object[] array = (object[])disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);
			for (int i = 0; i < size; i += 2)
			{
				_identifiers[i / 2] = persister.IdentifierType.Assemble(array[i], Session, owner);
				_values.Add((T) persister.ElementType.Assemble(array[i + 1], Session, owner));
			}
		}

		private object GetIdentifier(int index)
		{
			// NH specific : To emulate IDictionary behavior but using Dictionary<int, object> (without boxing/unboxing for index)
			object result;
			_identifiers.TryGetValue(index, out result);
			return result;
		}

		public override object GetIdentifier(object entry, int i)
		{
			return GetIdentifier(i);
		}

		public override bool IsWrapper(object collection)
		{
			return _values == collection;
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			object[] result = new object[_values.Count * 2];

			int i = 0;
			for (int j = 0; j < _values.Count; j++)
			{
				object val = _values[j];
				result[i++] = persister.IdentifierType.Disassemble(_identifiers[j], Session, null);
				result[i++] = persister.ElementType.Disassemble(val, Session, null);
			}

			return result;
		}

		public override bool Empty
		{
			get { return _values.Count == 0; }
		}

		public override IEnumerable Entries(ICollectionPersister persister)
		{
			return _values;
		}

		public override bool EntryExists(object entry, int i)
		{
			return entry != null;
		}

		public override bool EqualsSnapshot(ICollectionPersister persister)
		{
			IType elementType = persister.ElementType;
			var snap = (ISet<SnapshotElement>)GetSnapshot();
			if (snap.Count != _values.Count)
			{
				return false;
			}
			for (int i = 0; i < _values.Count; i++)
			{
				object val = _values[i];
				object id = GetIdentifier(i);
				object old = snap.Where(x => Equals(x.Id, id)).Select(x => x.Value).FirstOrDefault();
				if (elementType.IsDirty(old, val, Session))
				{
					return false;
				}
			}

			return true;
		}

		public override bool IsSnapshotEmpty(object snapshot)
		{
			return ((ISet<SnapshotElement>)snapshot).Count == 0;
		}

		public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
		{
			var snap = (ISet<SnapshotElement>)GetSnapshot();
			ArrayList deletes = new ArrayList(snap.Select(x => x.Id).ToArray());
			for (int i = 0; i < _values.Count; i++)
			{
				if (_values[i] != null)
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
			var snap = (ISet<SnapshotElement>)GetSnapshot();
			object id = GetIdentifier(i);
			return snap.Where(x => Equals(x.Id, id)).Select(x => x.Value).FirstOrDefault();
		}

		public override bool NeedsInserting(object entry, int i, IType elemType)
		{
			var snap = (ISet<SnapshotElement>)GetSnapshot();
			object id = GetIdentifier(i);
			object valueFound = snap.Where(x => Equals(x.Id, id)).Select(x => x.Value).FirstOrDefault();

			return entry != null && (id == null || valueFound == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType)
		{
			if (entry == null)
			{
				return false;
			}
			var snap = (ISet<SnapshotElement>)GetSnapshot();

			object id = GetIdentifier(i);
			if (id == null)
			{
				return false;
			}

			object old = snap.Where(x => Equals(x.Id, id)).Select(x => x.Value).FirstOrDefault();
			return old != null && elemType.IsDirty(old, entry, Session);
		}

		public override object ReadFrom(DbDataReader reader, ICollectionPersister persister, ICollectionAliases descriptor, object owner)
		{
			object element = persister.ReadElement(reader, owner, descriptor.SuffixedElementAliases, Session);
			object id = persister.ReadIdentifier(reader, descriptor.SuffixedIdentifierAlias, Session);

			// eliminate duplication if loaded in a cartesian product
			if (!_identifiers.ContainsValue(id))
			{
				_identifiers[_values.Count] = id;
				_values.Add((T) element);
			}
			return element;
		}

		public override object GetSnapshot(ICollectionPersister persister)
		{
			EntityMode entityMode = Session.EntityMode;

			var map = new HashSet<SnapshotElement>();
			int i = 0;
			foreach (object value in _values)
			{
				object id;
				_identifiers.TryGetValue(i++, out id);
				var valueCopy = persister.ElementType.DeepCopy(value, entityMode, persister.Factory);
				map.Add(new SnapshotElement { Id = id, Value = valueCopy });
			}
			return map;
		}

		public override ICollection GetOrphans(object snapshot, string entityName)
		{
			var sn = (ISet<SnapshotElement>)GetSnapshot();
			return GetOrphans(sn.Select(x => x.Value).ToArray(), (ICollection) _values, entityName, Session);
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
				foreach (object entry in _values)
				{
					int loc = i++;
					if (!_identifiers.ContainsKey(loc)) // TODO: native ids
					{
						object id = persister.IdentifierGenerator.Generate(Session, entry);
						_identifiers[loc] = id;
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
			_identifiers[i] = id;
		}

		protected void BeforeRemove(int index)
		{
			int last = _values.Count - 1;
			for (int i = index; i < last; i++)
			{
				object id = GetIdentifier(i + 1);
				if (id == null)
				{
					_identifiers.Remove(i);
				}
				else
				{
					_identifiers[i] = id;
				}
			}
			_identifiers.Remove(last);
		}

		protected void BeforeInsert(int index)
		{
			for (int i = _values.Count - 1; i >= index; i--)
			{
				object id = GetIdentifier(i);
				if (id == null)
				{
					_identifiers.Remove(i + 1);
				}
				else
				{
					_identifiers[i + 1] = id;
				}
			}
			_identifiers.Remove(index);
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			_identifiers = anticipatedSize <= 0 ? new Dictionary<int, object>() : new Dictionary<int, object>(anticipatedSize + 1);
			InternalValues = (IList<T>) persister.CollectionType.Instantiate(anticipatedSize);
		}

		int IList.Add(object value)
		{
			Add((T) value);
			return _values.Count - 1;
		}

		public void Clear()
		{
			Initialize(true);
			if (_values.Count > 0 || _identifiers.Count > 0)
			{
				_values.Clear();
				_identifiers.Clear();
				Dirty();
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T) value; }
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T) value);
		}

		public void RemoveAt(int index)
		{
			Write();
			BeforeRemove(index);
			_values.RemoveAt(index);
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		bool IList.Contains(object value)
		{
			return Contains((T) value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T) value);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		public int Count
		{
			get { return ReadSize() ? CachedSize : _values.Count; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			for (int i = index; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int IndexOf(T item)
		{
			Read();
			return _values.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			Write();
			BeforeInsert(index);
			_values.Insert(index, item);
		}

		public T this[int index]
		{
			get
			{
				Read();
				return _values[index];
			}
			set
			{
				Write();
				_values[index] = value;
			}
		}

		public void Add(T item)
		{
			Write();
			_values.Add(item);
		}

		public bool Contains(T item)
		{
			Read();
			return _values.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = arrayIndex; i < Count; i++)
			{
				array.SetValue(this[i], i);
			}
		}

		public bool Remove(T item)
		{
			Initialize(true);
			int index = _values.IndexOf(item);
			if (index >= 0)
			{
				BeforeRemove(index);
				_values.RemoveAt(index);
				Dirty();
				return true;
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			Read();
			return _values.GetEnumerator();
		}

		[Serializable]
		private class SnapshotElement : IEquatable<SnapshotElement>
		{
			public object Id { get; set; }
			public object Value { get; set; }

			public bool Equals(SnapshotElement other)
			{
				if (ReferenceEquals(null, other))
				{
					return false;
				}
				if (ReferenceEquals(this, other))
				{
					return true;
				}
				return Equals(other.Id, Id);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
				{
					return false;
				}
				if (ReferenceEquals(this, obj))
				{
					return true;
				}
				if (obj.GetType() != typeof(SnapshotElement))
				{
					return false;
				}
				return Equals((SnapshotElement)obj);
			}

			public override int GetHashCode()
			{
				return (Id != null ? Id.GetHashCode() : 0);
			}
		}
	}
}