using System;
using System.Data;
using NHibernate.SqlCommand;

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
            RegisterColumnType(DbType.String, "TEXT");
            RegisterColumnType(DbType.AnsiStringFixedLength, "TEXT");
            RegisterColumnType(DbType.StringFixedLength, "TEXT");

            RegisterColumnType(DbType.DateTime, "DATETIME");
            RegisterColumnType(DbType.Time, "DATETIME");
            RegisterColumnType(DbType.Boolean, "INTEGER");
            RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
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

        protected override bool SupportsIfExistsBeforeTableName
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
                pagingBuilder.Add(", ");
                pagingBuilder.Add(offset.ToString());
            }

            return pagingBuilder.ToSqlString();
        }
    }
}
