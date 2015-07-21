using System.Collections.Generic;
using System.Data;

namespace NHibernate.Dialect.Schema
{
	public class AbstractForeignKeyMetadata : IForeignKeyMetadata
	{
		private string name;
		private readonly List<IColumnMetadata> columns = new List<IColumnMetadata>();

		public AbstractForeignKeyMetadata(DataRow rs)
		{
		}

		public string Name
		{
			get { return name; }
			protected set { name = value; }
		}

		public void AddColumn(IColumnMetadata column)
		{
			if (column != null) columns.Add(column);
		}

		public IColumnMetadata[] Columns
		{
			get { return columns.ToArray(); }
		}

		public override string ToString()
		{
			return "ForeignKeyMetadata(" + name + ')';
		}
	}
}