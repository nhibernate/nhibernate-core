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
			Register( DbType.AnsiStringFixedLength, "CHAR(255)");
			Register( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 8000, "VARCHAR($1)" );
			Register( DbType.AnsiString, 2147483647, "TEXT"); // should use the IType.ClobType
			// TODO: figure out how to support this - VARCHAR > 8000 since
			// there is no DbType.CLOB - might just make it a mapping
			// requirement that they specify a sql-type or make NHibernate's
			// own DbType enum or use SqlType as the key for Register
			//Register( DbType.AnsiString, "TEXT" );
			Register( DbType.Binary, "VARBINARY(8000)");
			Register( DbType.Binary, 8000, "VARBINARY($1)");
			Register( DbType.Binary, 2147483647, "IMAGE" );// should use the IType.BlobType
			Register( DbType.Boolean, "BIT" ); //Sybase BIT type does not support null values
			Register( DbType.Byte, "TINYINT" );
			Register( DbType.Currency, "MONEY");
			Register( DbType.Date, "DATETIME");
			Register( DbType.DateTime, "DATETIME" );
			// TODO: figure out if this is the good way to fix the problem
			// with exporting a DECIMAL column
			// NUMERIC(precision, scale) has a hardcoded precision of 19, even though it can range from 1 to 38
			// and the scale has to be 0 <= scale <= precision.
			// I think how I might handle it is keep the type="Decimal(29,5)" and make them specify a 
			// sql-type="decimal(20,5)" if they need to do that.  The Decimal parameter and ddl will get generated
			// correctly with minimal work.
			Register( DbType.Decimal, "DECIMAL(19,5)" ); 
			Register( DbType.Decimal, 19, "DECIMAL(19, $1)");
			Register( DbType.Double, "DOUBLE PRECISION" ); //synonym for FLOAT(53)
			Register( DbType.Guid, "UNIQUEIDENTIFIER" );
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INT" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.Single, "REAL" ); //synonym for FLOAT(24) 
			Register( DbType.StringFixedLength, "NCHAR(255)");
			Register( DbType.StringFixedLength, 4000, "NCHAR($1)");
			Register( DbType.String, "NVARCHAR(255)" );
			Register( DbType.String, 4000, "NVARCHAR($1)" );
			Register( DbType.String, 1073741823, "NTEXT" );// should use the IType.ClobType
			Register( DbType.Time, "DATETIME" );
			
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
			return insertSql.Append( "; SELECT SCOPE_IDENTITY()" );
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

		public override int MaxAnsiStringSize
		{
			get { return 8000; }
		}

		public override int MaxBinarySize
		{
			get { return 8000; }
		}

		public override int MaxStringSize
		{
			get	{ return 4000; }
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
	}
}
