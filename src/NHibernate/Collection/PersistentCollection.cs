using System;
using System.Collections;
using System.Data;
using Iesi.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection
{
	/// <summary>
	/// Persistent collections are treated as value objects by Hibernate.
	/// i.e. they have no independent existence beyond the object holding
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
		private ISessionImplementor session;

		private bool initialized;

		[NonSerialized]
		private ArrayList additions;

		private ICollectionSnapshot collectionSnapshot;

		/// <summary></summary>
		[NonSerialized]
		private bool directlyAccessible;

		[NonSerialized]
		private bool initializing;

		//careful: these methods do not initialize the collection
		
		/// <summary></summary>
		public abstract ICollection Elements();
		
		/// <summary></summary>
		public abstract bool Empty { get; }

		/// <summary>
		/// Called by any read-only method of the collection interface
		/// </summary>
		public void Read()
		{
			Initialize( false );
		}

		/// <summary></summary>
		protected ISessionImplementor Session
		{
			get { return session; }
		}

		/// <summary>
		/// Is the collection currently connected to an open session?
		/// </summary>
		private bool IsConnectedToSession
		{
			get { return session != null && session.IsOpen; }
		}

		/// <summary></summary>
		protected bool DirectlyAccessible
		{
			set { directlyAccessible = value; }
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
		/// Queue an addition
		/// </summary>
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
		/// <param name="coll"></param>
		public virtual void DelayedAddAll( ICollection coll )
		{
			throw new AssertionFailure( "Collection does not support delayed initialization." );
		}

		/// <summary>
		/// Gets or Sets an <see cref="ArrayList"/> of objects that have been placed in the Queue 
		/// to be added.
		/// </summary>
		/// <value>An <see cref="ArrayList"/> of objects or null.</value>
		protected ArrayList Additions
		{
			get { return additions; }
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
		/// Not called by Hibernate, but used by non-NET serialization, eg. SOAP libraries.
		/// </summary>
		public PersistentCollection()
		{
		}

		/// <summary>
		/// Not called by Hibernate, but used by non-NET serialization, eg. SOAP libraries.
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
		/// </remarks>
		public virtual bool EndRead()
		{
			SetInitialized();
			//do this bit after setting initialized to true or it will recurse
			if ( additions != null ) 
			{
				DelayedAddAll( additions );
				additions = null;
				return false;
			}
			else 
			{
				return true;
			}
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
		/// Mark the collection as initialized.
		/// </summary>
		protected void SetInitialized()
		{
			initializing = false;
			initialized = true;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="role"></param>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="writeOrder"></param>
		public abstract void WriteTo( IDbCommand st, ICollectionPersister role, object entry, int i, bool writeOrder );

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
		protected abstract ICollection Snapshot( ICollectionPersister persister );

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
		/// <param name="oldElements"></param>
		/// <param name="currentElements"></param>
		/// <param name="session"></param>
		/// <returns></returns>
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