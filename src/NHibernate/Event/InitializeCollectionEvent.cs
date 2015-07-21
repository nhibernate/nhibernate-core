using System;
using NHibernate.Collection;

namespace NHibernate.Event
{
	/// <summary> 
	/// An event that occurs when a collection wants to be initialized
	/// </summary>
	[Serializable]
	public class InitializeCollectionEvent : AbstractCollectionEvent
	{
		public InitializeCollectionEvent(IPersistentCollection collection, IEventSource source)
			: base(
				GetLoadedCollectionPersister(collection, source), collection, source, GetLoadedOwnerOrNull(collection, source),
				GetLoadedOwnerIdOrNull(collection, source)) {}
	}
}