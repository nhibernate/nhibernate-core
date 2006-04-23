//#if NET_2_0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Iesi.Collections;
using Iesi.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Persister.Collection;
using NHibernate.Loader;

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
	public class PersistentGenericSet<T> : AbstractPersistentCollection, ISet<T>, ISet
	{
		/// <summary>
		/// The <see cref="ISet`1"/> that NHibernate is wrapping.
		/// </summary>
		protected ISet<T> internalSet;

		/// <summary>
		/// A temporary list that holds the objects while the set is being
		/// populated from the database.  
		/// </summary>
		/// <remarks>
		/// This is necessary to ensure that the object being added to the set doesn't
		/// have its <c>GetHashCode()</c> and <c>Equals()</c> methods called during the load
		/// process.
		/// </remarks>
		[NonSerialized]
		protected IList<T> tempList;

		/// <summary>
		/// Returns a Hashtable where the Key &amp; the Value are both a Copy of the
		/// same object.
		/// <see cref="AbstractPersistentCollection.Snapshot(ICollectionPersister)"/>
		/// </summary>
		/// <param name="persister"></param>
		protected override ICollection Snapshot( ICollectionPersister persister )
		{
			Hashtable clonedMap = new Hashtable( internalSet.Count );
			foreach( object obj in internalSet )
			{
				object copied = persister.ElementType.DeepCopy( obj );
				clonedMap[ copied ] = copied;
			}
			return clonedMap;
		}

		public override ICollection GetOrphans( object snapshot )
		{
			/*
			IDictionary sn = ( IDictionary ) snapshot;
			ArrayList result = new ArrayList( sn.Keys.Count );
			result.AddRange( sn.Keys );
			AbstractPersistentCollection.IdentityRemoveAll( result, internalSet, Session );
			return result;
			*/
			IDictionary sn = ( IDictionary ) snapshot;
			return AbstractPersistentCollection.GetOrphans( sn.Keys, ( ICollection ) internalSet, Session );
		}

		public override bool EqualsSnapshot( IType elementType )
		{
			IDictionary snapshot = ( IDictionary ) GetSnapshot();
			if( snapshot.Count != internalSet.Count )
			{
				return false;
			}
			else
			{
				foreach( object obj in internalSet )
				{
					object oldValue = snapshot[ obj ];
					if( oldValue == null || elementType.IsDirty( oldValue, obj, Session ) )
					{
						return false;
					}
				}
			}

			return true;
		}

		public override bool IsWrapper( object collection )
		{
			return internalSet == collection;
		}

		/// <summary>
		/// This constructor is NOT meant to be called from user code.
		/// </summary>
		public PersistentGenericSet( ISessionImplementor session )
			: base( session )
		{
		}

		/// <summary>
		/// Creates a new PersistentGenericSet initialized to the values in the Map.
		/// This constructor is NOT meant to be called from user code.
		/// </summary>
		/// <remarks>
		/// Only call this constructor if you consider the map initialized.
		/// </remarks>
		public PersistentGenericSet( ISessionImplementor session, ISet<T> collection )
			: base( session )
		{
			internalSet = collection;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		/// <summary>
		/// Initializes this PersistentGenericSet from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the set.</param>
		/// <param name="disassembled">The disassembled set.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner )
		{
			BeforeInitialize( persister );
			object[] array = ( object[] ) disassembled;
			for( int i = 0; i < array.Length; i++ )
			{
				internalSet.Add( ( T ) persister.ElementType.Assemble( array[ i ], Session, owner ) );
			}
			SetInitialized();
		}

		public override void BeforeInitialize( ICollectionPersister persister )
		{
			if( persister.HasOrdering )
			{
				throw new NotImplementedException( "No implementation of generic set with ordering" );
			}
			else
			{
				internalSet = ( ISet<T> ) persister.CollectionType.Instantiate();
			}
		}

		#region System.Collections.ICollection Members

		/// <summary>
		/// <see cref="ICollection.CopyTo"/>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo( Array array, int index )
		{
			Read();
			( ( ISet ) internalSet ).CopyTo( array, index );
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

		public bool Add( object value )
		{
			Write();
			return ( ( ISet ) internalSet ).Add( value );
		}

		public bool AddAll( ICollection coll )
		{
			if( coll.Count > 0 )
			{
				Write();
				return ( ( ISet ) internalSet ).AddAll( coll );
			}
			else
			{
				return false;
			}
		}

		public void Clear()
		{
			Write();
			internalSet.Clear();
		}

		public bool Contains( object key )
		{
			Read();
			return ( ( ISet ) internalSet ).Contains( key );
		}

		public bool ContainsAll( ICollection c )
		{
			Read();
			return ( ( ISet ) internalSet ).ContainsAll( c );
		}

		public ISet ExclusiveOr( ISet a )
		{
			Read();
			return ( ( ISet ) internalSet ).ExclusiveOr( a );
		}

		public ISet Intersect( ISet a )
		{
			Read();
			return ( ( ISet ) internalSet ).Intersect( a );
		}

		public bool IsEmpty
		{
			get
			{
				Read();
				return internalSet.IsEmpty;
			}
		}

		public ISet Minus( ISet a )
		{
			Read();
			return ( ( ISet ) internalSet ).Minus( a );
		}

		public bool Remove( object key )
		{
			Write();
			return ( ( ISet ) internalSet ).Remove( key );
		}

		public bool RemoveAll( ICollection c )
		{
			Write();
			return ( ( ISet ) internalSet ).RemoveAll( c );
		}

		public bool RetainAll( ICollection c )
		{
			Write();
			return ( ( ISet ) internalSet ).RetainAll( c );
		}

		public ISet Union( ISet a )
		{
			Read();
			return ( ( ISet ) internalSet ).Union( a );
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

		public override object ReadFrom( IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner )
		{
			object element = role.ReadElement( rs, owner, descriptor.SuffixedElementAliases, Session );
			tempList.Add( ( T ) element );
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
		/// set.
		/// </summary>
		public override bool EndRead()
		{
			internalSet.AddAll( tempList );
			tempList = null;
			SetInitialized();
			return true;
		}

		public override IEnumerable Entries()
		{
			return internalSet;
		}

		public override object Disassemble( ICollectionPersister persister )
		{
			object[] result = new object[ internalSet.Count ];
			int i = 0;

			foreach( object obj in internalSet )
			{
				result[ i++ ] = persister.ElementType.Disassemble( obj, Session );
			}
			return result;
		}

		public override ICollection GetDeletes( IType elemType, bool indexIsFormula )
		{
			IList deletes = new ArrayList();
			IDictionary snapshot = ( IDictionary ) GetSnapshot();

			foreach( DictionaryEntry e in snapshot )
			{
				object test = e.Key;

				if( internalSet.Contains( ( T ) test ) == false )
				{
					deletes.Add( test );
				}

			}

			foreach( object obj in internalSet )
			{
				//object testKey = e.Key;
				object oldKey = snapshot[ obj ];

				if( oldKey != null && elemType.IsDirty( obj, oldKey, Session ) )
				{
					deletes.Add( obj );
				}
			}

			return deletes;

		}

		public override bool NeedsInserting( object entry, int i, IType elemType )
		{
			IDictionary sn = ( IDictionary ) GetSnapshot();
			object oldKey = sn[ entry ];
			// note that it might be better to iterate the snapshot but this is safe,
			// assuming the user implements equals() properly, as required by the set
			// contract!
			return oldKey == null || elemType.IsDirty( oldKey, entry, Session );

		}

		public override bool NeedsUpdating( object entry, int i, IType elemType )
		{
			return false;
		}

		public override object GetIndex( object entry, int i )
		{
			throw new NotImplementedException( "Sets don't have indexes" );
		}

		public override object GetElement( object entry )
		{
			return entry;
		}

		public override object GetSnapshotElement( object entry, int i )
		{
			throw new NotSupportedException( "Sets don't support updating by element" );
		}

		public override bool EntryExists( object entry, int i )
		{
			return true;
		}

		public bool Add( T o )
		{
			Write();
			return internalSet.Add( o );
		}

		void ICollection<T>.Add( T o )
		{
			Write();
			internalSet.Add( o );
		}

		public bool AddAll( ICollection<T> c )
		{
			if( c.Count > 0 )
			{
				Write();
				return internalSet.AddAll( c );
			}
			else
			{
				return false;
			}
		}

		public bool ContainsAll( ICollection<T> c )
		{
			Read();
			return internalSet.ContainsAll( c );
		}

		public ISet<T> ExclusiveOr( ISet<T> a )
		{
			Read();
			return internalSet.ExclusiveOr( a );
		}

		public ISet<T> Intersect( ISet<T> a )
		{
			Read();
			return internalSet.Intersect( a );
		}

		public ISet<T> Minus( ISet<T> a )
		{
			Read();
			return internalSet.Minus( a );
		}

		public bool RemoveAll( ICollection<T> c )
		{
			Write();
			return internalSet.RemoveAll( c );
		}

		public bool RetainAll( ICollection<T> c )
		{
			Write();
			return internalSet.RetainAll( c );
		}

		public ISet<T> Union( ISet<T> a )
		{
			Read();
			return internalSet.Union( a );
		}

		public bool Contains( T item )
		{
			Read();
			return internalSet.Contains( item );
		}

		public void CopyTo( T[] array, int arrayIndex )
		{
			Read();
			internalSet.CopyTo( array, arrayIndex );
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove( T item )
		{
			Write();
			return internalSet.Remove( item );
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return internalSet.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			Read();
			return ( ( ICollection ) internalSet ).GetEnumerator();
		}
	}
}
//#endif