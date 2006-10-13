using System;
using System.Collections;
using System.Data;
using Iesi.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// Base class for implementing <see cref="IPersistentCollection"/>.
	/// </summary>
	[Serializable]
	public abstract class AbstractPersistentCollection : IPersistentCollection //, ICollection
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( AbstractPersistentCollection ) );

		[NonSerialized]
		private ISessionImplementor session;

		private bool initialized;

		[NonSerialized]
		private ArrayList additions;

		private ICollectionSnapshot collectionSnapshot;

		[NonSerialized]
		private bool directlyAccessible;

		[NonSerialized]
		private bool initializing;

		//careful: these methods do not initialize the collection
		
		/// <summary>
		/// Is the initialized collection empty?
		/// </summary>
		public abstract bool Empty { get; }

		/// <summary>
		/// Called by any read-only method of the collection interface
		/// </summary>
		public void Read()
		{
			Initialize( false );
		}

		/// <summary>
		/// Is the collection currently connected to an open session?
		/// </summary>
		private bool IsConnectedToSession
		{
			get { return session != null && session.IsOpen && session.GetCollectionEntry(this) != null; }
		}

		/// <summary>
		/// Called by any writer method of the collection interface
		/// </summary>
		protected void Write()
		{
			Initialize( true );
			collectionSnapshot.SetDirty();
		}

		/// <summary>
		/// Is this collection in a state that would allow us to "queue" additions?
		/// </summary>
		private bool IsQueueAdditionEnabled
		{
			get 
			{
				return !initialized &&
					IsConnectedToSession &&
					session.IsInverseCollection( this );
			}
		}

		/// <summary>
		/// Queue an addition if the peristent collection supports it
		/// </summary>
		/// <returns>
		/// <c>true</c> if the addition was queued up, <c>false</c> if the persistent collection
		/// doesn't support Queued Addition.
		/// </returns>
		protected bool QueueAdd( object element )
		{
			if( IsQueueAdditionEnabled )
			{
				if( additions == null )
				{
					additions = new ArrayList( 10 );
				}
				additions.Add( element );
				collectionSnapshot.SetDirty(); //needed so that we remove this collection from the second-level cache
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Queue additions
		/// </summary>
		protected bool QueueAddAll( ICollection coll )
		{
			if( IsQueueAdditionEnabled )
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

		/// <summary>
		/// After reading all existing elements from the database,
		/// add the queued elements to the underlying collection.
		/// </summary>
		/// <param name="coll">The <see cref="ICollection"/> to add.</param>
		/// <remarks>
		/// The default implementation is to throw an <see cref="AssertionFailure"/>
		/// because most collections do not support delayed addition.  If the collection
		/// does then override this method.
		/// </remarks>
		public virtual void DelayedAddAll( ICollection coll, ICollectionPersister persister )
		{
			throw new AssertionFailure( "Collection does not support delayed initialization." );
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
				additions = null;
			}
		}

		/// <summary>
		/// Not called by Hibernate, but used by non-NET serialization, eg. SOAP libraries.
		/// </summary>
		public AbstractPersistentCollection()
		{
		}

		/// <summary>
		/// Not called by Hibernate, but used by non-NET serialization, eg. SOAP libraries.
		/// </summary>
		/// <param name="session"></param>
		protected AbstractPersistentCollection( ISessionImplementor session )
		{
			this.session = session;
		}

		/// <summary>
		/// Return the user-visible collection (or array) instance
		/// </summary>
		/// <returns>
		/// By default, the NHibernate wrapper is an acceptable collection for
		/// the end user code to work with because it is interface compatible.
		/// An NHibernate PersistentList is an IList, an NHibernate PersistentMap is an IDictionary
		/// and those are the types user code is expecting.
		/// </returns>
		public virtual object GetValue()
		{
			return this;
		}

		/// <summary>
		/// Called just before reading any rows from the <see cref="IDataReader" />
		/// </summary>
		public virtual void BeginRead()
		{
			// override on some subclasses
			initializing = true;
		}

		/// <summary>
		/// Called after reading all rows from the <see cref="IDataReader" />
		/// </summary>
		/// <remarks>
		/// This should be overridden by sub collections that use temporary collections
		/// to store values read from the db.
		/// </remarks>
		public virtual bool EndRead(ICollectionPersister persister)
		{
			// override on some subclasses
			return AfterInitialize(persister);
		}

		public virtual bool AfterInitialize(ICollectionPersister persister)
		{
			SetInitialized();
			//do this bit after setting initialized to true or it will recurse
			if ( additions != null ) 
			{
				DelayedAddAll( additions, persister );
				additions = null;
				return false;
			}
			else 
			{
				return true;
			}
		}

		/// <summary>
		/// Initialize the collection, if possible, wrapping any exceptions
		/// in a runtime exception
		/// </summary>
		/// <param name="writing">currently obsolete</param>
		/// <exception cref="LazyInitializationException">if we cannot initialize</exception>
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
		/// Mark the collection as initialized.
		/// </summary>
		protected void SetInitialized()
		{
			initializing = false;
			initialized = true;
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
			set { directlyAccessible = value; }
		}

		/// <summary>
		/// Disassociate this collection from the given session.
		/// </summary>
		/// <param name="session"></param>
		/// <returns>true if this was currently associated with the given session</returns>
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
		/// Associate the collection with the given session.
		/// </summary>
		/// <param name="session"></param>
		/// <returns>false if the collection was already associated with the session</returns>
		public bool SetCurrentSession( ISessionImplementor session )
		{
			if( session == this.session
				// NH: added to fix NH-704
			    && session.GetCollectionEntry(this) != null
				)
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
		/// Read the state of the collection from a disassembled cached value.
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="disassembled"></param>
		/// <param name="owner"></param>
		public abstract void InitializeFromCache( ICollectionPersister persister, object disassembled, object owner );

		/// <summary>
		/// Iterate all collection entries, during update of the database
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable Entries();

        /// <summary>
        /// Reads the row from the <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="reader">The IDataReader that contains the value of the Identifier</param>
        /// <param name="role">The persister for this Collection.</param>
		/// <param name="descriptor">The descriptor providing result set column names</param>
		/// <param name="owner">The owner of this Collection.</param>
        /// <returns>The object that was contained in the row.</returns>
        public abstract object ReadFrom(IDataReader reader, ICollectionPersister role, ICollectionAliases descriptor, object owner);

		/// <summary>
		/// Get the index of the given collection entry
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract object GetIndex( object entry, int i );

		public abstract object GetElement( object entry );
		
		public abstract object GetSnapshotElement( object entry, int i );

		/// <summary>
		/// Called before any elements are read into the collection,
		/// allowing appropriate initializations to occur.
		/// </summary>
		/// <param name="persister"></param>
		public abstract void BeforeInitialize( ICollectionPersister persister );

		/// <summary>
		/// Does the current state exactly match the snapshot?
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public abstract bool EqualsSnapshot( IType elementType );

		/// <summary>
		/// Return a new snapshot of the current state
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns></returns>
		protected abstract ICollection Snapshot( ICollectionPersister persister );

		/// <summary>
		/// Disassemble the collection, ready for the cache
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
		/// individually updated/inserted/deleted.  Currently only <see cref="PersistentBag"/>'s for <c>many-to-many</c>
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
		public ICollection GetSnapshot( ICollectionPersister persister )
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
			if (!initialized)
			{
				if( initializing ) throw new AssertionFailure("force initialize loading collection");
				if( session == null ) throw new HibernateException("collection is not associated with any session");
				if( !session.IsConnected ) throw new HibernateException("disconnected session");
				session.InitializeCollection( this, false );
			}
		}

		/// <summary>
		/// Does an element exist at this entry in the collection?
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract bool EntryExists( object entry, int i );
		
		/// <summary>
		/// Do we need to insert this element?
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public abstract bool NeedsInserting( object entry, int i, IType elemType );

		/// <summary>
		/// Do we need to update this element?
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public abstract bool NeedsUpdating( object entry, int i, IType elemType );

		/// <summary>
		/// Get all the elements that need deleting
		/// </summary>
		public abstract ICollection GetDeletes( IType elemType, bool indexIsFormula );

		/// <summary>
		/// Is this the wrapper for the given underlying collection instance?
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public abstract bool IsWrapper( object collection );
		
		/// <summary>
		/// Gets the Snapshot from the current session the collection 
		/// is in.
		/// </summary>
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
			get 
			{ 
				if ( HasQueuedAdds )
				{	
					return additions; 
				}
				else
				{
					return new ArrayList();
				}
			}
		}

		/// <summary></summary>
		public virtual ICollectionSnapshot CollectionSnapshot
		{
			get { return collectionSnapshot; }
			set { collectionSnapshot = value; }
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
		/// Get all "orphaned" elements
		/// </summary>
		/// <param name="snapshot"></param>
		/// <returns></returns>
		public abstract ICollection GetOrphans( object snapshot, System.Type entityName );

		public static void IdentityRemoveAll( IList list, ICollection collection, System.Type entityName, ISessionImplementor session )
		{
			IEnumerator enumer = collection.GetEnumerator();
			while( enumer.MoveNext() )
			{
				IdentityRemove( list, enumer.Current, entityName, session );
			}
		}

		protected static ICollection GetOrphans( ICollection oldElements, ICollection currentElements, ISessionImplementor session )
		{
			// short-circuit(s)
			if ( currentElements.Count == 0 )
			{
				// no new elements, the old list contains only Orphans
				return oldElements;
			}
			if ( oldElements.Count == 0 )
			{
				// no old elements, so no Orphans neither
				return oldElements;
			}

			// create the collection holding the orphans
			IList res = new ArrayList();

			// collect EntityIdentifier(s) of the *current* elements - add them into a HashSet for fast access
			ISet currentIds = new HashedSet();
			foreach ( object current in currentElements )
			{
				if ( session.IsSaved( current ) )
				{
					currentIds.Add( session.GetEntityIdentifierIfNotUnsaved( current ) );
				}
			}

			// iterate over the *old* list
			foreach ( object old in oldElements )
			{
				object id = session.GetEntityIdentifierIfNotUnsaved( old );
				if ( !currentIds.Contains( id ) )
				{
					res.Add( old );
				}
			}

			return res;
		}

		public virtual object GetIdentifier( object entry, int i )
		{
			throw new NotSupportedException();
		}

		public static void IdentityRemove( IList list, object obj, System.Type entityName, ISessionImplementor session )
		{
			int indexOfEntityToRemove = -1;
			
			IType idType = session.Factory.GetEntityPersister( entityName ).IdentifierType;

			if( session.IsSaved( obj ) )
			{
				object idOfCurrent = session.GetEntityIdentifierIfNotUnsaved( obj );
				for( int i = 0; i < list.Count; i++ )
				{
					object idOfOld = session.GetEntityIdentifierIfNotUnsaved( list[ i ] );
					if( idType.Equals( idOfOld, idOfCurrent ) )
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

		/// <summary></summary>
		protected ISessionImplementor Session
		{
			get { return session; }
		}

		public virtual bool IsSnapshotEmpty( ICollection snapshot )
		{
			return snapshot.Count == 0;
		}

		#region - Hibernate Collection Proxy Classes

		/*
		 * These were needed by Hibernate because Java's collections provide methods
		 * to get sublists, modify a collection with an iterator - all things that 
		 * Hibernate needs to be made aware of.  If .net changes their collection interfaces
		 * then we can readd these back in.
		 */

		#endregion

	}


}