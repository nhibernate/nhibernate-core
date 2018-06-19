using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmElement: IColumnsMapping, IFormulasMapping, ITypeMapping
	{
		#region Implementation of IColumnsMapping

		[XmlIgnore]
		public IEnumerable<HbmColumn> Columns
		{
			get { return Items != null ? Items.OfType<HbmColumn>() : AsColumns(); }
		}

		#endregion

		private IEnumerable<HbmColumn> AsColumns()
		{
			if (string.IsNullOrEmpty(column))
			{
				yield break;
			}
			else
			{
				yield return
					new HbmColumn
						{
							name = column,
							length = length,
							scale = scale,
							precision = precision,
							notnull = notnull,
							notnullSpecified = true,
							unique = unique,
							uniqueSpecified = true,
						};
			}
		}

		#region Implementation of IFormulasMapping

		[XmlIgnore]
		public IEnumerable<HbmFormula> Formulas
		{
			get { return Items != null ? Items.OfType<HbmFormula>() : AsFormulas(); }
		}

		private IEnumerable<HbmFormula> AsFormulas()
		{
			if (string.IsNullOrEmpty(formula))
			{
				yield break;
			}
			else
			{
				yield return new HbmFormula { Text = new[] { formula } };
			}
		}

		#endregion


		#region Implementation of ITypeMapping

		public HbmType Type
		{
			get { return type ?? (!string.IsNullOrEmpty(type1) ? new HbmType { name = type1 } : null); }
		}

		#endregion

		/// <summary>
		/// Columns and Formulas, in declared order
		/// </summary>
		[XmlIgnore]
		public IEnumerable<object> ColumnsAndFormulas
		{
			get
			{
				if (Items != null)
					return Items;
				// Avoid a possible breaking change for mapping having left a column attribute along with a formula one.
				// This is a mapping error, but previous implementation was silently ignoring the column attribute in
				// such case.
				if (!string.IsNullOrEmpty(formula))
					return AsFormulas();
				return AsColumns();
			}
		}
	}
}
