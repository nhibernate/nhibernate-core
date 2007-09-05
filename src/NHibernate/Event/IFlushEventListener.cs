namespace NHibernate.Event
{
	/// <summary> Defines the contract for handling of session flush events. </summary>
	public interface IFlushEventListener
	{
		/// <summary>Handle the given flush event. </summary>
		/// <param name="theEvent">The flush event to be handled.</param>
		void OnFlush(FlushEvent theEvent);
	}
}