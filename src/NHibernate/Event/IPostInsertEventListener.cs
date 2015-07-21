namespace NHibernate.Event
{
	/// <summary> Called after inserting an item in the datastore </summary>
	public interface IPostInsertEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		void OnPostInsert(PostInsertEvent @event);
	}
}
