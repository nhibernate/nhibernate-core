namespace NHibernate.Tool.hbm2ddl
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;

	public class IndexMetadata
	{
		private readonly String name;
		private readonly List<ColumnMetadata> columns = new List<ColumnMetadata>();

		public IndexMetadata(DataRow rs)
		{
			name = (string) rs["INDEX_NAME"];
		}

		public string Name
		{
			get { return name; }
		}

		public void AddColumn(ColumnMetadata column)
		{
			if (column != null) columns.Add(column);
		}

		public ColumnMetadata[] GetColumns()
		{
			return columns.ToArray();
		}

		public override String ToString()
		{
			return "IndexMatadata(" + name + ')';
		}
	}
}
