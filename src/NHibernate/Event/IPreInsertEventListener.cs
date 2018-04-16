namespace NHibernate.Event
{
	/// <summary>
	/// Called before inserting an item in the datastore
	/// </summary>
	public partial interface IPreInsertEventListener
	{
		/// <summary> Return true if the operation should be vetoed</summary>
		/// <param name="event"></param>
		bool OnPreInsert(PreInsertEvent @event);
	}
}