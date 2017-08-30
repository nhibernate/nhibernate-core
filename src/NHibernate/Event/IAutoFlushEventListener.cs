namespace NHibernate.Event
{
	/// <summary> Defines the contract for handling of session auto-flush events. </summary>
	public partial interface IAutoFlushEventListener
	{
		/// <summary>
		/// Handle the given auto-flush event.
		/// </summary>
		/// <param name="event">The auto-flush event to be handled.</param>
		void OnAutoFlush(AutoFlushEvent @event);
	}
}