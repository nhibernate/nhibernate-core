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
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR($1)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR($1)");
			RegisterColumnType(DbType.Binary, "VARBINARY($1)");
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19, $1)");
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION");
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INT");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "REAL");
			RegisterColumnType(DbType.StringFixedLength, "NCHAR($1)");
			RegisterColumnType(DbType.String, "NVARCHAR($1)");
			RegisterColumnType(DbType.Time, "TIME");
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add column"; }
		}
	}
}