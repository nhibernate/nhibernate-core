using System;

namespace NHibernate.Event
{
	/// <summary> Defines an event class for the flushing of a session. </summary>
	[Serializable]
	public class FlushEvent : AbstractEvent
	{
		public FlushEvent(IEventSource source) : base(source) { }
	}
}