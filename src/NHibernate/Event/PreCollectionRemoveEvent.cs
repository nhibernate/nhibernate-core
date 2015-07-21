using System;
using NHibernate.Collection;
using NHibernate.Persister.Collection;

namespace NHibernate.Event
{
	/// <summary> An event that occurs before a collection is removed </summary>
	[Serializable]
	public class PreCollectionRemoveEvent : AbstractCollectionEvent
	{
		public PreCollectionRemoveEvent(ICollectionPersister collectionPersister, IPersistentCollection collection,
		                                IEventSource source, object loadedOwner)
			: base(collectionPersister, collection, source, loadedOwner, GetOwnerIdOrNull(loadedOwner, source)) {}
	}
}