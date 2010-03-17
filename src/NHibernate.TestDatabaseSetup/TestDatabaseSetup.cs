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
            var connStr = cfg.Properties["connection.connection_string"];

            connStr = @"Server=.\SQLExpress;initial catalog=nhibernate;Integrated Security=SSPI";

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


