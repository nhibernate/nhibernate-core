using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class HanaDataBaseSchema : AbstractDataBaseSchema
	{
		public HanaDataBaseSchema(DbConnection connection) : base(connection)
		{
		}

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new HanaTableMetadata(rs, this, extras);
		}

		public override ISet<string> GetReservedWords()
		{
			var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			DataTable dtReservedWords = Connection.GetSchema(DbMetaDataCollectionNames.ReservedWords);
			foreach (DataRow row in dtReservedWords.Rows)
			{
				result.Add(row["reserved_word"].ToString());
			}

			if (IncludeDataTypesInReservedWords)
			{
				DataTable dtTypes = Connection.GetSchema(DbMetaDataCollectionNames.DataTypes);
				foreach (DataRow row in dtTypes.Rows)
				{
					result.Add(row["TypeName"].ToString());
				}
			}

			return result;
		}
		
		public override DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
		{
			var restrictions = new[] {schemaPattern, tableNamePattern, null};
			DataTable objTbl = Connection.GetSchema("Tables", restrictions);
			return objTbl;
		}

		public override DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			var restrictions = new[] {schemaPattern, tableName, null};
			DataTable objTbl = Connection.GetSchema("Indexes", restrictions);
			return objTbl;
		}

		public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			var restrictions = new[] {schemaPattern, tableName, indexName, null};
			DataTable objTbl = Connection.GetSchema("IndexColumns", restrictions);
			return objTbl;
		}

		public override DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern,
												string columnNamePattern)
		{
			var restrictions = new[] {schemaPattern, tableNamePattern, null};
			DataTable objTbl = Connection.GetSchema("Columns", restrictions);
			return objTbl;
		}

		public override DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			var restrictions = new[] {schema, table, null};
			DataTable objTbl = Connection.GetSchema("ForeignKeys", restrictions);
			return objTbl;
		}

		public override bool StoresUpperCaseIdentifiers
		{
			get { return true; }
		}
	}

	public class HanaTableMetadata : AbstractTableMetadata
	{
		public HanaTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras)
		{
		}

		protected override void ParseTableInfo(DataRow rs)
		{
			Catalog = null;
			Schema = Convert.ToString(rs["TABLE_SCHEMA"]);
			if (string.IsNullOrEmpty(Schema)) Schema = null;
			Name = Convert.ToString(rs["TABLE_NAME"]);
		}

		protected override string GetConstraintName(DataRow rs)
		{
		    return Convert.ToString(rs["CONSTRAINT_NAME"]);
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
			return new HanaColumnMetadata(rs);
		}

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new HanaForeignKeyMetadata(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new HanaIndexMetadata(rs);
		}
	}

	public class HanaColumnMetadata : AbstractColumnMetaData
	{
		public HanaColumnMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);
			
			this.SetColumnSize(rs["CHARACTER_MAXIMUM_LENGTH"]);
			this.SetNumericalPrecision(rs["NUMERIC_PRECISION"]);

			Nullable = Convert.ToString(rs["IS_NULLABLE"]);
			TypeName = Convert.ToString(rs["DATA_TYPE"]);
		}
	}

	public class HanaIndexMetadata : AbstractIndexMetadata
	{
		public HanaIndexMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class HanaForeignKeyMetadata : AbstractForeignKeyMetadata
	{
		public HanaForeignKeyMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["CONSTRAINT_NAME"]);
		}
	}
}
