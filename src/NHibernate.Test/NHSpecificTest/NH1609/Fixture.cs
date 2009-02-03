using System.Collections;
using NHibernate.Criterion;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1609
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Test()
		{
			IDriver driver = sessions.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
			{
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
			}
			using (ISession session = sessions.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					EntityA a1 = CreateEntityA(session);
					EntityA a2 = CreateEntityA(session);
					EntityC c = CreateEntityC(session);
					EntityB b = CreateEntityB(session, a1, c);

					// make sure the created entities are no longer in the session
					session.Clear();

					IMultiCriteria multi = session.CreateMultiCriteria();

					// the first query is a simple select by id on EntityA
					multi.Add(session.CreateCriteria(typeof (EntityA)).Add(Restrictions.Eq("Id", a1.Id)));
					// the second query is also a simple select by id on EntityB
					multi.Add(session.CreateCriteria(typeof (EntityA)).Add(Restrictions.Eq("Id", a2.Id)));
					// the final query selects the first element (using SetFirstResult and SetMaxResults) for each EntityB where B.A.Id = a1.Id and B.C.Id = c.Id
					// the problem is that the paged query uses parameters @p0 and @p1 instead of @p2 and @p3
					multi.Add(
						session.CreateCriteria(typeof (EntityB)).Add(Restrictions.Eq("A.Id", a1.Id)).Add(Restrictions.Eq("C.Id", c.Id)).
							SetFirstResult(0).SetMaxResults(1));

					IList results = multi.List();

					Assert.AreEqual(1, ((IList) results[0]).Count);
					Assert.AreEqual(1, ((IList) results[1]).Count);
					Assert.AreEqual(1, ((IList) results[2]).Count);
				}
			}
		}

		private EntityA CreateEntityA(ISession session)
		{
			var a = new EntityA();
			session.Save(a);
			session.Flush();
			return a;
		}

		private EntityC CreateEntityC(ISession session)
		{
			var c = new EntityC();
			session.Save(c);
			session.Flush();
			return c;
		}

		private EntityB CreateEntityB(ISession session, EntityA a, EntityC c)
		{
			var b = new EntityB {A = a, C = c};
			session.Save(b);
			session.Flush();
			return b;
		}
	}
}