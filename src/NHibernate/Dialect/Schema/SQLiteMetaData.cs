using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class SQLiteDataBaseMetaData : AbstractDataBaseSchema
	{
		public SQLiteDataBaseMetaData(DbConnection connection) : base(connection) {}

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