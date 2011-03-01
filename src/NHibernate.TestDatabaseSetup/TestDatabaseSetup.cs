using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using Npgsql;
using NUnit.Framework;

namespace NHibernate.TestDatabaseSetup
{
    [TestFixture]
    public class DatabaseSetup
    {
		private static IDictionary<string, Action<Cfg.Configuration>> SetupMethods;

		static DatabaseSetup()
		{
			SetupMethods = new Dictionary<string, Action<Cfg.Configuration>>();
			SetupMethods.Add("NHibernate.Driver.SqlClientDriver", SetupSqlServer);
			SetupMethods.Add("NHibernate.Driver.FirebirdClientDriver", SetupFirebird);
			SetupMethods.Add("NHibernate.Driver.SQLite20Driver", SetupNoop);
			SetupMethods.Add("NHibernate.Driver.NpgsqlDriver", SetupNpgsql);
		}

		[Test]
		public void SetupDatabase()
		{
            var cfg = new Cfg.Configuration();
			var driver = cfg.Properties[Cfg.Environment.ConnectionDriver];

			Assert.That(SetupMethods.ContainsKey(driver), "No setup method found for " + driver);

			var setupMethod = SetupMethods[driver];
			setupMethod(cfg);
		}

        private static void SetupSqlServer(Cfg.Configuration cfg)
        {
            var connStr = cfg.Properties[Cfg.Environment.ConnectionString];
			
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                using (var cmd = new System.Data.SqlClient.SqlCommand("use master", conn))
                {
                    cmd.ExecuteNonQuery();

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
		}

		private static void SetupNoop(Cfg.Configuration cfg)
		{
		}
	}
}


