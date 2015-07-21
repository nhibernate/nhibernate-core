using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.SqlCommand.Parser;
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
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.SqlClientDriver" /></description>
	///	</item>
	///	<item>
	///		<term>adonet.batch_size</term>
	///		<description>10</description>
	///	</item>
	///	<item>
	///		<term>query.substitutions</term>
	///		<description>true 1, false 0, yes 'Y', no 'N'</description>
	///	</item>
	/// </list>
	/// </remarks>
	public class MsSql2000Dialect : Dialect
	{
		public MsSql2000Dialect()
		{
			RegisterCharacterTypeMappings();
			RegisterNumericTypeMappings();
			RegisterDateTimeTypeMappings();
			RegisterLargeObjectTypeMappings();
			RegisterGuidTypeMapping();

			RegisterFunctions();

			RegisterKeywords();

			RegisterDefaultProperties();
		}

		protected virtual void RegisterDefaultProperties()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SqlClientDriver";
			DefaultProperties[Environment.BatchSize] = "20";
			DefaultProperties[Environment.QuerySubstitutions] = "true 1, false 0, yes 'Y', no 'N'";
		}

		protected virtual void RegisterKeywords()
		{
			RegisterKeyword("top");
			RegisterKeyword("int");
			RegisterKeyword("integer"); // a commonly-used alias for 'int'
			RegisterKeyword("tinyint");
			RegisterKeyword("smallint");
			RegisterKeyword("bigint");
			RegisterKeyword("numeric");
			RegisterKeyword("decimal");
			RegisterKeyword("bit");
			RegisterKeyword("money");
			RegisterKeyword("smallmoney");
			RegisterKeyword("float");
			RegisterKeyword("real");
			RegisterKeyword("datetime");
			RegisterKeyword("smalldatetime");
			RegisterKeyword("char");
			RegisterKeyword("varchar");
			RegisterKeyword("text");
			RegisterKeyword("nchar");
			RegisterKeyword("nvarchar");
			RegisterKeyword("ntext");
			RegisterKeyword("binary");
			RegisterKeyword("varbinary");
			RegisterKeyword("image");
			RegisterKeyword("cursor");
			RegisterKeyword("timestamp");
			RegisterKeyword("uniqueidentifier");
			RegisterKeyword("sql_variant");
			RegisterKeyword("table");
		}

		protected virtual void RegisterFunctions()
		{
			RegisterFunction("count", new CountBigQueryFunction());

			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("absval", new StandardSQLFunction("absval"));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));

			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));
			RegisterFunction("round", new StandardSQLFunction("round"));

			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));

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
			RegisterFunction("mod", new SQLFunctionTemplate(NHibernateUtil.Int32, "((?1) % (?2))"));
			RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
			RegisterFunction("rand", new NoArgSQLFunction("rand", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("soundex", new StandardSQLFunction("soundex", NHibernateUtil.String));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));

			RegisterFunction("left", new SQLFunctionTemplate(NHibernateUtil.String, "left(?1, ?2)"));
			RegisterFunction("right", new SQLFunctionTemplate(NHibernateUtil.String, "right(?1, ?2)"));
			RegisterFunction("locate", new StandardSQLFunction("charindex", NHibernateUtil.Int32));

			RegisterFunction("current_timestamp", new NoArgSQLFunction("getdate", NHibernateUtil.DateTime, true));
			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(second, ?1)"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(minute, ?1)"));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(hour, ?1)"));
			RegisterFunction("day", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(day, ?1)"));
			RegisterFunction("month", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(month, ?1)"));
			RegisterFunction("year", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(year, ?1)"));
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "dateadd(dd, 0, datediff(dd, 0, ?1))"));
			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "+", ")"));
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
			RegisterFunction("replace", new StandardSafeSQLFunction("replace", NHibernateUtil.String, 3));

			// Casting to CHAR (without specified length) truncates to 30 characters. 
			// A longer version would be safer, but 50 is enough to prevent errors when casting uniqueidentifer to a string representation (NH-2858)
			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as nvarchar(50))"));

			RegisterFunction("substring", new EmulatedLengthSubstringFunction());

			RegisterFunction("bit_length", new SQLFunctionTemplate(NHibernateUtil.Int32, "datalength(?1) * 8"));
			RegisterFunction("extract", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(?1, ?3)"));
		}

		protected virtual void RegisterGuidTypeMapping()
		{
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
		}

		protected virtual void RegisterLargeObjectTypeMappings()
		{
			RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
			RegisterColumnType(DbType.Binary, SqlClientDriver.MaxSizeForLengthLimitedBinary, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, SqlClientDriver.MaxSizeForBlob, "IMAGE");
		}

		protected virtual void RegisterDateTimeTypeMappings()
		{
			RegisterColumnType(DbType.Time, "DATETIME");
			RegisterColumnType(DbType.Date, "DATETIME");
			RegisterColumnType(DbType.DateTime, "DATETIME");
		}

		protected virtual void RegisterNumericTypeMappings()
		{
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "DECIMAL($p, $s)");
			RegisterColumnType(DbType.Double, "FLOAT(53)");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INT");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "REAL"); //synonym for FLOAT(24)
		}

		protected virtual void RegisterCharacterTypeMappings()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, SqlClientDriver.MaxSizeForLengthLimitedAnsiString, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, SqlClientDriver.MaxSizeForAnsiClob, "TEXT");
			RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, SqlClientDriver.MaxSizeForLengthLimitedString, "NCHAR($l)");
			RegisterColumnType(DbType.String, "NVARCHAR(255)");
			RegisterColumnType(DbType.String, SqlClientDriver.MaxSizeForLengthLimitedString, "NVARCHAR($l)");
			RegisterColumnType(DbType.String, SqlClientDriver.MaxSizeForClob, "NTEXT");
		}

		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override string NullColumnString
		{
			get { return " null"; }
		}

		public override string CurrentTimestampSQLFunctionName
		{
			get { return "CURRENT_TIMESTAMP"; }
		}

		public override string CurrentTimestampSelectString
		{
			get { return "SELECT CURRENT_TIMESTAMP"; }
		}

		public override bool IsCurrentTimestampSelectStringCallable
		{
			get { return false; }
		}

		public override bool SupportsCurrentTimestampSelection
		{
			get { return true; }
		}

		public override bool QualifyIndexName
		{
			get { return false; }
		}

		public override string SelectGUIDString
		{
			get { return "select newid()"; }
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

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		public override string IdentitySelectString
		{
			get { return "select SCOPE_IDENTITY()"; }
		}

		public override string IdentityColumnString
		{
			get { return "IDENTITY NOT NULL"; }
		}

		public override string NoColumnsInsertString
		{
			get { return "DEFAULT VALUES"; }
		}

		public override char CloseQuote
		{
			get { return ']'; }
		}

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

		public override SqlString GetLimitString(SqlString querySqlString, SqlString offset, SqlString limit)
		{
			var tokenEnum = new SqlTokenizer(querySqlString).GetEnumerator();
			if (!tokenEnum.TryParseUntilFirstMsSqlSelectColumn()) return null;

			int insertPoint = tokenEnum.Current.SqlIndex;
			return querySqlString.Insert(insertPoint, new SqlString("top ", limit, " "));
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

		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		public override string GenerateTemporaryTableName(string baseTableName)
		{
			return "#" + baseTableName;
		}

		public override bool DropTemporaryTableAfterUse()
		{
			return true;
		}

		/// <summary />
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

		public override string UnQuote(string quoted)
		{
			if (IsQuoted(quoted))
			{
				quoted = quoted.Substring(1, quoted.Length - 2);
			}

			return quoted.Replace(new string(CloseQuote, 2), CloseQuote.ToString());
		}

		protected bool NeedsLockHint(LockMode lockMode)
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

		public override SqlString ApplyLocksToSql(SqlString sql, IDictionary<string, LockMode> aliasedLockModes, IDictionary<string, string[]> keyColumnNames)
		{
			bool doWork = false;

			foreach (KeyValuePair<string, LockMode> de in aliasedLockModes)
			{
				if (NeedsLockHint(de.Value))
				{
					doWork = true;
					break;
				}
			}

			if (!doWork)
			{
				return sql;
			}

			return new LockHintAppender(this, aliasedLockModes).AppendLockHint(sql);
		}

		public override long TimestampResolutionInTicks
		{
			get
			{
				// MS SQL resolution is actually 3.33 ms, rounded here to 10 ms
				return TimeSpan.TicksPerMillisecond * 10L;
			}
		}

		public override string GetIfExistsDropConstraint(Table table, string name)
		{
			string selectExistingObject = GetSelectExistingObject(name, table);
			return string.Format(@"if exists ({0})", selectExistingObject);
		}

		protected virtual string GetSelectExistingObject(string name, Table table)
		{
			string objName = table.GetQuotedSchemaName(this) + Quote(name);
			return string.Format("select 1 from sysobjects where id = OBJECT_ID(N'{0}') AND parent_obj = OBJECT_ID('{1}')",
								 objName, table.GetQuotedName(this));
		}

		public override string GetIfNotExistsCreateConstraint(Table table, string name)
		{
			string selectExistingObject = GetSelectExistingObject(name, table);
			return string.Format(@"if not exists ({0})", selectExistingObject);
		}
		
		[Serializable]
		protected class CountBigQueryFunction : ClassicAggregateFunction
		{
			public CountBigQueryFunction() : base("count_big", true) { }

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
				// cascade delete constraints which can circle back to the mutating
				// table
				return false;
			}
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new MsSqlDataBaseSchema(connection);
		}

		public override bool SupportsUnionAll
		{
			get { return true; }
		}

		public override bool SupportsSqlBatches
		{
			get { return true; }
		}

		public override bool IsKnownToken(string currentToken, string nextToken)
		{
			return currentToken == "n" && nextToken == "'"; // unicode character
		}

		public struct LockHintAppender
		{
			private const string UnescapedNameRegex = @"\w+";
			private const string EscapedNameRegex = @"\[([^\]]|\]\])+\]";
			private const string NameRegex = "(" + UnescapedNameRegex + "|" + EscapedNameRegex + ")";
			private const string NameSeparatorRegex = @"\s*\.\s*";
			private const string FromTableNameRegex = @"from\s+(" + NameRegex + "?" + NameSeparatorRegex + "){0,2}" + NameRegex;

			private static readonly Regex FromClauseTableNameRegex = new Regex(FromTableNameRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);

			private readonly MsSql2000Dialect _dialect;
			private readonly IDictionary<string, LockMode> _aliasedLockModes;

			private readonly Regex _matchRegex;
			private readonly Regex _unionSubclassRegex;

			public LockHintAppender(MsSql2000Dialect dialect, IDictionary<string, LockMode> aliasedLockModes)
			{
				_dialect = dialect;
				_aliasedLockModes = aliasedLockModes;

				// Regex matching any alias out of those given. Aliases should contain
				// no dangerous characters (they are identifiers) so they are not escaped.
				var aliasesPattern = StringHelper.Join("|", aliasedLockModes.Keys);

				// Match < alias >, < alias,>, or < alias$>, the intent is to capture alias names
				// in various kinds of "FROM table1 alias1, table2 alias2".
				_matchRegex = new Regex(" (" + aliasesPattern + ")([, ]|$)");
				_unionSubclassRegex = new Regex(@"from\s+\(((?:.|\r|\n)*)\)(?:\s+as)?\s+(?<alias>" + aliasesPattern + ")", RegexOptions.IgnoreCase | RegexOptions.Multiline);
	}

			public SqlString AppendLockHint(SqlString sql)
			{
				var result = new SqlStringBuilder();

				foreach (object part in sql)
				{
					if (part == Parameter.Placeholder)
					{
						result.Add((Parameter)part);
						continue;
}

					result.Add(ProcessUnionSubclassCase((string)part) ?? _matchRegex.Replace((string)part, ReplaceMatch));
				}

				return result.ToSqlString();
			}

			private string ProcessUnionSubclassCase(string part)
			{
				var unionMatch = _unionSubclassRegex.Match(part);
				if (!unionMatch.Success)
				{
					return null;
				}

				var alias = unionMatch.Groups["alias"].Value;
				var lockMode = _aliasedLockModes[alias];
				var @this = this;
				var replacement = FromClauseTableNameRegex.Replace(unionMatch.Value, m => @this._dialect.AppendLockHint(lockMode, m.Value));

				return _unionSubclassRegex.Replace(part, replacement);
			}

			private string ReplaceMatch(Match match)
			{
				string alias = match.Groups[1].Value;
				string lockHint = _dialect.AppendLockHint(_aliasedLockModes[alias], alias);
				return string.Concat(" ", lockHint, match.Groups[2].Value); // TODO: seems like this line is redundant
			}
		}
	}
}
