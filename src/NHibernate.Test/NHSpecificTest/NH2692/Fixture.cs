using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2692
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void QueryingParentWhichHasChildren()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Parent>()
									.Where(x => x.ChildComponents.Any())
									.ToList();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test, KnownBug("NH-2692")]
		public void QueryingChildrenComponents()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Parent>()
									.SelectMany(x => x.ChildComponents)
									.ToList();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test, KnownBug("NH-2692")]
		public void QueryingChildrenComponentsHql()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.CreateQuery("select c from Parent as p join p.ChildComponents as c")
									.List<ChildComponent>();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var parent1 = new Parent();
				var child1 = new ChildComponent { Parent = parent1, SomeBool = true, SomeString = "something" };
				parent1.ChildComponents.Add(child1);

				var parent2 = new Parent();

				session.Save(parent1);
				session.Save(parent2);

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
