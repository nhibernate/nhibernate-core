using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2692
{
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
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
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Parent");
					tx.Commit();
				}
			}
		}

		[Test]
		public void Querying_ParentWhichHasChilds_ShouldWork_With_Any()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var result = session.Query<Parent>()
					   .Where(x => x.ChildComponents.Any())
					   .ToList();

					Assert.AreEqual(1, result.Count);
				}
			}
		}

		[Test]
		public void Querying_ParentWhichHasChilds_ShouldWorkWith_All()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var result = session.Query<Parent>()
					   .Where(x => x.ChildComponents.All(y=>y.SomeString=="something"))
					   .ToList();

					Assert.AreEqual(2, result.Count);
				}
			}
		}
	}
}