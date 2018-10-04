using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect for SQLite.
	/// </summary>
	/// <remarks>
	/// <p>
	/// Author: <a href="mailto:ib@stalker.ro"> Ioan Bizau </a>
	/// </p>
	/// </remarks>
	public class SQLiteDialect : Dialect
	{
		/// <summary>
		/// The effective value of the BinaryGuid connection string parameter.
		/// The default value in SQLite is true.
		/// </summary>
		private bool _binaryGuid = true;

		/// <summary>
		/// 
		/// </summary>
		public SQLiteDialect()
		{
			RegisterColumnTypes();
			RegisterFunctions();
			RegisterKeywords();
			RegisterDefaultProperties();
		}

		protected virtual void RegisterColumnTypes()
		{
			RegisterColumnType(DbType.Binary, "BLOB");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INT");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.SByte, "INTEGER");
			RegisterColumnType(DbType.UInt16, "INTEGER");
			RegisterColumnType(DbType.UInt32, "INTEGER");
			RegisterColumnType(DbType.UInt64, "INTEGER");
			RegisterColumnType(DbType.Currency, "NUMERIC");
			RegisterColumnType(DbType.Decimal, "NUMERIC");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Single, "DOUBLE");
			RegisterColumnType(DbType.VarNumeric, "NUMERIC");
			RegisterColumnType(DbType.AnsiString, "TEXT");
			RegisterColumnType(DbType.String, "TEXT");
			RegisterColumnType(DbType.AnsiStringFixedLength, "TEXT");
			RegisterColumnType(DbType.StringFixedLength, "TEXT");

			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Time, "TIME");
			RegisterColumnType(DbType.Boolean, "BOOL");
			// UNIQUEIDENTIFIER is not a SQLite type, but SQLite does not care much, see
			// https://www.sqlite.org/datatype3.html
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
		}

		protected virtual void RegisterFunctions()
		{
			// Using strftime returns 0-padded strings.  '07' <> 7, so it is better to convert to an integer.
			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.Int32, "cast(strftime('%S', ?1) as int)"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.Int32, "cast(strftime('%M', ?1) as int)"));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.Int32, "cast(strftime('%H', ?1) as int)"));
			RegisterFunction("day", new SQLFunctionTemplate(NHibernateUtil.Int32, "cast(strftime('%d', ?1) as int)"));
			RegisterFunction("month", new SQLFunctionTemplate(NHibernateUtil.Int32, "cast(strftime('%m', ?1) as int)"));
			RegisterFunction("year", new SQLFunctionTemplate(NHibernateUtil.Int32, "cast(strftime('%Y', ?1) as int)"));
			// Uses local time like MSSQL and PostgreSQL.
			RegisterFunction("current_timestamp", new SQLFunctionTemplate(NHibernateUtil.LocalDateTime, "datetime(current_timestamp, 'localtime')"));
			RegisterFunction("current_utctimestamp", new SQLFunctionTemplate(NHibernateUtil.UtcDateTime, "datetime(current_timestamp)"));
			// The System.Data.SQLite driver stores both Date and DateTime as 'YYYY-MM-DD HH:MM:SS'
			// The SQLite date() function returns YYYY-MM-DD, which unfortunately SQLite does not consider
			// as equal to 'YYYY-MM-DD 00:00:00'.  Because of this, it is best to return the
			// 'YYYY-MM-DD 00:00:00' format for the date function.
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "datetime(date(?1))"));
			// SQLite has current_date, but as current_timestamp, it is in UTC. So converting the timestamp to
			// localtime then to date then, like the above date function, go back to datetime format for comparisons
			// sake.
			RegisterFunction("current_date", new SQLFunctionTemplate(NHibernateUtil.LocalDate, "datetime(date(current_timestamp, 'localtime'))"));

			RegisterFunction("substring", new StandardSQLFunction("substr", NHibernateUtil.String));
			RegisterFunction("left", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1,1,?2)"));
			RegisterFunction("trim", new AnsiTrimEmulationFunction());
			RegisterFunction("replace", new StandardSafeSQLFunction("replace", NHibernateUtil.String, 3));
			RegisterFunction("chr", new StandardSQLFunction("char", NHibernateUtil.Character));

			RegisterFunction("mod", new SQLFunctionTemplate(NHibernateUtil.Int32, "((?1) % (?2))"));

			RegisterFunction("iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));

			RegisterFunction("cast", new SQLiteCastFunction());

			RegisterFunction("round", new StandardSQLFunction("round"));

			// SQLite has no built-in support of bitwise xor, but can emulate it.
			// http://sqlite.1065341.n5.nabble.com/XOR-operator-td98004.html
			RegisterFunction("bxor", new SQLFunctionTemplate(null, "((?1 | ?2) - (?1 & ?2))"));

			// NH-3787: SQLite requires the cast in SQL too for not defaulting to string.
			RegisterFunction("transparentcast", new CastFunction());

			if (_binaryGuid)
				RegisterFunction("strguid", new SQLFunctionTemplate(NHibernateUtil.String, "substr(hex(?1), 7, 2) || substr(hex(?1), 5, 2) || substr(hex(?1), 3, 2) || substr(hex(?1), 1, 2) || '-' || substr(hex(?1), 11, 2) || substr(hex(?1), 9, 2) || '-' || substr(hex(?1), 15, 2) || substr(hex(?1), 13, 2) || '-' || substr(hex(?1), 17, 4) || '-' || substr(hex(?1), 21) "));
			else
				RegisterFunction("strguid", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as char)"));
		}

		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			ConfigureBinaryGuid(settings);

			// Re-register functions depending on settings.
			RegisterFunctions();
		}

		private void ConfigureBinaryGuid(IDictionary<string, string> settings)
		{
			// We can use a SQLite specific setting to force it, but in common cases it
			// should be detected automatically from the connection string below.
			settings.TryGetValue(Cfg.Environment.SqliteBinaryGuid, out var strBinaryGuid);

			if (string.IsNullOrWhiteSpace(strBinaryGuid))
			{
				string connectionString = Cfg.Environment.GetConfiguredConnectionString(settings);
				if (!string.IsNullOrWhiteSpace(connectionString))
				{
					var builder = new DbConnectionStringBuilder {ConnectionString = connectionString};

					strBinaryGuid = GetConnectionStringProperty(builder, "BinaryGuid");
				}
			}

			// Note that "BinaryGuid=false" is supported by System.Data.SQLite but not Microsoft.Data.Sqlite.

			_binaryGuid = string.IsNullOrWhiteSpace(strBinaryGuid) || bool.Parse(strBinaryGuid);
		}

		private string GetConnectionStringProperty(DbConnectionStringBuilder builder, string propertyName)
		{
			builder.TryGetValue(propertyName, out object propertyValue);
			return (string) propertyValue;
		}

		#region private static readonly string[] DialectKeywords = { ... }

		private static readonly string[] DialectKeywords =
		{
			"abort",
			"action",
			"after",
			"analyze",
			"asc",
			"attach",
			"autoincrement",
			"before",
			"bit",
			"bool",
			"boolean",
			"cascade",
			"conflict",
			"counter",
			"currency",
			"database",
			"datetime",
			"deferrable",
			"deferred",
			"desc",
			"detach",
			"exclusive",
			"explain",
			"fail",
			"general",
			"glob",
			"guid",
			"ignore",
			"image",
			"index",
			"indexed",
			"initially",
			"instead",
			"isnull",
			"key",
			"limit",
			"logical",
			"long",
			"longtext",
			"memo",
			"money",
			"note",
			"nothing",
			"notnull",
			"ntext",
			"nvarchar",
			"offset",
			"oleobject",
			"plan",
			"pragma",
			"query",
			"raise",
			"regexp",
			"reindex",
			"rename",
			"replace",
			"restrict",
			"single",
			"smalldate",
			"smalldatetime",
			"smallmoney",
			"sql_variant",
			"string",
			"temp",
			"temporary",
			"text",
			"tinyint",
			"transaction",
			"uniqueidentifier",
			"vacuum",
			"varbinary",
			"view",
			"virtual",
			"yesno",
		};

		#endregion

		protected virtual void RegisterKeywords()
		{
			RegisterKeywords(DialectKeywords);
		}

		protected virtual void RegisterDefaultProperties()
		{
			DefaultProperties[Cfg.Environment.ConnectionDriver] = "NHibernate.Driver.SQLite20Driver";
			DefaultProperties[Cfg.Environment.QuerySubstitutions] = "true 1, false 0, yes 'Y', no 'N'";
		}

		public override Schema.IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new Schema.SQLiteDataBaseMetaData(connection, this);
		}

		public override string AddColumnString
		{
			get
			{
				return "add column";
			}
		}

		public override string IdentitySelectString
		{
			get { return "select last_insert_rowid()"; }
		}

		public override SqlString AppendIdentitySelectToInsert(SqlString insertSql)
		{
			return insertSql.Append("; " + IdentitySelectString);
		}

		public override bool SupportsInsertSelectIdentity
		{
			get { return true; }
		}

		public override bool DropConstraints
		{
			get { return false; }
		}

		public override string ForUpdateString
		{
			get { return string.Empty; }
		}

		public override bool SupportsSubSelects
		{
			get { return true; }
		}

		public override bool SupportsIfExistsBeforeTableName
		{
			get { return true; }
		}

		public override bool HasDataTypeInIdentityColumn
		{
			get { return false; }
		}

		public override bool SupportsIdentityColumns
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

		public override string IdentityColumnString
		{
			get
			{
				// Adding the "autoincrement" keyword ensures that the same id will
				// not be generated twice.  When just utilizing "integer primary key",
				// SQLite just takes the max value currently in the table and adds one.
				// This causes problems with caches that use primary keys of deleted
				// entities.
				return "integer primary key autoincrement";
			}
		}

		public override bool GenerateTablePrimaryKeyConstraintForIdentityColumn
		{
			get { return false; }
		}

		public override string Qualify(string catalog, string schema, string table)
		{
			StringBuilder qualifiedName = new StringBuilder();
			bool quoted = false;
			
			if (!string.IsNullOrEmpty(catalog))
			{
				if (catalog.StartsWith(OpenQuote))
				{
					catalog = catalog.Substring(1, catalog.Length - 1);
					quoted = true;
				} 
				if (catalog.EndsWith(CloseQuote))
				{
					catalog = catalog.Substring(0, catalog.Length - 1);
					quoted = true;
				}
				qualifiedName.Append(catalog).Append(StringHelper.Underscore);
			}
			if (!string.IsNullOrEmpty(schema))
			{
				if (schema.StartsWith(OpenQuote))
				{
					schema = schema.Substring(1, schema.Length - 1);
					quoted = true;
				}
				if (schema.EndsWith(CloseQuote))
				{
					schema = schema.Substring(0, schema.Length - 1);
					quoted = true;
				} 
				qualifiedName.Append(schema).Append(StringHelper.Underscore);
			}

			if (table.StartsWith(OpenQuote))
			{
				table = table.Substring(1, table.Length - 1);
				quoted = true;
			}
			if (table.EndsWith(CloseQuote))
			{
				table = table.Substring(0, table.Length - 1);
				quoted = true;
			}

			string name = qualifiedName.Append(table).ToString();
			if (quoted)
				return OpenQuote + name + CloseQuote;
			return name;
		}

		public override string NoColumnsInsertString
		{
			get { return "DEFAULT VALUES"; }
		}

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(queryString);

			pagingBuilder.Add(" limit ");
			if (limit != null)
				pagingBuilder.Add(limit);
			else
				// We must have a limit present if we have an offset.
				pagingBuilder.Add(int.MaxValue.ToString());

			if (offset != null)
			{
				pagingBuilder.Add(" offset ");
				pagingBuilder.Add(offset);
			}

			return pagingBuilder.ToSqlString();
		}

		public override bool SupportsTemporaryTables
		{
			get { return true; }
		}

		public override string CreateTemporaryTableString
		{
			get { return "create temp table"; }
		}

		public override bool DropTemporaryTableAfterUse()
		{
			return true;
		}

		public override string SelectGUIDString
		{
			get { return "select randomblob(16)"; }
		}

		/// <summary>
		/// SQLite does not currently support dropping foreign key constraints by alter statements.
		/// This means that tables cannot be dropped if there are any rows that depend on those.
		/// If there are cycles between tables, it would even be excessively difficult to delete
		/// the data in the right order first.  Because of this, we just turn off the foreign
		/// constraints before we drop the schema and hope that we're not going to break anything. :(
		/// We could theoretically check for data consistency afterwards, but we don't currently.
		/// </summary>
		public override string DisableForeignKeyConstraintsString
		{
			get { return "PRAGMA foreign_keys = OFF"; }
		}

		public override string EnableForeignKeyConstraintsString
		{
			get { return "PRAGMA foreign_keys = ON"; }
		}

		public override bool SupportsForeignKeyConstraintInAlterTable
		{
			get { return false; }
		}

		/// <summary>
		/// Does this dialect support concurrent writing connections?
		/// </summary>
		/// <remarks>
		/// As documented at https://www.sqlite.org/faq.html#q5
		/// </remarks>
		public override bool SupportsConcurrentWritingConnections => false;

		/// <summary>
		/// Does this dialect supports distributed transaction? <c>false</c>.
		/// </summary>
		/// <remarks>
		/// SQLite does not have a two phases commit and as such does not respect distributed transaction semantic.
		/// But moreover, it fails handling the threading involved with distributed transactions (see
		/// https://system.data.sqlite.org/index.html/tktview/5cee5409f84da5f62172 ).
		/// It has moreover some flakyness in tests due to seemingly highly delayed (> 500ms) commits when distributed. 
		/// </remarks>
		public override bool SupportsDistributedTransactions => false;

		// Said to be unlimited. http://sqlite.1065341.n5.nabble.com/Max-limits-on-the-following-td37859.html
		/// <inheritdoc />
		public override int MaxAliasLength => 128;

		[Serializable]
		protected class SQLiteCastFunction : CastFunction
		{
			protected override bool CastingIsRequired(string sqlType)
			{
				// SQLite doesn't support casting to datetime types.  It assumes you want an integer and destroys the date string.
				if (StringHelper.ContainsCaseInsensitive(sqlType, "date") || StringHelper.ContainsCaseInsensitive(sqlType, "time"))
					return false;
				return true;
			}
		}
	}
}
