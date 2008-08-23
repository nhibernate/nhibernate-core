using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for FirebirdDialect.
	/// </summary>
	/// <remarks>
	/// The FirebirdDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.FirebirdDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class FirebirdDialect : Dialect
	{
		public FirebirdDialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "BLOB SUB_TYPE 1"); // should use the IType.ClobType
			RegisterColumnType(DbType.Binary, "BLOB SUB_TYPE 0");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB SUB_TYPE 0"); // should use the IType.BlobType
			RegisterColumnType(DbType.Boolean, "SMALLINT");
			RegisterColumnType(DbType.Byte, "SMALLINT");
			RegisterColumnType(DbType.Currency, "DECIMAL(18,4)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
			RegisterColumnType(DbType.Decimal, "DECIMAL(18,5)"); // NUMERIC(18,5) is equivalent to DECIMAL(18,5)
			RegisterColumnType(DbType.Decimal, 18, "DECIMAL(18, $l)");
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
			RegisterColumnType(DbType.Guid, "CHAR(16) CHARACTER SET OCTETS");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "FLOAT");
			RegisterColumnType(DbType.StringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 4000, "CHAR($l)");
			RegisterColumnType(DbType.String, "VARCHAR(255)");
			RegisterColumnType(DbType.String, 4000, "VARCHAR($l)");
			RegisterColumnType(DbType.String, 1073741823, "BLOB SUB_TYPE 1"); // should use the IType.ClobType
			RegisterColumnType(DbType.Time, "TIME");

			// Override standard HQL function
			RegisterFunction("current_timestamp", new CurrentTimeStamp());
			RegisterFunction("length", new StandardSafeSQLFunction("char_length", NHibernateUtil.Int64, 1));
			RegisterFunction("substring", new AnsiSubstringFunction());
			RegisterFunction("nullif", new StandardSafeSQLFunction("nullif", 2));
			RegisterFunction("lower", new StandardSafeSQLFunction("lower", NHibernateUtil.String, 1));
			RegisterFunction("upper", new StandardSafeSQLFunction("upper",NHibernateUtil.String, 1)); ;
			RegisterFunction("mod", new StandardSafeSQLFunction("mod", NHibernateUtil.Double, 2));
			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as VARCHAR(255))"));
			RegisterFunction("sysdate", new CastedFunction("today", NHibernateUtil.Date));

			// Firebird server embedded functions
			RegisterFunction("today", new CastedFunction("today", NHibernateUtil.Date));
			RegisterFunction("yesterday", new CastedFunction("yesterday", NHibernateUtil.Date));
			RegisterFunction("tomorrow", new CastedFunction("tomorrow", NHibernateUtil.Date));
			RegisterFunction("now", new CastedFunction("now", NHibernateUtil.DateTime));
			RegisterFunction("iif", new StandardSafeSQLFunction("iif", 3));
			// New embedded functions in FB 2.0 (http://www.firebirdsql.org/rlsnotes20/rnfbtwo-str.html#str-string-func)
			RegisterFunction("char_length", new StandardSafeSQLFunction("char_length", NHibernateUtil.Int64, 1));
			RegisterFunction("bit_length", new StandardSafeSQLFunction("bit_length", NHibernateUtil.Int64, 1));
			RegisterFunction("octet_length", new StandardSafeSQLFunction("octet_length", NHibernateUtil.Int64, 1));

			// External Firebird and Interbase standard UDFs
			//Mathematical Functions
			RegisterFunction("abs", new StandardSQLFunction("abs", NHibernateUtil.Double));
			RegisterFunction("bin_and", new StandardSQLFunction("bin_and", NHibernateUtil.Int32));
			RegisterFunction("bin_or", new StandardSQLFunction("bin_or", NHibernateUtil.Int32));
			RegisterFunction("bin_xor", new StandardSQLFunction("bin_xor", NHibernateUtil.Int32));
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling", NHibernateUtil.Double));
			RegisterFunction("div", new StandardSQLFunction("div", NHibernateUtil.Double));
			RegisterFunction("dpower", new StandardSQLFunction("dpower", NHibernateUtil.Double));
			RegisterFunction("ln", new StandardSQLFunction("ln", NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
			RegisterFunction("pi", new NoArgSQLFunction("pi", NHibernateUtil.Double));
			RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("sing", new StandardSQLFunction("sing", NHibernateUtil.Double));
			RegisterFunction("sqtr", new StandardSQLFunction("sqtr", NHibernateUtil.Double));
			RegisterFunction("truncate", new StandardSQLFunction("truncate"));
			RegisterFunction("floor", new StandardSafeSQLFunction("floor", NHibernateUtil.Double, 1));
			RegisterFunction("round", new StandardSQLFunction("round"));
			//Date and Time Functions
			RegisterFunction("dow", new StandardSQLFunction("dow", NHibernateUtil.String));
			RegisterFunction("sdow", new StandardSQLFunction("sdow", NHibernateUtil.String));
			RegisterFunction("addday", new StandardSQLFunction("addday", NHibernateUtil.DateTime));
			RegisterFunction("addhour", new StandardSQLFunction("addhour", NHibernateUtil.DateTime));
			RegisterFunction("addmillisecond", new StandardSQLFunction("addmillisecond", NHibernateUtil.DateTime));
			RegisterFunction("addminute", new StandardSQLFunction("addminute", NHibernateUtil.DateTime));
			RegisterFunction("addmonth", new StandardSQLFunction("addmonth", NHibernateUtil.DateTime));
			RegisterFunction("addsecond", new StandardSQLFunction("addsecond", NHibernateUtil.DateTime));
			RegisterFunction("addweek", new StandardSQLFunction("addweek", NHibernateUtil.DateTime));
			RegisterFunction("addyear", new StandardSQLFunction("addyear", NHibernateUtil.DateTime));
			RegisterFunction("getexacttimestamp", new NoArgSQLFunction("getexacttimestamp", NHibernateUtil.DateTime));
			//String and Character Functions
			RegisterFunction("ascii_char", new StandardSQLFunction("ascii_char"));
			RegisterFunction("ascii_val", new StandardSQLFunction("ascii_val", NHibernateUtil.Int16));
			RegisterFunction("lpad", new StandardSQLFunction("lpad"));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("sright", new StandardSQLFunction("sright"));
			RegisterFunction("rpad", new StandardSQLFunction("rpad"));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim"));
			RegisterFunction("strlen", new StandardSQLFunction("strlen", NHibernateUtil.Int16));
			RegisterFunction("substr", new StandardSQLFunction("substr"));
			RegisterFunction("substrlen", new StandardSQLFunction("substrlen", NHibernateUtil.Int16));
			//BLOB Functions
			RegisterFunction("string2blob", new StandardSQLFunction("string2blob"));
			//Trigonometric Functions
			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("atan2", new StandardSQLFunction("atan2", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cosh", new StandardSQLFunction("cosh", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("sinh", new StandardSQLFunction("sinh", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("tanh", new StandardSQLFunction("tanh", NHibernateUtil.Double));

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.FirebirdDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override string GetSequenceNextValString(string sequenceName)
		{
			return string.Format("select gen_id({0}, 1 ) from RDB$DATABASE", sequenceName);
		}

		public override string GetCreateSequenceString(string sequenceName)
		{
			return string.Format("create generator {0}", sequenceName);
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			return string.Format("drop generator {0}", sequenceName);
		}

		public override bool SupportsSequences
		{
			get { return true; }
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		/// <summary>
		/// Add a <c>FIRST x [SKIP] y</c> clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">A Query in the form of a SqlString.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query</param>
		/// <param name="offset">Offset of the first row to process in the result set</param>
		/// <returns>A new SqlString that contains the <c>FIRST</c> clause.</returns>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			/*
			 * "SELECT FIRST x [SKIP y] rest-of-sql-statement"
			 */

			int insertIndex = GetAfterSelectInsertPoint(querySqlString);
			if (offset > 0)
			{
				return querySqlString.Insert(insertIndex, " first " + limit.ToString() + " skip " + offset.ToString());
			}
			else
			{
				return querySqlString.Insert(insertIndex, " first " + limit.ToString());
			}
		}

		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		private static int GetAfterSelectInsertPoint(SqlString text)
		{
			if (text.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}

			return -1;
		}

		private class CastedFunction : NoArgSQLFunction
		{
			public CastedFunction(string name, IType returnType)
				: base(name, returnType, false)
			{
			}

			public override SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				return new SqlStringBuilder()
					.Add("cast('")
					.Add(name)
					.Add("' as ")
					.Add(returnType.SqlTypes(factory)[0].ToString())
					.Add(")")
					.ToSqlString();
			}
		}

		private class CurrentTimeStamp: NoArgSQLFunction
		{
			public CurrentTimeStamp()
				: base("current_timestamp", NHibernateUtil.DateTime, true)
			{ }

			public override SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				return new SqlString(name);
			}
		}
	}



}