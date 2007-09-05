using Iesi.Collections;

namespace NHibernate.Event
{
	/// <summary> Defines the contract for handling of deletion events generated from a session. </summary>
	public interface IDeleteEventListener
	{
		/// <summary>Handle the given delete event. </summary>
		/// <param name="theEvent">The delete event to be handled. </param>
		void OnDelete(DeleteEvent theEvent);

		void OnDelete(DeleteEvent theEvent, ISet transientEntities);
	}
}