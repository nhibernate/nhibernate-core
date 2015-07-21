using System;
using NHibernate.Collection;
using NHibernate.Persister.Collection;

namespace NHibernate.Event
{
	/// <summary> An event that occurs before a collection is updated </summary>
	[Serializable]
	public class PreCollectionUpdateEvent : AbstractCollectionEvent
	{
		public PreCollectionUpdateEvent(ICollectionPersister collectionPersister, IPersistentCollection collection,
		                                IEventSource source)
			: base(
				collectionPersister, collection, source, GetLoadedOwnerOrNull(collection, source),
				GetLoadedOwnerIdOrNull(collection, source)) {}
	}
}