using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmMapKeyManyToMany: IColumnsMapping, IFormulasMapping, IRelationship
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
				yield return new HbmColumn
				{
					name = column,
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

		#region Implementation of IRelationship

		public string EntityName
		{
			get { return entityname; }
		}

		public string Class
		{
			get { return @class; }
		}

		public HbmNotFoundMode NotFoundMode
		{
			get { return HbmNotFoundMode.Exception; }
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
				if (Items != null && (!string.IsNullOrEmpty(column) || !string.IsNullOrEmpty(formula)))
					throw new MappingException(
						$"On a map-key-many-to-many targeting {EntityName}: specifying columns or formulas with both " +
						$"attributes and sub-elements is invalid. Please use only sub-elements, or only one of them " +
						$"as attribute");
				if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(formula))
					throw new MappingException(
						$"On a map-key-many-to-many targeting {EntityName}: specifying both column and formula " +
						$"attributes is invalid. Please specify only one of them, or use sub-elements");
				return Items ?? AsColumns().Cast<object>().Concat(AsFormulas().Cast<object>());
			}
		}
	}
}
