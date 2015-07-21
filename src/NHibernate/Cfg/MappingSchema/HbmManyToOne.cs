using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmManyToOne : AbstractDecoratable, IEntityPropertyMapping, IColumnsMapping, IFormulasMapping, IRelationship
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		public string Access
		{
			get { return access; }
		}

		public bool IsLazyProperty
		{
			get { return false; }
		}

		public bool OptimisticLock
		{
			get { return optimisticlock; }
		}

		#endregion

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		#endregion

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
					notnull = notnull,
					notnullSpecified = notnullSpecified,
					unique = unique,
					uniqueSpecified = true,
					uniquekey = uniquekey,
					index = index
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
			get { return notfound; }
		}

		#endregion

		/// <summary>
		/// Columns and Formulas, in declared order
		/// </summary>
		[XmlIgnore]
		public IEnumerable<object> ColumnsAndFormulas
		{
			// when Items is empty the column attribute AND formula attribute will be used
			// and it may cause an issue (breaking change)
			// On the other hand it work properly when a mixing between <formula> and <column> tags are used
			// respecting the order used in the mapping to map multi-columns id.
			get { return Items ?? Columns.Cast<object>().Concat(Formulas.Cast<object>()); }
		}
		
		public HbmLaziness? Lazy
		{
			get { return lazySpecified ? lazy : (HbmLaziness?) null;}
		}
	}
}