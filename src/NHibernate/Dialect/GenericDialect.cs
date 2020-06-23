using System.Data;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A generic SQL dialect which may or may not work on any actual databases
	/// </summary>
	public class GenericDialect : Dialect
	{
		/// <summary></summary>
		public GenericDialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "VARCHAR($l)");
			RegisterColumnType(DbType.Binary, "VARBINARY(255)");
			RegisterColumnType(DbType.Binary, 8000, "VARBINARY($l)");
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19, 5)");
			RegisterColumnType(DbType.Decimal, 19, "DECIMAL($p, $s)");
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INT");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "REAL");
			RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 4000, "NCHAR($l)");
			RegisterColumnType(DbType.String, "NVARCHAR(255)");
			RegisterColumnType(DbType.String, 4000, "NVARCHAR($l)");
			RegisterColumnType(DbType.Time, "TIME");
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add column"; }
		}
	}
}
