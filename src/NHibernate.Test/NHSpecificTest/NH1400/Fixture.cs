using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1400
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void DotInStringLiteralsConstant()
		{
			using (ISession s = OpenSession())
			{
				Assert.DoesNotThrow(() => s.CreateQuery("from SimpleGeographicalAddress as dga where dga.Line2 = 'B1 P9, Scb, Ap. 18'").List());
			}
		}
	}
}