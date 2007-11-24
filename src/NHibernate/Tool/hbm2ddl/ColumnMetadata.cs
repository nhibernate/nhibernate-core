namespace NHibernate.Tool.hbm2ddl
{
	using System;
	using System.Data;
	using Util;

	public class ColumnMetadata
	{
		private readonly String name;
		private readonly String typeName;
		private readonly int columnSize;
		private readonly int numericalPrecision;
		private readonly String isNullable;

		public ColumnMetadata(DataRow rs)
		{
			name = (string) rs["COLUMN_NAME"];
			if (rs["CHARACTER_MAXIMUM_LENGTH"]!=DBNull.Value)
				columnSize = (int)rs["CHARACTER_MAXIMUM_LENGTH"];
			if (rs["NUMERIC_PRECISION"] != DBNull.Value)
				numericalPrecision = Convert.ToInt32(rs["NUMERIC_PRECISION"]);
			isNullable = (string) rs["IS_NULLABLE"];
			typeName = (string)rs["DATA_TYPE"];
		}

		public string Name
		{
			get { return name; }
		}

		public string TypeName
		{
			get { return typeName; }
		}

		public int ColumnSize
		{
			get { return columnSize; }
		}

		public int NumericalPrecision
		{
			get { return numericalPrecision; }
		}

		public string Nullable
		{
			get { return isNullable; }
		}

		public override String ToString()
		{
			return "ColumnMetadata(" + name + ')';
		}

	}
}
