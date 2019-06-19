using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	// 6.0 TODO: remove IColumnsAndFormulasMapper once IElementMapper inherits it.
	public class ElementMapper : IElementMapper, IColumnsAndFormulasMapper
	{
		private const string DefaultColumnName = "element";
		private readonly HbmElement elementMapping;
		private readonly System.Type elementType;

		public ElementMapper(System.Type elementType, HbmElement elementMapping)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (elementMapping == null)
			{
				throw new ArgumentNullException("elementMapping");
			}
			this.elementType = elementType;
			this.elementMapping = elementMapping;
			this.elementMapping.type1 = elementType.GetNhTypeName();
		}

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (elementMapping.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			elementMapping.formula = null;
			HbmColumn hbm = elementMapping.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = elementMapping.column,
			      	length = elementMapping.length,
			      	scale = elementMapping.scale,
			      	precision = elementMapping.precision,
			      	notnull = elementMapping.notnull,
			      	unique = elementMapping.unique,
			      	uniqueSpecified = elementMapping.unique,
			      };
			columnMapper(new ColumnMapper(hbm, DefaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				elementMapping.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				elementMapping.column = !DefaultColumnName.Equals(hbm.name) ? hbm.name : null;
				elementMapping.length = hbm.length;
				elementMapping.precision = hbm.precision;
				elementMapping.scale = hbm.scale;
				elementMapping.notnull = hbm.notnull;
				elementMapping.unique = hbm.unique;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = DefaultColumnName + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			elementMapping.Items = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			elementMapping.column = null;
			elementMapping.length = null;
			elementMapping.precision = null;
			elementMapping.scale = null;
			elementMapping.notnull = false;
			elementMapping.unique = false;
			elementMapping.formula = null;
		}

		#endregion

		#region Implementation of IColumnsAndFormulasMapper

		/// <inheritdoc />
		public void ColumnsAndFormulas(params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			ResetColumnPlainValues();

			elementMapping.Items = ColumnOrFormulaMapper.GetItemsFor(columnOrFormulaMapper, DefaultColumnName);
		}

		/// <inheritdoc cref="IColumnsAndFormulasMapper.Formula" />
		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			elementMapping.Items = null;
			string[] formulaLines = formula.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				elementMapping.Items = new object[] {new HbmFormula {Text = formulaLines}};
			}
			else
			{
				elementMapping.formula = formula;
			}
		}

		/// <inheritdoc />
		public void Formulas(params string[] formulas)
		{
			if (formulas == null)
				throw new ArgumentNullException(nameof(formulas));

			ResetColumnPlainValues();
			elementMapping.Items =
				formulas
					.ToArray(
						f => (object) new HbmFormula {Text = f.Split(StringHelper.LineSeparators, StringSplitOptions.None)});
		}

		#endregion

		#region Implementation of IElementMapper

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				elementMapping.type1 = persistentType.Name;
				elementMapping.type = null;
			}
		}

		public void Type<TPersistentType>()
		{
			Type(typeof (TPersistentType), null);
		}

		public void Type<TPersistentType>(object parameters)
		{
			Type(typeof (TPersistentType), parameters);
		}

		public void Type(System.Type persistentType, object parameters)
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

			if (parameters != null)
			{
				elementMapping.type1 = null;
				var hbmType = new HbmType
				{
					name = persistentType.AssemblyQualifiedName,
					param = parameters.GetType().GetProperties().ToArray(
						pi =>
						{
							var pvalue = pi.GetValue(parameters, null);
							return
								new HbmParam {name = pi.Name, Text = new[] {ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString()}};
						})
				};
				elementMapping.type = hbmType;
			}
			else
			{
				elementMapping.type1 = persistentType.AssemblyQualifiedName;
				elementMapping.type = null;
			}
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		public void Precision(short precision)
		{
			Column(x => x.Precision(precision));
		}

		public void Scale(short scale)
		{
			Column(x => x.Scale(scale));
		}

		public void NotNullable(bool notnull)
		{
			Column(x => x.NotNullable(notnull));
		}

		public void Unique(bool unique)
		{
			Column(x => x.Unique(unique));
		}

		#endregion
	}
}
