using System;
using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Defines mapping elements to which filters may be applied.
	/// </summary>
	public interface IFilterable
	{
		void AddFilter(String name, String condition);

		IDictionary FilterMap { get; }
	}
}
