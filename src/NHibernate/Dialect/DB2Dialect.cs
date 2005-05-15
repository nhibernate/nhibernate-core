using System.Data;
using System.Text;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for DB2.
	/// </summary>
	/// <remarks>
	/// The DB2Dialect defaults the following configuration properties:
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
	///			<description><see cref="NHibernate.Driver.DB2Driver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class DB2Dialect : Dialect
	{
		/// <summary></summary>
		public DB2Dialect()
		{
			RegisterColumnType( DbType.AnsiStringFixedLength, "CHAR(254)" );
			RegisterColumnType( DbType.AnsiStringFixedLength, 254, "CHAR($1)" );
			RegisterColumnType( DbType.AnsiString, "VARCHAR(254)" );
			RegisterColumnType( DbType.AnsiString, 8000, "VARCHAR($1)" );
			RegisterColumnType( DbType.AnsiString, 2147483647, "CLOB" );
			RegisterColumnType( DbType.Binary, 2147483647, "BLOB" );
			RegisterColumnType( DbType.Boolean, "SMALLINT" );
			RegisterColumnType( DbType.Byte, "SMALLINT" );
			RegisterColumnType( DbType.Currency, "DECIMAL(16,4)" );
			RegisterColumnType( DbType.Date, "DATE" );
			RegisterColumnType( DbType.DateTime, "TIMESTAMP" );
			RegisterColumnType( DbType.Decimal, "DECIMAL(19,5)" );
			RegisterColumnType( DbType.Decimal, 19, "DECIMAL(19, $1)" );
			RegisterColumnType( DbType.Double, "DOUBLE" );
			RegisterColumnType( DbType.Int16, "SMALLINT" );
			RegisterColumnType( DbType.Int32, "INTEGER" );
			RegisterColumnType( DbType.Int64, "BIGINT" );
			RegisterColumnType( DbType.Single, "REAL" );
			RegisterColumnType( DbType.StringFixedLength, "CHAR(254)" );
			RegisterColumnType( DbType.StringFixedLength, 254, "CHAR($1)" );
			RegisterColumnType( DbType.String, "VARCHAR(254)" );
			RegisterColumnType( DbType.String, 8000, "VARCHAR($1)" );
			RegisterColumnType( DbType.String, 2147483647, "CLOB" );
			RegisterColumnType( DbType.Time, "TIME" );

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

			RegisterFunction("julian_day", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("microsecond", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("midnight_seconds", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("minute", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("month", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("monthname", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("quarter", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("hour", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("second", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("date", new StandardSQLFunction( NHibernateUtil.Date ) );
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

			RegisterFunction("digits", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("chr", new StandardSQLFunction( NHibernateUtil.Character ) );
			RegisterFunction("upper", new StandardSQLFunction() );
			RegisterFunction("ucase", new StandardSQLFunction() );
			RegisterFunction("lcase", new StandardSQLFunction() );
			RegisterFunction("lower", new StandardSQLFunction() );
			RegisterFunction("length", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("ltrim", new StandardSQLFunction() );

			DefaultProperties[ Environment.UseOuterJoin ] = "true";
			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.DB2Driver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add column"; }
		}

		/// <summary></summary>
		public override bool DropConstraints
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string IdentitySelectString
		{
			get { return "values IDENTITY_VAL_LOCAL()"; }
		}

		/// <summary></summary>
		public override string IdentityColumnString
		{
			get { return "not null generated by default as identity"; }
		}

		/// <summary></summary>
		public override string IdentityInsertString
		{
			get { return "default"; }
		}

		/// <summary></summary>
		public override string GetSequenceNextValString( string sequenceName )
		{
			return "values nextval for " + sequenceName;
		}

		/// <summary></summary>
		public override string GetCreateSequenceString( string sequenceName )
		{
			return "create sequence " + sequenceName;
		}

		/// <summary></summary>
		public override string GetDropSequenceString( string sequenceName )
		{
			return string.Concat( "drop sequence ", sequenceName, " restrict" );
		}

		/// <summary></summary>
		public override bool SupportsSequences
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool BindLimitParametersInReverseOrder
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool UseMaxForLimit
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="querySqlString"></param>
		/// <returns></returns>
		public override SqlString GetLimitString( SqlString querySqlString )
		{
			Parameter p1 = new Parameter( "p1", new Int32SqlType() );
			Parameter p2 = new Parameter( "p2", new Int32SqlType() );
			
			/*
			 * "select * from (select row_number() over(orderby_clause) as rownum, "
			 * querySqlString_without select
			 * " ) as tempresult where rownum between ? and ?"
			 */
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			bool isInOrderBy = false;
			StringBuilder orderByStringBuilder = new StringBuilder();

			// build a new query and extract the order by part
			foreach( object sqlPart in querySqlString.SqlParts )
			{
				string sqlPartString = sqlPart as string;
				if( sqlPartString != null )
				{
					if( sqlPartString.ToLower().TrimStart().StartsWith( "order by" ) )
					{
						isInOrderBy = true;
					}
				}

				if( isInOrderBy && sqlPart is string )
				{
					orderByStringBuilder.Append( ( string ) sqlPart );
				}
				else
				{
					pagingBuilder.AddObject( sqlPart );
				}
			}

			string rownumClause = "rownumber() over(" + orderByStringBuilder.ToString() + ") as rownum, ";
			// Add the rownum clause first, right after the original select
			pagingBuilder.Insert( 1, rownumClause );
			// Add the rest
			pagingBuilder.Insert( 0, "select * from (" );
			pagingBuilder.Add( ") as tempresult " );
			// Add the where clause
			pagingBuilder.Add( " where rownum between " );
			pagingBuilder.Add( p1 );
			pagingBuilder.Add( " and " );
			pagingBuilder.Add( p2 );

			return pagingBuilder.ToSqlString();
		}
	}
}