using System;
using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// SQL Dialect for SQL Anywhere 10 - for the NHibernate 3.0.0 distribution
	/// Copyright (C) 2010 Glenn Paulley
	/// Contact: http://iablog.sybase.com/paulley
	///
	/// This NHibernate dialect should be considered BETA software.
	///
	/// This library is free software; you can redistribute it and/or
	/// modify it under the terms of the GNU Lesser General Public
	/// License as published by the Free Software Foundation; either
	/// version 2.1 of the License, or (at your option) any later version.
	///
	/// This library is distributed in the hope that it will be useful,
	/// but WITHOUT ANY WARRANTY; without even the implied warranty of
	/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	/// Lesser General Public License for more details.
	///
	/// You should have received a copy of the GNU Lesser General Public
	/// License along with this library; if not, write to the Free Software
	/// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	/// </summary>
	/// <remarks>
	/// The dialect defaults the following configuration properties:
	/// <list type="table">
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.SybaseSQLAnywhereDriver" /></description>
	///	</item>
	///	<item>
	///		<term>prepare_sql</term>
	///		<description><see langword="false" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public partial class SybaseSQLAnywhere10Dialect : Dialect
	{
		public SybaseSQLAnywhere10Dialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SybaseSQLAnywhereDriver";
			DefaultProperties[Environment.PrepareSql] = "false";

			RegisterCharacterTypeMappings();
			RegisterNumericTypeMappings();
			RegisterDateTimeTypeMappings();
			RegisterReverseNHibernateTypeMappings();
			RegisterFunctions();
			RegisterKeywords();
		}

		protected virtual void RegisterCharacterTypeMappings()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(1)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 32767, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(1)");
			RegisterColumnType(DbType.AnsiString, 32767, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "LONG VARCHAR");
			RegisterColumnType(DbType.StringFixedLength, "NCHAR(1)");
			RegisterColumnType(DbType.StringFixedLength, 32767, "NCHAR($l)");
			RegisterColumnType(DbType.String, "NVARCHAR(1)");
			RegisterColumnType(DbType.String, 32767, "NVARCHAR($l)");
			RegisterColumnType(DbType.String, 2147483647, "LONG NVARCHAR");
			RegisterColumnType(DbType.Binary, "BINARY(1)");
			RegisterColumnType(DbType.Binary, 32767, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, 2147483647, "LONG BINARY");
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
		}

		protected virtual void RegisterNumericTypeMappings()
		{
			RegisterColumnType(DbType.Boolean, "BIT"); // BIT type is NOT NULL by default
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.UInt64, "UNSIGNED BIGINT");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.UInt16, "UNSIGNED SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.UInt32, "UNSIGNED INTEGER");
			RegisterColumnType(DbType.Single, "REAL");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)"); // Precision ranges from 0-127
			RegisterColumnType(DbType.Decimal, 19, "NUMERIC($p, $s)"); // Precision ranges from 0-127
		}

		protected virtual void RegisterDateTimeTypeMappings()
		{
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.Time, "TIME");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
		}

		protected virtual void RegisterReverseNHibernateTypeMappings()
		{
		}

		protected virtual void RegisterFunctions()
		{
			RegisterMathFunctions();
			RegisterXmlFunctions();
			RegisterAggregationFunctions();
			RegisterBitFunctions();
			RegisterDateFunctions();
			RegisterStringFunctions();
			RegisterSoapFunctions();
			RegisterMiscellaneousFunctions();
		}

		protected virtual void RegisterMathFunctions()
		{
			// mathematical functions
			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("atan2", new StandardSQLFunction("atan2", NHibernateUtil.Double));
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("degrees", new StandardSQLFunction("degrees", NHibernateUtil.Double));
			RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));
			RegisterFunction("floor", new StandardSQLFunction("floor", NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
			RegisterFunction("mod", new StandardSQLFunction("mod"));
			RegisterFunction("pi", new NoArgSQLFunction("pi", NHibernateUtil.Double, true));
			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));
			RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
			RegisterFunction("rand", new StandardSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("remainder", new StandardSQLFunction("remainder"));
			RegisterFunction("round", new StandardSQLFunction("round"));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("truncate", new StandardSQLFunction("truncate"));
		}

		protected virtual void RegisterXmlFunctions()
		{
			// XML scalar functions only
			RegisterFunction("xmlconcat", new VarArgsSQLFunction(NHibernateUtil.String, "xmlconcat(", ",", ")"));
			RegisterFunction("xmlelement", new VarArgsSQLFunction(NHibernateUtil.String, "xmlelement(", ",", ")"));
			RegisterFunction("xmlgen", new VarArgsSQLFunction(NHibernateUtil.String, "xmlgen(", ",", ")"));
			// missing: XMLForest().
		}

		protected virtual void RegisterAggregationFunctions()
		{
			// basic aggregate, linear regression OLAP, and window functions
			RegisterFunction("bit_or", new StandardSQLFunction("bit_or"));
			RegisterFunction("bit_and", new StandardSQLFunction("bit_and"));
			RegisterFunction("bit_xor", new StandardSQLFunction("bit_xor"));
			RegisterFunction("covar_pop", new StandardSQLFunction("covar_pop", NHibernateUtil.Double));
			RegisterFunction("covar_samp", new StandardSQLFunction("covar_samp", NHibernateUtil.Double));
			RegisterFunction("corr", new StandardSQLFunction("corr", NHibernateUtil.Double));
			RegisterFunction("first_value", new VarArgsSQLFunction(NHibernateUtil.Double, "first_value(", ",", ")"));
			RegisterFunction("grouping", new StandardSQLFunction("grouping", NHibernateUtil.Int32));
			RegisterFunction("last_value", new VarArgsSQLFunction(NHibernateUtil.Double, "last_value(", ",", ")"));
			RegisterFunction("list", new VarArgsSQLFunction("list(", ",", ")"));
			RegisterFunction("regr_avgx", new StandardSQLFunction("regr_avgx", NHibernateUtil.Double));
			RegisterFunction("regr_avgy", new StandardSQLFunction("regr_avgy", NHibernateUtil.Double));
			RegisterFunction("regr_count", new StandardSQLFunction("regr_count", NHibernateUtil.Double));
			RegisterFunction("regr_intercept", new StandardSQLFunction("regr_intercept", NHibernateUtil.Double));
			RegisterFunction("regr_r2", new StandardSQLFunction("regr_r2", NHibernateUtil.Double));
			RegisterFunction("regr_slope", new StandardSQLFunction("regr_slope", NHibernateUtil.Double));
			RegisterFunction("regr_sxx", new StandardSQLFunction("regr_sxx", NHibernateUtil.Double));
			RegisterFunction("regr_sxy", new StandardSQLFunction("regr_sxy", NHibernateUtil.Double));
			RegisterFunction("regr_syy", new StandardSQLFunction("regr_syy", NHibernateUtil.Double));
			RegisterFunction("set_bits", new StandardSQLFunction("set_bits"));
			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("stddev_pop", new StandardSQLFunction("stddev_pop", NHibernateUtil.Double));
			RegisterFunction("stddev_samp", new StandardSQLFunction("stddev_samp", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));
			RegisterFunction("var_pop", new StandardSQLFunction("var_pop", NHibernateUtil.Double));
			RegisterFunction("var_samp", new StandardSQLFunction("var_samp", NHibernateUtil.Double));
			RegisterFunction("xmlagg", new StandardSQLFunction("xmlagg"));
		}

		protected virtual void RegisterBitFunctions()
		{
			RegisterFunction("bit_length", new StandardSQLFunction("bit_length", NHibernateUtil.Int32));
			RegisterFunction("bit_substr", new StandardSQLFunction("bit_substr"));
			RegisterFunction("get_bit", new StandardSQLFunction("get_bit", NHibernateUtil.Boolean));
			RegisterFunction("set_bit", new VarArgsSQLFunction("set_bit(", ",", ")"));
		}

		protected virtual void RegisterDateFunctions()
		{
			RegisterFunction("date", new StandardSQLFunction("date", NHibernateUtil.Date));
			RegisterFunction("dateadd", new StandardSQLFunction("dateadd", NHibernateUtil.Timestamp));
			RegisterFunction("datediff", new StandardSQLFunction("datediff", NHibernateUtil.Int32));
			RegisterFunction("dateformat", new StandardSQLFunction("dateformat", NHibernateUtil.String));
			RegisterFunction("datename", new StandardSQLFunction("datename", NHibernateUtil.String));
			RegisterFunction("datepart", new StandardSQLFunction("datepart", NHibernateUtil.Int32));
			RegisterFunction("datetime", new StandardSQLFunction("datetime", NHibernateUtil.Timestamp));
			RegisterFunction("day", new StandardSQLFunction("day", NHibernateUtil.Int32));
			RegisterFunction("dayname", new StandardSQLFunction("dayname", NHibernateUtil.String));
			RegisterFunction("days", new StandardSQLFunction("days"));
			RegisterFunction("dow", new StandardSQLFunction("dow", NHibernateUtil.Int32));
			RegisterFunction("getdate", new StandardSQLFunction("getdate", NHibernateUtil.Timestamp));
			RegisterFunction("hour", new StandardSQLFunction("hour", NHibernateUtil.Int32));
			RegisterFunction("hours", new StandardSQLFunction("hours"));
			RegisterFunction("minute", new StandardSQLFunction("minute", NHibernateUtil.Int32));
			RegisterFunction("minutes", new StandardSQLFunction("minutes"));
			RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
			RegisterFunction("monthname", new StandardSQLFunction("monthname", NHibernateUtil.String));
			RegisterFunction("months", new StandardSQLFunction("months"));
			RegisterFunction("now", new NoArgSQLFunction("now", NHibernateUtil.Timestamp));
			RegisterFunction("quarter", new StandardSQLFunction("quarter", NHibernateUtil.Int32));
			RegisterFunction("second", new StandardSQLFunction("second", NHibernateUtil.Int32));
			RegisterFunction("seconds", new StandardSQLFunction("seconds"));
			RegisterFunction("today", new NoArgSQLFunction("now", NHibernateUtil.Date));
			RegisterFunction("weeks", new StandardSQLFunction("weeks"));
			RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));
			RegisterFunction("years", new StandardSQLFunction("years"));
			RegisterFunction("ymd", new StandardSQLFunction("ymd", NHibernateUtil.Date));

			// compatibility functions
			RegisterFunction("current_timestamp", new NoArgSQLFunction("getdate", NHibernateUtil.Timestamp, true));
			RegisterFunction("current_time", new NoArgSQLFunction("getdate", NHibernateUtil.Time, true));
			RegisterFunction("current_date", new NoArgSQLFunction("getdate", NHibernateUtil.Date, true));
		}

		protected virtual void RegisterStringFunctions()
		{
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("byte64_decode", new StandardSQLFunction("byte64_decode", NHibernateUtil.StringClob));
			RegisterFunction("byte64_encode", new StandardSQLFunction("byte64_encode", NHibernateUtil.StringClob));
			RegisterFunction("byte_length", new StandardSQLFunction("byte_length", NHibernateUtil.Int32));
			RegisterFunction("byte_substr", new VarArgsSQLFunction(NHibernateUtil.String, "byte_substr(", ",", ")"));
			RegisterFunction("char", new StandardSQLFunction("char", NHibernateUtil.String));
			RegisterFunction("charindex", new StandardSQLFunction("charindex", NHibernateUtil.Int32));
			RegisterFunction("char_length", new StandardSQLFunction("char_length", NHibernateUtil.Int32));
			RegisterFunction("compare", new VarArgsSQLFunction(NHibernateUtil.Int32, "compare(", ",", ")"));
			RegisterFunction("compress", new VarArgsSQLFunction(NHibernateUtil.BinaryBlob, "compress(", ",", ")"));
			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "+", ")"));
			RegisterFunction("csconvert", new VarArgsSQLFunction(NHibernateUtil.StringClob, "csconvert(", ",", ")"));
			RegisterFunction("decompress", new VarArgsSQLFunction(NHibernateUtil.BinaryBlob, "decompress(", ",", ")"));
			RegisterFunction("decrypt", new VarArgsSQLFunction(NHibernateUtil.BinaryBlob, "decrypt(", ",", ")"));
			RegisterFunction("difference", new StandardSQLFunction("difference", NHibernateUtil.Int32));
			RegisterFunction("encrypt", new VarArgsSQLFunction(NHibernateUtil.BinaryBlob, "encrypt(", ",", ")"));
			RegisterFunction("hash", new VarArgsSQLFunction(NHibernateUtil.String, "hash(", ",", ")"));
			RegisterFunction("insertstr", new StandardSQLFunction("insertstr", NHibernateUtil.String));
			RegisterFunction("lcase", new StandardSQLFunction("lcase", NHibernateUtil.String));
			RegisterFunction("left", new StandardSQLFunction("left", NHibernateUtil.String));
			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int32));
			RegisterFunction("locate", new VarArgsSQLFunction(NHibernateUtil.Int32, "locate(", ",", ")"));
			RegisterFunction("lower", new StandardSQLFunction("lower", NHibernateUtil.String));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim", NHibernateUtil.String));
			RegisterFunction("patindex", new StandardSQLFunction("patindex", NHibernateUtil.Int32));
			RegisterFunction("repeat", new StandardSQLFunction("repeat", NHibernateUtil.String));
			RegisterFunction("replace", new StandardSQLFunction("replace", NHibernateUtil.String));
			RegisterFunction("replicate", new StandardSQLFunction("replicate", NHibernateUtil.String));
			RegisterFunction("reverse", new StandardSQLFunction("reverse", NHibernateUtil.String));
			RegisterFunction("right", new StandardSQLFunction("right", NHibernateUtil.String));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim", NHibernateUtil.String));
			RegisterFunction("similar", new StandardSQLFunction("rtrim", NHibernateUtil.Int32));
			RegisterFunction("sortkey", new VarArgsSQLFunction(NHibernateUtil.Binary, "sortkey(", ",", ")"));
			RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.Int32));
			RegisterFunction("space", new StandardSQLFunction("space", NHibernateUtil.String));
			RegisterFunction("str", new VarArgsSQLFunction(NHibernateUtil.String, "str(", ",", ")"));
			RegisterFunction("string", new VarArgsSQLFunction(NHibernateUtil.String, "string(", ",", ")"));
			RegisterFunction("strtouuid", new StandardSQLFunction("strtouuid"));
			RegisterFunction("stuff", new StandardSQLFunction("stuff", NHibernateUtil.String));

			// In SQL Anywhere 10, substr() semantics depends on the ANSI_substring option

			RegisterFunction("substr", new VarArgsSQLFunction(NHibernateUtil.String, "substr(", ",", ")"));
			RegisterFunction("substring", new VarArgsSQLFunction(NHibernateUtil.String, "substr(", ",", ")"));
			RegisterFunction("to_char", new VarArgsSQLFunction(NHibernateUtil.String, "to_char(", ",", ")"));
			RegisterFunction("to_nchar", new VarArgsSQLFunction(NHibernateUtil.String, "to_nchar(", ",", ")"));

			RegisterFunction("trim", new StandardSQLFunction("trim", NHibernateUtil.String));
			RegisterFunction("ucase", new StandardSQLFunction("ucase", NHibernateUtil.String));
			RegisterFunction("unicode", new StandardSQLFunction("unicode", NHibernateUtil.Int32));
			RegisterFunction("unistr", new StandardSQLFunction("unistr", NHibernateUtil.String));
			RegisterFunction("upper", new StandardSQLFunction("upper", NHibernateUtil.String));
			RegisterFunction("uuidtostr", new StandardSQLFunction("uuidtostr", NHibernateUtil.String));
		}

		protected virtual void RegisterSoapFunctions()
		{
			RegisterFunction("html_decode", new StandardSQLFunction("html_decode", NHibernateUtil.String));
			RegisterFunction("html_encode", new StandardSQLFunction("html_encode", NHibernateUtil.String));
			RegisterFunction("http_decode", new StandardSQLFunction("http_decode", NHibernateUtil.String));
			RegisterFunction("http_encode", new StandardSQLFunction("http_encode", NHibernateUtil.String));
			RegisterFunction("http_header", new StandardSQLFunction("http_header", NHibernateUtil.String));
			RegisterFunction("http_variable", new VarArgsSQLFunction("http_variable(", ",", ")"));
			RegisterFunction("next_http_header", new StandardSQLFunction("next_http_header", NHibernateUtil.String));
			RegisterFunction("next_http_variable", new StandardSQLFunction("next_http_variable", NHibernateUtil.String));
			RegisterFunction("next_soap_header", new VarArgsSQLFunction("next_soap_header(", ",", ")"));
		}

		protected virtual void RegisterMiscellaneousFunctions()
		{
			RegisterFunction("argn", new VarArgsSQLFunction("argn(", ",", ")"));
			RegisterFunction("coalesce", new VarArgsSQLFunction("coalesce(", ",", ")"));
			RegisterFunction("conflict", new StandardSQLFunction("conflict", NHibernateUtil.Boolean));
			RegisterFunction("connection_property", new VarArgsSQLFunction("connection_property(", ",", ")"));
			RegisterFunction("connection_extended_property", new VarArgsSQLFunction("connection_extended_property(", ",", ")"));
			RegisterFunction("db_extended_property", new VarArgsSQLFunction("db_extended_property(", ",", ")"));
			RegisterFunction("db_property", new VarArgsSQLFunction("db_property(", ",", ")"));
			RegisterFunction("errormsg", new NoArgSQLFunction("errormsg", NHibernateUtil.String, true));
			RegisterFunction("estimate", new VarArgsSQLFunction("estimate(", ",", ")"));
			RegisterFunction("estimate_source", new VarArgsSQLFunction(NHibernateUtil.String, "estimate_source(", ",", ")"));
			RegisterFunction("experience_estimate", new VarArgsSQLFunction("experience_estimate(", ",", ")"));
			RegisterFunction("explanation", new VarArgsSQLFunction(NHibernateUtil.String, "explanation(", ",", ")"));
			RegisterFunction("exprtype", new StandardSQLFunction("exprtype", NHibernateUtil.String));
			RegisterFunction("get_identity", new VarArgsSQLFunction("get_identity(", ",", ")"));
			RegisterFunction("graphical_plan", new VarArgsSQLFunction(NHibernateUtil.String, "graphical_plan(", ",", ")"));
			RegisterFunction("greater", new StandardSQLFunction("greater"));
			RegisterFunction("identity", new StandardSQLFunction("identity"));
			RegisterFunction("ifnull", new VarArgsSQLFunction("ifnull(", ",", ")"));
			RegisterFunction("index_estimate", new VarArgsSQLFunction("index_estimate(", ",", ")"));
			RegisterFunction("isnull", new VarArgsSQLFunction("isnull(", ",", ")"));
			RegisterFunction("lesser", new StandardSQLFunction("lesser"));
			RegisterFunction("newid", new NoArgSQLFunction("newid", NHibernateUtil.String, true));
			RegisterFunction("nullif", new StandardSQLFunction("nullif"));
			RegisterFunction("number", new NoArgSQLFunction("number", NHibernateUtil.Int32));
			RegisterFunction("plan", new VarArgsSQLFunction(NHibernateUtil.String, "plan(", ",", ")"));
			RegisterFunction("property", new StandardSQLFunction("property", NHibernateUtil.String));
			RegisterFunction("property_description", new StandardSQLFunction("property_description", NHibernateUtil.String));
			RegisterFunction("property_name", new StandardSQLFunction("property_name", NHibernateUtil.String));
			RegisterFunction("property_number", new StandardSQLFunction("property_number", NHibernateUtil.Int32));
			RegisterFunction("rewrite", new VarArgsSQLFunction(NHibernateUtil.String, "rewrite(", ",", ")"));
			RegisterFunction("row_number", new NoArgSQLFunction("row_number", NHibernateUtil.Int32, true));
			RegisterFunction("sqldialect", new StandardSQLFunction("sqldialect", NHibernateUtil.String));
			RegisterFunction("sqlflagger", new StandardSQLFunction("sqlflagger", NHibernateUtil.String));
			RegisterFunction("traceback", new NoArgSQLFunction("traceback", NHibernateUtil.String));
			RegisterFunction("transactsql", new StandardSQLFunction("transactsql", NHibernateUtil.String));
			RegisterFunction("varexists", new StandardSQLFunction("varexists", NHibernateUtil.Int32));
			RegisterFunction("watcomsql", new StandardSQLFunction("watcomsql", NHibernateUtil.String));
		}

		protected virtual void RegisterKeywords()
		{
			RegisterKeyword("TOP");
			RegisterKeyword("FIRST");
			RegisterKeyword("FETCH");
			RegisterKeyword("START");
			RegisterKeyword("AT");
			RegisterKeyword("WITH");
			RegisterKeyword("CONTAINS");
			RegisterKeyword("REGEXP");
			RegisterKeyword("SIMILAR");
			RegisterKeyword("SEQUENCE");
		}

		#region IDENTITY or AUTOINCREMENT support

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		public override string IdentitySelectString
		{
			get { return "select @@identity"; }
		}

		/// <summary>
		/// SQL Anywhere uses <tt>DEFAULT AUTOINCREMENT</tt> to identify an IDENTITY
		/// column in a <tt>CREATE TABLE</tt> statement.
		/// </summary>
		public override string IdentityColumnString
		{
			get { return "not null default autoincrement"; }
		}

		public override SqlString AppendIdentitySelectToInsert(SqlString insertSql)
		{
			return insertSql.Append("; " + IdentitySelectString);
		}

		public override bool SupportsInsertSelectIdentity
		{
			get { return true; }
		}

		#endregion

		#region LIMIT/OFFSET support

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return true; }
		}

		public override bool OffsetStartsAtOne
		{
			get { return true; }
		}

		private static int GetAfterSelectInsertPoint(SqlString sql)
		{
			// Assume no common table expressions with the statement.
			if (sql.StartsWithCaseInsensitive("select distinct"))
			{
				return 15;
			}
			else if (sql.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}
			return 0;
		}

		public override SqlString GetLimitString(SqlString sql, SqlString offset, SqlString limit)
		{
			// SQL Anywhere 11 uses SELECT TOP n START AT m [ select list items ]
			// for LIMIT/OFFSET support.  Does not support a limit of zero.

			// FIXME - Add support for where offset is set, but limit is not.

			int insertionPoint = GetAfterSelectInsertPoint(sql);

			if (insertionPoint > 0)
			{
				SqlStringBuilder limitBuilder = new SqlStringBuilder();
				limitBuilder.Add("select");
				if (insertionPoint > 6)
				{
					limitBuilder.Add(" distinct ");
				}
				limitBuilder.Add(" top ");
				limitBuilder.Add(limit);
				if (offset != null)
				{
					limitBuilder.Add(" start at ");
					limitBuilder.Add(offset);
				}
				limitBuilder.Add(sql.Substring(insertionPoint));
				return limitBuilder.ToSqlString();
			}
			else
			{
				return sql; // unchanged
			}
		}

		#endregion

		#region Lock acquisition support

		/// <summary>
		/// SQL Anywhere 10 supports READ, WRITE, and INTENT row
		/// locks. INTENT locks are sufficient to ensure that other
		/// concurrent connections cannot modify a row (though other
		/// connections can still read that row). SQL Anywhere also
		/// supports 3 modes of snapshot isolation (multi-version
		/// concurrency control (MVCC).
		///
		/// SQL Anywhere's <tt>FOR UPDATE</tt> clause supports
		///	<tt>FOR UPDATE BY [ LOCK | VALUES ]</tt>
		///	<tt>FOR UPDATE OF ( COLUMN LIST )</tt>
		///
		/// though they cannot be specified at the same time. <tt>BY LOCK</tt> is
		/// the syntax that acquires INTENT locks.  <tt>FOR UPDATE BY VALUES</tt>
		/// forces the use of the KEYSET cursor, which returns a warning to
		/// the application when a row in the cursor has been subsequently
		/// modified by another connection, and an error if the row has
		/// been deleted.
		///
		/// SQL Anywhere does not support the <tt>FOR UPDATE NOWAIT</tt> syntax of
		/// Oracle on a statement-by-statement basis.  However, the
		/// identical functionality is provided by setting the connection
		/// option <tt>BLOCKING</tt> to "OFF", or setting an appropriate timeout
		/// period through the connection option <tt>BLOCKING_TIMEOUT</tt>.
		/// </summary>
		public override string GetForUpdateString(LockMode lockMode)
		{
			if (lockMode == LockMode.Read)
			{
				return ForReadOnlyString;
			}
			else if (lockMode == LockMode.Upgrade)
			{
				return ForUpdateByLockString;
			}
			else if (lockMode == LockMode.UpgradeNoWait)
			{
				return ForUpdateNowaitString;
			}
			else if (lockMode == LockMode.Force)
			{
				return ForUpdateNowaitString;
			}
			else
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// SQL Anywhere does support <tt>FOR UPDATE OF</tt> syntax. However,
		/// in SQL Anywhere one cannot specify both <tt>FOR UPDATE OF</tt> syntax
		/// and <tt>FOR UPDATE BY LOCK</tt> in the same statement. To achieve INTENT
		/// locking when using <tt>FOR UPDATE OF</tt> syntax one must use a table hint
		/// in the query's FROM clause, ie.
		///
		/// SELECT * FROM FOO WITH( UPDLOCK ) FOR UPDATE OF ( column-list ).
		///
		/// In this dialect, we avoid this issue by supporting only
		/// <tt>FOR UPDATE BY LOCK</tt>.
		/// </summary>
		public override bool ForUpdateOfColumns
		{
			get { return false; }
		}

		/// <summary>
		/// SQL Anywhere supports <tt>FOR UPDATE</tt> over cursors containing
		/// outer joins.
		/// </summary>
		public override bool SupportsOuterJoinForUpdate
		{
			get { return true; }
		}

		/// <summary>
		/// Lock rows in the cursor explicitly using INTENT row locks.
		/// </summary>
		public override string ForUpdateString
		{
			get { return ForUpdateByLockString; }
		}

		/// <summary>
		/// Enforce the condition that this query is read-only. This ensure that certain
		/// query rewrite optimizations, such as join elimination, can be used.
		/// </summary>
		public string ForReadOnlyString
		{
			get { return " for read only"; }
		}

		/// <summary>
		/// Lock rows in the cursor explicitly using INTENT row locks.
		/// </summary>
		public string ForUpdateByLockString
		{
			get { return " for update by lock"; }
		}

		/// <summary>
		/// SQL Anywhere does not support <tt>FOR UPDATE NOWAIT</tt>. However, the intent
		/// is to acquire pessimistic locks on the underlying rows; with NHibernate
		/// one can accomplish this through setting the BLOCKING connection option.
		/// Hence, with this API we lock rows in the cursor explicitly using INTENT row locks.
		/// </summary>
		public override string ForUpdateNowaitString
		{
			get { return ForUpdateByLockString; }
		}

		/// <summary>
		/// We assume that applications using this dialect are NOT using
		/// SQL Anywhere's snapshot isolation modes.
		/// </summary>
		public override bool DoesReadCommittedCauseWritersToBlockReaders
		{
			get { return true; }
		}

		/// <summary>
		/// We assume that applications using this dialect are NOT using
		/// SQL Anywhere's snapshot isolation modes.
		/// </summary>
		public override bool DoesRepeatableReadCauseReadersToBlockWriters
		{
			get { return true; }
		}

		// SQL Anywhere-specific query syntax

		public override bool SupportsCurrentTimestampSelection
		{
			get { return true; }
		}

		public override string CurrentTimestampSQLFunctionName
		{
			get { return "getdate"; }
		}

		public override bool IsCurrentTimestampSelectStringCallable
		{
			get { return false; }
		}

		public override string CurrentTimestampSelectString
		{
			get { return "select getdate()"; }
		}

		/// <summary>
		/// SQL Anywhere supports both double quotes or '[' (Microsoft syntax) for
		/// quoted identifiers.
		///
		/// Note that quoted identifiers are controlled through
		/// the QUOTED_IDENTIFIER connection option.
		/// </summary>
		public override char CloseQuote
		{
			get { return '"'; }
		}

		/// <summary>
		/// SQL Anywhere supports both double quotes or '[' (Microsoft syntax) for
		/// quoted identifiers.
		/// </summary>
		public override char OpenQuote
		{
			get { return '"'; }
		}

		#endregion

		#region Informational metadata

		public override bool SupportsEmptyInList
		{
			get { return false; }
		}

		/// <summary>
		/// SQL Anywhere's implementation of KEYSET-DRIVEN cursors does not
		/// permit absolute positioning. With jConnect as the driver, this support
		/// will succeed because jConnect FETCHes the entire result set to the client
		/// first; it will fail with the iAnywhere JDBC driver. Because the server
		/// may decide to use a KEYSET cursor even if the cursor is declared as
		/// FORWARD ONLY, this support is disabled.
		/// </summary>
		public override bool SupportsResultSetPositionQueryMethodsOnForwardOnlyCursor
		{
			get { return false; }
		}

		public override bool SupportsExistsInSelect
		{
			get { return false; }
		}

		/// <summary>
		/// By default, the SQL Anywhere dbinit utility creates a
		/// case-insensitive database for the CHAR collation.  This can
		/// be changed through the use of the -c command line switch on
		/// dbinit, and the setting may differ for the NCHAR collation
		/// for national character sets.  Whether or not a database
		/// supports case-sensitive comparisons can be determined via
		/// the DB_Extended_property() function, for example
		///
		/// SELECT DB_EXTENDED_PROPERTY( 'Collation', 'CaseSensitivity');
		/// </summary>
		public override bool AreStringComparisonsCaseInsensitive
		{
			get { return true; }
		}

		#endregion

		#region DDL support

		/// <summary>
		/// SQL Anywhere supports <tt>COMMENT ON</tt> statements for a wide variety of
		/// database objects. When the COMMENT statement is executed an implicit
		/// <tt>COMMIT</tt> is performed. However, COMMENT syntax for <tt>CREATE TABLE</tt>, as
		/// expected by NHibernate (see Table.cs), is not supported.
		/// </summary>
		public override bool SupportsCommentOn
		{
			get { return false; }
		}

		public override int MaxAliasLength
		{
			get { return 127; }
		}

		public override string AddColumnString
		{
			get { return "add "; }
		}

		public override string NullColumnString
		{
			get { return " null"; }
		}

		public override bool QualifyIndexName
		{
			get { return false; }
		}

		/// <summary>
		/// SQL Anywhere currently supports only "VALUES (DEFAULT)", not
		/// the ANSI standard "DEFAULT VALUES". This latter syntax will be
		/// supported in the SQL Anywhere 11.0.1 release.  For the moment,
		/// "VALUES (DEFAULT)" works only for a single-column table.
		/// </summary>
		public override string NoColumnsInsertString
		{
			get { return " values (default) "; }
		}

		/// <summary>
		/// SQL Anywhere does not require dropping a constraint before
		/// dropping a table, and the DROP statement syntax used by Hibernate
		/// to drop a constraint is not compatible with SQL Anywhere, so disable it.
		/// </summary>
		public override bool DropConstraints
		{
			get { return false; }
		}

		public override string DropForeignKeyString
		{
			get { return " drop foreign key "; }
		}

		#endregion

		#region Temporary table support

		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		/// <summary>
		/// In SQL Anywhere, the syntax, DECLARE LOCAL TEMPORARY TABLE ...,
		/// can also be used, which creates a temporary table with procedure scope,
		/// which may be important for stored procedures.
		/// </summary>
		public override string CreateTemporaryTableString
		{
			get { return "create local temporary table "; }
		}

		/// <summary>
		/// Assume that temporary table rows should be preserved across COMMITs.
		/// </summary>
		public override string CreateTemporaryTablePostfix
		{
			get { return " on commit preserve rows "; }
		}

		/// <summary>
		/// SQL Anywhere 10 does not perform a COMMIT upon creation of
		/// a temporary table.  However, it does perform an implicit
		/// COMMIT when creating an index over a temporary table, or
		/// upon ALTERing the definition of temporary table.
		/// </summary>
		public override bool? PerformTemporaryTableDDLInIsolation()
		{
			return null;
		}

		#endregion

		#region Callable statement support

		/// <summary>
		/// SQL Anywhere does support OUT parameters with callable stored procedures.
		///  </summary>
		public override int RegisterResultSetOutParameter(DbCommand statement, int position)
		{
			return position;
		}

		public override DbDataReader GetResultSet(DbCommand statement)
		{
			var rdr = statement.ExecuteReader();
			return rdr;
		}

		#endregion

		public override string SelectGUIDString
		{
			get { return "select newid()"; }
		}

		public override bool SupportsUnionAll
		{
			get { return true; }
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new SybaseAnywhereDataBaseMetaData(connection);
		}
	}
}