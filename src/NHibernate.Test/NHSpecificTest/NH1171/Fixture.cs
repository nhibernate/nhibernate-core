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
			// Firebird has issues with comments containing apostrophes
			return !(dialect is FirebirdDialect);
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
				Assert.That(message, Is.StringContaining("-- Comment with ' number 1"));
				Assert.That(message, Is.StringContaining("/* Comment with ' number 2 */"));
			}
		}
	}
}