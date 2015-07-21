using System;
using NHibernate.Engine;

namespace NHibernate.Event
{
	/// <summary> 
	/// An event class for saveOrUpdate()
	/// </summary>
	[Serializable]
	public class SaveOrUpdateEvent : AbstractEvent
	{
		private object entity;
		private string entityName;
		private object requestedId;
		private object resultEntity;
		private EntityEntry entry;
		private object resultId;

		public SaveOrUpdateEvent(object entity, IEventSource source)
			: base(source)
		{
			if (entity == null)
				throw new ArgumentNullException("entity", "attempt to create saveOrUpdate event with null entity");

			this.entity = entity;
		}

		public SaveOrUpdateEvent(string entityName, object original, IEventSource source)
			: this(original, source)
		{
			this.entityName = entityName;
		}

		public SaveOrUpdateEvent(string entityName, object original, object id, IEventSource source)
			: this(entityName, original, source)
		{
			if (id == null)
				throw new ArgumentNullException("id", "attempt to create saveOrUpdate event with null identifier");

			requestedId = id;
		}

		public object Entity
		{
			get { return entity; }
			set { entity = value; }
		}

		public string EntityName
		{
			get { return entityName; }
			set { entityName = value; }
		}

		public object RequestedId
		{
			get { return requestedId; }
			set { requestedId = value; }
		}

		public object ResultEntity
		{
			get { return resultEntity; }
			set { resultEntity = value; }
		}

		public EntityEntry Entry
		{
			get { return entry; }
			set { entry = value; }
		}

		public object ResultId
		{
			get { return resultId; }
			set { resultId = value; }
		}
	}
}
