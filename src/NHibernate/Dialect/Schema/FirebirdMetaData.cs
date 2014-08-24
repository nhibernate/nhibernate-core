using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	public class FirebirdDataBaseSchema : AbstractDataBaseSchema
	{
		public FirebirdDataBaseSchema(DbConnection connection) : base(connection) { }

		public override bool StoresUpperCaseIdentifiers
		{
			get { return true; }
		}

		public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
		{
			return new FirebirdTableMetadata(rs, this, extras);
		}
	}

	public class FirebirdTableMetadata : AbstractTableMetadata
	{
		public FirebirdTableMetadata(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras) { }

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
			return new FirebirdColumnMetadata(rs);
		}

		protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
		{
			return new FirebirdForeignKeyMetadata(rs);
		}

		protected override IIndexMetadata GetIndexMetadata(DataRow rs)
		{
			return new FirebirdIndexMetadata(rs);
		}
	}

	public class FirebirdColumnMetadata : AbstractColumnMetaData
	{
		public FirebirdColumnMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["COLUMN_NAME"]);

			this.SetColumnSize(rs["COLUMN_SIZE"]);
			this.SetNumericalPrecision(rs["NUMERIC_PRECISION"]);

			Nullable = Convert.ToString(rs["IS_NULLABLE"]);
			TypeName = Convert.ToString(rs["COLUMN_DATA_TYPE"]);
		}
	}

	public class FirebirdIndexMetadata : AbstractIndexMetadata
	{
		public FirebirdIndexMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["INDEX_NAME"]);
		}
	}

	public class FirebirdForeignKeyMetadata : AbstractForeignKeyMetadata
	{
		public FirebirdForeignKeyMetadata(DataRow rs)
			: base(rs)
		{
			Name = Convert.ToString(rs["CONSTRAINT_NAME"]);
		}
	}

}
