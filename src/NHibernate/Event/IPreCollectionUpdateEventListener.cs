namespace NHibernate.Event
{
	/// <summary> Called before updating a collection </summary>
	public partial interface IPreCollectionUpdateEventListener
	{
		void OnPreUpdateCollection(PreCollectionUpdateEvent @event);
	}
}