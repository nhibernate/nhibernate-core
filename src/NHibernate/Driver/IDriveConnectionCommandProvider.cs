using System;
using System.Data.Common;

namespace NHibernate.Driver
{
	public interface IDriveConnectionCommandProvider
	{
		DbConnection CreateConnection();
		DbCommand CreateCommand();
#if NET6_0_OR_GREATER
		DbBatch CreateBatch() => throw new NotSupportedException();
		bool CanCreateBatch => false;
#endif
	}
}
