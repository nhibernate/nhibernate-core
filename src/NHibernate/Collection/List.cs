using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// A persistent wrapper for an IList
	/// </summary>
	[Serializable]
	public class List : PersistentCollection, IList
	{
		private IList list;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected override object Snapshot( ICollectionPersister persister )
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
			PersistentCollection.IdentityRemoveAll( result, list, session );
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public override bool EqualsSnapshot( IType elementType )
		{
			IList sn = ( IList ) GetSnapshot();
			if( sn.Count != this.list.Count )
			{
				return false;
			}
			for( int i = 0; i < list.Count; i++ )
			{
				if( elementType.IsDirty( list[ i ], sn[ i ], session ) )
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override bool IsWrapper( object collection )
		{
			return list == collection;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		internal List( ISessionImplementor session ) : base( session )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="list"></param>
		internal List( ISessionImplementor session, IList list ) : base( session )
		{
			this.list = list;
			SetInitialized();
			directlyAccessible = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		public override void BeforeInitialize( ICollectionPersister persister )
		{
			this.list = new ArrayList();
		}

		/// <summary></summary>
		public override int Count
		{
			get
			{
				Read();
				return list.Count;
			}
		}

		/// <summary></summary>
		public bool IsEmpty
		{
			get
			{
				Read();
				return list.Count == 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public override void CopyTo( Array array, int index )
		{
			Read();
			list.CopyTo( array, index );
		}

		/// <summary></summary>
		public override object SyncRoot
		{
			get { return this; }
		}

		/// <summary></summary>
		public override bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary></summary>
		public bool IsFixedSize
		{
			get { return false; }
		}

		/// <summary></summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Contains( object obj )
		{
			Read();
			return list.Contains( obj );
		}

		/// <summary></summary>
		public override IEnumerator GetEnumerator()
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
			if( !QueueAdd( obj ) )
			{
				Write();
				return list.Add( obj );
			}
			else
			{
				return -1;
			}
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
		public override ICollection Elements()
		{
			return list;
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
			persister.WriteElement( st, entry, writeOrder, session );
			persister.WriteIndex( st, i, writeOrder, session );
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
			object element = persister.ReadElement(rs, owner, session);
			int index = (int)persister.ReadIndex( rs, session );

			for( int i=list.Count; i<=index; i++ )
			{
				list.Insert( i, null );
			}

			list[ index ] = element;
			return element;
		}

		/// <summary></summary>
		public override ICollection Entries()
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
				list.Add( persister.ElementType.Assemble( array[ i ], session, owner ) );
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
				result[ i ] = persister.ElementType.Disassemble( list[ i ], session );
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
			return i < sn.Count && sn[ i ] != null && list[ i ] != null && elemType.IsDirty( list[ i ], sn[ i ], session );
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