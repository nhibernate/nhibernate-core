using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ListIndexMapper : IListIndexMapper
	{
		private const string DefaultIndexColumnName = "ListIdx";
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
			Column(x => x.Name(columnName));
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			HbmColumn hbm = mapping.Columns.SingleOrDefault();
			hbm = hbm
						??
						new HbmColumn
						{
							name = mapping.column1
						};
			columnMapper(new ColumnMapper(hbm, DefaultIndexColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				mapping.column = hbm;
				ResetColumnPlainValues();
			}
			else
			{
				mapping.column1 = !DefaultIndexColumnName.Equals(hbm.name) ? hbm.name : null;
			}
		}

		private void ResetColumnPlainValues()
		{
			mapping.column1 = null;
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.length != null || hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.unique
						 || hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null
						 || hbm.check != null;
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