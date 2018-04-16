namespace NHibernate.Event
{
	/// <summary> Called after removing a collection </summary>
	public partial interface IPostCollectionRemoveEventListener
	{
		void OnPostRemoveCollection(PostCollectionRemoveEvent @event);
	}
}