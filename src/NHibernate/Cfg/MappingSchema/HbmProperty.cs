using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NHibernate.Util;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmProperty : AbstractDecoratable, IEntityPropertyMapping, IColumnsMapping, IFormulasMapping, ITypeMapping
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		public bool IsLazyProperty
		{
			get { return lazy; }
		}

		public string Access
		{
			get { return access; }
		}

		public bool OptimisticLock
		{
			get { return optimisticlock; }
		}

		#endregion

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		#endregion

		#region Implementation of IColumnsMapping

		[XmlIgnore]
		public IEnumerable<HbmColumn> Columns
		{
			get { return !ArrayHelper.IsNullOrEmpty(Items) ? Items.OfType<HbmColumn>() : AsColumns(); }
		}

		#endregion

		private IEnumerable<HbmColumn> AsColumns()
		{
			if(string.IsNullOrEmpty(column))
			{
				yield break;
			}
			else
			{
				yield return new HbmColumn
				{
					name = column,
					length = length,
					scale = scale,
					precision = precision,
					notnull = notnull,
					notnullSpecified = notnullSpecified,
					unique = unique,
					uniqueSpecified = true,
					uniquekey = uniquekey,
					index = index,
				};
			}
		}

		#region Implementation of IFormulasMapping

		[XmlIgnore]
		public IEnumerable<HbmFormula> Formulas
		{
			get { return !ArrayHelper.IsNullOrEmpty(Items) ? Items.OfType<HbmFormula>() : AsFormulas(); }
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
			get { return type ?? (!string.IsNullOrEmpty(type1) ? new HbmType {name = type1} : null); }
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
				if (!ArrayHelper.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(column) || !string.IsNullOrEmpty(formula)))
					throw new MappingException(
						$"On {Name} property: specifying columns or formulas with both attributes and " +
						"sub-elements is invalid. Please use only sub-elements, or only one of them as attribute");
				if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(formula))
					throw new MappingException(
						$"On {Name} property: specifying both column and formula attributes is invalid. Please " +
						"specify only one of them, or use sub-elements");
				return !ArrayHelper.IsNullOrEmpty(Items) ? Items : AsColumns().Cast<object>().Concat(AsFormulas());
			}
		}
	}
}
