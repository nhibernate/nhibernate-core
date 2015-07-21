using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class OracleDataBaseSchema : AbstractDataBaseSchema
	{
		public OracleDataBaseSchema(DbConnection connection) : base(connection) { }

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new OracleTableMetadata(rs, this, extras);
		}

		public override bool StoresUpperCaseIdentifiers
		{
			get { return true; }
		}

		public override DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
		{
			string owner = string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern;
			var restrictions = new[] { owner, tableNamePattern };
			return Connection.GetSchema("Tables", restrictions);
		}

		public override DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern)
		{
			string owner = string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern;
			var restrictions = new[] { owner, tableNamePattern, columnNamePattern };
			return Connection.GetSchema("Columns", restrictions);
		}

		public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			string owner = string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern;
			var restrictions = new[] { owner, indexName, null, tableName, null };
			return Connection.GetSchema("IndexColumns", restrictions);
		}

		public override DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			string owner = string.IsNullOrEmpty(schemaPattern) ? null : schemaPattern;
			var restrictions = new[] { owner, null, null, tableName };
			return Connection.GetSchema("Indexes", restrictions);
		}

		public override DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			string owner = string.IsNullOrEmpty(schema) ? null : schema;
			var restrictions = new[] { owner, table, null };
			return Connection.GetSchema("ForeignKeys", restrictions);
		}
	}

	public class OracleTableMetadata : AbstractTableMetadata
	{
		public OracleTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras) { }

		protected override void ParseTableInfo(DataRow rs)
		{
			Catalog = null;
			Schema = Convert.ToString(rs["OWNER"]);
			if (string.IsNullOrEmpty(Schema)) Schema = null;
			Name = Convert.ToString(rs["TABLE_NAME"]);
		}

		protected override string GetConstraintName(DataRow rs)
		{
			return Convert.ToString(rs["FOREIGN_KEY_CONSTRAINT_NAME"]);
		}

		protected override string GetColumnName(DataRow rs)
		{
			return Convert.ToString(rs["COLUMN_NAME"]);
		}

		protected override string GetIndexName(DataRow rs)
		{
			return Convert.ToString(rs["INDEX_NAME"]);
		}

		protected override IColumnMetadata GetColumnMetadata(DataRow rs)
		{
			return new OracleColumnMetadata(rs);
		}

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new OracleForeignKeyMetadata(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new OracleIndexMetadata(rs);
		}
	}

	public class OracleColumnMetadata : AbstractColumnMetaData
	{
		public OracleColumnMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);

			this.SetColumnSize(rs["LENGTH"]);
			this.SetNumericalPrecision(rs["PRECISION"]);

			Nullable = Convert.ToString(rs["NULLABLE"]);
			TypeName = Convert.ToString(rs["DATATYPE"]);
		}
	}

	public class OracleIndexMetadata : AbstractIndexMetadata
	{
		public OracleIndexMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class OracleForeignKeyMetadata : AbstractForeignKeyMetadata
	{
		public OracleForeignKeyMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["FOREIGN_KEY_CONSTRAINT_NAME"]);
		}
	}
}
