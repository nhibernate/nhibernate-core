using System.Data;
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
			var connection1 = MakeConnection();
			var connection2 = MakeConnection();

			//open first connection
			connection1.Open();
			VerifyCountOfEstablishedConnectionsIs(1);

			//return it to the pool
			connection1.Close();
			VerifyCountOfEstablishedConnectionsIs(1);

			//open the second connection
			connection2.Open();
			VerifyCountOfEstablishedConnectionsIs(1);

			//return it to the pool
			connection2.Close();
			VerifyCountOfEstablishedConnectionsIs(1);
		}

		[Test]
		public void ConnectionPooling_OpenThenCloseTwoAtTheSameTime_TowConnectionsArePooled()
		{
			MakeDriver();
			var connection1 = MakeConnection();
			var connection2 = MakeConnection();

			//open first connection
			connection1.Open();
			VerifyCountOfEstablishedConnectionsIs(1);

			//open second one
			connection2.Open();
			VerifyCountOfEstablishedConnectionsIs(2);

			//return connection1 to the pool
			connection1.Close();
			VerifyCountOfEstablishedConnectionsIs(2);

			//return connection2 to the pool
			connection2.Close();
			VerifyCountOfEstablishedConnectionsIs(2);
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

		private IDbConnection MakeConnection()
		{
			var result = _driver.CreateConnection();
			result.ConnectionString = _connectionString;
			return result;
		}

		private void VerifyCountOfEstablishedConnectionsIs(int expectedCount)
		{
			var physicalConnections = GetEstablishedConnections();
			physicalConnections.Should().Be(expectedCount);
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
					return (int) cmd.ExecuteScalar();
				}
			}
		}

		#endregion
	}
}
