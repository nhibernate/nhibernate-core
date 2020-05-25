using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.Engine;
using NHibernate.SqlCommand;
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

		#region Temporary Table Support

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
			public CurrentTimeStamp() : base("current_timestamp", NHibernateUtil.LocalDateTime, true)
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
			// Resolution of 1 ms, i.e. 10000 ticks.
			get { return 10000L; }
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
		private class PositionFunction : ISQLFunction, ISQLFunctionExtended
		{
			// The cast is needed, at least in the case that ?3 is a named integer parameter, otherwise firebird will generate an error.  
			// We have a unit test to cover this potential firebird bug.
			private static readonly ISQLFunction LocateWith2Params = new StandardSQLFunction("position", NHibernateUtil.Int32);

			private static readonly ISQLFunction LocateWith3Params = new SQLFunctionTemplate(NHibernateUtil.Int32,
				"position(?1, ?2, cast(?3 as int))");

			// Since v5.3
			[Obsolete("Use GetReturnType method instead.")]
			public IType ReturnType(IType columnType, IMapping mapping)
			{
				return NHibernateUtil.Int32;
			}

			/// <inheritdoc />
			public IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
			{
				return NHibernateUtil.Int32;
			}

			/// <inheritdoc />
			public IType GetEffectiveReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
			{
				return GetReturnType(argumentTypes, mapping, throwOnError);
			}

			/// <inheritdoc />
			public string Name => "position";

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

		#region private static readonly string[] DialectKeywords = { ... }

		private static readonly string[] DialectKeywords =
		{
			"action",
			"active",
			"admin",
			"after",
			"asc",
			"ascending",
			"auto",
			"avg",
			"base_name",
			"before",
			"blob sub_type 1",
			"break",
			"cache",
			"cascade",
			"check_point_length",
			"coalesce",
			"committed",
			"computed",
			"conditional",
			"connection_id",
			"containing",
			"count",
			"cstring",
			"database",
			"debug",
			"desc",
			"descending",
			"descriptor",
			"domain",
			"double precision",
			"entry_point",
			"exception",
			"extract",
			"file",
			"first",
			"free_it",
			"gdscode",
			"gen_id",
			"generator",
			"group_commit_wait_time",
			"inactive",
			"index",
			"input_type",
			"isolation",
			"key",
			"last",
			"length",
			"level",
			"lock",
			"log_buffer_size",
			"logfile",
			"long",
			"manual",
			"max",
			"maximum_segment",
			"message",
			"min",
			"module_name",
			"names",
			"nullif",
			"nulls",
			"num_log_buffers",
			"option",
			"output_type",
			"overflow",
			"page",
			"page_size",
			"pages",
			"password",
			"plan",
			"position",
			"post_event",
			"privileges",
			"protected",
			"raw_partitions",
			"rdb$db_key",
			"read",
			"record_version",
			"recreate",
			"reserv",
			"reserving",
			"restrict",
			"retain",
			"returning_values",
			"role",
			"rows_affected",
			"schema",
			"segment",
			"shadow",
			"shared",
			"singular",
			"size",
			"skip",
			"snapshot",
			"sort",
			"sqlcode",
			"stability",
			"starting",
			"starts",
			"statistics",
			"sub_type",
			"substring",
			"sum",
			"suspend",
			"transaction",
			"transaction_id",
			"type",
			"uncommitted",
			"upper",
			"variable",
			"view",
			"wait",
			"weekday",
			"work",
			"write",
			"yearday",
		};

		#endregion

		protected virtual void RegisterKeywords()
		{
			RegisterKeywords(DialectKeywords);
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
			RegisterFunction("current_date", new NoArgSQLFunction("current_date", NHibernateUtil.LocalDate, false));
			RegisterFunction("length", new StandardSafeSQLFunction("char_length", NHibernateUtil.Int32, 1));
			RegisterFunction("nullif", new StandardSafeSQLFunction("nullif", 2));
			RegisterFunction("lower", new StandardSafeSQLFunction("lower", NHibernateUtil.String, 1));
			RegisterFunction("upper", new StandardSafeSQLFunction("upper", NHibernateUtil.String, 1));
			// Modulo does not throw for decimal parameters but they are casted to int by Firebird, which produces unexpected results
			RegisterFunction("mod", new ModulusFunction(false, false));
			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as VARCHAR(255))"));
			RegisterFunction("strguid", new StandardSQLFunction("uuid_to_char", NHibernateUtil.String));
			RegisterFunction("sysdate", new CastedFunction("today", NHibernateUtil.Date));
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "cast(?1 as date)"));
			RegisterFunction("new_uuid", new NoArgSQLFunction("gen_uuid", NHibernateUtil.Guid));
			// Bitwise operations
			RegisterFunction("band", new Function.BitwiseFunctionOperation("bin_and"));
			RegisterFunction("bor", new Function.BitwiseFunctionOperation("bin_or"));
			RegisterFunction("bxor", new Function.BitwiseFunctionOperation("bin_xor"));
			RegisterFunction("bnot", new Function.BitwiseFunctionOperation("bin_not"));
		}

		private void RegisterFirebirdServerEmbeddedFunctions()
		{
			RegisterFunction("today", new CastedFunction("today", NHibernateUtil.Date));
			RegisterFunction("yesterday", new CastedFunction("yesterday", NHibernateUtil.Date));
			RegisterFunction("tomorrow", new CastedFunction("tomorrow", NHibernateUtil.Date));
			RegisterFunction("now", new CastedFunction("now", NHibernateUtil.DateTime));
			RegisterFunction("iif", new IifSafeSQLFunction());
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
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("div", new StandardSQLFunction("div", NHibernateUtil.Double));
			RegisterFunction("dpower", new StandardSQLFunction("dpower", NHibernateUtil.Double));
			RegisterFunction("ln", new StandardSQLFunction("ln", NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
			RegisterFunction("pi", new NoArgSQLFunction("pi", NHibernateUtil.Double));
			RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("random", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
			RegisterFunction("sqtr", new StandardSQLFunction("sqtr", NHibernateUtil.Double));
			RegisterFunction("trunc", new StandardSQLFunction("trunc"));
			RegisterFunction("truncate", new StandardSQLFunction("trunc"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));
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
			RegisterFunction("ascii_char", new StandardSQLFunction("ascii_char", NHibernateUtil.Character));
			RegisterFunction("chr", new StandardSQLFunction("ascii_char", NHibernateUtil.Character));
			RegisterFunction("ascii_val", new StandardSQLFunction("ascii_val", NHibernateUtil.Int32));
			RegisterFunction("ascii", new StandardSQLFunction("ascii_val", NHibernateUtil.Int32));
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

		// As of Firebird 2.5 documentation, limit is 30/31 (not all source are concordant), with some
		// cases supporting more but considered as bugs and no more tolerated in v3.
		// It seems it may be extended to 63 for Firebird v4.
		/// <inheritdoc />
		public override int MaxAliasLength => 30;

		#region Informational metadata

		/// <summary>
		/// Does this dialect support distributed transaction?
		/// </summary>
		/// <remarks>
		/// As of v2.5 and 3.0.2, fails rollback-ing changes when distributed: changes are instead persisted in database.
		/// (With ADO .Net Provider 5.9.1)
		/// </remarks>
		public override bool SupportsDistributedTransactions => false;

		#endregion
	}
}
