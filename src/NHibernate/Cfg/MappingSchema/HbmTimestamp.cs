using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmTimestamp : AbstractDecoratable, IColumnsMapping
	{
		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		#region Implementation of IColumnsMapping

		[XmlIgnore]
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