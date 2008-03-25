using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs after deleting an item from the datastore 
	/// </summary>
	[Serializable]
	public class PostDeleteEvent : AbstractEvent
	{
		private readonly object entity;
		private readonly object id;
		private readonly IEntityPersister persister;
		private readonly object[] deletedState;

		public PostDeleteEvent(object entity, object id, object[] deletedState, IEntityPersister persister, IEventSource source)
			: base(source)
		{
			this.entity = entity;
			this.id = id;
			this.persister = persister;
			this.deletedState = deletedState;
		}

		public object Entity
		{
			get { return entity; }
		}

		public object Id
		{
			get { return id; }
		}

		public IEntityPersister Persister
		{
			get { return persister; }
		}

		public object[] DeletedState
		{
			get { return deletedState; }
		}
	}
}
