using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2469
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ShouldNotThrowSqlException()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var criteria = session.CreateCriteria(typeof(Entity2), "e2")
					.CreateAlias("e2.Entity1", "e1")
					.Add(Restrictions.Eq("e1.Foo", 0));

				Assert.AreEqual(0, criteria.List<Entity2>().Count);
			}
		}
	}
}