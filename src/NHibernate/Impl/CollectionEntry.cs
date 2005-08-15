using System;
using System.Collections;
using log4net;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// We need an entry to tell us all about the current state
	/// of a collection with respect to its persistent state
	/// </summary>
	[Serializable]
	public class CollectionEntry : ICollectionSnapshot
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( CollectionEntry ) );

		/// <summary>
		/// Collections detect changes made via their public interface
		/// and mark themselves as dirty. False by default.
		/// </summary>
		private bool dirty;

		/// <summary>
		/// Indicates that the Collection can still be reached by an Entity
		/// that exist in the <see cref="ISession"/>.
		/// </summary>
		/// <remarks>
		/// It is also used to ensure that the Collection is not shared between
		/// two Entities.  
		/// </remarks>
		[NonSerialized]
		internal bool reached;

		/// <summary>
		/// Indicates that the Collection has been processed and is ready
		/// to have its state synchronized with the database.
		/// </summary>
		[NonSerialized]
		internal bool processed;

		/// <summary>
		/// Indicates that a Collection needs to be updated.
		/// </summary>
		/// <remarks>
		/// A Collection needs to be updated whenever the contents of the Collection
		/// have been changed. 
		/// </remarks>
		[NonSerialized]
		internal bool doupdate;

		/// <summary>
		/// Indicates that a Collection has old elements that need to be removed.
		/// </summary>
		/// <remarks>
		/// A Collection needs to have removals performed whenever its role changes or
		/// the key changes and it has a loadedPersister - ie - it was loaded by NHibernate.
		/// </remarks>
		[NonSerialized]
		internal bool doremove;

		/// <summary>
		/// Indicates that a Collection needs to be recreated.
		/// </summary>
		/// <remarks>
		/// A Collection needs to be recreated whenever its role changes
		/// or the owner changes.
		/// </remarks>
		[NonSerialized]
		internal bool dorecreate;

		/// <summary>
		/// If we instantiate a collection during the <see cref="ISession.Flush" />
		/// process, we must ignore it for the rest of the flush.
		/// </summary>
		[NonSerialized]
		internal bool ignore;

		/// <summary>
		/// Indicates that the Collection has been fully initialized.
		/// </summary>
		internal bool initialized;

		/// <summary>
		/// The <see cref="ICollectionPersister"/> that is currently responsible
		/// for the Collection.
		/// </summary>
		/// <remarks>
		/// This is set when NHibernate is updating a reachable or an
		/// unreachable collection.
		/// </remarks>
		[NonSerialized]
		internal ICollectionPersister currentPersister;

		/// <summary></summary>
		[NonSerialized]
		internal object currentKey;

		/// <summary>
		/// The <see cref="ICollectionPersister"/> when the Collection was loaded.
		/// </summary>
		/// <remarks>
		/// This can be <c>null</c> if the Collection was not loaded by NHibernate and 
		/// was passed in along with a transient object.
		/// </remarks>
		[NonSerialized]
		internal ICollectionPersister loadedPersister;

		/// <summary>
		/// The identifier of the Entity that is the owner of this Collection 
		/// during the load or post flush.
		/// </summary>
		internal object loadedKey;

		/// <summary>session-start/post-flush persistent state</summary>
		internal ICollection snapshot;

		/// <summary>allow the snapshot to be serialized</summary>
		internal string role;

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/>.
		/// </summary>
		/// <remarks> 
		/// The CollectionEntry is for a Collection that is not dirty and 
		/// has already been initialized.
		/// </remarks>
		public CollectionEntry()
		{
			//a newly wrapped collection is NOT dirty (or we get unnecessary version updates)
			this.dirty = false;
			// A newly wrapped collection is NOT dirty (or we get unnecessary version updates)
			//this.dirty = false;
			this.initialized = true;

			// New collections that get found and wrapped during flush shouldn't be ignored
			this.ignore = false;
		}

		public CollectionEntry( ICollectionPersister loadedPersister, object loadedID )
			// Detached collection wrappers that get found and reattached
			// during flush shouldn't be ignored
			: this( loadedPersister, loadedID, false )
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/>. 
		/// </summary>
		/// <param name="loadedPersister">The <see cref="ICollectionPersister"/> that persists this Collection type.</param>
		/// <param name="loadedID">The identifier of the Entity that is the owner of this Collection.</param>
		/// <param name="ignore">A boolean indicating whether to ignore the collection during current (or next) flush.</param>
		public CollectionEntry( ICollectionPersister loadedPersister, object loadedID, bool ignore )
		{
			this.dirty = false;
			this.initialized = false;
			this.loadedKey = loadedID;
			SetLoadedPersister( loadedPersister );
			this.ignore = ignore;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/>. 
		/// </summary>
		/// <param name="cs">The <see cref="ICollectionSnapshot"/> from another <see cref="ISession"/>.</param>
		/// <param name="factory">The <see cref="ISessionFactoryImplementor"/> that created this <see cref="ISession"/>.</param>
		/// <remarks>
		/// This takes an <see cref="ICollectionSnapshot"/> from another <see cref="ISession"/> and 
		/// creates an entry for it in this <see cref="ISession"/> by copying the values from the 
		/// <c>cs</c> parameter.
		/// </remarks>
		public CollectionEntry( ICollectionSnapshot cs, ISessionFactoryImplementor factory )
		{
			this.dirty = cs.Dirty;
			this.snapshot = cs.Snapshot;
			this.loadedKey = cs.Key;
			this.initialized = true;
			// Detached collections that get found and reattached during flush
			// shouldn't be ignored
			this.ignore = false;
			SetLoadedPersister( factory.GetCollectionPersister( cs.Role ) );
		}

		/// <summary>
		/// Checks to see if the <see cref="PersistentCollection"/> has had any changes to the 
		/// collections contents or if any of the elements in the collection have been modified.
		/// </summary>
		/// <param name="coll"></param>
		/// <returns><c>true</c> if the <see cref="PersistentCollection"/> is dirty.</returns>
		/// <remarks>
		/// default behavior; will be overridden in deep lazy collections
		/// </remarks>
		public virtual bool IsDirty( PersistentCollection coll )
		{
			// if this has already been marked as dirty or the collection can not 
			// be directly accessed (ie- we can guarantee that the NHibernate collection
			// wrappers are used) and the elements in the collection are not mutable 
			// then return the dirty flag.
			if( dirty || (
				!coll.IsDirectlyAccessible && !loadedPersister.ElementType.IsMutable
				) )
			{
				return dirty;
			}
			else
			{
				// need to have the coll determine if it is the same as the snapshot
				// that was last taken.
				return !coll.EqualsSnapshot( loadedPersister.ElementType );
			}
		}

		/// <summary>
		/// Prepares this CollectionEntry for the Flush process.
		/// </summary>
		/// <param name="collection">The <see cref="PersistentCollection"/> that this CollectionEntry will be responsible for flushing.</param>
		public void PreFlush( PersistentCollection collection )
		{
			// if the collection is initialized and it was previously persistent
			// initialize the dirty flag
			dirty = ( initialized && loadedPersister != null && IsDirty( collection ) ) ||
				( !initialized && dirty ); //only need this so collection with queued adds will be removed from second-level cache

			if( log.IsDebugEnabled && dirty && loadedPersister != null )
			{
				log.Debug( "Collection dirty: " + MessageHelper.InfoString( loadedPersister, loadedKey ) );
			}

			// reset all of these values so any previous flush status 
			// information is cleared from this CollectionEntry
			doupdate = false;
			doremove = false;
			dorecreate = false;
			reached = false;
			processed = false;
		}

		/// <summary>
		/// Updates the CollectionEntry to reflect that the <see cref="PersistentCollection"/>
		/// has been initialized.
		/// </summary>
		/// <param name="collection">The initialized <see cref="PersistentCollection"/> that this Entry is for.</param>
		public void PostInitialize( PersistentCollection collection )
		{
			initialized = true;
			snapshot = collection.GetSnapshot( loadedPersister );
		}

		/// <summary>
		/// Updates the CollectionEntry to reflect that it is has been successfully flushed to the database.
		/// </summary>
		/// <param name="collection">The <see cref="PersistentCollection"/> that was flushed.</param>
		public void PostFlush( PersistentCollection collection )
		{
			if( ignore )
			{
				ignore = false;
			}
			else
			{
				// the CollectionEntry should be processed if we are in the PostFlush()
				if( !processed )
				{
					throw new AssertionFailure( "collection was not processed by Flush()" );
				}

				// now that the flush has gone through move everything that is the current
				// over to the loaded fields and set dirty to false since the db & collection
				// are in synch.
				loadedKey = currentKey;
				SetLoadedPersister( currentPersister );
				dirty = false;

				// collection needs to know its' representation in memory and with
				// the db is now in synch - esp important for collections like a bag
				// that can add without initializing the collection.
				collection.PostFlush();

				// if it was initialized or any of the scheduled actions were performed then
				// need to resnapshot the contents of the collection.
				if( initialized && ( doremove || dorecreate || doupdate ) )
				{
					InitSnapshot( collection, loadedPersister );
				}
			}
		}

		#region Engine.ICollectionSnapshot Members

		internal void InitSnapshot(PersistentCollection collection, ICollectionPersister persister)
		{
			snapshot = collection.GetSnapshot( persister );
		}

		/// <summary></summary>
		public object Key
		{
			get { return loadedKey; }
		}

		/// <summary></summary>
		public string Role
		{
			get { return role; }
		}

		/// <summary></summary>
		public ICollection Snapshot
		{
			get { return snapshot; }
		}

		/// <summary></summary>
		public bool Dirty
		{
			get { return dirty; }
		}

		public void SetDirty()
		{
			dirty = true;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool WasDereferenced
		{
			get { return loadedKey == null; }
		}

		#endregion

		/// <summary>
		/// Sets the information in this CollectionEntry that is specific to the
		/// <see cref="ICollectionPersister"/>.
		/// </summary>
		/// <param name="persister">
		/// The <see cref="ICollectionPersister"/> that is 
		/// responsible for the Collection.
		/// </param>
		internal void SetLoadedPersister( ICollectionPersister persister )
		{
			loadedPersister = persister;
			if( persister != null )
			{
				role = persister.Role;
			}
		}

		/// <summary></summary>
		public bool SnapshotIsEmpty
		{
			get
			{
				return initialized && snapshot != null && snapshot.Count == 0;
			}
		}

		/// <summary></summary>
		public bool IsNew
		{
			// TODO: is this correct implementation
			get { return initialized && ( snapshot == null ); }
		}
	}
}