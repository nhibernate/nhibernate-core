using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	/// <summary>
	/// Implemented by any mapping elemes supports simple and/or multicolumn mapping.
	/// </summary>
	public interface IColumnsMapping
	{
		IEnumerable<HbmColumn> Columns { get; }
	}
}