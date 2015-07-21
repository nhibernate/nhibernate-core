using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class MsSqlDataBaseSchema: AbstractDataBaseSchema
	{
		public MsSqlDataBaseSchema(DbConnection connection) : base(connection) {}

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new MsSqlTableMetadata(rs, this, extras);
		}
	}

	public class MsSqlTableMetadata: AbstractTableMetadata
	{
		public MsSqlTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras) { }

		protected override void ParseTableInfo(DataRow rs)
		{
			// Clearly, we cannot use the same names when connected via ODBC...
			Catalog = SchemaHelper.GetString(rs, "TABLE_CATALOG", "TABLE_CAT");
			Schema = SchemaHelper.GetString(rs, "TABLE_SCHEMA", "TABLE_SCHEM");
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
			return new MsSqlColumnMetadata(rs);
		}

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new MsSqlForeignKeyMetadata(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new MsSqlIndexMetadata(rs);
		}
	}

	public class MsSqlColumnMetadata : AbstractColumnMetaData
	{
		public MsSqlColumnMetadata(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);

			// Clearly, we cannot use the same names when connected via ODBC...
			this.SetColumnSize(SchemaHelper.GetValue(rs, "CHARACTER_MAXIMUM_LENGTH", "COLUMN_SIZE"));
			this.SetNumericalPrecision(SchemaHelper.GetValue(rs, "NUMERIC_PRECISION", "COLUMN_SIZE"));

			Nullable = Convert.ToString(rs["IS_NULLABLE"]);

			// For the type name, DATA_TYPE is numeric when using ODBC, so use the
			// string-valued ODBC-only TYPE_NAME as first alternative.
			TypeName = SchemaHelper.GetString(rs, "TYPE_NAME", "DATA_TYPE");
		}
	}

	public class MsSqlIndexMetadata: AbstractIndexMetadata
	{
		public MsSqlIndexMetadata(DataRow rs) : base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class MsSqlForeignKeyMetadata : AbstractForeignKeyMetadata
	{
		public MsSqlForeignKeyMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["CONSTRAINT_NAME"]);
		}
	}
}
