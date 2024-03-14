using System;
using System.Data.Common;

namespace NHibernate.Driver
{
	public class DbProviderFactoryDriveConnectionCommandProvider : IDriveConnectionCommandProvider
#if NET6_0_OR_GREATER
		, IDriveConnectionCommandProviderWithBatchSupport
#endif
	{
		private readonly DbProviderFactory dbProviderFactory;

		public DbProviderFactoryDriveConnectionCommandProvider(DbProviderFactory dbProviderFactory)
		{
			if (dbProviderFactory == null)
			{
				throw new ArgumentNullException("dbProviderFactory");
			}
			this.dbProviderFactory = dbProviderFactory;
		}

		public DbConnection CreateConnection()
		{
			return dbProviderFactory.CreateConnection();
		}

		public DbCommand CreateCommand()
		{
			return dbProviderFactory.CreateCommand();
		}
#if NET6_0_OR_GREATER
		public DbBatch CreateBatch()
		{
			return dbProviderFactory.CreateBatch();
		}

		public bool CanCreateBatch => dbProviderFactory.CanCreateBatch && dbProviderFactory.CreateCommand() is ICloneable;
#endif
	}
}
