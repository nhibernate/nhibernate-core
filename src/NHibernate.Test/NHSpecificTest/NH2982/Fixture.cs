using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH2982
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Id = 1, Name = "A" };
				session.Save(e1);
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

			base.OnTearDown();
		}

		[Test]
		public void SimpleExpressionWithProxy()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var a = session.Load<Entity>(1);
				var restriction = Restrictions.Eq("A", a);
				Assert.AreEqual("A = Entity#1", restriction.ToString());
			}
		}

		[Test]
		public void SimpleExpressionWithNewInstance()
		{
			var a = new Entity() { Id = 2, Name = "2" };
			var restriction = Restrictions.Eq("A", a);
			Assert.AreEqual(@"A = Entity@" + a.GetHashCode() + "(hash)", restriction.ToString());
		}

		[Test]
		public void SimpleExpressionWithNull()
		{
			var restriction = Restrictions.Eq("A", null);
			Assert.AreEqual("A = null", restriction.ToString());
		}

		[Test]
		public void SimpleExpressionWithPrimitive()
		{
			var restriction = Restrictions.Eq("A", 5);
			Assert.AreEqual("A = 5", restriction.ToString());
		}
	}
}
