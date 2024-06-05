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
			RegisterFunction("right", new StandardSQLFunction("left", NHibernateUtil.String));

			RegisterFunction("mod", new ModulusFunction(true, false));

			RegisterFunction("substring", new StandardSQLFunction("substr", NHibernateUtil.String));

			// Bitwise operations
			RegisterFunction("band", new Function.BitwiseFunctionOperation("bitand"));
			RegisterFunction("bor", new Function.BitwiseFunctionOperation("bitor"));
			RegisterFunction("bxor", new Function.BitwiseFunctionOperation("bitxor"));
			RegisterFunction("bnot", new Function.BitwiseFunctionOperation("bitnot"));

			RegisterKeywords();

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.DB2Driver";
		}
		
		#region private static readonly string[] DialectKeywords = { ... }

		private static readonly string[] DialectKeywords =
		{
			"aao",
			"abs",
			"absolute",
			"access",
			"access_method",
			"according",
			"account",
			"acctng",
			"acos",
			"action",
			"activate",
			"active",
			"add_months",
			"address",
			"admin",
			"after",
			"aggregate",
			"alias",
			"alignment",
			"all_rows",
			"allow",
			"ansi",
			"append",
			"applname",
			"array_agg",
			"asc",
			"ascii",
			"asin",
			"associate",
			"asutime",
			"atan",
			"atan2",
			"attach",
			"attributes",
			"audit",
			"authentication",
			"authid",
			"authorized",
			"auto",
			"auto_readahead",
			"auto_reprepare",
			"auto_stat_mode",
			"autofree",
			"aux",
			"auxiliary",
			"avg",
			"avoid_execute",
			"avoid_fact",
			"avoid_full",
			"avoid_hash",
			"avoid_index",
			"avoid_index_sj",
			"avoid_multi_index",
			"avoid_nl",
			"avoid_star_join",
			"avoid_subqf",
			"bargroup",
			"based",
			"before",
			"bigserial",
			"bind",
			"bit",
			"bitand",
			"bitandnot",
			"bitnot",
			"bitor",
			"bitxor",
			"blobdir",
			"boolean",
			"bound_impl_pdq",
			"buckets",
			"buffered",
			"bufferpool",
			"builtin",
			"byte",
			"cache",
			"cannothash",
			"capture",
			"cardinality",
			"cascade",
			"ccsid",
			"ceil",
			"ceiling",
			"char_length",
			"character_length",
			"charindexcheck",
			"cl",
			"class",
			"class_origin",
			"client",
			"clobdir",
			"clone",
			"cluster",
			"clustersize",
			"coalesce",
			"cobol",
			"codeset",
			"collation",
			"collect",
			"collection",
			"collid",
			"columns",
			"comment",
			"committed",
			"commutator",
			"compact",
			"component",
			"components",
			"compress",
			"concat",
			"concurrent",
			"connect_by_iscycle",
			"connect_by_isleaf",
			"connect_by_root",
			"connect_by_rootconst",
			"connection",
			"connection_name",
			"constraints",
			"constructor",
			"contains",
			"content",
			"context",
			"convert",
			"copy",
			"corr",
			"cos",
			"costfunc",
			"count",
			"count_big",
			"covar_pop",
			"covar_samp",
			"crcols",
			"cume_dist",
			"current_default_transform_group",
			"current_lc_ctype",
			"current_schema",
			"current_server",
			"current_timezone",
			"current_transform_group_for_type",
			"currval",
			"data",
			"database",
			"datafiles",
			"datapartitionname",
			"datapartitionnum",
			"dataskip",
			"datetime",
			"days",
			"db2general",
			"db2genrl",
			"db2sql",
			"dba",
			"dbclob",
			"dbdate",
			"dbinfo",
			"dbpartitionname",
			"dbpartitionnum",
			"dbpassword",
			"dbsa",
			"dbsecadm",
			"dbservername",
			"dbsso",
			"deactivate",
			"debug",
			"debug_env",
			"debugmode",
			"dec_t",
			"decfloat",
			"decode",
			"decrypt_binary",
			"decrypt_char",
			"default_role",
			"default_user",
			"defaults",
			"defer",
			"deferred",
			"deferred_prepare",
			"define",
			"definition",
			"degreesdelay",
			"deleting",
			"delimited",
			"delimiter",
			"deluxe",
			"dense_rank",
			"denserank",
			"desc",
			"descriptor",
			"detach",
			"diagnostics",
			"directives",
			"dirty",
			"disable",
			"disabled",
			"disallow",
			"disk",
			"distributebinary",
			"distributesreferences",
			"distributions",
			"document",
			"domain",
			"donotdistribute",
			"dormant",
			"dssize",
			"dtime_t",
			"editproc",
			"elif",
			"elseif",
			"enable",
			"enabled",
			"encoding",
			"encrypt_aes",
			"encrypt_tdes",
			"encryption",
			"end-exec",
			"end-exec2",
			"ending",
			"enforced",
			"enum",
			"environment",
			"erase",
			"erkey",
			"error",
			"every",
			"exception",
			"excluding",
			"exclusive",
			"executeanywhere",
			"exemption",
			"exp",
			"explain",
			"explicit",
			"express",
			"expression",
			"extdirectives",
			"extend",
			"extended",
			"extent",
			"extract",
			"fact",
			"far",
			"fenced",
			"fieldproc",
			"file",
			"filetoblob",
			"filetoclob",
			"fillfactor",
			"filtering",
			"final",
			"first",
			"first_rows",
			"fixchar",
			"fixed",
			"floor",
			"flush",
			"force",
			"force_ddl_exec",
			"forced",
			"foreach",
			"format",
			"format_units",
			"fortran",
			"found",
			"fraction",
			"fragment",
			"fragments",
			"freepage",
			"fusion",
			"gbpcache",
			"general",
			"generated",
			"gethint",
			"go",
			"goto",
			"graphic",
			"greaterthan",
			"greaterthanorequal",
			"handlesnulls",
			"hash",
			"hashed_value",
			"hdr",
			"hex",
			"high",
			"hint",
			"home",
			"hours",
			"id",
			"idslbacreadarray",
			"idslbacreadset",
			"idslbacreadtree",
			"idslbacrules",
			"idslbacwritearray",
			"idslbacwriteset",
			"idslbacwritetree",
			"idssecuritylabel",
			"ifx_auto_reprepare",
			"ifx_batchedread_table",
			"ifx_int8_t",
			"ifx_lo_create_spec_t",
			"ifx_lo_stat_t",
			"ignore",
			"implicit",
			"implicit_pdq",
			"implicitly",
			"inactive",
			"include",
			"including",
			"inclusive",
			"increment",
			"index",
			"index_all",
			"index_sj",
			"indexbp",
			"indexes",
			"indicators",
			"inf",
			"infinity",
			"informix",
			"inherit",
			"init",
			"initcap",
			"inline",
			"inserting",
			"instead",
			"instrint",
			"int8",
			"integ",
			"integrity",
			"internal",
			"internallength",
			"intersection",
			"intrvl_t",
			"iscanonical",
			"isobid",
			"isolation",
			"item",
			"iterator",
			"jar",
			"java",
			"keep",
			"key",
			"label",
			"labeleq",
			"labelge",
			"labelglb",
			"labelgt",
			"labelle",
			"labellt",
			"labellub",
			"labeltostring",
			"last",
			"last_day",
			"lc_ctype",
			"lenlength",
			"lessthan",
			"lessthanorequal",
			"let",
			"level",
			"level2",
			"limit",
			"linktype",
			"list",
			"listing",
			"ln",
			"load",
			"loc_t",
			"localdate",
			"locale",
			"location",
			"locator",
			"locators",
			"lock",
			"lockmax",
			"locks",
			"locksize",
			"locopy",
			"log",
			"log10",
			"logged",
			"logn",
			"long",
			"lotofile",
			"low",
			"lower",
			"lpad",
			"ltrim",
			"lvarchar",
			"maintained",
			"matched",
			"matches",
			"materialized",
			"max",
			"maxerrors",
			"maxlen",
			"maxvalue",
			"mdy",
			"median",
			"medium",
			"memory",
			"memory_resident",
			"message_length",
			"message_text",
			"microsecond",
			"microseconds",
			"middle",
			"min",
			"minpctused",
			"minutes",
			"minvalue",
			"mixed",
			"mod",
			"mode",
			"moderate",
			"modify",
			"money",
			"months",
			"months_between",
			"mounting",
			"multi_index",
			"name",
			"namespace",
			"nan",
			"negator",
			"new_table",
			"next",
			"next_day",
			"nextval",
			"nlscase",
			"nocache",
			"nocycle",
			"nodename",
			"nodenumber",
			"nomaxvalue",
			"nomigrate",
			"nominvalue",
			"non_dim",
			"non_resident",
			"noorder",
			"normal",
			"normalize",
			"normalized",
			"notemplatearg",
			"notequal",
			"nullif",
			"nulls",
			"numparts",
			"numrows",
			"numtodsinterval",
			"numtoyminterval",
			"nvarchar",
			"nvl",
			"obid",
			"octet_length",
			"off",
			"offset",
			"old_table",
			"online",
			"opaque",
			"opclass",
			"optcompind",
			"optical",
			"optimization",
			"optimize",
			"option",
			"ordered",
			"ordinality",
			"organization1",
			"organize",
			"overlay",
			"override",
			"overriding",
			"package",
			"padded",
			"page",
			"pagesize",
			"parallelizable",
			"part",
			"partitioned",
			"partitioning",
			"partitions",
			"pascal",
			"passedbyvalue",
			"password",
			"path",
			"pctfree",
			"pdqpriority",
			"percall_cost",
			"percent_rank",
			"percentile_cont",
			"percentile_disc",
			"period1",
			"piecesize",
			"pipe",
			"plan",
			"pli",
			"pload",
			"policy",
			"position",
			"pow",
			"power",
			"previous",
			"prevval",
			"prior",
			"priqty",
			"private",
			"privileges",
			"program",
			"programid",
			"properties",
			"psid",
			"public",
			"put",
			"query",
			"queryno",
			"radiansraise",
			"rank",
			"raw",
			"rcdfmt",
			"read",
			"recordend",
			"recovery",
			"refresh",
			"register",
			"regr_avgx",
			"regr_avgy",
			"regr_count",
			"regr_intercept",
			"regr_r2",
			"regr_slope",
			"regr_sxx",
			"regr_sxy",
			"regr_syy",
			"rejectfile",
			"relative",
			"remainder",
			"rename",
			"reoptimization",
			"repeatable",
			"replace",
			"replication",
			"reserve",
			"reset",
			"resolution",
			"resource",
			"restart",
			"restrict",
			"result_set_locator",
			"resume",
			"retain",
			"retainupdatelocks",
			"returned_sqlstate",
			"returning",
			"reuse",
			"reverserevoke",
			"rid",
			"robin",
			"role",
			"rollforward",
			"root",
			"round",
			"round_ceiling",
			"round_down",
			"round_floor",
			"round_half_down",
			"round_half_even",
			"round_half_up",
			"round_up",
			"routine",
			"row_count",
			"row_number",
			"rowid",
			"rowids",
			"rownumber",
			"rowset",
			"rpad",
			"rrn",
			"rtrim",
			"rule",
			"run",
			"sameas",
			"samples",
			"sampling",
			"save",
			"sbcs",
			"schema",
			"scope",
			"scratchpad",
			"seclabel_by_comp",
			"seclabel_by_name",
			"seclabel_to_char",
			"secondary",
			"seconds",
			"secqty",
			"section",
			"secured",
			"security",
			"selconst",
			"selecting",
			"selfunc",
			"selfuncargs",
			"sequence",
			"serial",
			"serial8",
			"serializable",
			"server_name",
			"serveruuid",
			"session",
			"session_user",
			"setsessionauth",
			"share",
			"short",
			"siblings",
			"signed",
			"simple",
			"sin",
			"sitename",
			"size",
			"skall",
			"skinhibit",
			"skip",
			"skshow",
			"smallfloat",
			"snan",
			"source",
			"spacespecific",
			"sqlcode",
			"sqlcontext",
			"sqlerror",
			"sqlid",
			"sqrt",
			"stability",
			"stack",
			"stacked",
			"standard",
			"star_join",
			"starting",
			"statchange",
			"statement",
			"statistics",
			"statlevel",
			"statment",
			"status",
			"stay",
			"stddev_pop",
			"stddev_samp",
			"stdev",
			"step",
			"stogroup",
			"stop",
			"storage",
			"store",
			"stores",
			"strategies",
			"string",
			"stringtolabel",
			"struct",
			"style",
			"subclass_origin",
			"substr",
			"substrinf_indexsum",
			"substring",
			"sum",
			"summary",
			"support",
			"sync",
			"synonym",
			"sys_connect_by_path",
			"sysdate",
			"sysdbclose",
			"sysdbopen",
			"sysfun",
			"sysibm",
			"sysproc",
			"tables",
			"tablespace",
			"tablespaces",
			"tan",
			"task",
			"temp",
			"template",
			"test",
			"text",
			"threadsafe",
			"timeout",
			"to_char",
			"to_date",
			"to_dsinterval",
			"to_number",
			"to_yminterval",
			"today",
			"trace",
			"transaction",
			"transition",
			"translate",
			"tree",
			"triggers",
			"trim",
			"trim_array",
			"trunc",
			"truncate",
			"trusted",
			"type",
			"typedef",
			"typeid",
			"typename",
			"typeof",
			"uescape",
			"uid",
			"uncommitted",
			"under",
			"unit",
			"units",
			"unload",
			"unlock",
			"unsigned",
			"updating",
			"upon",
			"upper",
			"uri",
			"usage",
			"use",
			"use_hash",
			"use_nl",
			"use_subqf",
			"uselastcommitted",
			"userid",
			"ustlow_sample",
			"validproc",
			"var",
			"var_pop",
			"var_samp",
			"vargraphic",
			"variable",
			"variance",
			"variant",
			"vcat",
			"vercols",
			"version",
			"view",
			"violations",
			"void",
			"volatile",
			"volumes",
			"wait",
			"warning",
			"weekday",
			"width_bucket",
			"wlm",
			"work",
			"write",
			"writedown",
			"writeup",
			"wrkstnname",
			"xadatasource",
			"xid",
			"xload",
			"xml",
			"xmlagg",
			"xmlattributes",
			"xmlcast",
			"xmlcomment",
			"xmlconcat",
			"xmldocument",
			"xmlelement",
			"xmlexists",
			"xmlforest",
			"xmlgroup",
			"xmlnamespaces",
			"xmlparse",
			"xmlpi",
			"xmlrow",
			"xmlserialize",
			"xmltext",
			"xmlvalidate",
			"xsltransform",
			"xsrobject",
			"xunload",
			"years",
			"yes",
			"zone1",
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
	}
}
