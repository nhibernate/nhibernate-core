namespace NHibernate.Event
{
	/// <summary> Called after recreating a collection </summary>
	public partial interface IPostCollectionRecreateEventListener
	{
		void OnPostRecreateCollection(PostCollectionRecreateEvent @event);
	}
}