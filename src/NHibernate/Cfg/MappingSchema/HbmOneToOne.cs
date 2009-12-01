using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmOneToOne : AbstractDecoratable, IEntityPropertyMapping, IFormulasMapping
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

		public bool OptimisticKock
		{
			get { return true; }
		}

		#endregion	
		
		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		#endregion

		#region Implementation of IFormulasMapping

		public IEnumerable<HbmFormula> Formulas
		{
			get { return formula ?? AsFormulas(); }
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

	}
}