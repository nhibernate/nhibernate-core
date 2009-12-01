using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmListIndex: IColumnsMapping
	{
		public IEnumerable<HbmColumn> Columns
		{
			get
			{
				if (column != null)
				{
					yield return column;
				}
				else if (string.IsNullOrEmpty(column1))
				{
					yield break;
				}
				else
				{
					yield return new HbmColumn
					{
						name = column1,
					};
				}
			}
		}
	}
}