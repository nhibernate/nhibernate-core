using System.Collections;

namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of create events generated from a session.
	/// </summary>
	public interface IPersistEventListener
	{
		/// <summary> Handle the given create event.</summary>
		/// <param name="theEvent">The create event to be handled.</param>
		void OnPersist(PersistEvent theEvent);

		/// <summary> Handle the given create event. </summary>
		/// <param name="theEvent">The create event to be handled.</param>
		/// <param name="createdAlready"></param>
		void OnPersist(PersistEvent theEvent, IDictionary createdAlready);
	}
}