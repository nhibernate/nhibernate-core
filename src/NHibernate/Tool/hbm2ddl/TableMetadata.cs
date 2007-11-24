namespace NHibernate.Tool.hbm2ddl
{
	using System;
	using System.Collections;
	using System.Data;
	using Iesi.Collections;
	using log4net;
	using Util;

	public class TableMetadata
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TableMetadata));

		private readonly String schema;
		private readonly String name;
		private readonly IDictionary columns = new Hashtable();
		private readonly IDictionary foreignKeys = new Hashtable();
		private readonly IDictionary indexes = new Hashtable();
		internal TableMetadata(DataRow rs, ISchemaReader meta, bool extras)
		{
			schema = (string)rs["TABLE_SCHEMA"];
			name = (string)rs["TABLE_NAME"];
			InitColumns(meta);
			if (extras)
			{
				InitForeignKeys(meta);
				InitIndexes(meta);
			}
			String schem = schema == null ? "" : schema + '.';
			log.Info("table found: " + schem + name);
			log.Info("columns: " + StringHelper.CollectionToString(columns.Keys));
			if (extras)
			{
				log.Info("foreign keys: " + StringHelper.CollectionToString(foreignKeys.Keys));
				log.Info("indexes: " + StringHelper.CollectionToString(indexes.Keys));
			}
		}

		public string Name
		{
			get { return name; }
		}


		public string Schema
		{
			get { return schema; }
		}

		public override String ToString()
		{
			return "TableMetadata(" + name + ')';
		}

		public ColumnMetadata GetColumnMetadata(String columnName)
		{
			return (ColumnMetadata)columns[columnName.ToLower()];
		}

		public ForeignKeyMetadata GetForeignKeyMetadata(String keyName)
		{
			return (ForeignKeyMetadata)foreignKeys[keyName.ToLower()];
		}

		public IndexMetadata GetIndexMetadata(String indexName)
		{
			return (IndexMetadata)indexes[indexName.ToLower()];
		}

		private void AddForeignKey(DataRow rs, ISchemaReader meta)
		{
			String fk = (string)rs["CONSTRAINT_NAME"];

			if (fk == null) return;

			ForeignKeyMetadata info = GetForeignKeyMetadata(fk);
			if (info == null)
			{
				info = new ForeignKeyMetadata(rs);
				foreignKeys.Add(info.getName().ToLower(), info);
			}

			foreach (DataRow row in meta.GetIndexColumns(schema, name, fk).Rows)
			{
				info.AddColumn(GetColumnMetadata((string)row["COLUMN_NAME"]));
			}
		}

		private void AddIndex(DataRow rs, ISchemaReader meta)
		{
			String index = (string)rs["INDEX_NAME"];

			if (index == null) return;

			IndexMetadata info = GetIndexMetadata(index);
			if (info == null)
			{
				info = new IndexMetadata(rs);
				indexes.Add(info.getName().ToLower(), info);
			}

			foreach (DataRow row in meta.GetIndexColumns(schema, name, index).Rows)
			{
				info.AddColumn(GetColumnMetadata((string)row["COLUMN_NAME"]));
			}
		}

		public void AddColumn(DataRow rs)
		{
			String column = (string)rs["COLUMN_NAME"];

			if (column == null) return;

			if (GetColumnMetadata(column) == null)
			{
				ColumnMetadata info = new ColumnMetadata(rs);
				columns.Add(info.Name.ToLower(), info);
			}
		}

		private void InitForeignKeys(ISchemaReader meta)
		{
			foreach (DataRow row in meta.GetForeignKeys(schema, name).Rows)
			{
				AddForeignKey(row, meta);
			}
		}

		private void InitIndexes(ISchemaReader meta)
		{
			foreach (DataRow row in meta.GetIndexInfo(schema, name).Rows)
			{
				AddIndex(row, meta);
			}
		}

		private void InitColumns(ISchemaReader meta)
		{
			foreach (DataRow row in meta.GetColumns(schema, name).Rows)
			{
				AddColumn(row);
			}
		}
	}
}
