using System.Collections;
using NHibernate.SqlCommand;
using NHibernate.Test.TypesTest;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3004
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void RemoveUnusedCommandParametersBug_1()
		{
			/* UseNamedPrefixInSql       is true 
			 * UseNamedPrefixInParameter is false
			 * */
			var driver = new TestSqlClientDriver(true, false);

			RunTest(driver);
		}

		[Test]
		public void RemoveUnusedCommandParametersBug_2()
		{
			/* UseNamedPrefixInSql       is true 
			 * UseNamedPrefixInParameter is true
			 * */
			var driver = new TestSqlClientDriver(true, true);

			RunTest(driver);
		}

		[Test]
		public void RemoveUnusedCommandParametersBug_3()
		{			
			var driver = new TestSqlClientDriver(true, true);

			var command = driver.CreateCommand();
			command.CommandText = "SELECT * FROM Test where value = @p0 or value=@p1";

			var usedParam1 = command.CreateParameter();
			usedParam1.ParameterName = driver.FormatNameForParameter("p0");
			command.Parameters.Add(usedParam1);

			var usedParam2 = command.CreateParameter();
			usedParam2.ParameterName = driver.FormatNameForParameter("p1");
			command.Parameters.Add(usedParam2);

			Assert.AreEqual(2, command.Parameters.Count);

			// Alias @p0 and @p1 to be the same thing
			var param = Parameter.GenerateParameters(2);

			foreach (var p in param)
			{
				p.ParameterPosition = 0;
			}

			SqlString sqlString = new SqlStringBuilder()
				.Add(param[0])
				.Add(param[1])
				.ToSqlString();

			driver.RemoveUnusedCommandParameters(command, sqlString);

			Assert.IsFalse(command.CommandText.Contains("p1"));

			Assert.AreEqual(1, command.Parameters.Count);

			Assert.AreEqual(command.Parameters[0], usedParam1);
		}

		private static void RunTest(TestSqlClientDriver driver)
		{
			var command = driver.CreateCommand();

			var usedParam = command.CreateParameter();
			usedParam.ParameterName = driver.FormatNameForParameter("p0");
			command.Parameters.Add(usedParam);

			var unusedParam = command.CreateParameter();
			unusedParam.ParameterName = driver.FormatNameForParameter("unused");
			command.Parameters.Add(unusedParam);

			Assert.AreEqual(2, command.Parameters.Count);

			SqlString sqlString = new SqlStringBuilder()
				.AddParameter()
				.ToSqlString();

			driver.RemoveUnusedCommandParameters(command, sqlString);

			Assert.AreEqual(1, command.Parameters.Count);
			
			Assert.AreEqual(command.Parameters[0], usedParam);
		}
	}
}
