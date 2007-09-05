namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of replicate events generated from a session.
	/// </summary>
	public interface IReplicateEventListener
	{
		/// <summary>Handle the given replicate event. </summary>
		/// <param name="theEvent">The replicate event to be handled.</param>
		void OnReplicate(ReplicateEvent theEvent);
	}
}