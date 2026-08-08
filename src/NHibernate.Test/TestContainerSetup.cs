namespace NHibernate.Test
{
	using System;
	using System.Threading.Tasks;
	using DotNet.Testcontainers.Builders;
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
		private const string Db2Image = "icr.io/db2_community/db2:12.1.0.0";
		private const string FirebirdSqlImage = "firebirdsql/firebird:4";
		private const string MariaDbImage = "mariadb:10.10";
		private const string MsSqlImage = "mcr.microsoft.com/mssql/server:2019-latest";
		private const string MySqlImage = "mysql:5.7";
		private const string OracleImage = "gvenzl/oracle-xe:21-slim";
		private const string PostgreSqlImage = "postgres:13";

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
					return new Db2Builder(Db2Image).Build();
				case "firebirdsql":
					// The official image defines no health check to wait on, and resolves the relative
					// database name of the connection string under /tmp unless ISC_INET_SERVER_HOME
					// points at the data directory.
					return new FirebirdSqlBuilder(FirebirdSqlImage)
						.WithEnvironment("ISC_INET_SERVER_HOME", "/var/lib/firebird/data")
						.WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(FirebirdSqlBuilder.FirebirdSqlPort))
						.Build();
				case "mariadb":
					return new MariaDbBuilder(MariaDbImage).Build();
				case "mssql":
					return new MsSqlBuilder(MsSqlImage).Build();
				case "mysql":
					return new MySqlBuilder(MySqlImage).Build();
				case "oracle":
					return new OracleBuilder(OracleImage).Build();
				case "postgresql":
					return new PostgreSqlBuilder(PostgreSqlImage).Build();
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
