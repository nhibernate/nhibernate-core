using System;

namespace NHibernate.Event
{
	/// <summary> 
	/// Defines a base class for Session generated events.
	/// </summary>
	[Serializable]
	public class AbstractEvent : IDatabaseEventArgs
	{
		/// <summary> 
		/// Constructs an event from the given event session.
		/// </summary>
		/// <param name="source">The session event source. </param>
		public AbstractEvent(IEventSource source)
		{
			Session = source;
		}

		/// <summary> 
		/// Returns the session event source for this event.  
		/// This is the underlying session from which this event was generated.
		/// </summary>
		public IEventSource Session { get; private set; }
	}
}