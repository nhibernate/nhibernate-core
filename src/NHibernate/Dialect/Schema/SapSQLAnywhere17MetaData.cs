using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	// Metadata for connections using the Sap.Data.SQLAnywhere.v4.5 ADO.NET provider
	public class SapSqlAnywhere17DataBaseMetaData : AbstractDataBaseSchema
	{
		public SapSqlAnywhere17DataBaseMetaData(DbConnection connection) : base(connection)
		{
		}

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new SapSqlAnywhere17TableMetaData(rs, this, extras);
		}

		public override ISet<string> GetReservedWords()
		{
			var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var dtReservedWords = Connection.GetSchema(DbMetaDataCollectionNames.ReservedWords);
			foreach (DataRow row in dtReservedWords.Rows)
			{
				result.Add(row["reserved_word"].ToString());
			}

			return result;
		}

		public override DataTable GetTables(
			string catalog,
			string schemaPattern,
			string tableNamePattern,
			string[] types)
		{
			var restrictions = new[] { schemaPattern, tableNamePattern, null };
			var objTbl = Connection.GetSchema("Tables", restrictions);
			return objTbl;
		}

		public override DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			var restrictions = new[] { schemaPattern, tableName, null };
			var objTbl = Connection.GetSchema("Indexes", restrictions);
			return objTbl;
		}

		public override DataTable GetIndexColumns(
			string catalog,
			string schemaPattern,
			string tableName,
			string indexName)
		{
			var restrictions = new[] { schemaPattern, tableName, indexName, null };
			var objTbl = Connection.GetSchema("IndexColumns", restrictions);
			return objTbl;
		}

		public override DataTable GetColumns(
			string catalog,
			string schemaPattern,
			string tableNamePattern,
			string columnNamePattern)
		{
			var restrictions = new[] { schemaPattern, tableNamePattern, null };
			var objTbl = Connection.GetSchema("Columns", restrictions);
			return objTbl;
		}

		public override DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			var restrictions = new[] { schema, table, null };
			var objTbl = Connection.GetSchema("ForeignKeys", restrictions);
			return objTbl;
		}
	}

	public class SapSqlAnywhere17TableMetaData : AbstractTableMetadata
	{
		public SapSqlAnywhere17TableMetaData(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras)
		{
		}

		protected override IColumnMetadata GetColumnMetadata(DataRow rs)
		{
			return new SapSqlAnywhere17ColumnMetaData(rs);
		}

		protected override string GetColumnName(DataRow rs)
		{
			return Convert.ToString(rs["COLUMN_NAME"]);
		}

		protected override string GetConstraintName(DataRow rs)
		{
			// There is no thing like a constraint name for Anywhere - so
			// we just use the column name here ...
			return Convert.ToString(rs["COLUMN_NAME"]);
		}

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new SapSqlAnywhere17ForeignKeyMetaData(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new SapSqlAnywhere17IndexMetaData(rs);
		}

		protected override string GetIndexName(DataRow rs)
		{
			return (string) rs["INDEX_NAME"];
		}

		protected override void ParseTableInfo(DataRow rs)
		{
			Catalog = null;
			Schema = Convert.ToString(rs["TABLE_SCHEMA"]);
			if (string.IsNullOrEmpty(Schema))
			{
				Schema = null;
			}

			Name = Convert.ToString(rs["TABLE_NAME"]);
		}
	}

	public class SapSqlAnywhere17ColumnMetaData : AbstractColumnMetaData
	{
		public SapSqlAnywhere17ColumnMetaData(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);

			SetColumnSize(rs["CHARACTER_MAXIMUM_LENGTH"]);
			SetNumericalPrecision(rs["NUMERIC_PRECISION"]);

			Nullable = Convert.ToString(rs["IS_NULLABLE"]);
			TypeName = Convert.ToString(rs["DATA_TYPE"]);
		}
	}

	public class SapSqlAnywhere17IndexMetaData : AbstractIndexMetadata
	{
		public SapSqlAnywhere17IndexMetaData(DataRow rs) : base(rs)
		{
			Name = (string) rs["INDEX_NAME"];
		}
	}

	public class SapSqlAnywhere17ForeignKeyMetaData : AbstractForeignKeyMetadata
	{
		public SapSqlAnywhere17ForeignKeyMetaData(DataRow rs) : base(rs)
		{
			// There is no thing like a constraint name for Anywhere - so
			// we just use the column name here ...
			Name = (string) rs["COLUMN_NAME"];
		}
	}
}
