using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect compatible with Microsoft SQL Server 2000.
	/// </summary>
	/// <remarks>
	/// The MsSql2000Dialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>use_outer_join</term>
	///			<description><see langword="true" /></description>
	///		</item>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.SqlClientDriver" /></description>
	///		</item>
	///		<item>
	///			<term>prepare_sql</term>
	///			<description><see langword="false" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class MsSql2000Dialect : Dialect
	{
		/// <summary></summary>
		public MsSql2000Dialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "TEXT");
			RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
			RegisterColumnType(DbType.Binary, 8000, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, 2147483647, "IMAGE");
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATETIME");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			// TODO: figure out if this is the good way to fix the problem
			// with exporting a DECIMAL column
			// NUMERIC(precision, scale) has a hardcoded precision of 19, even though it can range from 1 to 38
			// and the scale has to be 0 <= scale <= precision.
			// I think how I might handle it is keep the type="Decimal(29,5)" and make them specify a 
			// sql-type="decimal(20,5)" if they need to do that.  The Decimal parameter and ddl will get generated
			// correctly with minimal work.
			RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "DECIMAL(19, $l)");
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION"); //synonym for FLOAT(53)
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INT");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "REAL"); //synonym for FLOAT(24) 
			RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 4000, "NCHAR($l)");
			RegisterColumnType(DbType.String, "NVARCHAR(255)");
			RegisterColumnType(DbType.String, 4000, "NVARCHAR($l)");
			RegisterColumnType(DbType.String, 1073741823, "NTEXT");
			RegisterColumnType(DbType.Time, "DATETIME");

			RegisterFunction("count", new CountBigQueryFunction());

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
			RegisterFunction("mod", new SQLFunctionTemplate(NHibernateUtil.Int32, "(?1 % ?2)"));
			RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
			RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.String));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));

			RegisterFunction("current_timestamp", new NoArgSQLFunction("getdate", NHibernateUtil.DateTime, true));
			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(second, ?1)"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(minute, ?1)"));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(hour, ?1)"));
			RegisterFunction("day", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(day, ?1)"));
			RegisterFunction("month", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(month, ?1)"));
			RegisterFunction("year", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(year, ?1)"));

			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "+", ")"));
			/*
			RegisterFunction("julian_day", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("microsecond", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("midnight_seconds", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("monthname", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("quarter", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("date", new StandardSQLFunction(Hibernate.DATE) );
			RegisterFunction("dayname", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("dayofweek", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("dayofweek_iso", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("dayofyear", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("days", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("time", new StandardSQLFunction( NHibernateUtil.Time ) );
			RegisterFunction("timestamp", new StandardSQLFunction( NHibernateUtil.Timestamp ) );
			RegisterFunction("timestamp_iso", new StandardSQLFunction( NHibernateUtil.Timestamp ) );
			RegisterFunction("week", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("week_iso", new StandardSQLFunction( NHibernateUtil.Int32 ) );

			RegisterFunction("double", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction("varchar", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction("real", new StandardSQLFunction( NHibernateUtil.Single ) );
			RegisterFunction("bigint", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("char", new StandardSQLFunction( NHibernateUtil.Character ) );
			RegisterFunction("integer", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction("smallint", new StandardSQLFunction( NHibernateUtil.Int16 ) );
			*/

			RegisterFunction("digits", new StandardSQLFunction("digits", NHibernateUtil.String));
			RegisterFunction("chr", new StandardSQLFunction("chr", NHibernateUtil.Character));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("ucase", new StandardSQLFunction("ucase"));
			RegisterFunction("lcase", new StandardSQLFunction("lcase"));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("length", new StandardSQLFunction("len", NHibernateUtil.Int32));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));

			RegisterFunction("trim", new AnsiTrimEmulationFunction());
			RegisterFunction("iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SqlClientDriver";
			DefaultProperties[Environment.PrepareSql] = "false";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		/// <summary></summary>
		public override string NullColumnString
		{
			get { return " null"; }
		}

		/// <summary></summary>
		public override bool QualifyIndexName
		{
			get { return false; }
		}

		/// <summary>
		/// Generates the string to drop the table using SQL Server syntax.
		/// </summary>
		/// <param name="tableName">The name of the table to drop.</param>
		/// <returns>The SQL with the <paramref name="tableName" /> inserted.</returns>
		public override string GetDropTableString(string tableName)
		{
			string dropTable =
				"if exists (select * from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1)" +
				" drop table {0}";

			return String.Format(dropTable, tableName);
		}

		public override string ForUpdateString
		{
			get { return string.Empty; }
		}

		public override SqlString AppendIdentitySelectToInsert(SqlString insertSql)
		{
			return insertSql.Append("; " + IdentitySelectString);
		}

		public override bool SupportsInsertSelectIdentity
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		public override string IdentitySelectString
		{
			get { return "select SCOPE_IDENTITY()"; }
		}

		/// <summary></summary>
		public override string IdentityColumnString
		{
			get { return "IDENTITY NOT NULL"; }
		}

		/// <summary></summary>
		public override string NoColumnsInsertString
		{
			get { return "DEFAULT VALUES"; }
		}

		/// <summary></summary>
		public override char CloseQuote
		{
			get { return ']'; }
		}

		/// <summary></summary>
		public override char OpenQuote
		{
			get { return '['; }
		}

		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>True, we'll use the SELECT TOP nn syntax.</value>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// Does this Dialect support an offset?
		/// </summary>
		public override bool SupportsLimitOffset
		{
			get { return false; }
		}

		/// <summary>
		/// Can parameters be used for a statement containing a LIMIT?
		/// </summary>
		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		/// <summary>
		/// Add a <c>LIMIT (TOP)</c> clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">A Query in the form of a SqlString.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query</param>
		/// <param name="offset">Offset of the first row to process in the result set</param>
		/// <returns>A new SqlString that contains the <c>LIMIT</c> clause.</returns>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			if (offset > 0)
			{
				throw new NotSupportedException("SQL Server does not support an offset");
			}

			/*
			 * "SELECT TOP limit rest-of-sql-statement"
			 */

			return querySqlString.Insert(GetAfterSelectInsertPoint(querySqlString), " top " + limit.ToString());
		}

		/// <summary>
		/// Does the <c>LIMIT</c> clause take a "maximum" row number
		/// instead of a total number of returned rows?
		/// </summary>
		/// <returns>false, unless overridden</returns>
		public override bool UseMaxForLimit
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>
		/// MsSql does not require the OpenQuote to be escaped as long as the first char
		/// is an OpenQuote.
		/// </remarks>
		protected override string Quote(string name)
		{
			return OpenQuote + name.Replace(CloseQuote.ToString(), new string(CloseQuote, 2)) + CloseQuote;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="quoted"></param>
		/// <returns></returns>
		public override string UnQuote(string quoted)
		{
			if (IsQuoted(quoted))
			{
				quoted = quoted.Substring(1, quoted.Length - 2);
			}

			return quoted.Replace(new string(CloseQuote, 2), CloseQuote.ToString());
		}

		private static int GetAfterSelectInsertPoint(SqlString sql)
		{
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

		private bool NeedsLockHint(LockMode lockMode)
		{
			return lockMode.GreaterThan(LockMode.Read);
		}

		public override string AppendLockHint(LockMode lockMode, string tableName)
		{
			if (NeedsLockHint(lockMode))
			{
				return tableName + " with (updlock, rowlock)";
			}

			return tableName;
		}

		private struct LockHintAppender
		{
			private MsSql2000Dialect dialect;
			private IDictionary aliasedLockModes;

			public LockHintAppender(MsSql2000Dialect dialect, IDictionary aliasedLockModes)
			{
				this.dialect = dialect;
				this.aliasedLockModes = aliasedLockModes;
			}

			public string ReplaceMatch(Match match)
			{
				string alias = match.Groups[1].Value;
				string lockHint = dialect.AppendLockHint((LockMode)aliasedLockModes[alias], alias);
				return string.Concat(" ", lockHint, match.Groups[2].Value);
			}
		}

		public override SqlString ApplyLocksToSql(SqlString sql, IDictionary aliasedLockModes, IDictionary keyColumnNames)
		{
			bool doWork = false;

			foreach (DictionaryEntry de in aliasedLockModes)
			{
				if (NeedsLockHint((LockMode)de.Value))
				{
					doWork = true;
					break;
				}
			}

			if (!doWork)
			{
				return sql;
			}

			// Regex matching any alias out of those given. Aliases should contain
			// no dangerous characters (they are identifiers) so they are not escaped.
			string aliasesPattern = StringHelper.Join("|", aliasedLockModes.Keys);

			// Match < alias >, < alias,>, or < alias$>, the intent is to capture alias names
			// in various kinds of "FROM table1 alias1, table2 alias2".
			Regex matchRegex = new Regex(" (" + aliasesPattern + ")([, ]|$)");

			SqlStringBuilder result = new SqlStringBuilder();
			MatchEvaluator evaluator = new MatchEvaluator(new LockHintAppender(this, aliasedLockModes).ReplaceMatch);

			foreach (object part in sql.Parts)
			{
				if (part == Parameter.Placeholder)
				{
					result.AddParameter();
					continue;
				}

				result.Add(matchRegex.Replace((string)part, evaluator));
			}

			return result.ToSqlString();
		}

		public override long TimestampResolutionInTicks
		{
			get
			{
				// MS SQL resolution is actually 3.33 ms, rounded here to 10 ms
				return TimeSpan.TicksPerMillisecond * 10L;
			}
		}

		public override string GetIfExistsDropConstraint(NHibernate.Mapping.Table table, string name)
		{
			string selectExistingObject = GetSelectExistingObject(name, table);
			return string.Format(@"if exists ({0})", selectExistingObject);
		}

		protected virtual string GetSelectExistingObject(string name, Table table)
		{
			string objName = table.GetQuotedSchemaName(this) + this.Quote(name);
			return string.Format("select 1 from sysobjects where id = OBJECT_ID(N'{0}') AND parent_obj = OBJECT_ID('{1}')",
								 objName, table.GetQuotedName(this));
		}

		public override string GetIfNotExistsCreateConstraint(NHibernate.Mapping.Table table, string name)
		{
			string selectExistingObject = GetSelectExistingObject(name, table);
			return string.Format(@"if not exists ({0})", selectExistingObject);
		}

		protected class CountBigQueryFunction : ClassicAggregateFunction
		{
			public CountBigQueryFunction()
				: base("count_big", true)
			{
			}

			public override IType ReturnType(IType columnType, IMapping mapping)
			{
				return NHibernateUtil.Int64;
			}
		}

		public override bool SupportsCircularCascadeDeleteConstraints
		{
			get
			{
				// SQL Server (at least up through 2005) does not support defining
				// cascade delete constraints which can circel back to the mutating
				// table
				return false;
			}
		}
	}
}