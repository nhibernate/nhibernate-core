namespace NHibernate.Event
{
	/// <summary> Called before recreating a collection </summary>
	public interface IPreCollectionRecreateEventListener
	{
		void OnPreRecreateCollection(PreCollectionRecreateEvent @event);
	}
}