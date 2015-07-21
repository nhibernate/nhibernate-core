namespace NHibernate.Event
{
	/// <summary> Called before removing a collection </summary>
	public interface IPreCollectionRemoveEventListener
	{
		void OnPreRemoveCollection(PreCollectionRemoveEvent @event);
	}
}