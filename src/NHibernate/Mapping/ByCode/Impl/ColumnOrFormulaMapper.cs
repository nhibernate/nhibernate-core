using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ColumnOrFormulaMapper : ColumnMapper, IColumnOrFormulaMapper
	{
		private readonly HbmFormula _formulaMapping;

		public ColumnOrFormulaMapper(HbmColumn columnMapping, string memberName, HbmFormula formulaMapping) :
			base(columnMapping, memberName)
		{
			_formulaMapping = formulaMapping ?? throw new ArgumentNullException(nameof(formulaMapping));
		}

		public void Formula(string formula)
		{
			_formulaMapping.Text = formula?.Split(StringHelper.LineSeparators, StringSplitOptions.None);
		}

		public static object[] GetItemsFor(Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper, string baseDefaultColumnName)
		{
			if (columnOrFormulaMapper == null)
				throw new ArgumentNullException(nameof(columnOrFormulaMapper));

			var i = 1;
			var columnsOrFormulas = new List<object>(columnOrFormulaMapper.Length);
			foreach (var action in columnOrFormulaMapper)
			{
				var hbmCol = new HbmColumn();
				var hbmFormula = new HbmFormula();
				var defaultColumnName = baseDefaultColumnName + i++;
				action(new ColumnOrFormulaMapper(hbmCol, defaultColumnName, hbmFormula));
				if (hbmFormula.Text != null)
				{
					columnsOrFormulas.Add(hbmFormula);
				}
				else
				{
					columnsOrFormulas.Add(hbmCol);
				}
			}
			return columnsOrFormulas.ToArray();
		}
	}
}
