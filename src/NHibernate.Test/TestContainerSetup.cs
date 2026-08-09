namespace NHibernate.Test
{
	using System;
	using System.Threading.Tasks;
	using DotNet.Testcontainers.Containers;
	using NUnit.Framework;
	using Testcontainers.Db2;
	using Testcontainers.FirebirdSql;
	using Testcontainers.MariaDb;
	using Testcontainers.MsSql;
	using Testcontainers.MySql;
	using Testcontainers.Oracle;
	using Testcontainers.PostgreSql;

	[SetUpFixture]
	public class TestContainerSetup
	{
		private static volatile IDatabaseContainer _container;
		private static readonly object _lock = new object();

		internal static string GetConnectionString(string connectionString)
		{
			var parts = connectionString.Split('=');
			if (parts.Length != 2 || parts[0] != "testcontainers")
			{
				throw new System.ArgumentException("Invalid testcontainers connection string format. Expected format: testcontainers=DbType");
			}
			// For now, only one container is supported. In the future, we can extend this to support multiple containers.
			if (_container == null)
			{
				lock (_lock)
				{
					if (_container == null)
					{
						var container = GetContainer(parts[1]);
						Task.Run(() => container.StartAsync()).GetAwaiter().GetResult();
						_container = container;
					}
				}
			}
			return _container.GetConnectionString();
		}

		private static IDatabaseContainer GetContainer(string dbType)
		{
			switch (dbType.ToLower())
			{
				case "db2":
					return new Db2Builder("icr.io/db2_community/db2:12.1.0.0").Build();
				case "firebirdsql":
					return new FirebirdSqlBuilder("jacobalberty/firebird:v4.0").Build();
				case "mariadb":
					return new MariaDbBuilder("mariadb:10.10").Build();
				case "mssql":
					return new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();
				case "mysql":
					return new MySqlBuilder("mysql:8.0").Build();
				case "oracle":
					return new OracleBuilder("gvenzl/oracle-xe:21.3.0-slim-faststart").Build();
				case "postgresql":
					return new PostgreSqlBuilder("postgres:15.1").Build();
				default:
					throw new NotSupportedException("Database type not supported: " + dbType);
			}
		}

		[OneTimeTearDown]
		public async Task TearDown()
		{
			if (_container != null)
			{
				await _container.DisposeAsync();
			}
		}
	}
}
