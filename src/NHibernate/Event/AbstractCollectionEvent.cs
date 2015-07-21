using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Event
{
	/// <summary> Defines a base class for events involving collections. </summary>
	[Serializable]
	public abstract class AbstractCollectionEvent : AbstractEvent
	{
		private readonly object affectedOwner;
		private readonly string affectedOwnerEntityName;
		private readonly object affectedOwnerId;
		private readonly IPersistentCollection collection;

		/// <summary> Constructs an AbstractCollectionEvent object. </summary>
		/// <param name="collectionPersister">The collection persister.</param>
		/// <param name="collection">The collection </param>
		/// <param name="source">The Session source </param>
		/// <param name="affectedOwner">The owner that is affected by this event; can be null if unavailable </param>
		/// <param name="affectedOwnerId">
		/// The ID for the owner that is affected by this event; can be null if unavailable
		/// that is affected by this event; can be null if unavailable
		/// </param>
		protected AbstractCollectionEvent(ICollectionPersister collectionPersister, IPersistentCollection collection,
		                               IEventSource source, object affectedOwner, object affectedOwnerId) : base(source)
		{
			this.collection = collection;
			this.affectedOwner = affectedOwner;
			this.affectedOwnerId = affectedOwnerId;
			affectedOwnerEntityName = GetAffectedOwnerEntityName(collectionPersister, affectedOwner, source);
		}

		public IPersistentCollection Collection
		{
			get { return collection; }
		}

		/// <summary> The collection owner entity that is affected by this event. </summary>
		/// <value> 
		/// Returns null if the entity is not in the persistence context
		/// (e.g., because the collection from a detached entity was moved to a new owner)
		/// </value>
		public object AffectedOwnerOrNull
		{
			get { return affectedOwner; }
		}

		/// <summary> Get the ID for the collection owner entity that is affected by this event. </summary>
		/// <value>
		/// Returns null if the ID cannot be obtained
		/// from the collection's loaded key (e.g., a property-ref is used for the
		/// collection and does not include the entity's ID)
		/// </value>
		public object AffectedOwnerIdOrNull
		{
			get { return affectedOwnerId; }
		}

		protected static ICollectionPersister GetLoadedCollectionPersister(IPersistentCollection collection,
		                                                                   IEventSource source)
		{
			CollectionEntry ce = source.PersistenceContext.GetCollectionEntry(collection);
			return (ce == null ? null : ce.LoadedPersister);
		}

		protected static object GetLoadedOwnerOrNull(IPersistentCollection collection, IEventSource source)
		{
			return source.PersistenceContext.GetLoadedCollectionOwnerOrNull(collection);
		}

		protected static object GetLoadedOwnerIdOrNull(IPersistentCollection collection, IEventSource source)
		{
			return source.PersistenceContext.GetLoadedCollectionOwnerIdOrNull(collection);
		}

		protected static object GetOwnerIdOrNull(object owner, IEventSource source)
		{
			EntityEntry ownerEntry = source.PersistenceContext.GetEntry(owner);
			return (ownerEntry == null ? null : ownerEntry.Id);
		}

		protected static string GetAffectedOwnerEntityName(ICollectionPersister collectionPersister, object affectedOwner,
		                                                   IEventSource source)
		{
			// collectionPersister should not be null, but we don't want to throw
			// an exception if it is null
			string entityName = (collectionPersister == null ? null : collectionPersister.OwnerEntityPersister.EntityName);
			if (affectedOwner != null)
			{
				EntityEntry ee = source.PersistenceContext.GetEntry(affectedOwner);
				if (ee != null && ee.EntityName != null)
				{
					entityName = ee.EntityName;
				}
			}
			return entityName;
		}

		/// <summary> Get the entity name for the collection owner entity that is affected by this event. </summary>
		/// <returns> 
		/// The entity name; if the owner is not in the PersistenceContext, the
		/// returned value may be a superclass name, instead of the actual class name
		/// </returns>
		public virtual string GetAffectedOwnerEntityName()
		{
			return affectedOwnerEntityName;
		}
	}
}