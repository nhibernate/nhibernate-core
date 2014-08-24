namespace NHibernate.Event
{
	/// <summary> 
	/// Called before injecting property values into a newly loaded entity instance.
	/// </summary>
	public interface IPreLoadEventListener
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		void OnPreLoad(PreLoadEvent @event);
	}
}