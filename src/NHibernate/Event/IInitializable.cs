using NHibernate.Cfg;

namespace NHibernate.Event
{
	/// <summary> 
	/// An event listener that requires access to mappings to
	/// initialize state at initialization time.
	/// </summary>
	public interface IInitializable
	{
		void Initialize(Configuration cfg);
	}
}