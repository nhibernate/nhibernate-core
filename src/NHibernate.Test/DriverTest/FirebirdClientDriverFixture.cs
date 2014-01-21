using NHibernate.Driver;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.DriverTest
{
	[TestFixture]
	public class FirebirdClientDriverFixture
	{
		#region Fields
		private string _connectionString;
		private FirebirdClientDriver _driver;
		#endregion

		#region Tests

		[Test]
		public void ConnectionPooling_OpenThenCloseThenOpenAnotherOne_OnlyOneConnectionIsPooled()
		{
			MakeDriver();
			var connection1 = _driver.CreateConnection();
			var connection2 = _driver.CreateConnection();
			connection1.ConnectionString = _connectionString;
			connection2.ConnectionString = _connectionString;

			connection1.Open();
			//return the connection1 to the pool
			connection1.Close();
			//open the second connection
			connection2.Open();

			var physicalConnections = GetEstablishedConnections();
			physicalConnections.Should().Be(1);
		}

		[Test]
		public void ConnectionPooling_OpenThenCloseTwoAtTheSameTime_TowConnectionsArePooled()
		{
			MakeDriver();
			var connection1 = _driver.CreateConnection();
			var connection2 = _driver.CreateConnection();
			connection1.ConnectionString = _connectionString;
			connection2.ConnectionString = _connectionString;

			connection1.Open();
			connection2.Open();
			//return both to the pool
			connection1.Close();
			connection2.Close();

			var physicalConnections = GetEstablishedConnections();
			physicalConnections.Should().Be(2);
		}

		#endregion

		#region Private Members
		private void MakeDriver()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var dlct = cfg.GetProperty("dialect");
			if (!dlct.Contains("Firebird"))
				Assert.Ignore("Applies only to Firebird");

			_driver = new FirebirdClientDriver();
			_connectionString = cfg.GetProperty("connection.connection_string");
		}

		private int GetEstablishedConnections()
		{
			using (var conn = _driver.CreateConnection())
			{
				conn.ConnectionString = _connectionString;
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select count(*) from mon$attachments where mon$attachment_id <> current_connection";
					return (int)cmd.ExecuteScalar();
				}
			}
		}
		#endregion
	}
}
