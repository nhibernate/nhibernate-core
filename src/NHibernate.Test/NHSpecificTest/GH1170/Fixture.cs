using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1170
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test, KnownBug("GH1170")]
		public void DeleteComponentWithNull()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var parent = session.Query<Parent>().Single();
				Assert.That(
					parent.ChildComponents,
					Has.Count.EqualTo(2).And.One.Property(nameof(ChildComponent.SomeString)).Null);
				parent.ChildComponents.Remove(parent.ChildComponents.Single(c => c.SomeString == null));
				tx.Commit();
			}

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var parent = session.Query<Parent>().Single();
				Assert.That(
					parent.ChildComponents,
					Has.Count.EqualTo(1).And.None.Property(nameof(ChildComponent.SomeString)).Null);
				tx.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var parent = new Parent();
				parent.ChildComponents.Add(new ChildComponent { SomeBool = true, SomeString = "something" });
				parent.ChildComponents.Add(new ChildComponent { SomeBool = false, SomeString = null });
				session.Save(parent);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from Parent");
				tx.Commit();
			}
		}
	}
}
