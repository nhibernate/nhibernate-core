using System;
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
			RegisterFunction("current_timestamp", new SQLFunctionTemplate(NHibernateUtil.DateTime, "datetime(current_timestamp, 'localtime')"));
			// The System.Data.SQLite driver stores both Date and DateTime as 'YYYY-MM-DD HH:MM:SS'
			// The SQLite date() function returns YYYY-MM-DD, which unfortunately SQLite does not consider
			// as equal to 'YYYY-MM-DD 00:00:00'.  Because of this, it is best to return the
			// 'YYYY-MM-DD 00:00:00' format for the date function.
			RegisterFunction("date", new SQLFunctionTemplate(NHibernateUtil.Date, "datetime(date(?1))"));

			RegisterFunction("substring", new StandardSQLFunction("substr", NHibernateUtil.String));
			RegisterFunction("left", new SQLFunctionTemplate(NHibernateUtil.String, "substr(?1,1,?2)"));
			RegisterFunction("trim", new AnsiTrimEmulationFunction());
			RegisterFunction("replace", new StandardSafeSQLFunction("replace", NHibernateUtil.String, 3));

			RegisterFunction("mod", new SQLFunctionTemplate(NHibernateUtil.Int32, "((?1) % (?2))"));

			RegisterFunction("iif", new SQLFunctionTemplate(null, "case when ?1 then ?2 else ?3 end"));

			RegisterFunction("cast", new SQLiteCastFunction());

			RegisterFunction("round", new StandardSQLFunction("round"));
		}

		protected virtual void RegisterKeywords()
		{
			RegisterKeyword("int"); // Used in our function templates.
		}

		protected virtual void RegisterDefaultProperties()
		{
			DefaultProperties[Cfg.Environment.ConnectionDriver] = "NHibernate.Driver.SQLite20Driver";
			DefaultProperties[Cfg.Environment.QuerySubstitutions] = "true 1, false 0, yes 'Y', no 'N'";
		}

		public override Schema.IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new Schema.SQLiteDataBaseMetaData(connection);
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
				if (catalog.StartsWith(OpenQuote.ToString()))
				{
					catalog = catalog.Substring(1, catalog.Length - 1);
					quoted = true;
				} 
				if (catalog.EndsWith(CloseQuote.ToString()))
				{
					catalog = catalog.Substring(0, catalog.Length - 1);
					quoted = true;
				}
				qualifiedName.Append(catalog).Append(StringHelper.Underscore);
			}
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
				qualifiedName.Append(schema).Append(StringHelper.Underscore);
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

		[Serializable]
		protected class SQLiteCastFunction : CastFunction
		{
			protected override bool CastingIsRequired(string sqlType)
			{
				// SQLite doesn't support casting to datetime types.  It assumes you want an integer and destroys the date string.
				if (sqlType.ToLowerInvariant().Contains("date") || sqlType.ToLowerInvariant().Contains("time"))
					return false;
				return true;
			}
		}
	}
}