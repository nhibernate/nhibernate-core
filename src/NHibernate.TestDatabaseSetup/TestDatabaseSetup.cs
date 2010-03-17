using System;
using NUnit.Framework;
using System.Data.SqlClient;

namespace NHibernate.TestDatabaseSetup
{
    [TestFixture]
    public class DatabaseSetup
    {
        [Test]
        public void SetupDatabase()
        {
            var cfg = new Cfg.Configuration();

            using (var conn = new SqlConnection(cfg.Properties["connection.connection_string"]))
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
                    catch(Exception)
                    {
                    }

                    cmd.CommandText = "create database nhibernate";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}


