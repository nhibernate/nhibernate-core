namespace NHibernate.Event
{
	/// <summary> Called after deleting an item from the datastore </summary>
	public interface IPostDeleteEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="theEvent"></param>
		void OnPostDelete(PostDeleteEvent theEvent);
	}
}