using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ListIndexMapper : IListIndexMapper
	{
		private readonly HbmListIndex mapping;
		private readonly System.Type ownerEntityType;

		public ListIndexMapper(System.Type ownerEntityType, HbmListIndex mapping)
		{
			this.ownerEntityType = ownerEntityType;
			this.mapping = mapping;
		}

		#region Implementation of IListIndexMapper

		public void Column(string columnName)
		{
			mapping.column1 = columnName;
		}

		public void Base(int baseIndex)
		{
			if (baseIndex <= 0)
			{
				throw new ArgumentOutOfRangeException("baseIndex", "The baseIndex should be positive value");
			}

			mapping.@base = baseIndex.ToString();
		}

		#endregion
	}
}