using System;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<HbmColumn> Columns
		{
			get { return column != null ? column.OfType<HbmColumn>() : AsColumns(); }
		}

		#endregion

		private IEnumerable<HbmColumn> AsColumns()
		{
			if (string.IsNullOrEmpty(column1))
			{
				yield break;
			}
			else
			{
				yield return new HbmColumn
				{
					name = column1,
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

		public IEnumerable<HbmFormula> Formulas
		{
			get { return formula != null ? formula.OfType<HbmFormula>() : AsFormulas(); }
		}

		private IEnumerable<HbmFormula> AsFormulas()
		{
			if (string.IsNullOrEmpty(formula1))
			{
				yield break;
			}
			else
			{
				yield return new HbmFormula { Text = new[] { formula1 } };
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
		public IEnumerable<object> ColumnsAndFormulas
		{
			get { return Columns.Cast<object>().Concat(Formulas.Cast<object>()); }
		}
		
		public HbmLaziness? Lazy
		{
			get { return lazySpecified ? lazy : (HbmLaziness?) null;}
		}
	}
}