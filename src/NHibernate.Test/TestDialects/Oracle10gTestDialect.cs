using System.Runtime.InteropServices;

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

		/// <inheritdoc />
		/// <remarks>Canceling a query hangs under Linux with OracleManagedDataClientDriver 21.6.1.</remarks>
		public override bool SupportsCancelQuery => !RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	}
}
