using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2746
{
	public class Fixture: BugTestCase
	{
		[Test]
		public void TestQuery()
		{
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
					.SetFetchMode("Children", NHibernate.FetchMode.Join);

				session.EnableFilter("nameFilter").SetParameter("name", "Another child");

				crit.Executing(c=> c.List<T1>()).NotThrows();
			}
		}
	}
}