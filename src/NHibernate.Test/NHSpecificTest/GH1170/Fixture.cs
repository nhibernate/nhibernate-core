using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1170
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		// Only the set case is tested, because other cases were not affected:
		// - bags delete everything first.
		// - indexed collections use their index, which is currently not mappable as a composite index with nullable
		//   column. All index columns are forced to not-nullable by mapping implementation. When using a formula in
		//   index, they use the element, but its columns are also forced to not-nullable.

		[Test]
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

		[Test]
		public void UpdateComponentWithNull()
		{
			// Updates on set are indeed handled as delete/insert, so this test is not really needed.
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var parent = session.Query<Parent>().Single();
				Assert.That(
					parent.ChildComponents,
					Has.Count.EqualTo(2).And.One.Property(nameof(ChildComponent.SomeString)).Null);
				parent.ChildComponents.Single(c => c.SomeString == null).SomeString = "no more null";
				tx.Commit();
			}

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var parent = session.Query<Parent>().Single();
				Assert.That(
					parent.ChildComponents,
					Has.Count.EqualTo(2).And.None.Property(nameof(ChildComponent.SomeString)).Null);
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
