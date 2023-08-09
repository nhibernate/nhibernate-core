using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class DB2MetaData : AbstractDataBaseSchema
	{
		public DB2MetaData(DbConnection connection) : base(connection)
		{
		}

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new DB2TableMetaData(rs, this, extras);
		}

		public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			var restrictions = new[] {tableName, indexName};
			return Connection.GetSchema("Indexes", restrictions);
		}

		public override ISet<string> GetReservedWords()
		{
			var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var dtReservedWords = Connection.GetSchema(DbMetaDataCollectionNames.ReservedWords);
			foreach (DataRow row in dtReservedWords.Rows)
			{
				result.Add(row["ReservedWord"].ToString());
			}

			if (IncludeDataTypesInReservedWords)
			{
				var dtTypes = Connection.GetSchema(DbMetaDataCollectionNames.DataTypes);
				foreach (DataRow row in dtTypes.Rows)
				{
					result.Add(row["SQL_TYPE_NAME"].ToString());
				}
			}

			return result;
		}
	}

	public class DB2TableMetaData : AbstractTableMetadata
	{
		public DB2TableMetaData(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras)
		{
		}

		protected override IColumnMetadata GetColumnMetadata(DataRow rs)
		{
			return new DB2ColumnMetaData(rs);
		}

		protected override string GetColumnName(DataRow rs)
		{
			return Convert.ToString(rs["COLUMN_NAME"]);
		}

		protected override string GetConstraintName(DataRow rs)
		{
			return Convert.ToString(rs["FK_NAME"]);
		}

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new DB2ForeignKeyMetaData(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new DB2IndexMetaData(rs);
		}

		protected override string GetIndexName(DataRow rs)
		{
			return Convert.ToString(rs["INDEX_NAME"]);
		}

		protected override void ParseTableInfo(DataRow rs)
		{
			Catalog = Convert.ToString(rs["TABLE_CATALOG"]);
			Schema = Convert.ToString(rs["TABLE_SCHEMA"]);
			if (string.IsNullOrEmpty(Catalog)) Catalog = null;
			if (string.IsNullOrEmpty(Schema)) Schema = null;
			Name = Convert.ToString(rs["TABLE_NAME"]);
		}
	}

	public class DB2ColumnMetaData : AbstractColumnMetaData
	{
		public DB2ColumnMetaData(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);
			Nullable = Convert.ToString(rs["IS_NULLABLE"]);
			TypeName = Convert.ToString(rs["DATA_TYPE_NAME"]);
		}
	}

	public class DB2IndexMetaData : AbstractIndexMetadata
	{
		public DB2IndexMetaData(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class DB2ForeignKeyMetaData : AbstractForeignKeyMetadata
	{
		public DB2ForeignKeyMetaData(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["FK_NAME"]);
		}
	}
}
