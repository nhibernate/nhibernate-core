namespace NHibernate.Mapping
{
	/// <summary>
	/// Any mapping with an outer-join attribute
	/// </summary>
	public interface IFetchable
	{
		FetchMode FetchMode { get; set; }
		bool IsLazy { get; set; }
	}
}