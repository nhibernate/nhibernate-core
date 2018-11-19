﻿using System;
using System.Data;
using System.Data.Common;
using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	[TestFixture]
	public class FirebirdClientDriverFixture
	{
		private string _connectionString;
		private FirebirdClientDriver _driver;
		private FirebirdClientDriver _driverWithoutCasting;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();

			var dlct = cfg.GetProperty("dialect");
			if (!dlct.Contains("Firebird"))
				Assert.Ignore("Applies only to Firebird");

			_driver = new FirebirdClientDriver();
			_driver.Configure(cfg.Properties);
			_connectionString = cfg.GetProperty("connection.connection_string");

			_driverWithoutCasting = new FirebirdClientDriver();
			cfg.SetProperty(Cfg.Environment.FirebirdDisableParameterCasting, "true");
			_driverWithoutCasting.Configure(cfg.Properties);
		}

		[Test]
		public void ConnectionPooling_OpenThenCloseThenOpenAnotherOne_OnlyOneConnectionIsPooled()
		{
			_driver.ClearPool(_connectionString);

			var allreadyEstablished = GetEstablishedConnections();

			using (var connection1 = MakeConnection())
			using (var connection2 = MakeConnection())
			{
				//open first connection
				connection1.Open();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 1, "After first open");

				//return it to the pool
				connection1.Close();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 1, "After first close");

				//open the second connection
				connection2.Open();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 1, "After second open");

				//return it to the pool
				connection2.Close();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 1, "After second close");
			}
		}

		[Test]
		public void ConnectionPooling_OpenThenCloseTwoAtTheSameTime_TowConnectionsArePooled()
		{
			_driver.ClearPool(_connectionString);

			var allreadyEstablished = GetEstablishedConnections();

			using (var connection1 = MakeConnection())
			using (var connection2 = MakeConnection())
			{
				//open first connection
				connection1.Open();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 1, "After first open");

				//open second one
				connection2.Open();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 2, "After second open");

				//return connection1 to the pool
				connection1.Close();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 2, "After first close");

				//return connection2 to the pool
				connection2.Close();
				VerifyCountOfEstablishedConnectionsIs(allreadyEstablished + 2, "After second close");
			}
		}

		[Test]
		public void AdjustCommand_StringParametersWithinConditionalSelect_ThenParameterIsWrappedByAVarcharCastStatement()
		{
			using (var cmd = BuildSelectCaseCommand(SqlTypeFactory.GetString(255)))
			{
				_driver.AdjustCommand(cmd);

				var expectedCommandTxt =
					"select (case when col = @p0 then cast(@p1 as VARCHAR(4000)) else cast(@p2 as VARCHAR(4000)) end) from table";
				Assert.That(cmd.CommandText, Is.EqualTo(expectedCommandTxt));
			}
		}

		[Test]
		public void AdjustCommand_IntParametersWithinConditionalSelect_ThenParameterIsWrappedByAnIntCastStatement()
		{
			using (var cmd = BuildSelectCaseCommand(SqlTypeFactory.Int32))
			{
				_driver.AdjustCommand(cmd);

				var expectedCommandTxt =
					"select (case when col = @p0 then cast(@p1 as INTEGER) else cast(@p2 as INTEGER) end) from table";
				Assert.That(cmd.CommandText, Is.EqualTo(expectedCommandTxt));
			}
		}

		[Test]
		public void AdjustCommand_ParameterWithinSelectConcat_ParameterIsCasted()
		{
			using (var cmd = BuildSelectConcatCommand(SqlTypeFactory.GetString(255)))
			{
				_driver.AdjustCommand(cmd);

				var expected = "select col || cast(@p0 as VARCHAR(4000)) || col from table";
				Assert.That(cmd.CommandText, Is.EqualTo(expected));
			}
		}

		[Test]
		public void AdjustCommand_ParameterWithinSelectAddFunction_ParameterIsCasted()
		{
			using (var cmd = BuildSelectAddCommand(SqlTypeFactory.GetString(255)))
			{
				_driver.AdjustCommand(cmd);

				var expected = "select col + cast(@p0 as VARCHAR(4000)) from table";
				Assert.That(cmd.CommandText, Is.EqualTo(expected));
			}
		}

		[Test]
		public void AdjustCommand_InsertWithParamsInSelect_ParameterIsCasted()
		{
			using (var cmd = BuildInsertWithParamsInSelectCommand(SqlTypeFactory.Int32))
			{
				_driver.AdjustCommand(cmd);

				var expected = "insert into table1 (col1, col2) select col1, cast(@p0 as INTEGER) from table2";
				Assert.That(cmd.CommandText, Is.EqualTo(expected));
			}
		}

		[Test]
		public void AdjustCommand_InsertWithParamsInSelect_ParameterIsNotCasted_WhenColumnNameContainsSelect()
		{
			using (var cmd = BuildInsertWithParamsInSelectCommandWithSelectInColumnName(SqlTypeFactory.Int32))
			{
				_driver.AdjustCommand(cmd);

				var expected = "insert into table1 (col1_select_aaa, select_aaa, col1_select) values(@p0, @p1, @p2) from table2";
				Assert.That(cmd.CommandText, Is.EqualTo(expected));
			}
		}

		[Test]
		public void AdjustCommand_InsertWithParamsInSelect_ParameterIsNotCasted_WhenColumnNameContainsWhere()
		{
			using (var cmd = BuildInsertWithParamsInSelectCommandWithWhereInColumnName(SqlTypeFactory.Int32))
			{
				_driver.AdjustCommand(cmd);

				var expected = "insert into table1 (col1_where_aaa, where_aaa, col1_where) values(@p0, @p1, @p2) from table2";
				Assert.That(cmd.CommandText, Is.EqualTo(expected));
			}
		}

		[Test]
		public void AdjustCommand_ParameterWithinBetween_ParameterIsNotCasted()
		{
			using (var cmd = BuildBetweenCommand(SqlTypeFactory.GetString(255)))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		[Test]
		public void AdjustCommand_ParameterWithinPaging_ParameterIsNotCasted()
		{
			using (var cmd = BuildPagingCommand(SqlTypeFactory.Int32))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		[Test]
		public void AdjustCommand_ParameterWithinIn_ParameterIsNotCasted()
		{
			using (var cmd = BuildInCommand(SqlTypeFactory.GetString(255)))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		[Test]
		public void AdjustCommand_StringParametersWithinConditionalSelect_NotCastedWhenDisabled()
		{
			using (var cmd = BuildSelectCaseCommand(SqlTypeFactory.GetString(255)))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		[Test]
		public void AdjustCommand_IntParametersWithinConditionalSelect_NotCastedWhenDisabled()
		{
			using (var cmd = BuildSelectCaseCommand(SqlTypeFactory.Int32))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		[Test]
		public void AdjustCommand_ParameterWithinSelectConcat_NotCastedWhenDisabled()
		{
			using (var cmd = BuildSelectConcatCommand(SqlTypeFactory.GetString(255)))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		[Test]
		public void AdjustCommand_ParameterWithinSelectAddFunction_NotCastedWhenDisabled()
		{
			using (var cmd = BuildSelectAddCommand(SqlTypeFactory.GetString(255)))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		[Test]
		public void AdjustCommand_InsertWithParamsInSelect_NotCastedWhenDisabled()
		{
			using (var cmd = BuildInsertWithParamsInSelectCommand(SqlTypeFactory.Int32))
			{
				var originalSql = cmd.CommandText;
				_driver.AdjustCommand(cmd);

				Assert.That(cmd.CommandText, Is.EqualTo(originalSql));
			}
		}

		private DbConnection MakeConnection()
		{
			var result = _driver.CreateConnection();
			result.ConnectionString = _connectionString;
			return result;
		}

		private void VerifyCountOfEstablishedConnectionsIs(int expectedCount, string step)
		{
			var physicalConnections = GetEstablishedConnections();
			Assert.That(physicalConnections, Is.EqualTo(expectedCount), step);
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
					return Convert.ToInt32(cmd.ExecuteScalar());
				}
			}
		}

		private DbCommand BuildSelectCaseCommand(SqlType paramType)
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

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType, paramType, paramType });
		}

		private DbCommand BuildSelectConcatCommand(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
				.Add("select col || ")
				.AddParameter()
				.Add(" || ")
				.Add("col ")
				.Add("from table")
				.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType });
		}

		private DbCommand BuildSelectAddCommand(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
				.Add("select col + ")
				.AddParameter()
				.Add(" from table")
				.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType });
		}

		private DbCommand BuildInsertWithParamsInSelectCommand(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
				.Add("insert into table1 (col1, col2) ")
				.Add("select col1, ")
				.AddParameter()
				.Add(" from table2")
				.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType });
		}

		private DbCommand BuildInsertWithParamsInSelectCommandWithSelectInColumnName(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
				.Add("insert into table1 (col1_select_aaa, select_aaa, col1_select) ")
				.Add("values(")
				.AddParameter()
				.Add(", ")
				.AddParameter()
				.Add(", ")
				.AddParameter()
				.Add(") from table2")
				.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType, paramType, paramType });
		}

		private DbCommand BuildInsertWithParamsInSelectCommandWithWhereInColumnName(SqlType paramType)
		{
			var sqlString = new SqlStringBuilder()
				.Add("insert into table1 (col1_where_aaa, where_aaa, col1_where) ")
				.Add("values(")
				.AddParameter()
				.Add(", ")
				.AddParameter()
				.Add(", ")
				.AddParameter()
				.Add(") from table2")
				.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType, paramType, paramType });
		}

		private DbCommand BuildBetweenCommand(SqlType paramType)
		{
			var sqlString =
				new SqlStringBuilder()
					.Add("select col1 from table where col2 between ")
					.AddParameter()
					.Add(" and ")
					.AddParameter()
					.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType, paramType });
		}

		private DbCommand BuildPagingCommand(SqlType paramType)
		{
			var sqlString =
				new SqlStringBuilder()
					.Add("select first ")
					.AddParameter()
					.Add(" skip ")
					.AddParameter()
					.Add(" col1 from table")
					.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType, paramType });
		}

		private DbCommand BuildInCommand(SqlType paramType)
		{
			var sqlString =
				new SqlStringBuilder()
					.Add("select col1 from table where col2 in (")
					.AddParameter()
					.Add(", ")
					.AddParameter()
					.Add(", ")
					.AddParameter()
					.Add(")")
					.ToSqlString();

			return _driver.GenerateCommand(CommandType.Text, sqlString, new[] { paramType, paramType });
		}
	}
}
