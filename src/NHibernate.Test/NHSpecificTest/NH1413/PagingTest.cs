using System;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1413
{
	[TestFixture]
	public class PagingTest : BugTestCase
	{
		[Test]
		public void Bug()
		{
			using(ISession session = OpenSession())
			using(ITransaction t = session.BeginTransaction())
			{
				session.Persist(new Foo("Foo1", DateTime.Today.AddDays(5)));
				session.Persist(new Foo("Foo2", DateTime.Today.AddDays(1)));
				session.Persist(new Foo("Foo3", DateTime.Today.AddDays(3)));
				t.Commit();
			}

			DetachedCriteria criteria = DetachedCriteria.For(typeof (Foo));
			criteria.Add(Restrictions.Like("Name", "Foo", MatchMode.Start));
			criteria.AddOrder(Order.Desc("Name"));
			criteria.AddOrder(Order.Asc("BirthDate"));
			using (ISession session = OpenSession())
			{
				ICriteria icriteria = criteria.GetExecutableCriteria(session);
				icriteria.SetFirstResult(0);
				icriteria.SetMaxResults(2);
				Assert.That(2, Is.EqualTo(icriteria.List<Foo>().Count));
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Delete("from Foo");
				t.Commit();
			}
		}
	}
}