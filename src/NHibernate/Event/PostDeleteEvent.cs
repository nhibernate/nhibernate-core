using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs after deleting an item from the datastore 
	/// </summary>
	[Serializable]
	public class PostDeleteEvent : AbstractPostDatabaseOperationEvent
	{
		public PostDeleteEvent(object entity, object id, object[] deletedState, IEntityPersister persister, IEventSource source)
			: base(source, entity, id, persister)
		{
			DeletedState = deletedState;
		}

		public object[] DeletedState { get; private set; }
	}
}
