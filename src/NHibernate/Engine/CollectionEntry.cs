using System;
using System.Collections;
using log4net;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Engine
{
	/// <summary>
	/// We need an entry to tell us all about the current state
	/// of a collection with respect to its persistent state
	/// </summary>
	[Serializable]
	public class CollectionEntry : ICollectionSnapshot
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(CollectionEntry));

		/// <summary>
		/// Indicates that the Collection can still be reached by an Entity
		/// that exist in the <see cref="ISession"/>.
		/// </summary>
		/// <remarks>
		/// It is also used to ensure that the Collection is not shared between
		/// two Entities.  
		/// </remarks>
		[NonSerialized]
		private bool reached;

		/// <summary>
		/// Indicates that the Collection has been processed and is ready
		/// to have its state synchronized with the database.
		/// </summary>
		[NonSerialized]
		private bool processed;

		/// <summary>
		/// Indicates that a Collection needs to be updated.
		/// </summary>
		/// <remarks>
		/// A Collection needs to be updated whenever the contents of the Collection
		/// have been changed. 
		/// </remarks>
		[NonSerialized]
		private bool doupdate;

		/// <summary>
		/// Indicates that a Collection has old elements that need to be removed.
		/// </summary>
		/// <remarks>
		/// A Collection needs to have removals performed whenever its role changes or
		/// the key changes and it has a loadedPersister - ie - it was loaded by NHibernate.
		/// </remarks>
		[NonSerialized]
		private bool doremove;

		/// <summary>
		/// Indicates that a Collection needs to be recreated.
		/// </summary>
		/// <remarks>
		/// A Collection needs to be recreated whenever its role changes
		/// or the owner changes.
		/// </remarks>
		[NonSerialized]
		private bool dorecreate;

		/// <summary>
		/// If we instantiate a collection during the <see cref="ISession.Flush" />
		/// process, we must ignore it for the rest of the flush.
		/// </summary>
		[NonSerialized]
		private bool ignore;

		// <summary>
		// Indicates that the Collection has been fully initialized.
		// </summary>
		//private bool initialized;

		// For the fields below, "current" means the reference that was found
		// during Flush(), and "loaded" means the reference that is consistent
		// with the current database state

		/// <summary>
		/// The <see cref="ICollectionPersister"/> that is currently responsible
		/// for the Collection.
		/// </summary>
		/// <remarks>
		/// This is set when NHibernate is updating a reachable or an
		/// unreachable collection.
		/// </remarks>
		[NonSerialized]
		private ICollectionPersister currentPersister;

		[NonSerialized]
		private object currentKey;

		/// <summary>
		/// The <see cref="ICollectionPersister"/> when the Collection was loaded.
		/// </summary>
		/// <remarks>
		/// This can be <see langword="null" /> if the Collection was not loaded by NHibernate and 
		/// was passed in along with a transient object.
		/// </remarks>
		[NonSerialized]
		private ICollectionPersister loadedPersister;

		/// <summary>
		/// The identifier of the Entity that is the owner of this Collection 
		/// during the load or post flush.
		/// </summary>
		private object loadedKey;

		/// <summary>session-start/post-flush persistent state</summary>
		private ICollection snapshot;

		/// <summary>allow the snapshot to be serialized</summary>
		private string role;

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/>.
		/// </summary>
		/// <remarks> 
		/// The CollectionEntry is for a Collection that is not dirty and 
		/// has already been initialized.
		/// </remarks>
		public CollectionEntry(IPersistentCollection collection)
		{
			// New collections that get found and wrapped during flush shouldn't be ignored
			ignore = false;

			//a newly wrapped collection is NOT dirty (or we get unnecessary version updates)
			collection.ClearDirty();
			//this.initialized = true;
		}

		public CollectionEntry(ICollectionPersister loadedPersister, object loadedID)
			// Detached collection wrappers that get found and reattached
			// during flush shouldn't be ignored
			: this(loadedPersister, loadedID, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/> for collections just loaded from the database.
		/// </summary>
		/// <param name="loadedPersister">The <see cref="ICollectionPersister"/> that persists this Collection type.</param>
		/// <param name="loadedID">The identifier of the Entity that is the owner of this Collection.</param>
		/// <param name="ignore">A boolean indicating whether to ignore the collection during current (or next) flush.</param>
		public CollectionEntry(ICollectionPersister loadedPersister, object loadedID, bool ignore)
		{
			//this.dirty = false;
			//this.initialized = false;
			loadedKey = loadedID;
			SetLoadedPersister(loadedPersister);
			this.ignore = ignore;
		}

		/// <summary> For collections just loaded from the database</summary>
		public CollectionEntry(IPersistentCollection collection, ICollectionPersister loadedPersister, object loadedKey, bool ignore)
		{
			this.ignore = ignore;

			//collection.clearDirty()

			this.loadedKey = loadedKey;
			SetLoadedPersister(loadedPersister);
			snapshot = null;
			collection.CollectionSnapshot = this;
			//postInitialize() will be called after initialization
		}

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/> for initialized detached collections.
		/// </summary>
		/// <param name="cs">The <see cref="ICollectionSnapshot"/> from another <see cref="ISession"/>.</param>
		/// <param name="factory">The <see cref="ISessionFactoryImplementor"/> that created this <see cref="ISession"/>.</param>
		/// <remarks>
		/// This takes an <see cref="ICollectionSnapshot"/> from another <see cref="ISession"/> and 
		/// creates an entry for it in this <see cref="ISession"/> by copying the values from the 
		/// <c>cs</c> parameter.
		/// </remarks>
		public CollectionEntry(ICollectionSnapshot cs, ISessionFactoryImplementor factory)
		{
			//this.dirty = cs.Dirty;
			snapshot = cs.Snapshot;
			loadedKey = cs.Key;
			//this.initialized = true;
			// Detached collections that get found and reattached during flush
			// shouldn't be ignored
			ignore = false;
			SetLoadedPersister(factory.GetCollectionPersister(cs.Role));
		}

		/// <summary> 
		/// Determine if the collection is "really" dirty, by checking dirtiness
		/// of the collection elements, if necessary
		/// </summary>
		private void Dirty(IPersistentCollection collection)
		{
			// if the collection is initialized and it was previously persistent
			// initialize the dirty flag
			bool forceDirty = collection.WasInitialized && !collection.IsDirty && LoadedPersister != null
			                  && LoadedPersister.IsMutable
			                  && (collection.IsDirectlyAccessible || LoadedPersister.ElementType.IsMutable)
			                  && !collection.EqualsSnapshot(LoadedPersister);

			if (forceDirty)
			{
				collection.Dirty();
			}
		}

		/// <summary>
		/// Prepares this CollectionEntry for the Flush process.
		/// </summary>
		/// <param name="collection">The <see cref="IPersistentCollection"/> that this CollectionEntry will be responsible for flushing.</param>
		public void PreFlush(IPersistentCollection collection)
		{
			Dirty(collection);

			if (log.IsDebugEnabled && collection.IsDirty && loadedPersister != null)
			{
				log.Debug("Collection dirty: " + MessageHelper.InfoString(loadedPersister, loadedKey));
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
		/// Updates the CollectionEntry to reflect that the <see cref="IPersistentCollection"/>
		/// has been initialized.
		/// </summary>
		/// <param name="collection">The initialized <see cref="AbstractPersistentCollection"/> that this Entry is for.</param>
		public void PostInitialize(IPersistentCollection collection)
		{
			snapshot = collection.GetSnapshot(loadedPersister);
		}

		/// <summary>
		/// Updates the CollectionEntry to reflect that it is has been successfully flushed to the database.
		/// </summary>
		/// <param name="collection">The <see cref="IPersistentCollection"/> that was flushed.</param>
		/// <remarks>
		/// Called after a <em>successful</em> flush.
		/// </remarks>
		public bool PostFlush(IPersistentCollection collection)
		{
			if (ignore)
			{
				ignore = false;
			}
			else
			{
				// the CollectionEntry should be processed if we are in the PostFlush()
				if (!processed)
				{
					throw new AssertionFailure("collection was not processed by Flush()");
				}
			}

			return loadedPersister == null;
		}

		#region Engine.ICollectionSnapshot Members

		public void InitSnapshot(IPersistentCollection collection, ICollectionPersister persister)
		{
			snapshot = collection.GetSnapshot(persister);
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
		internal void SetLoadedPersister(ICollectionPersister persister)
		{
			loadedPersister = persister;
			role = (persister == null) ? null : persister.Role;
		}

		public bool IsSnapshotEmpty(IPersistentCollection collection)
		{
			return collection.WasInitialized &&
			       collection.IsSnapshotEmpty(Snapshot);
		}

		public bool IsReached
		{
			get { return reached; }
			set { reached = value; }
		}

		public bool IsIgnore
		{
			get { return ignore; }
		}

		public bool IsDorecreate
		{
			get { return dorecreate; }
			set { dorecreate = value; }
		}

		public bool IsDoremove
		{
			get { return doremove; }
			set { doremove = value; }
		}

		public bool IsDoupdate
		{
			get { return doupdate; }
			set { doupdate = value; }
		}

		public ICollectionPersister CurrentPersister
		{
			get { return currentPersister; }
			set { currentPersister = value; }
		}

		public object CurrentKey
		{
			get { return currentKey; }
			set { currentKey = value; }
		}

		public ICollectionPersister LoadedPersister
		{
			get { return loadedPersister; }
		}

		public object LoadedKey
		{
			get { return loadedKey; }
		}

		public bool IsProcessed
		{
			get { return processed; }
			set { processed = value; }
		}

		public ICollection GetOrphans(string entityName, IPersistentCollection collection)
		{
			if (snapshot == null)
			{
				throw new AssertionFailure("no collection snapshot for orphan delete");
			}
			return collection.GetOrphans(snapshot, entityName);
		}

		public void AfterAction(IPersistentCollection collection)
		{
			loadedKey = CurrentKey;
			SetLoadedPersister(CurrentPersister);

			bool resnapshot = collection.WasInitialized &&
			                  (IsDoremove || IsDorecreate || IsDoupdate);
			if (resnapshot)
			{
				snapshot = loadedPersister == null
				           	? //|| !loadedPersister.IsMutable ? 
				           null
				           	:
				           		collection.GetSnapshot(loadedPersister); //re-snapshot
			}

			collection.PostAction();
		}
	}
}