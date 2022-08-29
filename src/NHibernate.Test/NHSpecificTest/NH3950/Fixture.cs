using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3950
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
		public void FirstFutureValue()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.OrderBy(e => e.Name)
					.ToFutureValue(q => q.First());

				Assert.AreEqual("Bob", result.Value.Name);
			}
		}

		[Test]
		public void FirstOrDefaultFutureValue()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(e => e.Name)
					.OrderBy(n => n)
					.ToFutureValue(q => q.FirstOrDefault());

				Assert.AreEqual("Bob", result.Value);
			}
		}

		[Test]
		public void SingleFutureValue()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.Name == "Bob")
					.ToFutureValue(q => q.Single());

				Assert.AreEqual("Bob", result.Value.Name);
			}
		}

		[Test]
		public void SingleOrDefaultFutureValue()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(e => e.Name)
					.Where(n => n == "Bob")
					.ToFutureValue(q => q.SingleOrDefault());

				Assert.AreEqual("Bob", result.Value);
			}
		}
	}
}
