namespace NHibernate.Event
{
	/// <summary> Called after deleting an item from the datastore </summary>
	public partial interface IPostDeleteEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		void OnPostDelete(PostDeleteEvent @event);
	}
}