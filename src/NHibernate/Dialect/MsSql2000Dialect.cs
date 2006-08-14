using System;
using System.Data;

using NHibernate.SqlCommand;

using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect compatible with Microsoft SQL Server 2000.
	/// </summary>
	/// <remarks>
	/// The MsSql2000Dialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>hibernate.use_outer_join</term>
	///			<description><c>true</c></description>
	///		</item>
	///		<item>
	///			<term>hibernate.connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.SqlClientDriver" /></description>
	///		</item>
	///		<item>
	///			<term>hibernate.prepare_sql</term>
	///			<description><c>false</c></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class MsSql2000Dialect : Dialect
	{
		/// <summary></summary>
		public MsSql2000Dialect() : base()
		{
			RegisterColumnType( DbType.AnsiStringFixedLength, "CHAR(255)" );
			RegisterColumnType( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			RegisterColumnType( DbType.AnsiString, "VARCHAR(255)" );
			RegisterColumnType( DbType.AnsiString, 8000, "VARCHAR($1)" );
			RegisterColumnType( DbType.AnsiString, 2147483647, "TEXT" );
			RegisterColumnType( DbType.Binary, "VARBINARY(8000)" );
			RegisterColumnType( DbType.Binary, 8000, "VARBINARY($1)" );
			RegisterColumnType( DbType.Binary, 2147483647, "IMAGE" );
			RegisterColumnType( DbType.Boolean, "BIT" );
			RegisterColumnType( DbType.Byte, "TINYINT" );
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
			RegisterColumnType( DbType.Double, "DOUBLE PRECISION" ); //synonym for FLOAT(53)
			RegisterColumnType( DbType.Guid, "UNIQUEIDENTIFIER" );
			RegisterColumnType( DbType.Int16, "SMALLINT" );
			RegisterColumnType( DbType.Int32, "INT" );
			RegisterColumnType( DbType.Int64, "BIGINT" );
			RegisterColumnType( DbType.Single, "REAL" ); //synonym for FLOAT(24) 
			RegisterColumnType( DbType.StringFixedLength, "NCHAR(255)" );
			RegisterColumnType( DbType.StringFixedLength, 4000, "NCHAR($1)" );
			RegisterColumnType( DbType.String, "NVARCHAR(255)" );
			RegisterColumnType( DbType.String, 4000, "NVARCHAR($1)" );
			RegisterColumnType( DbType.String, 1073741823, "NTEXT" );
			RegisterColumnType( DbType.Time, "DATETIME" );

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

			RegisterFunction("digits", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("chr", new StandardSQLFunction( NHibernateUtil.Character ) );
			RegisterFunction("upper", new StandardSQLFunction() );
			RegisterFunction("ucase", new StandardSQLFunction() );
			RegisterFunction("lcase", new StandardSQLFunction() );
			RegisterFunction("lower", new StandardSQLFunction() );
			RegisterFunction("length", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("ltrim", new StandardSQLFunction() );

			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.SqlClientDriver";
			DefaultProperties[ Environment.PrepareSql ] = "false";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		/// <summary></summary>
		public override string NullColumnString
		{
			get { return " null"; }
		}

		/// <summary></summary>
		public override bool QualifyIndexName
		{
			get { return false; }
		}

		/// <summary>
		/// Generates the string to drop the table using the MsSql syntax
		/// <code>
		/// if exists (select * from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
		/// drop table {0}
		/// </code>
		/// </summary>
		/// <param name="tableName">The name of the table to drop.</param>
		/// <returns>The SQL with the <c>tableName</c> inserted.</returns>
		public override string GetDropTableString(string tableName)
		{
			string dropTable = "if exists (select * from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1)" + 
				" drop table {0}";

			return String.Format( dropTable, tableName );
		}

		public override string ForUpdateString
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// Add the Identity Select string to the Insert Sql.
		/// </summary>
		/// <param name="insertSql">The SqlString that contains the INSERT sql.</param>
		/// <returns>A new SqlString with <c>; SELECT SCOPE_IDENTITY()</c> at the end.</returns>
		public override SqlString AddIdentitySelectToInsert( SqlString insertSql, string identityColumn, string tableName )
		{
			return insertSql.Append( "; " + GetIdentitySelectString( identityColumn, tableName ) );
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string GetIdentitySelectString( string identityColumn, string tableName )
		{
			return "select SCOPE_IDENTITY()";
		}

		/// <summary></summary>
		public override string IdentityColumnString
		{
			get { return "IDENTITY NOT NULL"; }
		}

		/// <summary></summary>
		public override string NoColumnsInsertString
		{
			get { return "DEFAULT VALUES"; }
		}

		/// <summary></summary>
		public override int MaxAnsiStringSize
		{
			get { return 8000; }
		}

		/// <summary></summary>
		public override int MaxBinaryBlobSize
		{
			get { return 2147483647; }
		}

		/// <summary></summary>
		public override int MaxBinarySize
		{
			get { return 8000; }
		}

		/// <summary></summary>
		public override int MaxStringClobSize
		{
			get { return 1073741823; }
		}

		/// <summary></summary>
		public override int MaxStringSize
		{
			get { return 4000; }
		}

		/// <summary></summary>
		public override char CloseQuote
		{
			get { return ']'; }
		}

		/// <summary></summary>
		public override char OpenQuote
		{
			get { return '['; }
		}

		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>True, we'll use the SELECT TOP nn syntax.</value>
		public override bool SupportsLimit
		{
			get { return true; }
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
		/// <summary>
		/// Add a <c>LIMIT (TOP)</c> clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">A Query in the form of a SqlString.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query</param>
		/// <param name="offset">Offset of the first row to process in the result set</param>
		/// <returns>A new SqlString that contains the <c>LIMIT</c> clause.</returns>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			if (offset > 0)
			{
				throw new NotSupportedException("SQL Server does not support an offset" );
			}

			/*
			 * "SELECT TOP limit rest-of-sql-statement"
			 */
			querySqlString = querySqlString.Compact();
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			bool topAdded = false;
			foreach( object sqlPart in querySqlString.SqlParts )
			{
				if (!topAdded)
				{
					string sqlPartString = sqlPart as string;
					if( sqlPartString != null )
					{
						string sqlFragment = sqlPartString.TrimStart();
						int insertIndex = GetAfterSelectInsertPoint(sqlFragment);
						if( insertIndex > 0 )
						{
							string newFragment = sqlFragment.Insert(insertIndex, " top " + limit.ToString());
							pagingBuilder.Add(newFragment);
							topAdded = true;
							continue;
						}
					}
				}
				pagingBuilder.AddObject(sqlPart);
			}

			return pagingBuilder.ToSqlString();
		}

		/// <summary>
		/// Does the <c>LIMIT</c> clause take a "maximum" row number
		/// instead of a total number of returned rows?
		/// </summary>
		/// <returns>false, unless overridden</returns>
		public override bool UseMaxForLimit
		{
			get { return true; }
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

		private static int GetAfterSelectInsertPoint(string fragment)
		{
			string fragmentLowerCased = fragment.ToLower( System.Globalization.CultureInfo.InvariantCulture );
			if (fragmentLowerCased.StartsWith("select distinct"))
			{
				return 15;
			}
			else if (fragmentLowerCased.StartsWith("select"))
			{
				return 6;
			}
			return 0;
		}
	}
}