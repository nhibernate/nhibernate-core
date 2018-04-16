namespace NHibernate.Event
{
	/// <summary>
	/// Called after updating the datastore
	/// </summary>
	public partial interface IPostUpdateEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		void OnPostUpdate(PostUpdateEvent @event);
	}
}