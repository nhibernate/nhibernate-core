using System.Collections;

namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of create events generated from a session.
	/// </summary>
	public partial interface IPersistEventListener
	{
		/// <summary> Handle the given create event.</summary>
		/// <param name="event">The create event to be handled.</param>
		void OnPersist(PersistEvent @event);

		/// <summary> Handle the given create event. </summary>
		/// <param name="event">The create event to be handled.</param>
		/// <param name="createdAlready"></param>
		void OnPersist(PersistEvent @event, IDictionary createdAlready);
	}
}