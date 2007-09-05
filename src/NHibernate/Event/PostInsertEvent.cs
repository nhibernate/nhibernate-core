using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs after inserting an item in the datastore 
	/// </summary>
	[Serializable]
	public class PostInsertEvent : AbstractEvent
	{
		private readonly object entity;
		private readonly object id;
		private readonly object[] state;
		private readonly IEntityPersister persister;

		public PostInsertEvent(object entity, object id, object[] state, IEntityPersister persister, IEventSource source)
			: base(source)
		{
			this.entity = entity;
			this.id = id;
			this.state = state;
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

		public IEntityPersister Persister
		{
			get { return persister; }
		}
	}
}
