using System;
using NUnit.Framework;
using Sql=System.Data.SqlClient;

namespace NHibernate.TeamCity.TestDatabaseSetup
{
    [TestFixture]
    public class DatabaseSetup
    {
        [Test]
        public void SetupDatabase()
        {
            var cfg = new Cfg.Configuration();

            using (var conn = new Sql.SqlConnection(cfg.Properties["connection.connection_string"]))
            {
                conn.Open();

                using (var cmd = new Sql.SqlCommand("use master", conn))
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
