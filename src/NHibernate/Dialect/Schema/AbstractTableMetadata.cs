using System.Collections.Generic;
using System.Data;

using NHibernate.Util;

namespace NHibernate.Dialect.Schema
{
	public abstract class AbstractTableMetadata : ITableMetadata
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ITableMetadata));
		private string catalog;
		private string schema;
		private string name;
		private readonly Dictionary<string, IColumnMetadata> columns = new Dictionary<string, IColumnMetadata>();
		private readonly Dictionary<string, IForeignKeyMetadata> foreignKeys = new Dictionary<string, IForeignKeyMetadata>();
		private readonly Dictionary<string, IIndexMetadata> indexes = new Dictionary<string, IIndexMetadata>();

		public AbstractTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras)
		{
			ParseTableInfo(rs);
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

		protected abstract void ParseTableInfo(DataRow rs);
		protected abstract string GetConstraintName(DataRow rs);
		protected abstract string GetColumnName(DataRow rs);
		protected abstract string GetIndexName(DataRow rs);
		protected abstract IColumnMetadata GetColumnMetadata(DataRow rs);
		protected abstract IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs);
		protected abstract IIndexMetadata GetIndexMetadata(DataRow rs);

		public string Name
		{
			get { return name; }
			protected set { name = value; }
		}

		public string Catalog
		{
			get { return catalog; }
			protected set { catalog = value; }
		}

		public string Schema
		{
			get { return schema; }
			protected set { schema = value; }
		}

		public override string ToString()
		{
			return "TableMetadata(" + name + ')';
		}

		public IColumnMetadata GetColumnMetadata(string columnName)
		{
			IColumnMetadata result;
			columns.TryGetValue(columnName.ToLowerInvariant(), out result);
			return result;
		}

		public IForeignKeyMetadata GetForeignKeyMetadata(string keyName)
		{
			IForeignKeyMetadata result;
			foreignKeys.TryGetValue(keyName.ToLowerInvariant(), out result);
			return result;
		}

		public IIndexMetadata GetIndexMetadata(string indexName)
		{
			IIndexMetadata result;
			indexes.TryGetValue(indexName.ToLowerInvariant(), out result);
			return result;
		}

		public virtual bool NeedPhysicalConstraintCreation(string fkName)
		{
			return GetIndexMetadata(fkName) == null;
		}

		private void AddForeignKey(DataRow rs, IDataBaseSchema meta)
		{
			string fk = GetConstraintName(rs);

			if (string.IsNullOrEmpty(fk))
				return;

			IForeignKeyMetadata info = GetForeignKeyMetadata(fk);
			if (info == null)
			{
				info = GetForeignKeyMetadata(rs);
				foreignKeys[info.Name.ToLowerInvariant()] = info;
			}

			foreach (DataRow row in meta.GetIndexColumns(catalog, schema, name, fk).Rows)
			{
				info.AddColumn(GetColumnMetadata(GetColumnName(row)));
			}
		}

		private void AddIndex(DataRow rs, IDataBaseSchema meta)
		{
			string index = GetIndexName(rs);

			if (string.IsNullOrEmpty(index)) return;

			IIndexMetadata info = GetIndexMetadata(index);
			if (info == null)
			{
				info = GetIndexMetadata(rs);
				indexes[info.Name.ToLowerInvariant()] = info;
			}

			foreach (DataRow row in meta.GetIndexColumns(catalog, schema, name, index).Rows)
			{
				info.AddColumn(GetColumnMetadata(GetColumnName(row)));
			}
		}

		private void AddColumn(DataRow rs)
		{
			string column = GetColumnName(rs);

			if (string.IsNullOrEmpty(column)) return;

			if (GetColumnMetadata(column) == null)
			{
				IColumnMetadata info = GetColumnMetadata(rs);
				columns[info.Name.ToLowerInvariant()] = info;
			}
		}

		private void InitForeignKeys(IDataBaseSchema meta)
		{
			foreach (DataRow row in meta.GetForeignKeys(catalog, schema, name).Rows)
			{
				AddForeignKey(row, meta);
			}
		}

		private void InitIndexes(IDataBaseSchema meta)
		{
			foreach (DataRow row in meta.GetIndexInfo(catalog, schema, name).Rows)
			{
				AddIndex(row, meta);
			}
		}

		private void InitColumns(IDataBaseSchema meta)
		{
			foreach (DataRow row in meta.GetColumns(catalog, schema, name, null).Rows)
			{
				AddColumn(row);
			}
		}
	}
}