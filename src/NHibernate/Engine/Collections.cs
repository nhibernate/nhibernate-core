using log4net;
using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Engine
{
	public static class Collections
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Collections));

		/// <summary> 
		/// Record the fact that this collection was dereferenced 
		/// </summary>
		/// <param name="coll">The collection to be updated by unreachability. </param>
		/// <param name="session">The session.</param>
		public static void ProcessUnreachableCollection(IPersistentCollection coll, ISessionImplementor session)
		{
			if (coll.Owner == null)
			{
				ProcessNeverReferencedCollection(coll, session);
			}
			else
			{
				ProcessDereferencedCollection(coll, session);
			}
		}

		private static void ProcessNeverReferencedCollection(IPersistentCollection coll, ISessionImplementor session)
		{
			CollectionEntry entry = session.GetCollectionEntry(coll);

			log.Debug("Found collection with unloaded owner: " + MessageHelper.InfoString(entry.LoadedPersister, entry.LoadedKey, session.Factory));

			entry.CurrentPersister = entry.LoadedPersister;
			entry.CurrentKey = entry.LoadedKey;

			PrepareCollectionForUpdate(coll, entry);
		}

		private static void ProcessDereferencedCollection(IPersistentCollection coll, ISessionImplementor session)
		{
			CollectionEntry entry = session.GetCollectionEntry(coll);
			ICollectionPersister loadedPersister = entry.LoadedPersister;

			if (log.IsDebugEnabled && loadedPersister != null)
				log.Debug("Collection dereferenced: " + MessageHelper.InfoString(loadedPersister, entry.LoadedKey, session.Factory));

			// do a check
			bool hasOrphanDelete = loadedPersister != null && loadedPersister.HasOrphanDelete;
			if (hasOrphanDelete)
			{
				object ownerId = loadedPersister.OwnerEntityPersister.GetIdentifier(coll.Owner);
				//if (ownerId == null)
				//{
				//  // the owning entity may have been deleted and its identifier unset due to
				//  // identifier-rollback; in which case, try to look up its identifier from
				//  // the persistence context
				//  if (session.Factory.Settings.IsIdentifierRollbackEnabled)
				//  {
				//    EntityEntry ownerEntry = session.GetEntry(coll.Owner);
				//    if (ownerEntry != null)
				//    {
				//      ownerId = ownerEntry.Id;
				//    }
				//  }
				//  if (ownerId == null)
				//  {
				//    throw new AssertionFailure("Unable to determine collection owner identifier for orphan-delete processing");
				//  }
				//}
				EntityKey key = new EntityKey(ownerId, loadedPersister.OwnerEntityPersister);
				object owner = session.GetEntity(key);
				if (owner == null)
				{
					throw new AssertionFailure("collection owner not associated with session: " + loadedPersister.Role);
				}
				EntityEntry e = session.GetEntry(owner);
				//only collections belonging to deleted entities are allowed to be dereferenced in the case of orphan delete
				if (e != null && e.Status != Impl.Status.Deleted && e.Status != Impl.Status.Gone)
				{
					throw new HibernateException("A collection with cascade=\"all-delete-orphan\" was no longer referenced by the owning entity instance: " + loadedPersister.Role);
				}
			}

			// do the work
			entry.CurrentPersister = null;
			entry.CurrentKey = null;
			PrepareCollectionForUpdate(coll, entry);
		}

		 //1. record the collection role that this collection is referenced by
		 //2. decide if the collection needs deleting/creating/updating (but don't actually schedule the action yet)
		private static void PrepareCollectionForUpdate(IPersistentCollection coll, CollectionEntry entry)
		{
			if (entry.IsProcessed)
				throw new AssertionFailure("collection was processed twice by flush()");

			entry.IsProcessed = true;

			ICollectionPersister loadedPersister = entry.LoadedPersister;
			ICollectionPersister currentPersister = entry.CurrentPersister;
			if (loadedPersister != null || currentPersister != null)
			{
				// it is or was referenced _somewhere_
				bool ownerChanged = loadedPersister != currentPersister || 
					!currentPersister.KeyType.Equals(entry.LoadedKey, entry.CurrentKey);

				if (ownerChanged)
				{
					// do a check
					bool orphanDeleteAndRoleChanged = loadedPersister != null && 
						currentPersister != null && loadedPersister.HasOrphanDelete;

					if (orphanDeleteAndRoleChanged)
					{
						throw new HibernateException("Don't change the reference to a collection with cascade=\"all-delete-orphan\": " + loadedPersister.Role);
					}

					// do the work
					if (currentPersister != null)
					{
						entry.IsDorecreate = true; // we will need to create new entries
					}

					if (loadedPersister != null)
					{
						entry.IsDoremove = true; // we will need to remove ye olde entries
						if (entry.IsDorecreate)
						{
							log.Debug("Forcing collection initialization");
							coll.ForceInitialization(); // force initialize!
						}
					}
				}
				else if (coll.IsDirty)
				{
					// else if it's elements changed
					entry.IsDoupdate = true;
				}
			}
		}

		/// <summary> 
		/// Initialize the role of the collection. 
		/// </summary>
		/// <param name="collection">The collection to be updated by reachibility. </param>
		/// <param name="type">The type of the collection. </param>
		/// <param name="entity">The owner of the collection. </param>
		/// <param name="session">The session.</param>
		public static void ProcessReachableCollection(IPersistentCollection collection, CollectionType type, object entity, ISessionImplementor session)
		{
			collection.Owner = entity;
			CollectionEntry ce = session.GetCollectionEntry(collection);

			if (ce == null)
			{
				// refer to comment in StatefulPersistenceContext.addCollection()
				throw new HibernateException(string.Format("Found two representations of same collection: {0}", type.Role));
			}

			// The CollectionEntry.isReached() stuff is just to detect any silly users  
			// who set up circular or shared references between/to collections.
			if (ce.IsReached)
			{
				// We've been here before
				throw new HibernateException(string.Format("Found shared references to a collection: {0}", type.Role));
			}
			ce.IsReached = true;

			ISessionFactoryImplementor factory = session.Factory;
			ICollectionPersister persister = factory.GetCollectionPersister(type.Role);
			ce.CurrentPersister = persister;
			ce.CurrentKey = type.GetKeyOfOwner(entity, session); //TODO: better to pass the id in as an argument?

			if (log.IsDebugEnabled)
			{
				log.Debug("Collection found: " + 
					MessageHelper.InfoString(persister, ce.CurrentKey, factory) + ", was: " + 
					MessageHelper.InfoString(ce.LoadedPersister, ce.LoadedKey, factory) + 
					(collection.WasInitialized ? " (initialized)" : " (uninitialized)"));
			}

			PrepareCollectionForUpdate(collection, ce);
		}
	}
}
