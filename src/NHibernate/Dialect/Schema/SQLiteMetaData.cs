using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class SQLiteDataBaseMetaData : AbstractDataBaseSchema
	{
		// Since v5.2
		[Obsolete("Use overload with dialect argument.")]
		public SQLiteDataBaseMetaData(DbConnection connection) : this(connection, Dialect.GetDialect()) {}

		public SQLiteDataBaseMetaData(DbConnection connection, Dialect dialect) : base(connection)
		{
			_dialect = dialect;
		}

		private readonly Dialect _dialect;

		public override DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
		{
			if (_dialect is SQLiteDialect)
			{
				// SQLiteDialect concatenates catalog and schema to the table name.
				var actualTablePattern = _dialect.Qualify(catalog, schemaPattern, tableNamePattern);
				var tables = base.GetTables(null, null, actualTablePattern, types);
				// Caller may check the table name of yielded results, we need to patch them
				foreach (DataRow tableRow in tables.Rows)
				{
					var tableName = Convert.ToString(tableRow[ColumnNameForTableName]);
					if (tableName.Equals(actualTablePattern, StringComparison.InvariantCultureIgnoreCase))
					{
						tableRow[ColumnNameForTableName] = tableNamePattern;
						// Columns are looked-up according to the row table name, and schema and catalog data.
						// We need to patch schema and catalog for being able to reconstruct the adequate table name.
						if (!string.IsNullOrEmpty(catalog))
						{
							tableRow["TABLE_CATALOG"] = catalog;
						}
						else
						{
							// SQLite indicates "main" here by default, wrecking the other Get method
							// overrides.
							tableRow["TABLE_CATALOG"] = string.Empty;
						}
						if (!string.IsNullOrEmpty(schemaPattern))
						{
							tableRow["TABLE_SCHEMA"] = schemaPattern;
						}
					}
				}

				return tables;
			}

			return base.GetTables(catalog, schemaPattern, tableNamePattern, types);
		}

		public override DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern)
		{
			if (_dialect is SQLiteDialect)
			{
				// SQLiteDialect concatenates catalog and schema to the table name.
				var actualTablePattern = _dialect.Qualify(catalog, schemaPattern, tableNamePattern);
				return base.GetColumns(null, null, actualTablePattern, columnNamePattern);
			}

			return base.GetColumns(catalog, schemaPattern, tableNamePattern, columnNamePattern);
		}

		public override DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			if (_dialect is SQLiteDialect)
			{
				// SQLiteDialect concatenates catalog and schema to the table name.
				var actualTable = _dialect.Qualify(catalog, schema, table);
				return base.GetForeignKeys(null, null, actualTable);
			}

			return base.GetForeignKeys(catalog, schema, table);
		}

		public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			if (_dialect is SQLiteDialect)
			{
				// SQLiteDialect concatenates catalog and schema to the table name.
				var actualTableName = _dialect.Qualify(catalog, schemaPattern, tableName);
				return base.GetIndexColumns(null, null, actualTableName, indexName);
			}

			return base.GetIndexColumns(catalog, schemaPattern, tableName, indexName);
		}

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new SQLiteTableMetaData(rs, this, extras);
		}
	}

	public class SQLiteTableMetaData : AbstractTableMetadata
	{
		public SQLiteTableMetaData(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras) {}

		protected override IColumnMetadata GetColumnMetadata(DataRow rs)
		{
			return new SQLiteColumnMetaData(rs);
		}

		protected override string GetColumnName(DataRow rs)
		{
			return Convert.ToString(rs["COLUMN_NAME"]);
		}

		protected override string GetConstraintName(DataRow rs)
		{
            return Convert.ToString(rs["CONSTRAINT_NAME"]);
        }

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new SQLiteForeignKeyMetaData(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new SQLiteIndexMetaData(rs);
		}

		protected override string GetIndexName(DataRow rs)
		{
			return Convert.ToString(rs["INDEX_NAME"]);
		}

		protected override void ParseTableInfo(DataRow rs)
		{
			Catalog = Convert.ToString(rs["TABLE_CATALOG"]);
			Schema = Convert.ToString(rs["TABLE_SCHEMA"]);
			if (string.IsNullOrEmpty(Catalog))
			{
				Catalog = null;
			}
			if (string.IsNullOrEmpty(Schema))
			{
				Schema = null;
			}
			Name = Convert.ToString(rs["TABLE_NAME"]);
		}
	}

	public class SQLiteColumnMetaData : AbstractColumnMetaData
	{
		public SQLiteColumnMetaData(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);

			this.SetColumnSize(rs["CHARACTER_MAXIMUM_LENGTH"]);
			this.SetNumericalPrecision(rs["NUMERIC_PRECISION"]);

			Nullable = Convert.ToString(rs["IS_NULLABLE"]);
			TypeName = Convert.ToString(rs["DATA_TYPE"]);
		}
	}

	public class SQLiteIndexMetaData : AbstractIndexMetadata
	{
		public SQLiteIndexMetaData(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class SQLiteForeignKeyMetaData : AbstractForeignKeyMetadata
	{
		public SQLiteForeignKeyMetaData(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["CONSTRAINT_NAME"]);
		}
	}
}
