namespace NHibernate.Event
{
	/// <summary> Called after recreating a collection </summary>
	public interface IPostCollectionRecreateEventListener
	{
		void OnPostRecreateCollection(PostCollectionRecreateEvent @event);
	}
}