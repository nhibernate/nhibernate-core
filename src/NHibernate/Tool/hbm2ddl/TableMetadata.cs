using System;
using System.Collections;
using System.Data;
using log4net;
using NHibernate.Dialect.Schema;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Tool.hbm2ddl
{
	public class TableMetadata
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TableMetadata));

		private readonly string catalog;
		private readonly string schema;
		private readonly string name;
		private readonly Dictionary<string, ColumnMetadata> columns = new Dictionary<string, ColumnMetadata>();
		private readonly Dictionary<string, ForeignKeyMetadata> foreignKeys = new Dictionary<string, ForeignKeyMetadata>();
		private readonly Dictionary<string, IndexMetadata> indexes = new Dictionary<string, IndexMetadata>();

		internal TableMetadata(DataRow rs, ISchemaReader meta, bool extras)
		{
			schema = (string)rs["TABLE_CATALOG"];
			schema = (string)rs["TABLE_SCHEMA"];
			name = (string)rs["TABLE_NAME"];
			InitColumns(meta);
			if (extras)
			{
				InitForeignKeys(meta);
				InitIndexes(meta);
			}
			string cat = catalog == null ? "" : catalog + '.';
			string schem = schema == null ? "" : schema + '.';
			log.Info("table found: " + cat + schem + name);
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

		public string Catalog
		{
			get { return catalog; }
		}

		public string Schema
		{
			get { return schema; }
		}

		public override string ToString()
		{
			return "TableMetadata(" + name + ')';
		}

		public ColumnMetadata GetColumnMetadata(string columnName)
		{
			ColumnMetadata result;
			columns.TryGetValue(columnName.ToLowerInvariant(), out result);
			return result;
		}

		public ForeignKeyMetadata GetForeignKeyMetadata(string keyName)
		{
			ForeignKeyMetadata result;
			foreignKeys.TryGetValue(keyName.ToLowerInvariant(), out result);
			return result;
		}

		public IndexMetadata GetIndexMetadata(string indexName)
		{
			IndexMetadata result;
			indexes.TryGetValue(indexName.ToLowerInvariant(), out result);
			return result;
		}

		private void AddForeignKey(DataRow rs, ISchemaReader meta)
		{
			string fk = (string)rs["CONSTRAINT_NAME"];

			if (fk == null)
				return;

			ForeignKeyMetadata info = GetForeignKeyMetadata(fk);
			if (info == null)
			{
				info = new ForeignKeyMetadata(rs);
				foreignKeys[info.Name.ToLowerInvariant()] = info;
			}

			foreach (DataRow row in meta.GetIndexColumns(catalog, schema, name, fk).Rows)
			{
				info.AddColumn(GetColumnMetadata((string)row["COLUMN_NAME"]));
			}
		}

		private void AddIndex(DataRow rs, ISchemaReader meta)
		{
			string index = (string)rs["INDEX_NAME"];

			if (index == null) return;

			IndexMetadata info = GetIndexMetadata(index);
			if (info == null)
			{
				info = new IndexMetadata(rs);
				indexes[info.Name.ToLowerInvariant()] = info;
			}

			foreach (DataRow row in meta.GetIndexColumns(catalog, schema, name, index).Rows)
			{
				info.AddColumn(GetColumnMetadata((string) row["COLUMN_NAME"]));
			}
		}

		public void AddColumn(DataRow rs)
		{
			String column = (string)rs["COLUMN_NAME"];

			if (column == null) return;

			if (GetColumnMetadata(column) == null)
			{
				ColumnMetadata info = new ColumnMetadata(rs);
				columns[info.Name.ToLowerInvariant()] = info;
			}
		}

		private void InitForeignKeys(ISchemaReader meta)
		{
			foreach (DataRow row in meta.GetForeignKeys(catalog, schema, name).Rows)
			{
				AddForeignKey(row, meta);
			}
		}

		private void InitIndexes(ISchemaReader meta)
		{
			foreach (DataRow row in meta.GetIndexInfo(catalog, schema, name).Rows)
			{
				AddIndex(row, meta);
			}
		}

		private void InitColumns(ISchemaReader meta)
		{
			foreach (DataRow row in meta.GetColumns(catalog, schema, name, null).Rows)
			{
				AddColumn(row);
			}
		}
	}
}
