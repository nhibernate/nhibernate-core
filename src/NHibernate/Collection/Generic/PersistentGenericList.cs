#if NET_2_0

using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// A persistent wrapper for an <see cref="IList&lt;T&gt;"/>
	/// </summary>
	/// <typeparam name="T">The type of the element the list should hold.</typeparam>
	/// <remarks>The underlying collection used is a <see cref="List&lt;T&gt;"/></remarks>
	[Serializable]
	public class PersistentGenericList<T> : PersistentCollection, IList<T>
	{
		private IList<T> list;

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericList&lt;T&gt;"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		internal PersistentGenericList(ISessionImplementor session)
			: base(session)
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericList&lt;T&gt;"/>
		/// that wraps an existing <see cref="IList&lt;T&gt;"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
		/// <param name="coll">The <see cref="IList&lt;T&gt;"/> to wrap.</param>
		internal PersistentGenericList(ISessionImplementor session, IList<T> coll)
			: base(session)
		{
			list = coll;

			if (list == null)
			{
				list = new List<T>(coll);	
			}
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Insert(int index, T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RemoveAt(int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public T this[int index]
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

		#region ICollection<T> Members

		public void Add(T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Clear()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Contains(T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsReadOnly
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool Remove(T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable Members

		public override System.Collections.IEnumerator GetEnumerator()
		{
			Read();
			return (System.Collections.IEnumerator)list.GetEnumerator();
		}

		#endregion

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

		#endregion
	}
}
#endif
