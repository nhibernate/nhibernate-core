using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect base class for SAP HANA
	/// </summary>
	/// <remarks>
	/// The AbstractHanaDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.HanaDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public abstract class AbstractHanaDialect : Dialect
	{
		[Serializable]
		private class TypeConvertingVarArgsSQLFunction : ISQLFunction
		{
			private readonly string begin;
			private readonly string sep;
			private readonly string end;
			private SqlType type = null;
			private Dialect dialect = null;

			public TypeConvertingVarArgsSQLFunction(string begin, string sep, string end)
			{
				this.begin = begin;
				this.sep = sep;
				this.end = end;
			}

			#region ISQLFunction Members

			public IType ReturnType(IType columnType, IMapping mapping)
			{
				type = columnType.SqlTypes(mapping)[0];
				dialect = mapping.Dialect;
				return columnType;
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
				SqlStringBuilder buf = new SqlStringBuilder().Add(begin);
				for (int i = 0; i < args.Count; i++)
				{
					object arg = args[i];
					if (arg is SqlString && (arg as SqlString).EqualsCaseInsensitive("?"))
					{
						buf.Add("cast(");
						buf.AddObject(arg);
						buf.Add(" as ");
						buf.Add(dialect.GetCastTypeName(type));
						buf.Add(")");
					}
					else
					{
						buf.AddObject(arg);
					}
					if (i < args.Count - 1) buf.Add(sep);
				}
				return buf.Add(end).ToSqlString();
			}

			#endregion
		}

		public AbstractHanaDialect()
		{
			//string type
			RegisterColumnType(DbType.AnsiStringFixedLength, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 5000, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 2147483647, "CLOB");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 5000, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "CLOB");
			RegisterColumnType(DbType.StringFixedLength, "NVARCHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 5000, "NVARCHAR($l)");
			RegisterColumnType(DbType.StringFixedLength, 2147483647, "NCLOB");
			RegisterColumnType(DbType.String, "NVARCHAR(255)");
			RegisterColumnType(DbType.String, 5000, "NVARCHAR($l)");
			RegisterColumnType(DbType.String, 2147483647, "NCLOB");

			//binary type:
			RegisterColumnType(DbType.Binary, "VARBINARY(5000)");
			RegisterColumnType(DbType.Binary, 5000, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB");

			//Numeric type:
			RegisterColumnType(DbType.Boolean, "BOOLEAN");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "DECIMAL(19, 4)");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19, 5)");
			RegisterColumnType(DbType.Decimal, 29, "DECIMAL($p, $s)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "FLOAT");
			RegisterColumnType(DbType.SByte, "SMALLINT");

			//Date and time type:
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
			RegisterColumnType(DbType.Time, "TIME");

			//special:
			RegisterColumnType(DbType.Guid, "VARCHAR(36)");

			RegisterKeywords();

			//functions:
			RegisterFunctions();

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.HanaDriver";
		}

		#region private static readonly string[] DialectKeywords = { ... }

		private static readonly string[] DialectKeywords =
		{
			"add",
			"aes_decrypt",
			"all",
			"alphanum",
			"alter",
			"and",
			"any",
			"array",
			"as",
			"asc",
			"attach",
			"backup",
			"begin",
			"between",
			"bigint",
			"binary",
			"bit",
			"bottom",
			"break",
			"by",
			"call",
			"capability",
			"cascade",
			"case",
			"cast",
			"char",
			"char_convert",
			"character",
			"check",
			"checkpoint",
			"close",
			"comment",
			"commit",
			"compressed",
			"conflict",
			"connect",
			"constraint",
			"contains",
			"continue",
			"convert",
			"create",
			"cross",
			"cube",
			"current",
			"current_timestamp",
			"current_user",
			"cursor",
			"date",
			"datetimeoffset",
			"dbspace",
			"deallocate",
			"dec",
			"decimal",
			"declare",
			"default",
			"delete",
			"deleting",
			"desc",
			"detach",
			"distinct",
			"do",
			"double",
			"drop",
			"dynamic",
			"else",
			"elseif",
			"encrypted",
			"end",
			"endif",
			"escape",
			"except",
			"exception",
			"exec",
			"execute",
			"existing",
			"exists",
			"externlogin",
			"fetch",
			"first",
			"float",
			"for",
			"force",
			"foreign",
			"forward",
			"from",
			"full",
			"goto",
			"grant",
			"group",
			"having",
			"holdlock",
			"identified",
			"if",
			"in",
			"index",
			"inner",
			"inout",
			"insensitive",
			"insert",
			"inserting",
			"install",
			"instead",
			"integer",
			"integrated",
			"intersect",
			"into",
			"iq",
			"is",
			"isolation",
			"join",
			"json",
			"kerberos",
			"key",
			"lateral",
			"left",
			"like",
			"limit",
			"lock",
			"login",
			"long",
			"match",
			"membership",
			"merge",
			"message",
			"mode",
			"modify",
			"natural",
			"nchar",
			"new",
			"no",
			"noholdlock",
			"not",
			"notify",
			"null",
			"numeric",
			"nvarchar",
			"of",
			"off",
			"on",
			"open",
			"openstring",
			"openxml",
			"option",
			"options",
			"or",
			"order",
			"others",
			"out",
			"outer",
			"over",
			"passthrough",
			"precision",
			"prepare",
			"primary",
			"print",
			"privileges",
			"proc",
			"procedure",
			"publication",
			"raiserror",
			"readtext",
			"real",
			"reference",
			"references",
			"refresh",
			"release",
			"remote",
			"remove",
			"rename",
			"reorganize",
			"resource",
			"restore",
			"restrict",
			"return",
			"revoke",
			"right",
			"rollback",
			"rollup",
			"row",
			"rowtype",
			"save",
			"savepoint",
			"scroll",
			"seconddate",
			"select",
			"sensitive",
			"session",
			"set",
			"setuser",
			"share",
			"shorttext",
			"smalldecimal",
			"smallint",
			"some",
			"spatial",
			"sqlcode",
			"sqlstate",
			"start",
			"stop",
			"subtrans",
			"subtransaction",
			"synchronize",
			"table",
			"temporary",
			"text",
			"then",
			"time",
			"timestamp",
			"tinyint",
			"to",
			"top",
			"treat",
			"trigger",
			"truncate",
			"tsequal",
			"unbounded",
			"union",
			"unique",
			"uniqueidentifier",
			"unknown",
			"unnest",
			"unsigned",
			"update",
			"updating",
			"user",
			"using",
			"validate",
			"values",
			"varbinary",
			"varbit",
			"varchar",
			"variable",
			"varray",
			"varying",
			"view",
			"wait",
			"waitfor",
			"when",
			"where",
			"while",
			"window",
			"with",
			"within",
			"work",
			"writetext",
			"xml"
		};

		#endregion

		protected virtual void RegisterKeywords()
		{
			RegisterKeywords(DialectKeywords);
		}

		protected virtual void RegisterFunctions()
		{
			RegisterHANAFunctions();
			RegisterNHibernateFunctions();
		}

		protected virtual void RegisterNHibernateFunctions()
		{
			RegisterFunction("band", new BitwiseFunctionOperation("bitand"));
			RegisterFunction("bor", new BitwiseFunctionOperation("bitor"));
			RegisterFunction("bxor", new BitwiseFunctionOperation("bitxor"));
			RegisterFunction("bnot", new BitwiseFunctionOperation("bitnot"));
			RegisterFunction("bit_length", new SQLFunctionTemplate(NHibernateUtil.Int32, "length(to_binary(?1))*8"));
			RegisterFunction("ceiling", new StandardSQLFunction("ceil"));
			RegisterFunction("chr", new StandardSQLFunction("char", NHibernateUtil.AnsiChar));
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "to_date(?1)"));
			RegisterFunction("iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));
			RegisterFunction("Iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));
			RegisterFunction("sysdate", new NoArgSQLFunction("current_timestamp", NHibernateUtil.DateTime, false));
			RegisterFunction("truncate", new SQLFunctionTemplateWithRequiredParameters(null, "floor(?1 * power(10, ?2)) / power(10, ?2)", new object[] { null, "0" }));
		}

		protected virtual void RegisterHANAFunctions()
		{
			RegisterFunction("abap_alphanum", new StandardSQLFunction("abap_alphanum", NHibernateUtil.String));
			RegisterFunction("abap_lower", new StandardSQLFunction("abap_lower", NHibernateUtil.String));
			RegisterFunction("abap_numc", new StandardSQLFunction("abap_numc", NHibernateUtil.String));
			RegisterFunction("abap_upper", new StandardSQLFunction("abap_upper", NHibernateUtil.String));
			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("add_days", new StandardSQLFunction("add_days", NHibernateUtil.DateTime));
			RegisterFunction("add_months", new StandardSQLFunction("add_months", NHibernateUtil.DateTime));
			RegisterFunction("add_months_last", new StandardSQLFunction("add_months_last", NHibernateUtil.DateTime));
			RegisterFunction("add_seconds", new StandardSQLFunction("add_seconds", NHibernateUtil.DateTime));
			RegisterFunction("add_workdays", new StandardSQLFunction("add_workdays", NHibernateUtil.DateTime));
			RegisterFunction("add_years", new StandardSQLFunction("add_years", NHibernateUtil.DateTime));
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("atan2", new StandardSQLFunction("atan2", NHibernateUtil.Double));
			RegisterFunction("auto_corr", new StandardSQLFunction("auto_corr", NHibernateUtil.Object));
			RegisterFunction("bintohex", new StandardSQLFunction("bintohex", NHibernateUtil.String));
			RegisterFunction("bintonhex", new StandardSQLFunction("bintonhex", NHibernateUtil.String));
			RegisterFunction("bintostr", new StandardSQLFunction("bintostr", NHibernateUtil.String));
			RegisterFunction("bitand", new StandardSQLFunction("bitand"));
			RegisterFunction("bitcount", new StandardSQLFunction("bitcount", NHibernateUtil.Int32));
			RegisterFunction("bitnot", new StandardSQLFunction("bitnot"));
			RegisterFunction("bitor", new StandardSQLFunction("bitor"));
			RegisterFunction("bitset", new StandardSQLFunction("bitset", NHibernateUtil.Binary));
			RegisterFunction("bitunset", new StandardSQLFunction("bitunset", NHibernateUtil.Binary));
			RegisterFunction("bitxor", new StandardSQLFunction("bitxor"));
			RegisterFunction("cardinality", new StandardSQLFunction("cardinality", NHibernateUtil.Int32));
			RegisterFunction("cast", new CastFunction());
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("char", new StandardSQLFunction("char", NHibernateUtil.AnsiChar));
			RegisterFunction("coalesce", new TypeConvertingVarArgsSQLFunction("coalesce(", ",", ")"));
			RegisterFunction("concat", new VarArgsSQLFunction("(", " || ", ")"));
			RegisterFunction("concat_naz", new StandardSQLFunction("concat_naz", NHibernateUtil.String));
			RegisterFunction("convert_currency", new VarArgsSQLFunction("convert_currency(", ",", ")"));
			RegisterFunction("convert_unit", new VarArgsSQLFunction("convert_unit(", ",", ")"));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cosh", new StandardSQLFunction("cosh", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("current_connection", new NoArgSQLFunction("current_connection", NHibernateUtil.Int32));
			RegisterFunction("current_date", new NoArgSQLFunction("current_date", NHibernateUtil.DateTime, false));
			RegisterFunction("current_identity_value", new NoArgSQLFunction("current_identity_value", NHibernateUtil.Int64));
			RegisterFunction("current_mvcc_snapshot_timestamp", new NoArgSQLFunction("current_mvcc_snapshot_timestamp", NHibernateUtil.Int32));
			RegisterFunction("current_object_schema", new NoArgSQLFunction("current_object_schema", NHibernateUtil.String));
			RegisterFunction("current_schema", new NoArgSQLFunction("current_schema", NHibernateUtil.String, false));
			RegisterFunction("current_time", new NoArgSQLFunction("current_time", NHibernateUtil.DateTime, false));
			RegisterFunction("current_timestamp", new NoArgSQLFunction("current_timestamp", NHibernateUtil.DateTime, false));
			RegisterFunction("current_transaction_isolation_level", new NoArgSQLFunction("current_transaction_isolation_level", NHibernateUtil.String, false));
			RegisterFunction("current_update_statement_sequence", new NoArgSQLFunction("current_update_statement_sequence", NHibernateUtil.Int64));
			RegisterFunction("current_update_transaction", new NoArgSQLFunction("current_update_transaction", NHibernateUtil.Int64));
			RegisterFunction("current_user", new NoArgSQLFunction("current_user", NHibernateUtil.String, false));
			RegisterFunction("current_utcdate", new NoArgSQLFunction("current_utcdate", NHibernateUtil.DateTime, false));
			RegisterFunction("current_utctime", new NoArgSQLFunction("current_utctime", NHibernateUtil.DateTime, false));
			RegisterFunction("current_utctimestamp", new NoArgSQLFunction("current_utctimestamp", NHibernateUtil.DateTime, false));
			RegisterFunction("dayname", new StandardSQLFunction("dayname", NHibernateUtil.String));
			RegisterFunction("dayofmonth", new StandardSQLFunction("dayofmonth", NHibernateUtil.Int32));
			RegisterFunction("dayofyear", new StandardSQLFunction("dayofyear", NHibernateUtil.Int32));
			RegisterFunction("days_between", new StandardSQLFunction("days_between", NHibernateUtil.Int32));
			RegisterFunction("encryption_root_keys_extract_keys", new StandardSQLFunction("encryption_root_keys_extract_keys", NHibernateUtil.StringClob));
			RegisterFunction("encryption_root_keys_has_backup_password", new StandardSQLFunction("encryption_root_keys_has_backup_password", NHibernateUtil.Int16));
			RegisterFunction("escape_double_quotes", new StandardSQLFunction("escape_double_quotes", NHibernateUtil.String));
			RegisterFunction("escape_single_quotes", new StandardSQLFunction("escape_single_quotes", NHibernateUtil.String));
			RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));
			RegisterFunction("extract", new AnsiExtractFunction());
			RegisterFunction("floor", new StandardSQLFunction("floor"));
			RegisterFunction("greatest", new VarArgsSQLFunction("greatest(", ",", ")"));
			RegisterFunction("grouping", new StandardSQLFunction("grouping", NHibernateUtil.Int32));
			RegisterFunction("grouping_id", new StandardSQLFunction("grouping_id", NHibernateUtil.Int32));
			RegisterFunction("hamming_distance", new StandardSQLFunction("hamming_distance", NHibernateUtil.Int32));
			RegisterFunction("hash_md5", new StandardSQLFunction("hash_md5", NHibernateUtil.Binary));
			RegisterFunction("hash_sha256", new StandardSQLFunction("hash_sha256", NHibernateUtil.Binary));
			RegisterFunction("hextobin", new StandardSQLFunction("hextobin", NHibernateUtil.Binary));
			RegisterFunction("hour", new StandardSQLFunction("hour", NHibernateUtil.Int32));
			RegisterFunction("ifnull", new StandardSQLFunction("ifnull"));
			RegisterFunction("indexing_error_code", new StandardSQLFunction("indexing_error_code", NHibernateUtil.Int32));
			RegisterFunction("indexing_error_message", new StandardSQLFunction("indexing_error_message", NHibernateUtil.String));
			RegisterFunction("indexing_status", new StandardSQLFunction("indexing_status", NHibernateUtil.String));
			RegisterFunction("initcap", new StandardSQLFunction("initcap", NHibernateUtil.String));
			RegisterFunction("is_sql_injection_safe", new StandardSQLFunction("is_sql_injection_safe", NHibernateUtil.Int32));
			RegisterFunction("isoweek", new StandardSQLFunction("isoweek", NHibernateUtil.Int32));
			RegisterFunction("language", new StandardSQLFunction("language", NHibernateUtil.String));
			RegisterFunction("last_day", new StandardSQLFunction("last_day", NHibernateUtil.DateTime));
			RegisterFunction("lcase", new StandardSQLFunction("lcase", NHibernateUtil.String));
			RegisterFunction("least", new VarArgsSQLFunction("least(", ",", ")"));
			RegisterFunction("left", new StandardSQLFunction("left", NHibernateUtil.String));
			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int32));
			RegisterFunction("ln", new StandardSQLFunction("ln", NHibernateUtil.Double));
			RegisterFunction("localtoutc", new StandardSQLFunction("localtoutc", NHibernateUtil.DateTime));
			RegisterFunction("locate", new SQLFunctionTemplateWithRequiredParameters(NHibernateUtil.Int32, "locate(?2, ?1, ?3)", new object[] { null, null, "0" }));
			RegisterFunction("locate_regexpr", new StandardSQLFunction("locate_regexpr", NHibernateUtil.Int32));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("lower", new StandardSQLFunction("lower", NHibernateUtil.String));
			RegisterFunction("lpad", new StandardSQLFunction("lpad", NHibernateUtil.String));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim", NHibernateUtil.String));
			RegisterFunction("map", new VarArgsSQLFunction("map(", ",", ")"));
			RegisterFunction("mimetype", new StandardSQLFunction("mimetype", NHibernateUtil.String));
			RegisterFunction("minute", new StandardSQLFunction("minute", NHibernateUtil.Int32));
			RegisterFunction("mod", new StandardSQLFunction("mod", NHibernateUtil.Int32));
			RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
			RegisterFunction("monthname", new StandardSQLFunction("monthname", NHibernateUtil.String));
			RegisterFunction("months_between", new StandardSQLFunction("months_between", NHibernateUtil.Int32));
			RegisterFunction("nano100_between", new StandardSQLFunction("nano100_between", NHibernateUtil.Int64));
			RegisterFunction("nchar", new StandardSQLFunction("nchar", NHibernateUtil.Character));
			RegisterFunction("ndiv0", new StandardSQLFunction("ndiv0", NHibernateUtil.Double));
			RegisterFunction("next_day", new StandardSQLFunction("next_day", NHibernateUtil.DateTime));
			RegisterFunction("newuid", new NoArgSQLFunction("newuid", NHibernateUtil.String));
			RegisterFunction("now", new NoArgSQLFunction("now", NHibernateUtil.DateTime));
			RegisterFunction("nullif", new StandardSQLFunction("nullif"));
			RegisterFunction("occurrences_regexpr", new StandardSQLFunction("occurrences_regexpr", NHibernateUtil.Int32));
			RegisterFunction("plaintext", new StandardSQLFunction("plaintext", NHibernateUtil.String));
			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));
			RegisterFunction("quarter", new StandardSQLFunction("quarter", NHibernateUtil.String));
			RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("rand_secure", new NoArgSQLFunction("rand_secure", NHibernateUtil.Double));
			RegisterFunction("replace", new StandardSQLFunction("replace", NHibernateUtil.String));
			RegisterFunction("replace_regexpr", new StandardSQLFunction("replace_regexpr", NHibernateUtil.String));
			RegisterFunction("result_cache_id", new NoArgSQLFunction("result_cache_id", NHibernateUtil.Int64));
			RegisterFunction("result_cache_refresh_time", new NoArgSQLFunction("result_cache_refresh_time", NHibernateUtil.DateTime));
			RegisterFunction("right", new StandardSQLFunction("right", NHibernateUtil.String));
			RegisterFunction("round", new StandardSQLFunction("round"));
			RegisterFunction("rpad", new StandardSQLFunction("rpad", NHibernateUtil.String));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim", NHibernateUtil.String));
			RegisterFunction("score", new NoArgSQLFunction("score", NHibernateUtil.Double));
			RegisterFunction("second", new StandardSQLFunction("second", NHibernateUtil.Double));
			RegisterFunction("seconds_between", new StandardSQLFunction("seconds_between", NHibernateUtil.Int64));
			RegisterFunction("session_context", new StandardSQLFunction("session_context", NHibernateUtil.String));
			RegisterFunction("session_user", new NoArgSQLFunction("session_user", NHibernateUtil.String, false));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("sinh", new StandardSQLFunction("sinh", NHibernateUtil.Double));
			RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.String));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("stddev_pop", new StandardSQLFunction("stddev_pop", NHibernateUtil.Double));
			RegisterFunction("stddev_samp", new StandardSQLFunction("stddev_samp", NHibernateUtil.Double));
			RegisterFunction("string_agg", new StandardSQLFunction("String_agg", NHibernateUtil.String));
			RegisterFunction("strtobin", new StandardSQLFunction("strtobin", NHibernateUtil.BinaryBlob));
			RegisterFunction("subarray", new StandardSQLFunction("subarray"));
			RegisterFunction("substr_after", new StandardSQLFunction("substr_after", NHibernateUtil.String));
			RegisterFunction("substr_before", new StandardSQLFunction("substr_before", NHibernateUtil.String));
			RegisterFunction("substring_regexpr", new StandardSQLFunction("substring_regexpr", NHibernateUtil.String));
			RegisterFunction("substring", new StandardSQLFunction("substring", NHibernateUtil.String));
			RegisterFunction("sysuuid", new NoArgSQLFunction("sysuuid", NHibernateUtil.String, false));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("tanh", new StandardSQLFunction("tanh", NHibernateUtil.Double));
			RegisterFunction("to_alphanum", new StandardSQLFunction("to_alphanum", NHibernateUtil.String));
			RegisterFunction("to_bigint", new StandardSQLFunction("to_bigint", NHibernateUtil.Int64));
			RegisterFunction("to_binary", new StandardSQLFunction("to_binary", NHibernateUtil.Binary));
			RegisterFunction("to_blob", new StandardSQLFunction("to_blob", NHibernateUtil.BinaryBlob));
			RegisterFunction("to_clob", new StandardSQLFunction("to_clob", NHibernateUtil.StringClob));
			RegisterFunction("to_date", new StandardSQLFunction("to_date", NHibernateUtil.Date));
			RegisterFunction("to_dats", new StandardSQLFunction("to_dats", NHibernateUtil.String));
			RegisterFunction("to_decimal", new StandardSQLFunction("to_decimal", NHibernateUtil.Decimal));
			RegisterFunction("to_double", new StandardSQLFunction("to_double", NHibernateUtil.Double));
			RegisterFunction("to_fixedchar", new StandardSQLFunction("to_fixedchar", NHibernateUtil.Character));
			RegisterFunction("to_int", new StandardSQLFunction("to_int", NHibernateUtil.Int32));
			RegisterFunction("to_integer", new StandardSQLFunction("to_integer", NHibernateUtil.Int32));
			RegisterFunction("to_nclob", new StandardSQLFunction("to_nclob", NHibernateUtil.StringClob));
			RegisterFunction("to_nvarchar", new StandardSQLFunction("to_nvarchar", NHibernateUtil.String));
			RegisterFunction("to_real", new StandardSQLFunction("to_real", NHibernateUtil.Double));
			RegisterFunction("to_seconddate", new StandardSQLFunction("to_seconddate", NHibernateUtil.DateTime));
			RegisterFunction("to_smalldecimal", new StandardSQLFunction("to_smalldecimal", NHibernateUtil.Decimal));
			RegisterFunction("to_smallint", new StandardSQLFunction("to_smallint", NHibernateUtil.Int16));
			RegisterFunction("to_time", new StandardSQLFunction("to_time", NHibernateUtil.Time));
			RegisterFunction("to_timestamp", new StandardSQLFunction("to_timestamp", NHibernateUtil.DateTime));
			RegisterFunction("to_tinyint", new StandardSQLFunction("to_tinyint", NHibernateUtil.Byte));
			RegisterFunction("to_varchar", new StandardSQLFunction("to_varchar", NHibernateUtil.AnsiString));
			RegisterFunction("trim", new AnsiTrimFunction());
			RegisterFunction("trim_array", new StandardSQLFunction("trim_array"));
			RegisterFunction("ucase", new StandardSQLFunction("ucase", NHibernateUtil.String));
			RegisterFunction("uminus", new StandardSQLFunction("uminus"));
			RegisterFunction("unicode", new StandardSQLFunction("unicode", NHibernateUtil.Int32));
			RegisterFunction("upper", new StandardSQLFunction("upper", NHibernateUtil.String));
			RegisterFunction("utctolocal", new StandardSQLFunction("utctolocal", NHibernateUtil.DateTime));
			RegisterFunction("var_pop", new StandardSQLFunction("var_pop", NHibernateUtil.Double));
			RegisterFunction("var_samp", new StandardSQLFunction("var_samp", NHibernateUtil.Double));
			RegisterFunction("week", new StandardSQLFunction("week", NHibernateUtil.Int32));
			RegisterFunction("weekday", new StandardSQLFunction("weekday", NHibernateUtil.Int32));
			RegisterFunction("width_bucket", new StandardSQLFunction("width_bucket", NHibernateUtil.Int32));
			RegisterFunction("workdays_between", new StandardSQLFunction("workdays_between", NHibernateUtil.Int32));
			RegisterFunction("xmlextract", new StandardSQLFunction("xmlextract", NHibernateUtil.String));
			RegisterFunction("xmlextractvalue", new StandardSQLFunction("xmlextractvalue", NHibernateUtil.String));
			RegisterFunction("xmltable", new StandardSQLFunction("xmltable"));
			RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));
			RegisterFunction("years_between", new StandardSQLFunction("years_between", NHibernateUtil.Int32));

		}

		#region DDL support

		/// <summary>
		/// Do we need to drop constraints before dropping tables in the dialect?
		/// </summary>
		public override bool DropConstraints => false;

		/// <summary>
		/// Do we need to qualify index names with the schema name?
		/// </summary>
		public override bool QualifyIndexName => false;

		public override bool SupportsCommentOn => true;

		public override string GetTableComment(string comment)
		{
			return "comment '" + comment + "'";
		}

		public override string GetColumnComment(string comment)
		{
			return "comment '" + comment + "'";
		}

		/// <summary> Does this dialect support column-level check constraints? </summary>
		/// <returns> True if column-level CHECK constraints are supported; false otherwise. </returns>
		public override bool SupportsColumnCheck => false;

		/// <summary> Does this dialect support table-level check constraints? </summary>
		/// <returns> True if table-level CHECK constraints are supported; false otherwise. </returns>
		public override bool SupportsTableCheck => true;

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new HanaDataBaseSchema(connection);
		}

		#endregion

		#region Lock acquisition support

		/// <summary>Is <c>FOR UPDATE OF</c> syntax expecting columns?</summary>
		/// <value><see langword="true"/> if the database expects a column list with <c>FOR UPDATE OF</c> syntax,
		/// <see langword="false"/> if it expects table alias instead or do not support <c>FOR UPDATE OF</c> syntax.</value>
		// Since v5.1
		[Obsolete("Use UsesColumnsWithForUpdateOf instead")]
		public override bool ForUpdateOfColumns => true;

		/// <summary> 
		/// Get the <tt>FOR UPDATE OF column_list</tt> fragment appropriate for this
		/// dialect given the aliases of the columns to be write locked.
		///  </summary>
		/// <param name="aliases">The columns to be write locked. </param>
		/// <returns> The appropriate <tt>FOR UPDATE OF column_list</tt> clause string. </returns>
		public override string GetForUpdateString(string aliases)
		{
			return ForUpdateString + " of " + aliases;
		}

		/// <summary>
		/// Retrieves the <c>FOR UPDATE NOWAIT</c> syntax specific to this dialect
		/// </summary>
		/// <value>The appropriate <c>FOR UPDATE NOWAIT</c> clause string.</value>
		public override string ForUpdateNowaitString
		{
			get { return ForUpdateString + " nowait"; }
		}

		/// <summary> 
		/// Get the <c>FOR UPDATE OF column_list NOWAIT</c> fragment appropriate
		/// for this dialect given the aliases of the columns or tables to be write locked.
		/// </summary>
		/// <param name="aliases">The columns or tables to be write locked.</param>
		/// <returns>The appropriate <c>FOR UPDATE colunm_or_table_list NOWAIT</c> clause string.</returns>
		public override string GetForUpdateNowaitString(string aliases)
		{
			return GetForUpdateString(aliases) + " nowait";
		}

		#endregion

		#region Table support

		#region Temporary table support

		/// <summary> Does this dialect support temporary tables? </summary>
		public override bool SupportsTemporaryTables => true;

		/// <summary> Generate a temporary table name given the bas table. </summary>
		/// <param name="baseTableName">The table name from which to base the temp table name. </param>
		/// <returns> The generated temp table name. </returns>
		public override string GenerateTemporaryTableName(string baseTableName)
		{
			return "#HT_" + baseTableName;
		}

		/// <summary> 
		/// Does the dialect require that temporary table DDL statements occur in
		/// isolation from other statements?  This would be the case if the creation
		/// would cause any current transaction to get committed implicitly.
		///  </summary>
		/// <returns> see the result matrix above. </returns>
		/// <remarks>
		/// JDBC defines a standard way to query for this information via the
		/// {@link java.sql.DatabaseMetaData#dataDefinitionCausesTransactionCommit()}
		/// method.  However, that does not distinguish between temporary table
		/// DDL and other forms of DDL; MySQL, for example, reports DDL causing a
		/// transaction commit via its driver, even though that is not the case for
		/// temporary table DDL.
		/// <p/>
		/// Possible return values and their meanings:<ul>
		/// <li>{@link Boolean#TRUE} - Unequivocally, perform the temporary table DDL in isolation.</li>
		/// <li>{@link Boolean#FALSE} - Unequivocally, do <b>not</b> perform the temporary table DDL in isolation.</li>
		/// <li><i>null</i> - defer to the JDBC driver response in regards to {@link java.sql.DatabaseMetaData#dataDefinitionCausesTransactionCommit()}</li>
		/// </ul>
		/// </remarks>
		public override bool? PerformTemporaryTableDDLInIsolation()
		{
			return false;
		}

		/// <summary> Do we need to drop the temporary table after use? </summary>
		public override bool DropTemporaryTableAfterUse()
		{
			return true;
		}

		#endregion

		#endregion

		#region Callable statement support

		/// <summary> 
		/// Registers an OUT parameter which will be returning a
		/// <see cref="DbDataReader"/>.  How this is accomplished varies greatly
		/// from DB to DB, hence its inclusion (along with {@link #getResultSet}) here.
		///  </summary>
		/// <param name="statement">The callable statement. </param>
		/// <param name="position">The bind position at which to register the OUT param. </param>
		/// <returns> The number of (contiguous) bind positions used. </returns>
		public override int RegisterResultSetOutParameter(DbCommand statement, int position)
		{
			// Result set (TABLE) OUT parameters don't need to be registered
			return position;
		}

		#endregion

		#region Current timestamp support

		/// <summary> Does this dialect support a way to retrieve the database's current timestamp value? </summary>
		public override bool SupportsCurrentTimestampSelection => true;

		/// <summary>
		/// Gives the best resolution that the database can use for storing
		/// date/time values, in ticks.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For example, if the database can store values with 100-nanosecond
		/// precision, this property is equal to 1L. If the database can only
		/// store values with 1-millisecond precision, this property is equal
		/// to 10000L (number of ticks in a millisecond).
		/// </para>
		/// <para>
		/// Used in TimestampType.
		/// </para>
		/// </remarks>
		public override long TimestampResolutionInTicks
		{
			get { return 10L; } // Maximum precision (one tick)
		}

		#endregion

		#region Constraint support


		/// <summary>
		/// Completely optional cascading drop clause
		/// </summary>
		public override string CascadeConstraintsString
		{
			get { return " cascade"; }
		}

		#endregion

		#region Native identifier generation

		#region IDENTITY support

		/// <summary>
		/// Does this dialect support identity column key generation?
		/// </summary>
		public override bool SupportsIdentityColumns => true;

		/// <summary> 
		/// Get the select command to use to retrieve the last generated IDENTITY
		/// value for a particular table 
		/// </summary>
		/// <param name="tableName">The table into which the insert was done </param>
		/// <param name="identityColumn">The PK column. </param>
		/// <param name="type">The <see cref="DbType"/> type code. </param>
		/// <returns> The appropriate select command </returns>
		public override string GetIdentitySelectString(string identityColumn, string tableName, DbType type)
		{
			return IdentitySelectString + tableName;
		}

		/// <summary> 
		/// Get the select command to use to retrieve the last generated IDENTITY value.
		/// </summary>
		/// <returns> The appropriate select command </returns>
		public override string IdentitySelectString
		{
			get { return "select current_identity_value() from "; }
		}

		/// <summary>
		/// The keyword used to specify an identity column, if native key generation is supported
		/// </summary>
		public override string IdentityColumnString
		{
			get { return "generated by default as identity"; }
		}

		#endregion

		#region SEQUENCE support

		/// <summary>
		/// Does this dialect support sequences?
		/// </summary>
		public override bool SupportsSequences => true;

		/// <summary> 
		/// Does this dialect support "pooled" sequences?
		/// </summary>
		/// <returns> True if such "pooled" sequences are supported; false otherwise. </returns>
		/// <remarks>
		/// A pooled sequence is one that has a configurable initial size and increment 
		/// size. It enables NHibernate to be allocated a pool/block/range of IDs,
		/// which can reduce the frequency of round trips to the database during ID
		/// generation.
		/// </remarks>
		/// <seealso cref="Dialect.GetCreateSequenceStrings(string, int, int)"> </seealso>
		/// <seealso cref="GetCreateSequenceString(string, int, int)"> </seealso>
		public override bool SupportsPooledSequences => true;

		/// <summary> 
		/// Generate the appropriate select statement to to retreive the next value
		/// of a sequence.
		/// </summary>
		/// <param name="sequenceName">the name of the sequence </param>
		/// <returns> String The "nextval" select string. </returns>
		/// <remarks>This should be a "stand alone" select statement.</remarks>
		public override string GetSequenceNextValString(string sequenceName)
		{
			return "select " + GetSelectSequenceNextValString(sequenceName) + " from dummy";
		}

		/// <summary> 
		/// Typically dialects which support sequences can drop a sequence
		/// with a single command.  
		/// </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <returns> The sequence drop commands </returns>
		/// <remarks>
		/// This is convenience form of <see cref="Dialect.GetDropSequenceStrings"/>
		/// to help facilitate that.
		/// 
		/// Dialects which support sequences and can drop a sequence in a
		/// single command need *only* override this method.  Dialects
		/// which support sequences but require multiple commands to drop
		/// a sequence should instead override <see cref="Dialect.GetDropSequenceStrings"/>. 
		/// </remarks>
		public override string GetDropSequenceString(string sequenceName)
		{
			return "drop sequence " + sequenceName;
		}

		/// <summary> 
		/// Generate the select expression fragment that will retrieve the next
		/// value of a sequence as part of another (typically DML) statement.
		/// </summary>
		/// <param name="sequenceName">the name of the sequence </param>
		/// <returns> The "nextval" fragment. </returns>
		/// <remarks>
		/// This differs from <see cref="GetSequenceNextValString"/> in that this
		/// should return an expression usable within another statement.
		/// </remarks>
		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return sequenceName + ".nextval";
		}

		/// <summary> 
		/// Typically dialects which support sequences can create a sequence
		/// with a single command.
		/// </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <returns> The sequence creation command </returns>
		/// <remarks>
		/// This is convenience form of <see cref="Dialect.GetCreateSequenceStrings(string,int,int)"/> to help facilitate that.
		/// Dialects which support sequences and can create a sequence in a
		/// single command need *only* override this method.  Dialects
		/// which support sequences but require multiple commands to create
		/// a sequence should instead override <see cref="Dialect.GetCreateSequenceStrings(string,int,int)"/>.
		/// </remarks>
		public override string GetCreateSequenceString(string sequenceName)
		{
			return "create sequence " + sequenceName;
		}

		/// <summary> 
		/// Overloaded form of <see cref="GetCreateSequenceString(string)"/>, additionally
		/// taking the initial value and increment size to be applied to the sequence
		/// definition.
		///  </summary>
		/// <param name="sequenceName">The name of the sequence </param>
		/// <param name="initialValue">The initial value to apply to 'create sequence' statement </param>
		/// <param name="incrementSize">The increment value to apply to 'create sequence' statement </param>
		/// <returns> The sequence creation command </returns>
		/// <remarks>
		/// The default definition is to suffix <see cref="GetCreateSequenceString(string,int,int)"/>
		/// with the string: " start with {initialValue} increment by {incrementSize}" where
		/// {initialValue} and {incrementSize} are replacement placeholders.  Generally
		/// dialects should only need to override this method if different key phrases
		/// are used to apply the allocation information.
		/// </remarks>
		protected override string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
		{
			if (incrementSize == 0)
			{
				throw new MappingException("Unable to create the sequence [" + sequenceName + "]: the increment size must not be 0");
			}

			String createSequenceString = GetCreateSequenceString(sequenceName) + " start with " + initialValue + " increment by " + incrementSize;
			if (incrementSize > 0)
			{
				if (initialValue < 1)
				{
					// default minvalue for an ascending sequence is 1
					createSequenceString += " minvalue " + initialValue;
				}
			}
			else if (incrementSize < 0)
			{
				if (initialValue > -1)
				{
					// default maxvalue for a descending sequence is -1
					createSequenceString += " maxvalue " + initialValue;
				}
			}
			return createSequenceString;
		}

		/// <summary> Get the select command used retrieve the names of all sequences.</summary>
		/// <returns> The select command; or null if sequences are not supported. </returns>
		public override string QuerySequencesString
		{
			get { return "select sequence_name from sys.sequences"; }
		}

		#endregion

		#endregion

		#region Miscellaneous support

		/// <summary> The SQL literal value to which this database maps boolean values. </summary>
		/// <param name="value">The boolean value </param>
		/// <returns> The appropriate SQL literal. </returns>
		public override string ToBooleanValueString(bool value)
		{
			return value ? "true" : "false";
		}

		/// <summary>
		/// Does this dialect support concurrent writing connections in the same transaction?
		/// </summary>
		public override bool SupportsConcurrentWritingConnectionsInSameTransaction => false;

		#endregion

		#region Limit/offset support

		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>False, unless overridden.</value>
		public override bool SupportsLimit => true;

		/// <summary>
		/// Attempts to add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>.
		/// Expects any database-specific offset and limit adjustments to have already been performed (ex. UseMaxForLimit, OffsetStartsAtOne).
		/// </summary>
		/// <param name="queryString">The <see cref="SqlString"/> to base the limit query off.</param>
		/// <param name="offset">Offset of the first row to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no limit is requested.  This should have already been adjusted to account for OffsetStartsAtOne.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no offset is requested.  This should have already been adjusted to account for UseMaxForLimit.</param>
		/// <returns>A new <see cref="SqlString"/> that contains the <c>LIMIT</c> clause. Returns <c>null</c> 
		/// if <paramref name="queryString"/> represents a SQL statement to which a limit clause cannot be added, 
		/// for example when the query string is custom SQL invoking a stored procedure.</returns>
		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			if (offset == null && limit == null)
			{
				return queryString;
			}

			var limitBuilder = new SqlStringBuilder(queryString);
			limitBuilder.Add(" limit ");

			if (limit == null)
			{
				limitBuilder.Add(uint.MaxValue.ToString());
			}
			else
			{
				limitBuilder.Add(limit);
			}

			if (offset != null)
			{
				limitBuilder.Add(" offset ");
				limitBuilder.Add(offset);
			}

			return limitBuilder.ToSqlString();
		}

		#endregion

		#region Identifier quoting support

		#endregion

		#region Union subclass support

		/// <summary> 
		/// Given a <see cref="DbType"/> type code, determine an appropriate
		/// null value to use in a select clause.
		/// </summary>
		/// <param name="sqlType">The <see cref="DbType"/> type code. </param>
		/// <returns> The appropriate select clause value fragment. </returns>
		/// <remarks>
		/// One thing to consider here is that certain databases might
		/// require proper casting for the nulls here since the select here
		/// will be part of a UNION/UNION ALL.
		/// </remarks>
		public override string GetSelectClauseNullString(SqlType sqlType)
		{
			switch (sqlType.DbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.Guid:
					return "to_varchar(null)";
				case DbType.Binary:
					return "to_binary(null)";
				case DbType.Boolean:
					return "cast(null as boolean)";
				case DbType.Byte:
					return "to_tinyint(null)";
				case DbType.Currency:
				case DbType.Decimal:
					return "to_decimal(null)";
				case DbType.Date:
					return "to_date(null)";
				case DbType.DateTime:
					return "to_timestamp(null)";
				case DbType.Double:
					return "to_double(null)";
				case DbType.Int16:
				case DbType.SByte:
					return "to_smallint(null)";
				case DbType.Int32:
					return "to_integer(null)";
				case DbType.Int64:
					return "to_bigint(null)";
				case DbType.Single:
					return "to_real(null)";
				case DbType.String:
				case DbType.StringFixedLength:
					return "to_nvarchar(null)";
				case DbType.Time:
					return "to_time(null)";
			}
			return "null";
		}

		/// <summary> 
		/// Does this dialect support UNION ALL, which is generally a faster variant of UNION? 
		/// True if UNION ALL is supported; false otherwise.
		/// </summary>
		public override bool SupportsUnionAll => true;

		#endregion

		#region Informational metadata

		/// <summary> 
		/// Does this dialect support empty IN lists?
		/// For example, is [where XYZ in ()] a supported construct?
		/// </summary>
		/// <returns> True if empty in lists are supported; false otherwise. </returns>
		public override bool SupportsEmptyInList => false;

		/// <summary> 
		/// Is this dialect known to support what ANSI-SQL terms "row value
		/// constructor" syntax; sometimes called tuple syntax.
		/// <p/>
		/// Basically, does it support syntax like
		/// "... where (FIRST_NAME, LAST_NAME) = ('Steve', 'Ebersole') ...". 
		/// </summary>
		/// <returns> 
		/// True if this SQL dialect is known to support "row value
		/// constructor" syntax; false otherwise.
		/// </returns>
		public override bool SupportsRowValueConstructorSyntax => true;

		/// <summary> 
		/// If the dialect supports {@link #supportsRowValueConstructorSyntax() row values},
		/// does it offer such support in IN lists as well?
		/// <p/>
		/// For example, "... where (FIRST_NAME, LAST_NAME) IN ( (?, ?), (?, ?) ) ..." 
		/// </summary>
		/// <returns> 
		/// True if this SQL dialect is known to support "row value
		/// constructor" syntax in the IN list; false otherwise.
		/// </returns>
		public override bool SupportsRowValueConstructorSyntaxInInList => true;

		/// <summary> 
		/// Does this dialect support definition of cascade delete constraints
		/// which can cause circular chains? 
		/// </summary>
		/// <returns> True if circular cascade delete constraints are supported; false otherwise. </returns>
		public override bool SupportsCircularCascadeDeleteConstraints => false;

		/// <summary> 
		/// Expected LOB usage pattern is such that I can perform an insert
		/// via prepared statement with a parameter binding for a LOB value
		/// without crazy casting to JDBC driver implementation-specific classes...
		/// <p/>
		/// Part of the trickiness here is the fact that this is largely
		/// driver dependent.  For example, Oracle (which is notoriously bad with
		/// LOB support in their drivers historically) actually does a pretty good
		/// job with LOB support as of the 10.2.x versions of their drivers... 
		/// </summary>
		/// <returns> 
		/// True if normal LOB usage patterns can be used with this driver;
		/// false if driver-specific hookiness needs to be applied.
		/// </returns>
		public override bool SupportsExpectedLobUsagePattern => false;

		/// <summary> 
		/// Is it supported to materialize a LOB locator outside the transaction in
		/// which it was created?
		/// <p/>
		/// Again, part of the trickiness here is the fact that this is largely
		/// driver dependent.
		/// <p/>
		/// NOTE: all database I have tested which {@link #supportsExpectedLobUsagePattern()}
		/// also support the ability to materialize a LOB outside the owning transaction... 
		/// </summary>
		/// <returns> True if unbounded materialization is supported; false otherwise. </returns>
		public override bool SupportsUnboundedLobLocatorMaterialization => false;

		/// <summary> Does the dialect support an exists statement in the select clause? </summary>
		/// <returns> True if exists checks are allowed in the select clause; false otherwise. </returns>
		public override bool SupportsExistsInSelect => false;

		/// <summary>
		/// Does this dialect support scalar sub-selects?
		/// </summary>
		/// <remarks>
		/// Scalar sub-selects are sub-queries returning a scalar value, not a set. See https://stackoverflow.com/a/648049/1178314
		/// </remarks>
		public override bool SupportsScalarSubSelects => false;

		#endregion


		/// <summary> 
		/// Get the command used to select a GUID from the underlying database.
		/// (Optional operation.)
		///  </summary>
		/// <returns> The appropriate command. </returns>
		public override string SelectGUIDString
		{
			get { return "select sysuuid from dummy"; }
		}

		/// <summary> 
		/// Should the value returned by <see cref="CurrentTimestampSelectString"/>
		/// be treated as callable.  Typically this indicates that JDBC escape
		/// syntax is being used...
		/// </summary>
		public override bool IsCurrentTimestampSelectStringCallable => false;

		/// <summary> 
		/// Retrieve the command used to retrieve the current timestammp from the database. 
		/// </summary>
		public override string CurrentTimestampSelectString
		{
			get { return "select current_timestamp from dummy"; }
		}

		/// <summary>
		/// The keyword used to insert a row without specifying any column values
		/// </summary>
		public override string NoColumnsInsertString
		{
			get { throw new MappingException("HANA does not support inserting a row without specifying any column values"); }
		}

		// 18 is the smallest of all dialects we handle.
		/// <summary>
		/// The maximum length a SQL alias can have.
		/// </summary>
		public override int MaxAliasLength => 128;

		/// <summary>
		/// The syntax used to add a column to a table. Note this is deprecated
		/// </summary>
		public override string AddColumnString
		{
			get { return "add ("; }
		}

		/// <summary>
		/// The syntax for the suffix used to add a column to a table. Note this is deprecated
		/// </summary>
		public override string AddColumnSuffixString
		{
			get { return ")"; }
		}
	}
}
