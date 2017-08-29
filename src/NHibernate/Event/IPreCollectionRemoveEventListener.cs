namespace NHibernate.Event
{
	/// <summary> Called before removing a collection </summary>
	public partial interface IPreCollectionRemoveEventListener
	{
		void OnPreRemoveCollection(PreCollectionRemoveEvent @event);
	}
}