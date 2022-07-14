using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Defines a contract for filtering entities implemented by persisters.
	/// </summary>
	public interface IFilterable
	{
		FilterHelper FilterHelper { get; }
		
		/// <summary>
		/// Get the where clause filter, given a query alias and considering enabled session filters.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="enabledFilters"></param>
		/// <returns></returns>
		string FilterFragment(string alias, IDictionary<string, IFilter> enabledFilters);
	}
}
