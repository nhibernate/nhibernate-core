using System;
using Iesi.Collections;

namespace NHibernate.Event
{
	/// <summary>Defines an event class for the auto-flushing of a session. </summary>
	[Serializable]
	public class AutoFlushEvent : FlushEvent
	{
		private ISet querySpaces;
		private bool flushRequired;

		public AutoFlushEvent(ISet querySpaces, IEventSource source)
			: base(source)
		{
			this.querySpaces = querySpaces;
		}

		public ISet QuerySpaces
		{
			get { return querySpaces; }
			set { querySpaces = value; }
		}

		public bool FlushRequired
		{
			get { return flushRequired; }
			set { flushRequired = value; }
		}
	}
}