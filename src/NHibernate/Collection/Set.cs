using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection 
{
	/// <summary>
	/// .NET has no design equivalent for Java's Set so we are going to use the
	/// Iesi.Collections library.
	/// </summary>
	/// <remarks>
	/// The code for the Iesi.Collections library was taken from the article
	/// <a href="http://www.codeproject.com/csharp/sets.asp">Add Support for "Set" Collections
	/// to .NET</a> that was written by JasonSmith.
	/// </remarks>
	[Serializable]
	public class Set : PersistentCollection, Iesi.Collections.ISet	
	{
		protected Iesi.Collections.ISet _set;
		
		[NonSerialized] protected IList tempIdentifierList;

		/// <summary>
		/// Returns a Hashtable where the Key &amp; the Value are both a Copy of the
		/// same object.
		/// <see cref="PersistentCollection.Snapshot"/>
		/// </summary>
		protected override object Snapshot(CollectionPersister persister) 
		{
			Hashtable clonedMap = new Hashtable( _set.Count );
			foreach( object obj in _set )
			{
				object copied = persister.ElementType.DeepCopy( obj );
				clonedMap[ copied ] = copied;
			}
			return clonedMap;
		}

		public override ICollection GetOrphans(object snapshot)
		{
			IDictionary sn = (IDictionary)snapshot;
			ArrayList result = new ArrayList(sn.Keys.Count);
			result.AddRange(sn.Keys);
			PersistentCollection.IdentityRemoveAll( result,_set, session );
			return result;
		}

		/// <summary>
		/// <see cref="PersistentCollection.EqualsSnapshot"/>
		/// </summary>
		public override bool EqualsSnapshot(IType elementType) 
		{
			IDictionary snapshot = (IDictionary) GetSnapshot();
			if ( snapshot.Count!= _set.Count ) 
			{
				return false;
			}
			else 
			{
				foreach( object obj in _set ) 
				{
					object oldValue = snapshot[ obj ];
					if ( oldValue==null || elementType.IsDirty( oldValue, obj, session)) 
					{
						return false;
					}
				}
			}

			return true;
		}

		public Set(ISessionImplementor session) : base(session) { }

		/// <summary>
		/// Creates a new Set initialized to the values in the Map.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="map"></param>
		/// <remarks>
		/// Only call this constructor if you consider the map initialized.
		/// </remarks>
		public Set(ISessionImplementor session, Iesi.Collections.ISet collection) : base(session) 
		{
			_set = collection;
			initialized = true;
			directlyAccessible = true;
		}

		public Set(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) 
			: base(session) 
		{
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;
			for (int i=0; i<array.Length; i+=2)
			{
				_set.Add( persister.ElementType.Assemble( array[i], session, owner ) );
			}
			initialized = true;
		}

		/// <summary>
		/// <see cref="PersistentCollection.BeforeInitialize"/>
		/// </summary>
		public override void BeforeInitialize(CollectionPersister persister) {
			
			if(persister.HasOrdering) 
			{
				_set = new Iesi.Collections.ListSet();
			}
			else 
			{
				_set = new Iesi.Collections.HashedSet();
			}
		}
		
		

		#region System.Collections.ICollection Members
        
		/// <summary>
		/// <see cref="ICollection.CopyTo"/>
		/// </summary>
		public override void CopyTo(System.Array array, int index) 
		{
			Read();
			_set.CopyTo( array, index );
		}

		/// <summary>
		/// <see cref="ICollection.Count"/>
		/// </summary>
		public override int Count 
		{
			get 
			{
				Read();
				return _set.Count;
			}
		}

		/// <summary>
		/// <see cref="ICollection.IsSynchronized"/>
		/// </summary>
		public override bool IsSynchronized 
		{
			get { return false; }
		}

		/// <summary>
		/// <see cref="ICollection.SyncRoot"/>
		/// </summary>
		public override object SyncRoot 
		{
			get { return this; }
		}

		#endregion

		#region Iesi.Collections.ISet Memebers

		public bool Add(object value) 
		{
			Write();
			return _set.Add( value );
		}
		
		public void Clear() 
		{
			Write();
			_set.Clear();
		}

		public bool Contains(object key) 
		{
			Read();
			return _set.Contains( key );
		}

		public bool ContainsAll(ICollection c) 
		{
			Read();
			return _set.ContainsAll( c );
		}
		
		public Iesi.Collections.ISet ExclusiveOr(Iesi.Collections.ISet a) 
		{
			Read();
			return _set.ExclusiveOr( a );
		}

		public Iesi.Collections.ISet Intersect(Iesi.Collections.ISet a) 
		{
			Read();
			return _set.Intersect( a );
		}

		public bool IsEmpty 
		{
			get 
			{
				Read();
				return _set.IsEmpty;
			}
		}

		public Iesi.Collections.ISet Minus(Iesi.Collections.ISet a) 
		{
			Read();
			return _set.Minus( a );
		}

		public bool Remove(object key) 
		{
			Write();
			return _set.Remove( key );
		}

		public bool RemoveAll(ICollection c) 
		{
			Write();
			return _set.RemoveAll( c );
		}

		public bool RetainAll(ICollection c) 
		{
			Write();
			return _set.RetainAll( c );
		}

		public Iesi.Collections.ISet Union(Iesi.Collections.ISet a) 
		{
			Read();
			return _set.Union( a );
		}

		#endregion

//		/// <summary>
//		/// <see cref="IDictionary.IsFixedSize"/>
//		/// </summary>
//		public bool IsFixedSize 
//		{
//			get { return false; }
//		}
//
//		/// <summary>
//		/// <see cref="IDictionary.IsReadOnly"/>
//		/// </summary>
//		public bool IsReadOnly 
//		{
//			get { return false; }
//		}

		#region System.Collections.IEnumerable Members

		/// <summary>
		/// <see cref="IEnumerable.GetEnumerator"/>
		/// </summary>
		public override IEnumerator GetEnumerator() 
		{
			Read();
			return _set.GetEnumerator();
		}

		#endregion

		
		#region System.Collections.ICloneable Members
		
		public object Clone() 
		{
			Read();
			return _set.Clone();
		}

		#endregion

		/// <summary>
		/// <see cref="PersistentCollection.Elements"/>
		/// </summary>
		public override ICollection Elements() 
		{
			return _set;
		}

		/// <summary>
		/// <see cref="PersistentCollection.Empty"/>
		/// </summary>
		public override bool Empty 
		{
			get { return _set.Count==0; }
		}
		
		public override string ToString() 
		{
			Read();
			return _set.ToString();
		}

		
		/// <summary>
		/// <see cref="PersistentCollection.WriteTo"/>
		/// </summary>
		public override void WriteTo(IDbCommand st, CollectionPersister persister, object entry, int i, bool writeOrder) 
		{
			persister.WriteElement(st, entry, writeOrder, session);
		}
		
		/// <summary>
		/// <see cref="PersistentCollection.ReadFrom"/>
		/// </summary>
		public override object ReadFrom(IDataReader rs, CollectionPersister persister, object owner) 
		{
			//object elementIdentifier = persister.ReadElement(rs, owner, session);
			object elementIdentifier = persister.ReadElementIdentifier(rs, owner, session);

			tempIdentifierList.Add(elementIdentifier);
			return elementIdentifier;
		}

		/// <summary>
		/// Set up the temporary Identifier List that will be used in the EndRead() 
		/// to resolve the Identifier to an Entity.
		/// <see cref="PersistentCollection.BeginRead"/>
		/// </summary>
		public override void BeginRead() 
		{
			tempIdentifierList = new ArrayList();
		}

		/// <summary>
		/// Resolves all of the Identifiers to an Entity.
		/// <see cref="PersistentCollection.BeginRead"/>
		/// </summary>
		public override void EndRead(CollectionPersister persister, object owner) 
		{
			foreach(object identifier in tempIdentifierList) 
			{
				object element = persister.ElementType.ResolveIdentifier(identifier, session, owner);
				_set.Add( element );
			}
				
		}

		/// <summary>
		/// <see cref="PersistentCollection.Entries"/>
		/// </summary>
		public override ICollection Entries() 
		{
			return _set;
		}


		/// <summary>
		/// <see cref="PersistentCollection.Disassemble"/>
		/// </summary>
		public override object Disassemble(CollectionPersister persister) 
		{
			object[] result = new object[ _set.Count ];
			int i=0;

			foreach( object obj in _set ) 
			{
				result[i++] = persister.ElementType.Disassemble( obj, session );
			}
			return result;
		}

		/// <summary>
		/// <see cref="PersistentCollection.GetDeletes"/>
		/// </summary>
		public override ICollection GetDeletes(IType elemType) 
		{
			IList deletes = new ArrayList();
			IDictionary snapshot = (IDictionary)GetSnapshot();

			foreach(DictionaryEntry e in snapshot) 
			{
				object test = e.Key;
				
				if( _set.Contains( test )==false ) 
				{
					deletes.Add( test );
				}

			}

			foreach(object obj in _set) 
			{
				//object testKey = e.Key;
				object oldKey = snapshot[ obj ];

				if( oldKey!=null && elemType.IsDirty( obj, oldKey, session ) ) 
				{
					deletes.Add( obj );
				}
			}

			return deletes;
			
		}

		/// <summary>
		/// <see cref="PersistentCollection.NeedsInserting"/>
		/// </summary>
		public override bool NeedsInserting(object entry, int i, IType elemType) 
		{
			IDictionary sn = (IDictionary) GetSnapshot();
			object oldKey = sn[entry];
			// note that it might be better to iterate the snapshot but this is safe,
			// assuming the user implements equals() properly, as required by the Set
			// contract!
			return oldKey==null || elemType.IsDirty(oldKey, entry, session);

		}

		/// <summary>
		/// <see cref="PersistentCollection.NeedsUpdating"/>
		/// </summary>
		public override bool NeedsUpdating(object entry, int i, IType elemType) 
		{
			return false;
		}

		/// <summary>
		/// <see cref="PersistentCollection.GetIndex"/>
		/// </summary>
		public override object GetIndex(object entry, int i) 
		{
			throw new NotImplementedException("Sets don't have indexes");
		}

		/// <summary>
		/// <see cref="PersistentCollection.EntryExists"/>
		/// </summary>
		public override bool EntryExists(object entry, int i) 
		{
			//TODO: find out where this is used...
			return true;
		}


		
	}
}

