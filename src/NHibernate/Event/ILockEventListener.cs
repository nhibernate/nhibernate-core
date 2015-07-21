namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of lock events generated from a session.
	/// </summary>
	public interface ILockEventListener
	{
		/// <summary>Handle the given lock event. </summary>
		/// <param name="event">The lock event to be handled. </param>
		void OnLock(LockEvent @event);
	}
}