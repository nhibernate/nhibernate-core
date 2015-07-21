using System;

namespace NHibernate.Event
{
	/// <summary>  Defines an event class for the evicting of an entity. </summary>
	[Serializable]
	public class EvictEvent : AbstractEvent
	{
		private object entity;

		public EvictEvent(object entity, IEventSource source)
			: base(source)
		{
			this.entity = entity;
		}

		public object Entity
		{
			get { return entity; }
			set { entity = value; }
		}
	}
}