using System;

namespace NHibernate.Event
{
	/// <summary> An event class for persist() </summary>
	public class PersistEvent : AbstractEvent
	{
		private object entity;
		private string entityName;

		public PersistEvent(object entity, IEventSource source)
			: base(source)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity", "Attempt to create create event with null entity");
			}
			this.entity = entity;
		}

		public PersistEvent(string entityName, object original, IEventSource source)
			: this(original, source)
		{
			this.entityName = entityName;
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
	}
}