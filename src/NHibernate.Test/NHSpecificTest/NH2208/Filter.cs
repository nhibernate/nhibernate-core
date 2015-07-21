using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2208
{
	public class Filter : BugTestCase
	{
		[Test, Ignore("Not fixed yet")]
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
