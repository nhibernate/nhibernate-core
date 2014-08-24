using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Event
{
	/// <summary>  
	/// Defines an event class for the replication of an entity.
	/// </summary>
	[Serializable]
	public class ReplicateEvent : AbstractEvent
	{
		private string entityName;
		private object entity;
		private ReplicationMode replicationMode;

		public ReplicateEvent(object entity, ReplicationMode replicationMode, IEventSource source)
			: this(null, entity, replicationMode, source)
		{}

		public ReplicateEvent(string entityName, object entity, ReplicationMode replicationMode, IEventSource source)
			: base(source)
		{
			if (entity == null)
				throw new ArgumentNullException("entity", "attempt to create replication strategy with null entity");

			if (replicationMode == null)
				throw new ArgumentNullException("replicationMode",
				                                "attempt to create replication strategy with null replication mode");

			this.entityName = entityName;
			this.entity = entity;
			this.replicationMode = replicationMode;
		}

		public string EntityName
		{
			get { return entityName; }
			set { entityName = value; }
		}

		public object Entity
		{
			get { return entity; }
			set { entity = value; }
		}

		public ReplicationMode ReplicationMode
		{
			get { return replicationMode; }
			set { replicationMode = value; }
		}
	}
}
