using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Dialect 
{
	/// <summary>
	/// An SQL dialect compatible with Microsoft SQL Server 2000.
	/// </summary>
	public class MsSql2000Dialect: Dialect 
	{

		public MsSql2000Dialect() : base() 
		{
			/*
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, NO_BATCH);
			*/
		}

		public override string AddColumnString 
		{
			get { return "add"; }
		}
		public override string NullColumnString 
		{
			get { return " null"; }
		}
		public override bool QualifyIndexName 
		{
			get { return false; }
		}
	
		public override bool SupportsForUpdate 
		{
			get { return false; }
		}
	
		public override bool SupportsIdentityColumns 
		{
			get { return true; }
		}
		public override string IdentitySelectString 
		{
			get { return "select @@identity"; }
		}
		public override string IdentityColumnString 
		{
			get { return "IDENTITY NOT NULL"; } 
		}

		public override string NoColumnsInsertString 
		{
			get { return "DEFAULT VALUES"; }
		}

		[Obsolete("See the Dialect class for reason")]
		public override bool UseNamedParameters 
		{
			get { return true; }
		}

		[Obsolete("See the Dialect class for reason")]
		public override string NamedParametersPrefix 
		{
			get { return "@"; }
		}						

		public override string QuoteForTableName(string tableName)
		{
			return "[" + tableName + "]";
		}

		public override string QuoteForAliasName(string aliasName)
		{
			return "[" + aliasName + "]";
		}

		public override string QuoteForColumnName(string columnName)
		{
			return "[" + columnName + "]";
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
			if(sqlType.Length <= 8000) 
			{
				return SqlTypeToString("VARCHAR", sqlType.Length);
			}
			else 
			{
				return "TEXT"; // should use the IType.ClobType
			}
		}

		protected override string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType) 
		{
			
			if(sqlType.Length <= 8000) 
			{
				return SqlTypeToString("CHAR", sqlType.Length);
			}
			else 
			{
				return "TEXT"; // should use the IType.ClobType
			}
					
		}

		protected override string SqlTypeToString(BinarySqlType sqlType) 
		{
			
			if(sqlType.Length <= 8000) 
			{
				return SqlTypeToString("VARBINARY", sqlType.Length);
			}
			else 
			{
				return "IMAGE"; // should use the IType.BlobType
			}
					
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
			return "INT";
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
			
			if(sqlType.Length <= 4000) 
			{
				return SqlTypeToString("NCHAR", sqlType.Length);
			}
			else 
			{
				return "NTEXT"; // should use the IType.ClobType
			}
					
		}

		protected override string SqlTypeToString(StringSqlType sqlType) {
			
			if(sqlType.Length <= 4000) 
			{
				return SqlTypeToString("NVARCHAR", sqlType.Length);
			}
			else 
			{
				return "NTEXT";
			}
					
		}

	}
}
