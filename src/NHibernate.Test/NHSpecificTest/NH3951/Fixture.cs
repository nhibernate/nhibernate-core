using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3951
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void AllNamedBob()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.All(e => e.Name == "Bob");

				Assert.AreEqual(false, result);
			}
		}

		[Test]
		public void AllNamedWithAtLeast3Char()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.All(e => e.Name.Length > 2);

				Assert.AreEqual(true, result);
			}
		}

		[Test]
		public void AllNamedBobWorkaround()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = !session.Query<Entity>()
					.Any(e => e.Name != "Bob");

				Assert.AreEqual(false, result);
			}
		}

		[Test]
		public void AllNamedWithAtLeast3CharWorkaround()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = !session.Query<Entity>()
					.Any(e => e.Name.Length < 3);

				Assert.AreEqual(true, result);
			}
		}

		[Test]
		public void AnyAndAllInSubQueries()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(e => new { e.Id, hasRelated = e.Related.Any(), allBobRelated = e.Related.All(r => r.Name == "Bob") })
					.ToList();

				Assert.AreEqual(false, result[0].hasRelated);
				Assert.AreEqual(true, result[0].allBobRelated);
			}
		}
	}
}
