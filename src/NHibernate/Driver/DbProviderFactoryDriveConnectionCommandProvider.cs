using System;
using System.Data.Common;

namespace NHibernate.Driver
{
	public class DbProviderFactoryDriveConnectionCommandProvider : IDriveConnectionCommandProvider
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
		public DbBatch CreateBatch() => dbProviderFactory.CreateBatch();

		public bool CanCreateBatch => dbProviderFactory.CanCreateBatch && dbProviderFactory.CreateCommand().CreateParameter() is ICloneable;
#endif
	}
}
