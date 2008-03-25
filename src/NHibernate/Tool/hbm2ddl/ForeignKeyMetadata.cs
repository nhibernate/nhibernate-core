namespace NHibernate.Tool.hbm2ddl
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	public class ForeignKeyMetadata
	{
		private readonly String name;
		private readonly List<ColumnMetadata> columns = new List<ColumnMetadata>();

		public ForeignKeyMetadata(DataRow rs)
		{
			name = (string)rs["CONSTRAINT_NAME"];
		}

		public String getName()
		{
			return name;
		}

		public void AddColumn(ColumnMetadata column)
		{
			if (column != null) columns.Add(column);
		}

		public ColumnMetadata[] getColumns()
		{
			return columns.ToArray();
		}

		public override String ToString()
		{
			return "ForeignKeyMetadata(" + name + ')';
		}
	}
}
