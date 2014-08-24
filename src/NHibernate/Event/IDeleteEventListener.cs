using System.Collections.Generic;

namespace NHibernate.Event
{
	/// <summary> Defines the contract for handling of deletion events generated from a session. </summary>
	public interface IDeleteEventListener
	{
		/// <summary>Handle the given delete event. </summary>
		/// <param name="event">The delete event to be handled. </param>
		void OnDelete(DeleteEvent @event);

		void OnDelete(DeleteEvent @event, ISet<object> transientEntities);
	}
}