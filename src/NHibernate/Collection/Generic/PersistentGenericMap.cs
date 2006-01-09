#if NET_2_0

using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// A persistent wrapper for a <see cref="IDictionary&lt;TKey,TValue&gt;"/>.  Underlying
	/// collection is a <see cref="Dictionary&lt;TKey,TValue&gt;"/>
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the IDictionary.</typeparam>
	/// <typeparam name="TValue">The type of the elements in the IDictionary.</typeparam>
	[Serializable]
	public class PersistentGenericMap<TKey, TValue> : PersistentCollection, IDictionary<TKey, TValue>
	{
		protected IDictionary<TKey, TValue> map;

		internal PersistentGenericMap(ISessionImplementor session)
			: base(session)
		{
		}

		internal PersistentGenericMap(ISessionImplementor session, IDictionary<TKey, TValue> map)
			: base(session)
		{
		}

		#region PersistentCollection Members

		public override bool Empty
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override System.Collections.ICollection Entries()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override object ReadFrom(System.Data.IDataReader reader, ICollectionPersister persister, object owner)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteTo(System.Data.IDbCommand st, ICollectionPersister role, object entry, int i, bool writeOrder)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override object GetIndex(object entry, int i)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void BeforeInitialize(ICollectionPersister persister)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool EqualsSnapshot(NHibernate.Type.IType elementType)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override System.Collections.ICollection Snapshot(ICollectionPersister persister)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override object Disassemble(ICollectionPersister persister)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool EntryExists(object entry, int i)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool NeedsInserting(object entry, int i, NHibernate.Type.IType elemType)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool NeedsUpdating(object entry, int i, NHibernate.Type.IType elemType)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override System.Collections.ICollection GetDeletes(NHibernate.Type.IType elemType)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool IsWrapper(object collection)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override System.Collections.ICollection GetOrphans(object snapshot)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool IsSynchronized
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override int Count
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void CopyTo(Array array, int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override object SyncRoot
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IDictionary<TKey,TValue> Members

		public void Add(TKey key, TValue value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool ContainsKey(TKey key)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ICollection<TKey> Keys
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool Remove(TKey key)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ICollection<TValue> Values
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public TValue this[TKey key]
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Clear()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsReadOnly
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
#endif