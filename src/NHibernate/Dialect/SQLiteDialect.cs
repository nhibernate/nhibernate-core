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
			RegisterColumnType(DbType.Binary, "BLOB");
			RegisterColumnType(DbType.Byte, "INTEGER");
			RegisterColumnType(DbType.Int16, "INTEGER");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "INTEGER");
			RegisterColumnType(DbType.SByte, "INTEGER");
			RegisterColumnType(DbType.UInt16, "INTEGER");
			RegisterColumnType(DbType.UInt32, "INTEGER");
			RegisterColumnType(DbType.UInt64, "INTEGER");
			RegisterColumnType(DbType.Currency, "NUMERIC");
			RegisterColumnType(DbType.Decimal, "NUMERIC");
			RegisterColumnType(DbType.Double, "NUMERIC");
			RegisterColumnType(DbType.Single, "NUMERIC");
			RegisterColumnType(DbType.VarNumeric, "NUMERIC");
			RegisterColumnType(DbType.AnsiString, "TEXT");
			RegisterColumnType(DbType.String, "TEXT");
			RegisterColumnType(DbType.AnsiStringFixedLength, "TEXT");
			RegisterColumnType(DbType.StringFixedLength, "TEXT");

			RegisterColumnType(DbType.Date, "DATETIME"); 
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Time, "DATETIME");
			RegisterColumnType(DbType.Boolean, "INTEGER");
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");

			RegisterFunction("second", new SQLFunctionTemplate(NHibernateUtil.String, "strftime(\"%S\", ?1)"));
			RegisterFunction("minute", new SQLFunctionTemplate(NHibernateUtil.String, "strftime(\"%M\", ?1)"));
			RegisterFunction("hour", new SQLFunctionTemplate(NHibernateUtil.String, "strftime(\"%H\", ?1)"));
			RegisterFunction("day", new SQLFunctionTemplate(NHibernateUtil.String, "strftime(\"%d\", ?1)"));
			RegisterFunction("month", new SQLFunctionTemplate(NHibernateUtil.String, "strftime(\"%m\", ?1)"));
			RegisterFunction("year", new SQLFunctionTemplate(NHibernateUtil.String, "strftime(\"%Y\", ?1)"));
			RegisterFunction("substring", new StandardSQLFunction("substr", NHibernateUtil.String));
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

		public override bool HasAlterTable
		{
			get { return false; }
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

		public override bool SupportsVariableLimit
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

		public override string IdentityColumnString
		{
			get
			{
				// identity columns in sqlite are marked as being integer primary key
				// the primary key part will be put in at the end of the create table,
				// so just the integer part is needed here
				return "integer";
			}
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

		/// <summary>
		/// Add a LIMIT N clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">A Query in the form of a SqlString.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query</param>
		/// <param name="offset">Offset of the first row to process in the result set</param>
		/// <returns>A new SqlString that contains the <c>LIMIT</c> clause.</returns>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(querySqlString);
			pagingBuilder.Add(" limit ");
			pagingBuilder.Add(limit.ToString());

			if (offset > 0)
			{
				pagingBuilder.Add(" offset ");
				pagingBuilder.Add(offset.ToString());
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
	}
}