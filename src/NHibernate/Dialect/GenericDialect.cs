using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Dialect {

	/// <summary>
	/// A generic SQL dialect which may or may not work on any actual databases
	/// </summary>
	public class GenericDialect : Dialect {

		public GenericDialect() : base() {
		}

		public override string AddColumnString {
			get { return "add column"; }
		}

		private string SqlTypeToString(string name, int length) 
		{
			return name + "(" + length + ")";
		}

		private string SqlTypeToString(string name, int precision, int scale) 
		{
			return name + "(" + precision + ", " + scale + ")";
		}

		protected override string SqlTypeToString(AnsiStringSqlType sqlType)
		{
			return SqlTypeToString("VARCHAR", sqlType.Length);
		}

		protected override string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType) 
		{
			return SqlTypeToString("CHAR", sqlType.Length);	
		}

		protected override string SqlTypeToString(BinarySqlType sqlType) 
		{
			return SqlTypeToString("VARBINARY", sqlType.Length);	
		}
		
		protected override string SqlTypeToString(BooleanSqlType sqlType)
		{
			return "BIT";
		}

		
		protected override string SqlTypeToString(ByteSqlType sqlType)
		{
			return "TINYINT";
		}

		protected override string SqlTypeToString(CurrencySqlType sqlType)
		{
			return "MONEY";
		}

		protected override string SqlTypeToString(DateSqlType sqlType)
		{
			return "DATETIME";
		}

		protected override string SqlTypeToString(DateTimeSqlType sqlType)
		{
			return "DATETIME";
		}

		protected override string SqlTypeToString(TimeSqlType sqlType)
		{
			return "DATETIME";
		}

		protected override string SqlTypeToString(DecimalSqlType sqlType)
		{
			return SqlTypeToString("DECIMAL", sqlType.Precision, sqlType.Scale);
		}

		protected override string SqlTypeToString(DoubleSqlType sqlType)
		{
			return SqlTypeToString("FLOAT", sqlType.Length);
		}

		protected override string SqlTypeToString(GuidSqlType sqlType)
		{
			return "UNIQUEIDENTIFIER";
		}

		protected override string SqlTypeToString(Int16SqlType sqlType)
		{
			return "SMALLINT";
		}

		protected override string SqlTypeToString(Int32SqlType sqlType)
		{
			return "INTEGER";
		}

		protected override string SqlTypeToString(Int64SqlType sqlType)
		{
			return "BIGINT";
		}

		protected override string SqlTypeToString(SingleSqlType sqlType)
		{
			return SqlTypeToString("FLOAT", sqlType.Length);
		}

		protected override string SqlTypeToString(StringFixedLengthSqlType sqlType) 
		{
			return SqlTypeToString("NCHAR", sqlType.Length);		
		}

		protected override string SqlTypeToString(StringSqlType sqlType) 
		{
			return SqlTypeToString("NVARCHAR", sqlType.Length);
		}
	}
}
