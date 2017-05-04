namespace NHibernate.Event
{
	/// <summary> 
	/// Defines the contract for handling of collection initialization events 
	/// generated by a session. 
	/// </summary>
	public partial interface IInitializeCollectionEventListener
	{
		void OnInitializeCollection(InitializeCollectionEvent @event);
	}
}