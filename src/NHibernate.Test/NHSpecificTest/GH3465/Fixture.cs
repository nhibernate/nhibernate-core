using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3465
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ThetaJoinSubQuery()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var query = session.CreateQuery("select e.Id from EntityA e where exists (from e.Children b, EntityC c)");
				Assert.DoesNotThrow(() => query.List());
			}
		}
	}
}
