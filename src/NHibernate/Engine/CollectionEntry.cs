using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Engine
{
	/// <summary>
	/// We need an entry to tell us all about the current state
	/// of a collection with respect to its persistent state
	/// </summary>
	[Serializable]
	public class CollectionEntry
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof (CollectionEntry));

		/// <summary>session-start/post-flush persistent state</summary>
		private object snapshot;

		/// <summary>allow the snapshot to be serialized</summary>
		private string role;

		/// <summary>
		/// The <see cref="ICollectionPersister"/> when the Collection was loaded.
		/// </summary>
		/// <remarks>
		/// This can be <see langword="null" /> if the Collection was not loaded by NHibernate and 
		/// was passed in along with a transient object.
		/// </remarks>
		[NonSerialized] private ICollectionPersister loadedPersister;

		/// <summary>
		/// The identifier of the Entity that is the owner of this Collection 
		/// during the load or post flush.
		/// </summary>
		private object loadedKey;

		/// <summary>
		/// Indicates that the Collection can still be reached by an Entity
		/// that exist in the <see cref="ISession"/>.
		/// </summary>
		/// <remarks>
		/// It is also used to ensure that the Collection is not shared between
		/// two Entities.  
		/// </remarks>
		[NonSerialized] private bool reached;

		/// <summary>
		/// Indicates that the Collection has been processed and is ready
		/// to have its state synchronized with the database.
		/// </summary>
		[NonSerialized] private bool processed;

		/// <summary>
		/// Indicates that a Collection needs to be updated.
		/// </summary>
		/// <remarks>
		/// A Collection needs to be updated whenever the contents of the Collection
		/// have been changed. 
		/// </remarks>
		[NonSerialized] private bool doupdate;

		/// <summary>
		/// Indicates that a Collection has old elements that need to be removed.
		/// </summary>
		/// <remarks>
		/// A Collection needs to have removals performed whenever its role changes or
		/// the key changes and it has a loadedPersister - ie - it was loaded by NHibernate.
		/// </remarks>
		[NonSerialized] private bool doremove;

		/// <summary>
		/// Indicates that a Collection needs to be recreated.
		/// </summary>
		/// <remarks>
		/// A Collection needs to be recreated whenever its role changes
		/// or the owner changes.
		/// </remarks>
		[NonSerialized] private bool dorecreate;

		/// <summary>
		/// If we instantiate a collection during the <see cref="ISession.Flush" />
		/// process, we must ignore it for the rest of the flush.
		/// </summary>
		[NonSerialized] private bool ignore;

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
		[NonSerialized] private ICollectionPersister currentPersister;

		[NonSerialized] private object currentKey;

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/>.
		/// </summary>
		/// <remarks> 
		/// For newly wrapped collections, or dereferenced collection wrappers
		/// </remarks>
		public CollectionEntry(ICollectionPersister persister, IPersistentCollection collection)
		{
			// new collections that get found + wrapped
			// during flush shouldn't be ignored
			ignore = false;

			collection.ClearDirty(); //a newly wrapped collection is NOT dirty (or we get unnecessary version updates)

			snapshot = persister.IsMutable ? collection.GetSnapshot(persister) : null;
			collection.SetSnapshot(loadedKey, role, snapshot);
		}

		/// <summary> For collections just loaded from the database</summary>
		public CollectionEntry(IPersistentCollection collection, ICollectionPersister loadedPersister, object loadedKey,
		                       bool ignore)
		{
			this.ignore = ignore;
			this.loadedKey = loadedKey;
			SetLoadedPersister(loadedPersister);
			collection.SetSnapshot(loadedKey, role, null);
			//postInitialize() will be called after initialization
		}

		public CollectionEntry(ICollectionPersister loadedPersister, object loadedKey)
		{
			// detached collection wrappers that get found + reattached
			// during flush shouldn't be ignored
			ignore = false;

			//collection.clearDirty()

			this.loadedKey = loadedKey;
			SetLoadedPersister(loadedPersister);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionEntry"/> for initialized detached collections.
		/// </summary>
		/// <remarks>
		///  For initialized detached collections
		/// </remarks>
		internal CollectionEntry(IPersistentCollection collection, ISessionFactoryImplementor factory)
		{
			// detached collections that get found + reattached
			// during flush shouldn't be ignored
			ignore = false;

			loadedKey = collection.Key;
			SetLoadedPersister(factory.GetCollectionPersister(collection.Role));

			snapshot = collection.StoredSnapshot;
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
			set { role = value; }
		}

		/// <summary></summary>
		public object Snapshot
		{
			get { return snapshot; }
		}

		public bool IsReached
		{
			get { return reached; }
			set { reached = value; }
		}

		public bool IsProcessed
		{
			get { return processed; }
			set { processed = value; }
		}

		public bool IsDoupdate
		{
			get { return doupdate; }
			set { doupdate = value; }
		}

		public bool IsDoremove
		{
			get { return doremove; }
			set { doremove = value; }
		}

		public bool IsDorecreate
		{
			get { return dorecreate; }
			set { dorecreate = value; }
		}

		public bool IsIgnore
		{
			get { return ignore; }
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

		public object LoadedKey
		{
			get { return loadedKey; }
		}

		public ICollectionPersister LoadedPersister
		{
			get { return loadedPersister; }
		}

		public bool WasDereferenced
		{
			get { return loadedKey == null; }
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
			bool nonMutableChange = collection.IsDirty && LoadedPersister != null && !LoadedPersister.IsMutable;
			if (nonMutableChange)
			{
				throw new HibernateException("changed an immutable collection instance: " + MessageHelper.InfoString(LoadedPersister.Role, LoadedKey));
			}
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
			snapshot = LoadedPersister.IsMutable ? collection.GetSnapshot(LoadedPersister) : null;
			collection.SetSnapshot(loadedKey, role, snapshot);
		}

		/// <summary>
		/// Updates the CollectionEntry to reflect that it is has been successfully flushed to the database.
		/// </summary>
		/// <param name="collection">The <see cref="IPersistentCollection"/> that was flushed.</param>
		/// <remarks>
		/// Called after a <em>successful</em> flush.
		/// </remarks>
		public void PostFlush(IPersistentCollection collection)
		{
			if (IsIgnore)
			{
				ignore = false;
			}
			else if (!IsProcessed)
			{
				// the CollectionEntry should be processed if we are in the PostFlush()
				throw new AssertionFailure("collection [" + collection.Role + "] was not processed by flush()");
			}

			collection.SetSnapshot(loadedKey, role, snapshot);
		}

		public void AfterAction(IPersistentCollection collection)
		{
			loadedKey = CurrentKey;
			SetLoadedPersister(CurrentPersister);

			bool resnapshot = collection.WasInitialized && (IsDoremove || IsDorecreate || IsDoupdate);
			if (resnapshot)
			{
				//re-snapshot
				snapshot = loadedPersister == null || !loadedPersister.IsMutable ? null : collection.GetSnapshot(loadedPersister);
			}

			collection.PostAction();
		}

		/// <summary>
		/// Sets the information in this CollectionEntry that is specific to the
		/// <see cref="ICollectionPersister"/>.
		/// </summary>
		/// <param name="persister">
		/// The <see cref="ICollectionPersister"/> that is 
		/// responsible for the Collection.
		/// </param>
		private void SetLoadedPersister(ICollectionPersister persister)
		{
			loadedPersister = persister;
			Role = (persister == null) ? null : persister.Role;
		}

		internal void AfterDeserialize(ISessionFactoryImplementor factory)
		{
			loadedPersister = factory.GetCollectionPersister(role);
		}

		public ICollection GetOrphans(string entityName, IPersistentCollection collection)
		{
			if (snapshot == null)
			{
				throw new AssertionFailure("no collection snapshot for orphan delete");
			}
			return collection.GetOrphans(snapshot, entityName);
		}

		public bool IsSnapshotEmpty(IPersistentCollection collection)
		{
			return collection.WasInitialized && (LoadedPersister == null || LoadedPersister.IsMutable) && collection.IsSnapshotEmpty(Snapshot);
		}

		public override string ToString()
		{
			string result = "CollectionEntry" + MessageHelper.InfoString(loadedPersister.Role, loadedKey);
			if (currentPersister != null)
			{
				result += ("->" + MessageHelper.InfoString(currentPersister.Role, currentKey));
			}
			return result;
		}
	}
}