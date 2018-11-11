using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	// 6.0 TODO: remove IColumnsAndFormulasMapper once IMapKeyManyToManyMapper inherits it.
	public class MapKeyManyToManyMapper : IMapKeyManyToManyMapper, IColumnsAndFormulasMapper
	{
		private const string DefaultColumnName = "mapKeyRelation";
		private readonly HbmMapKeyManyToMany mapping;

		public MapKeyManyToManyMapper(HbmMapKeyManyToMany mapping)
		{
			this.mapping = mapping;
		}

		public HbmMapKeyManyToMany MapKeyManyToManyMapping
		{
			get { return mapping; }
		}

		#region Implementation of IMapKeyManyToManyMapper

		public void ForeignKey(string foreignKeyName)
		{
			mapping.foreignkey = foreignKeyName;
		}

		#endregion

		#region Implementation of IColumnsAndFormulasMapper

		/// <inheritdoc />
		public void ColumnsAndFormulas(params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			ResetColumnPlainValues();

			mapping.Items = ColumnOrFormulaMapper.GetItemsFor(columnOrFormulaMapper, DefaultColumnName);
		}

		/// <inheritdoc cref="IColumnsAndFormulasMapper.Formula" />
		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			mapping.Items = null;
			string[] formulaLines = formula.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				mapping.Items = new object[] {new HbmFormula {Text = formulaLines}};
			}
			else
			{
				mapping.formula = formula;
			}
		}

		/// <inheritdoc />
		public void Formulas(params string[] formulas)
		{
			if (formulas == null)
				throw new ArgumentNullException(nameof(formulas));

			ResetColumnPlainValues();
			mapping.Items =
				formulas
					.Select(
						f => (object) new HbmFormula { Text = f.Split(StringHelper.LineSeparators, StringSplitOptions.None) })
					.ToArray();
		}

		#endregion

		#region IColumnsMapper Members

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (mapping.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");
			}
			mapping.formula = null;
			HbmColumn hbm = mapping.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = mapping.column,
			      };
			columnMapper(new ColumnMapper(hbm, DefaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				mapping.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				mapping.column = !DefaultColumnName.Equals(hbm.name) ? hbm.name : null;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			mapping.column = null;
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = DefaultColumnName + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			mapping.Items = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(cm => cm.Name(name));
		}

		#endregion

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.length != null || hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.unique
			       || hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null
			       || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			mapping.column = null;
			mapping.formula = null;
		}
	}
}
