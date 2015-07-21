using System.Collections;
using NHibernate.SqlCommand;
using NUnit.Framework;
using System.Data;

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

		private static void RunTest(TestSqlClientDriver driver)
		{
			var command = driver.CreateCommand();

			var usedParam = command.CreateParameter();
			usedParam.ParameterName = driver.FormatNameForParameter("p0");
			command.Parameters.Add(usedParam);

			var unusedParam = command.CreateParameter();
			unusedParam.ParameterName = driver.FormatNameForParameter("unused");
			command.Parameters.Add(unusedParam);

			Assert.AreEqual(command.Parameters.Count, 2);

			SqlString sqlString = new SqlStringBuilder()
				.AddParameter()
				.ToSqlString();

			driver.RemoveUnusedCommandParameters(command, sqlString);

			Assert.AreEqual(command.Parameters.Count, 1);
			
			Assert.AreEqual(command.Parameters[0], usedParam);
		}
	}
}
