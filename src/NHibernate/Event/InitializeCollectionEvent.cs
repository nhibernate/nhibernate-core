using System;
using NHibernate.Collection;

namespace NHibernate.Event
{
	/// <summary> 
	/// An event that occurs when a collection wants to be initialized
	/// </summary>
	[Serializable]
	public class InitializeCollectionEvent : AbstractEvent
	{
		private readonly IPersistentCollection collection;

		public InitializeCollectionEvent(IPersistentCollection collection, IEventSource source)
			: base(source)
		{
			this.collection = collection;
		}

		public IPersistentCollection Collection
		{
			get { return collection; }
		}
	}
}