using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1171
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return
				// Firebird has issues with comments containing apostrophes
				!(dialect is FirebirdDialect)
				// GH-2853: Oracle client bug: throws Oracle.ManagedDataAccess.Client.OracleException : ORA-01008: not all variables bound
				&& !(dialect is Oracle8iDialect);
		}

		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "false");
		}

		[Test]
		public void SupportSQLQueryWithComments()
		{
			string sql =
				@"
SELECT id 
FROM tablea 
-- Comment with ' number 1 
WHERE Name = :name 
/* Comment with ' number 2 */ 
ORDER BY Name 
";
			using (ISession s = OpenSession())
			{
				var q = s.CreateSQLQuery(sql);
				q.SetString("name", "Evgeny Potashnik");
				q.List();
			}
		}

		[Test]
		public void ExecutedContainsComments()
		{
			string sql =
				@"
SELECT id 
FROM tablea 
-- Comment with ' number 1 
WHERE Name = :name 
/* Comment with ' number 2 */ 
ORDER BY Name 
";
			using (var ls = new SqlLogSpy())
			{
				using (ISession s = OpenSession())
				{
					var q = s.CreateSQLQuery(sql);
					q.SetString("name", "Evgeny Potashnik");
					q.List();
				}
				string message = ls.GetWholeLog();
				Assert.That(message, Does.Contain("-- Comment with ' number 1"));
				Assert.That(message, Does.Contain("/* Comment with ' number 2 */"));
			}
		}
	}
}
