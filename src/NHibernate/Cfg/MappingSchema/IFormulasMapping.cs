using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	public interface IFormulasMapping
	{
		IEnumerable<HbmFormula> Formulas { get; }
	}
}