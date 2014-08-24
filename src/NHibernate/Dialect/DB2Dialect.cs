using System.Data;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;

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
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.DB2Driver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class DB2Dialect : Dialect
	{
		/// <summary></summary>
		public DB2Dialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(254)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 254, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(254)");
			RegisterColumnType(DbType.AnsiString, 8000, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "CLOB");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB");
			RegisterColumnType(DbType.Boolean, "SMALLINT");
			RegisterColumnType(DbType.Byte, "SMALLINT");
			RegisterColumnType(DbType.Currency, "DECIMAL(16,4)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "DECIMAL(19, $l)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "REAL");
			RegisterColumnType(DbType.StringFixedLength, "CHAR(254)");
			RegisterColumnType(DbType.StringFixedLength, 254, "CHAR($l)");
			RegisterColumnType(DbType.String, "VARCHAR(254)");
			RegisterColumnType(DbType.String, 8000, "VARCHAR($l)");
			RegisterColumnType(DbType.String, 2147483647, "CLOB");
			RegisterColumnType(DbType.Time, "TIME");

			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("absval", new StandardSQLFunction("absval"));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));

			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));
			RegisterFunction("round", new StandardSQLFunction("round"));

			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("degrees", new StandardSQLFunction("degrees", NHibernateUtil.Double));
			RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));
			RegisterFunction("float", new StandardSQLFunction("float", NHibernateUtil.Double));
			RegisterFunction("hex", new StandardSQLFunction("hex", NHibernateUtil.String));
			RegisterFunction("ln", new StandardSQLFunction("ln", NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
			RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
			RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.String));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));

			RegisterFunction("julian_day", new StandardSQLFunction("julian_day", NHibernateUtil.Int32));
			RegisterFunction("microsecond", new StandardSQLFunction("microsecond", NHibernateUtil.Int32));
			RegisterFunction("midnight_seconds", new StandardSQLFunction("midnight_seconds", NHibernateUtil.Int32));
			RegisterFunction("minute", new StandardSQLFunction("minute", NHibernateUtil.Int32));
			RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
			RegisterFunction("monthname", new StandardSQLFunction("monthname", NHibernateUtil.String));
			RegisterFunction("quarter", new StandardSQLFunction("quarter", NHibernateUtil.Int32));
			RegisterFunction("hour", new StandardSQLFunction("hour", NHibernateUtil.Int32));
			RegisterFunction("second", new StandardSQLFunction("second", NHibernateUtil.Int32));
			RegisterFunction("date", new StandardSQLFunction("date", NHibernateUtil.Date));
			RegisterFunction("day", new StandardSQLFunction("day", NHibernateUtil.Int32));
			RegisterFunction("dayname", new StandardSQLFunction("dayname", NHibernateUtil.String));
			RegisterFunction("dayofweek", new StandardSQLFunction("dayofweek", NHibernateUtil.Int32));
			RegisterFunction("dayofweek_iso", new StandardSQLFunction("dayofweek_iso", NHibernateUtil.Int32));
			RegisterFunction("dayofyear", new StandardSQLFunction("dayofyear", NHibernateUtil.Int32));
			RegisterFunction("days", new StandardSQLFunction("days", NHibernateUtil.Int32));
			RegisterFunction("time", new StandardSQLFunction("time", NHibernateUtil.Time));
			RegisterFunction("timestamp", new StandardSQLFunction("timestamp", NHibernateUtil.Timestamp));
			RegisterFunction("timestamp_iso", new StandardSQLFunction("timestamp_iso", NHibernateUtil.Timestamp));
			RegisterFunction("week", new StandardSQLFunction("week", NHibernateUtil.Int32));
			RegisterFunction("week_iso", new StandardSQLFunction("week_iso", NHibernateUtil.Int32));
			RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));

			RegisterFunction("double", new StandardSQLFunction("double", NHibernateUtil.Double));
			RegisterFunction("varchar", new StandardSQLFunction("varchar", NHibernateUtil.String));
			RegisterFunction("real", new StandardSQLFunction("real", NHibernateUtil.Single));
			RegisterFunction("bigint", new StandardSQLFunction("bigint", NHibernateUtil.Int32));
			RegisterFunction("char", new StandardSQLFunction("char", NHibernateUtil.Character));
			RegisterFunction("integer", new StandardSQLFunction("integer", NHibernateUtil.Int32));
			RegisterFunction("smallint", new StandardSQLFunction("smallint", NHibernateUtil.Int16));

			RegisterFunction("digits", new StandardSQLFunction("digits", NHibernateUtil.String));
			RegisterFunction("chr", new StandardSQLFunction("chr", NHibernateUtil.Character));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("ucase", new StandardSQLFunction("ucase"));
			RegisterFunction("lcase", new StandardSQLFunction("lcase"));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int32));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));

			RegisterFunction("mod", new StandardSQLFunction("mod", NHibernateUtil.Int32));

			// DB2 does not support ANSI substring syntax.
			RegisterFunction("substring", new SQLFunctionTemplate(NHibernateUtil.String, "substring(?1, ?2, ?3)"));

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.DB2Driver";
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
			get { return "select identity_val_local() from sysibm.sysdummy1"; }
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


		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return "nextval for " + sequenceName;
		}

		/// <summary></summary>
		public override string GetSequenceNextValString(string sequenceName)
		{
			return "values nextval for " + sequenceName;
		}

		/// <summary></summary>
		public override string GetCreateSequenceString(string sequenceName)
		{
			return "create sequence " + sequenceName;
		}

		/// <summary></summary>
		public override string GetDropSequenceString(string sequenceName)
		{
			return string.Concat("drop sequence ", sequenceName, " restrict");
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
		public override bool UseMaxForLimit
		{
			get { return true; }
		}

		public override SqlString GetLimitString(SqlString querySqlString, SqlString offset, SqlString limit)
		{
			/*
			 * "select * from (select row_number() over(orderby_clause) as rownum, "
			 * querySqlString_without select
			 * " ) as tempresult where rownum between ? and ?"
			 */
			string rownumClause = GetRowNumber(querySqlString);

			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder
				.Add("select * from (select ")
				.Add(rownumClause)
				.Add(querySqlString.Substring(7))
				.Add(") as tempresult where rownum ");

			if (offset != null && limit != null)
			{
				pagingBuilder
					.Add("between ")
					.Add(offset)
					.Add("+1 and ")
					.Add(limit);
			}
			else if (limit != null)
			{
				pagingBuilder
					.Add("<= ")
					.Add(limit);
			}
			else
			{
				// We just have an offset.
				pagingBuilder
					.Add("> ")
					.Add(offset);
			}

			return pagingBuilder.ToSqlString();
		}

		private static string GetRowNumber(SqlString sql)
		{
			return new StringBuilder()
				.Append("rownumber() over(")
				.Append(sql.SubstringStartingWithLast("order by"))
				.Append(") as rownum, ")
				.ToString();
		}

		public override string ForUpdateString
		{
			get { return " for read only with rs"; }
		}
	}
}