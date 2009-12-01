using System;
using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmDiscriminator: IColumnsMapping, IFormulasMapping
	{
		#region Implementation of IColumnsMapping

		public IEnumerable<HbmColumn> Columns
		{
			get
			{
				if(Item as HbmColumn != null)
				{
					yield return (HbmColumn)Item;
				}
				else if (string.IsNullOrEmpty(column))
				{
					yield break;
				}
				else
				{
					yield return new HbmColumn
					{
						name = column,
						length = length,
					};
				}
			}
		}

		#endregion

		#region Implementation of IFormulasMapping

		public IEnumerable<HbmFormula> Formulas
		{
			get
			{
				if (Item as HbmFormula != null)
				{
					yield return (HbmFormula) Item;
				}
				else if (string.IsNullOrEmpty(formula))
				{
					yield break;
				}
				else
				{
					yield return new HbmFormula {Text = new[] {formula}};
				}
			}
		}

		#endregion
	}
}