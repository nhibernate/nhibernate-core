using System.Data;
using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.DriverTest
{
	[TestFixture]
	public class FirebirdClientDriverFixture
	{
		private string _connectionString;
		private FirebirdClientDriver _driver;

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

		[Test]
		public void AdjustCommand_StringParametersWithinConditionalSelect_ThenParameterIsWrappedByAVarcharCastStatement()
		{
			MakeDriver();
			var cmd = BuildSelectCaseCommand(SqlTypeFactory.GetString(0));

			_driver.AdjustCommand(cmd);

			var expectedCommandTxt = "select (case when col = @p0 then cast(@p1 as VARCHAR(255)) else cast(@p2 as VARCHAR(255)) end) from table";
			cmd.CommandText.Should().Be(expectedCommandTxt);
		}

		[Test]
		public void AdjustCommand_IntParametersWithinConditionalSelect_ThenParameterIsWrappedByAnIntCastStatement()
		{
			MakeDriver();
			var cmd = BuildSelectCaseCommand(SqlTypeFactory.Int32);

			_driver.AdjustCommand(cmd);

			var expectedCommandTxt = "select (case when col = @p0 then cast(@p1 as INTEGER) else cast(@p2 as INTEGER) end) from table";
			cmd.CommandText.Should().Be(expectedCommandTxt);
		}

		[Test]
		public void AdjustCommand_ParameterWithinSelectConcat_ParameterIsCasted()
		{
			MakeDriver();
			var cmd = BuildSelectConcatCommand(SqlTypeFactory.GetString(0));

			_driver.AdjustCommand(cmd);

			var expected = "select col || cast(@p0 as VARCHAR(255)) || col from table";
			cmd.CommandText.Should().Be(expected);
		}

		[Test]
		public void AdjustCommand_ParameterWithinSelectAddFunction_ParameterIsCasted()
		{
			MakeDriver();
			var cmd = BuildSelectAddCommand(SqlTypeFactory.GetString(0));

			_driver.AdjustCommand(cmd);

			var expected = "select col + cast(@p0 as VARCHAR(255)) from table";
			cmd.CommandText.Should().Be(expected);
		}

		[Test]
		public void AdjustCommand_InsertWithParamsInSelect_ParameterIsCasted()
		{
			MakeDriver();
			var cmd = BuildInsertWithParamsInSelectCommand(SqlTypeFactory.Int32);

			_driver.AdjustCommand(cmd);

			var expected = "insert into table1 (col1, col2) select col1, cast(@p0 as INTEGER) from table2";
			cmd.CommandText.Should().Be(expected);
		}

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
					return (int)cmd.ExecuteScalar();
				}
			}
		}

		private IDbCommand BuildSelectCaseCommand(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
					.Add("select (case when col = ")
					.AddParameter()
					.Add(" then ")
					.AddParameter()
					.Add(" else ")
					.AddParameter()
					.Add(" end) from table")
					.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new SqlType[] { paramType, paramType, paramType });
		}

		private IDbCommand BuildSelectConcatCommand(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
					.Add("select col || ")
					.AddParameter()
					.Add(" || ")
					.Add("col ")
					.Add("from table")
					.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new SqlType[] { paramType });
		}

		private IDbCommand BuildSelectAddCommand(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
					.Add("select col + ")
					.AddParameter()
					.Add(" from table")
					.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new SqlType[] { paramType });
		}

		private IDbCommand BuildInsertWithParamsInSelectCommand(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
				.Add("insert into table1 (col1, col2) ")
				.Add("select col1, ")
				.AddParameter()
				.Add(" from table2")
				.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new SqlType[] { paramType });
		}

	}
}
