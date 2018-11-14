﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Engine
{
	using System.Threading.Tasks;
	using System.Threading;
	public static partial class Collections
	{

		/// <summary> 
		/// Record the fact that this collection was dereferenced 
		/// </summary>
		/// <param name="coll">The collection to be updated by unreachability. </param>
		/// <param name="session">The session.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public static Task ProcessUnreachableCollectionAsync(IPersistentCollection coll, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				if (coll.Owner == null)
				{
					return ProcessNeverReferencedCollectionAsync(coll, session, cancellationToken);
				}
				else
				{
					return ProcessDereferencedCollectionAsync(coll, session, cancellationToken);
				}
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		private static Task ProcessDereferencedCollectionAsync(IPersistentCollection coll, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				IPersistenceContext persistenceContext = session.PersistenceContext;
				CollectionEntry entry = persistenceContext.GetCollectionEntry(coll);
				ICollectionPersister loadedPersister = entry.LoadedPersister;

				if (log.IsDebugEnabled() && loadedPersister != null)
					log.Debug("Collection dereferenced: {0}", MessageHelper.CollectionInfoString(loadedPersister, coll, entry.LoadedKey, session));

				// do a check
				bool hasOrphanDelete = loadedPersister != null && loadedPersister.HasOrphanDelete;
				if (hasOrphanDelete)
				{
					object ownerId = loadedPersister.OwnerEntityPersister.GetIdentifier(coll.Owner);
					// TODO NH Different behavior
					//if (ownerId == null)
					//{
					//  // the owning entity may have been deleted and its identifier unset due to
					//  // identifier-rollback; in which case, try to look up its identifier from
					//  // the persistence context
					//  if (session.Factory.Settings.IsIdentifierRollbackEnabled)
					//  {
					//    EntityEntry ownerEntry = persistenceContext.GetEntry(coll.Owner);
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
					EntityKey key = session.GenerateEntityKey(ownerId, loadedPersister.OwnerEntityPersister);
					object owner = persistenceContext.GetEntity(key);
					if (owner == null)
					{
						return Task.FromException<object>(new AssertionFailure("collection owner not associated with session: " + loadedPersister.Role));
					}
					EntityEntry e = persistenceContext.GetEntry(owner);
					//only collections belonging to deleted entities are allowed to be dereferenced in the case of orphan delete
					if (e != null && e.Status != Status.Deleted && e.Status != Status.Gone)
					{
						return Task.FromException<object>(new HibernateException("A collection with cascade=\"all-delete-orphan\" was no longer referenced by the owning entity instance: " + loadedPersister.Role));
					}
				}

				// do the work
				entry.CurrentPersister = null;
				entry.CurrentKey = null;
				return PrepareCollectionForUpdateAsync(coll, entry, session.Factory, cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		private static Task ProcessNeverReferencedCollectionAsync(IPersistentCollection coll, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(coll);

				log.Debug("Found collection with unloaded owner: {0}", MessageHelper.CollectionInfoString(entry.LoadedPersister, coll, entry.LoadedKey, session));

				entry.CurrentPersister = entry.LoadedPersister;
				entry.CurrentKey = entry.LoadedKey;

				return PrepareCollectionForUpdateAsync(coll, entry, session.Factory, cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary> 
		/// Initialize the role of the collection. 
		/// </summary>
		/// <param name="collection">The collection to be updated by reachability. </param>
		/// <param name="type">The type of the collection. </param>
		/// <param name="entity">The owner of the collection. </param>
		/// <param name="session">The session.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public static async Task ProcessReachableCollectionAsync(IPersistentCollection collection, CollectionType type, object entity, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			collection.Owner = entity;
			CollectionEntry ce = session.PersistenceContext.GetCollectionEntry(collection);

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
			ce.CurrentKey = await (type.GetKeyOfOwnerAsync(entity, session, cancellationToken)).ConfigureAwait(false); //TODO: better to pass the id in as an argument?

			if (log.IsDebugEnabled())
			{
				log.Debug("Collection found: {0}, was: {1}{2}",
				          MessageHelper.CollectionInfoString(persister, collection, ce.CurrentKey, session),
				          MessageHelper.CollectionInfoString(ce.LoadedPersister, collection, ce.LoadedKey, session),
				          (collection.WasInitialized ? " (initialized)" : " (uninitialized)"));
			}

			await (PrepareCollectionForUpdateAsync(collection, ce, factory, cancellationToken)).ConfigureAwait(false);
		}

		private static Task PrepareCollectionForUpdateAsync(IPersistentCollection collection, CollectionEntry entry, ISessionFactoryImplementor factory, CancellationToken cancellationToken)
		{
			//1. record the collection role that this collection is referenced by
			//2. decide if the collection needs deleting/creating/updating (but don't actually schedule the action yet)

			if (entry.IsProcessed)
				throw new AssertionFailure("collection was processed twice by flush()");
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{

				entry.IsProcessed = true;

				ICollectionPersister loadedPersister = entry.LoadedPersister;
				ICollectionPersister currentPersister = entry.CurrentPersister;
				if (loadedPersister != null || currentPersister != null)
				{
					// it is or was referenced _somewhere_
					bool ownerChanged = loadedPersister != currentPersister ||
					!currentPersister.KeyType.IsEqual(entry.LoadedKey, entry.CurrentKey, factory);

					if (ownerChanged)
					{
						// do a check
						bool orphanDeleteAndRoleChanged = loadedPersister != null && 
						currentPersister != null && loadedPersister.HasOrphanDelete;

						if (orphanDeleteAndRoleChanged)
						{
							return Task.FromException<object>(new HibernateException("Don't change the reference to a collection with cascade=\"all-delete-orphan\": " + loadedPersister.Role));
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
								return collection.ForceInitializationAsync(cancellationToken); // force initialize!
							}
						}
					}
					else if (collection.IsDirty)
					{
						// else if it's elements changed
						entry.IsDoupdate = true;
					}
				}
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
