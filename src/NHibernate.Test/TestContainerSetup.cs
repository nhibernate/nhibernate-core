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
					return new Db2Builder().Build();
				case "firebirdsql":
					return new FirebirdSqlBuilder().Build();
				case "mariadb":
					return new MariaDbBuilder().Build();
				case "mssql":
					return new MsSqlBuilder().Build();
				case "mysql":
					return new MySqlBuilder().Build();
				case "oracle":
					return new OracleBuilder().Build();
				case "postgresql":
					return new PostgreSqlBuilder().Build();
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
