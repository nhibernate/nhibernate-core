using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmKeyProperty: IColumnsMapping, ITypeMapping
	{

		#region Implementation of IColumnsMapping

		public IEnumerable<HbmColumn> Columns
		{
			get { return column ?? AsColumns(); }
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
					length = length,
				};
			}
		}

		#region Implementation of ITypeMapping

		public HbmType Type
		{
			get { return type ?? (!string.IsNullOrEmpty(type1) ? new HbmType { name = type1 } : null); }
		}

		#endregion
	}
}