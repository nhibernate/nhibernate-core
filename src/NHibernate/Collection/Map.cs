using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// A persistent wrapper for a <see cref="IDictionary" />. Underlying collection
	/// is a <see cref="Hashtable" />.
	/// </summary>
	[Serializable]
	public class Map : PersistentCollection, IDictionary
	{
		/// <summary></summary>
		protected IDictionary map;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected override object Snapshot( CollectionPersister persister )
		{
			Hashtable clonedMap = new Hashtable( map.Count );
			foreach( DictionaryEntry e in map )
			{
				clonedMap[ e.Key ] = persister.ElementType.DeepCopy( e.Value );
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
			IDictionary sn = ( IDictionary ) snapshot;
			ArrayList result = new ArrayList( sn.Values.Count );
			result.AddRange( sn.Values );
			PersistentCollection.IdentityRemoveAll( result, map.Values, session );
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public override bool EqualsSnapshot( IType elementType )
		{
			IDictionary xmap = ( IDictionary ) GetSnapshot();
			if( xmap.Count != this.map.Count )
			{
				return false;
			}
			foreach( DictionaryEntry entry in map )
			{
				if( elementType.IsDirty( entry.Value, xmap[ entry.Key ], session ) )
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Construct an uninitialized Map.
		/// </summary>
		/// <param name="session">The ISession the Map should be a part of.</param>
		public Map( ISessionImplementor session ) : base( session )
		{
		}

		/// <summary>
		/// Construct an initialized Map based off the values from the existing IDictionary.
		/// </summary>
		/// <param name="session">The ISession the Map should be a part of.</param>
		/// <param name="map">The IDictionary that contains the initial values.</param>
		public Map( ISessionImplementor session, IDictionary map ) : base( session )
		{
			this.map = map;
			SetInitialized();
			directlyAccessible = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		public override void BeforeInitialize( CollectionPersister persister )
		{
			if( persister.HasOrdering )
			{
				// if this Persister has an Ordering then use the ListDictionary because
				// it maintains items in the Dictionary in the same order as they were 
				// added.
				this.map = new ListDictionary();
			}
			else
			{
				this.map = new Hashtable();
			}
		}

		/// <summary></summary>
		public override int Count
		{
			get
			{
				Read();
				return map.Count;
			}
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

		/// <summary></summary>
		public override object SyncRoot
		{
			get { return this; }
		}

		/// <summary></summary>
		public ICollection Keys
		{
			get
			{
				Read();
				return map.Keys;
			}
		}

		/// <summary></summary>
		public ICollection Values
		{
			get
			{
				Read();
				return map.Values;
			}

		}

		/// <summary></summary>
		public override IEnumerator GetEnumerator()
		{
			Read();
			return map.GetEnumerator();

		}

		/// <summary></summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			Read();
			return map.GetEnumerator();

		}

		/// <summary></summary>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			Read();
			return map.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public override void CopyTo( Array array, int index )
		{
			Read();
			map.CopyTo( array, index );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add( object key, object value )
		{
			Write();
			map.Add( key, value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains( object key )
		{
			Read();
			return map.Contains( key );
		}

		/// <summary></summary>
		public object this[ object key ]
		{
			get
			{
				Read();
				return map[ key ];
			}
			set
			{
				Write();
				map[ key ] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Remove( object key )
		{
			Write();
			map.Remove( key );
		}

		/// <summary></summary>
		public void Clear()
		{
			Write();
			map.Clear();
		}

		/// <summary></summary>
		public override ICollection Elements()
		{
			return map.Values;
		}

		/// <summary></summary>
		public override bool Empty
		{
			get { return map.Count == 0; }
		}

		/// <summary></summary>
		public override string ToString()
		{
			Read();
			return map.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="persister"></param>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="writeOrder"></param>
		public override void WriteTo( IDbCommand st, CollectionPersister persister, object entry, int i, bool writeOrder )
		{
			DictionaryEntry e = ( DictionaryEntry ) entry;
			persister.WriteElement( st, e.Value, writeOrder, session );
			persister.WriteIndex( st, e.Key, writeOrder, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="persister"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ReadFrom( IDataReader rs, CollectionPersister persister, object owner )
		{
			object element = persister.ReadElement(rs, owner, session);
			object index = persister.ReadIndex( rs, session );

			map[ index ] = element;
			return element;
		}

		/// <summary></summary>
		public override ICollection Entries()
		{
			ArrayList entries = new ArrayList();
			foreach( DictionaryEntry entry in map )
			{
				entries.Add( entry );
			}
			return entries;
		}

		/// <summary>
		/// Initializes this Map from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the Map.</param>
		/// <param name="disassembled">The disassembled Map.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(CollectionPersister persister, object disassembled, object owner)
		{
			BeforeInitialize( persister );
			object[ ] array = ( object[ ] ) disassembled;
			for( int i = 0; i < array.Length; i += 2 )
			{
				map[ persister.IndexType.Assemble( array[ i ], session, owner ) ] =
					persister.ElementType.Assemble( array[ i + 1 ], session, owner );
			}
			SetInitialized();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override object Disassemble( CollectionPersister persister )
		{
			object[ ] result = new object[map.Count*2];
			int i = 0;
			foreach( DictionaryEntry e in map )
			{
				result[ i++ ] = persister.IndexType.Disassemble( e.Key, session );
				result[ i++ ] = persister.ElementType.Disassemble( e.Value, session );
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
			foreach( DictionaryEntry e in ( ( IDictionary ) GetSnapshot() ) )
			{
				object key = e.Key;
				if( e.Value != null && map[ key ] == null )
				{
					deletes.Add( key );
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
			IDictionary sn = ( IDictionary ) GetSnapshot();
			DictionaryEntry e = ( DictionaryEntry ) entry;
			return ( e.Value != null && sn[ e.Key ] == null );
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
			IDictionary sn = ( IDictionary ) GetSnapshot();
			DictionaryEntry e = ( DictionaryEntry ) entry;
			object snValue = sn[ e.Key ];
			return ( e.Value != null && snValue != null && elemType.IsDirty( snValue, e.Value, session ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override object GetIndex( object entry, int i )
		{
			return ( ( DictionaryEntry ) entry ).Key;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool EntryExists( object entry, int i )
		{
			return ( ( DictionaryEntry ) entry ).Value != null;
		}


	}
}