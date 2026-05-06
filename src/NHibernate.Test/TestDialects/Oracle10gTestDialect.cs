using System.Data;
using System.Runtime.InteropServices;
using NHibernate.SqlTypes;

namespace NHibernate.Test.TestDialects
{
	public class Oracle10gTestDialect : TestDialect
	{
		public Oracle10gTestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		/// <summary>
		/// Does not support SELECT FOR UPDATE with paging
		/// </summary>
		public override bool SupportsSelectForUpdateWithPaging => false;

		public override bool SupportsAggregateInSubSelect => true;

		public override bool SupportsSqlType(SqlType sqlType)
		{
			// The Oracle dialects define types for DbType the Oracle driver does not support.
			return sqlType.DbType switch
			{
				DbType.UInt16 or DbType.UInt32 or DbType.UInt64 => false,
				_ => base.SupportsSqlType(sqlType)
			};
		}

		/// <inheritdoc />
		/// <remarks>Canceling a query hangs under Linux with OracleManagedDataClientDriver 21.6.1.</remarks>
		public override bool SupportsCancelQuery => !RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	}
}
