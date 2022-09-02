using System;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace NHibernate.Dialect.Schema
{
	public class PostgreSQLDataBaseMetadata : AbstractDataBaseSchema
	{
		public PostgreSQLDataBaseMetadata(DbConnection connection) : base(connection) { }

		public override bool IncludeDataTypesInReservedWords => false;

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new PostgreSQLTableMetadata(rs, this, extras);
		}

		public override bool StoresMixedCaseQuotedIdentifiers
		{
			get { return false; }
		}

		public override bool StoresLowerCaseIdentifiers
		{
			get { return true; }
		}

		public override DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern)
		{
			var table = base.GetColumns(catalog, schemaPattern, tableNamePattern, columnNamePattern);

			// Unlike MSSQL, the Postgresql data provider doesn't override the culture.
			// This may cause problems in e.g. Turkish culture due to different casing rules.
			table.Locale = CultureInfo.InvariantCulture;
			return table;
		}

		public override DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
		{
			var table = base.GetTables(catalog, schemaPattern, tableNamePattern, types);

			// Unlike MSSQL, the Postgresql data provider doesn't override the culture.
			// This may cause problems in e.g. Turkish culture due to different casing rules.
			table.Locale = CultureInfo.InvariantCulture;
			return table;
		}

		public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			var table = base.GetIndexColumns(catalog, schemaPattern, tableName, indexName);

			// Unlike MSSQL, the Postgresql data provider doesn't override the culture.
			// This may cause problems in e.g. Turkish culture due to different casing rules.
			table.Locale = CultureInfo.InvariantCulture;
			return table;
		}

		public override DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			var indexInfo = base.GetIndexInfo(catalog, schemaPattern, tableName);

			// Unlike MSSQL, the Postgresql data provider doesn't override the culture.
			// This may cause problems in e.g. Turkish culture due to different casing rules.
			indexInfo.Locale = CultureInfo.InvariantCulture;
			return indexInfo;
		}

		public override DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			var foreignKeys = base.GetForeignKeys(catalog, schema, table);

			// Unlike MSSQL, the Postgresql data provider doesn't override the culture.
			// This may cause problems in e.g. Turkish culture due to different casing rules.
			foreignKeys.Locale = CultureInfo.InvariantCulture;
			return foreignKeys;
		}
	}

	public class PostgreSQLTableMetadata : AbstractTableMetadata
	{
		public PostgreSQLTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras) { }

		protected override IColumnMetadata GetColumnMetadata(DataRow rs)
		{
			return new PostgreSQLColumnMetadata(rs);
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
			return new PostgreSQLForeignKeyMetadata(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new PostgreSQLIndexMetadata(rs);
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

	public class PostgreSQLColumnMetadata : AbstractColumnMetaData
	{
		public PostgreSQLColumnMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);

			this.SetColumnSize(rs["CHARACTER_MAXIMUM_LENGTH"]);
			this.SetNumericalPrecision(rs["NUMERIC_PRECISION"]);

			Nullable = Convert.ToString(rs["IS_NULLABLE"]);
			TypeName = NormalizeTypeNames(Convert.ToString(rs["DATA_TYPE"]));
		}

		private static string NormalizeTypeNames(string typeName)
		{
			switch (typeName)
			{
				case "double precision":
					return "float8";
				case "real":
					return "float4";
				case "smallint":
					return "int2";
				case "integer":
					return "int4";
				case "bigint":
					return "int8";
			}

			if (typeName.StartsWith("character", StringComparison.Ordinal))
			{
				return typeName.Replace("character varying", "varchar").Replace("character", "char");
			}

			if (typeName.EndsWith(" with time zone", StringComparison.Ordinal))
			{
				return typeName.StartsWith("timestamp", StringComparison.Ordinal)
					? typeName.Replace(" with time zone", string.Empty).Replace("timestamp", "timestamptz")
					: typeName.Replace(" with time zone", string.Empty).Replace("time", "timetz");
			}

			if (typeName.EndsWith(" without time zone", StringComparison.Ordinal))
			{
				return typeName.Replace(" without time zone", string.Empty);
			}

			return typeName;
		}
	}

	public class PostgreSQLIndexMetadata : AbstractIndexMetadata
	{
		public PostgreSQLIndexMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class PostgreSQLForeignKeyMetadata : AbstractForeignKeyMetadata
	{
		public PostgreSQLForeignKeyMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["CONSTRAINT_NAME"]);
		}
	}
}
