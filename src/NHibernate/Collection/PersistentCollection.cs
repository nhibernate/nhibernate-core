using System;
using System.Collections;
using System.Data;
using log4net;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// Persistent collections are treated as value objects by Hibernate.
	/// ie. they have no independent existence beyond the object holding
	/// a reference to them. Unlike instances of entity classes, they are
	/// automatically deleted when unreferenced and automatically become
	/// persistent when held by a persistent object. Collections can be
	/// passed between different objects (change "roles") and this might
	/// cause their elements to move from one database table to another.
	/// <br /><br />
	/// Hibernate "wraps" a java collection in an instance of
	/// PersistentCollection. This mechanism is designed to support
	/// tracking of changes to the collection's persistent state and
	/// lazy instantiation of collection elements. The downside is that
	/// only certain abstract collection types are supported and any
	/// extra semantics are lost.
	/// <br /><br />
	/// Applications should <b>never</b> use classes in this package 
	/// directly, unless extending the "framework" here.
	/// <br /><br />
	/// Changes to <b>structure</b> of the collection are recorded by the
	/// collection calling back to the session. Changes to mutable
	/// elements (ie. composite elements) are discovered by cloning their
	/// state when the collection is initialized and comparing at flush
	/// time.
	/// 
	/// </summary>
	[Serializable]
	public abstract class PersistentCollection : ICollection
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( PersistentCollection ) );

		/// <summary></summary>
		[NonSerialized]
		protected ISessionImplementor session;

		/// <summary></summary>
		protected bool initialized;
		private bool initializing;

		[NonSerialized]
		private ArrayList additions;

		private ICollectionSnapshot collectionSnapshot;

		/// <summary></summary>
		[NonSerialized]
		protected bool directlyAccessible;

		//careful: these methods do not initialize the collection
		
		/// <summary></summary>
		public abstract ICollection Elements();
		
		/// <summary></summary>
		public abstract bool Empty { get; }

		/// <summary></summary>
		public void Read()
		{
			Initialize( false );
		}

		private bool IsConnectedToSession
		{
			get { return session != null && session.IsOpen; }
		}

		/// <summary></summary>
		protected void Write()
		{
			Initialize( true );
			if( IsConnectedToSession )
			{
				session.Dirty( this );
			}
			else
			{
				collectionSnapshot.SetDirty();
			}
		}

		private bool MayQueueAdd
		{
			get
			{
				return
					!initialized &&
						IsConnectedToSession &&
						session.IsInverseCollection( this );
			}
		}

		/// <summary></summary>
		protected bool QueueAdd( object element )
		{
			if( MayQueueAdd )
			{
				if( additions == null )
				{
					additions = new ArrayList( 10 );
				}
				additions.Add( element );
				session.Dirty( this ); //needed so that we remove this collection from the JCS cache
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
		protected bool QueueAddAll( ICollection coll )
		{
			if( MayQueueAdd )
			{
				if( additions == null )
				{
					additions = new ArrayList( 20 );
				}
				additions.AddRange( coll );
				return true;
			}
			else
			{
				return false;
			}
		}

		//TODO: H2.0.3 new method
		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		public virtual void DelayedAddAll( ICollection coll )
		{
			throw new AssertionFailure( "Collection does not support delayed initialization." );
		}

		// TODO: h2.0.3 synhc - I don't see AddAll in the H code...
		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public virtual bool AddAll( ICollection coll )
		{
			throw new AssertionFailure( "Collection does not support delayed initialization" );
		}

		/// <summary>
		/// Gets or Sets an <see cref="ArrayList"/> of objects that have been placed in the Queue 
		/// to be added.
		/// </summary>
		/// <value>An <see cref="ArrayList"/> of objects or null.</value>
		protected ArrayList Additions
		{
			get { return additions; }
			set { additions = value; }
		}

		/// <summary>
		/// Clears out any Queued Additions.
		/// </summary>
		/// <remarks>
		/// After a Flush() the database is in synch with the in-memory
		/// contents of the Collection.  Since everything is in synch remove
		/// any Queued Additions.
		/// </remarks>
		public virtual void PostFlush()
		{
			if( additions != null )
			{
				additions.Clear();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		protected PersistentCollection( ISessionImplementor session )
		{
			this.session = session;
		}

		/// <summary>
		/// Return the user-visible collection (or array) instance
		/// </summary>
		/// <returns>
		/// By default, the NHibernate wrapper is an acceptable collection for
		/// the end user code to work with because it is interface compatible.
		/// An NHibernate List is an IList, an NHibernate Map is an IDictionary
		/// and those are the types user code is expecting.
		/// </returns>
		public virtual object GetValue()
		{
			return this;
		}

		/*
		/// <summary></summary>
		public virtual object GetCachedValue()
		{
			initialized = true; //TODO: only needed for query FETCH so should move out of here
			return this;
		}
		*/

		/// <summary>
		/// Override on some subclasses
		/// </summary>
		public virtual void BeginRead()
		{
			initializing = true;
		}

		/// <summary>
		/// Called when the reading the Collection from the database is finished.
		/// </summary>
		/// <remarks>
		/// <p>
		/// This should be overridden by sub collections that use temporary collections
		/// to store values read from the db.
		/// </p>
		/// <p>
		/// This is also responsible for determining if the collection is cacheable by
		/// setting the property <see cref="IsCacheable"/>.  When NH is synched with h2.1 
		/// this method will be changed to return that bool instead of setting the property,
		/// but this accomplishes the same thing until then.
		/// </p>
		/// </remarks>
		public virtual void EndRead()
		{
			// TODO:SYNCH:hib2.1 has this return a bool
			SetInitialized();
			//do this bit after setting initialized to true or it will recurse
			if (additions!=null) 
			{
				DelayedAddAll(additions);
				additions=null;
				// can't cache the collection because it contains additions that are 
				// not in the database - those additions can't be put in the cache
				// because they might throw TransientObjectExceptions when attempting
				// to get their database identifier.
				IsCacheable = false;
				//return false;
			}
			else 
			{
				// nothing happened that would prevent this collection from being safe
				// to cache.
				IsCacheable = true;
				//return true;
			}
		}

		private bool _isCacheable;

		/// <summary>
		/// Gets or sets a boolean indicating if this collection can be put into the 
		/// cache.
		/// </summary>
		/// <remarks>
		/// This is a method new to NHibernate and is in here until the Loader design
		/// can get synched up with h2.1's loader design.  In h2.1 the <c>EndRead()</c>
		/// method returns a bool that is used by the ISession to determine wether or
		/// not the Collection can go in cache.  Right now, the only thing that can keep 
		/// a collection from being cached is if Delayed Adds are supported - ie, adding
		/// items to the collection without fully initializing it.
		/// </remarks>
		internal bool IsCacheable
		{
			get { return _isCacheable; }
			set { _isCacheable = value; }
		}

		/// <summary>
		/// Initialize the collection, if possible, wrapping any exceptions in a runtime exception
		/// </summary>
		/// <param name="writing"></param>
		protected void Initialize( bool writing )
		{
			if( !initialized )
			{
				if( initializing ) throw new LazyInitializationException("cannot access loading collection");
				if( IsConnectedToSession )
				{
					if( session.IsConnected )
					{
						try
						{
							session.InitializeCollection( this, writing );
						}
						catch( Exception e )
						{
							log.Error( "Failed to lazily initialize a collection", e );
							throw new LazyInitializationException( "Failed to lazily initialize a collection", e );
						}
					}
					else
					{
						throw new LazyInitializationException( "Failed to lazily initialize a collection - session is disconnected" );
					}
				}
				else
				{
					throw new LazyInitializationException( "Failed to lazily initialize a collection - no session" );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public bool UnsetSession( ISessionImplementor session )
		{
			if( session == this.session )
			{
				this.session = null;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public bool SetCurrentSession( ISessionImplementor session )
		{
			if( session == this.session )
			{
				return false;
			}
			else
			{
				if( IsConnectedToSession )
				{
					throw new HibernateException( "Illegal attempt to associate a collection with two open sessions" );
				}
				else
				{
					this.session = session;
					return true;
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the underlying collection is directly
		/// accessable through code.
		/// </summary>
		/// <value>
		/// <c>true</c> if we are not guaranteed that the NHibernate collection wrapper
		/// is being used.
		/// </value>
		/// <remarks>
		/// This is typically <c>false</c> whenever a transient object that contains a collection is being
		/// associated with an ISession through <c>Save</c> or <c>SaveOrUpdate</c>.  NHibernate can't guarantee
		/// that it will know about all operations that would call cause NHibernate's collections to call
		/// <c>Read()</c> or <c>Write()</c>.
		/// </remarks>
		public virtual bool IsDirectlyAccessible
		{
			get { return directlyAccessible; }
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual bool IsArrayHolder
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public abstract ICollection Entries();

		/// <summary>
		/// Read the state of the collection from a disassembled cached value.
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="disassembled"></param>
		/// <param name="owner"></param>
		public abstract void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner );

		
		/// <summary>
		/// Reads the elements Identifier from the reader.
		/// </summary>
		/// <param name="reader">The IDataReader that contains the value of the Identifier</param>
		/// <param name="role">The persister for this Collection.</param>
		/// <param name="owner">The owner of this Collection.</param>
		/// <returns>The value of the Identifier.</returns>
		public abstract object ReadFrom( IDataReader reader, ICollectionPersister role, object owner );

		/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="role"></param>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="writeOrder"></param>
		public abstract void WriteTo( IDbCommand st, ICollectionPersister role, object entry, int i, bool writeOrder );
		*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="role"></param>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="writeOrder"></param>
		/// <remarks>This is the 2.1 version, should be abstract, but trying to avoid lots of cascading changes</remarks>
		public virtual void WriteTo( IDbCommand st, ICollectionPersister role, object entry, int i, bool writeOrder )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public abstract bool IsWrapper( object collection );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract object GetIndex( object entry, int i );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		public abstract void BeforeInitialize( ICollectionPersister persister );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public abstract bool EqualsSnapshot( IType elementType );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected abstract object Snapshot( ICollectionPersister persister );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		public abstract object Disassemble( ICollectionPersister persister );

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the rows for this collection
		/// need to be recreated in the table.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns>
		/// <c>false</c> by default since most collections can determine which rows need to be
		/// individually updated/inserted/deleted.  Currently only <see cref="Bag"/>'s for <c>many-to-many</c>
		/// need to be recreated.
		/// </returns>
		public virtual bool NeedsRecreate( ICollectionPersister persister )
		{
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		public object GetSnapshot( ICollectionPersister persister )
		{
			return ( persister == null ) ? null : Snapshot( persister );
		}

		/// <summary>
		/// To be called internally by the session, forcing
		/// immediate initalization.
		/// </summary>
		/// <remarks>
		/// This method is similar to <see cref="Initialize" />, except that different exceptions are thrown.
		/// </remarks>
		public void ForceInitialization()
		{
			if( initializing ) throw new AssertionFailure("force initialize loading collection");
			if( session == null ) throw new HibernateException("collection is not associated with any session");
			if( !session.IsConnected ) throw new HibernateException("disconnected session");
			if( !initialized ) session.InitializeCollection(this, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract bool EntryExists( object entry, int i );
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public abstract bool NeedsInserting( object entry, int i, IType elemType );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public abstract bool NeedsUpdating( object entry, int i, IType elemType );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public abstract ICollection GetDeletes( IType elemType );

		
		/// <summary></summary>
		protected object GetSnapshot()
		{
			return session.GetSnapshot( this );
		}

		/// <summary></summary>
		public bool WasInitialized
		{
			get { return initialized; }
		}

		/// <summary></summary>
		public bool HasQueuedAdds
		{
			get { return additions != null; }
		}

		/// <summary></summary>
		public ICollection QueuedAddsCollection
		{
			get { return additions; }
		}

		/// <summary></summary>
		public virtual ICollectionSnapshot CollectionSnapshot
		{
			get { return collectionSnapshot; }
			set { collectionSnapshot = value; }
		}

		// looks like it is used by IdentifierBag
		/// <summary>
		/// By default, no operation is performed.  This provides a hook to get an identifer of the
		/// collection row for <see cref="IdentifierBag"/>.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <param name="entry">
		/// The entry to preInsert.  If this is a Map this will be a DictionaryEntry.  If this is
		/// a List then it will be the object at that index.
		/// </param>
		/// <param name="i">The index of the Entry while enumerating through the Collection.</param>
		public virtual void PreInsert( ICollectionPersister persister, object entry, int i )
		{
		}

		/// <summary>
		/// Called before inserting rows, to ensure that any surrogate keys are fully generated
		/// </summary>
		/// <param name="persister"></param>
		public virtual void PreInsert( ICollectionPersister persister )
		{
		}

		/// <summary>
		/// Called after inserting a row, to fetch the natively generated id
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		public virtual void AfterRowInsert( ICollectionPersister persister, object entry, int i )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="snapshot"></param>
		/// <returns></returns>
		public abstract ICollection GetOrphans( object snapshot );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="collection"></param>
		/// <param name="session"></param>
		public static void IdentityRemoveAll( IList list, ICollection collection, ISessionImplementor session )
		{
			IEnumerator enumer = collection.GetEnumerator();
			while( enumer.MoveNext() )
			{
				PersistentCollection.IdentityRemove( list, enumer.Current, session );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public static void IdentityRemove( IList list, object obj, ISessionImplementor session )
		{
			int indexOfEntityToRemove = -1;

			if( session.IsSaved( obj ) )
			{
				object idOfCurrent = session.GetEntityIdentifierIfNotUnsaved( obj );
				for( int i = 0; i < list.Count; i++ )
				{
					object idOfOld = session.GetEntityIdentifierIfNotUnsaved( list[ i ] );
					if( idOfCurrent.Equals( idOfOld ) )
					{
						// in hibernate this used the Iterator to remove the item - since in .NET
						// the Enumerator is read only we have to change the implementation slightly. 
						indexOfEntityToRemove = i;
						break;
					}
				}

				if( indexOfEntityToRemove != -1 )
				{
					list.RemoveAt( indexOfEntityToRemove );
				}
			}
		}

		/// <summary>
		/// Mark the collection as initialized.
		/// </summary>
		protected void SetInitialized()
		{
			initializing = false;
			initialized = true;
		}

		#region - Hibernate Collection Proxy Classes

		/*
		 * These were needed by Hibernate because Java's collections provide methods
		 * to get sublists, modify a collection with an iterator - all things that 
		 * Hibernate needs to be made aware of.  If .net changes their collection interfaces
		 * then we can readd these back in.
		 */

		#endregion

		#region ICollection Members

		/// <summary></summary>
		public abstract bool IsSynchronized { get; }

		/// <summary></summary>
		public abstract int Count { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public abstract void CopyTo( Array array, int index );

		/// <summary></summary>
		public abstract object SyncRoot { get; }

		#endregion

		#region IEnumerable Members

		/// <summary></summary>
		public abstract IEnumerator GetEnumerator();

		#endregion
	}


}