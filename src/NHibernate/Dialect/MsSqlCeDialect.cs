using System;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.Id;
using NHibernate.SqlCommand;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A dialect for SQL Server Everywhere (SQL Server CE).
	/// </summary>
	public class MsSqlCeDialect : Dialect
	{
		public MsSqlCeDialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 4000, "NCHAR($l)");
			RegisterColumnType(DbType.AnsiString, "NVARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 4000, "NVARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 1073741823, "NTEXT");
			RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
			RegisterColumnType(DbType.Binary, 8000, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, 1073741823, "IMAGE");
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATETIME");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "NUMERIC($p, $s)");
			RegisterColumnType(DbType.Double, "FLOAT");
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

			RegisterFunction("substring", new EmulatedLengthSubstringFunction());
			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as nvarchar)")); 

			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(dd, 0, datediff(dd, 0, ?1))"));
			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(second, ?1)"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(minute, ?1)"));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(hour, ?1)"));
			RegisterFunction("day", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(day, ?1)"));
			RegisterFunction("month", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(month, ?1)"));
			RegisterFunction("year", new SQLFunctionTemplate(NHibernateUtil.Int32, "datepart(year, ?1)"));

			RegisterFunction("length", new StandardSQLFunction("len", NHibernateUtil.Int32));
			RegisterFunction("locate", new StandardSQLFunction("charindex", NHibernateUtil.Int32));
			RegisterFunction("replace", new StandardSafeSQLFunction("replace", NHibernateUtil.String, 3));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim"));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("lower", new StandardSQLFunction("lower"));

			RegisterFunction("trim", new AnsiTrimEmulationFunction());

			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "+", ")"));

			RegisterFunction("round", new StandardSQLFunction("round"));

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SqlServerCeDriver";
			DefaultProperties[Environment.PrepareSql] = "false";
		}

		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override string NullColumnString
		{
			get { return " null"; }
		}

		public override bool QualifyIndexName
		{
			get { return false; }
		}

		public override string ForUpdateString
		{
			get { return string.Empty; }
		}

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		public override string IdentitySelectString
		{
			get { return "select @@IDENTITY"; }
		}

		public override string IdentityColumnString
		{
			get { return "IDENTITY NOT NULL"; }
		}

		public override string SelectGUIDString
		{
			get { return "select newid()"; }
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsLimitOffset
		{
			get { return false; }
		}

		public override System.Type NativeIdentifierGeneratorClass => typeof(TableHiLoGenerator);

		public override bool SupportsCircularCascadeDeleteConstraints => false;

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new MsSqlCeDataBaseSchema(connection);
		}

		public override SqlString GetLimitString(SqlString querySqlString, SqlString offset, SqlString limit)
		{
			var top = new SqlString(" top (", limit, ")");
			return querySqlString.Insert(GetAfterSelectInsertPoint(querySqlString), top);
		}

		public override string Qualify(string catalog, string schema, string table)
		{
			// SQL Server Compact doesn't support Schemas. So join schema name and table name with underscores
			// similar to the SQLLite dialect.
			
			var qualifiedName = new StringBuilder();
			bool quoted = false;

			if (!string.IsNullOrEmpty(catalog))
			{
				qualifiedName.Append(catalog).Append(StringHelper.Dot);
			}

			var tableName = new StringBuilder();
			if (!string.IsNullOrEmpty(schema))
			{
				if (schema.StartsWith(OpenQuote.ToString()))
				{
					schema = schema.Substring(1, schema.Length - 1);
					quoted = true;
				}
				if (schema.EndsWith(CloseQuote.ToString()))
				{
					schema = schema.Substring(0, schema.Length - 1);
					quoted = true;
				}
				tableName.Append(schema).Append(StringHelper.Underscore);
			}

			if (table.StartsWith(OpenQuote.ToString()))
			{
				table = table.Substring(1, table.Length - 1);
				quoted = true;
			}
			if (table.EndsWith(CloseQuote.ToString()))
			{
				table = table.Substring(0, table.Length - 1);
				quoted = true;
			}

			string name = tableName.Append(table).ToString();
			if (quoted)
				name = OpenQuote + name + CloseQuote;
			return qualifiedName.Append(name).ToString();
		}

		private static int GetAfterSelectInsertPoint(SqlString sql)
		{
			if (sql.StartsWithCaseInsensitive("select distinct"))
			{
				return 15;
			}
			if (sql.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}
			throw new NotSupportedException("The query should start with 'SELECT' or 'SELECT DISTINCT'");
		}

		public override long TimestampResolutionInTicks
		{
			get
			{
				// MS SQL resolution is actually 3.33 ms, rounded here to 10 ms
				return TimeSpan.TicksPerMillisecond*10L;
			}
		}

		#region Informational metadata

		/// <summary>
		/// Does this dialect support pooling parameter in connection string?
		/// </summary>
		public override bool SupportsPoolingParameter => false;

		#endregion
	}
}