using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using Environment = NHibernate.Cfg.Environment;

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
	///			<description><see cref="NHibernate.Driver.FirebirdClientDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class FirebirdDialect : Dialect
	{
		#region Fields

		private const int MAX_DECIMAL_PRECISION = 18;

		#endregion

		public FirebirdDialect()
		{
			RegisterKeywords();
			RegisterColumnTypes();
			RegisterFunctions();
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.FirebirdClientDriver";
		}

		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override string GetTypeName(SqlType sqlType)
		{
			if (IsUnallowedDecimal(sqlType.DbType, sqlType.Precision))
				return base.GetTypeName(new SqlType(sqlType.DbType, MAX_DECIMAL_PRECISION, sqlType.Scale));

			return base.GetTypeName(sqlType);
		}

		public override string GetTypeName(SqlType sqlType, int length, int precision, int scale)
		{
			var fbDecimalPrecision = precision;
			if (IsUnallowedDecimal(sqlType.DbType, precision))
				fbDecimalPrecision = MAX_DECIMAL_PRECISION;

			return base.GetTypeName(sqlType, length, fbDecimalPrecision, scale);
		}

		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return string.Format("gen_id({0}, 1 )", sequenceName);
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

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			// FIXME - This should use the ROWS syntax in Firebird to avoid problems with subqueries metioned here:
			// http://www.firebirdsql.org/refdocs/langrefupd20-select.html#langrefupd20-first-skip

			/*
			 * "SELECT FIRST x [SKIP y] rest-of-sql-statement"
			 */

			int insertIndex = GetAfterSelectInsertPoint(queryString);

			var limitFragment = new SqlStringBuilder();

			if (limit != null)
			{
				limitFragment.Add(" first ");
				limitFragment.Add(limit);
			}

			if (offset != null)
			{
				limitFragment.Add(" skip ");
				limitFragment.Add(offset);
			}

			return queryString.Insert(insertIndex, limitFragment.ToSqlString());
		}

		#region Temporaray Table Support

		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		public override string CreateTemporaryTableString
		{
			get { return "create global temporary table"; }
		}

		public override bool? PerformTemporaryTableDDLInIsolation()
		{
			return true;
		}

		public override bool DropTemporaryTableAfterUse()
		{
			return true;
		}

		#endregion

		private static int GetAfterSelectInsertPoint(SqlString text)
		{
			if (text.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}
			return -1;
		}

		[Serializable]
		private class CastedFunction : NoArgSQLFunction
		{
			public CastedFunction(string name, IType returnType) : base(name, returnType, false)
			{
			}

			public override SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				return new SqlString("cast('", Name, "' as ", FunctionReturnType.SqlTypes(factory)[0].ToString(), ")");
			}
		}

		[Serializable]
		private class CurrentTimeStamp : NoArgSQLFunction
		{
			public CurrentTimeStamp() : base("current_timestamp", NHibernateUtil.DateTime, true)
			{
			}

			public override SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				return new SqlString(Name);
			}
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new FirebirdDataBaseSchema(connection);
		}

		public override string QuerySequencesString
		{
			get
			{
				return "select RDB$GENERATOR_NAME from RDB$GENERATORS where (RDB$SYSTEM_FLAG is NULL) or (RDB$SYSTEM_FLAG <> 1)";
			}
		}

		public override SqlString AddIdentifierOutParameterToInsert(SqlString insertString, string identifierColumnName,
			string parameterName)
		{
			return insertString.Append(" returning " + identifierColumnName);
		}

		public override long TimestampResolutionInTicks
		{
			// Lousy documentation. Various mailing lists and articles seem to 
			// indicate resolution of 0.1 ms, i.e. 1000 ticks.
			get { return 1000L; }
		}

		public override bool SupportsCurrentTimestampSelection
		{
			get { return true; }
		}

		public override string CurrentTimestampSelectString
		{
			get { return "select CURRENT_TIMESTAMP from RDB$DATABASE"; }
		}

		public override string SelectGUIDString
		{
			get { return "select GEN_UUID() from RDB$DATABASE"; }
		}

		[Serializable]
		private class PositionFunction : ISQLFunction
		{
			// The cast is needed, at least in the case that ?3 is a named integer parameter, otherwise firebird will generate an error.  
			// We have a unit test to cover this potential firebird bug.
			private static readonly ISQLFunction LocateWith2Params = new StandardSQLFunction("position", NHibernateUtil.Int32);

			private static readonly ISQLFunction LocateWith3Params = new SQLFunctionTemplate(NHibernateUtil.Int32,
				"position(?1, ?2, cast(?3 as int))");

			public IType ReturnType(IType columnType, IMapping mapping)
			{
				return NHibernateUtil.Int32;
			}

			public bool HasArguments
			{
				get { return true; }
			}

			public bool HasParenthesesIfNoArguments
			{
				get { return true; }
			}

			public SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				if (args.Count == 2)
				{
					return LocateWith2Params.Render(args, factory);
				}
				if (args.Count == 3)
				{
					return LocateWith3Params.Render(args, factory);
				}

				throw new QueryException("'postition' function takes 2 or 3 arguments");
			}
		}

		protected virtual void RegisterKeywords()
		{
			RegisterKeyword("date");
		}

		protected virtual void RegisterColumnTypes()
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
			RegisterColumnType(DbType.Currency, "DECIMAL(18, 4)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
			RegisterColumnType(DbType.Decimal, "DECIMAL(18, 5)"); // NUMERIC(18,5) is equivalent to DECIMAL(18,5)
			RegisterColumnType(DbType.Decimal, 18, "DECIMAL($p, $s)");
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
		}

		protected virtual void RegisterFunctions()
		{
			OverrideStandardHQLFunctions();
			RegisterFirebirdServerEmbeddedFunctions();
			RegisterExternalFbAndIbStandardUDFs();
		}

		private void OverrideStandardHQLFunctions()
		{
			RegisterFunction("current_timestamp", new CurrentTimeStamp());
			RegisterFunction("length", new StandardSafeSQLFunction("char_length", NHibernateUtil.Int64, 1));
			RegisterFunction("nullif", new StandardSafeSQLFunction("nullif", 2));
			RegisterFunction("lower", new StandardSafeSQLFunction("lower", NHibernateUtil.String, 1));
			RegisterFunction("upper", new StandardSafeSQLFunction("upper", NHibernateUtil.String, 1));
			RegisterFunction("mod", new StandardSafeSQLFunction("mod", NHibernateUtil.Double, 2));
			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as VARCHAR(255))"));
			RegisterFunction("sysdate", new CastedFunction("today", NHibernateUtil.Date));
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "cast(?1 as date)"));
		}

		private void RegisterFirebirdServerEmbeddedFunctions()
		{
			RegisterFunction("today", new CastedFunction("today", NHibernateUtil.Date));
			RegisterFunction("yesterday", new CastedFunction("yesterday", NHibernateUtil.Date));
			RegisterFunction("tomorrow", new CastedFunction("tomorrow", NHibernateUtil.Date));
			RegisterFunction("now", new CastedFunction("now", NHibernateUtil.DateTime));
			RegisterFunction("iif", new StandardSafeSQLFunction("iif", 3));
			// New embedded functions in FB 2.0 (http://www.firebirdsql.org/rlsnotes20/rnfbtwo-str.html#str-string-func)
			RegisterFunction("char_length", new StandardSafeSQLFunction("char_length", NHibernateUtil.Int64, 1));
			RegisterFunction("bit_length", new StandardSafeSQLFunction("bit_length", NHibernateUtil.Int64, 1));
			RegisterFunction("octet_length", new StandardSafeSQLFunction("octet_length", NHibernateUtil.Int64, 1));
		}

		private void RegisterExternalFbAndIbStandardUDFs()
		{
			RegisterMathematicalFunctions();
			RegisterDateTimeFunctions();
			RegisterStringAndCharFunctions();
			RegisterBlobFunctions();
			RegisterTrigonometricFunctions();
		}

		private void RegisterMathematicalFunctions()
		{
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
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
			RegisterFunction("sqtr", new StandardSQLFunction("sqtr", NHibernateUtil.Double));
			RegisterFunction("truncate", new StandardSQLFunction("truncate"));
			RegisterFunction("floor", new StandardSafeSQLFunction("floor", NHibernateUtil.Double, 1));
			RegisterFunction("round", new StandardSQLFunction("round"));
		}

		private void RegisterDateTimeFunctions()
		{
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
		}

		private void RegisterStringAndCharFunctions()
		{
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
			RegisterFunction("locate", new PositionFunction());
			RegisterFunction("replace", new StandardSafeSQLFunction("replace", NHibernateUtil.String, 3));
			RegisterFunction("left", new StandardSQLFunction("left"));
		}

		private void RegisterBlobFunctions()
		{
			RegisterFunction("string2blob", new StandardSQLFunction("string2blob"));
		}

		private void RegisterTrigonometricFunctions()
		{
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
		}

		private static bool IsUnallowedDecimal(DbType dbType, int precision)
		{
			return dbType == DbType.Decimal && precision > MAX_DECIMAL_PRECISION;
		}
	}
}