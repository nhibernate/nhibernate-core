using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2208
{
	[TestFixture]
	public class Filter : BugTestCase
	{
		[Test]
		public void Test()
		{
			using (ISession session = OpenSession())
			{
				session.EnableFilter("myfilter");
				session.CreateQuery("from E1 e join fetch e.BO").List();
			}
		}
	}
}
