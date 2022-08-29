using NHibernate.AdoNet.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class SqlStatementLoggerFixture
	{
		[Test]
		public void SupportsInvalidSql()
		{
			using (var spy = new SqlLogSpy())
			{
				var logger = new SqlStatementLogger(false, true);
				using (var cmd = new System.Data.SqlClient.SqlCommand(
					"UPDATE Table Set Column = @p0; @p0 = 'Some data with an embedded quote in parentheses and signal word (''UPDATE'') like this: (don't update)'"))
				{
					Assert.DoesNotThrow(() => logger.LogCommand(cmd, FormatStyle.Basic));
				}

				Assert.That(spy.GetWholeLog(), Contains.Substring("Some data with an embedded quote"));
			}
		}
		[Test]
		public void SupportsKeywordInParameter()
		{
			using (var spy = new SqlLogSpy())
			{
				var logger = new SqlStatementLogger(false, true);
				using (var cmd = new System.Data.SqlClient.SqlCommand("UPDATE Table Set Column = @p0;"))
				{
					cmd.Parameters.AddWithValue(
						"p0",
						"Some data with an embedded quote in parentheses and signal word ('UPDATE') like this: (don't update)");
					Assert.DoesNotThrow(() => logger.LogCommand(cmd, FormatStyle.Basic));
				}

				var log = spy.GetWholeLog();
				Assert.That(log, Contains.Substring("Some data with an embedded quote"));
				Assert.That(log.Split('\n'), Has.Length.GreaterThan(2), "SQL seems not formatted");
			}
		}
	}
}
