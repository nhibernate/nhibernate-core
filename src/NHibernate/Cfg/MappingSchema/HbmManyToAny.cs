using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmManyToAny : IColumnsMapping, IAnyMapping
	{

		#region Implementation of IColumnsMapping

		[XmlIgnore]
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
				};
			}
		}

		#region Implementation of IAnyMapping

		public string MetaType
		{
			get { return metatype; }
		}

		public ICollection<HbmMetaValue> MetaValues
		{
			get
			{
				return metavalue ?? new HbmMetaValue[0];
			}
		}

		#endregion
	}
}