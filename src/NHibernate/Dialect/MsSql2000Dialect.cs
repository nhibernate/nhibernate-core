using System;
using System.Data;

using NHibernate.SqlCommand;
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
			DefaultProperties[Cfg.Environment.OuterJoin] = "true";
			DefaultProperties[Cfg.Environment.StatementBatchSize] = NoBatch;
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
	
		/// <summary>
		/// MsSql allows the use of SELECT SCOPE_IDENTITY to be in the same
		/// Command as the INSERT
		/// </summary>
		/// <value>true</value>
		public override bool SupportsIdentitySelectInInsert
		{
			get	{ return true;	}
		}
		
		/// <summary>
		/// Add the Identity Select string to the Insert Sql.
		/// </summary>
		/// <param name="insertSql">The SqlString that contains the INSERT sql.</param>
		/// <returns>A new SqlString with <c>; SELECT SCOPE_IDENTITY()</c> at the end.</returns>
		public override SqlString AddIdentitySelectToInsert(SqlString insertSql)
		{
			return insertSql.Append( new SqlString("; SELECT SCOPE_IDENTITY()") );
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

		protected override char CloseQuote
		{
			get { return ']';}
		}

		protected override char OpenQuote
		{
			get { return '[';}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>
		/// MsSql does not require the OpenQuote to be escaped as long as the first char
		/// is an OpenQuote.
		/// </remarks>
		protected override string Quote(string name) 
		{
			return OpenQuote + name.Replace(CloseQuote.ToString(), new string(CloseQuote, 2) ) + CloseQuote;
		}

		public override string UnQuote(string quoted)
		{
			if ( IsQuoted(quoted) )
				quoted = quoted.Substring(1,quoted.Length - 2);

			return quoted.Replace( new string(CloseQuote, 2), CloseQuote.ToString() );
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
