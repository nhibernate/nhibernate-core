namespace NHibernate.Event
{
	/// <summary>
	/// Called before updating the datastore
	/// </summary>
	public partial interface IPreUpdateEventListener
	{
		/// <summary> Return true if the operation should be vetoed</summary>
		/// <param name="event"></param>
		bool OnPreUpdate(PreUpdateEvent @event);
	}
}