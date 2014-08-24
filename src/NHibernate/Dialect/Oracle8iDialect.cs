using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
	/// A dialect for Oracle 8i. 
	/// </summary>
	public class Oracle8iDialect : Dialect
	{
		public override string CurrentTimestampSelectString
		{
			get { return "select sysdate from dual"; }
		}

		public override string CurrentTimestampSQLFunctionName
		{
			get { return "sysdate"; }
		}

		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override string CascadeConstraintsString
		{
			get { return " cascade constraints"; }
		}

		public override string QuerySequencesString
		{
			get { return "select sequence_name from user_sequences"; }
		}

		public override string SelectGUIDString
		{
			get { return "select rawtohex(sys_guid()) from dual"; }
		}

		public override string CreateTemporaryTableString
		{
			get { return "create global temporary table"; }
		}

		public override string CreateTemporaryTablePostfix
		{
			get { return "on commit delete rows"; }
		}

		public override bool IsCurrentTimestampSelectStringCallable
		{
			get { return false; }
		}

		public Oracle8iDialect()
		{
			RegisterCharacterTypeMappings();
			RegisterNumericTypeMappings();
			RegisterDateTimeTypeMappings();
			RegisterLargeObjectTypeMappings();
			RegisterGuidTypeMapping();
			RegisterReverseHibernateTypeMappings();

			RegisterFunctions();

			RegisterDefaultProperties();
		}

		protected virtual void RegisterGuidTypeMapping()
		{
			RegisterColumnType(DbType.Guid, "RAW(16)");
		}

		protected virtual void RegisterCharacterTypeMappings()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 2000, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR2(255)");
			RegisterColumnType(DbType.AnsiString, 4000, "VARCHAR2($l)");
			RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 2000, "NCHAR($l)");
			RegisterColumnType(DbType.String, "NVARCHAR2(255)");
			RegisterColumnType(DbType.String, 4000, "NVARCHAR2($l)");
		}

		protected virtual void RegisterNumericTypeMappings()
		{
			RegisterColumnType(DbType.Boolean, "NUMBER(1,0)");
			RegisterColumnType(DbType.Byte, "NUMBER(3,0)");
			RegisterColumnType(DbType.Int16, "NUMBER(5,0)");
			RegisterColumnType(DbType.Int32, "NUMBER(10,0)");
			RegisterColumnType(DbType.Int64, "NUMBER(20,0)");
			RegisterColumnType(DbType.UInt16, "NUMBER(5,0)");
			RegisterColumnType(DbType.UInt32, "NUMBER(10,0)");
			RegisterColumnType(DbType.UInt64, "NUMBER(20,0)");

			RegisterColumnType(DbType.Currency, "NUMBER(20,2)");
			RegisterColumnType(DbType.Single, "FLOAT(24)");
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
			RegisterColumnType(DbType.Double, 19, "NUMBER($p,$s)");
			RegisterColumnType(DbType.Decimal, "NUMBER(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "NUMBER($p,$s)");
		}

		protected virtual void RegisterDateTimeTypeMappings()
		{
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATE");
			RegisterColumnType(DbType.Time, "DATE");
		}

		protected virtual void RegisterLargeObjectTypeMappings()
		{
			RegisterColumnType(DbType.Binary, "RAW(2000)");
			RegisterColumnType(DbType.Binary, 2000, "RAW($l)");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB");
			RegisterColumnType(DbType.AnsiString, 2147483647, "CLOB"); // should use the IType.ClobType
			RegisterColumnType(DbType.String, 1073741823, "NCLOB");
		}

		protected virtual void RegisterReverseHibernateTypeMappings() {}

		protected virtual void RegisterFunctions()
		{
			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));

			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cosh", new StandardSQLFunction("cosh", NHibernateUtil.Double));
			RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));
			RegisterFunction("ln", new StandardSQLFunction("ln", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("sinh", new StandardSQLFunction("sinh", NHibernateUtil.Double));
			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("tanh", new StandardSQLFunction("tanh", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));

			RegisterFunction("round", new StandardSQLFunction("round"));
			RegisterFunction("trunc", new StandardSQLFunction("trunc"));
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));

			RegisterFunction("chr", new StandardSQLFunction("chr", NHibernateUtil.Character));
			RegisterFunction("initcap", new StandardSQLFunction("initcap"));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim"));
			RegisterFunction("soundex", new StandardSQLFunction("soundex"));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int64));
			RegisterFunction("left", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1, 1, ?2)"));
			RegisterFunction("right", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1, -?2)"));

			RegisterFunction("to_char", new StandardSQLFunction("to_char", NHibernateUtil.String));
			RegisterFunction("to_date", new StandardSQLFunction("to_date", NHibernateUtil.Timestamp));

			RegisterFunction("current_date", new NoArgSQLFunction("current_date", NHibernateUtil.Date, false));
			RegisterFunction("current_time", new NoArgSQLFunction("current_timestamp", NHibernateUtil.Time, false));
			RegisterFunction("current_timestamp", new CurrentTimeStamp());

			// Cast is needed because EXTRACT treats DATE not as legacy Oracle DATE but as ANSI DATE, without time elements.
			// Therefore, you can extract only YEAR, MONTH, and DAY from a DATE value.
			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(second from cast(?1 as timestamp))"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(minute from cast(?1 as timestamp))"));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "extract(hour from cast(?1 as timestamp))"));

			RegisterFunction("date", new StandardSQLFunction("trunc", NHibernateUtil.Date));

			RegisterFunction("last_day", new StandardSQLFunction("last_day", NHibernateUtil.Date));
			RegisterFunction("sysdate", new NoArgSQLFunction("sysdate", NHibernateUtil.Date, false));
			RegisterFunction("systimestamp", new NoArgSQLFunction("systimestamp", NHibernateUtil.Timestamp, false));
			RegisterFunction("uid", new NoArgSQLFunction("uid", NHibernateUtil.Int32, false));
			RegisterFunction("user", new NoArgSQLFunction("user", NHibernateUtil.String, false));

			RegisterFunction("rowid", new NoArgSQLFunction("rowid", NHibernateUtil.Int64, false));
			RegisterFunction("rownum", new NoArgSQLFunction("rownum", NHibernateUtil.Int64, false));

			// Multi-param string dialect functions...
			RegisterFunction("instr", new StandardSQLFunction("instr", NHibernateUtil.Int32));
			RegisterFunction("instrb", new StandardSQLFunction("instrb", NHibernateUtil.Int32));
			RegisterFunction("lpad", new StandardSQLFunction("lpad", NHibernateUtil.String));
			RegisterFunction("replace", new StandardSQLFunction("replace", NHibernateUtil.String));
			RegisterFunction("rpad", new StandardSQLFunction("rpad", NHibernateUtil.String));
			RegisterFunction("substr", new StandardSQLFunction("substr", NHibernateUtil.String));
			RegisterFunction("substrb", new StandardSQLFunction("substrb", NHibernateUtil.String));
			RegisterFunction("translate", new StandardSQLFunction("translate", NHibernateUtil.String));

			RegisterFunction("locate", new LocateFunction());
			RegisterFunction("substring", new StandardSQLFunction("substr", NHibernateUtil.String));
			RegisterFunction("bit_length", new SQLFunctionTemplate(NHibernateUtil.Int32, "vsize(?1)*8"));
			RegisterFunction("coalesce", new NvlFunction());

			// Multi-param numeric dialect functions...
			RegisterFunction("atan2", new StandardSQLFunction("atan2", NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Int32));
			RegisterFunction("mod", new StandardSQLFunction("mod", NHibernateUtil.Int32));
			RegisterFunction("nvl", new StandardSQLFunction("nvl"));
			RegisterFunction("nvl2", new StandardSQLFunction("nvl2"));
			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));

			// Multi-param date dialect functions...
			RegisterFunction("add_months", new StandardSQLFunction("add_months", NHibernateUtil.Date));
			RegisterFunction("months_between", new StandardSQLFunction("months_between", NHibernateUtil.Single));
			RegisterFunction("next_day", new StandardSQLFunction("next_day", NHibernateUtil.Date));

			RegisterFunction("str", new StandardSQLFunction("to_char", NHibernateUtil.String));

			RegisterFunction("iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));
		}

		protected internal virtual void RegisterDefaultProperties()
		{
			//DefaultProperties[Environment.DefaultBatchFetchSize] = DefaultBatchSize; It can break some test and it is a user matter

			// Oracle driver reports to support GetGeneratedKeys(), but they only
			// support the version taking an array of the names of the columns to
			// be returned (via its RETURNING clause).  No other driver seems to
			// support this overloaded version.
			DefaultProperties[Environment.UseGetGeneratedKeys] = "false";
		}

		// features which change between 8i, 9i, and 10g ~~~~~~~~~~~~~~~~~~~~~~~~~~

		/// <summary> 
		/// Support for the oracle proprietary join syntax... 
		/// </summary>
		/// <returns> The orqacle join fragment </returns>
		public override JoinFragment CreateOuterJoinFragment()
		{
			return new OracleJoinFragment();
		}

		/// <summary> 
		/// Map case support to the Oracle DECODE function.  Oracle did not
		/// add support for CASE until 9i. 
		/// </summary>
		/// <returns> The oracle CASE -> DECODE fragment </returns>
		public override CaseFragment CreateCaseFragment()
		{
			return new DecodeCaseFragment(this);
		}

		public override SqlString GetLimitString(SqlString sql, SqlString offset, SqlString limit)
		{
			sql = sql.Trim();
			bool isForUpdate = false;
			if (sql.EndsWithCaseInsensitive(" for update"))
			{
				sql = sql.Substring(0, sql.Length - 11);
				isForUpdate = true;
			}

			string selectColumns = ExtractColumnOrAliasNames(sql);

			var pagingSelect = new SqlStringBuilder(sql.Count + 10);
			if (offset != null)
			{
				pagingSelect.Add("select " + selectColumns + " from ( select row_.*, rownum rownum_ from ( ");
			}
			else
			{
				pagingSelect.Add("select " + selectColumns + " from ( ");
			}
			pagingSelect.Add(sql);
			if (offset != null && limit != null)
			{
				pagingSelect.Add(" ) row_ where rownum <=").Add(limit).Add(") where rownum_ >").Add(offset);
			}
			else if (limit != null)
			{
				pagingSelect.Add(" ) where rownum <=").Add(limit);
			}
			else
			{
				// offset is specified, but limit is not.
				pagingSelect.Add(" ) row_ ) where rownum_ >").Add(offset);
			}

			if (isForUpdate)
			{
				pagingSelect.Add(" for update");
			}

			return pagingSelect.ToSqlString();
		}

		private string ExtractColumnOrAliasNames(SqlString select)
		{
			List<SqlString> columnsOrAliases;
			Dictionary<SqlString, SqlString> aliasToColumn;
			Dictionary<SqlString, SqlString> columnToAlias;
			ExtractColumnOrAliasNames(select, out columnsOrAliases, out aliasToColumn, out columnToAlias);

			return StringHelper.Join(",", columnsOrAliases);
		}

		/// <summary> 
		/// Allows access to the basic <see cref="Dialect.GetSelectClauseNullString"/>
		/// implementation... 
		/// </summary>
		/// <param name="sqlType">The <see cref="SqlType"/> mapping type</param>
		/// <returns> The appropriate select cluse fragment </returns>
		public virtual string GetBasicSelectClauseNullString(SqlType sqlType)
		{
			return base.GetSelectClauseNullString(sqlType);
		}

		public override string GetSelectClauseNullString(SqlType sqlType)
		{
			switch (sqlType.DbType)
			{
				case DbType.String:
				case DbType.AnsiString:
				case DbType.StringFixedLength:
				case DbType.AnsiStringFixedLength:
					return "to_char(null)";

				case DbType.Date:
				case DbType.DateTime:
				case DbType.Time:
					return "to_date(null)";

				default:
					return "to_number(null)";
			}
		}

		public override string GetSequenceNextValString(string sequenceName)
		{
			return "select " + GetSelectSequenceNextValString(sequenceName) + " from dual";
		}

		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return sequenceName + ".nextval";
		}

		public override SqlString AddIdentifierOutParameterToInsert(SqlString insertString, string identifierColumnName, string parameterName)
		{
			return insertString.Append(" returning " + identifierColumnName + " into :" + parameterName);
		}

		public override string GetCreateSequenceString(string sequenceName)
		{
			return "create sequence " + sequenceName; //starts with 1, implicitly
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			return "drop sequence " + sequenceName;
		}

		public override bool DropConstraints
		{
			get { return false; }
		}

		public override string ForUpdateNowaitString
		{
			get { return " for update nowait"; }
		}

		public override bool SupportsSequences
		{
			get { return true; }
		}

		public override bool SupportsPooledSequences
		{
			get { return true; }
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override string GetForUpdateString(string aliases)
		{
			return ForUpdateString + " of " + aliases;
		}

		public override string GetForUpdateNowaitString(string aliases)
		{
			return ForUpdateString + " of " + aliases + " nowait";
		}

		public override bool UseMaxForLimit
		{
			get { return true; }
		}

		public override bool ForUpdateOfColumns
		{
			get { return true; }
		}

		public override bool SupportsUnionAll
		{
			get { return true; }
		}

		public override bool SupportsCommentOn
		{
			get { return true; }
		}

		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		public override string GenerateTemporaryTableName(String baseTableName)
		{
			string name = base.GenerateTemporaryTableName(baseTableName);
			return name.Length > 30 ? name.Substring(1, (30) - (1)) : name;
		}

		public override bool DropTemporaryTableAfterUse()
		{
			return false;
		}

		public override bool SupportsCurrentTimestampSelection
		{
			get { return true; }
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new OracleDataBaseSchema(connection);
		}

		public override long TimestampResolutionInTicks
		{
			get
			{
				// Timestamps are DateTime, which in this dialect maps to Oracle DATE,
				// which doesn't support fractional seconds.
				return TimeSpan.TicksPerSecond;
			}
		}

		#region Overridden informational metadata

		public override bool SupportsEmptyInList
		{
			get { return false; }
		}

		public override bool SupportsExistsInSelect
		{
			get { return false; }
		}

		#endregion

		#region Functions
		[Serializable]
		private class CurrentTimeStamp : NoArgSQLFunction
		{
			public CurrentTimeStamp() : base("current_timestamp", NHibernateUtil.DateTime, true) {}

			public override SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				return new SqlString(Name);
			}
		}
		[Serializable]
		private class LocateFunction : ISQLFunction
		{
			private static readonly ISQLFunction LocateWith2Params = new SQLFunctionTemplate(NHibernateUtil.Int32,
																							 "instr(?2, ?1)");

			private static readonly ISQLFunction LocateWith3Params = new SQLFunctionTemplate(NHibernateUtil.Int32,
																							 "instr(?2, ?1, ?3)");

			#region Implementation of ISQLFunction

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
				if (args.Count != 2 && args.Count != 3)
				{
					throw new QueryException("'locate' function takes 2 or 3 arguments");
				}
				if (args.Count == 2)
				{
					return LocateWith2Params.Render(args, factory);
				}
				else
				{
					return LocateWith3Params.Render(args, factory);
				}
			}

			#endregion
		}

		#endregion
	}
}