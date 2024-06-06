using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
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
			RegisterColumnType(DbType.Binary, "BLOB");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB");
			RegisterColumnType(DbType.Boolean, "SMALLINT");
			RegisterColumnType(DbType.Byte, "SMALLINT");
			RegisterColumnType(DbType.Currency, "DECIMAL(18,4)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
			// DB2 max precision is 31, but .Net is 28-29 anyway.
			RegisterColumnType(DbType.Decimal, 29, "DECIMAL($p, $s)");
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
			RegisterColumnType(DbType.Guid, "CHAR(16) FOR BIT DATA");

			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("absval", new StandardSQLFunction("absval"));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));

			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));
			RegisterFunction("round", new StandardSQLFunctionWithRequiredParameters("round", new object[] { null, "0" }));

			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("atan2", new SQLFunctionTemplate(NHibernateUtil.Double, "atan2(?2,?1)"));
			RegisterFunction("atanh", new StandardSQLFunction("atanh", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cosh", new StandardSQLFunction("cosh", NHibernateUtil.Double));
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
			RegisterFunction("random", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("sinh", new StandardSQLFunction("sinh", NHibernateUtil.Double));
			RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.String));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("tanh", new StandardSQLFunction("tanh", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));
			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));

			RegisterFunction("current_timestamp", new NoArgSQLFunction("current_timestamp", NHibernateUtil.LocalDateTime, false));
			RegisterFunction("current_date", new NoArgSQLFunction("current_date", NHibernateUtil.LocalDate, false));
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
			RegisterFunction("timestamp", new StandardSQLFunction("timestamp", NHibernateUtil.DateTime));
			RegisterFunction("timestamp_iso", new StandardSQLFunction("timestamp_iso", NHibernateUtil.DateTime));
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

			RegisterFunction("str", new StandardSQLFunction("varchar", NHibernateUtil.AnsiString));
			RegisterFunction("strguid", new SQLFunctionTemplate(NHibernateUtil.String, "substr(hex(?1), 7, 2) || substr(hex(?1), 5, 2) || substr(hex(?1), 3, 2) || substr(hex(?1), 1, 2) || '-' || substr(hex(?1), 11, 2) || substr(hex(?1), 9, 2) || '-' || substr(hex(?1), 15, 2) || substr(hex(?1), 13, 2) || '-' || substr(hex(?1), 17, 4) || '-' || substr(hex(?1), 21) "));

			RegisterFunction("digits", new StandardSQLFunction("digits", NHibernateUtil.String));
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("chr", new StandardSQLFunction("chr", NHibernateUtil.Character));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("ucase", new StandardSQLFunction("ucase"));
			RegisterFunction("lcase", new StandardSQLFunction("lcase"));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int32));
			RegisterFunction("octet_length", new StandardSQLFunction("octet_length", NHibernateUtil.Int32));
			RegisterFunction("bit_length", new SQLFunctionTemplate(NHibernateUtil.Int32, "octet_length(?1) * 8"));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("replace", new StandardSQLFunction("replace", NHibernateUtil.String));
			RegisterFunction("truncate", new StandardSQLFunction("truncate"));
			RegisterFunction("left", new StandardSQLFunction("left", NHibernateUtil.String));
			RegisterFunction("right", new StandardSQLFunction("right", NHibernateUtil.String));

			RegisterFunction("mod", new ModulusFunction(true, false));

			RegisterFunction("substring", new StandardSQLFunction("substr", NHibernateUtil.String));

			// Bitwise operations
			RegisterFunction("band", new Function.BitwiseFunctionOperation("bitand"));
			RegisterFunction("bor", new Function.BitwiseFunctionOperation("bitor"));
			RegisterFunction("bxor", new Function.BitwiseFunctionOperation("bitxor"));
			RegisterFunction("bnot", new Function.BitwiseFunctionOperation("bitnot"));

			RegisterFunction("nvl", new StandardSQLFunction("nvl"));
			RegisterFunction("iif", new IifSQLFunction());

			RegisterKeywords();

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.DB2Driver";
		}
		
		#region private static readonly string[] DialectKeywords = { ... }

		private static readonly string[] DialectKeywords =
		{
			"abs",
			"activate",
			"add",
			"after",
			"alias",
			"all",
			"allocate",
			"allow",
			"alter",
			"and",
			"any",
			"are",
			"array",
			"array_exists",
			"as",
			"asensitive",
			"associate",
			"asutime",
			"asymmetric",
			"at",
			"atomic",
			"attributes",
			"audit",
			"authorization",
			"aux",
			"auxiliary",
			"avg",
			"before",
			"begin",
			"between",
			"bigint",
			"binary",
			"blob",
			"boolean",
			"both",
			"bufferpool",
			"by",
			"cache",
			"call",
			"called",
			"capture",
			"cardinality",
			"cascaded",
			"case",
			"cast",
			"ccsid",
			"ceil",
			"ceiling",
			"char",
			"char_length",
			"character",
			"character_length",
			"check",
			"clob",
			"clone",
			"close",
			"cluster",
			"coalesce",
			"collate",
			"collect",
			"collection",
			"collid",
			"column",
			"comment",
			"commit",
			"concat",
			"condition",
			"connect",
			"connection",
			"constraint",
			"contains",
			"content",
			"continue",
			"convert",
			"corr",
			"corresponding",
			"count",
			"count_big",
			"covar_pop",
			"covar_samp",
			"create",
			"cross",
			"cube",
			"cume_dist",
			"current",
			"current_date",
			"current_default_transform_group",
			"current_lc_ctype",
			"current_path",
			"current_role",
			"current_schema",
			"current_server",
			"current_time",
			"current_timestamp",
			"current_timezone",
			"current_transform_group_for_type",
			"current_user",
			"currval",
			"cursor",
			"cycle",
			"data",
			"database",
			"datapartitionname",
			"datapartitionnum",
			"date",
			"day",
			"days",
			"db2general",
			"db2genrl",
			"db2sql",
			"dbinfo",
			"dbpartitionname",
			"dbpartitionnum",
			"deallocate",
			"dec",
			"decimal",
			"declare",
			"default",
			"defaults",
			"definition",
			"delete",
			"dense_rank",
			"denserank",
			"deref",
			"describe",
			"descriptor",
			"deterministic",
			"diagnostics",
			"disable",
			"disallow",
			"disconnect",
			"distinct",
			"do",
			"document",
			"double",
			"drop",
			"dssize",
			"dynamic",
			"each",
			"editproc",
			"element",
			"else",
			"elseif",
			"enable",
			"encoding",
			"encryption",
			"end",
			"end-exec",
			"ending",
			"erase",
			"escape",
			"every",
			"except",
			"exception",
			"excluding",
			"exclusive",
			"exec",
			"execute",
			"exists",
			"exit",
			"exp",
			"explain",
			"extended",
			"external",
			"extract",
			"false",
			"fenced",
			"fetch",
			"fieldproc",
			"file",
			"filter",
			"final",
			"first",
			"fl",
			"float",
			"floor",
			"for",
			"foreign",
			"free",
			"from",
			"full",
			"function",
			"fusion",
			"general",
			"generated",
			"get",
			"global",
			"go",
			"goto",
			"grant",
			"graphic",
			"group",
			"grouping",
			"handler",
			"hash",
			"hashed_value",
			"having",
			"hint",
			"hold",
			"hour",
			"hours",
			"identity",
			"if",
			"immediate",
			"import",
			"in",
			"including",
			"inclusive",
			"increment",
			"index",
			"indicator",
			"indicators",
			"inf",
			"infinity",
			"inherit",
			"inner",
			"inout",
			"insensitive",
			"insert",
			"int",
			"integer",
			"integrity",
			"intersect",
			"intersection",
			"interval",
			"into",
			"is",
			"isnull",
			"isobid",
			"isolation",
			"iterate",
			"jar",
			"java",
			"join",
			"keep",
			"key",
			"label",
			"language",
			"large",
			"last",
			"lateral",
			"lc_ctype",
			"leading",
			"leave",
			"left",
			"like",
			"limit",
			"linktype",
			"ln",
			"local",
			"localdate",
			"locale",
			"localtime",
			"localtimestamp",
			"locator",
			"locators",
			"lock",
			"lockmax",
			"locksize",
			"long",
			"loop",
			"lower",
			"maintained",
			"match",
			"materialized",
			"max",
			"maxvalue",
			"member",
			"merge",
			"method",
			"microsecond",
			"microseconds",
			"min",
			"minute",
			"minutes",
			"minvalue",
			"mod",
			"mode",
			"modifies",
			"module",
			"month",
			"months",
			"multiset",
			"nan",
			"national",
			"natural",
			"nchar",
			"nclob",
			"new",
			"new_table",
			"next",
			"nextval",
			"no",
			"nocache",
			"nocycle",
			"nodename",
			"nodenumber",
			"nomaxvalue",
			"nominvalue",
			"none",
			"noorder",
			"normalize",
			"normalized",
			"not",
			"notnull",
			"null",
			"nullif",
			"nulls",
			"numeric",
			"numparts",
			"obid",
			"octet_length",
			"of",
			"off",
			"offset",
			"old",
			"old_table",
			"on",
			"only",
			"open",
			"optimization",
			"optimize",
			"option",
			"or",
			"order",
			"organization",
			"out",
			"outer",
			"over",
			"overlaps",
			"overlay",
			"overriding",
			"package",
			"padded",
			"pagesize",
			"parameter",
			"part",
			"partition",
			"partitioned",
			"partitioning",
			"partitions",
			"password",
			"path",
			"percent",
			"percent_rank",
			"percentile_cont",
			"percentile_disc",
			"period",
			"piecesize",
			"plan",
			"position",
			"power",
			"precision",
			"prepare",
			"prevval",
			"primary",
			"prior",
			"priqty",
			"privileges",
			"procedure",
			"program",
			"psid",
			"public",
			"query",
			"queryno",
			"range",
			"rank",
			"read",
			"reads",
			"real",
			"recovery",
			"recursive",
			"ref",
			"references",
			"referencing",
			"refresh",
			"regr_avgx",
			"regr_avgy",
			"regr_count",
			"regr_intercept",
			"regr_r2",
			"regr_slope",
			"regr_sxx",
			"regr_sxy",
			"regr_syy",
			"release",
			"rename",
			"repeat",
			"reset",
			"resignal",
			"restart",
			"restrict",
			"result",
			"result_set_locator",
			"return",
			"returns",
			"revoke",
			"right",
			"role",
			"rollback",
			"rollup",
			"round_ceiling",
			"round_down",
			"round_floor",
			"round_half_down",
			"round_half_even",
			"round_half_up",
			"round_up",
			"routine",
			"row",
			"row_number",
			"rownumber",
			"rows",
			"rowset",
			"rrn",
			"run",
			"savepoint",
			"schema",
			"scope",
			"scratchpad",
			"scroll",
			"search",
			"second",
			"seconds",
			"secqty",
			"security",
			"select",
			"sensitive",
			"sequence",
			"session",
			"session_user",
			"set",
			"signal",
			"similar",
			"simple",
			"smallint",
			"snan",
			"some",
			"source",
			"specific",
			"specifictype",
			"sql",
			"sqlexception",
			"sqlid",
			"sqlstate",
			"sqlwarning",
			"sqrt",
			"stacked",
			"standard",
			"start",
			"starting",
			"statement",
			"static",
			"statment",
			"stay",
			"stddev_pop",
			"stddev_samp",
			"stogroup",
			"stores",
			"style",
			"submultiset",
			"substring",
			"sum",
			"summary",
			"symmetric",
			"synonym",
			"sysdate",
			"sysfun",
			"sysibm",
			"sysproc",
			"system",
			"system_user",
			"systimestamp",
			"table",
			"tablesample",
			"tablespace",
			"then",
			"time",
			"timestamp",
			"timezone_hour",
			"timezone_minute",
			"to",
			"trailing",
			"transaction",
			"translate",
			"translation",
			"treat",
			"trigger",
			"trim",
			"true",
			"truncate",
			"type",
			"uescape",
			"undo",
			"union",
			"unique",
			"unknown",
			"unnest",
			"until",
			"update",
			"upper",
			"usage",
			"user",
			"using",
			"validproc",
			"value",
			"values",
			"var_pop",
			"var_samp",
			"varbinary",
			"varchar",
			"variable",
			"variant",
			"varying",
			"vcat",
			"version",
			"versioning",
			"view",
			"volatile",
			"volumes",
			"when",
			"whenever",
			"where",
			"while",
			"width_bucket",
			"window",
			"with",
			"within",
			"without",
			"wlm",
			"write",
			"xmlcast",
			"xmlelement",
			"xmlexists",
			"xmlnamespaces",
			"year",
			"years",
			"zone",
		};

		#endregion

		protected virtual void RegisterKeywords()
		{
			RegisterKeywords(DialectKeywords);
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

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new DB2MetaData(connection);
		}

		/// <summary></summary>
		public override bool SupportsSequences
		{
			get { return true; }
		}

		public override string QuerySequencesString => "select seqname from syscat.sequences";

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

		/// <summary></summary>
		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		public override SqlString GetLimitString(SqlString sql, SqlString offset, SqlString limit)
		{
			if (offset == null)
			{
				return new SqlString(sql, " fetch first ", limit, " rows only");
			}

			ExtractColumnOrAliasNames(sql, out var selectColumns, out _, out _);

			/*
			 * "select * from (select row_number() over(orderby_clause) as rownum, "
			 * querySqlString_without select
			 * " ) as tempresult where rownum between ? and ?"
			 */
			string rownumClause = GetRowNumber(sql);

			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder
				.Add("select " )
				.Add(string.Join(",", selectColumns))
				.Add(" from (select ")
				.Add(rownumClause)
				.Add(sql.Substring(7))
				.Add(") as tempresult where rownum ");

			if (limit != null)
			{
				pagingBuilder
					.Add("between ")
					.Add(offset)
					.Add("+1 and ")
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

		// As of DB2 9.5 documentation, limit is 128 bytes which with Unicode names could mean only 32 characters.
		/// <inheritdoc />
		public override int MaxAliasLength => 32;

		public override long TimestampResolutionInTicks => 10L; // Microseconds.

		#region Overridden informational metadata

		public override bool SupportsNullInUnique => false;

		public override bool SupportsUnionAll => true;

		public override bool SupportsEmptyInList => false;

		public override bool SupportsResultSetPositionQueryMethodsOnForwardOnlyCursor => false;

		/// <inheritdoc />
		public override bool SupportsCrossJoin => false; // DB2 v9.1 doesn't support 'cross join' syntax

		public override bool SupportsLobValueChangePropogation => false;

		public override bool SupportsExistsInSelect => false;

		/// <inheritdoc/>
		public override bool SupportsHavingOnGroupedByComputation => false;

		public override bool DoesReadCommittedCauseWritersToBlockReaders => true;

		#endregion

		public override bool SupportsTemporaryTables => true;
		public override string CreateTemporaryTableString => "create global temporary table";
	}
}
