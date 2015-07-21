namespace NHibernate.Event
{
	/// <summary> Called after updating a collection </summary>
	public interface IPostCollectionUpdateEventListener
	{
		void OnPostUpdateCollection(PostCollectionUpdateEvent @event);
	}
}