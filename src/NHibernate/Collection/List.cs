using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// A persistent wrapper for an <see cref="IList"/>
	/// </summary>	
	/// <remarks>
	/// The underlying collection used in an <see cref="ArrayList"/>.
	/// </remarks>
	[Serializable]
	public class List : PersistentCollection, IList
	{
		private IList list;

		/// <summary>
		/// Return a new snapshot of the current state.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns>
		/// A new <see cref="ArrayList"/> that contains Deep Copies of the 
		/// Elements stored in this wrapped collection.
		/// </returns>
		protected override ICollection Snapshot( ICollectionPersister persister )
		{
			ArrayList clonedList = new ArrayList( list.Count );
			foreach( object obj in list )
			{
				clonedList.Add( persister.ElementType.DeepCopy( obj ) );
			}
			return clonedList;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="snapshot"></param>
		/// <returns></returns>
		public override ICollection GetOrphans( object snapshot )
		{
			IList sn = ( IList ) snapshot;
			ArrayList result = new ArrayList( sn.Count );
			result.AddRange( sn );
			PersistentCollection.IdentityRemoveAll( result, list, Session );
			return result;
		}

		/// <summary>
		/// Does the current state of the list exactly match the snapshot?
		/// </summary>
		/// <param name="elementType">The <see cref="IType"/> to compare the elements of the Collection.</param>
		/// <returns>
		/// <c>true</c> if the wrapped list is different than the snapshot
		/// of the list or if one of the elements in the collection is
		/// dirty.
		/// </returns>
		public override bool EqualsSnapshot(IType elementType)
		{
			IList sn = ( IList ) GetSnapshot();
			if( sn.Count != this.list.Count )
			{
				return false;
			}
			for( int i = 0; i < list.Count; i++ )
			{
				if( elementType.IsDirty( list[ i ], sn[ i ], Session ) )
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsWrapper( object collection )
		{
			return list == collection;
		}

		/// <summary>
		/// Initializes an instance of the <see cref="List"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		internal List(ISessionImplementor session)
			: base(session)
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="List"/>
		/// that wraps an existing <see cref="IList"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the list is in.</param>
		/// <param name="list">The <see cref="IList"/> to wrap.</param>
		internal List(ISessionImplementor session, IList list)
			: base(session)
		{
			this.list = list;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		public override void BeforeInitialize( ICollectionPersister persister )
		{
			this.list = new ArrayList();
		}

		public int Count
		{
			get
			{
				Read();
				return list.Count;
			}
		}

		public void CopyTo( Array array, int index )
		{
			Read();
			list.CopyTo( array, index );
		}

		/// <seealso cref="ICollection.SyncRoot"/>
		public object SyncRoot
		{
			get { return this; }
		}

		/// <seealso cref="ICollection.IsSynchronized"/>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <seealso cref="IList.IsFixedSize"/>
		public bool IsFixedSize
		{
			get { return false; }
		}

		/// <seealso cref="IList.IsReadOnly"/>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <seealso cref="IList.Contains(Object)"/>
		public bool Contains( object obj )
		{
			Read();
			return list.Contains( obj );
		}

		/// <summary></summary>
		public IEnumerator GetEnumerator()
		{
			Read();
			return list.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		public override void DelayedAddAll( ICollection coll )
		{
			foreach( object obj in coll )
			{
				list.Add( obj );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int Add( object obj )
		{
			// can't perform a Queued Addition because the non-generic
			// IList interface requires the index the object was added
			// at to be returned
			Write();
			return list.Add( obj );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="obj"></param>
		public void Insert( int index, object obj )
		{
			Write();
			list.Insert( index, obj );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void Remove( object obj )
		{
			Write();
			list.Remove( obj );
		}

		/// <summary></summary>
		public void Clear()
		{
			Write();
			list.Clear();
		}

		/// <summary></summary>
		public object this[ int index ]
		{
			get
			{
				Read();
				return list[ index ];
			}
			set
			{
				Write();
				list[ index ] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt( int index )
		{
			Write();
			list.RemoveAt( index );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int IndexOf( object obj )
		{
			Read();
			return list.IndexOf( obj );
		}

		/// <summary></summary>
		public override bool Empty
		{
			get { return list.Count == 0; }
		}

		/// <summary></summary>
		public override string ToString()
		{
			Read();
			return list.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="persister"></param>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="writeOrder"></param>
		public override void WriteTo( IDbCommand st, ICollectionPersister persister, object entry, int i, bool writeOrder )
		{
			persister.WriteElement( st, entry, writeOrder, Session );
			persister.WriteIndex( st, i, writeOrder, Session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="persister"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ReadFrom( IDataReader rs, ICollectionPersister persister, object owner )
		{
			object element = persister.ReadElement(rs, owner, Session);
			int index = (int)persister.ReadIndex( rs, Session );

			for( int i=list.Count; i<=index; i++ )
			{
				list.Insert( i, null );
			}

			list[ index ] = element;
			return element;
		}

		/// <summary></summary>
		public override IEnumerable Entries()
		{
			return list;
		}

		/// <summary>
		/// Initializes this List from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the List.</param>
		/// <param name="disassembled">The disassembled List.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner )
		{
			BeforeInitialize( persister );
			object[ ] array = ( object[ ] ) disassembled;
			for( int i = 0; i < array.Length; i++ )
			{
				list.Add( persister.ElementType.Assemble( array[ i ], Session, owner ) );
			}
			SetInitialized();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override object Disassemble( ICollectionPersister persister )
		{
			int length = list.Count;
			object[ ] result = new object[length];
			for( int i = 0; i < length; i++ )
			{
				result[ i ] = persister.ElementType.Disassemble( list[ i ], Session );
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override ICollection GetDeletes( IType elemType )
		{
			IList deletes = new ArrayList();
			IList sn = ( IList ) GetSnapshot();
			int end;
			if( sn.Count > list.Count )
			{
				for( int i = list.Count; i < sn.Count; i++ )
				{
					deletes.Add( i );
				}
				end = list.Count;
			}
			else
			{
				end = sn.Count;
			}
			for( int i = 0; i < end; i++ )
			{
				if( list[ i ] == null && sn[ i ] != null )
				{
					deletes.Add( i );
				}
			}
			return deletes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override bool NeedsInserting( object entry, int i, IType elemType )
		{
			IList sn = ( IList ) GetSnapshot();
			return list[ i ] != null && ( i >= sn.Count || sn[ i ] == null );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override bool NeedsUpdating( object entry, int i, IType elemType )
		{
			IList sn = ( IList ) GetSnapshot();
			return i < sn.Count && sn[ i ] != null && list[ i ] != null && elemType.IsDirty( list[ i ], sn[ i ], Session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override object GetIndex( object entry, int i )
		{
			return i;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool EntryExists( object entry, int i )
		{
			return entry != null;
		}


	}
}