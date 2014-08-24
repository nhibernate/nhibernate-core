using System;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2409
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var contest1 = new Contest {Id = 1};
				var contest2 = new Contest {Id = 2};
				var user = new User();

				var message = new Message {Contest = contest2 };

				session.Save(contest1);
				session.Save(contest2);
				session.Save(user);

				session.Save(message);
				tx.Commit();
			}

			using (var session = OpenSession())
			{
				var contest2 = session.CreateCriteria<Contest>().Add(Restrictions.IdEq(2)).UniqueResult<Contest>();
				var user = session.CreateCriteria<User>().List<User>().Single();

				var msgs = session.CreateCriteria<Message>()
					.Add(Restrictions.Eq("Contest", contest2))
					.CreateAlias("Readings", "mr", JoinType.LeftOuterJoin, Restrictions.Eq("mr.User", user))
					.List<Message>();
				
				Assert.AreEqual(1, msgs.Count, "We should be able to find our message despite any left outer joins");
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from Contest");
				session.Delete("from User");
				session.Delete("from Message");
				session.Delete("from MessageReading");
				tx.Commit();
			}
			base.OnTearDown();
		}
	}
}
