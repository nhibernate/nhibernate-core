using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmKey: IColumnsMapping
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
					notnull = notnull,
					notnullSpecified = notnullSpecified,
					unique = unique,
					uniqueSpecified = uniqueSpecified,
				};
			}
		}

		public bool? IsNullable
		{
			get { return notnullSpecified ? !notnull : (bool?)null; }
		}

		public bool? IsUpdatable
		{
			get { return updateSpecified ? update : (bool?)null; }
		}
	}
}