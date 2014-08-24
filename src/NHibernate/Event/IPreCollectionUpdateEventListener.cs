namespace NHibernate.Event
{
	/// <summary> Called before updating a collection </summary>
	public interface IPreCollectionUpdateEventListener
	{
		void OnPreUpdateCollection(PreCollectionUpdateEvent @event);
	}
}