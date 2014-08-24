using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2408
{
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.MsSql2000Dialect;
		}

		[Test]
		public void ShouldGenerateCorrectSqlStatement()
		{
			using (var session = OpenSession())
			{
				var query = session.CreateQuery("from Animal a where a.Name = ?");
				query.SetParameter(0, "Prince");

				query.SetLockMode("a", LockMode.Upgrade);

				Assert.DoesNotThrow(() => query.List());
			}
		}
	}
}
