using NHibernate.Loader;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Any mapping with an outer-join attribute
	/// </summary>
	public interface IFetchable
	{
		/// <summary>
		/// 
		/// </summary>
		OuterJoinFetchStrategy OuterJoinFetchSetting { get; set; }
	}
}
