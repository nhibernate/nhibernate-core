using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1171
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
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
				var q =s.CreateSQLQuery(sql);
				q.SetString("name", "Evgeny Potashnik");
				q.List();
			}
		}
	}
}