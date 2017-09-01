namespace NHibernate.Event
{
	/// <summary> Called after updating a collection </summary>
	public partial interface IPostCollectionUpdateEventListener
	{
		void OnPostUpdateCollection(PostCollectionUpdateEvent @event);
	}
}