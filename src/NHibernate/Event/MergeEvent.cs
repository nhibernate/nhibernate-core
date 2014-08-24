using System;
namespace NHibernate.Event
{
	/// <summary> 
	/// An event class for merge() and saveOrUpdateCopy()
	/// </summary>
	[Serializable]
	public class MergeEvent : AbstractEvent
	{
		private object original;
		private string entityName;
		private object requestedId;

		private object entity; // ported from H3.2 even if I don't understand why is unused in constructor
		private object result;

		public MergeEvent(object entity, IEventSource source)
			: base(source)
		{
			if (entity == null)
				throw new ArgumentNullException("entity", "attempt to create merge event with null entity");

			Original = entity;
		}

		public MergeEvent(string entityName, object original, IEventSource source)
			: this(original, source)
		{
			EntityName = entityName;
		}

		public MergeEvent(string entityName, object original, object id, IEventSource source)
			: this(entityName, original, source)
		{
			if (id == null)
				throw new ArgumentNullException("id", "attempt to create merge event with null identifier");

			RequestedId = id;
		}

		public object Original
		{
			get { return original; }
			set { original = value; }
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

		public object Entity
		{
			get { return entity; }
			set { entity = value; }
		}

		public object Result
		{
			get { return result; }
			set { result = value; }
		}
	}
}
