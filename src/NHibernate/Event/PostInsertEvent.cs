using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs after inserting an item in the datastore 
	/// </summary>
	[Serializable]
	public class PostInsertEvent : AbstractPostDatabaseOperationEvent
	{
		public PostInsertEvent(object entity, object id, object[] state, IEntityPersister persister, IEventSource source)
			: base(source, entity,id,persister)
		{
			State = state;
		}

		public object[] State { get; private set; }
	}
}
