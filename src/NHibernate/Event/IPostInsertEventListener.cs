namespace NHibernate.Event
{
	/// <summary> Called after insterting an item in the datastore </summary>
	public interface IPostInsertEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="theEvent"></param>
		void OnPostInsert(PostInsertEvent theEvent);
	}
}