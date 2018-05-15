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
	/// An SQL dialect for PostgreSQL.
	/// </summary>
	/// <remarks>
	/// The PostgreSQLDialect defaults the following configuration properties:
	/// <list type="table">
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.NpgsqlDriver" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public class PostgreSQLDialect : Dialect
	{
		public PostgreSQLDialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.NpgsqlDriver";

			RegisterDateTimeTypeMappings();
			RegisterColumnType(DbType.AnsiStringFixedLength, "char(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "char($l)");
			RegisterColumnType(DbType.AnsiString, "varchar(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "varchar($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "text");
			RegisterColumnType(DbType.Binary, "bytea");
			RegisterColumnType(DbType.Binary, 2147483647, "bytea");
			RegisterColumnType(DbType.Boolean, "boolean");
			RegisterColumnType(DbType.Byte, "int2");
			RegisterColumnType(DbType.Currency, "decimal(18,4)");
			RegisterColumnType(DbType.Decimal, "decimal(19,5)");
			// PostgreSQL max precision is unlimited, but .Net is limited to 28-29.
			RegisterColumnType(DbType.Decimal, 29, "decimal($p, $s)");
			RegisterColumnType(DbType.Double, "float8");
			RegisterColumnType(DbType.Int16, "int2");
			RegisterColumnType(DbType.Int32, "int4");
			RegisterColumnType(DbType.Int64, "int8");
			RegisterColumnType(DbType.Single, "float4");
			RegisterColumnType(DbType.StringFixedLength, "char(255)");
			RegisterColumnType(DbType.StringFixedLength, 4000, "char($l)");
			RegisterColumnType(DbType.String, "varchar(255)");
			RegisterColumnType(DbType.String, 4000, "varchar($l)");
			RegisterColumnType(DbType.String, 1073741823, "text");

			// Override standard HQL function
			RegisterFunction("current_timestamp", new NoArgSQLFunction("now", NHibernateUtil.DateTime, true));
			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as varchar)"));
			RegisterFunction("locate", new PositionSubstringFunction());
			RegisterFunction("iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));
			RegisterFunction("replace", new StandardSQLFunction("replace", NHibernateUtil.String));
			RegisterFunction("left", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1,1,?2)"));
			RegisterFunction("mod", new SQLFunctionTemplate(NHibernateUtil.Int32, "((?1) % (?2))"));

			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));
			RegisterFunction("round", new RoundFunction(false));
			RegisterFunction("truncate", new RoundFunction(true));
			RegisterFunction("trunc", new RoundFunction(true));

			// Trigonometric functions.
			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("atan2", new StandardSQLFunction("atan2", NHibernateUtil.Double));

			RegisterFunction("power", new StandardSQLFunction("power", NHibernateUtil.Double));
			RegisterFunction("bxor", new Function.BitwiseNativeOperation("#"));

			RegisterFunction("floor", new StandardSQLFunction("floor"));
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("chr", new StandardSQLFunction("chr", NHibernateUtil.Character));
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));

			// Register the date function, since when used in LINQ select clauses, NH must know the data type.
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "cast(?1 as date)"));

			RegisterKeywords();
		}

		#region private static readonly string[] DialectKeywords = { ... }

		private static readonly string[] DialectKeywords =
		{
			"analyse",
			"analyze",
			"asc",
			"concurrently",
			"current_catalog",
			"current_schema",
			"deferrable",
			"desc",
			"freeze",
			"ilike",
			"initially",
			"isnull",
			"limit",
			"notnull",
			"offset",
			"placing",
			"returning",
			"session_user",
			"variadic",
			"verbose",
		};

		#endregion

		protected virtual void RegisterDateTimeTypeMappings()
		{
			RegisterColumnType(DbType.Date, "date");
			RegisterColumnType(DbType.DateTime, "timestamp");
			RegisterColumnType(DbType.Time, "time");
		}

		protected virtual void RegisterKeywords()
		{
			RegisterKeywords(DialectKeywords);
		}

		public override string AddColumnString
		{
			get { return "add column"; }
		}

		public override bool DropConstraints
		{
			get { return false; }
		}

		public override string CascadeConstraintsString
		{
			get { return " cascade"; }
		}

		public override string GetSequenceNextValString(string sequenceName)
		{
			return string.Concat("select ",GetSelectSequenceNextValString(sequenceName));
		}

		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return string.Concat("nextval ('", sequenceName, "')");
		}

		public override string GetCreateSequenceString(string sequenceName)
		{
			return "create sequence " + sequenceName;
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			return "drop sequence " + sequenceName;
		}

		public override SqlString AddIdentifierOutParameterToInsert(SqlString insertString, string identifierColumnName, string parameterName)
		{
			return insertString.Append(" returning " + identifierColumnName);
		}

		public override InsertGeneratedIdentifierRetrievalMethod InsertGeneratedIdentifierRetrievalMethod
		{
			get { return InsertGeneratedIdentifierRetrievalMethod.OutputParameter; }
		}

		public override bool SupportsSequences
		{
			get { return true; }
		}

		/// <summary>
		/// Supported with SQL 2003 syntax since 7.4, released 2003-11-17. For older versions
		/// we need to override GetCreateSequenceString(string, int, int) and provide alternative
		/// syntax, but I don't think we need to bother for such ancient releases (considered EOL).
		/// </summary>
		public override bool SupportsPooledSequences
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
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(queryString);

			if (limit != null)
			{
				pagingBuilder.Add(" limit ");
				pagingBuilder.Add(limit);
			}

			if (offset != null)
			{
				pagingBuilder.Add(" offset ");
				pagingBuilder.Add(offset);
			}

			return pagingBuilder.ToSqlString();
		}

		/// <inheritdoc />
		public override bool SupportsForUpdateOf => true;

		/// <inheritdoc />
		public override bool SupportsOuterJoinForUpdate => false;

		public override string GetForUpdateString(string aliases)
		{
			return ForUpdateString + " of " + aliases;
		}

		/// <summary>PostgreSQL supports UNION ALL clause</summary>
		/// <remarks>
		/// Reference: <see href="http://www.postgresql.org/docs/8.0/static/sql-select.html#SQL-UNION">
		/// PostgreSQL 8.0 UNION Clause documentation</see>
		/// </remarks>
		/// <value><see langword="true"/></value>
		public override bool SupportsUnionAll
		{
			get { return true; }
		}

		/// <summary>PostgreSQL requires to cast NULL values to correctly handle UNION/UNION ALL</summary>
		/// <remarks>
		/// See <see href="http://archives.postgresql.org/pgsql-bugs/2005-08/msg00239.php">
		/// PostgreSQL BUG #1847: Error in some kind of UNION query.</see>
		/// </remarks>
		/// <param name="sqlType">The <see cref="DbType"/> type code.</param>
		/// <returns>null casted as <paramref name="sqlType"/>: "<c>null::sqltypename</c>"</returns>
		public override string GetSelectClauseNullString(SqlType sqlType)
		{
			//This will cast 'null' with the full SQL type name, including eventual parameters.
			//It shouldn't have any influence, but note that it's not mandatory.
			//i.e. 'null::decimal(19, 2)', even if 'null::decimal' would be enough
			return "null::" + GetTypeName(sqlType);
		}

		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		public override string CreateTemporaryTableString
		{
			get { return "create temporary table"; }
		}

		public override string CreateTemporaryTablePostfix
		{
			get { return "on commit drop"; }
		}

		public override string ToBooleanValueString(bool value)
		{
			return value ? "TRUE" : "FALSE";
		}

		public override string SelectGUIDString
		{
			get { return "select uuid_generate_v4()"; }
		}
		
		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new PostgreSQLDataBaseMetadata(connection);
		}

		public override long TimestampResolutionInTicks
		{
			get { return 10L; }   // Microseconds.
		}

		public override bool SupportsCurrentTimestampSelection
		{
			get { return true; }
		}

		public override string CurrentTimestampSelectString
		{
			get { return "SELECT CURRENT_TIMESTAMP"; }
		}

		#region Overridden informational metadata

		public override bool SupportsEmptyInList => false;

		/// <summary> 
		/// Should LOBs (both BLOB and CLOB) be bound using stream operations (i.e.
		/// {@link java.sql.PreparedStatement#setBinaryStream}). 
		/// </summary>
		/// <returns> True if BLOBs and CLOBs should be bound using stream operations. </returns>
		public override bool UseInputStreamToInsertBlob => false;

		public override bool SupportsLobValueChangePropogation => false;

		public override bool SupportsUnboundedLobLocatorMaterialization => false;

		#endregion

		[Serializable]
		private class RoundFunction : ISQLFunction
		{
			private static readonly ISQLFunction Round = new StandardSQLFunction("round");
			private static readonly ISQLFunction Truncate = new StandardSQLFunction("trunc");

			// PostgreSQL round/trunc with two arguments only accepts decimal as input, thus the cast.
			// It also yields only decimal, but for emulating similar behavior to other databases, we need
			// to have it converted to the original input type, which will be done by NHibernate thanks to
			// not specifying the function type.
			private static readonly ISQLFunction RoundWith2Params = new SQLFunctionTemplate(null, "round(cast(?1 as numeric), ?2)");
			private static readonly ISQLFunction TruncateWith2Params = new SQLFunctionTemplate(null, "trunc(cast(?1 as numeric), ?2)");

			private readonly ISQLFunction _singleParamFunction;
			private readonly ISQLFunction _twoParamFunction;
			private readonly string _name;

			public RoundFunction(bool truncate)
			{
				if (truncate)
				{
					_singleParamFunction = Truncate;
					_twoParamFunction = TruncateWith2Params;
					_name = "truncate";
				}
				else
				{
					_singleParamFunction = Round;
					_twoParamFunction = RoundWith2Params;
					_name = "round";
				}
			}

			public IType ReturnType(IType columnType, IMapping mapping) => columnType;

			public bool HasArguments => true;

			public bool HasParenthesesIfNoArguments => true;

			public SqlString Render(IList args, ISessionFactoryImplementor factory)
			{
				return args.Count == 2 ? _twoParamFunction.Render(args, factory) : _singleParamFunction.Render(args, factory);
			}

			public override string ToString() => _name;
		}
	}
}
