using System;
using System.Collections;
using System.Data;
using Iesi.Collections;
using NHibernate.Engine;
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
	public class Set : PersistentCollection, ISet
	{
		/// <summary>
		/// The <see cref="ISet"/> that NHibernate is wrapping.
		/// </summary>
		protected ISet internalSet;

		/// <summary>
		/// A temporary list that holds the objects while the Set is being
		/// populated from the database.  
		/// </summary>
		/// <remarks>
		/// This is necessary to ensure that the object being added to the Set doesn't
		/// have its' <c>GetHashCode()</c> and <c>Equals()</c> methods called during the load
		/// process.
		/// </remarks>
		[NonSerialized]
		protected IList tempList;

		/// <summary>
		/// Returns a Hashtable where the Key &amp; the Value are both a Copy of the
		/// same object.
		/// <see cref="PersistentCollection.Snapshot(ICollectionPersister)"/>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="snapshot"></param>
		/// <returns></returns>
		public override ICollection GetOrphans( object snapshot )
		{
			/*
			IDictionary sn = ( IDictionary ) snapshot;
			ArrayList result = new ArrayList( sn.Keys.Count );
			result.AddRange( sn.Keys );
			PersistentCollection.IdentityRemoveAll( result, internalSet, Session );
			return result;
			*/
			IDictionary sn = ( IDictionary ) snapshot;
			return PersistentCollection.GetOrphans( sn.Keys, internalSet, Session );
		}

		/// <summary>
		/// <see cref="IPersistentCollection.EqualsSnapshot"/>
		/// </summary>
		/// <param name="elementType"></param>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override bool IsWrapper( object collection )
		{
			return internalSet == collection;
		}

		/// <summary>
		/// This constructor is NOT meant to be called from user code.
		/// </summary>
		/// <param name="session"></param>
		public Set( ISessionImplementor session ) : base( session )
		{
		}

		/// <summary>
		/// Creates a new Set initialized to the values in the Map.
		/// This constructor is NOT meant to be called from user code.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="collection"></param>
		/// <remarks>
		/// Only call this constructor if you consider the map initialized.
		/// </remarks>
		public Set( ISessionImplementor session, ISet collection ) : base( session )
		{
			internalSet = collection;
			SetInitialized();
			IsDirectlyAccessible = true;
		}

		/// <summary>
		/// Initializes this Set from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the Set.</param>
		/// <param name="disassembled">The disassembled Set.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner )
		{
			BeforeInitialize( persister );
			object[ ] array = ( object[ ] ) disassembled;
			for( int i = 0; i < array.Length; i++ )
			{
				internalSet.Add( persister.ElementType.Assemble( array[ i ], Session, owner ) );
			}
			SetInitialized();
		}

		/// <summary>
		/// <see cref="IPersistentCollection.BeforeInitialize"/>
		/// </summary>
		/// <param name="persister"></param>
		public override void BeforeInitialize( ICollectionPersister persister )
		{
			if( persister.HasOrdering )
			{
				internalSet = new ListSet();
			}
			else
			{
				internalSet = new HashedSet();
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
			internalSet.CopyTo( array, index );
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Add( object value )
		{
			Write();
			return internalSet.Add( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public bool AddAll( ICollection coll )
		{
			if( coll.Count > 0 )
			{
				Write();
				return internalSet.AddAll( coll );
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
		public void Clear()
		{
			Write();
			internalSet.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains( object key )
		{
			Read();
			return internalSet.Contains( key );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public bool ContainsAll( ICollection c )
		{
			Read();
			return internalSet.ContainsAll( c );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public ISet ExclusiveOr( ISet a )
		{
			Read();
			return internalSet.ExclusiveOr( a );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public ISet Intersect( ISet a )
		{
			Read();
			return internalSet.Intersect( a );
		}

		/// <summary></summary>
		public bool IsEmpty
		{
			get
			{
				Read();
				return internalSet.IsEmpty;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public ISet Minus( ISet a )
		{
			Read();
			return internalSet.Minus( a );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Remove( object key )
		{
			Write();
			return internalSet.Remove( key );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public bool RemoveAll( ICollection c )
		{
			Write();
			return internalSet.RemoveAll( c );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public bool RetainAll( ICollection c )
		{
			Write();
			return internalSet.RetainAll( c );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public ISet Union( ISet a )
		{
			Read();
			return internalSet.Union( a );
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

		/// <summary></summary>
		public object Clone()
		{
			Read();
			return internalSet.Clone();
		}

		#endregion

		/*
		/// <summary>
		/// <see cref="PersistentCollection.Elements"/>
		/// </summary>
		public override ICollection Elements()
		{
			return internalSet;
		}
		*/

		/// <summary>
		/// <see cref="IPersistentCollection.Empty"/>
		/// </summary>
		public override bool Empty
		{
			get { return internalSet.Count == 0; }
		}

		/// <summary></summary>
		public override string ToString()
		{
			Read();
			return internalSet.ToString();
		}

		/// <summary>
		/// <see cref="IPersistentCollection.WriteTo"/>
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
		/// <see cref="IPersistentCollection.ReadFrom"/>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="persister"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ReadFrom( IDataReader rs, ICollectionPersister persister, object owner )
		{
			object element = persister.ReadElement(rs, owner, Session);
			tempList.Add( element );
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
		/// Set.
		/// </summary>
		public override bool EndRead()
		{
			internalSet.AddAll( tempList );
			tempList = null;
			SetInitialized();
			return true;
		}

		/// <summary>
		/// <see cref="IPersistentCollection.Entries"/>
		/// </summary>
		public override IEnumerable Entries()
		{
			return internalSet;
		}


		/// <summary>
		/// <see cref="IPersistentCollection.Disassemble"/>
		/// </summary>
		/// <param name="persister"></param>
		public override object Disassemble( ICollectionPersister persister )
		{
			object[ ] result = new object[internalSet.Count];
			int i = 0;

			foreach( object obj in internalSet )
			{
				result[ i++ ] = persister.ElementType.Disassemble( obj, Session );
			}
			return result;
		}

		/// <summary>
		/// <see cref="IPersistentCollection.GetDeletes"/>
		/// </summary>
		/// <param name="elemType"></param>
		public override ICollection GetDeletes( IType elemType )
		{
			IList deletes = new ArrayList();
			IDictionary snapshot = ( IDictionary ) GetSnapshot();

			foreach( DictionaryEntry e in snapshot )
			{
				object test = e.Key;

				if( internalSet.Contains( test ) == false )
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

		/// <summary>
		/// <see cref="IPersistentCollection.NeedsInserting"/>
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override bool NeedsInserting( object entry, int i, IType elemType )
		{
			IDictionary sn = ( IDictionary ) GetSnapshot();
			object oldKey = sn[ entry ];
			// note that it might be better to iterate the snapshot but this is safe,
			// assuming the user implements equals() properly, as required by the Set
			// contract!
			return oldKey == null || elemType.IsDirty( oldKey, entry, Session );

		}

		/// <summary>
		/// <see cref="IPersistentCollection.NeedsUpdating"/>
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override bool NeedsUpdating( object entry, int i, IType elemType )
		{
			return false;
		}

		/// <summary>
		/// <see cref="IPersistentCollection.GetIndex"/>
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override object GetIndex( object entry, int i )
		{
			throw new NotImplementedException( "Sets don't have indexes" );
		}

		/// <summary>
		/// <see cref="IPersistentCollection.EntryExists"/>
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool EntryExists( object entry, int i )
		{
			return true;
		}
	}
}