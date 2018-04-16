namespace NHibernate.Event
{
	/// <summary>
	/// Called before deleting an item from the datastore
	/// </summary>
	public partial interface IPreDeleteEventListener
	{
		/// <summary> Return true if the operation should be vetoed</summary>
		/// <param name="event"></param>
		bool OnPreDelete(PreDeleteEvent @event);
	}
}