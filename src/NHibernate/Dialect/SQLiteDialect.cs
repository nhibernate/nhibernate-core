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
		public SQLiteDialect()
		{
			Register(DbType.Binary, "BLOB");
			Register(DbType.Byte, "INTEGER");
			Register(DbType.Int16, "INTEGER");
			Register(DbType.Int32, "INTEGER");
			Register(DbType.Int64, "INTEGER");
			Register(DbType.SByte, "INTEGER");
			Register(DbType.UInt16, "INTEGER");
			Register(DbType.UInt32, "INTEGER");
			Register(DbType.UInt64, "INTEGER");
			Register(DbType.Currency, "NUMERIC");
			Register(DbType.Decimal, "NUMERIC");
			Register(DbType.Double, "NUMERIC");
			Register(DbType.Single, "NUMERIC");
			Register(DbType.VarNumeric, "NUMERIC");
			Register(DbType.String, "TEXT");
		}

		public override string IdentitySelectString
		{
			get { return "select last_insert_rowid()"; }
		}
	}
}

