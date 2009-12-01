using System;
using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmTimestamp : AbstractDecoratable, IColumnsMapping
	{
		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		#region Implementation of IColumnsMapping

		public IEnumerable<HbmColumn> Columns
		{
			get
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
		}

		#endregion
	}
}