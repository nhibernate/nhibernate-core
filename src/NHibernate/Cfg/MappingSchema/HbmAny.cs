using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmAny : AbstractDecoratable, IEntityPropertyMapping, IColumnsMapping, IAnyMapping
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

		public bool OptimisticLock
		{
			get { return optimisticlock; }
		}

		public bool IsLazyProperty
		{
			get { return lazy; }
		}

		#endregion

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		#endregion

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
					index = index
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