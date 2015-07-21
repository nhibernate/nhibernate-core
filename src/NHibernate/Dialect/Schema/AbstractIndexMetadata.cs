using System;
using System.Collections.Generic;
using System.Data;

namespace NHibernate.Dialect.Schema
{
	public abstract class AbstractIndexMetadata : IIndexMetadata
	{
		private String name;
		private readonly List<IColumnMetadata> columns = new List<IColumnMetadata>();

		public AbstractIndexMetadata(DataRow rs)
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
			return "IndexMatadata(" + name + ')';
		}
	}
}