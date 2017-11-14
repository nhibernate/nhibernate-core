namespace NHibernate.Event
{
	/// <summary> Called before recreating a collection </summary>
	public partial interface IPreCollectionRecreateEventListener
	{
		void OnPreRecreateCollection(PreCollectionRecreateEvent @event);
	}
}