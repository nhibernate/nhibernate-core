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
		/// <param name="theEvent"></param>
		void OnPreLoad(PreLoadEvent theEvent);
	}
}