using System;
using System.Data;

namespace NHibernate.Dialect.Schema
{
	public abstract class AbstractColumnMetaData : IColumnMetadata
	{
		private string name;
		private string typeName;
		private int columnSize;
		private int numericalPrecision;
		private string isNullable;

		public AbstractColumnMetaData(DataRow rs)
		{
		}

		public string Name
		{
			get { return name; }
			protected set { name = value; }
		}

		public string TypeName
		{
			get { return typeName; }
			protected set { typeName = value; }
		}

		public int ColumnSize
		{
			get { return columnSize; }
			protected set { columnSize = value; }
		}

		public int NumericalPrecision
		{
			get { return numericalPrecision; }
			protected set { numericalPrecision = value; }
		}

		public string Nullable
		{
			get { return isNullable; }
			protected set { isNullable = value; }
		}

		public override string ToString()
		{
			return "ColumnMetadata(" + name + ')';
		}

		protected void SetColumnSize(object columnSizeValue)
		{
			if (columnSizeValue != DBNull.Value)
			{
				long longColumnSize = Convert.ToInt64(columnSizeValue);
				columnSize = (int)Math.Min(int.MaxValue, longColumnSize);
			}
		}

		protected void SetNumericalPrecision(object numericalPrecisionValue)
		{
			if (numericalPrecisionValue != DBNull.Value)
			{
				NumericalPrecision = Convert.ToInt32(numericalPrecisionValue);
			}
		}
	}
}