using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs after the datastore is updated
	/// </summary>
	[Serializable]
	public class PostUpdateEvent : AbstractEvent
	{
		private readonly object entity;
		private readonly object id;
		private readonly object[] state;
		private readonly object[] oldState;
		private readonly IEntityPersister persister;

		public PostUpdateEvent(object entity, object id, object[] state, object[] oldState, IEntityPersister persister, IEventSource source)
			: base(source)
		{
			this.entity = entity;
			this.id = id;
			this.state = state;
			this.oldState = oldState;
			this.persister = persister;
		}

		public object Entity
		{
			get { return entity; }
		}

		public object Id
		{
			get { return id; }
		}

		public object[] State
		{
			get { return state; }
		}

		public object[] OldState
		{
			get { return oldState; }
		}

		public IEntityPersister Persister
		{
			get { return persister; }
		}
	}
}
