namespace NHibernate.Event
{
	/// <summary> Called after removing a collection </summary>
	public interface IPostCollectionRemoveEventListener
	{
		void OnPostRemoveCollection(PostCollectionRemoveEvent @event);
	}
}