using System;
using System.Data;

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
		}

		/// <summary>
		/// 
		/// </summary>
		public override string IdentitySelectString
		{
			get { return "select last_insert_rowid()"; }
		}
	}
}

