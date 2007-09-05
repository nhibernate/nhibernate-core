namespace NHibernate.Event
{
	/// <summary> Defines the contract for handling of evict events generated from a session. </summary>
	public interface IEvictEventListener
	{
		/// <summary> Handle the given evict event. </summary>
		/// <param name="theEvent">The evict event to be handled.</param>
		void OnEvict(EvictEvent theEvent);
	}
}