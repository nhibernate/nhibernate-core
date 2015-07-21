using System;
using NHibernate.Collection;
using NHibernate.Persister.Collection;

namespace NHibernate.Event
{
	/// <summary> An event that occurs before a collection is recreated </summary>
	[Serializable]
	public class PreCollectionRecreateEvent : AbstractCollectionEvent
	{
		public PreCollectionRecreateEvent(ICollectionPersister collectionPersister, IPersistentCollection collection,
		                                  IEventSource source)
			: base(collectionPersister, collection, source, collection.Owner, GetOwnerIdOrNull(collection.Owner, source)) {}
	}
}