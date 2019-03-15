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

namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect base class for SAP HANA
	/// </summary>
	public abstract class HanaDialectBase : Dialect
	{
		[Serializable]
		private class TypeConvertingVarArgsSQLFunction : ISQLFunction
		{
			private readonly string _begin;
			private readonly string _sep;
			private readonly string _end;
			private SqlType _type;

			public TypeConvertingVarArgsSQLFunction(string begin, string sep, string end)
			{
				_begin = begin;
				_sep = sep;
				_end = end;
			}

			#region ISQLFunction Members

			public IType ReturnType(IType columnType, IMapping mapping)
			{
				_type = columnType.SqlTypes(mapping)[0];
				return columnType;
			}

			public bool HasArguments => true;

			public bool HasParenthesesIfNoArguments => true;

			public SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				var buf = new SqlStringBuilder().Add(_begin);
				for (var i = 0; i < args.Count; i++)
				{
					var arg = args[i];
					if (arg is SqlString str && str.EqualsCaseInsensitive("?"))
					{
						buf.Add("cast(");
						buf.AddObject(arg);
						buf.Add(" as ");
						buf.Add(factory.Dialect.GetCastTypeName(_type));
						buf.Add(")");
					}
					else
					{
						buf.AddObject(arg);
					}
					if (i < args.Count - 1) buf.Add(_sep);
				}
				return buf.Add(_end).ToSqlString();
			}

			#endregion
		}

		protected HanaDialectBase()
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
			RegisterFunction("band", new Function.BitwiseFunctionOperation("bitand"));
			RegisterFunction("bor", new Function.BitwiseFunctionOperation("bitor"));
			RegisterFunction("bxor", new Function.BitwiseFunctionOperation("bitxor"));
			RegisterFunction("bnot", new Function.BitwiseFunctionOperation("bitnot"));
			RegisterFunction("bit_length", new SQLFunctionTemplate(NHibernateUtil.Int32, "length(to_binary(?1))*8"));
			RegisterFunction("ceiling", new StandardSQLFunction("ceil"));
			RegisterFunction("chr", new StandardSQLFunction("char", NHibernateUtil.AnsiChar));
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "to_date(?1)"));
			RegisterFunction("iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));
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

		/// <inheritdoc />
		public override bool DropConstraints => false;

		/// <inheritdoc />
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

		/// <inheritdoc />
		public override bool SupportsColumnCheck => false;

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new HanaDataBaseSchema(connection);
		}

		#endregion

		#region Lock acquisition support

		/// <inheritdoc />
		public override bool UsesColumnsWithForUpdateOf => true;

		/// <inheritdoc />
		public override string GetForUpdateString(string aliases)
		{
			return ForUpdateString + " of " + aliases;
		}

		/// <inheritdoc />
		public override string ForUpdateNowaitString
		{
			get { return ForUpdateString + " nowait"; }
		}

		/// <inheritdoc />
		public override string GetForUpdateNowaitString(string aliases)
		{
			return GetForUpdateString(aliases) + " nowait";
		}

		#endregion

		#region Table support

		#region Temporary table support

		/// <inheritdoc />
		public override bool SupportsTemporaryTables => true;

		/// <inheritdoc />
		public override string GenerateTemporaryTableName(string baseTableName)
		{
			return "#HT_" + baseTableName;
		}

		/// <inheritdoc />
		public override bool? PerformTemporaryTableDDLInIsolation()
		{
			return false;
		}

		#endregion

		#endregion

		#region Callable statement support

		/// <inheritdoc />
		public override int RegisterResultSetOutParameter(DbCommand statement, int position)
		{
			// Result set (TABLE) OUT parameters don't need to be registered
			return position;
		}

		#endregion

		#region Current timestamp support

		/// <inheritdoc />
		public override bool SupportsCurrentTimestampSelection => true;

		/// <inheritdoc />
		public override bool SupportsCurrentUtcTimestampSelection => true;

		/// <inheritdoc />
		public override long TimestampResolutionInTicks
			// According to https://help.sap.com/viewer/4fe29514fd584807ac9f2a04f6754767/2.0.02/en-US/3f81ccc7e35d44cbbc595c7d552c202a.html,
			// it is supposed to have a 7 digits fractional second precision, but tests show only a 6 digits one. This is maybe a
			// limitation of the data provider.
			=> 10L;

		#endregion

		#region Constraint support

		/// <inheritdoc />
		public override string CascadeConstraintsString
		{
			get { return " cascade"; }
		}

		#endregion

		#region Native identifier generation

		#region IDENTITY support

		/// <inheritdoc />
		public override bool SupportsIdentityColumns => true;

		/// <inheritdoc />
		public override string GetIdentitySelectString(string identityColumn, string tableName, DbType type)
		{
			return IdentitySelectString + tableName;
		}

		/// <inheritdoc />
		public override string IdentitySelectString
		{
			get { return "select current_identity_value() from "; }
		}

		/// <inheritdoc />
		public override string IdentityColumnString
		{
			get { return "generated by default as identity"; }
		}

		#endregion

		#region SEQUENCE support

		/// <inheritdoc />
		public override bool SupportsSequences => true;

		/// <inheritdoc />
		public override bool SupportsPooledSequences => true;

		/// <inheritdoc />
		public override string GetSequenceNextValString(string sequenceName)
		{
			// See https://help.sap.com/viewer/4fe29514fd584807ac9f2a04f6754767/2.0.02/en-US/20d509277519101489029c064d468c5d.html,
			// this seems to be the recommended way of querying a sequence.
			// SYS.DUMMY is a system table having normally one row. If someone has fiddled with it, this will cause failures...
			return "select " + GetSelectSequenceNextValString(sequenceName) + " from sys.dummy";
		}

		/// <inheritdoc />
		public override string GetDropSequenceString(string sequenceName)
		{
			return "drop sequence " + sequenceName;
		}

		/// <inheritdoc />
		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return sequenceName + ".nextval";
		}

		/// <inheritdoc />
		public override string GetCreateSequenceString(string sequenceName)
		{
			return "create sequence " + sequenceName;
		}

		/// <inheritdoc />
		protected override string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
		{
			if (incrementSize == 0)
			{
				throw new MappingException("Unable to create the sequence [" + sequenceName + "]: the increment size must not be 0");
			}

			var createSequenceString = GetCreateSequenceString(sequenceName) + " start with " + initialValue + " increment by " + incrementSize;
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

		/// <inheritdoc />
		public override string QuerySequencesString
		{
			get { return "select sequence_name from sys.sequences"; }
		}

		#endregion

		#endregion

		#region Miscellaneous support

		/// <inheritdoc />
		public override string ToBooleanValueString(bool value)
		{
			return value ? "true" : "false";
		}

		#endregion

		#region Limit/offset support

		/// <inheritdoc />
		public override bool SupportsLimit => true;

		/// <inheritdoc />
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

		#region Union subclass support

		/// <inheritdoc />
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

		/// <inheritdoc />
		public override bool SupportsUnionAll => true;

		#endregion

		#region Informational metadata

		/// <inheritdoc />
		public override bool SupportsEmptyInList => false;

		/// <inheritdoc />
		public override bool SupportsRowValueConstructorSyntax => false; // HANA supports this, but setting it to true causes the tests NH2294 and NH2394 to fail (see issue #1676)

		/// <inheritdoc />
		public override bool SupportsRowValueConstructorSyntaxInInList => true;

		/// <inheritdoc />
		public override bool SupportsExpectedLobUsagePattern => false;

		/// <inheritdoc />
		public override bool SupportsUnboundedLobLocatorMaterialization => false;

		/// <inheritdoc />
		public override bool SupportsExistsInSelect => false;

		#endregion

		/// <inheritdoc />
		public override string SelectGUIDString
			// SYS.DUMMY is a system table having normally one row. If someone has fiddled with it, this will cause failures...
			=> "select sysuuid from sys.dummy";

		/// <inheritdoc />
		public override bool IsCurrentTimestampSelectStringCallable => false;

		/// <inheritdoc />
		public override string CurrentTimestampSelectString
			// SYS.DUMMY is a system table having normally one row. If someone has fiddled with it, this will cause failures...
			=> "select current_timestamp from sys.dummy";

		/// <inheritdoc />
		public override string CurrentUtcTimestampSelectString
			// SYS.DUMMY is a system table having normally one row. If someone has fiddled with it, this will cause failures...
			=> $"select {CurrentUtcTimestampSQLFunctionName} from sys.dummy";

		/// <inheritdoc />
		public override string CurrentUtcTimestampSQLFunctionName => "current_utctimestamp";

		/// <inheritdoc />
		public override int MaxAliasLength => 128;

		/// <inheritdoc />
		public override string AddColumnString => "add (";

		/// <inheritdoc />
		public override string AddColumnSuffixString => ")";
	}
}
