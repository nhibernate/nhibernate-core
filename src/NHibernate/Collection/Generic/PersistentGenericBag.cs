#if NET_2_0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .NET collections API, has no <c>Bag</c>.
	/// The <see cref="ICollection{T}" /> interface closely resembles bag semantics,
	/// however NHibernate for net-1.1 used <see cref="System.Collections.IList"/> so 
	/// <see cref="IList{T}"/> is used to ensure the easiest transition
	/// to generics.
	/// </summary>
	/// <typeparam name="T">The type of the element the bag should hold.</typeparam>
	/// <remarks>The underlying collection used is an <see cref="List{T}"/></remarks>
	[Serializable]
	public class PersistentGenericBag<T> : AbstractPersistentCollection, IList<T>, IList
	{
		private IList<T> bag;

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericBag{T}"/>
		/// in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
		public PersistentGenericBag( ISessionImplementor session )
			: base( session )
		{
		}

		/// <summary>
		/// Initializes an instance of the <see cref="PersistentGenericBag{T}"/>
		/// that wraps an existing <see cref="IList{T}"/> in the <paramref name="session"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the bag is in.</param>
		/// <param name="coll">The <see cref="IList{T}"/> to wrap.</param>
		public PersistentGenericBag( ISessionImplementor session, IList<T> coll )
			: base( session )
		{
			bag = coll;

			if( bag == null )
			{
				bag = new List<T>();
				( ( List<T> ) bag ).AddRange( coll );
			}
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		#region ICollection<T> Members

		public void Add( T item )
		{
			if( !QueueAdd( item ) )
			{
				Write();
				bag.Add( item );
			}
		}

		public void Clear()
		{
			Write();
			bag.Clear();
		}

		public bool Contains( T item )
		{
			Read();
			return bag.Contains( item );
		}

		public void CopyTo( T[] array, int arrayIndex )
		{
			Read();
			bag.CopyTo( array, arrayIndex );
		}

		public int Count
		{
			get
			{
				Read();
				return bag.Count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		public bool Remove( T item )
		{
			Write();
			return bag.Remove( item );
		}

		#endregion

		#region IList<T> Members

		public int IndexOf( T item )
		{
			Read();
			return bag.IndexOf( item );
		}

		public void Insert( int index, T item )
		{
			Write();
			bag.Insert( index, item );
		}

		public void RemoveAt( int index )
		{
			Write();
			bag.RemoveAt( index );
		}

		public T this[ int index ]
		{
			get
			{
				Read();
				return bag[ index ];
			}
			set
			{
				Write();
				bag[ index ] = value;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return bag.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return bag.GetEnumerator();
		}

		#endregion

		#region AbstractPersistentCollection Members

		/// <summary>
		/// Is the initialized GenericBag empty?
		/// </summary>
		/// <value><c>true</c> if the bag has a Count==0, <c>false</c> otherwise.</value>
		public override bool Empty
		{
			get { return bag.Count == 0; }
		}

		public override void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner )
		{
			BeforeInitialize( persister );
			object[] array = ( object[] ) disassembled;
			for( int i = 0; i < array.Length; i++ )
			{
				bag.Add( ( T ) persister.ElementType.Assemble( array[ i ], Session, owner ) );
			}
			SetInitialized();
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if this Bag needs to be recreated
		/// in the database.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns>
		/// <c>false</c> if this is a <c>one-to-many</c> Bag, <c>true</c> if this is not
		/// a <c>one-to-many</c> Bag.  Since a Bag is an unordered, unindexed collection 
		/// that permits duplicates it is not possible to determine what has changed in a
		/// <c>many-to-many</c> so it is just recreated.
		/// </returns>
		public override bool NeedsRecreate( ICollectionPersister persister )
		{
			return !persister.IsOneToMany;
		}

		public override IEnumerable Entries()
		{
			return bag;
		}

		public override object ReadFrom( IDataReader reader, ICollectionPersister persister, ICollectionAliases descriptor,
		                                 object owner )
		{
			object element = persister.ReadElement( reader, owner, descriptor.SuffixedElementAliases, Session );
			// TODO: to make this more net-2.0 friendly the value returned from persister.ReadElement
			// should be specified by a type parameter.  However, that would really break NH with net-1.1
			// and I don't want to do that yet - so the cast is appropriate.
			bag.Add( ( T ) element );
			return element;
		}

		//		public override void WriteTo(IDbCommand st, ICollectionPersister persister, object entry, int i, bool writeOrder)
		//		{
		//			persister.WriteElement(st, entry, writeOrder, Session);
		//		}

		public override object GetIndex( object entry, int i )
		{
			throw new NotSupportedException( "Bags don't have indexes" );
		}

		public override object GetElement( object entry )
		{
			return entry;
		}

		public override object GetSnapshotElement( object entry, int i )
		{
			IList<T> sn = ( IList<T> ) GetSnapshot();
			return sn[ i ];
		}

		public override void BeforeInitialize( ICollectionPersister persister )
		{
			this.bag = ( IList<T> ) persister.CollectionType.Instantiate();
		}

		public override bool EqualsSnapshot( IType elementType )
		{
			IList<T> sn = ( IList<T> ) GetSnapshot();
			if( sn.Count != bag.Count )
			{
				return false;
			}

			foreach( T elt in bag )
			{
				if( CountOccurrences( elt, bag, elementType ) != CountOccurrences( elt, sn, elementType ) )
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Counts the number of times that the <paramref name="element"/> occurs
		/// in the <paramref name="list"/>.
		/// </summary>
		/// <param name="element">The element to find in the list.</param>
		/// <param name="list">The <see cref="ICollection{T}"/> to search.</param>
		/// <param name="elementType">The <see cref="IType"/> that can determine equality.</param>
		/// <returns>
		/// The number of occurrences of the element in the list.
		/// </returns>
		private int CountOccurrences( T element, ICollection<T> list, IType elementType )
		{
			int result = 0;
			foreach( T obj in list )
			{
				if( elementType.Equals( element, obj ) )
				{
					result++;
				}
			}

			return result;
		}

		protected override ICollection Snapshot( ICollectionPersister persister )
		{
			List<T> clonedList = new List<T>();
			foreach( T obj in bag )
			{
				clonedList.Add( ( T ) persister.ElementType.DeepCopy( obj ) );
			}

			return clonedList;
		}

		public override object Disassemble( ICollectionPersister persister )
		{
			int length = bag.Count;
			object[] result = new object[length];

			int i = 0;
			foreach( T item in bag )
			{
				result[ i ] = persister.ElementType.Disassemble( item, Session );
				i++;
			}

			return result;
		}

		public override bool EntryExists( object entry, int i )
		{
			return entry != null;
		}

		public override bool NeedsInserting( object entry, int i, IType elemType )
		{
			IList sn = ( IList ) GetSnapshot();
			if( sn.Count > i && elemType.Equals( sn[ i ], entry ) )
			{
				// a shortcut if its location didn't change
				return false;
			}
			else
			{
				//search for it
				foreach( object oldObject in sn )
				{
					if( elemType.Equals( oldObject, entry ) )
					{
						return false;
					}
				}
				return true;
			}
		}

		public override bool NeedsUpdating( object entry, int i, IType elemType )
		{
			return false;
		}

		public override ICollection GetDeletes( IType elemType, bool indexIsFormula )
		{
			ArrayList deletes = new ArrayList();
			IList sn = ( IList ) GetSnapshot();

			int i = 0;

			foreach( object oldObject in sn )
			{
				bool found = false;
				if( bag.Count > i && elemType.Equals( oldObject, bag[ i++ ] ) )
				{
					//a shortcut if its location didn't change!
					found = true;
				}
				else
				{
					//search for it
					foreach( object newObject in bag )
					{
						if( elemType.Equals( oldObject, newObject ) )
						{
							found = true;
							break;
						}
					}
				}
				if( !found )
				{
					deletes.Add( oldObject );
				}
			}

			return deletes;
		}

		/// <summary>
		/// Is this the wrapper for the given underlying bag instance?
		/// </summary>
		/// <param name="collection">The bag that might be wrapped.</param>
		/// <returns>
		/// <c>true</c> if the <paramref name="collection"/> is equal to the
		/// wrapped collection by object reference.
		/// </returns>
		public override bool IsWrapper( object collection )
		{
			return bag == collection;
		}

		public override ICollection GetOrphans( object snapshot, System.Type entityName )
		{
			IList sn = ( IList ) snapshot;
			ArrayList result = new ArrayList();
			result.AddRange( sn );
			// HACK: careful with cast here...
			IdentityRemoveAll( result, ( ICollection ) bag, entityName, Session );
			return result;
		}

		public override void DelayedAddAll( ICollection coll )
		{
			foreach( T obj in coll )
			{
				bag.Add( obj );
			}
		}

		#endregion

		#region IList Members

		// when the method/property takes an "object" parameter then 
		// make sure to use a reference to the non-generic interface
		// so we can ensure that the same exception gets thrown as if
		// there was no NHibernate wrapper around the collection.  For
		// the methods that don't take an "object" parameter then we
		// can just use "this" so we don't duplicate the Read/Write 
		// logic.

		int IList.Add( object value )
		{
			if( !QueueAdd( value ) )
			{
				Write();
				return ( ( IList ) bag ).Add( value );
			}
			else
			{
				return -1;
			}
		}

		void IList.Clear()
		{
			this.Clear();
		}

		bool IList.Contains( object value )
		{
			Read();
			return ( ( IList ) bag ).Contains( value );
		}

		int IList.IndexOf( object value )
		{
			Read();
			return ( ( IList ) bag ).IndexOf( value );
		}

		void IList.Insert( int index, object value )
		{
			Write();
			( ( IList ) bag ).Insert( index, value );
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		void IList.Remove( object value )
		{
			Write();
			( ( IList ) bag ).Remove( value );
		}

		void IList.RemoveAt( int index )
		{
			this.RemoveAt( index );
		}

		object IList.this[ int index ]
		{
			get { return this[ index ]; }
			set
			{
				Write();
				( ( IList ) bag )[ index ] = value;
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo( Array array, int index )
		{
			Read();
			( ( IList ) bag ).CopyTo( array, index );
		}

		int ICollection.Count
		{
			get { return this.Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		#endregion
	}
}

#endif