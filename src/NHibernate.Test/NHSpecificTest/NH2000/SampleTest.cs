using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2000
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		// In this version of nHibernate, GetEnabledFilter throws an exception
		// instead returning nothing like in previous versions.
		[Test]
		public void TestSessionGetEnableFilter()
		{
			using (ISession session = OpenSession())
			{
				IFilter filter = session.GetEnabledFilter("TestFilter");
			}
		}
	}
}
