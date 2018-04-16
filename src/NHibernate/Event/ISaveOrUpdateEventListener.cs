namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of update events generated from a session.
	/// </summary>
	public partial interface ISaveOrUpdateEventListener
	{
		/// <summary> Handle the given update event. </summary>
		/// <param name="event">The update event to be handled.</param>
		void OnSaveOrUpdate(SaveOrUpdateEvent @event);
	}
}