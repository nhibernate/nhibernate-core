using System;

namespace NHibernate.Event
{
	/// <summary>Defines an event class for the dirty-checking of a session. </summary>
	[Serializable]
	public class DirtyCheckEvent : FlushEvent
	{
		public DirtyCheckEvent(IEventSource source) : base(source) { }

		private bool dirty;

		public bool Dirty
		{
			get { return dirty; }
			set { dirty = value; }
		}
	}
}