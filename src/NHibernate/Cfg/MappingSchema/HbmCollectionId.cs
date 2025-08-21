using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmCollectionId : IColumnsMapping, ITypeMapping
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
			if(string.IsNullOrEmpty(column1))
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

		public HbmType Type
		{
			get { return !string.IsNullOrEmpty(type) ? new HbmType { name = type } : null; }
		}

		public HbmGenerator Generator
		{
			get { return string.IsNullOrEmpty(generator1) ? generator : new HbmGenerator { @class = generator1 }; }
		}
	}
}
