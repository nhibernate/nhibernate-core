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
	}
}