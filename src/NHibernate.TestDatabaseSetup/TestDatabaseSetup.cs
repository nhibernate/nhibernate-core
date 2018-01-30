using System;
using System.Collections.Generic;
#if !NETCOREAPP2_0
using System.Data.Odbc;
using System.Data.SqlServerCe;
using System.Data.SQLite;
#endif
using System.Data.SqlClient;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using NHibernate.Test;
using Npgsql;
using NUnit.Framework;

namespace NHibernate.TestDatabaseSetup
{
	[TestFixture]
	public class DatabaseSetup
	{
		private static readonly IDictionary<string, Action<Cfg.Configuration>> SetupMethods = new Dictionary<string, Action<Cfg.Configuration>>
			{
#if !NETCOREAPP2_0
				{"NHibernate.Driver.OdbcDriver", SetupSqlServerOdbc},
				{"NHibernate.Driver.SQLiteDriver, NHibernate.Driver.SQLite", SetupSQLite},
				{"NHibernate.Driver.OracleDataClientDriver", SetupOracle},
				{"NHibernate.Driver.OracleClientDriver", SetupOracle},
				{"NHibernate.Driver.OracleManagedDriver, NHibernate.Driver.Oracle.Managed", SetupOracle},
				{"NHibernate.Driver.SqlServerCompactDriver, NHibernate.Driver.SqlServer.Compact", SetupSqlServerCe},
#endif
				{"NHibernate.Driver.SqlServer2000Driver, NHibernate.Driver.SqlServer", SetupSqlServer},
				{"NHibernate.Driver.SqlServer2008Driver, NHibernate.Driver.SqlServer", SetupSqlServer},
				{"NHibernate.Driver.FirebirdDriver, NHibernate.Driver.Firebird", SetupFirebird},
				{"NHibernate.Driver.PostgreSqlDriver, NHibernate.Driver.PostgreSql", SetupNpgsql},
				{"NHibernate.Driver.MySqlDriver, NHibernate.Driver.MySql", SetupMySql},
			};

		private static void SetupMySql(Cfg.Configuration obj)
		{
			//TODO: do nothing
		}

		[Test]
		public void SetupDatabase()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var driver = cfg.Properties[Cfg.Environment.ConnectionDriver];

			Assert.That(SetupMethods.ContainsKey(driver), "No setup method found for " + driver);

			var setupMethod = SetupMethods[driver];
			setupMethod(cfg);
		}

		private static void SetupSqlServer(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString];

			using (var conn = new SqlConnection(connStr.Replace("initial catalog=nhibernate", "initial catalog=master")))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "drop database nhibernate";

					try
					{
						cmd.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine(e);
					}

					cmd.CommandText = "create database nhibernate";
					cmd.ExecuteNonQuery();
				}
			}
		}

#if !NETCOREAPP2_0
		private static void SetupSqlServerOdbc(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString];

			using (var conn = new OdbcConnection(connStr.Replace("Database=nhibernateOdbc", "Database=master")))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "drop database nhibernateOdbc";

					try
					{
						cmd.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine(e);
					}

					cmd.CommandText = "create database nhibernateOdbc";
					cmd.ExecuteNonQuery();
				}
			}
		}
#endif

		private static void SetupFirebird(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString];
			try
			{
				FbConnection.DropDatabase(connStr);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			FbConnection.CreateDatabase(connStr, forcedWrites:false);
		}

#if !NETCOREAPP2_0
		private static void SetupSqlServerCe(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString];

			try
			{
				var connStrBuilder = new SqlCeConnectionStringBuilder(connStr);
				var dataSource = connStrBuilder.DataSource;
				if (File.Exists(dataSource))
					File.Delete(dataSource);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			using (var en = new SqlCeEngine(connStr))
			{
				en.CreateDatabase();
			}
		}
#endif

		private static void SetupNpgsql(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString];

			using (var conn = new NpgsqlConnection(connStr.Replace("Database=nhibernate", "Database=postgres")))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "drop database nhibernate";

					try
					{
						cmd.ExecuteNonQuery();
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}

					cmd.CommandText = "create database nhibernate";
					cmd.ExecuteNonQuery();
				}
			}

			// Install the GUID generator function that uses the most common "random" algorithm.
			using (var conn = new NpgsqlConnection(connStr))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText =
						@"CREATE OR REPLACE FUNCTION uuid_generate_v4()
						RETURNS uuid
						AS '$libdir/uuid-ossp', 'uuid_generate_v4'
						VOLATILE STRICT LANGUAGE C;";

					cmd.ExecuteNonQuery();
				}
			}
		}

#if !NETCOREAPP2_0
		private static void SetupSQLite(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString];

			try
			{
				var connStrBuilder = new SQLiteConnectionStringBuilder(connStr);
				var dataSource = connStrBuilder.DataSource;
				if (File.Exists(dataSource))
					File.Delete(dataSource);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private static void SetupOracle(Cfg.Configuration cfg)
		{
			// disabled until system password is set on TeamCity

			//var connStr =
			//    cfg.Properties[Cfg.Environment.ConnectionString]
			//        .Replace("User ID=nhibernate", "User ID=SYSTEM")
			//        .Replace("Password=nhibernate", "Password=password");

			//cfg.DataBaseIntegration(db =>
			//    {
			//        db.ConnectionString = connStr;
			//        db.Dialect<NHibernate.Dialect.Oracle10gDialect>();
			//        db.KeywordsAutoImport = Hbm2DDLKeyWords.None;
			//    });

			//using (var sf = cfg.BuildSessionFactory())
			//{
			//    try
			//    {
			//        using(var s = sf.OpenSession())
			//            s.CreateSQLQuery("drop user nhibernate cascade").ExecuteUpdate();
			//    }
			//    catch {}

			//    using (var s = sf.OpenSession())
			//    {
			//        s.CreateSQLQuery("create user nhibernate identified by nhibernate").ExecuteUpdate();
			//        s.CreateSQLQuery("grant dba to nhibernate with admin option").ExecuteUpdate();
			//    }
			//}
		}
#endif
	}
}
