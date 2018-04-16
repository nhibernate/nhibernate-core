namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of replicate events generated from a session.
	/// </summary>
	public partial interface IReplicateEventListener
	{
		/// <summary>Handle the given replicate event. </summary>
		/// <param name="event">The replicate event to be handled.</param>
		void OnReplicate(ReplicateEvent @event);
	}
}