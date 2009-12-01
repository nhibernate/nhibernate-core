using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ColumnsBinder: Binder
	{
		private readonly SimpleValue value;

		public ColumnsBinder(SimpleValue value, Mappings mappings)
			: base(mappings)
		{
			this.value = value;
		}

		public void Bind(IEnumerable<HbmColumn> columns, bool isNullable, Func<HbmColumn> defaultColumnDelegate)
		{
			var table = value.Table;
			int colIndex = 0;
			foreach (var hbmColumn in columns)
			{
				BindColumn(hbmColumn, table, colIndex++, isNullable);
			}

			if (value.ColumnSpan == 0 && defaultColumnDelegate != null)
			{
				BindColumn(defaultColumnDelegate(), table, colIndex, isNullable);
			}
		}

		private void BindColumn(HbmColumn hbmColumn, Table table, int colIndex, bool isNullable)
		{
			var col = new Column {Value = value, TypeIndex = colIndex};
			BindColumn(hbmColumn, col, isNullable);

			if (table != null)
				table.AddColumn(col);

			value.AddColumn(col);

			//column index
			BindIndex(hbmColumn.index, table, col);
			//column group index (although it can serve as a separate column index)
			BindUniqueKey(hbmColumn.uniquekey, table, col);
		}

		private void BindColumn(HbmColumn columnMapping, Column column, bool isNullable)
		{
			column.Name = mappings.NamingStrategy.ColumnName(columnMapping.name);

			if (columnMapping.length != null)
				column.Length = int.Parse(columnMapping.length);
			if (columnMapping.scale != null)
				column.Scale = int.Parse(columnMapping.scale);
			if (columnMapping.precision != null)
				column.Precision = int.Parse(columnMapping.precision);

			column.IsNullable = columnMapping.notnullSpecified ? !columnMapping.notnull : isNullable;
			column.IsUnique = columnMapping.uniqueSpecified && columnMapping.unique;
			column.CheckConstraint = columnMapping.check ?? string.Empty;
			column.SqlType = columnMapping.sqltype;
			column.DefaultValue = columnMapping.@default;
			if (columnMapping.comment != null)
				column.Comment = columnMapping.comment.Text.LinesToString().Trim();
		}

		private static void BindIndex(string indexAttribute, Table table, Column column)
		{
			if (indexAttribute != null && table != null)
			{
				var tokens = indexAttribute.Split(',');
				System.Array.ForEach(tokens, t => table.GetOrCreateIndex(t.Trim()).AddColumn(column));
			}
		}

		private static void BindUniqueKey(string uniqueKeyAttribute, Table table, Column column)
		{
			if (uniqueKeyAttribute != null && table != null)
			{
				var tokens = uniqueKeyAttribute.Split(',');
				System.Array.ForEach(tokens, t => table.GetOrCreateUniqueKey(t.Trim()).AddColumn(column));
			}
		}
	}
}