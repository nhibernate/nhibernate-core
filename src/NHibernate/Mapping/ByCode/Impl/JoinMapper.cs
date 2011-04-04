using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class TableNameChangedEventArgs
	{
		public TableNameChangedEventArgs(string oldName, string newName)
		{
			OldName = oldName;
			NewName = newName;
		}
		public string OldName { get; private set; }
		public string NewName { get; private set; }
	}
	public delegate void TableNameChangedHandler(IJoinMapper mapper, TableNameChangedEventArgs args);

	public class JoinMapper: AbstractPropertyContainerMapper,IJoinMapper
	{
		private readonly HbmJoin hbmJoin;

		public JoinMapper(System.Type container, string splitGroupId, HbmJoin hbmJoin, HbmMapping mapDoc) : base(container, mapDoc)
		{
			if (splitGroupId == null)
			{
				throw new ArgumentNullException("splitGroupId");
			}
			if (hbmJoin == null)
			{
				throw new ArgumentNullException("hbmJoin");
			}
			this.hbmJoin = hbmJoin;
			this.hbmJoin.table = splitGroupId.Trim();
			if (string.IsNullOrEmpty(this.hbmJoin.table))
			{
				throw new ArgumentOutOfRangeException("splitGroupId", "The table-name cant be empty.");
			}
		}

		public event TableNameChangedHandler TableNameChanged;

		public void InvokeTableNameChanged(TableNameChangedEventArgs e)
		{
			TableNameChangedHandler handler = TableNameChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected override void AddProperty(object property)
		{
		}

		public void Loader(string namedQueryReference)
		{
			// not supported by Join
		}

		public void SqlInsert(string sql)
		{
		}

		public void SqlUpdate(string sql)
		{
		}

		public void SqlDelete(string sql)
		{
		}

		public void Subselect(string sql)
		{
		}

		public void Table(string tableName)
		{
			if (tableName == null)
			{
				throw new ArgumentNullException("tableName");
			}

			var trimmedName = tableName.Trim();
			if (string.IsNullOrEmpty(trimmedName))
			{
				throw new ArgumentOutOfRangeException("tableName", "The table-name cant be empty.");
			}
			var oldName = hbmJoin.table;
			hbmJoin.table = trimmedName;
			if(!trimmedName.Equals(oldName))
			{
				InvokeTableNameChanged(new TableNameChangedEventArgs(oldName, trimmedName));
			}
		}

		public void Catalog(string catalogName)
		{
		}

		public void Schema(string schemaName)
		{
		}

		public void Key(Action<IKeyMapper> keyMapping)
		{
		}

		public void Inverse(bool value)
		{
		}

		public void Optional(bool isOptional)
		{
		}

		public void Fetch(FetchMode fetchMode)
		{
		}
	}
}