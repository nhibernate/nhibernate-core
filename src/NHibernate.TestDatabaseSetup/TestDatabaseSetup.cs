using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
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
				{"NHibernate.Driver.SqlClientDriver", SetupSqlServer},
				{"NHibernate.Driver.Sql2008ClientDriver", SetupSqlServer},
				{"NHibernate.Driver.OdbcDriver", SetupSqlServerOdbc},
				{"NHibernate.Driver.FirebirdClientDriver", SetupFirebird},
				{"NHibernate.Driver.SQLite20Driver", SetupSQLite},
				{"NHibernate.Driver.NpgsqlDriver", SetupNpgsql},
				{"NHibernate.Driver.OracleDataClientDriver", SetupOracle},
				{"NHibernate.Driver.MySqlDataDriver", SetupMySql},
				{"NHibernate.Driver.OracleClientDriver", SetupOracle},
				{"NHibernate.Driver.OracleManagedDataClientDriver", SetupOracle},
				{"NHibernate.Driver.SqlServerCeDriver", SetupSqlServerCe}
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

		private static void SetupFirebird(Cfg.Configuration cfg)
		{
			try
			{
				if (File.Exists("NHibernate.fdb"))
					File.Delete("NHibernate.fdb");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			FbConnection.CreateDatabase("Database=NHibernate.fdb;ServerType=1");
		}

		private static void SetupSqlServerCe(Cfg.Configuration cfg)
		{
			try
			{
				if (File.Exists("NHibernate.sdf"))
					File.Delete("NHibernate.sdf");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			using (var en = new SqlCeEngine("DataSource=\"NHibernate.sdf\""))
			{
				en.CreateDatabase();
			}
		}

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

		private static void SetupSQLite(Cfg.Configuration cfg)
		{
			if (File.Exists("NHibernate.db"))
				File.Delete("NHibernate.db");
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
	}
}


