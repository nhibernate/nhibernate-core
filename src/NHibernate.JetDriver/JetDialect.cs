using System;
using System.Data;
using NHibernate.SqlCommand;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.JetDriver
{
	/// <summary>
	/// Dialect for Jet database engine.
	/// 
	/// <p>
	/// Author: <a href="mailto:lukask@welldatatech.com">Lukas Krejci</a>
	/// </p>
	/// </summary>
	public class JetDialect : Dialect.Dialect
	{
		/// <summary>
		/// 
		/// </summary>
		public JetDialect()
			: base()
		{
			//BINARY 1 byte per character Any type of data may be stored in a field of this type. No translation of the data (for example, to text) is made. How the data is input in a binary field dictates how it will appear as output. 
			//BIT 1 byte Yes and No values and fields that contain only one of two values. 
			//TINYINT 1 byte An integer value between 0 and 255. 
			//MONEY 8 bytes A scaled integer between
			//– 922,337,203,685,477.5808 and 922,337,203,685,477.5807. 
			//DATETIME
			//(See DOUBLE) 8 bytes A date or time value between the years 100 and 9999. 
			//UNIQUEIDENTIFIER 128 bits A unique identification number used with remote procedure calls. 
			//REAL 4 bytes A single-precision floating-point value with a range of – 3.402823E38 to – 1.401298E-45 for negative values, 1.401298E-45 to 3.402823E38 for positive values, and 0. 
			//FLOAT 8 bytes A double-precision floating-point value with a range of – 1.79769313486232E308 to – 4.94065645841247E-324 for negative values, 4.94065645841247E-324 to 1.79769313486232E308 for positive values, and 0. 
			//SMALLINT 2 bytes A short integer between – 32,768 and 32,767. (See Notes) 
			//INTEGER 4 bytes A long integer between – 2,147,483,648 and 2,147,483,647. (See Notes) 
			//DECIMAL 17 bytes An exact numeric data type that holds values from 1028 - 1 through - 1028 - 1. You can define both precision (1 - 28) and scale (0 - defined precision). The default precision and scale are 18 and 0, respectively. 
			//TEXT 2 bytes per character (See Notes) Zero to a maximum of 2.14 gigabytes. 
			//IMAGE As required Zero to a maximum of 2.14 gigabytes. Used for OLE objects. 
			//CHARACTER 

			//Although it is clearly stated in MS Access documentation, that Jet engine supports TINYINT, it is actually not true.
			//Byte size number datatype is called BYTE.

			RegisterColumnType( DbType.AnsiStringFixedLength, "CHAR(255)" );
			RegisterColumnType( DbType.AnsiStringFixedLength, 255, "CHAR($1)" );
			RegisterColumnType( DbType.AnsiString, "TEXT(255)" );
			RegisterColumnType( DbType.AnsiString, 255, "TEXT($1)" );
			RegisterColumnType( DbType.AnsiString, 1073741823, "TEXT($1)" );
			RegisterColumnType( DbType.AnsiString, 1073741823, "TEXT" );
			RegisterColumnType( DbType.Binary, "IMAGE" );
			//RegisterColumnType( DbType.Binary, 8000, "VARBINARY($1)" );
			RegisterColumnType( DbType.Binary, 2147483647, "IMAGE" );
			RegisterColumnType( DbType.Boolean, "BIT" );
			RegisterColumnType( DbType.Byte, "BYTE" );
			RegisterColumnType( DbType.Currency, "MONEY" );
			RegisterColumnType( DbType.Date, "DATETIME" );
			RegisterColumnType( DbType.DateTime, "DATETIME" );
			// TODO: figure out if this is the good way to fix the problem
			// with exporting a DECIMAL column
			// NUMERIC(precision, scale) has a hardcoded precision of 19, even though it can range from 1 to 38
			// and the scale has to be 0 <= scale <= precision.
			// I think how I might handle it is keep the type="Decimal(29,5)" and make them specify a 
			// sql-type="decimal(20,5)" if they need to do that.  The Decimal parameter and ddl will get generated
			// correctly with minimal work.
			RegisterColumnType( DbType.Decimal, "DECIMAL(19,5)" );
			RegisterColumnType( DbType.Decimal, 19, "DECIMAL(19, $1)" );
			RegisterColumnType( DbType.Double, "FLOAT" );
			RegisterColumnType( DbType.Guid, "UNIQUEIDENTIFIER" );
			RegisterColumnType( DbType.Int16, "SMALLINT" );
			RegisterColumnType( DbType.Int32, "INT" );
			RegisterColumnType( DbType.Int64, "INT" ); //this is dangerous, I know
			RegisterColumnType( DbType.Single, "REAL" );
			RegisterColumnType( DbType.StringFixedLength, "CHAR(255)" );
			RegisterColumnType( DbType.StringFixedLength, 1073741823, "TEXT($1)" );
			RegisterColumnType( DbType.String, "TEXT(255)" );
			RegisterColumnType( DbType.String, 255, "TEXT($1)" );
			RegisterColumnType( DbType.String, 1073741823, "TEXT($1)" );
			RegisterColumnType( DbType.String, 1073741823, "TEXT" );
			RegisterColumnType( DbType.Time, "DATETIME" );

/*
			RegisterFunction("abs", new StandardSQLFunction() );
			RegisterFunction("absval", new StandardSQLFunction() );
			RegisterFunction("sign", new StandardSQLFunction( NHibernateUtil.Int32 ) );

			RegisterFunction("ceiling", new StandardSQLFunction() );
			RegisterFunction("ceil", new StandardSQLFunction() );
			RegisterFunction("floor", new StandardSQLFunction() );
			RegisterFunction("round", new StandardSQLFunction() );

			RegisterFunction("acos", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("asin", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("atan", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("cos", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("cot", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("degrees", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("exp", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("float", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("hex", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("ln", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("log", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("log10", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("radians", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("rand", new NoArgSQLFunction( NHibernateUtil.Double ));
			RegisterFunction("sin", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("soundex", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("sqrt", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("stddev", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("tan", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("variance", new StandardSQLFunction( NHibernateUtil.Double ) );
*/
			/*
			RegisterFunction("julian_day", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("microsecond", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("midnight_seconds", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("minute", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("month", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("monthname", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("quarter", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("hour", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("second", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("date", new StandardSQLFunction(Hibernate.DATE) );
			RegisterFunction("day", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("dayname", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("dayofweek", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("dayofweek_iso", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("dayofyear", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("days", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("time", new StandardSQLFunction( NHibernateUtil.Time ) );
			RegisterFunction("timestamp", new StandardSQLFunction( NHibernateUtil.Timestamp ) );
			RegisterFunction("timestamp_iso", new StandardSQLFunction( NHibernateUtil.Timestamp ) );
			RegisterFunction("week", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("week_iso", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("year", new StandardSQLFunction( NHibernateUtil.Int32 ) );

			RegisterFunction("double", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("varchar", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("real", new StandardSQLFunction( NHibernateUtil.Single ) );
			RegisterFunction("bigint", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("char", new StandardSQLFunction( NHibernateUtil.Character ) );
			RegisterFunction("integer", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("smallint", new StandardSQLFunction( NHibernateUtil.Int16 ) );
			*/
/*
			RegisterFunction("digits", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("chr", new StandardSQLFunction( NHibernateUtil.Character ) );
			RegisterFunction("upper", new StandardSQLFunction() );
			RegisterFunction("ucase", new StandardSQLFunction() );
			RegisterFunction("lcase", new StandardSQLFunction() );
			RegisterFunction("lower", new StandardSQLFunction() );
			RegisterFunction("length", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("ltrim", new StandardSQLFunction() );
*/
			//although theoretically Access should support outer joins, it has some severe 
			//limitations on complexity of the SQL statements, so we better switch it off.
			DefaultProperties[ Environment.MaxFetchDepth ] = "0";
			DefaultProperties[ Environment.PrepareSql ] = "false";

			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.JetDriver";
		}

		/// <summary>
		/// The name of the SQL function that transforms a string to lowercase
		/// </summary>
		public override string LowercaseFunction
		{
			get { return "lcase"; }
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

		public override string ForUpdateString
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// Add the Identity Select string to the Insert Sql.
		/// Not supported by Access.
		/// </summary>
		/// <param name="insertSql">The SqlString that contains the INSERT sql.</param>
		/// <returns>null.</returns>
        public override SqlString AddIdentitySelectToInsert( SqlString insertSql, string identityColumn, string tableName )
		{
			return null;
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string GetIdentitySelectString( string identityColumn, string tableName )
		{
			return "select @@identity";
		}

		/// <summary>
		/// Access is not conforming to standards of other databases in identity/autoincrement columns specifications.
		/// Instead of something like "INT NOT NULL AUTO_INCREMENT" for MySQL or "INT IDENTITY NOT NULL" for MsSQL, 
		/// Access autoincremented column has a special datatype - COUNTER. This is not compatible with NHibernate way
		/// of doing things, so I define some non-SQL string, that is translated in the JetDbCommand..
		/// </summary>
		public override string IdentityColumnString
		{
			get { return JetDbCommand.IdentitySpecPlaceHolder; }
		}

		/// <summary></summary>
		public override string NoColumnsInsertString
		{
			get { return "DEFAULT VALUES"; }
		}

		/// <summary></summary>
		public override int MaxAnsiStringSize
		{
			get { return 1073741823; }
		}

		/// <summary></summary>
		public override int MaxBinaryBlobSize
		{
			get { return 2147483647; }
		}

		/// <summary></summary>
		public override int MaxBinarySize
		{
			get { return 2147483647; }
		}

		/// <summary></summary>
		public override int MaxStringClobSize
		{
			get { return 1073741823; }
		}

		/// <summary></summary>
		public override int MaxStringSize
		{
			get { return 1073741823; }
		}

		/// <summary></summary>
		public override char CloseQuote
		{
			get { return '`'; }
		}

		/// <summary></summary>
		public override char OpenQuote
		{
			get { return '`'; }
		}

		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>False.</value>
		public override bool SupportsLimit
		{
			get { return false; }
		}

		/// <summary>
		/// Does this Dialect support an offset?
		/// </summary>
		public override bool SupportsLimitOffset
		{
			get { return false; }
		}

		/// <summary>
		/// Can parameters be used for a statement containing a LIMIT?
		/// </summary>
		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		public override SqlString GetLimitString( SqlString querySqlString, int offset, int limit )
		{
			throw new NotSupportedException( "SQL Server does not support an offset" );
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
		protected override string Quote( string name )
		{
			return OpenQuote + name.Replace( CloseQuote.ToString(), new string( CloseQuote, 2 ) ) + CloseQuote;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="quoted"></param>
		/// <returns></returns>
		public override string UnQuote( string quoted )
		{
			if( IsQuoted( quoted ) )
			{
				quoted = quoted.Substring( 1, quoted.Length - 2 );
			}

			return quoted.Replace( new string( CloseQuote, 2 ), CloseQuote.ToString() );
		}

		public override bool SupportsIdentitySelectInInsert
		{
			get { return false; }
		}

		public override JoinFragment CreateOuterJoinFragment()
		{
			return new JetJoinFragment();
		}

		/// <summary>
		/// Create an <c>CaseFragment</c> for this dialect
		/// </summary>
		/// <returns></returns>
		public override CaseFragment CreateCaseFragment()
		{
			return new JetCaseFragment( this );
		}
	}
}