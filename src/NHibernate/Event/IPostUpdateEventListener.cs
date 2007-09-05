namespace NHibernate.Event
{
	/// <summary>
	/// Called after updating the datastore
	/// </summary>
	public interface IPostUpdateEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="theEvent"></param>
		void OnPostUpdate(PostUpdateEvent theEvent);
	}
}