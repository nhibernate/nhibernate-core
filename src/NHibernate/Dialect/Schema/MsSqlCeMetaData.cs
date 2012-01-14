using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class MsSqlCeDataBaseSchema: AbstractDataBaseSchema
	{
		public MsSqlCeDataBaseSchema(DbConnection connection) : base(connection) {}

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new MsSqlCeTableMetadata(rs, this, extras);
		}
	}

	public class MsSqlCeTableMetadata: AbstractTableMetadata
	{
		public MsSqlCeTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras) { }

		protected override void ParseTableInfo(DataRow rs)
		{
			Catalog = Convert.ToString(rs["TABLE_CATALOG"]);
			Schema = Convert.ToString(rs["TABLE_SCHEMA"]);
			if (string.IsNullOrEmpty(Catalog)) Catalog = null;
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
			return new MsSqlCeColumnMetadata(rs);
		}

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new MsSqlCeForeignKeyMetadata(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new MsSqlCeIndexMetadata(rs);
		}
	}

	public class MsSqlCeColumnMetadata : AbstractColumnMetaData
	{
		public MsSqlCeColumnMetadata(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);

			object aValue = rs["CHARACTER_MAXIMUM_LENGTH"];
			if (aValue != DBNull.Value)
			{
				long originalColumnSize = Convert.ToInt64(aValue);
				if (originalColumnSize > (long)int.MaxValue)
				{
					ColumnSize = int.MaxValue;
				}
				else
				{
					ColumnSize = (int)originalColumnSize;
				}
			}

			aValue = rs["NUMERIC_PRECISION"];
			if (aValue != DBNull.Value)
				NumericalPrecision = Convert.ToInt32(aValue);

			Nullable = Convert.ToString(rs["IS_NULLABLE"]);
			TypeName = Convert.ToString(rs["DATA_TYPE"]);			
		}
	}

	public class MsSqlCeIndexMetadata: AbstractIndexMetadata
	{
		public MsSqlCeIndexMetadata(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class MsSqlCeForeignKeyMetadata : AbstractForeignKeyMetadata
	{
		public MsSqlCeForeignKeyMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["CONSTRAINT_NAME"]);
		}
	}
}
