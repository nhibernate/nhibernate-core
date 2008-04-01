using System.Collections.Generic;
using System.Data;

namespace NHibernate.Tool.hbm2ddl
{
	public class ForeignKeyMetadata
	{
		private readonly string name;
		private readonly List<ColumnMetadata> columns = new List<ColumnMetadata>();

		public ForeignKeyMetadata(DataRow rs)
		{
			name = (string)rs["CONSTRAINT_NAME"];
		}

		public string Name
		{
			get { return name; }
		}

		public void AddColumn(ColumnMetadata column)
		{
			if (column != null) columns.Add(column);
		}

		public ColumnMetadata[] Columns
		{
			get { return columns.ToArray(); }
		}

		public override string ToString()
		{
			return "ForeignKeyMetadata(" + name + ')';
		}
	}
}
