using System.Runtime.InteropServices;

namespace NHibernate.Test.TestDialects
{
	public class MsSql2008TestDialect : TestDialect
	{
		public MsSql2008TestDialect(Dialect.Dialect dialect)
			: base(dialect)
		{
		}

		/// <summary>
		/// Does not support SELECT FOR UPDATE with paging
		/// </summary>
		public override bool SupportsSelectForUpdateWithPaging => false;

		/// <inheritdoc />
		/// <remarks>Canceling a query hangs under Linux with Sql2008ClientDriver. (It may be a data provider bug fixed with MicrosoftDataSqlClientDriver.)</remarks>
		public override bool SupportsCancelQuery => !RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	}
}
