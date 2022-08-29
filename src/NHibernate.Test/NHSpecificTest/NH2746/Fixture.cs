using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2746
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void TestQuery()
		{
			if (!Dialect.SupportsSubSelectsWithPagingAsInPredicateRhs)
				Assert.Ignore("Current dialect does not support paging within IN sub-queries");

			using (ISession session = OpenSession())
			{
				DetachedCriteria page = DetachedCriteria.For<T1>()
					.SetFirstResult(3)
					.SetMaxResults(7)
					.AddOrder(NHibernate.Criterion.Order.Asc(Projections.Id()))
					.SetProjection(Projections.Id());

				ICriteria crit = session.CreateCriteria<T1>()
					.Add(Subqueries.PropertyIn("id", page))
					.SetResultTransformer(new DistinctRootEntityResultTransformer())
					.Fetch("Children");

				session.EnableFilter("nameFilter").SetParameter("name", "Another child");

				Assert.That(() => crit.List<T1>(), Throws.Nothing);
			}
		}
	}
}
