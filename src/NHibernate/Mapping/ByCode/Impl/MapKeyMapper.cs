using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	// 6.0 TODO: remove IColumnsAndFormulasMapper once IMapKeyMapper inherits it.
	public class MapKeyMapper : IMapKeyMapper, IColumnsAndFormulasMapper
	{
		private readonly HbmMapKey hbmMapKey;
		private const string DefaultColumnName = "mapKey";

		public MapKeyMapper(HbmMapKey hbmMapKey)
		{
			this.hbmMapKey = hbmMapKey;
		}

		public HbmMapKey MapKeyMapping
		{
			get { return hbmMapKey; }
		}

		#region IMapKeyMapper Members

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (hbmMapKey.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");
			}
			hbmMapKey.formula = null;
			HbmColumn hbm = hbmMapKey.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = hbmMapKey.column,
			      	length = hbmMapKey.length,
			      };
			columnMapper(new ColumnMapper(hbm, DefaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				hbmMapKey.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				hbmMapKey.column = !DefaultColumnName.Equals(hbm.name) ? hbm.name : null;
				hbmMapKey.length = hbm.length;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			var columns = new HbmColumn[columnMapper.Length];
			for (var i = 0; i < columnMapper.Length; i++)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = DefaultColumnName + i + 1;
				columnMapper[i](new ColumnMapper(hbm, defaultColumnName));
				columns[i] = hbm;
			}
			hbmMapKey.Items = columns;
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				hbmMapKey.type = persistentType.Name;
			}
		}

		public void Type<TPersistentType>()
		{
			Type(typeof (TPersistentType));
		}

		public void Type(System.Type persistentType)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}

			if (!typeof(IUserType).IsAssignableFrom(persistentType) &&
				!typeof(IType).IsAssignableFrom(persistentType) &&
				!typeof(ICompositeUserType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException(
					nameof(persistentType),
					"Expected type implementing IUserType, ICompositeUserType or IType.");
			}

			hbmMapKey.type = persistentType.AssemblyQualifiedName;
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		#endregion

		#region Implementation of IColumnsAndFormulasMapper

		/// <inheritdoc />
		public void ColumnsAndFormulas(params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			ResetColumnPlainValues();

			hbmMapKey.Items = ColumnOrFormulaMapper.GetItemsFor(columnOrFormulaMapper, DefaultColumnName);
		}

		/// <inheritdoc cref="IColumnsAndFormulasMapper.Formula" />
		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			hbmMapKey.Items = null;
			string[] formulaLines = formula.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				hbmMapKey.Items = new object[] {new HbmFormula {Text = formulaLines}};
			}
			else
			{
				hbmMapKey.formula = formula;
			}
		}

		/// <inheritdoc />
		public void Formulas(params string[] formulas)
		{
			if (formulas == null)
				throw new ArgumentNullException(nameof(formulas));

			ResetColumnPlainValues();
			hbmMapKey.Items =
				formulas
					.ToArray(
						f => (object) new HbmFormula {Text = f.Split(StringHelper.LineSeparators, StringSplitOptions.None)});
		}

		#endregion

		private void ResetColumnPlainValues()
		{
			hbmMapKey.column = null;
			hbmMapKey.length = null;
			hbmMapKey.formula = null;
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.unique
			       || hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null
			       || hbm.check != null;
		}
	}
}
