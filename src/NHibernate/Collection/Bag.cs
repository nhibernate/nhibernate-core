using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// An unordered, unkeyed collection that can contain the same element
	/// multiple times. The .net collections API, has no <tt>Bag</tt>.
	/// Most developers seem to use <tt>IList</tt>s to represent bag semantics,
	/// so NHibernate follows this practice.
	/// </summary>
	[Serializable]
	public class Bag : PersistentCollection, IList
	{
		private IList bag;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		internal Bag( ISessionImplementor session ) : base( session )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="coll"></param>
		internal Bag( ISessionImplementor session, ICollection coll ) : base( session )
		{
			bag = coll as IList;

			if( bag == null )
			{
				bag = new ArrayList();
				( ( ArrayList ) bag ).AddRange( coll );
			}

			SetInitialized();
			DirectlyAccessible = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override ICollection Elements()
		{
			return bag;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool Empty
		{
			get { return bag.Count == 0; }
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override bool IsWrapper( object collection )
		{
			return bag == collection;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override ICollection Entries()
		{
			return bag;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="persister"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ReadFrom( IDataReader reader, ICollectionPersister persister, object owner )
		{
			object element = persister.ReadElement( reader, owner, Session );
			bag.Add( element );
			return element;
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
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		public override void BeforeInitialize( ICollectionPersister persister )
		{
			this.bag = new ArrayList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public override bool EqualsSnapshot( IType elementType )
		{
			IList sn = ( IList ) GetSnapshot();
			if( sn.Count != bag.Count )
			{
				return false;
			}

			foreach( object elt in bag )
			{
				if( CountOccurrences( elt, bag, elementType ) != CountOccurrences( elt, sn, elementType ) )
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="list"></param>
		/// <param name="elementType"></param>
		/// <returns></returns>
		private int CountOccurrences( object element, IList list, IType elementType )
		{
			int result = 0;
			foreach( object obj in list )
			{
				if( elementType.Equals( element, obj ) )
				{
					result++;
				}
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected override object Snapshot( ICollectionPersister persister )
		{
			ArrayList clonedList = new ArrayList( bag.Count );
			foreach( object obj in bag )
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
			ArrayList result = new ArrayList();
			result.AddRange( sn );
			PersistentCollection.IdentityRemoveAll( result, bag, Session );
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override object Disassemble( ICollectionPersister persister )
		{
			int length = bag.Count;
			object[ ] result = new object[length];

			for( int i = 0; i < length; i++ )
			{
				result[ i ] = persister.ElementType.Disassemble( bag[ i ], Session );
			}

			return result;
		}

		/// <summary>
		/// Initializes this Bag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the Bag.</param>
		/// <param name="disassembled">The disassembled Bag.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner )
		{
			BeforeInitialize( persister );
			object[] array = ( object[] ) disassembled;
			for( int i = 0; i < array.Length; i++ )
			{
				bag.Add( persister.ElementType.Assemble( array[ i ], Session, owner ) );
			}
			SetInitialized();
		}


		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if this Bag needs to be recreated
		/// in the database.
		/// </summary>
		/// <param name="persister"></param>
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


		// For a one-to-many, a <bag> is not really a bag;
		// it is *really* a set, since it can't contain the
		// same element twice. It could be considered a bug
		// in the mapping dtd that <bag> allows <one-to-many>.

		// Anyway, here we implement <set> semantics for a
		// <one-to-many> <bag>!

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override ICollection GetDeletes( IType elemType )
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
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override bool NeedsUpdating( object entry, int i, IType elemType )
		{
			return false;
		}

		#region IList Members

		/// <summary>
		/// 
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		public object this[ int index ]
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt( int index )
		{
			Write();
			bag.RemoveAt( index );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert( int index, object value )
		{
			Write();
			bag.Insert( index, value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void Remove( object value )
		{
			Write();
			bag.Remove( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains( object value )
		{
			Read();
			return bag.Contains( value );
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			Write();
			bag.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf( object value )
		{
			Read();
			return bag.IndexOf( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add( object value )
		{
			if( !QueueAdd( value ) )
			{
				Write();
				return bag.Add( value );
			}
			else
			{
				//TODO: take a look at this - I don't like it because it changes the 
				// meaning of Add - instead of returning the index it was added at 
				// returns a "fake" index - not consistent with IList interface...
				return -1;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		/// <summary>
		/// 
		/// </summary>
		public override bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Count
		{
			get
			{
				Read();
				return bag.Count;
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
			bag.CopyTo( array, index );
		}

		/// <summary>
		/// 
		/// </summary>
		public override object SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IEnumerator GetEnumerator()
		{
			Read();
			return bag.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		public override void DelayedAddAll( ICollection coll )
		{
			foreach( object obj in coll )
			{
				bag.Add( obj );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override object GetIndex( object entry, int i )
		{
			throw new NotSupportedException( "Bags don't have indexes" );
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