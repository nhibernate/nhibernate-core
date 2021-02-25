using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1833
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.AreStringComparisonsCaseInsensitive;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Id = "Bob", Name = "Bob"};
				session.Save(e1);

				var e2 = new Entity {Id = "Sally", Name = "Sally"};
				session.Save(e2);

				session.Flush();

				var c1 = new Child {ParentName = "Bob", Name = "Max"};
				session.Save(c1);

				var c2 = new Child {ParentName = "sally", Name = "Cindy"};
				session.Save(c2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Child").ExecuteUpdate();
				session.CreateQuery("delete from Entity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		// #1828
		[Test]
		public void GetOnParentWithDifferentCaseCanLoadChildren()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var bob = s.Get<Entity>("bob");
				Assert.That(bob, Is.Not.Null);
				Assert.That(bob.Children, Has.Count.EqualTo(1));
				t.Commit();
			}
		}

		// NH-3833
		[Test]
		public void ParentCanLoadChildrenWithDifferentParentCase()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var sally = s.Get<Entity>("Sally");
				Assert.That(sally, Is.Not.Null);
				Assert.That(sally.Children, Has.Count.EqualTo(1));
				t.Commit();
			}
		}
	}
}
