using System.Collections.Generic;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Defines mapping elements to which filters may be applied.
	/// </summary>
	public interface IFilterable
	{
		void AddFilter(string name, string condition);

		IDictionary<string, string> FilterMap { get; }
	}
}
