using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// An <c>IdentiferBag</c> implements "bag" semantics more efficiently than
	/// a regular <see cref="Bag" /> by adding a synthetic identifier column to the
	/// table.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The identifier is unique for all rows in the table, allowing very efficient
	/// updates and deletes.  The value of the identifier is never exposed to the 
	/// application. 
	/// </para>
	/// <para>
	/// <c>IdentifierBag</c>s may not be used for a many-to-one association.  Furthermore,
	/// there is no reason to use <c>inverse="true"</c>.
	/// </para>
	/// </remarks>
	[Serializable]
	public class IdentifierBag : PersistentCollection, IList
	{
		private IList values;

		// TODO: h2.1 changed this to index->id, haven't made that change in nh yet.
		private IDictionary identifiers; //element -> id 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		internal IdentifierBag( ISessionImplementor session ) : base( session )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="coll"></param>
		internal IdentifierBag( ISessionImplementor session, ICollection coll ) : base( session )
		{
			IList list = coll as IList;

			if( list!=null )
			{
				values = list;
			}
			else
			{
				values = new ArrayList();
				foreach( object obj in coll )
				{
					values.Add( obj );
				}
			}

			SetInitialized();
			directlyAccessible = true;
			identifiers = new Hashtable();
		}

		/// <summary>
		/// Initializes this Bag from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the IdentifierBag.</param>
		/// <param name="disassembled">The disassembled IdentifierBag.</param>
		/// <param name="owner">The owner object.</param>
		public override void InitializeFromCache(CollectionPersister persister, object disassembled, object owner)
		{
			
			BeforeInitialize( persister );
			object[ ] array = ( object[ ] ) disassembled;

			for( int i = 0; i < array.Length; i += 2 )
			{
				object obj = persister.ElementType.Assemble( array[ i + 1 ], session, owner );
				identifiers[ obj ] = persister.IdentifierType.Assemble( array[ i ], session, owner );
				values.Add( obj );
			}

			SetInitialized();
		}


		#region IList Members

		/// <summary></summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary></summary>
		public object this[ int index ]
		{
			get
			{
				Read();
				return values[ index ];
			}
			set
			{
				Write();
				values[ index ] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt( int index )
		{
			Write();
			values.RemoveAt( index );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert( int index, object value )
		{
			Write();
			values.Insert( index, value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void Remove( object value )
		{
			Write();
			values.Remove( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains( object value )
		{
			Read();
			return values.Contains( value );
		}

		/// <summary></summary>
		public void Clear()
		{
			Write();
			values.Clear();
			identifiers.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf( object value )
		{
			Read();
			return values.IndexOf( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add( object value )
		{
			Write();
			return values.Add( value );
		}

		/// <summary></summary>
		public bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		/// <summary></summary>
		public override bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary></summary>
		public override int Count
		{
			get
			{
				Read();
				return values.Count;
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
			values.CopyTo( array, index );
		}

		/// <summary></summary>
		public override object SyncRoot
		{
			get { return values.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		/// <summary></summary>
		public override IEnumerator GetEnumerator()
		{
			Read();
			return values.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		public override void BeforeInitialize( CollectionPersister persister )
		{
			identifiers = new Hashtable();
			values = new ArrayList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override object Disassemble( CollectionPersister persister )
		{
			object[ ] result = new object[values.Count*2];

			int i = 0;
			foreach( object obj in values )
			{
				result[ i++ ] = persister.IdentifierType.Disassemble( identifiers[ obj ], session );
				result[ i++ ] = persister.ElementType.Disassemble( obj, session );
			}

			return result;
		}

		/// <summary></summary>
		public override ICollection Elements()
		{
			return values;
		}

		/// <summary></summary>
		public override bool Empty
		{
			get { return ( values.Count == 0 ); }
		}

		/// <summary></summary>
		public override ICollection Entries()
		{
			return values;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public override bool EqualsSnapshot( IType elementType )
		{
			IDictionary snap = ( IDictionary ) GetSnapshot();
			if( snap.Count != values.Count )
			{
				return false;
			}

			int i = 0;
			foreach( object obj in values )
			{
				object id = identifiers[ i++ ];
				if( id == null )
				{
					return false;
				}

				object old = snap[ id ];
				if( elementType.IsDirty( old, obj, session ) )
				{
					return false;
				}
			}

			return true;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public override ICollection GetDeletes( IType elemType )
		{
			IDictionary snap = ( IDictionary ) GetSnapshot();
			IList deletes = new ArrayList( snap.Keys );

			int i = 0;
			foreach( object obj in values )
			{
				if( obj != null )
				{
					deletes.Remove( identifiers[ i++ ] );
				}
			}

			return deletes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override object GetIndex( object entry, int i )
		{
			return new NotImplementedException( "Bags don't have indexes" );
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
			IDictionary snap = ( IDictionary ) GetSnapshot();
			object id = identifiers[ i ];

			return entry != null && ( id == null || snap[ id ] == null );
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
			if( entry == null )
			{
				return false;
			}
			IDictionary snap = ( IDictionary ) GetSnapshot();

			object id = identifiers[ i ];
			if( id == null )
			{
				return false;
			}

			object old = snap[ id ];
			return entry != null && old != null && elemType.IsDirty( old, entry, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="persister"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ReadFrom( IDataReader reader, CollectionPersister persister, object owner )
		{
			object element = persister.ReadElement( reader, owner, session );
			values.Add( element );
			identifiers[ values.Count - 1 ] = persister.ReadIdentifier( reader, session );
			return element;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected override object Snapshot( CollectionPersister persister )
		{
			IDictionary map = new Hashtable( values.Count );

			int i = 0;
			foreach( object obj in values )
			{
				object key = identifiers[ i++ ];
				map[ key ] = persister.ElementType.DeepCopy( obj );
			}

			return map;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="snapshot"></param>
		/// <returns></returns>
		public override ICollection GetOrphans( object snapshot )
		{
			IDictionary sn = ( IDictionary ) GetSnapshot();
			ArrayList result = new ArrayList();
			result.AddRange( sn.Values );
			PersistentCollection.IdentityRemoveAll( result, values, session );
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		public override void PreInsert( CollectionPersister persister, object entry, int i )
		{
			try
			{
				object id = persister.IdentifierGenerator.Generate( session, entry );
				// TODO: native ids
				identifiers[ i ] = id;
			}
			catch( Exception sqle )
			{
				throw new ADOException( "Could not generate collection row id.", sqle );
			}
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
			persister.WriteElement( st, entry, writeOrder, session );
			persister.WriteIdentifier( st, identifiers[ i ], writeOrder, session );
		}


	}
}