using System;
using System.Linq;
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
		private readonly KeyMapper keyMapper;

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
			if (hbmJoin.key == null)
			{
				hbmJoin.key = new HbmKey { column1 = container.Name.ToLowerInvariant() + "_key" };
			}
			keyMapper = new KeyMapper(container, hbmJoin.key);
		}

		public event TableNameChangedHandler TableNameChanged;

		private void InvokeTableNameChanged(TableNameChangedEventArgs e)
		{
			TableNameChangedHandler handler = TableNameChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			hbmJoin.Items = hbmJoin.Items == null ? toAdd : hbmJoin.Items.Concat(toAdd).ToArray();
		}

		public void Loader(string namedQueryReference)
		{
			// not supported by Join
		}

		public void SqlInsert(string sql)
		{
			if (hbmJoin.SqlInsert == null)
			{
				hbmJoin.sqlinsert = new HbmCustomSQL();
			}
			hbmJoin.sqlinsert.Text = new[] { sql };
		}

		public void SqlUpdate(string sql)
		{
			if (hbmJoin.SqlUpdate == null)
			{
				hbmJoin.sqlupdate = new HbmCustomSQL();
			}
			hbmJoin.sqlupdate.Text = new[] { sql };
		}

		public void SqlDelete(string sql)
		{
			if (hbmJoin.SqlDelete == null)
			{
				hbmJoin.sqldelete = new HbmCustomSQL();
			}
			hbmJoin.sqldelete.Text = new[] { sql };
		}

		public void Subselect(string sql)
		{
			if (hbmJoin.Subselect == null)
			{
				hbmJoin.subselect = new HbmSubselect();
			}
			hbmJoin.subselect.Text = new[] { sql };
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
			hbmJoin.catalog = catalogName;
		}

		public void Schema(string schemaName)
		{
			hbmJoin.schema = schemaName;
		}

		public void Key(Action<IKeyMapper> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void Inverse(bool value)
		{
			hbmJoin.inverse = value;
		}

		public void Optional(bool isOptional)
		{
			hbmJoin.optional = isOptional;
		}

		public void Fetch(FetchKind fetchMode)
		{
			hbmJoin.fetch = fetchMode.ToHbmJoinFetch();
		}
	}
}