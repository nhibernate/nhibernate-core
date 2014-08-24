using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Event
{
	/// <summary>
	/// Defines an event class for the locking of an entity.
	/// </summary>
	[Serializable]
	public class LockEvent : AbstractEvent
	{
		private string entityName;
		private object entity;
		private LockMode lockMode;

		public LockEvent(object entity, LockMode lockMode, IEventSource source)
			: base(source)
		{
			this.Entity = entity;
			this.LockMode = lockMode;
		}

		public LockEvent(string entityName, object original, LockMode lockMode, IEventSource source)
			: this(original, lockMode, source)
		{
			this.EntityName = entityName;
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

		public LockMode LockMode
		{
			get { return lockMode; }
			set { lockMode = value; }
		}
	}
}
