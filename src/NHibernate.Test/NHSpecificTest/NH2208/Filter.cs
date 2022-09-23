using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2208
{
	[TestFixture]
	public class Filter : BugTestCase
	{
		[Test]
		public void TestHql()
		{
			using (ISession session = OpenSession())
			{
				session.EnableFilter("myfilter");
				session.CreateQuery("from E1 e join fetch e.BO").List();
			}
		}

		[Test]
		public void TestQueryOver()
		{
			using (ISession session = OpenSession())
			{
				session.EnableFilter("myfilter");
				session.QueryOver<E1>().JoinQueryOver(x => x.BO).List();
			}
		}
	}
}
