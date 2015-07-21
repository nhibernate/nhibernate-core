using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs after the datastore is updated
	/// </summary>
	[Serializable]
	public class PostUpdateEvent : AbstractPostDatabaseOperationEvent
	{
		public PostUpdateEvent(object entity, object id, object[] state, object[] oldState, IEntityPersister persister, IEventSource source)
			: base(source, entity, id, persister)
		{
			State = state;
			OldState = oldState;
		}

		public object[] State { get; private set; }

		public object[] OldState { get; private set; }
	}
}
