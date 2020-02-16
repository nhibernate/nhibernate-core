using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmKeyProperty : AbstractDecoratable, IColumnsMapping, ITypeMapping, IEntityPropertyMapping
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

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		#endregion

		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		public bool IsLazyProperty
		{
			get { return false; }
		}

		public string Access
		{
			get { return access; }
		}

		public bool OptimisticLock
		{
			get { return false; }
		}

		#endregion
	}
}
