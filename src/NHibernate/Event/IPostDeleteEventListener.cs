namespace NHibernate.Event
{
	/// <summary> Called after deleting an item from the datastore </summary>
	public interface IPostDeleteEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		void OnPostDelete(PostDeleteEvent @event);
	}
}