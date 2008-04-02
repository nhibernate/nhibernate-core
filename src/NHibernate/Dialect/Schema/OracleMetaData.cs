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
			return new MsSqlTableMetadata(rs, this, extras);
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
			return Convert.ToString(rs["FOREIGN_KEY_CONSTRIANT_NAME"]);
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
			object aValue;

			aValue = rs["LENGTH"];
			if (aValue != DBNull.Value)
				ColumnSize = Convert.ToInt32(aValue);

			aValue = rs["PRECISION"];
			if (aValue != DBNull.Value)
				NumericalPrecision = Convert.ToInt32(aValue);

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
			Name = Convert.ToString(rs["FOREIGN_KEY_CONSTRIANT_NAME"]);
		}
	}
}
