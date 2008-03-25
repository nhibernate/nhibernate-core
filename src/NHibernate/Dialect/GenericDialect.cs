using System.Data;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A generic SQL dialect which may or may not work on any actual databases
	/// </summary>
	public class GenericDialect : Dialect
	{
		/// <summary></summary>
		public GenericDialect() : base()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR($l)");
			RegisterColumnType(DbType.Binary, "VARBINARY($l)");
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19, $l)");
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INT");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "REAL");
			RegisterColumnType(DbType.StringFixedLength, "NCHAR($l)");
			RegisterColumnType(DbType.String, "NVARCHAR($l)");
			RegisterColumnType(DbType.Time, "TIME");
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add column"; }
		}
	}
}