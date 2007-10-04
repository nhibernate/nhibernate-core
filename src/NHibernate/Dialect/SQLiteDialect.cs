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

		/// <summary>
		/// 
		/// </summary>
		public override string GetIdentitySelectString(string identityColumn, string tableName)
		{
			return "select last_insert_rowid()";
		}

		public override SqlString AddIdentitySelectToInsert(SqlString insertSql, string identityColumn, string tableName)
		{
			return insertSql.Append("; " + GetIdentitySelectString(identityColumn, tableName));
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

		// TODO: SQLite actually does support subselects, but gives syntax errors
		// in tests. Need to investigate this.
		public override bool SupportsSubSelects
		{
			get { return false; }
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
}
